Shader "RetroLookPro/Legacy/First_RetroLook" {
	Properties {
		_MainTex ("Render Input", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }

		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#include "UnityCG.cginc"
				#pragma glsl
				sampler2D _MainTex;
				sampler2D _TapeTex;
			
				struct appdata{
				   float4 vertex : POSITION;
				   float4 texcoord : TEXCOORD0;   	
				   float4 texcoord2 : TEXCOORD3;   	
				};

				struct v2f {
			   	float4 uvn : TEXCOORD1; 
			   	float2 uv : TEXCOORD2; 
			   	float4 pos : SV_POSITION; 
				};

				v2f vert (appdata i){
				   v2f o;
				   o.pos = UnityObjectToClipPos( i.vertex );
				   o.uv = o.pos.xy/o.pos.w;
				   o.uvn = float4( i.texcoord.xy, 0.0, 0.0);
				   return o;
				}
				float screenLinesNum = 240.0;
				
				#pragma shader_feature VHS_FISHEYE_ON
	   		half cutoffX = 2.0;
	   		half cutoffY = 3.0;			   		
	   		half cutoffFadeX = 100.0;
	   		half cutoffFadeY = 100.0;

				float noiseLinesNum = 240.0;
				float noiseQuantizeX = 1.0; 
				#pragma shader_feature VHS_FILMGRAIN_ON
				#pragma shader_feature VHS_LINENOISE_ON
				#pragma shader_feature VHS_TAPENOISE_ON
				float tapeNoiseAmount = 1.0;

				#pragma shader_feature VHS_YIQNOISE_ON
				float signalNoisePower = 1.0f;
				float signalNoiseAmount = 1.0f;

				#pragma shader_feature VHS_LINESFLOAT_ON
				float linesFloatSpeed = 1.0;
				#pragma shader_feature VHS_SCANLINES_ON
				float scanLineWidth = 10.0;				
				#pragma shader_feature VHS_STRETCH_ON

				#pragma shader_feature VHS_TWITCH_H_ON
				float twitchHFreq = 1.0;
				#pragma shader_feature VHS_TWITCH_V_ON
				float twitchVFreq = 1.0; 
				#pragma shader_feature VHS_JITTER_H_ON
				float jitterHAmount = 0.5; 

				#pragma shader_feature VHS_JITTER_V_ON
				float jitterVAmount = 1.0; 
				float jitterVSpeed = 1.0;			

				//float4 _MainTex_TexelSize;
				//float _ChromaticAberration;

				#define PI 3.14159265359
				float time_ = 0.0;
				float SLN = 0.0; 
				float SLN_Noise = 0.0; 
				float ONE_X = 0.0; 
				float ONE_Y = 0.0; 

				half3 bms(half3 c1, half3 c2){ return 1.0- (1.0-c1)*(1.0-c2); }

				float onOff(float a, float b, float c, float t){
					return step(c, sin(t + a*cos(t*b)));
				}

				

				#if VHS_YIQNOISE_ON

					#define MOD3 float3(443.8975,397.2973, 491.1871)

					float2 hash22(float2 p) {
						float3 p3 = frac(float3(p.xyx) * MOD3);
					   p3 += dot(p3.zxy, p3.yzx+19.19);
					   return frac(float2((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y));
					}

					float2 n4rand_bw( float2 p, float t, float c ){
					    
						t = frac( t );
						float2 nrnd0 = hash22( p + 0.07*t );
					   c = 1.0 / (10.0*c); 
					   nrnd0 = pow(nrnd0, c);				    
						return nrnd0;
					}
				#endif

				#if VHS_JITTER_V_ON
					float rnd_rd(float2 co){
					     float a = 12.9898;
					     float b = 78.233;
					     float c = 43758.5453;
					     float dt= dot(co.xy ,float2(a,b));
					     float sn= fmod(dt,3.14);
					    return frac(sin(sn) * c);
					}
				#endif

				half3 rgb2yiq(half3 c){   
					return half3(
						(0.2989*c.x + 0.5959*c.y + 0.2115*c.z),
						(0.5870*c.x - 0.2744*c.y - 0.5229*c.z),
						(0.1140*c.x - 0.3216*c.y + 0.3114*c.z)
					);
				};

				half3 yiq2rgb(half3 c){				
					return half3(
						(	 1.0*c.x +	  1.0*c.y + 	1.0*c.z),
						( 0.956*c.x - 0.2720*c.y - 1.1060*c.z),
						(0.6210*c.x - 0.6474*c.y + 1.7046*c.z)
					);
				};
				#if VHS_SCANLINES_ON
	    			float scanLines(float2 p, float t){
				   	float t_sl = 0.0;					   	
				   	#if VHS_LINESFLOAT_ON
				   		t_sl = t*linesFloatSpeed;
				   	#endif			        	
			        	float scans = 0.5*(cos( (p.y*screenLinesNum+t_sl)*2.0*PI) + 1.0);
			        	scans = pow(scans, scanLineWidth); 
			        	scans = 1.0 - scans;
			        	return scans; 
	    			}				
    			#endif

    			#if VHS_STRETCH_ON
					float gcos(float2 uv, float s, float p){
					    return (cos( uv.y * PI * 2.0 * s + p)+1.0)*0.5;
					}
					float2 stretch(float2 uv, float t, float mw, float wcs, float lfs, float lfp){						    
					   float tt = t*wcs;
					   float t2 = tt-fmod(tt, 0.5);
					   
					   float w = gcos(uv, 2.0*(1.0-frac(t2)), PI-t2) * clamp( gcos(uv, frac(t2), t2) , 0.5, 1.0);
					   w = floor(w*mw)/mw;
					   w *= mw;
					   float ln = (1.0-frac(t*lfs + lfp)) *screenLinesNum; 
					   ln = ln - frac(ln); 
					   
					   float oy = 1.0/SLN; 
					  	float md = fmod(ln, w); 
					   float sh2 =  1.0-md/w; 					    
						float slb = SLN / w; 						   
						   if(uv.y<oy*ln && uv.y>oy*(ln-w)) 
						      uv.y = floor( uv.y*slb +sh2 )/slb - (sh2-1.0)/slb ;
				      

						return uv;
					}
				#endif	

				#if VHS_JITTER_V_ON
					float3 yiqDist(float2 uv, float m, float t){
					
					m *= 0.0001; 
						float3 offsetX = float3( uv.x, uv.x, uv.x );	

						offsetX.r += rnd_rd(float2(t*0.03, uv.y*0.42)) * 0.001 + sin(rnd_rd(float2(t*0.2, uv.y)))*m;
						offsetX.g += rnd_rd(float2(t*0.004,uv.y*0.002)) * 0.004 + sin(t*9.0)*m;
					    
					   half3 signal = half3(0.0, 0.0, 0.0);
					   signal.x = rgb2yiq( tex2D( _MainTex, float2(offsetX.r, uv.y) ).rgb ).x;
					   signal.y = rgb2yiq( tex2D( _MainTex, float2(offsetX.g, uv.y) ).rgb ).y;
					   signal.z = rgb2yiq( tex2D( _MainTex, float2(offsetX.b, uv.y) ).rgb ).z;
						return signal;					    
					}
				#endif
				#if VHS_TWITCH_V_ON	
	    			float2 twitchVertical(float freq, float2 uv, float t){

					   float vShift = 0.4*onOff(freq,3.0,0.9, t);
					   vShift*=(sin(t)*sin(t*20.0) + (0.5 + 0.1*sin(t*200.0)*cos(t)));
						uv.y = fmod(uv.y + vShift, 1.0); 
						return uv;
					}
    			#endif
    			#if VHS_TWITCH_H_ON	

	    			float2 twitchHorizonal(float freq, float2 uv, float t){
	    				
						float window = 1.0/(1.0+80.0*(uv.y-fmod(t/4.0,1.0))*(uv.y-fmod(t/4.0, 1.0)));
					   	uv.x += sin(uv.y*10.0 + t)/50.0
					   		*onOff(freq,4.0,0.3, t)
					   		*(1.0+cos(t*80.0))
					   		*window;
					   
					   return uv;
	    			}

				#endif

				half4 frag( v2f i ) : COLOR {

					float t = time_;			
					float2 p = i.uvn;



					if(screenLinesNum==0.0) screenLinesNum = _ScreenParams.y;
					SLN = screenLinesNum; 
					SLN_Noise = noiseLinesNum; 

					if(SLN_Noise==0 || SLN_Noise>SLN) SLN_Noise = SLN;									
					
					ONE_X = 1.0/_ScreenParams.x; 
					ONE_Y = 1.0/_ScreenParams.y; 					
					

					#if VHS_TWITCH_V_ON			
						p = twitchVertical(0.5*twitchVFreq, p, t); 
					#endif	

					#if VHS_TWITCH_H_ON
						p = twitchHorizonal(0.1*twitchHFreq, p, t);
					#endif	
				   
				   #if VHS_LINESFLOAT_ON
				   	float sh = frac(-t*linesFloatSpeed); 
				    	p.y = -floor( -p.y * SLN + sh )/SLN + sh/SLN;  
				   #else 
				    	p.y = -floor( -p.y * SLN )/SLN; 
				   #endif
					
					#if VHS_STRETCH_ON
					   p = stretch(p, t, 15.0, 1.0, 0.5, 0.0);
					   p = stretch(p, t, 8.0, 1.2, 0.45, 0.5);
					   p = stretch(p, t, 11.0, 0.5, -0.35, 0.25); 
					#endif

					#if VHS_JITTER_H_ON
				    	if( fmod( p.y * SLN, 2.0)<1.0) 
				    		p.x += ONE_X*sin(t*13000.0)*jitterHAmount;
					#endif
			   	half3 col = half3(0.0,0.0,0.0);
			   	half3 signal = half3(0.0,0.0,0.0);
			   	float2 pn = p;
			   	if(SLN!=SLN_Noise){
					   #if VHS_LINESFLOAT_ON
					   	float sh = frac(t); 
					    	pn.y = floor( pn.y * SLN_Noise + sh )/SLN_Noise - sh/SLN_Noise;
					   #else 
					    	pn.y = floor( pn.y * SLN_Noise )/SLN_Noise;
					   #endif				 
				   }  	

					float ScreenLinesNumX = SLN_Noise * _ScreenParams.x / _ScreenParams.y;
					float SLN_X = noiseQuantizeX*(_ScreenParams.x - ScreenLinesNumX) + ScreenLinesNumX;
					pn.x = floor( pn.x * SLN_X )/SLN_X;

					float2 pn_ = pn*_ScreenParams.xy;

					float ONEXN = 1.0/SLN_X;

					#if VHS_TAPENOISE_ON
						int distWidth = 20; 
						float distAmount = 4.0;
						float distThreshold = 0.55;
						float distShift = 0; 
						for (int ii = 0; ii < distWidth % 1023; ii++){

							float tnl = tex2Dlod(_TapeTex, float4(0.0,pn.y-ONEXN*ii, 0.0, 0.0)).y;
							if(tnl>distThreshold) {							
								float sh = sin( 1.0*PI*(float(ii)/float(distWidth))) ;							
								p.x -= float(int(sh)*distAmount*ONEXN); 
								distShift += sh ; 
							}

						}

					#endif	
					#if VHS_JITTER_V_ON						
				    	signal = yiqDist(p, jitterVAmount, t*jitterVSpeed);
			   	#else
						col = tex2D(_MainTex, p).rgb;
						signal = rgb2yiq(col);
			   	#endif


			   	#if VHS_LINENOISE_ON || VHS_FILMGRAIN_ON
			   		signal.x += tex2D(_TapeTex, pn).z;
			   	#endif
					   
				   #if VHS_YIQNOISE_ON

					   float2 noise = n4rand_bw( pn_,t, 1.0-signalNoisePower ) ; 
					   signal.y += (noise.x*2.0-1.0)*signalNoiseAmount*signal.x;
					   signal.z += (noise.y*2.0-1.0)*signalNoiseAmount*signal.x;
					#endif


				   #if VHS_TAPENOISE_ON
					
					half tn = tex2D(_TapeTex, pn).x;
						signal.x = bms(signal.x, tn*tapeNoiseAmount );  
						int tailLength=10; 

						for(int j = 0; j < tailLength % 1023; j++){

							float jj = float(j);
							float2 d = float2(pn.x-ONEXN*jj,pn.y);
							tn = tex2Dlod(_TapeTex, float4(d,0.0,0.0) ).x;
							float fadediff = tex2D(_TapeTex, d).a; 

							if( tn > 0.8 ){								
								float nsx =  0.0; 
								float newlength = float(tailLength)*(1.0-fadediff); 
								if( jj <= newlength ) nsx = 1.0-( jj/ newlength ); 
								signal.x = bms(signal.x, nsx*tapeNoiseAmount);									
							}
						}

						if(distShift>0.4){
							float tnl = tex2D(_TapeTex, pn).y;
						   signal.y *= 1.0/distShift;
						   signal.z *= 1.0/distShift;
						}
				   #endif

					col = yiq2rgb(signal);
				   #if VHS_SCANLINES_ON
						col *= scanLines(i.uvn, t); 						
				   #endif
			   	#if VHS_FISHEYE_ON

			   		half cof_x = cutoffFadeX;
			   		half cof_y = cutoffFadeY;
			   		p = i.uvn;
			   		half far;
						half2 hco = half2(ONE_X*cutoffX, ONE_Y*cutoffY); 
						half2 sco = half2(ONE_X*cutoffFadeX, ONE_Y*cutoffFadeY); 

			   		if( p.x<=(0.0+hco.x) || p.x>=(1.0-hco.x) ||
			   			 p.y<=(0.0+hco.y) || p.y>=(1.0-hco.y) ){

			   			col = half3(0.0,0.0,0.0);
						}
						else
						{
							if( 
								(p.x>(0.0+hco.x) 			 && p.x<(0.0+(sco.x+hco.x) )) ||
								(p.x>(1.0-(sco.x+hco.x)) && p.x<(1.0-hco.x)) 
							){								
								if(p.x<0.5)	far = (0.0-hco.x+p.x)/(sco.x);									
								else			far = (1.0-hco.x-p.x)/(sco.x);
								
								col *= half(far).xxx;
							}; 

							if( 
								(p.y>(0.0+hco.y) 			 && p.y<(0.0+(sco.y+hco.y) )) ||
								(p.y>(1.0-(sco.y+hco.y)) && p.y<(1.0-hco.y)) 
							){
								if(p.y<0.5)	far = (0.0-hco.y+p.y)/(sco.y);									
								else			far = (1.0-hco.y-p.y)/(sco.y);
								
								col *= half(far).xxx;
							}
						}
					#endif


				
				//col += color;
				//float exp = 1.0;
        		//return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
					return half4(col, 1.0); 
				}
			ENDCG
		}
	}
}

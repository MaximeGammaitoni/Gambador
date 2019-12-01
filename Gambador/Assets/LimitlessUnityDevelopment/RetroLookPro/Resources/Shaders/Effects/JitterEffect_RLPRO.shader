Shader "RetroLookPro/JitterEffect" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		sampler2D _MainTex;
		float screenLinesNum = 240.0;
				
				float noiseLinesNum = 240.0;
				float noiseQuantizeX = 1.0; 
		
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


				#define PI 3.14159265359
				float time_ = 0.0;
				float SLN = 0.0; 
				float SLN_Noise = 0.0; 
				float ONE_X = 0.0; 
				float ONE_Y = 0.0; 

				float onOff(float a, float b, float c, float t){
					return step(c, sin(t + a*cos(t*b)));
				}

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

        float4 Frag(VaryingsDefault i) : SV_Target
        {
			float t = time_;			
			float2 p = i.texcoordStereo.xy;

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

					float ScreenLinesNumX = SLN_Noise * _ScreenParams.x / _ScreenParams.y;
					float SLN_X = noiseQuantizeX*(_ScreenParams.x - ScreenLinesNumX) + ScreenLinesNumX;
					pn.x = floor( pn.x * SLN_X )/SLN_X;

					float2 pn_ = pn*_ScreenParams.xy;

					float ONEXN = 1.0/SLN_X;

					#if VHS_JITTER_V_ON						
				    	signal = yiqDist(p, jitterVAmount, t*jitterVSpeed);
			   	#else
						col = tex2D(_MainTex, p).rgb;
						signal = rgb2yiq(col);
			   	#endif

					col = yiq2rgb(signal);

					return half4(col, 1.0); 
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
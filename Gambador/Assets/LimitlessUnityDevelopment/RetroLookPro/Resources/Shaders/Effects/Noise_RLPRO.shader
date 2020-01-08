Shader "RetroLookPro/Noise" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
sampler2D _MainTex;
				sampler2D _TapeTex;
				float screenLinesNum = 240.0;

				float noiseLinesNum = 240.0;
				float noiseQuantizeX = 1.0; 
				#pragma shader_feature VHS_FILMGRAIN_ON
				#pragma shader_feature VHS_LINENOISE_ON
				#pragma shader_feature VHS_TAPENOISE_ON
				float tapeNoiseAmount = 1.0;

				#pragma shader_feature VHS_YIQNOISE_ON
				float signalNoisePower = 1.0f;
				float signalNoiseAmount = 1.0f;

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

						col = tex2D(_MainTex, p).rgb;
						signal = rgb2yiq(col);

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
Shader "RetroLookPro/Legacy/Tape_RetroLook" {
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
				
			
				struct appdata{
				   float4 vertex : POSITION;
				   float4 texcoord : TEXCOORD0;   	
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
				
				#pragma shader_feature VHS_TAPENOISE_ON
				float tapeNoiseTH = 0.7; 
				float tapeNoiseAmount = 1.0; 
				float tapeNoiseSpeed = 1.0; 

				#pragma shader_feature VHS_FILMGRAIN_ON
				float filmGrainAmount = 16.0;
				float filmGrainPower = 10.0;
				
				#pragma shader_feature VHS_LINENOISE_ON
				float lineNoiseAmount = 1.0; 
				float lineNoiseSpeed = 5.0; 
							
							
				#define PI 3.14159265359

				float time_ = 0.0;

				#define MOD3 float3(443.8975,397.2973, 491.1871)

				float hash12(float2 p){
					float3 p3  = frac(float3(p.xyx) * MOD3);
				    p3 += dot(p3, p3.yzx + 19.19);
				    return frac(p3.x * p3.z * p3.y);
				}

				float2 hash22(float2 p) {
					float3 p3 = frac(float3(p.xyx) * MOD3);
				   p3 += dot(p3.zxy, p3.yzx+19.19);
				   return frac(float2((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y));
				}


				float hash( float n ){ return frac(sin(n)*43758.5453123); }

				float niq( in float3 x ){
				    float3 p = floor(x);
				    float3 f = frac(x);
				    f = f*f*(3.0-2.0*f);
				    float n = p.x + p.y*57.0 + 113.0*p.z;
				    float res = lerp(lerp(	lerp( hash(n+  0.0), hash(n+  1.0),f.x),
				                        	lerp( hash(n+ 57.0), hash(n+ 58.0),f.x),f.y),
				                    	lerp( lerp( hash(n+113.0), hash(n+114.0),f.x),
				                        	lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
				    return res;
				}




				float tapeNoiseLines(float2 p, float t){

				   float y = p.y*_ScreenParams.y;
				   float s = t*2.0;
				   return  	(niq( float3(y*0.01 +s, 			1.0, 1.0) ) + 0.0)
				          	*(niq( float3(y*0.011+1000.0+s,	1.0, 1.0) ) + 0.0) 
				          	*(niq( float3(y*0.51+421.0+s, 	1.0, 1.0) ) + 0.0)   
				        ;


				}

				float tapeNoise(float nl, float2 p, float t){

				   float nm = 	hash12( frac(p+t*float2(0.234,0.637)) ) 
									;						
					nm = nm*nm*nm*nm +0.3; 
				   nl*= nm; 

					if(nl<tapeNoiseTH) nl = 0.0; else nl =1.0;  
				   return nl;
				}

				#if VHS_LINENOISE_ON

					float rnd_rd(float2 co){
					     float a = 12.9898;
					     float b = 78.233;
					     float c = 43758.5453;
					     float dt= dot(co.xy ,float2(a,b));
					     float sn= fmod(dt,3.14);
					    return frac(sin(sn) * c);
					}

					float rndln(float2 p, float t){
						float sample = rnd_rd(float2(1.0,2.0*cos(t))*t*8.0 + p*1.0).x;
						sample *= sample;
						return sample;
					}
					float lineNoise(float2 p, float t){
					   
						float n = rndln(p* float2(0.5,1.0) + float2(1.0,3.0), t)*20.0;
						
					   float freq = abs(sin(t));  
						float c = n*smoothstep(fmod(p.y*4.0 + t/2.0+sin(t + sin(t*0.63)),freq), 0.0,0.95);

					   return c;
					}
				#endif

				#if VHS_FILMGRAIN_ON
					float filmGrain(float2 uv, float t, float c ){ 
						
						float nr = hash12( uv + 0.07*frac( t ) );
						return nr*nr*nr;
					}	
				#endif
				
				half4 frag( v2f i ) : COLOR {

					float t = time_;
					float2 p = i.uvn; 

					#if UNITY_UV_STARTS_AT_TOP 
						p.y = 1-p.y; 
					#endif

					float2 p_ = p*_ScreenParams;

					float ns = 0.0; 
					float nt = 0.0; 
					float nl =0.0; 
					float ntail =0.0; 

					#if VHS_TAPENOISE_ON

						nl = tapeNoiseLines(p, t*tapeNoiseSpeed)*1.0;
						nt = tapeNoise(nl, p, t*tapeNoiseSpeed)*1.0;
						ntail = hash12(p+ float2(0.01,0.02) );
				   #endif
				   #if VHS_LINENOISE_ON
						ns += lineNoise(p_, t*lineNoiseSpeed)*lineNoiseAmount;						
				   #endif

				   #if VHS_FILMGRAIN_ON	
				   	float bg = filmGrain((p_-0.5*_ScreenParams.xy)*0.5, t, filmGrainPower );
				   	ns += bg * filmGrainAmount;
				   #endif

					return half4(nt,nl,ns,ntail);
				}
			ENDCG
		}
	}
}

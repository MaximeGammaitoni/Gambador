Shader "RetroLookPro/Fisheye" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
			sampler2D _MainTex;
			#pragma shader_feature VHS_FISHEYE_ON
	   		half cutoffX = 2.0;
	   		half cutoffY = 3.0;			   		
	   		half cutoffFadeX = 100.0;
	   		half cutoffFadeY = 100.0;	
			#pragma shader_feature VHS_FISHEYE_HYPERSPACE
			float fisheyeSize = 1.2; 
			float fisheyeBend = 2.0; 

			#define PI 3.14159265359
			float time_ = 0.0;
			float ONE_X = 0.0; 
			float ONE_Y = 0.0; 

struct Attributes1
{
    float3 vertex : POSITION;
	float4 texcoord : TEXCOORD0;   	
	float4 texcoord2 : TEXCOORD1;   	
	float4 texcoord3 : TEXCOORD2;   
};

struct Varyings1
{
    float4 vertex : SV_POSITION;
    float2 texcoord : TEXCOORD0;
    float2 texcoordStereo : TEXCOORD1;
	float2 uv : TEXCOORD2; 
#if STEREO_INSTANCING_ENABLED
    uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
#endif
};


        Varyings1 VertDef(Attributes1 v)
        {
            Varyings1 o;
            o.vertex = float4(v.vertex.xy, 0.0, 1.0);
            o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);
			o.uv = o.vertex.xy/o.vertex.w;
            #if UNITY_UV_STARTS_AT_TOP
            o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
            #endif
			o.texcoordStereo = float4( v.texcoord.xy, 0, 0 );
			return o;
        }


				#define fixCoord (p - float2( 0.5 * ONE_X, 0.0)) 
				#define fetch_offset(offset, one_x) t2d(fixCoord + float2( (offset) * (ONE_X), 0.0));
				half3 yiq2rgb(half3 c){				
					return half3(
						(	 1.0*c.x +	  1.0*c.y + 	1.0*c.z),
						( 0.956*c.x - 0.2720*c.y - 1.1060*c.z),
						(0.6210*c.x - 0.6474*c.y + 1.7046*c.z)
					);
				};

				half3 rgb2yiq(half3 c){   
					return half3(
						(0.2989*c.x + 0.5959*c.y + 0.2115*c.z),
						(0.5870*c.x - 0.2744*c.y - 0.5229*c.z),
						(0.1140*c.x - 0.3216*c.y + 0.3114*c.z)
					);
				};
					float2 fishEye(float2 uv, float size, float bend){

						#if !VHS_FISHEYE_HYPERSPACE
							uv -= float2(0.5,0.5);
							uv *= size*(1.0/size+bend*uv.x*uv.x*uv.y*uv.y);
							uv += float2(0.5,0.5);
							
						#endif 
						#if VHS_FISHEYE_HYPERSPACE
							float mx = bend/50.0;
							float2 p = (uv*_ScreenParams.xy) /_ScreenParams.x ;
							float prop = _ScreenParams.x / _ScreenParams.y;
							float2 m = float2(0.5, 0.5 / prop);	
							float2 d = p - m;	
							float r = sqrt(dot(d, d));
							float bind;

							float power = ( 2.0 * 3.141592 / (2.0 * sqrt(dot(m, m))) ) *
											(mx - 0.5); 

							if (power > 0.0) bind = sqrt(dot(m, m));
							else {if (prop < 1.0) bind = m.x; else bind = m.x;}

							if (power > 0.0) 
								uv = m + normalize(d) * tan(r * power) * bind / tan( bind * power);
							else if (power < 0.0) 
								uv = m + normalize(d) * atan(r * -power * 10.0) * bind / atan(-power * bind * 10.0);
							else uv = p; 
							uv.y *=  prop;
						#endif 
						return uv;
					}		
					#define get_t2d(offset, one_x) tex2D(_MainTex, p + (offset)*(one_x)).rgb;
									half3 t2d(float2 p){
				   half3 col = tex2D (_MainTex, p ).rgb;
					return rgb2yiq( col );
				}	

        float4 Frag(Varyings1 i) : SV_Target
        {
			i.texcoordStereo = float4( i.texcoord.xy, 0, 0 );
			float t = time_;			
					float2 p = i.texcoordStereo;								
					
					ONE_X = 1.0/_ScreenParams.x; 
					ONE_Y = 1.0/_ScreenParams.y; 					
					
				p = fishEye(p, fisheyeSize, fisheyeBend); 
			   	half3 col = half3(0.0,0.0,0.0);
			   	half3 signal = half3(0.0,0.0,0.0);			   
				col = tex2D(_MainTex, p).rgb;			   	

			   		half cof_x = cutoffFadeX;
			   		half cof_y = cutoffFadeY;					

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
					return half4(col, 1.0); 
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDef
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
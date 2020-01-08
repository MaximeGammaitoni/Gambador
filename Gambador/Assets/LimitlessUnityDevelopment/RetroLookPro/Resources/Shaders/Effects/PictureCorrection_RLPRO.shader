Shader "RetroLookPro/PictureCorrection" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        sampler2D _MainTex;
				float signalAdjustY = 0.0; 
				float signalAdjustI = 0.0; 
				float signalAdjustQ = 0.0; 

				float signalShiftY = 0.0; 
				float signalShiftI = 0.0; 
				float signalShiftQ = 0.0; 
				float gammaCorection = 1.0; 
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

				half3 t2d(float2 p){
				   half3 col = tex2D (_MainTex, p ).rgb;
					return rgb2yiq( col );
				}

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            half3 signal = half3(0.0,0.0,0.0);
            float2 p = i.texcoord.xy;
            signal = t2d(p);
					    signal.x += signalAdjustY; 
					   signal.y += signalAdjustI; 
					   signal.z += signalAdjustQ; 
					   signal.x *= signalShiftY; 
					   signal.y *= signalShiftI; 
					   signal.z *= signalShiftQ; 
            
            float3 rgb = yiq2rgb(signal);
                        if(gammaCorection!=1.0) rgb = pow(rgb, gammaCorection); 

            return half4(rgb, 1.0);
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
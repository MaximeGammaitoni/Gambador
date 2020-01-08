Shader "RetroLookPro/PulsatingVignette" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        sampler2D _MainTex;
        float vignetteAmount = 	1.0; 
		float vignetteSpeed = 	1.0; 
        float time_ = 0.0;
		float vignette(float2 uv, float t)
        {
			float vigAmt = 2.5+0.1*sin(t + 5.0*cos(t*5.0));
			float c = (1.0-vigAmt*(uv.y-0.5)*(uv.y-0.5))*(1.0-vigAmt*(uv.x-0.5)*(uv.x-0.5));
			c = pow(c, vignetteAmount); 
			return c;
		}

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float t = _Time;			
					float2 p = i.texcoordStereo.xy;
           float3 rgb = tex2D(_MainTex, p).rgb;	
            rgb *= vignette(p, t*vignetteSpeed); 
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
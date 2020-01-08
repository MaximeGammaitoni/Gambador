Shader "RetroLookPro/NegativeFilterRetroLook" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            uniform sampler2D _MainTex;
            uniform sampler2D _MainTex2;
            uniform float T;
            uniform float Luminosity;
            uniform float Vignette;
            uniform float Negative;
            uniform float2 _MainTex_TexelSize;
            half4 _MainTex_ST;

            float3 linearLight( float3 s, float3 d )
            {
                return 2.0 * s + d - 1.0 * Luminosity;
            }


        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float2 uvst = UnityStereoScreenSpaceUVAdjust(i.texcoord, _MainTex_ST);
            float2 uv = uvst.xy;
            float t = float(int(T * 15.0));
            float2 suv = uv ;
            float3 col = tex2D(_MainTex,suv).rgb;
            col=lerp(col,1-col,Negative);
            #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                    uv = 1-uv;
            #endif
            suv = uv ;
            uv.y=suv.y;
            float3 oldfilm = tex2D(_MainTex2,uv).rgb;
            uv = uvst.xy;
            col*=pow(16.0 * uv.x * (1.0-uv.x) * uv.y * (1.0-uv.y), 0.4)*1+Vignette;
            col = dot( float3(0.2126, 0.7152, 0.0722), col);
            col=linearLight(oldfilm,col);
            return float4(col, 1.0);
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
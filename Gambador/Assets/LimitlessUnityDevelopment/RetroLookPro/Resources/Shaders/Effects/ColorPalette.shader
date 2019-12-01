
Shader "RetroLookPro/ColorPalette" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

       sampler2D _MainTex;
       sampler3D _Colormap;
       float4 _Colormap_TexelSize;
       sampler2D _Palette;
       sampler2D _BlueNoise;
       float4 _BlueNoise_TexelSize;
       float _Opacity;
       float _Dither;
       #define unity_ColorSpaceLuminance half4(0.0396819152, 0.458021790, 0.00609653955, 1.0) 
       inline half Luminance(half3 rgb)
        {
            return dot(rgb, unity_ColorSpaceLuminance.rgb);
        }


        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 inputColor = tex2D(_MainTex, i.texcoord);
            inputColor = saturate(inputColor);
            int colorsteps = _Colormap_TexelSize.z;
            float4 colorInColormap = tex3D(_Colormap, inputColor.rgb);

            float random = tex2D(_BlueNoise, i.vertex.xy / _BlueNoise_TexelSize.z).r;
            random = saturate(random);

            if (Luminance(colorInColormap.r) > Luminance(colorInColormap.g))
            {
                random = 1 - random;
            }

            float paletteIndex;
            float blend = colorInColormap.b;
            float threshold = saturate((1 / _Dither) * (blend - 0.5 + (_Dither / 2)));

            if (random < threshold)
            {
                paletteIndex = colorInColormap.g;
            }
            else
            {
                paletteIndex = colorInColormap.r;
            }

            float4 result = tex2D(_Palette, float2(paletteIndex, 0));

            result = lerp(inputColor, result, _Opacity);

            return result;
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
Shader "RetroLookPro/Legacy/Glitch2RetroLook" 
{
    Properties
    {
        _MainTex  ("-", 2D) = "" {}
        _NoiseTex ("-", 2D) = "" {}
        _TrashTex ("-", 2D) = "" {}
    }
    SubShader
    {
        Pass
        {
        ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 3.0

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _NoiseTex;
    sampler2D _TrashTex;
    float _Intensity;

    float4 frag(v2f_img i) : SV_Target 
    {
        float4 glitch = tex2D(_NoiseTex, i.uv);

        float thresh = 1.001 - _Intensity * 1.001;
        float w_d = step(thresh, pow(glitch.z, 2.5)); // displacement glitch
        float w_f = step(thresh, pow(glitch.w, 2.5)); // frame glitch
        float w_c = step(thresh, pow(glitch.z, 3.5)); // color glitch
        float2 uv = frac(i.uv + glitch.xy * w_d);
        float4 source = tex2D(_MainTex, uv);
        float3 color = lerp(source, tex2D(_TrashTex, uv), w_f).rgb;
        float3 neg = saturate(color.grb + (1 - dot(color, 1)) * 0.1);
        color = lerp(color, neg, w_c);

        return float4(color, source.a);
    }
        ENDCG
        }
    }
}
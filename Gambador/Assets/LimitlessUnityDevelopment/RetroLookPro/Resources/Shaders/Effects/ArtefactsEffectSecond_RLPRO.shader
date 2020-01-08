Shader "RetroLookPro/ArtefactsEffectSecond" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
				
		sampler2D _MainTex;
		sampler2D _FeedbackTex;
		float feedbackAmp = 1.0;
		half3 bm_screen(half3 a, half3 b){ 	return 1.0- (1.0-a)*(1.0-b); }

        float4 Frag(VaryingsDefault i) : SV_Target
        {
			float2 p = i.texcoordStereo;
			half3 col = tex2D( _MainTex, i.texcoordStereo).rgb; 		
			half3 fbb = tex2D( _FeedbackTex, i.texcoordStereo).rgb; 
			col = bm_screen(col, fbb*feedbackAmp);
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
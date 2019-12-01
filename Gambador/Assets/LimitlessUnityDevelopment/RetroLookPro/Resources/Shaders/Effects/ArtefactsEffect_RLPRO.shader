Shader "RetroLookPro/ArtefactsEffect" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		sampler2D _MainTex;
		sampler2D _LastTex;
		sampler2D _FeedbackTex;		
		float feedbackAmount = 0.0;
		float feedbackFade = 0.0;
		float feedbackThresh = 5.0;
		half3 feedbackColor = half3(1.0,0.5,0.0); //

		half3 bm_screen(half3 a, half3 b){ 	return 1.0- (1.0-a)*(1.0-b); }

        float4 Frag(VaryingsDefault i) : SV_Target
        {
			float2 p = i.texcoordStereo.xy;
			float one_x = 1.0/_ScreenParams.x;
					
			half3 fc =  tex2D( _MainTex, i.texcoordStereo).rgb; 		
			half3 fl =  tex2D( _LastTex, i.texcoordStereo).rgb; 		
			float diff = abs(fl.x-fc.x + fl.y-fc.y + fl.z-fc.z)/3.0; 
			if(diff<feedbackThresh) diff = 0.0;

			half3 fbn = fc*diff*feedbackAmount; 			
							
			half3 fbb = half3(0.0, 0.0, 0.0); 
								
			fbb = ( 
					tex2D( _FeedbackTex, i.texcoordStereo).rgb + 
					tex2D( _FeedbackTex, i.texcoordStereo + float2(one_x, 0.0)).rgb + 
					tex2D( _FeedbackTex, i.texcoordStereo - float2(one_x, 0.0)).rgb
				  ) / 3.0;
			fbb *= feedbackFade;
			fbn = bm_screen(fbn, fbb); 
			return half4(fbn*feedbackColor, 1.0); 
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
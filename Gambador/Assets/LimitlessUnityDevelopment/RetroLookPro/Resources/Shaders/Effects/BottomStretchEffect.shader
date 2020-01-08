Shader "RetroLookPro/BottomStretchEffect" 
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

		sampler2D _MainTex;
        sampler2D _SecondaryTex;
		float amplitude;
        float frequency;
		half _NoiseBottomHeight;
        
		float onOff(float a, float b, float c, float t)
        {
			return step(c, sin(t + a*cos(t*b)));
		}
	    float2 twitchHorizonalRand(float amp,float freq, float2 uv, float t)
        {
	        float window = 1.0/(1.0+150.0*(uv.y-fmod(t/freq,0.1))*(uv.y-fmod(t/freq, 0.1)));
			uv.x += sin(uv.y*amp + t)/40.0
			*onOff(2.1,4.0,0.3, t)
			*(150.0+cos(t*80.0))
			*window;
			uv.x+= 20*_NoiseBottomHeight;
			return uv;
	    }
	    float2 twitchHorizonal(float amp,float freq, float2 uv, float t)
        {	    				
			float window = 1.0/(1.0+150.0*(uv.y-fmod(t/freq,0.1))*(uv.y-fmod(t/freq, 0.1)));
		   	uv.x += sin(uv.y*amp + t)/40.0
			*(150.0+cos(t*80.0))
			*window;
            uv.x+= 20*_NoiseBottomHeight;
			return uv;
	    }
        float4 FragDist(VaryingsDefault i) : SV_Target
        {
            half2 uv = i.texcoordStereo;
			uv.y = max(uv.y, twitchHorizonal(amplitude,frequency,uv,_Time*100.0)*(_NoiseBottomHeight/20));
			half4 color = tex2D(_MainTex, uv);
    		float exp = 1.0;
       		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
        }
        float4 FragDistRand(VaryingsDefault i) : SV_Target
        {
            half2 uv = i.texcoordStereo;
			uv.y = max(uv.y, twitchHorizonalRand(amplitude,frequency,uv,_Time*100.0)*(_NoiseBottomHeight/20));
			half4 color = tex2D(_MainTex, uv);
    		float exp = 1.0;
       		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
        }
        float4 Frag(VaryingsDefault i) : SV_Target
        {
            half2 uv = i.texcoordStereo;
			uv.y = max(uv.y, (_NoiseBottomHeight/2)-0.01);
			half4 color = tex2D(_MainTex, uv);
    		float exp = 1.0;
       		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragDist

            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragDistRand

            ENDHLSL
        }
                Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
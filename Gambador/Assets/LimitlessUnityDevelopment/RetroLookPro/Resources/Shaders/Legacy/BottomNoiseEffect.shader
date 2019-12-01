Shader "RetroLookPro/Legacy/BottomNoiseEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SecondaryTex ("Secondary Texture", 2D) = "white" {}
		_OffsetNoiseX ("Offset Noise X", float) = 0.0
		_OffsetNoiseY ("Offset Noise Y", float) = 0.0
		_NoiseBottomHeight ("Bottom Noise Height", float) = 0.04
		_NoiseBottomIntensity ("Bottom Noise Intensity", float) = 1.0
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma shader_feature NOISE_BOTTOM_ON
			#pragma shader_feature BOTTOM_STRETCH_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			half _OffsetNoiseX;
			half _OffsetNoiseY;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			o.uv2 = v.texcoord + float2(_OffsetNoiseX - 0.2f, _OffsetNoiseY);
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _SecondaryTex;
			float _ChromaticAberration;
			half _OffsetPosY;
			half _NoiseBottomHeight;
			half _NoiseBottomIntensity;
			
			fixed4 frag (v2f i) : SV_Target
			{
				half2 uv = i.uv;
				// BOTTOM STRETCH
				#if BOTTOM_STRETCH_ON
					float uvY = uv.y;
					uv.y = max(uv.y, _NoiseBottomHeight - 0.01);
				#endif

				half4 color = tex2D(_MainTex, uv);

				// BOTTOM STRETCH CONTD.
				#if BOTTOM_STRETCH_ON
					uv.y = uvY;
				#endif

				// BOTTOM NOISE
				#if NOISE_BOTTOM_ON
					fixed condition = saturate(floor(_NoiseBottomHeight / uv.y));
					fixed4 noise_bottom = tex2D(_SecondaryTex, i.uv2 - 0.5) * condition * _NoiseBottomIntensity;
					color = lerp(color, noise_bottom, - noise_bottom * ((uv.y / (_NoiseBottomHeight)) - 1.0));
				#endif

				float exp = 1.0 ;
        		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
			}
			ENDCG
		}
	}
}

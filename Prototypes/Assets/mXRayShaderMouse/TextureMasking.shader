Shader "Custom/TextureMasking" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaskTex ("Texture", 2D) = "white" {}
		_Alpha ("Alpha", Range(0,1)) = 0
	}
	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True"
		"RenderType"="Transparent"}
		ZWrite Off
		ZTest Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform sampler2D _MaskTex;
			uniform float4 _MainTex_ST;
			uniform float4 _MaskTex_ST;
			float _Alpha;
			
			struct app2vert
			{
				float4 position: POSITION;
				float2 texcoord: TEXCOORD0;
			};
			struct vert2frag
			{
				float4 position: POSITION;
				float2 texcoord: TEXCOORD0;
			};
			vert2frag vert(app2vert input)
			{
				vert2frag output;
				output.position = mul(UNITY_MATRIX_MVP, input.position);
				output.texcoord = TRANSFORM_TEX(input.texcoord, _MainTex);
				return output;
			}
			fixed4 frag(vert2frag input) : COLOR
			{
				fixed4 main_color = tex2D(_MainTex, input.texcoord);
				fixed4 mask_color = tex2D(_MaskTex, input.texcoord);
				float alpha = main_color.a * ( mask_color.a + _Alpha);
				return fixed4(main_color.r, main_color.g, main_color.b, alpha);
			}
			ENDCG
		}
	}
}
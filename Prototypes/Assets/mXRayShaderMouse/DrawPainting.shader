
///////////////////////////////////////////	Draw from maskTexture //////////////////////////////////////////
Shader "Custom/DrawPainting" 
{ 
	Properties 
	{ 
		_MainTex ("Texture", 2D) = "white" {} 
		_MaskTex ("Texture", 2D) = "white" {}
		_Alpha ("Alpha", Range(0,1)) = 0
	}

	SubShader 
	{ 
		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		} 

		CGPROGRAM
		#pragma surface surf Lambert  alpha:blend
		#pragma target 3.0
		
		sampler2D _MainTex; 	
		sampler2D _MaskTex;
		float _Alpha;

		struct Input 
		{ 
			float2 uv_MainTex;
			float2 uv_MaskTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{ 	
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 m = tex2D(_MaskTex, IN.uv_MaskTex);
			float alpha = c.a * ( m.a + _Alpha);
			o.Albedo = c.rgb;
			o.Alpha = c.a * _Alpha;			
		} 
		ENDCG 	        
	}
}

/////////////////////////////////////////////	Draw On Mouse Position //////////////////////////////////////////
//Shader "Custom/DrawPainting" 
//{ 
//	Properties 
//	{ 
//		_MainTex ("Texture", 2D) = "white" {} 
//		_PaintingSprite ("Painting Sprite", 2D) = "white" {} 
//		_MousePos ("MousePos", Vector) = (-1,-10,-1,-1)
//		_Radius ("HoleRadius", Range(0.1,5)) = 2
//	}
//
//	SubShader 
//	{ 
//		Tags 
//		{
//			"Queue"="Transparent"
//			"IgnoreProjector"="True"
//			"RenderType"="Transparent"
//		} 
//
//		CGPROGRAM
//		#pragma surface surf Lambert  alpha:blend
//		#pragma target 3.0
//		
//		sampler2D _MainTex; 	
//		sampler2D _PaintingSprite;
//		
//		float4 _MousePos;
//		float _Radius;
//
//		struct Input 
//		{ 
//			float2 uv_MainTex;
//			float2 uv_PaintingSprite;
//			float3 worldPos;
//		};
//
//		void surf (Input IN, inout SurfaceOutput o) 
//		{ 						
//			float dx = length(_MousePos.x-IN.worldPos.x);
//			float dy = length(_MousePos.y-IN.worldPos.y);
//			float dz = length(_MousePos.z-IN.worldPos.z);
//			
//			float alpha = (dx*dx+dy*dy+dz*dz) / _Radius;
//			alpha = clamp(alpha,0,1);	
//			if(alpha == 1)
//			{
//				alpha = 0;
////				tex2D(_MainTex, IN.uv_MainTex) = tex2D(_PaintingSprite, IN.uv_PaintingSprite);
//			}
//				else
//				alpha = 1;
//			
//			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
//			o.Albedo = c.rgb;
//			o.Alpha = c.a * alpha;
//			
//		} 
//		ENDCG 	        
//	}
//}

/////////////////////////////////////////////	SetPixel //////////////////////////////////////////
//Shader "Custom/DrawPainting" 
//{ 
//	Properties 
//	{ 
//		_MainTex ("Texture", 2D) = "white" {} 
//		_PaintingSprite ("Painting Sprite", 2D) = "white" {} 
//		_MousePos ("MousePos", Vector) = (-1,-10,-1,-1)
//		_Radius ("HoleRadius", Range(0.1,5)) = 2
//		_array("Array", 2D) = ""{}
//	}
//
//	SubShader 
//	{ 
//		Tags 
//		{
//			"Queue"="Transparent"
//			"IgnoreProjector"="True"
//			"RenderType"="Transparent"
//		} 
////		LOD 200
////		Cull Off
//
//		CGPROGRAM
//		// add Alpha
//		#pragma surface surf Lambert  alpha:blend
//		#pragma target 3.0
////finalcolor:finalblend
//		sampler2D _MainTex; 	
//		sampler2D _PaintingSprite;
//		sampler2D _array;
//		
//		float4 _MousePos;
//		float _Radius;
//		uniform float myarrayy[10];
//		uniform float myarrayx[2];
//
//		struct Input 
//		{ 
//			float2 uv_MainTex;
//			float2 uv_PaintingSprite;
//			float2 uv_array;
//			float3 worldPos;
//			float4 screenPos;
//			fixed4 color;
//		};
//
//		void surf (Input IN, inout SurfaceOutput o) 
//		{ 
////			float alpha = 0;
//			
//			fixed4 m = tex2D(_MainTex, IN.uv_MainTex);
//			fixed4 c = tex2D(_PaintingSprite, IN.uv_PaintingSprite);
//			fixed4 ar = tex2D(_array, IN.uv_array);
////			for(int i = 0; i < 100; i++)
////			{
////				if(myarrayx[i] == IN.worldPos.x && myarrayy[i] == IN.worldPos.y)
////				{					
////					alpha = 1;
////				}
////			}
////			_array = _MousePos.x;
////			if(myarrayx[1] == 482)
////				alpha = 1;
////			if(myarrayx[0] == 0)
////				alpha = 1;
////			float dx = length(_MousePos.x-IN.worldPos.x);
////			float dy = length(_MousePos.y-IN.worldPos.y);
////			float dz = length(_MousePos.z-IN.worldPos.z);
////			alpha = (dx*dx+dy*dy+dz*dz) / _Radius;
////			alpha = distance(_MousePos,IN.worldPos) / _Radius;
////			alpha = clamp(alpha,0,1);
//////			
////			if(alpha == 1)
////				alpha = 0;
////				else
////				alpha = 1;
////			if (dist <= 1)
////			{
////				tex2D(_MainTex, IN.uv_MainTex).rgb = tex2D(_PaintingSprite, IN.uv_PaintingSprite);
////			}
////			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * dist; 
////			
////			o.Albedo = c.rgb;
//			
//			o.Albedo = ar.rgb;
//			o.Alpha = ar.a;
//		} 
//		
//		void finalblend (Input IN, SurfaceOutput o, inout fixed4 color)
//		{
////			float dx = length(_MousePos.x-IN.worldPos.x);
////			float dy = length(_MousePos.y-IN.worldPos.y);
////			float dz = length(_MousePos.z-IN.worldPos.z);
////			float dist = (dx*dx+dy*dy+dz*dz) / _Radius;
////			float dist = distance(_ObjPos,IN.worldPos) / _Radius;
////			if (dist <= 1)
////			{
////				color = tex2D(_PaintingSprite, IN.uv_PaintingSprite);
////			}
////			else
////			{
////				color = tex2D(_MainTex, IN.uv_MainTex);
////			}
//			
////			else
////			{
////				color *= lerp(tex2D(_MainTex, IN.uv_MainTex), tex2D(_LitTex, IN.uv_MainTex), (light - _DarkThreshold) / (_LightThreshold - _DarkThreshold));
////			}
////			//color *= IN.color;
////			if (color.a < _Cutout) discard;
////			
////			//if (light <= _DarkThreshold) discard;
////			//color = light;
////			color.a = 1;
//			
//		}
//		ENDCG 
//	
//	}
////	Fallback "Transparent/VertexLit" 
//}

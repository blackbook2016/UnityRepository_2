Shader "Custom/DrawPainting" 
{ 
	Properties 
	{ 
		_MainTex ("Texture", 2D) = "white" {} 
		_Spray ("Spray Sprite", 2D) = "white" {} 
		_PaintingSprite ("Painting Sprite", 2D) = "white" {} 
		_MousePos ("ObjPos", Vector) = (-1,-10,-1,-1)
		_Radius ("HoleRadius", Range(0.1,5)) = 2
	}

	SubShader 
	{ 
		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		} 
//		LOD 200
//		Cull Off

		CGPROGRAM
		// add Alpha
		#pragma surface surf Lambert  alpha:blend
		#pragma target 3.0
//finalcolor:finalblend
		sampler2D _MainTex; 	
		sampler2D _PaintingSprite;
		sampler2D _Spray;
		
		float4 _MousePos;
		float _Radius;
		float myarrayy[200];
		float myarrayx[200];

		struct Input 
		{ 
			float2 uv_MainTex;
			float2 uv_PaintingSprite;
			float3 worldPos;
			float4 screenPos;
			fixed4 color;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{ 
			int alpha = 0;
			
			fixed4 c = tex2D(_PaintingSprite, IN.uv_PaintingSprite);
//			for(int i = 0; i < 100; i++)
//			{
//				if(myarrayx[i] == IN.worldPos.x && myarrayy[i] == IN.worldPos.y)
//				{					
//					alpha = 1;
//				}
//			}
			if(myarrayx[0] == 482)
				alpha = 1;
//			float dx = length(_ObjPos.x-IN.worldPos.x);
//			float dy = length(_ObjPos.y-IN.worldPos.y);
//			float dz = length(_ObjPos.z-IN.worldPos.z);
//			float dist = (dx*dx+dy*dy+dz*dz) / _Radius;
////			float dist = distance(_ObjPos,IN.worldPos) / _Radius;
//			dist = clamp(dist,0,1);
//			if (dist <= 1)
//			{
//				tex2D(_MainTex, IN.uv_MainTex).rgb = tex2D(_PaintingSprite, IN.uv_PaintingSprite);
//			}
//			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * dist; 
//			
//			o.Albedo = c.rgb;
			o.Albedo = c.rgb;
			o.Alpha = c.a * alpha;
		} 
		
		void finalblend (Input IN, SurfaceOutput o, inout fixed4 color)
		{
//			float dx = length(_MousePos.x-IN.worldPos.x);
//			float dy = length(_MousePos.y-IN.worldPos.y);
//			float dz = length(_MousePos.z-IN.worldPos.z);
//			float dist = (dx*dx+dy*dy+dz*dz) / _Radius;
//			float dist = distance(_ObjPos,IN.worldPos) / _Radius;
//			if (dist <= 1)
//			{
//				color = tex2D(_PaintingSprite, IN.uv_PaintingSprite);
//			}
//			else
//			{
//				color = tex2D(_MainTex, IN.uv_MainTex);
//			}
			
//			else
//			{
//				color *= lerp(tex2D(_MainTex, IN.uv_MainTex), tex2D(_LitTex, IN.uv_MainTex), (light - _DarkThreshold) / (_LightThreshold - _DarkThreshold));
//			}
//			//color *= IN.color;
//			if (color.a < _Cutout) discard;
//			
//			//if (light <= _DarkThreshold) discard;
//			//color = light;
//			color.a = 1;
			
		}
		ENDCG 
	
	}
//	Fallback "Transparent/VertexLit" 
}

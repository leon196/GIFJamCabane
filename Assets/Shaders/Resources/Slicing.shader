Shader "Unlit/Slicing"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NewFrame ("Frame", 2D) = "white" {}
		_LinePosition ("Line Position", Float) = 0
		_LineSize ("Line Size", Float) = 0.02
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _NewFrame;
			float4 _MainTex_ST;
			float _LinePosition;
			float _LineSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 buffer = tex2D(_MainTex, i.uv);
				fixed4 frame = tex2D(_NewFrame, i.uv);
				float should = step(_LinePosition, i.uv.y) - step(_LinePosition + _LineSize, i.uv.y);
				fixed4 color = lerp(buffer, frame, should);
				return color;
			}
			ENDCG
		}
	}
}

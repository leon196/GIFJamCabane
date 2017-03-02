Shader "Hidden/Filter"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _TimeTexture;

			fixed4 frag (v2f_img i) : SV_Target
			{
				float2 uv = i.uv;
				uv.y = 1.0 - uv.y;
				fixed4 col = tex2D(_TimeTexture, uv);
				return col;
			}
			ENDCG
		}
	}
}

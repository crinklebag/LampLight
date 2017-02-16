// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Beam" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_NoiseTex ("Noise (RGB) Trans (A)", 2D) = "white" {}
	_mix("mix", Float) = 0.05
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha One
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _NoiseTex;
			fixed _mix;
			
			float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				fixed4 nCol = tex2D(_NoiseTex, half2(i.texcoord.x + i.texcoord.y *0.1, _Time.x*0.4)*0.35);
				
				col*= nCol * _mix;
				
				return col;
			}
		ENDCG
	}
}

}

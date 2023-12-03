Shader"Unlit/Scroll"
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_MoveSpeed("MoveSpeed", float) = 0
		//_RotateSpeed("RotateSpeed", float) = 0
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

		ZWrite
			Off
        Blend
			SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _MoveSpeed; //_RotateSpeed;
			float4 _MainTex_ST;
			
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
						
			v2f vert(appdata v)
			{
			    v2f o;
			    o.vertex = UnityObjectToClipPos(v.vertex);
			    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			    return o;
			}
						
			fixed4 frag(v2f i) : SV_Target
			{
			    half2 uv = i.uv;
				half t = _Time.x - floor(_Time.x);
				uv.y = i.uv.y + t * _MoveSpeed;
				
			    fixed4 col = tex2D(_MainTex, uv);
				
			    return col;
			}
			ENDCG
		}
	}
}
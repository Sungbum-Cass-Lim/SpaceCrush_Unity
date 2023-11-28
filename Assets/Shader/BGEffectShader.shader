Shader"Unlit/Scroll"
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_MoveSpeed("MoveSpeed", float) = 1
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
			    float2 uv : TEXCOORD0;
			    float4 vertex : SV_POSITION;
				UNITY_FOG_COORDS(1)
				UNITY_VERTEX_OUTPUT_STEREO
			};
						
			v2f vert(appdata v)
			{
			    v2f o;
			    UNITY_SETUP_INSTANCE_ID(v);
			    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			    o.vertex = UnityObjectToClipPos(v.vertex);
			    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			    UNITY_TRANSFER_FOG(o, o.vertex);
			    return o;
			}
						
			fixed4 frag(v2f i) : SV_Target
			{
			    half2 uv = i.uv;
			    half t = _Time.x - floor(_Time.x);
				uv.y = i.uv.y + t * _MoveSpeed;
				
			    fixed4 col = tex2D(_MainTex, uv);
			    UNITY_APPLY_FOG(i.fogCoord, col);
				
			    return col;
			}
			ENDCG
		}
	}
}
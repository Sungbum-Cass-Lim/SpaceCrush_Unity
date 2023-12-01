Shader"Custom/TestScrollShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _CT ("Speed", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        float _CT;

        struct Input
        {
            float2 uv_MainTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 mainTex = tex2D(_MainTex, float2(IN.uv_MainTex.x, IN.uv_MainTex.y + _Time.y * _CT));
            o.Emission = mainTex.rgb;
}
        ENDCG
    }
    FallBack "Diffuse"
}

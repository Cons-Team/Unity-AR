Shader "Custom/TransparentLineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // 텍스처 속성
        _Color ("Main Color", Color) = (1,1,1,1)  // 색상 속성
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}  // 투명 처리 태그
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;  // 텍스처 색상 적용
            o.Alpha = c.a;  // 알파(투명도) 채널 적용
        }
        ENDCG
    }
    FallBack "Diffuse"
}
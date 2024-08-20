Shader "Custom/TransparentLineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // �ؽ�ó �Ӽ�
        _Color ("Main Color", Color) = (1,1,1,1)  // ���� �Ӽ�
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}  // ���� ó�� �±�
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
            o.Albedo = c.rgb;  // �ؽ�ó ���� ����
            o.Alpha = c.a;  // ����(����) ä�� ����
        }
        ENDCG
    }
    FallBack "Diffuse"
}
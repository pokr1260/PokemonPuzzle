Shader "Custom/Psychic"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ReflectionStrength("Reflection Strength", Range(0, 0.1)) = 0.05
        _ReflectionSpeed("Reflection Speed", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        zwrite off

        GrabPass { } // 화면 캡쳐

        CGPROGRAM
        #pragma surface surf nolight noambient alpha:fade

        sampler2D _GrabTexture;
        sampler2D _MainTex;
        float _ReflectionStrength;
        float _ReflectionSpeed;

        struct Input
        {
            float4 color:COLOR;
            float4 screenPos;
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float4 ref = tex2D(_MainTex, float2(IN.uv_MainTex.x, IN.uv_MainTex.y - _Time.y * _ReflectionSpeed));

            // 카메라의 거리 영향을 제거하기 위함.
            float3 screenUV = IN.screenPos.rgb / IN.screenPos.a;
            // 뒤집어진것을 바로 잡기위한 1 - (screenUV.y ... )
            o.Emission = 1 - tex2D(_GrabTexture, float2(screenUV.x + ref.x * _ReflectionStrength, 1 - (screenUV.y + ref.y * _ReflectionStrength)));
        }

        float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4(0, 0, 0, 1);
        }

        ENDCG
    }
    FallBack "Regacy Shaders/Transparent/Vertexlit"
}

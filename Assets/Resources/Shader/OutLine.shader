Shader "Custom/OutLine"
{
    //Sprites-Default.shader 를 수정해서 만듬.
    //https://www.febucci.com/2019/06/sprite-outline-shader/

    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        _OutLineColor("OutLineColor", Color) = (1,0,0,1)
        _OutLineThick("OutLineThick", Range(0,3)) = 1.0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Cull Off
            Lighting Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                    #pragma vertex SpriteVert
                    #pragma fragment Frag
                    #pragma target 2.0
                    #pragma multi_compile_instancing
                    #pragma multi_compile_local _ PIXELSNAP_ON
                    #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
                    #include "UnitySprites.cginc"

                float4 _MainTex_TexelSize;

                float4 _OutLineColor;
                float _OutLineThick;
                

                fixed4 Frag(v2f IN) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color;

                    col.rgb *= col.a;

                    fixed leftPixel   = tex2D(_MainTex, IN.texcoord + float2(-_MainTex_TexelSize.x * _OutLineThick, 0)).a;
                    fixed upPixel     = tex2D(_MainTex, IN.texcoord + float2(0, _MainTex_TexelSize.y * _OutLineThick)).a;
                    fixed rightPixel  = tex2D(_MainTex, IN.texcoord + float2(_MainTex_TexelSize.x * _OutLineThick, 0)).a;
                    fixed bottomPixel = tex2D(_MainTex, IN.texcoord + float2(0, -_MainTex_TexelSize.y * _OutLineThick)).a;

                    //fixed outline = (1 - leftPixel * upPixel * rightPixel * bottomPixel) * col.a;
                    fixed outline = max(max(leftPixel, upPixel), max(rightPixel, bottomPixel)) - col.a;

                    return lerp(col, _OutLineColor, outline) * IN.color.a;
                }


            ENDCG
            }



        }
}

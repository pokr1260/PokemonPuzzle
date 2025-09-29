Shader "Custom/StatusChange"
{
	//Sprites-Default.shader 를 수정해서 만듬.

	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

		_MixTexture("mix texture", 2D) = "white" {}
		_TextureSpeed("texture speed", Range(-1,1)) = 0.5
		_MixTextureUVSize("MixTexture UVSize", Float) = 10.0
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
				#pragma fragment SpriteFrag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_local _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"

			struct v2f_custom
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			//sampler2D _MainTex;
			sampler2D _MixTexture;
			
			float4 _MainTex_ST;
			float4 _MixTexture_ST;

			float _TextureSpeed;
			float _MixTextureUVSize;

			v2f_custom SpriteVert(appdata_full IN)
			{
				v2f_custom o;
				o.vertex = UnityObjectToClipPos(IN.vertex);
				o.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);

				// _MixTextureUVSize 없이 하고 싶은데 UV크기 조절이 필요할수 밖에 없었다 왜지? 음...
				o.texcoord1 = TRANSFORM_TEX(IN.texcoord1, _MixTexture) * _MixTextureUVSize;		

				o.color = IN.color * _Color;

				#ifdef PIXELSNAP_ON
				o.vertex = UnityPixelSnap(o.vertex);
				#endif

				return o;
			}

			fixed4 SampleSpriteTextures(float2 uv1, float2 uv2)
			{
				fixed4 color = tex2D(_MainTex, uv1);
				fixed4 mixTextureColor = tex2D(_MixTexture, float2(uv2.x, uv2.y - (_Time.y * _TextureSpeed)));

				return color * mixTextureColor;
			}

			fixed4 SpriteFrag(v2f_custom i) : SV_Target
			{
				fixed4 color = SampleSpriteTextures(i.texcoord,i.texcoord1);

				color.rgb *= color.a;

				return color;
			}
			ENDCG
			}
		}
}
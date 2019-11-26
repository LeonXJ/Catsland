// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/CustomerDiffuse"
{
  Properties
  {
    [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)
    _AmbientLight ("AmbientLight", Color) = (0,0,0,0)
    _HueShift ("Shift Hue", Color) = (0,0,0,0)
    [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
  }

  SubShader
  {
    Tags
    { 
      "Queue"="Transparent" 
      "IgnoreProjector"="True" 
      "RenderType"="Transparent" 
      "PreviewType"="Plane"
      "CanUseSpriteAtlas"="True"
    }

    Cull Off
    Lighting Off
    ZWrite Off
    Blend One OneMinusSrcAlpha

    Pass
    {
    CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile _ PIXELSNAP_ON
      #include "UnityCG.cginc"
      
      struct appdata_t
      {
        float4 vertex   : POSITION;
        float4 color    : COLOR;
        float2 texcoord : TEXCOORD0;
      };

      struct v2f
      {
        float4 vertex   : SV_POSITION;
        fixed4 color    : COLOR;
        float2 texcoord  : TEXCOORD0;
      };
      
      fixed4 _Color;
      fixed4 _AmbientLight;
      fixed4 _HueShift;

      v2f vert(appdata_t IN)
      {
        v2f OUT;
        OUT.vertex = UnityObjectToClipPos(IN.vertex);
        OUT.texcoord = IN.texcoord;
        OUT.color =  IN.color * _Color;
        #ifdef PIXELSNAP_ON
        OUT.vertex = UnityPixelSnap (OUT.vertex);
        #endif

        return OUT;
      }

      sampler2D _MainTex;
      sampler2D _AlphaTex;
      float _AlphaSplitEnabled;

      fixed4 SampleSpriteTexture (float2 uv)
      {
        fixed4 color = tex2D (_MainTex, uv);
#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
        if (_AlphaSplitEnabled)
          color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

        return color;
      }

      fixed4 frag(v2f IN) : SV_Target
      {
        fixed4 c = clamp(SampleSpriteTexture (IN.texcoord) * IN.color + fixed4(_AmbientLight.rgb, 0), fixed4(0,0,0,0), fixed4(1,1,1,1));
        
        // Hue shift
        c.rgb = lerp(c.rgb, c.gbr, _HueShift.r); //90 degrees
        c.rgb = lerp(c.rgb, c.gbr, _HueShift.g); //180 degrees
        c.rgb = lerp(c.rgb, c.bgr, _HueShift.b); //270 degrees

        c.rgb *= c.a;
        return c;
      }

    ENDCG
    }
  }
}
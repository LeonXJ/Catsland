Shader "Custom/ComposeLightmap" {
	Properties {
    _MainTex ("Base", 2D) = "white" {}
    _Lightmap ("Lightmap", 2D) = "white" {}
	}
	SubShader {
    Pass {
      CGPROGRAM
      #pragma vertex vert_img
      #pragma fragment frag
      #include "UnityCG.cginc"

      uniform sampler2D _MainTex;
      uniform sampler2D _Lightmap;

      fixed4 frag (v2f_img i) : COLOR {
        fixed4 light = tex2D(_Lightmap, i.uv);
        fixed4 base = tex2D(_MainTex, i.uv);
        return fixed4(base.rgb * light.rgb, base.a);
      }
      ENDCG
    }
  }
}

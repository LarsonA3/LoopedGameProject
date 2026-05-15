Shader "Custom/SaberBlade" {
  Properties {
    _Color ("Color", 2D) = "white" {}
  }
  
  SubShader {
    Pass {
      CGPROGRAM
      #pragma target 3.0
      #pragma glsl
      #pragma vertex vert
      #pragma fragment frag
      #include "UNITYCG.cginc"
      
      struct appdata {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };
      
      struct v2f {
        float4 position : SV_POSITION;
        float4 color : COLOR;
      };
      
      sampler2D _VertColorPal;
      
      v2f vert(appdata v) {
        v2f o;
        o.color = float4(1,0,0,1);
        o.position = UnityObjectToClipPos(v.vertex);
        return o;
      }
      
      fixed4 frag(v2f f) : COLOR {
        return fixed4(f.color);
      }
      ENDCG
    }
  }
}
Shader "Custom/SaberBlade" {
  Properties {
    _Color ("Color", 2D) = "white" {}
  }
  
  SubShader {
    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UNITYCG.cginc"
      
      struct appdata {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };
      
      struct v2f {
        float4 vertex : SV_POSITION;
        //float4 color : COLOR;
        float2 uv : TEXCOORD0;
      };
      
      v2f vert(appdata v) {
        v2f o;
        o.uv = v.uv;
        o.uv.y = sin(o.uv.y * 20 * _Time.y);
        o.uv.x = sin(o.uv.x * 20 * _Time.y);
        //o.color = float4(1,0,0,1);
        float3 vert = v.vertex;
        vert.y = o.uv.y / 50;
        vert.x = o.uv.x / 50;
        vert.z = -sqrt((vert.y*vert.y)+(vert.x*vert.x))*2;

        o.vertex = UnityObjectToClipPos(vert);
        return o;
      }
      
      fixed4 frag(v2f f) : SV_Target {
        return fixed4(f.uv.y, 0, 0, 1);
        //return fixed4(f.color);
      }
      ENDCG
    }
  }
}
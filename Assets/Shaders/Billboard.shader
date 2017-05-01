Shader "Custom/Billboard" {
  Properties {
    _MainTex ("Texture Image", 2D) = "white" {}
    _CutOff("Alpha Cutoff", float) = 0.5
  }

  SubShader {
    Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "DisableBatching"="True" }
    Cull off

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      uniform sampler2D _MainTex;
      uniform float _CutOff;
              
      struct vertexInput {
        float4 vertex : POSITION;
        float2 tex : TEXCOORD0;
      };
      struct vertexOutput {
        float4 pos : SV_POSITION;
        float2 tex : TEXCOORD0;
      };

      vertexOutput vert (vertexInput i) {
        vertexOutput o;
        float3 objSpaceCamPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1)).xyz;
        float3 offsetDir = normalize(cross(float3(0.0f, 1.0f, 0.0f), objSpaceCamPos));
 
        o.pos.xz = i.vertex.x * offsetDir.xz;
        o.pos.yw = i.vertex.yw;
 
        o.pos = UnityObjectToClipPos(o.pos);
        o.tex = i.tex;

        return o;
      }

      float4 frag(vertexOutput input) : COLOR {
        float4 color = tex2D(_MainTex, float2(input.tex.xy));   
        if(color.a < _CutOff) discard;
        return color;
      }
      ENDCG
    }
  }
}
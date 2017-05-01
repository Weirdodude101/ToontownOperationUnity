Shader "Custom/DoubleSided" {
  Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
  }
 
  SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 200
    Lighting On
    Cull Off

    CGPROGRAM
    #pragma surface surf Lambert alphatest:_Cutoff

	half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
    	half NdotL = dot (s.Normal, lightDir);
		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
		c.a = s.Alpha;
		return c;
	}
 
    sampler2D _MainTex;
    fixed4 _Color;
 
    struct Input {
      float2 uv_MainTex;
    };
 
    void surf (Input IN, inout SurfaceOutput o) {
      fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
      o.Albedo = c.rgb;
      o.Alpha = c.a;
    }
    ENDCG
  }
  Fallback "Diffuse"
}
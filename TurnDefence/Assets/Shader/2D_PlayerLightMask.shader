Shader "Custom/2D_PlayerLightMask"
{
    Properties
    {
        _Color ("Dark Color", Color) = (0,0,0,0.85)
        _PlayerWorldPos ("Player Position", Vector) = (0,0,0,0)
        _Radius ("Light Radius", Float) = 3.0
        _Softness ("Edge Softness", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float4 _PlayerWorldPos;
            float _Radius;
            float _Softness;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.worldPos.xy, _PlayerWorldPos.xy);
                float alpha = smoothstep(_Radius, _Radius - _Softness, dist);
                alpha = 1 - alpha;
                return fixed4(_Color.rgb, _Color.a * alpha);
            }
            ENDCG
        }
    }
}

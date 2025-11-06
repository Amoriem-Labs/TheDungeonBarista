Shader "UI/Outlined UI"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [HDR] _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness (px)", Range(0,10)) = 1
        _OutlineAlpha ("Outline Strength (0..1)", Range(0,1)) = 1

        // ---- MaskableGraphic stencil plumbing (Unity sets these on masked children) ----
        [HideInInspector]_StencilComp      ("Stencil Comparison", Float) = 8     // Always
        [HideInInspector]_Stencil          ("Stencil ID",         Float) = 0
        [HideInInspector]_StencilOp        ("Stencil Operation",  Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask  ("Stencil Read Mask",  Float) = 255
        [HideInInspector]_ColorMask        ("Color Mask",         Float) = 15
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

        // IMPORTANT: Use UI stencil properties so it works with and without masks
        Stencil
        {
            Ref       [_Stencil]
            Comp      [_StencilComp]
            Pass      [_StencilOp]
            ReadMask  [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; // x=1/width, y=1/height, z=width, w=height
            fixed4 _Color;

            fixed4 _OutlineColor;
            float  _OutlineThickness;    // in pixels
            float  _OutlineAlpha;     // 0..1

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.texcoord;
                o.color  = v.color * _Color;
                return o;
            }

            // Helper: max alpha in 4-neighborhood around uv with given pixel offset
            inline half MaxNeighborAlpha(float2 uv, float2 px)
            {
                // Sample 4 directions; you can add diagonals if you want a rounder outline
                half a1 = tex2D(_MainTex, uv + float2( px.x, 0    )).a;
                half a2 = tex2D(_MainTex, uv + float2(-px.x, 0    )).a;
                half a3 = tex2D(_MainTex, uv + float2( 0,    px.y )).a;
                half a4 = tex2D(_MainTex, uv + float2( 0,   -px.y )).a;
                return max(max(a1,a2), max(a3,a4));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample source color
                fixed4 src = tex2D(_MainTex, i.uv) * i.color;

                // Convert pixel thickness to UV offset
                float2 px = _OutlineThickness * _MainTex_TexelSize.xy;

                // Simple dilation-based outline mask: neighbors minus center
                half neighborMax = MaxNeighborAlpha(i.uv, px);
                half outlineMask = saturate(neighborMax - src.a);   // only outside the filled area

                // Apply user strength (0..1)
                outlineMask *= saturate(_OutlineAlpha);

                // Compose: draw outline behind src, preserving src color
                fixed4 outline = _OutlineColor;
                outline.a *= outlineMask;

                // Standard “over” compositing for outline underneath src
                half outA   = saturate(src.a + outline.a * (1 - src.a));
                half3 outRGB = outline.rgb * outline.a * (1 - src.a) + src.rgb;

                return fixed4(outRGB, outA);
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}

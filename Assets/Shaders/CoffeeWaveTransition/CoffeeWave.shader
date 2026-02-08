Shader "CoffeeWave"
{
    Properties
    {
        _WaveFrontTex ("WaveFrontTex", 2D) = "black" {}
        _WaveBackTex   ("WaveBackTex", 2D)   = "black" {}
        _WaveLightTex ("WaveLightTex", 2D) = "black" {}
        _WaveDarkTex ("WaveDarkTex", 2D) = "black" {}
        
        _DarkLayerTexture ("DarkLayerTexture", 2D) = "black" {}
        
        _FadeMask ("FadeMask", 2D) = "black" {}
        _SmallBubbleTex ("SmallBubbleTex", 2D) = "black" {}
        _BubbleAlpha ("BubbleAlpha", Range(0, 1)) = 0.8
    
        _LightColor ("LightColor", Color) = (1, 1, 1, 1)
        _NormalColor ("NormalColor", Color) = (.5, .5, .5, 1)
        _DarkColor  ("DarkColor", Color) = (0, 0, 0, 1)
        
        _BackMultiplier ("BackMultiplier", Color) = (.5, .5, .5, 1)
        _BackParallaxRatio ("BackParallaxRatio", Float) = .5
        
        _ScrollSpeed ("ScrollSpeed", Float) = 1

        _Progress ("Progress", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Overlay"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass
        {
            Name "SceneTransitionFullscreen"
            Tags { "LightMode"="UniversalForward" }

            Cull Off
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            TEXTURE2D(_WaveFrontTex);      SAMPLER(sampler_WaveFrontTex);
            TEXTURE2D(_WaveBackTex);        SAMPLER(sampler_WaveBackTex);
            TEXTURE2D(_WaveLightTex);     SAMPLER(sampler_WaveLightTex);
            TEXTURE2D(_WaveDarkTex);     SAMPLER(sampler_WaveDarkTex);

            TEXTURE2D(_DarkLayerTexture); SAMPLER(sampler_DarkLayerTexture);

            TEXTURE2D(_FadeMask);     SAMPLER(sampler_FadeMask);
            TEXTURE2D(_SmallBubbleTex);     SAMPLER(sampler_SmallBubbleTex);
            
            float4 _WaveFrontTex_ST;
            float4 _WaveBackTex_ST;
            float4 _WaveLightTex_ST;
            float4 _WaveDarkTex_ST;

            float4 _DarkLayerTexture_ST;

            float4 _FadeMask_ST;
            float4 _SmallBubbleTex_ST;
            float _BubbleAlpha;

            float _ScrollSpeed;
                        
            float4 _BackMultiplier;
            float _BackParallaxRatio;
            
            float _Progress;
            
            Varyings Vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 _LightColor;
            float4 _NormalColor;
            float4 _DarkColor;
            
            float4 Frag (Varyings IN) : SV_Target
            {
                float yOffset = (1 - _Progress) * 3 - 1.5;
                
                float2 absUV = float2(IN.uv.x, saturate(IN.uv.y + yOffset));

                float lightMask = SAMPLE_TEXTURE2D(_WaveLightTex, sampler_WaveLightTex, absUV).a;
                float darkMask = SAMPLE_TEXTURE2D(_WaveDarkTex, sampler_WaveDarkTex, absUV).a;

                float4 frontCol;
                {
                    float xScroll = _Time * _ScrollSpeed;
                    float2 scrollUV = float2(absUV.x + xScroll, absUV.y);
                    float waveMask = SAMPLE_TEXTURE2D(_WaveFrontTex, sampler_WaveFrontTex, scrollUV).a;
                    float4 darkCol = SAMPLE_TEXTURE2D(_DarkLayerTexture, sampler_DarkLayerTexture, IN.uv + float2(xScroll * 0.1, 0));
                    float4 col = lerp(lerp(_NormalColor, _LightColor, lightMask), darkCol, darkMask);
                    frontCol = float4(col.rgb, waveMask);

                    // float4 topBubbles = SAMPLE_TEXTURE2D(_FadeMask, sampler_FadeMask, scrollUV) * waveMask;
                    float bubbleMask = SAMPLE_TEXTURE2D(_FadeMask, sampler_FadeMask, absUV).r;
                    float4 smallBubbles = SAMPLE_TEXTURE2D(_SmallBubbleTex, sampler_SmallBubbleTex, absUV - float2(0, xScroll / 2)) * waveMask * bubbleMask;
                    
                    frontCol = lerp(frontCol, smallBubbles, smallBubbles.a * _BubbleAlpha);
                }

                float4 backCol;
                {
                    float xScroll = _Time * _ScrollSpeed;
                    float2 scrollUV = float2(absUV.x - xScroll * _BackParallaxRatio + .314, absUV.y);
                    float waveMask = SAMPLE_TEXTURE2D(_WaveBackTex, sampler_WaveBackTex, scrollUV).a;
                    // float4 col = lerp(lerp(_NormalColor, _LightColor, lightMask), _DarkColor, darkMask);
                    float4 col = _NormalColor * waveMask;
                    backCol = float4(col.rgb, waveMask) * _BackMultiplier;
                }

                return lerp(backCol, frontCol, frontCol.a);
            }
            ENDHLSL
        }
    }
    Fallback Off
}

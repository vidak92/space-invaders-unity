Shader "SGS/DestructibleQuad"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (1, 1, 1, 1)
        _CornerRadius ("Corner Radius", Range(0, 0.5)) = 0.25
        _NoiseThreshold ("Noise Threshold", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Packages/com.sgs.sgs-tools/Assets/Shaders/ShaderUtils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color1;
            float4 _Color2;
            float _NoiseThreshold;
            float _CornerRadius;
            
            int _Cols = 9; // @NOTE same as Constants.DESTRUCTIBLE_QUAD_GRID_COL_COUNT
            int _Rows = 4; // @NOTE same as Constants.DESTRUCTIBLE_QUAD_GRID_ROW_COUNT
            float _Flags[36]; // @NOTE same as Constants.DESTRUCTIBLE_QUAD_GRID_FLAG_COUNT

            float GetFlag(int x, int y)
            {
                if (x < 0 || x >= _Cols || y < 0 || y >= _Rows)
                {
                    return 0;
                }
                return _Flags[x + y * _Cols];
            }

            float GetMask(float2 cellCoords, int2 cellIndex, float2 cellCenter, float roundRect)
            {
                bool isFlagOn = GetFlag(cellIndex.x, cellIndex.y) > 0;

                bool isFlagTopOn = GetFlag(cellIndex.x, cellIndex.y + 1) > 0;
                bool isFlagBottomOn = GetFlag(cellIndex.x, cellIndex.y - 1) > 0;
                bool isFlagRightOn = GetFlag(cellIndex.x + 1, cellIndex.y) > 0;
                bool isFlagLeftOn = GetFlag(cellIndex.x - 1, cellIndex.y) > 0;

                bool isFlagTopRightOn = GetFlag(cellIndex.x + 1, cellIndex.y + 1) > 0;
                bool isFlagTopLeftOn = GetFlag(cellIndex.x - 1, cellIndex.y + 1) > 0;
                bool isFlagBottomLeftOn = GetFlag(cellIndex.x - 1, cellIndex.y - 1) > 0;
                bool isFlagBottomRightOn = GetFlag(cellIndex.x + 1, cellIndex.y - 1) > 0;

                bool isTopHalf = cellCoords.y > cellCenter.y;
                bool isBottomHalf = cellCoords.y < cellCenter.y;
                bool isRightHalf = cellCoords.x > cellCenter.x;
                bool isLeftHalf = cellCoords.x < cellCenter.x;

                bool isTopRightQuadrant = isTopHalf && isRightHalf;
                bool isBottomRightQuadrant = isBottomHalf && isRightHalf;
                bool isTopLeftQuadrant = isTopHalf && isLeftHalf;
                bool isBottomLeftQuadrant = isBottomHalf && isLeftHalf;
                
                float mask = isFlagOn ? roundRect : 0;
                
                mask += isFlagOn && isFlagTopOn && isTopHalf ? 1 : 0;
                mask = saturate(mask);
                
                mask += isFlagOn && isFlagBottomOn && isBottomHalf ? 1 : 0;
                mask = saturate(mask);
                
                mask += isFlagOn && isFlagRightOn && isRightHalf ? 1 : 0;
                mask = saturate(mask);

                mask += isFlagOn && isFlagLeftOn && isLeftHalf ? 1 : 0;
                mask = saturate(mask);

                mask += !isFlagOn && isFlagTopOn && isFlagRightOn && isFlagTopRightOn && isTopRightQuadrant ? 1 - roundRect : 0;
                mask = saturate(mask);
                
                mask += !isFlagOn && isFlagTopOn && isFlagLeftOn && isFlagTopLeftOn && isTopLeftQuadrant ? 1 - roundRect : 0;
                mask = saturate(mask);

                mask += !isFlagOn && isFlagBottomOn && isFlagRightOn && isFlagBottomRightOn && isBottomRightQuadrant ? 1 - roundRect : 0;
                mask = saturate(mask);

                mask += !isFlagOn && isFlagBottomOn && isFlagLeftOn && isFlagBottomLeftOn && isBottomLeftQuadrant ? 1 - roundRect : 0;
                mask = saturate(mask);

                return mask;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 tiling = _MainTex_ST.xy;
                float2 offset = _MainTex_ST.zw;
                
                float2 uniformSize = float2(_Cols, _Rows);
                float2 uniformCoords = (i.uv - offset) / tiling * uniformSize;
                float2 cellCoords = float2(uniformCoords.x % 1, uniformCoords.y % 1);
                float2 cellIndex = float2(floor(uniformCoords.x), floor(uniformCoords.y));
                float2 cellCenter = 0.5;
                float2 cellSize = 1;
                
                float roundRect = RoundRectFill(cellCoords, 0, cellSize, _CornerRadius);
                float mask = GetMask(cellCoords, cellIndex, cellCenter, roundRect);
                
                float colorT = tex2D(_MainTex, i.uv).x;
                colorT = step(colorT, _NoiseThreshold);
                float4 color = lerp(_Color1, _Color2, colorT);
                color = float4(color.rgb, mask);
                return color;
            }
            ENDCG
        }
    }
}

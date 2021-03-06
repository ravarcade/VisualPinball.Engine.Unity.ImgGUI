#ifndef DEARIMGUI_BUILTIN_INCLUDED
#define DEARIMGUI_BUILTIN_INCLUDED

#include "UnityCG.cginc"
#include "Packages/org.visualpinball.engine.unity.imgui/Resources/Shaders/Common.hlsl"

sampler2D _Tex;

/*
 This was very simple function:
 half4 unpack_color(uint c)
 {
    half4 color = half4(
        (c      ) & 0xff,
        (c >>  8) & 0xff,
        (c >> 16) & 0xff,
        (c >> 24) & 0xff
    ) / 255;
    return color;
 }
 Now it looks like &^%$.
 Unity & conversion to Metal on macos creates compilation problems.
 */
half4 unpack_color(uint c)
{
    uint4 t = uint4(c, c, c, c);
    t.x &= 0xff;
    t.y -= t.x;
    t.y &= 0xffff;
    t.z -= t.x;
    t.z -= t.y;
    t.z &= 0xffffff;

    half4 color = t / uint4(0x1, 0x100, 0x10000, 0x1000000);        
    color /= 255;

#ifndef UNITY_COLORSPACE_GAMMA
    color.rgb = GammaToLinearSpace(color.rgb);
#endif
    return color;
}

Varyings ImGuiPassVertex(ImVert input)
{
    Varyings output  = (Varyings)0;
    output.vertex    = UnityObjectToClipPos(float4(input.vertex, 0, 1));
    output.uv        = float2(input.uv.x, 1 - input.uv.y);
    output.color     = unpack_color(input.color);
    return output;
}

half4 ImGuiPassFrag(Varyings input) : SV_Target
{
    return input.color * tex2D(_Tex, input.uv);
}

#endif

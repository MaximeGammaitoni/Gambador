using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RetroLookPro.Enums
{
    public enum BlendingMode
    {
        Normal = 0,
        Darken = 1,
        Multiply = 2,
        ColorBurn = 3,
        LinearBurn = 4,
        DarkerColor = 5,
        Lighten = 6,
        Screen = 7,
        ColorDodge = 8,
        LinearDodge = 9,
        LighterColor = 10,
        Overlay = 11,
        SoftLight = 12,
        HardLight = 13,
        VividLight = 14,
        LinearLight = 15,
        PinLight = 16,
        HardMix = 17,
        Difference = 18,
        Exclusion = 19,
        Subtract = 20,
        Divide = 21
    }
    public enum VignetteShape
    {
        circle,
        roundedCorners
    }
    public enum WarpMode
    {
        SimpleWarp,
        CubicDistortion
    }
}
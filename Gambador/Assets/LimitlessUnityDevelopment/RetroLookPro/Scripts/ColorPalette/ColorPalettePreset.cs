using UnityEngine;
namespace LimitlessDev.RetroLookPro
{
[System.Serializable]
public class ColorPalettePreset : ScriptableObject
{
    public effectPreset preset;
}
[System.Serializable]
public class effectPreset
{
    public string effectName;
    public int numberOfColors;
    public Color32[] palette;
    public bool changed = false;
    public Color32[] pixels;
}
}


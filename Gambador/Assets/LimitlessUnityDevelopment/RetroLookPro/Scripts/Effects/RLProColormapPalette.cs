using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using LimitlessDev.RetroLookPro;
[Serializable]
public sealed class resModeParameter : ParameterOverride<ResolutionMode> { };
[Serializable]
public sealed class Vector2IntParameter : ParameterOverride<Vector2Int> { };
[Serializable]
public sealed class preLParameter : ParameterOverride<effectPresets> { };
[Serializable]

[PostProcess(typeof(RLPRO_SRP_ColormapPaletteRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Colormap Palette", false)]
public sealed class RLProColormapPalette : PostProcessEffectSettings
{
    public resModeParameter resolutionMode = new resModeParameter { };
    [Range(1, 255), Tooltip("Change pixel size if using resolutionModeIndex - 1, in this case resolution calculates automatically. ")]
    public IntParameter pixelSize = new IntParameter { value = 1 };
    [Tooltip("Change Resolution manually if using resolutionModeIndex - 0.")]
    public Vector2IntParameter resolution = new Vector2IntParameter {value = new Vector2Int(640,480) };
    [Range(0f, 1f), Tooltip("Opacity.")]
    public FloatParameter opacity = new FloatParameter { value = 1f };
    [Range(0f, 1f), Tooltip("Dithering effect.")]
    public FloatParameter dither = new FloatParameter { value = 1f };
    public preLParameter presetsList = new preLParameter { };

    public IntParameter presetIndex = new IntParameter { value = 0 };
    
    public TextureParameter bluenoise = new TextureParameter { };
}

public sealed class RLPRO_SRP_ColormapPaletteRenderer : PostProcessEffectRenderer<RLProColormapPalette>
{
    public int tempPresetIndex = 0;
    private bool m_Init;
    Texture2D colormapPalette;
    Texture3D colormapTexture;
    public override void Init()
    {
        m_Init = true;
    }
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/ColorPalette"));

        if (m_Init || intHasChanged(tempPresetIndex, settings.presetIndex.value))
        {
            tempPresetIndex = settings.presetIndex.value;
            ApplyColormapToMaterial(sheet.properties);
            m_Init = false;
        }
        ApplyMaterialVariables(sheet.properties);

        RenderTexture scaled = RenderTexture.GetTemporary(settings.resolution.value.x, settings.resolution.value.y);
        scaled.filterMode = FilterMode.Point;
        context.command.BlitFullscreenTriangle(context.source, scaled, sheet, 0);
        context.command.BlitFullscreenTriangle(scaled, context.destination, sheet, 0);

        RenderTexture.ReleaseTemporary(scaled);
    }
    public void ApplyMaterialVariables(MaterialPropertyBlock bl)
    {
        settings.pixelSize.value = (int)Mathf.Clamp(settings.pixelSize, 1, float.MaxValue);
        if (settings.resolutionMode.value == ResolutionMode.ConstantPixelSize)
        {
            settings.resolution.value.x = Screen.width / settings.pixelSize;
            settings.resolution.value.y = Screen.height / settings.pixelSize;
        }

        settings.resolution.value.x = Mathf.Clamp(settings.resolution.value.x, 1, 16384);
        settings.resolution.value.y = Mathf.Clamp(settings.resolution.value.y, 1, 16384);

        settings.opacity.value = Mathf.Clamp01(settings.opacity);
        settings.dither.value = Mathf.Clamp01(settings.dither);

        bl.SetFloat("_Opacity", settings.opacity);
        bl.SetFloat("_Dither", settings.dither);
    }
    public void ApplyColormapToMaterial(MaterialPropertyBlock bl)
    {
        if (settings.presetsList.value != null)
        {
            if (settings.bluenoise.value != null)
                bl.SetTexture("_BlueNoise", settings.bluenoise);
            ApplyPalette(bl);
            ApplyMap(bl);
        }
    }
    void ApplyPalette(MaterialPropertyBlock bl)
    {
        colormapPalette = new Texture2D(256, 1, TextureFormat.RGB24, false);
        colormapPalette.filterMode = FilterMode.Point;
        colormapPalette.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < settings.presetsList.value.presetsList[settings.presetIndex].preset.numberOfColors; ++i)
        {
            colormapPalette.SetPixel(i, 0, settings.presetsList.value.presetsList[settings.presetIndex].preset.palette[i]);
        }

        colormapPalette.Apply();

        bl.SetTexture("_Palette", colormapPalette);
    }
    public void ApplyMap(MaterialPropertyBlock bl)
    {
        int colorsteps = 64;
        colormapTexture = new Texture3D(colorsteps, colorsteps, colorsteps, TextureFormat.RGB24, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        colormapTexture.SetPixels32(settings.presetsList.value.presetsList[settings.presetIndex].preset.pixels);
        colormapTexture.Apply();
        bl.SetTexture("_Colormap", colormapTexture);
    }
    public bool intHasChanged(int A, int B)
    {
        bool result = false;
        if (B != A)
        {
            A = B;
            result = true;
        }
        return result;
    }
}

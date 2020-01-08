using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;
[Serializable]
public sealed class WarpModeParameter : ParameterOverride<WarpMode> { };
[Serializable]
[PostProcess(typeof(RLPRO_SRP_TVEffect_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/TV Effect", false)]
public sealed class RLProTVEffect : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Dark areas adjustment.")]
    public FloatParameter fade = new FloatParameter { value = 1f };
    [Range(0f, 2f), Tooltip("Dark areas adjustment.")]
    public FloatParameter maskDark = new FloatParameter { value = 0.5f };
        [Range(0f, 2f), Tooltip("Light areas adjustment.")]
    public FloatParameter maskLight = new FloatParameter { value = 1.5f };
        [Range(-8f, -16f), Tooltip("Dark areas fine tune.")]
    public FloatParameter hardScan = new FloatParameter { value = -8f };
        [Range(1f, 16f), Tooltip("Effect resolution.")]
    public FloatParameter resScale = new FloatParameter { value = 4f };
            [Range(-3f, 1f), Tooltip("pixels sharpness.")]
    public FloatParameter hardPix = new FloatParameter { value = -3f };
    [Tooltip("Warp mode.")]
    public WarpModeParameter warpMode = new WarpModeParameter {};
    [Tooltip("Warp picture.")]
    public Vector2Parameter warp = new Vector2Parameter {value = new Vector2(0.03125f,0.04166f)};
public FloatParameter scale = new FloatParameter { value = 1f };
}

public sealed class RLPRO_SRP_TVEffect_Renderer : PostProcessEffectRenderer<RLProTVEffect>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/TV_RetroLook"));
        sheet.properties.SetFloat("fade", settings.fade);
        sheet.properties.SetFloat("scale", settings.scale);
        sheet.properties.SetFloat("hardScan", settings.hardScan);
        sheet.properties.SetFloat("hardPix", settings.hardPix);
        sheet.properties.SetFloat("resScale", settings.resScale);
        sheet.properties.SetFloat("maskDark", settings.maskDark);
        sheet.properties.SetFloat("maskLight", settings.maskLight);
        sheet.properties.SetVector("warp", settings.warp);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.warpMode == WarpMode.SimpleWarp?0:1);
    }
}

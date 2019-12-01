using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;
[Serializable]
public sealed class VignetteModeParameter : ParameterOverride<VignetteShape> { };
[Serializable]
[PostProcess(typeof(RLProUltimateVignetteRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Ultimate Vignette", false)]
public sealed class RLProUltimateVignette : PostProcessEffectSettings
{
    public VignetteModeParameter vignetteShape = new VignetteModeParameter { };
    [Tooltip(".")]
    public Vector2Parameter center = new Vector2Parameter { value = new Vector2(0.5f, 0.5f) };

    [Range(0f, 100), Tooltip(".")]
    public FloatParameter vignetteAmount = new FloatParameter { value = 50f };
    [Range(-1f, -100f), Tooltip(".")]
    public FloatParameter vignetteFineTune = new FloatParameter { value = -10f };
    [Range(0f, 100f), Tooltip("Scanlines width.")]
    public FloatParameter edgeSoftness = new FloatParameter { value = 1.5f };
    [Range(200f, 0f), Tooltip("Horizontal/Vertical scanlines.")]
    public FloatParameter edgeBlend = new FloatParameter { value = 0f };
    [Range(0f, 200f), Tooltip(".")]
    public FloatParameter innerColorAlpha = new FloatParameter { value = 0f };
    public ColorParameter innerColor = new ColorParameter { };
}

public sealed class RLProUltimateVignetteRenderer : PostProcessEffectRenderer<RLProUltimateVignette>
{
    private float m_Horizontal;
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/UltimateVignette_RLPro"));
        sheet.DisableKeyword("VIGNETTE_CIRCLE");
        sheet.DisableKeyword("VIGNETTE_ROUNDEDCORNERS");
        switch (settings.vignetteShape.value)
        {
            case VignetteShape.circle:
                sheet.EnableKeyword("VIGNETTE_CIRCLE");
                break;
            case VignetteShape.roundedCorners:
                sheet.EnableKeyword("VIGNETTE_ROUNDEDCORNERS");
                break;
        }
        sheet.properties.SetVector("_Params", new Vector4(settings.edgeSoftness * 0.01f, settings.vignetteAmount * 0.02f, settings.innerColorAlpha * 0.01f, settings.edgeBlend * 0.01f));
        sheet.properties.SetColor("_InnerColor", settings.innerColor);
        sheet.properties.SetVector("_Center", settings.center);
        sheet.properties.SetVector("_Params1", new Vector2(settings.vignetteFineTune, 0.8f));

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

    }
}
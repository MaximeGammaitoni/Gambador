using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public enum FisheyeTypeEnum { Default = 0, Hyperspace = 1 }
[Serializable]
public sealed class FisheyeTypeParameter : ParameterOverride<FisheyeTypeEnum> { };
[Serializable]
[PostProcess(typeof(RLPRO_SRP_FisheyeRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Fisheye Vignette Effect", false)]
public sealed class RLProFisheye : PostProcessEffectSettings
{
    public FisheyeTypeParameter fisheyeType = new FisheyeTypeParameter { };
    [Range(0f, 50f), Tooltip("Bend Amount.")]
    public FloatParameter bend = new FloatParameter { value = 1f };
    [Range(0f, 50f), Tooltip("Cutoff on X axes.")]
    public FloatParameter cutOffX = new FloatParameter { value = 0.5f };
    [Range(0f, 50f), Tooltip("Cutoff on Y axes.")]
    public FloatParameter cutOffY = new FloatParameter { value = 0.5f };
    [Range(0f, 50f), Tooltip("Fade on X axes.")]
    public FloatParameter fadeX = new FloatParameter { value = 1f };
    [Range(0f, 50f), Tooltip("Fade on Y axes.")]
    public FloatParameter fadeY = new FloatParameter { value = 1f };
    [Range(0.001f, 50f), Tooltip("Fisheye size.")]
    public FloatParameter size = new FloatParameter { value = 1f };
}

public sealed class RLPRO_SRP_FisheyeRenderer : PostProcessEffectRenderer<RLProFisheye>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Fisheye"));

        ParamSwitch(sheet, true, "VHS_FISHEYE_ON");
        sheet.properties.SetFloat("cutoffX", settings.cutOffX);
        sheet.properties.SetFloat("cutoffY", settings.cutOffY);
        sheet.properties.SetFloat("cutoffFadeX", settings.fadeX);
        sheet.properties.SetFloat("cutoffFadeY", settings.fadeY);
        ParamSwitch(sheet, settings.fisheyeType.value == FisheyeTypeEnum.Hyperspace, "VHS_FISHEYE_HYPERSPACE");
        sheet.properties.SetFloat("fisheyeBend", settings.bend);
        sheet.properties.SetFloat("fisheyeSize", settings.size);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }
}
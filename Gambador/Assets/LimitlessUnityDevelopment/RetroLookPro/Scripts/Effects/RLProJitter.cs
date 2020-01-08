using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_JitterRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Jitter Effect", false)]
public sealed class RLProJitter : PostProcessEffectSettings
{
    [Tooltip("Enable Twitch on X axes.")]
    public BoolParameter twitchHorizontal = new BoolParameter { value = false };
    [Range(0f, 5f), Tooltip("Twitch frequency on X axes.")]
    public FloatParameter horizontalFreq = new FloatParameter { value = 1f };
    [Space]
    [Tooltip("Enable Twitch on Y axes.")]
    public BoolParameter twitchVertical = new BoolParameter { value = false };
    [Range(0f, 5f), Tooltip("Twitch frequency on Y axes.")]
    public FloatParameter verticalFreq = new FloatParameter { value = 1f };
    [Space]
    [Tooltip("Glitch1 effect intensity.")]
    public BoolParameter stretch = new BoolParameter { value = false };
    [Tooltip("Glitch1 effect intensity.")]
    public FloatParameter stretchResolution = new FloatParameter { value = 1f };
    [Space]
    [Tooltip("Enable Horizontal Interlacing.")]
    public BoolParameter jitterHorizontal = new BoolParameter { value = false };
    [Range(0f, 5f), Tooltip("Amount of horizontal interlacing.")]
    public FloatParameter jitterHorizontalAmount = new FloatParameter { value = 1f };
    [Space]
    [Tooltip("Shake Vertical.")]
    public BoolParameter jitterVertical = new BoolParameter { value = false };
    [Range(0f, 15f), Tooltip("Amount of shake.")]
    public FloatParameter jitterVerticalAmount = new FloatParameter { value = 1f };
    [Range(0f, 15f), Tooltip("Speed of vertical shake. ")]
    public FloatParameter jitterVerticalSpeed = new FloatParameter { value = 1f };
    [Space]
    [Tooltip("Time.unscaledTime .")]
    public BoolParameter unscaledTime = new BoolParameter { value = false };
}

public sealed class RLPRO_SRP_JitterRenderer : PostProcessEffectRenderer<RLProJitter>
{
    private float _time;
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/JitterEffect"));

        if (settings.unscaledTime) { _time = Time.unscaledTime; }
        else _time = Time.time;

        sheet.properties.SetFloat("screenLinesNum", settings.stretchResolution);
        sheet.properties.SetFloat("time_", _time);
        ParamSwitch(sheet, settings.twitchHorizontal, "VHS_TWITCH_H_ON");
        sheet.properties.SetFloat("twitchHFreq", settings.horizontalFreq);
        ParamSwitch(sheet, settings.twitchVertical, "VHS_TWITCH_V_ON");
        sheet.properties.SetFloat("twitchVFreq", settings.verticalFreq);
        ParamSwitch(sheet, settings.stretch, "VHS_STRETCH_ON");

        ParamSwitch(sheet, settings.jitterHorizontal, "VHS_JITTER_H_ON");
        sheet.properties.SetFloat("jitterHAmount", settings.jitterHorizontalAmount);

        ParamSwitch(sheet, settings.jitterVertical, "VHS_JITTER_V_ON");
        sheet.properties.SetFloat("jitterVAmount", settings.jitterVerticalAmount);
        sheet.properties.SetFloat("jitterVSpeed", settings.jitterVerticalSpeed);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }
}

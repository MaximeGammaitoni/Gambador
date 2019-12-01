using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_NoiseRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Noise", false)]
public sealed class RLProNoise : PostProcessEffectSettings
{
    [DisplayName("Resolution"), Tooltip(".")]
    public FloatParameter stretchResolution = new FloatParameter { value = 480f };

    [DisplayName("Vertical Resolution"), Tooltip(".")]
    public FloatParameter n_NoiseLinesAmountY = new FloatParameter { value = 480f };
    [Space]
    [Space]

    [DisplayName("Granularity"), Tooltip(".")]
    public BoolParameter f_Granularity = new BoolParameter { value = false };
    [DisplayName("Granularity Amount"), Range(0f, 0.5f), Tooltip("")]
    public FloatParameter f_GranularityAmount = new FloatParameter { value = 1f };
    [Space]
    [DisplayName("Tape Noise"), Tooltip(".")]
    public BoolParameter f_TapeNoise = new BoolParameter { value = false };
    [DisplayName("Tape Noise processing value"), Range(0f, 15f), Tooltip(" .")]
    public FloatParameter n_NoiseSignalProcessing = new FloatParameter { value = 1f };
    [DisplayName("Tape Noise Amount"), Range(0f, 1.5f), Tooltip(" .")]
    public FloatParameter f_TapeNoiseAmount = new FloatParameter { value = 1f };
    [DisplayName("Tape Noise Power"), Range(0f, 1.5f), Tooltip(" .")]
    public FloatParameter f_TapeNoiseTH = new FloatParameter { value = 1f };
    [DisplayName("Tape Noise Speed"), Range(-1.5f, 1.5f), Tooltip(" .")]
    public FloatParameter f_TapeNoiseSpeed = new FloatParameter { value = 0.5f };
    [Space]
    [DisplayName("Line Noise"), Tooltip(" .")]
    public BoolParameter f_LineNoise = new BoolParameter { value = false };
    [DisplayName("Line Noise Amount"), Range(0f, 15f), Tooltip(" .")]
    public FloatParameter f_LineNoiseAmount = new FloatParameter { value = 1f };
    [DisplayName("Line Noise Speed"), Range(0f, 10f), Tooltip(" .")]
    public FloatParameter f_LineNoiseSpeed = new FloatParameter { value = 1f };
    [Space]
    [DisplayName("Signal Noise"), Tooltip(" .")]
    public BoolParameter f_SignalNoise = new BoolParameter { value = false };
    [DisplayName("Signal Noise Power"), Range(0.5f, 0.97f), Tooltip(" .")]
    public FloatParameter f_SignalNoisePower = new FloatParameter { value = 0.9f };

    [DisplayName("Signal Noise Amount"), Range(0f, 2f), Tooltip(" .")]
    public FloatParameter f_SignalNoiseAmount = new FloatParameter { value = 1f };
    [Space]
    [Tooltip("Time.unscaledTime.")]
    public BoolParameter unscaledTime = new BoolParameter { value = false };
}

public sealed class RLPRO_SRP_NoiseRenderer : PostProcessEffectRenderer<RLProNoise>
{
    private float _time;
    private RenderTexture texTape;
    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Noise"));

        if (settings.unscaledTime) { _time = Time.unscaledTime; }
        else _time = Time.time;


        float screenLinesNum_ = settings.stretchResolution;
        if (screenLinesNum_ <= 0) screenLinesNum_ = context.screenHeight;

        if (texTape == null || (texTape.height != Mathf.Min(settings.n_NoiseLinesAmountY, screenLinesNum_)))
        {
            int texHeight = (int)Mathf.Min(settings.n_NoiseLinesAmountY, screenLinesNum_);
            int texWidth = (int)(
                  (float)texHeight * (float)context.screenWidth / (float)context.screenHeight);

#if UNITY_EDITOR
             UnityEngine.Object.DestroyImmediate(texTape);
#else
            UnityEngine.Object.Destroy(texTape);
#endif
            texTape = new RenderTexture(texWidth, texHeight, 0);
            texTape.hideFlags = HideFlags.HideAndDontSave;
            texTape.filterMode = FilterMode.Point;
            texTape.Create();
            context.command.BlitFullscreenTriangle(context.source, texTape, sheet, 0);
        }
        sheet.properties.SetFloat("time_", _time);
        sheet.properties.SetFloat("screenLinesNum", screenLinesNum_);
        sheet.properties.SetFloat("noiseLinesNum", settings.n_NoiseLinesAmountY);
        sheet.properties.SetFloat("noiseQuantizeX", settings.n_NoiseSignalProcessing);
        ParamSwitch(sheet, settings.f_Granularity, "VHS_FILMGRAIN_ON");
        ParamSwitch(sheet, settings.f_TapeNoise, "VHS_TAPENOISE_ON");
        ParamSwitch(sheet, settings.f_LineNoise, "VHS_LINENOISE_ON");
        ParamSwitch(sheet, settings.f_SignalNoise, "VHS_YIQNOISE_ON");

        sheet.properties.SetFloat("signalNoisePower", settings.f_SignalNoisePower);
        sheet.properties.SetFloat("signalNoiseAmount", settings.f_SignalNoiseAmount);

        var sheet1 = context.propertySheets.Get(Shader.Find("RetroLookPro/Noise2"));

        sheet1.properties.SetFloat("time_", _time);
        ParamSwitch(sheet1, settings.f_Granularity, "VHS_FILMGRAIN_ON");
        sheet1.properties.SetFloat("filmGrainAmount", settings.f_GranularityAmount);
        ParamSwitch(sheet1, settings.f_TapeNoise, "VHS_TAPENOISE_ON");
        sheet1.properties.SetFloat("tapeNoiseTH", settings.f_TapeNoiseTH);
        sheet1.properties.SetFloat("tapeNoiseAmount", settings.f_TapeNoiseAmount);
        sheet1.properties.SetFloat("tapeNoiseSpeed", settings.f_TapeNoiseSpeed);
        ParamSwitch(sheet1, settings.f_LineNoise, "VHS_LINENOISE_ON");
        sheet1.properties.SetFloat("lineNoiseAmount", settings.f_LineNoiseAmount);
        sheet1.properties.SetFloat("lineNoiseSpeed", settings.f_LineNoiseSpeed);
        context.command.BlitFullscreenTriangle(texTape, texTape, sheet1, 0);
        sheet.properties.SetTexture("_TapeTex", texTape);
        sheet.properties.SetFloat("tapeNoiseAmount", settings.f_TapeNoiseAmount);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }
}
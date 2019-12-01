using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
[Serializable]
[PostProcess(typeof(RLProArtefactsRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Artefacts Effect", false)]
public sealed class RLProArtefacts : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Brightness threshold of input.")]
    public FloatParameter cutOff = new FloatParameter { value = 0.5f };
    [Range(0f, 3f), Tooltip("Amplifies the input amount after cutoff.")]
    public FloatParameter amount = new FloatParameter { value = 1f };
    [Range(0f, 1f), Tooltip("Value represents how fast trail fades.")]
    public FloatParameter fade = new FloatParameter { value = 0.5f };
    [Tooltip("Artefacts color.")]
    public ColorParameter color = new ColorParameter { };
    [Tooltip("Render Artefacts only.")]
    public BoolParameter debugArtefacts = new BoolParameter { value = false };
}

public sealed class RLProArtefactsRenderer : PostProcessEffectRenderer<RLProArtefacts>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/ArtefactsEffect"));
        RenderTexture texLast = context.GetScreenSpaceTemporaryRT();
        RenderTexture texfeedback = context.GetScreenSpaceTemporaryRT();
        RenderTexture texfeedback2 = context.GetScreenSpaceTemporaryRT();
        sheet.properties.SetTexture("_LastTex", texLast);
        sheet.properties.SetTexture("_FeedbackTex", texfeedback);
        sheet.properties.SetFloat("feedbackThresh", settings.cutOff);
        sheet.properties.SetFloat("feedbackAmount", settings.amount);
        sheet.properties.SetFloat("feedbackFade", settings.fade);
        sheet.properties.SetColor("feedbackColor", settings.color);
        context.command.BlitFullscreenTriangle(context.source, texfeedback2, sheet, 0);
        context.command.BlitFullscreenTriangle(texfeedback2, texfeedback);
        var sheet1 = context.propertySheets.Get(Shader.Find("RetroLookPro/ArtefactsEffectSecond"));
        sheet1.properties.SetFloat("feedbackAmp", 1.0f);
        sheet1.properties.SetTexture("_FeedbackTex", texfeedback);

        context.command.BlitFullscreenTriangle(context.source, texLast, sheet1, 0);

        if (!settings.debugArtefacts)
            context.command.BlitFullscreenTriangle(texLast, context.destination);
        else
            context.command.BlitFullscreenTriangle(texfeedback, context.destination);

        RenderTexture.ReleaseTemporary(texLast);
        RenderTexture.ReleaseTemporary(texfeedback);
        RenderTexture.ReleaseTemporary(texfeedback2);
    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }
}
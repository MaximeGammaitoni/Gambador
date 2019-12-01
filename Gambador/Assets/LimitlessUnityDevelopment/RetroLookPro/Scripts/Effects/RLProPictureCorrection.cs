using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_PictureCorrectionRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Picture Correction", false)]
public sealed class RLProPictureCorrection : PostProcessEffectSettings
{


    [Range(-0.25f, 0.25f), Tooltip(" Y permanent adjustment..")]
    public FloatParameter signalAdjustY = new FloatParameter { value = 0f };
        [Range(-0.25f, 0.25f), Tooltip("I permanent adjustment..")]
    public FloatParameter signalAdjustI = new FloatParameter { value = 0f };
        [Range(-0.25f, 0.25f), Tooltip("Q permanent adjustment..")]
    public FloatParameter signalAdjustQ = new FloatParameter { value = 0f };
        [Range(-2f, 2f), Tooltip("tweak/shift Y values..")]
    public FloatParameter signalShiftY = new FloatParameter { value = 1f };
        [Range(-2f, 2f), Tooltip("tweak/shift I values..")]
    public FloatParameter signalShiftI = new FloatParameter { value = 1f };
        [Range(-2f, 2f), Tooltip("tweak/shift Q values..")]
    public FloatParameter signalShiftQ = new FloatParameter { value = 1f };
            [Range(0f, 2f), Tooltip("use this to balance the gamma(brightness) of the signal.")]
    public FloatParameter gammaCorection = new FloatParameter { value = 1f };
}

public sealed class RLPRO_SRP_PictureCorrectionRenderer : PostProcessEffectRenderer<RLProPictureCorrection>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/PictureCorrection"));

            sheet.properties.SetFloat("signalAdjustY", settings.signalAdjustY);
            sheet.properties.SetFloat("signalAdjustI", settings.signalAdjustI);
            sheet.properties.SetFloat("signalAdjustQ", settings.signalAdjustQ);
            sheet.properties.SetFloat("signalShiftY", settings.signalShiftY);
            sheet.properties.SetFloat("signalShiftI", settings.signalShiftI);
            sheet.properties.SetFloat("signalShiftQ", settings.signalShiftQ);
            sheet.properties.SetFloat("gammaCorection", settings.gammaCorection);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}


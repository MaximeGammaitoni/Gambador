using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(RLPRO_SRP_BottomStretch_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Bottom Stretch Effect", false)]
public sealed class RLProBottomStretch : PostProcessEffectSettings
{
    [Range(0.01f, 0.5f), Tooltip("Height of Noise.")]
    public FloatParameter height = new FloatParameter { value = 0.2f };

    [Space]
            [ Tooltip(".")]
    public BoolParameter distort = new BoolParameter { value = true };
        [Range(100f, 0.1f), Tooltip(".")]
    public FloatParameter frequency = new FloatParameter { value = 0.2f };
        [Range(0.01f, 200f), Tooltip(".")]
    public FloatParameter amplitude = new FloatParameter { value = 0.2f };

            [ Tooltip(".")]
    public BoolParameter distortRandomly = new BoolParameter { value = true };

}
public sealed class RLPRO_SRP_BottomStretch_Renderer : PostProcessEffectRenderer<RLProBottomStretch>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/BottomStretchEffect"));
        sheet.properties.SetFloat("_NoiseBottomHeight", settings.height);
        sheet.properties.SetFloat("frequency", settings.frequency);
        sheet.properties.SetFloat("amplitude", settings.amplitude);
        if(settings.distort){
if(settings.distortRandomly)
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 1);
        else
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
        else
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 2);
    }
}

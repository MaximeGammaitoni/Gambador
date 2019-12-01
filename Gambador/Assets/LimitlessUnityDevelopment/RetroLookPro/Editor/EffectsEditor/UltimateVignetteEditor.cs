using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

namespace UnityEditor.Rendering.PostProcessing
{
    [PostProcessEditor(typeof(RLProUltimateVignette))]
    internal sealed class UltimateVignetteEditor : PostProcessEffectEditor<RLProUltimateVignette>
    {

        SerializedParameterOverride vignetteShape;
        SerializedParameterOverride center;
        SerializedParameterOverride vignetteAmount;
        SerializedParameterOverride vignetteFineTune;
        SerializedParameterOverride sharpness;
        SerializedParameterOverride edgeBlend;
        SerializedParameterOverride innerColorAlpha;
        SerializedParameterOverride innerColor;

        bool laal;

        public override void OnEnable()
        {
            vignetteShape = FindParameterOverride(x => x.vignetteShape);
            center = FindParameterOverride(x => x.center);
            vignetteAmount = FindParameterOverride(x => x.vignetteAmount);
            vignetteFineTune = FindParameterOverride(x => x.vignetteFineTune);
            sharpness = FindParameterOverride(x => x.edgeSoftness);
            edgeBlend = FindParameterOverride(x => x.edgeBlend);
            innerColorAlpha = FindParameterOverride(x => x.innerColorAlpha);
            innerColor = FindParameterOverride(x => x.innerColor);

        }

        public override void OnInspectorGUI()
        {

            PropertyField(vignetteShape);
            if(vignetteShape.value.intValue == 0)
            PropertyField(center);



                PropertyField(vignetteAmount);
                if(vignetteShape.value.intValue == 1)
                PropertyField(vignetteFineTune);

                PropertyField(sharpness);

                PropertyField(edgeBlend);
                PropertyField(innerColorAlpha);

                PropertyField(innerColor);




        }
    }
}

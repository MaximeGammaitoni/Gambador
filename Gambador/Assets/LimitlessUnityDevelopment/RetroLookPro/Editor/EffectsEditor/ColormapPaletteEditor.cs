using UnityEngine;
using LimitlessDev.RetroLookPro;

namespace UnityEditor.Rendering.PostProcessing
{
    [PostProcessEditor(typeof(RLProColormapPalette))]
    internal sealed class ColormapPaletteEditor : PostProcessEffectEditor<RLProColormapPalette>
    {
        SerializedParameterOverride resolutionMode;
        SerializedParameterOverride pixelSize;
        SerializedParameterOverride resolution;
        SerializedParameterOverride opacity;
        SerializedParameterOverride dither;
        SerializedParameterOverride presetsList;
        SerializedParameterOverride presetsList1;
        SerializedParameterOverride presetIndex;
        SerializedParameterOverride blueNoise;
        string[] palettePresets;
        bool m_InfoFold;
        public override void OnEnable()
        {
            resolutionMode = FindParameterOverride(x => x.resolutionMode);
            pixelSize = FindParameterOverride(x => x.pixelSize);
            resolution = FindParameterOverride(x => x.resolution);
            opacity = FindParameterOverride(x => x.opacity);
            dither = FindParameterOverride(x => x.dither);
            blueNoise = FindParameterOverride(x => x.bluenoise);
            presetsList = FindParameterOverride(x => x.presetsList);
            presetIndex = FindParameterOverride(x => x.presetIndex);

            string[] paths = AssetDatabase.FindAssets("RetroLookProColorPaletePresetsList");
            string assetpath = AssetDatabase.GUIDToAssetPath(paths[0]);
            effectPresets tempPreset = (effectPresets)AssetDatabase.LoadAssetAtPath(assetpath, typeof(effectPresets));

            palettePresets = new string[tempPreset.presetsList.Count];
            for (int i = 0; i < palettePresets.Length; i++)
            {
                palettePresets[i] = tempPreset.presetsList[i].preset.effectName;
            }
        }

        public override void OnInspectorGUI()
        {
            if (presetsList.value.objectReferenceValue == null)
            {
                string[] efListPaths = AssetDatabase.FindAssets("RetroLookProColorPaletePresetsList");
                string efListPath = AssetDatabase.GUIDToAssetPath(efListPaths[0]);
                presetsList.value.objectReferenceValue = (effectPresets)AssetDatabase.LoadAssetAtPath(efListPath, typeof(effectPresets));
                presetsList.value.serializedObject.ApplyModifiedProperties();

                EditorGUILayout.HelpBox("Please insert Retro Look Pro Color Palete Presets List.", MessageType.Info);
                PropertyField(presetsList);
            }

            if (blueNoise.value.objectReferenceValue == null)
            {
                blueNoise.value.objectReferenceValue = Resources.Load("Noise Textures/blue_noise") as Texture2D;
                PropertyField(blueNoise);
            }
            presetIndex.value.intValue = EditorGUILayout.Popup("Color Preset", presetIndex.value.intValue, palettePresets);
            PropertyField(resolutionMode);

            if (resolutionMode.value.intValue == (int)ResolutionMode.ConstantPixelSize)
                PropertyField(pixelSize);
            else
                PropertyField(resolution);


            PropertyField(opacity);
            PropertyField(dither);
            presetIndex.overrideState.boolValue = true;
            presetsList.overrideState.boolValue = true;
        }
    }
}
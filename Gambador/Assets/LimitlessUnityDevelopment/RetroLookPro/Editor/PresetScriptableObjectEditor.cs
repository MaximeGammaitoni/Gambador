using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
namespace LimitlessDev.RetroLookPro
{
[CustomEditor(typeof(PresetScriptableObject))]
[CanEditMultipleObjects]
public class PresetScriptableObjectEditor : Editor
{
    public AnimBool[] animFolds;
    private bool fold;
    private string[] palettePresets;
    private Texture texture;
    //Group foldout
    SerializedProperty f_show_Bleed;
    SerializedProperty f_show_Fisheye;
    SerializedProperty f_show_Vignette;
    SerializedProperty f_show_Noise;
    SerializedProperty f_show_Bottom_Noise_mode;
    SerializedProperty f_show_Jitter;
    SerializedProperty f_show_Signal;
    SerializedProperty f_show_Artefacts;
    SerializedProperty f_show_Bypass;
    SerializedProperty f_show_TV_mode;
    SerializedProperty f_show_Old_TV_mode;
    SerializedProperty f_show_Glitch_Effects;
    //Bleed
    SerializedProperty b_Bleed;
    SerializedProperty b_Mode;
    SerializedProperty b_LinesMode;
    SerializedProperty b_ScreenLinesNum;
    SerializedProperty b_BleedAmount;
    SerializedProperty b_BleedCurve1;
    SerializedProperty b_BleedCurve2;
    SerializedProperty b_BleedCurve3;
    SerializedProperty b_BleedCurveSync;
    SerializedProperty b_BleedLength;
    SerializedProperty b_BleedDebug;
    //Fisheye
    SerializedProperty f_Fisheye;
    SerializedProperty f_FisheyeBend;
    SerializedProperty f_FisheyeType;
    SerializedProperty f_CutoffX;
    SerializedProperty f_CutoffY;
    SerializedProperty f_FadeX;
    SerializedProperty f_FadeY;
    //Vignette
    SerializedProperty v_Vignette;
    SerializedProperty v_VignetteAmount;
    SerializedProperty v_VignetteSpeed;
    //Noise
    //SerializedProperty n_enableNoise;
    SerializedProperty n_NoiseMode;
    SerializedProperty n_NoiseLinesAmountY;
    SerializedProperty n_NoiseSignalProcessing;

    SerializedProperty f_TapeNoise;
    SerializedProperty f_TapeNoiseTH;
    SerializedProperty f_TapeNoiseAmount;
    SerializedProperty f_TapeNoiseSpeed;
    SerializedProperty f_LineNoise;
    SerializedProperty f_LineNoiseAmount;
    SerializedProperty f_LineNoiseSpeed;
    //Granularity
    SerializedProperty f_Granularity;
    SerializedProperty f_GranularityAmount;
    //Jitter
    //SerializedProperty j_enableJitter;
    SerializedProperty j_ScanLines;
    SerializedProperty j_ScanLinesWidth;
    SerializedProperty j_LinesFloat;
    SerializedProperty j_LinesSpeed;
    SerializedProperty j_Stretch;
    SerializedProperty j_TwitchHorizontal;
    SerializedProperty j_TwitchHorizFreq;
    SerializedProperty j_TwitchVertical;
    SerializedProperty j_TwitchVertFreq;
    SerializedProperty j_JitterHorizontal;
    SerializedProperty j_JitterHorizAmount;
    SerializedProperty j_JitterVertical;
    SerializedProperty j_VertAmount;
    SerializedProperty j_VertSpeed;
    //
    // Glitch Effects
    SerializedProperty g_enable_glitch;
    SerializedProperty g_strength;
    SerializedProperty g_fade;
    SerializedProperty g_speed;
            //Glitch2
        SerializedProperty g_enable_glitch2;
        SerializedProperty g_2intensity;
        SerializedProperty g_2resolution;
        SerializedProperty g_2speed;
        //
    //
    //Picture correction
    SerializedProperty p_PictureCorrection;
    SerializedProperty p_PictureCorr1;
    SerializedProperty p_PictureCorr2;
    SerializedProperty p_PictureCorr3;
    SerializedProperty p_PictureShift1;
    SerializedProperty p_PictureShift2;
    SerializedProperty p_PictureShift3;
    SerializedProperty f_SignalNoise;
    SerializedProperty f_SignalNoiseAmount;
    SerializedProperty f_SignalNoisePower;
    SerializedProperty p_Gamma;
    //Artefacts
    SerializedProperty a_Artefacts;
    SerializedProperty a_ArtefactsAmount;
    SerializedProperty a_ArtefactsFadeAmount;
    SerializedProperty a_ArtefactsColor;
    SerializedProperty a_ArtefactsThreshold;
    SerializedProperty a_ArtefactsDebug;
    //Time
    SerializedProperty independentTimeOn;
    //BYPASS texture
    SerializedProperty enableCustomTexture;
    SerializedProperty bypassTex;
    SerializedProperty spriteTex;
    //TV mode properties
    SerializedProperty _enableTVmode;
    SerializedProperty _VHSNoise;
    SerializedProperty _textureIntensity;
    SerializedProperty _VerticalOffsetFrequency;
    SerializedProperty _verticalOffset;
    SerializedProperty _offsetColor;
    SerializedProperty _OffsetDistortion;
    SerializedProperty _scan;
    SerializedProperty _adjustLines;
    SerializedProperty _scanLinesColor;
    SerializedProperty _hardScan;
    SerializedProperty _resolution;
    SerializedProperty maskDark;
    SerializedProperty maskLight;
    SerializedProperty warp;
    //Old Tv props
    SerializedProperty o_Enable_negative_props;
    SerializedProperty o_Enable_Old_TV_effects;
    SerializedProperty o_Luminosity;
    SerializedProperty o_Negative;
    SerializedProperty o_Vignette;
    SerializedProperty o_FPS;
    SerializedProperty o_contrast;
    SerializedProperty o_burn;
    SerializedProperty o_sceneCut;
    SerializedProperty o_Fade;
    //
    //colorPalette
    SerializedProperty resolutionMode;
    SerializedProperty resolutionModeIndex;
    SerializedProperty resolution;
    SerializedProperty pixelSize;
    SerializedProperty dither;
    SerializedProperty opacity;
    SerializedProperty colorPresetIndex;
    SerializedProperty f_show_colorPalette;
    SerializedProperty enableColorPalette;

    //Bottom noise
    SerializedProperty b_BottomNoiseTexture;
    SerializedProperty b_useBottomNoise;
    SerializedProperty b_bottomHeight;
    SerializedProperty b_bottomIntensity;
    SerializedProperty b_useBottomStretch;
    //
    //SCriptable object reference
    SerializedObject so;
    SerializedProperty pre;
    //Info toggles
    bool JitterInfo = true;

    void OnEnable()
    {
        so = new SerializedObject(target); //serializedObject
        pre = so.FindProperty("currPreset");

        string[] paths = AssetDatabase.FindAssets("RetroLookProColorPaletePresetsList");
        string assetpath = AssetDatabase.GUIDToAssetPath(paths[0]);
        effectPresets tempPreset = (effectPresets)AssetDatabase.LoadAssetAtPath(assetpath, typeof(effectPresets));

        string[] texturePaths = AssetDatabase.FindAssets("RLProAssetIMG");
        string texturePath = AssetDatabase.GUIDToAssetPath(texturePaths[0]);
        texture = (Texture)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture));

        palettePresets = new string[tempPreset.presetsList.Count];
        for (int i = 0; i < palettePresets.Length; i++)
        {
            palettePresets[i] = tempPreset.presetsList[i].preset.effectName;
        }

        animFolds = new AnimBool[13];
        for (int i = 0; i < animFolds.Length; i++)
        {
            animFolds[i] = new AnimBool();
            animFolds[i].valueChanged.AddListener(Repaint);
        }

        //pre = new SerializedObject(target)
        // Preset lalal = so.FindProperty("currPreset");
        b_useBottomNoise = pre.FindPropertyRelative("useBottomNoise");
        b_BottomNoiseTexture = pre.FindPropertyRelative("_BottomNoiseTexture");
        b_bottomHeight = pre.FindPropertyRelative("bottomHeight");
        b_bottomIntensity = pre.FindPropertyRelative("bottomIntensity");
        b_useBottomStretch = pre.FindPropertyRelative("useBottomStretch");

        _enableTVmode = pre.FindPropertyRelative("_enableTVmode");
        _VHSNoise = pre.FindPropertyRelative("_VHSNoise");
        _textureIntensity = pre.FindPropertyRelative("_textureIntensity");
        _VerticalOffsetFrequency = pre.FindPropertyRelative("_VerticalOffsetFrequency");
        _verticalOffset = pre.FindPropertyRelative("_verticalOffset");
        _offsetColor = pre.FindPropertyRelative("_offsetColor");
        _OffsetDistortion = pre.FindPropertyRelative("_OffsetDistortion");
        _scan = pre.FindPropertyRelative("_scan");
        _adjustLines = pre.FindPropertyRelative("_adjustLines");
        _scanLinesColor = pre.FindPropertyRelative("_scanLinesColor");

        _hardScan = pre.FindPropertyRelative("_hardScan");
        _resolution = pre.FindPropertyRelative("_resolution");
        maskDark = pre.FindPropertyRelative("maskDark");
        maskLight = pre.FindPropertyRelative("maskLight");
        warp = pre.FindPropertyRelative("warp");
        //
        o_Enable_negative_props = pre.FindPropertyRelative("o_Enable_negative_props");
        o_Enable_Old_TV_effects = pre.FindPropertyRelative("o_Enable_Old_TV_effects");
        o_Luminosity = pre.FindPropertyRelative("o_Luminosity");
        o_Negative = pre.FindPropertyRelative("o_Negative");
        o_Vignette = pre.FindPropertyRelative("o_Vignette");
        o_FPS = pre.FindPropertyRelative("o_FPS");
        o_contrast = pre.FindPropertyRelative("o_contrast");
        o_burn = pre.FindPropertyRelative("o_burn");
        o_sceneCut = pre.FindPropertyRelative("o_sceneCut");
        o_Fade = pre.FindPropertyRelative("o_Fade");
        //
        f_show_Bottom_Noise_mode = pre.FindPropertyRelative("f_show_Bottom_Noise_mode");
        f_show_TV_mode = pre.FindPropertyRelative("f_show_TV_mode");
        f_show_Old_TV_mode = pre.FindPropertyRelative("f_show_Old_TV_mode");
        f_show_Bleed = pre.FindPropertyRelative("f_show_Bleed");
        f_show_Fisheye = pre.FindPropertyRelative("f_show_Fisheye");
        f_show_Vignette = pre.FindPropertyRelative("f_show_Vignette");
        f_show_Noise = pre.FindPropertyRelative("f_show_Noise");
        f_show_Jitter = pre.FindPropertyRelative("f_show_Jitter");
        f_show_Signal = pre.FindPropertyRelative("f_show_Signal");
        f_show_Artefacts = pre.FindPropertyRelative("f_show_Artefacts");
        f_show_Bypass = pre.FindPropertyRelative("f_show_Bypass");
        f_show_Glitch_Effects = pre.FindPropertyRelative("f_show_Glitch_Effects");
        //
        b_Bleed = pre.FindPropertyRelative("b_Bleed");
        b_Mode = pre.FindPropertyRelative("b_Mode");
        b_LinesMode = pre.FindPropertyRelative("b_LinesMode");
        b_ScreenLinesNum = pre.FindPropertyRelative("b_ScreenLinesNum");
        b_BleedAmount = pre.FindPropertyRelative("b_BleedAmount");

        b_BleedCurve1 = pre.FindPropertyRelative("b_BleedCurve1");
        b_BleedCurve2 = pre.FindPropertyRelative("b_BleedCurve2");
        b_BleedCurve3 = pre.FindPropertyRelative("b_BleedCurve3");
        b_BleedCurveSync = pre.FindPropertyRelative("b_BleedCurveSync");
        b_BleedLength = pre.FindPropertyRelative("b_BleedLength");

        b_BleedDebug = pre.FindPropertyRelative("b_BleedDebug");

        f_Fisheye = pre.FindPropertyRelative("f_Fisheye");
        f_FisheyeBend = pre.FindPropertyRelative("f_FisheyeBend");
        f_FisheyeType = pre.FindPropertyRelative("f_FisheyeType");
        f_CutoffX = pre.FindPropertyRelative("f_CutoffX");
        f_CutoffY = pre.FindPropertyRelative("f_CutoffY");
        f_FadeX = pre.FindPropertyRelative("f_FadeX");
        f_FadeY = pre.FindPropertyRelative("f_FadeY");

        v_Vignette = pre.FindPropertyRelative("v_Vignette");
        v_VignetteAmount = pre.FindPropertyRelative("v_VignetteAmount");
        v_VignetteSpeed = pre.FindPropertyRelative("v_VignetteSpeed");

        //n_enableNoise = pre.FindPropertyRelative("n_enableNoise");
        n_NoiseMode = pre.FindPropertyRelative("n_NoiseMode");
        n_NoiseLinesAmountY = pre.FindPropertyRelative("n_NoiseLinesAmountY");
        n_NoiseSignalProcessing = pre.FindPropertyRelative("n_NoiseSignalProcessing");

        g_enable_glitch = pre.FindPropertyRelative("g_enable_glitch");
        g_fade = pre.FindPropertyRelative("g_fade");
        g_speed = pre.FindPropertyRelative("g_speed");
        g_strength = pre.FindPropertyRelative("g_strength");
        //glitch2
            g_enable_glitch2 = pre.FindPropertyRelative("g_enable_glitch2");
            g_2intensity = pre.FindPropertyRelative("g_2Intensity");
            g_2resolution = pre.FindPropertyRelative("g_2Res");
            g_2speed = pre.FindPropertyRelative("g_2speed");

        f_Granularity = pre.FindPropertyRelative("f_Granularity");
        f_GranularityAmount = pre.FindPropertyRelative("f_GranularityAmount");
        f_TapeNoise = pre.FindPropertyRelative("f_TapeNoise");
        f_TapeNoiseTH = pre.FindPropertyRelative("f_TapeNoiseTH");
        f_TapeNoiseAmount = pre.FindPropertyRelative("f_TapeNoiseAmount");
        f_TapeNoiseSpeed = pre.FindPropertyRelative("f_TapeNoiseSpeed");
        f_LineNoise = pre.FindPropertyRelative("f_LineNoise");
        f_LineNoiseAmount = pre.FindPropertyRelative("f_LineNoiseAmount");
        f_LineNoiseSpeed = pre.FindPropertyRelative("f_LineNoiseSpeed");

        //j_enableJitter = pre.FindPropertyRelative("j_enableJitter");
        j_ScanLines = pre.FindPropertyRelative("j_ScanLines");
        j_ScanLinesWidth = pre.FindPropertyRelative("j_ScanLinesWidth");

        j_LinesFloat = pre.FindPropertyRelative("j_LinesFloat");
        j_LinesSpeed = pre.FindPropertyRelative("j_LinesSpeed");
        j_Stretch = pre.FindPropertyRelative("j_Stretch");

        j_TwitchHorizontal = pre.FindPropertyRelative("j_TwitchHorizontal");
        j_TwitchHorizFreq = pre.FindPropertyRelative("j_TwitchHorizFreq");
        j_TwitchVertical = pre.FindPropertyRelative("j_TwitchVertical");
        j_TwitchVertFreq = pre.FindPropertyRelative("j_TwitchVertFreq");

        j_JitterHorizontal = pre.FindPropertyRelative("j_JitterHorizontal");
        j_JitterHorizAmount = pre.FindPropertyRelative("j_JitterHorizAmount");
        j_JitterVertical = pre.FindPropertyRelative("j_JitterVertical");
        j_VertAmount = pre.FindPropertyRelative("j_VertAmount");
        j_VertSpeed = pre.FindPropertyRelative("j_VertSpeed");

        p_PictureCorrection = pre.FindPropertyRelative("p_PictureCorrection");
        p_PictureCorr1 = pre.FindPropertyRelative("p_PictureCorr1");
        p_PictureCorr2 = pre.FindPropertyRelative("p_PictureCorr2");
        p_PictureCorr3 = pre.FindPropertyRelative("p_PictureCorr3");

        p_PictureShift1 = pre.FindPropertyRelative("p_PictureShift1");
        p_PictureShift2 = pre.FindPropertyRelative("p_PictureShift2");
        p_PictureShift3 = pre.FindPropertyRelative("p_PictureShift3");

        f_SignalNoise = pre.FindPropertyRelative("f_SignalNoise");
        f_SignalNoiseAmount = pre.FindPropertyRelative("f_SignalNoiseAmount");
        f_SignalNoisePower = pre.FindPropertyRelative("f_SignalNoisePower");

        p_Gamma = pre.FindPropertyRelative("p_Gamma");

        a_Artefacts = pre.FindPropertyRelative("a_Artefacts");
        a_ArtefactsAmount = pre.FindPropertyRelative("a_ArtefactsAmount");
        a_ArtefactsFadeAmount = pre.FindPropertyRelative("a_ArtefactsFadeAmount");
        a_ArtefactsColor = pre.FindPropertyRelative("a_ArtefactsColor");
        a_ArtefactsThreshold = pre.FindPropertyRelative("a_ArtefactsThreshold");
        a_ArtefactsDebug = pre.FindPropertyRelative("a_ArtefactsDebug");

        //colorPalette
        resolutionMode = pre.FindPropertyRelative("resolutionMode");
        resolutionModeIndex = pre.FindPropertyRelative("resolutionModeIndex");

        resolution = pre.FindPropertyRelative("resolution");
        pixelSize = pre.FindPropertyRelative("pixelSize");
        dither = pre.FindPropertyRelative("dither");
        opacity = pre.FindPropertyRelative("opacity");
        colorPresetIndex = pre.FindPropertyRelative("colorPresetIndex");
        f_show_colorPalette = pre.FindPropertyRelative("f_show_colorPalette");
        enableColorPalette = pre.FindPropertyRelative("enableColorPalette");

        //

        independentTimeOn = pre.FindPropertyRelative("independentTimeOn");

        enableCustomTexture = pre.FindPropertyRelative("enableCustomTexture");
        bypassTex = pre.FindPropertyRelative("bypassTex");
        spriteTex = pre.FindPropertyRelative("spriteTex");
    }

    public override void OnInspectorGUI()
    {
        so.Update();
        GUILayout.Label(texture, GUILayout.MaxHeight(100), GUILayout.MinHeight(80));

        GUIStyle boldFoldout = new GUIStyle(EditorStyles.foldout);
        boldFoldout.fontStyle = FontStyle.Bold;
        f_show_TV_mode.boolValue = CustomUI.Foldout("TV mode", f_show_TV_mode.boolValue);
        animFolds[0].target = f_show_TV_mode.boolValue;
        //
        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[0].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                _enableTVmode.boolValue = EditorGUILayout.BeginToggleGroup("enable TV mode", _enableTVmode.boolValue);
                _VHSNoise.objectReferenceValue = EditorGUILayout.ObjectField("Noise Texture", _VHSNoise.objectReferenceValue, typeof(Texture), false);
                _textureIntensity.floatValue = EditorGUILayout.Slider("Texture intensity", _textureIntensity.floatValue, 0f, 8f);
                _offsetColor.floatValue = EditorGUILayout.Slider("Color offset", _offsetColor.floatValue, 0f, 0.1f);
                _VerticalOffsetFrequency.floatValue = EditorGUILayout.Slider("Vertical offset frequency", _VerticalOffsetFrequency.floatValue, 0f, 100f);
                _verticalOffset.floatValue = EditorGUILayout.Slider("Vertical offset", _verticalOffset.floatValue, 0f, 1f);
                _OffsetDistortion.floatValue = EditorGUILayout.Slider("Distortion offset", _OffsetDistortion.floatValue, 3500f, 1f);
                _scan.boolValue = EditorGUILayout.Toggle("Scan", _scan.boolValue);
                _adjustLines.floatValue = EditorGUILayout.Slider("Lines adjustment", _adjustLines.floatValue, 1f, 10f);
                _scanLinesColor.colorValue = EditorGUILayout.ColorField("Scan lines color", _scanLinesColor.colorValue);
                _hardScan.floatValue = EditorGUILayout.Slider("Hard scan", _hardScan.floatValue, -8f, -16f);
                _resolution.floatValue = EditorGUILayout.Slider("Resolution", _resolution.floatValue, 16f, 1f);
                maskDark.floatValue = EditorGUILayout.Slider("Mask dark", maskDark.floatValue, 0, 2);
                maskLight.floatValue = EditorGUILayout.Slider("Mask light", maskLight.floatValue, 0, 2);
                warp.vector4Value = EditorGUILayout.Vector4Field("Warp", warp.vector4Value);
                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
        }
        EditorGUILayout.Space();
        //
        f_show_Old_TV_mode.boolValue = CustomUI.Foldout("Old Film Filter", f_show_Old_TV_mode.boolValue);
        animFolds[11].target = f_show_Old_TV_mode.boolValue;
        //
        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[11].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                o_Enable_Old_TV_effects.boolValue = EditorGUILayout.BeginToggleGroup("Old Film Effect Properties", o_Enable_Old_TV_effects.boolValue);
                o_burn.floatValue = EditorGUILayout.Slider("Burn", o_burn.floatValue, -2f, 4f);
                o_sceneCut.floatValue = EditorGUILayout.Slider("Image Cut", o_sceneCut.floatValue, 0f, 16f);
                o_contrast.floatValue = EditorGUILayout.Slider("Contrast", o_contrast.floatValue, 0f, 5f);
                o_FPS.floatValue = EditorGUILayout.Slider("Frames Per Second", o_FPS.floatValue, 0f, 60f);
                o_Fade.floatValue = EditorGUILayout.Slider("Fade", o_Fade.floatValue, 0, 1f);
                EditorGUILayout.EndToggleGroup();
                o_Enable_negative_props.boolValue = EditorGUILayout.BeginToggleGroup("Negative Properties", o_Enable_negative_props.boolValue);
                o_Luminosity.floatValue = EditorGUILayout.Slider("Luminosity", o_Luminosity.floatValue, 0f, 2f);
                o_Negative.floatValue = EditorGUILayout.Slider("Negative", o_Negative.floatValue, 0f, 1f);
                o_Vignette.floatValue = EditorGUILayout.Slider("Grey Vignette", o_Vignette.floatValue, 0f, 1f);

                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
        }
        EditorGUILayout.Space();
        //
        f_show_Bleed.boolValue = CustomUI.Foldout("Bleed Effect", f_show_Bleed.boolValue);
        animFolds[2].target = f_show_Bleed.boolValue;
        //
        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[2].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                b_Bleed.boolValue = EditorGUILayout.BeginToggleGroup("Enable Bleed Effect", b_Bleed.boolValue);
                indP();
                b_LinesMode.intValue = EditorGUILayout.Popup("Vertical Resolution", b_LinesMode.intValue,
                        new string[4] { "Full", "240 ", "480 ", "Users" });

                if (b_LinesMode.intValue == 0) b_ScreenLinesNum.floatValue = Screen.currentResolution.height;
                if (b_LinesMode.intValue == 1) b_ScreenLinesNum.floatValue = 240;
                if (b_LinesMode.intValue == 2) b_ScreenLinesNum.floatValue = 480;
                if (b_LinesMode.intValue == 3)
                {
                    b_ScreenLinesNum.floatValue = EditorGUILayout.FloatField("Lines Per Height", b_ScreenLinesNum.floatValue);
                }

                b_Mode.intValue = EditorGUILayout.Popup("Bleed Mode", b_Mode.intValue,
                                                            new string[4] { "Old Three Phase", "Three Phase", "Two Phase (slow)", "Custom Curve" });

                if (b_Mode.intValue == 0) b_BleedLength.intValue = 24;
                if (b_Mode.intValue == 1) b_BleedLength.intValue = 24;
                if (b_Mode.intValue == 2) b_BleedLength.intValue = 32;
                if (b_Mode.intValue == 3)
                {
                    b_BleedCurveSync.boolValue = EditorGUILayout.Toggle("I and Q Sync", b_BleedCurveSync.boolValue);

                    b_BleedCurve1.animationCurveValue = EditorGUILayout.CurveField("Curve Y", b_BleedCurve1.animationCurveValue);
                    if (b_BleedCurveSync.boolValue)
                    {
                        b_BleedCurve2.animationCurveValue = EditorGUILayout.CurveField("Curve I,Q", b_BleedCurve2.animationCurveValue);
                    }
                    else
                    {
                        b_BleedCurve2.animationCurveValue = EditorGUILayout.CurveField("Curve I", b_BleedCurve2.animationCurveValue);
                        b_BleedCurve3.animationCurveValue = EditorGUILayout.CurveField("Curve Q", b_BleedCurve3.animationCurveValue);
                    }

                    b_BleedLength.intValue = (int)EditorGUILayout.Slider("Bleed Length", b_BleedLength.intValue, 0.0f, 50.0f);

                    EditorGUILayout.HelpBox("Note: \n1. Bigger 'Bleed Length' values cause slow performance.", MessageType.Info);
                }

                b_BleedAmount.floatValue = EditorGUILayout.Slider("Bleed Stretch", b_BleedAmount.floatValue, 0.0f, 15.0f);
                indM();
                b_BleedDebug.boolValue = EditorGUILayout.Toggle("Debug Bleed Curve", b_BleedDebug.boolValue);
                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
        }
        //
        EditorGUILayout.Space();

        f_show_colorPalette.boolValue = CustomUI.Foldout("Color Palette", f_show_colorPalette.boolValue);
        animFolds[3].target = f_show_colorPalette.boolValue;
        //
        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[3].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                enableColorPalette.boolValue = EditorGUILayout.BeginToggleGroup("Enable Color Palette", enableColorPalette.boolValue);
                indP();
                colorPresetIndex.intValue = EditorGUILayout.Popup("Color Preset", colorPresetIndex.intValue, palettePresets);
                resolutionModeIndex.intValue = EditorGUILayout.Popup("Resolution Mode", resolutionModeIndex.intValue, new string[2] { "Resolution", "Pixel Size" });
                switch (resolutionModeIndex.intValue)
                {
                    case 0:
                        resolution.vector2IntValue = EditorGUILayout.Vector2IntField("Resolution", resolution.vector2IntValue);

                        break;
                    case 1:
                        pixelSize.intValue = EditorGUILayout.IntSlider("Pixel Size", pixelSize.intValue, 1, 256);

                        break;
                    default:
                        break;
                }
                dither.floatValue = EditorGUILayout.Slider("Dithering", dither.floatValue, 0f, 1f);
                opacity.floatValue = EditorGUILayout.Slider("Opacity", opacity.floatValue, 0f, 1f);
                indM();
                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
        }
        //

        EditorGUILayout.Space();

        f_show_Fisheye.boolValue = CustomUI.Foldout("Fisheye", f_show_Fisheye.boolValue);
        animFolds[4].target = f_show_Fisheye.boolValue;
        //
        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[4].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                f_Fisheye.boolValue = EditorGUILayout.BeginToggleGroup("Enable Fisheye", f_Fisheye.boolValue);
                indP();
                f_FisheyeType.intValue = EditorGUILayout.Popup("Type", f_FisheyeType.intValue,
                                                        new string[2] { "Default", "Hyperspace" });
                f_FisheyeBend.floatValue = EditorGUILayout.Slider("Bend", f_FisheyeBend.floatValue, 0.0f, 50.0f);
                f_CutoffX.floatValue = EditorGUILayout.Slider("Cutoff X", f_CutoffX.floatValue, 0.0f, 50.0f);
                f_CutoffY.floatValue = EditorGUILayout.Slider("Cutoff Y", f_CutoffY.floatValue, 0.0f, 50.0f);
                f_FadeX.floatValue = EditorGUILayout.Slider("Cutoff Fade X", f_FadeX.floatValue, 0.0f, 50.0f);
                f_FadeY.floatValue = EditorGUILayout.Slider("Cutoff Fade Y", f_FadeY.floatValue, 0.0f, 50.0f);
                indM();
                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
        }
        //
        EditorGUILayout.Space();
        f_show_Vignette.boolValue = CustomUI.Foldout("Vignette", f_show_Vignette.boolValue);
        animFolds[5].target = f_show_Vignette.boolValue;
        //
        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[5].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                v_Vignette.boolValue = EditorGUILayout.BeginToggleGroup("enable Vignette", v_Vignette.boolValue);
                indP();
                v_VignetteAmount.floatValue = EditorGUILayout.Slider("Amount", v_VignetteAmount.floatValue, 0.0f, 5.0f);
                v_VignetteSpeed.floatValue = EditorGUILayout.Slider("Pulse Speed", v_VignetteSpeed.floatValue, 0.0f, 5.0f);
                indM();
                EditorGUILayout.Space();
                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
        }
        //
        EditorGUILayout.Space();
        // Bottom Noise
        f_show_Bottom_Noise_mode.boolValue = CustomUI.Foldout("Bottom Noise", f_show_Bottom_Noise_mode.boolValue);
        animFolds[6].target = f_show_Bottom_Noise_mode.boolValue;

        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[6].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                b_BottomNoiseTexture.objectReferenceValue = EditorGUILayout.ObjectField("Bottom Noise Texture", b_BottomNoiseTexture.objectReferenceValue, typeof(Texture2D), false);
                b_useBottomNoise.boolValue = EditorGUILayout.Toggle("Enable bottom noise", b_useBottomNoise.boolValue);
                b_bottomIntensity.floatValue = EditorGUILayout.Slider("Bottom intensity", b_bottomIntensity.floatValue, 0.0f, 3f);
                b_bottomHeight.floatValue = EditorGUILayout.Slider("Bottom height", b_bottomHeight.floatValue, 0.0f, 0.5f);
                b_useBottomStretch.boolValue = EditorGUILayout.Toggle("Enable bottom stretch", b_useBottomStretch.boolValue);
                EditorGUI.indentLevel--;
            }
        }
        //
        EditorGUILayout.Space();
        //
        f_show_Glitch_Effects.boolValue = CustomUI.Foldout("Glitch Effects", f_show_Glitch_Effects.boolValue);
        animFolds[12].target = f_show_Glitch_Effects.boolValue;
        //
        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[12].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                g_enable_glitch.boolValue = EditorGUILayout.BeginToggleGroup("Enable Glitch 1", g_enable_glitch.boolValue);
                g_strength.floatValue = EditorGUILayout.Slider("Strength", g_strength.floatValue, 0f, 2f);
                g_speed.floatValue = EditorGUILayout.Slider("Speed", g_speed.floatValue, 0f, 1f);
                g_fade.floatValue = EditorGUILayout.Slider("Fade", g_fade.floatValue, 0f, 1f);

                EditorGUILayout.EndToggleGroup();

                                        g_enable_glitch2.boolValue = EditorGUILayout.BeginToggleGroup("Enable Glitch 2", g_enable_glitch2.boolValue);
                    g_2intensity.floatValue = EditorGUILayout.Slider("Intensity", g_2intensity.floatValue, 0f, 2f);
                    g_2resolution.floatValue = EditorGUILayout.Slider("Resolution", g_2resolution.floatValue, 1f, 2f);
                    //g_fade.floatValue = EditorGUILayout.Slider("Fade", g_fade.floatValue, 0f, 1f);

                    EditorGUILayout.EndToggleGroup();

                EditorGUI.indentLevel--;
            }
        }
        EditorGUILayout.Space();
        //
        f_show_Noise.boolValue = CustomUI.Foldout("Noise", f_show_Noise.boolValue);
        animFolds[7].target = f_show_Noise.boolValue;
        //
        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[7].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                n_NoiseMode.intValue = EditorGUILayout.Popup("Vertical Resolution", n_NoiseMode.intValue,
                                                                    new string[2] { "Global", "Custom" });
                if (n_NoiseMode.intValue == 0) n_NoiseLinesAmountY.floatValue = b_ScreenLinesNum.floatValue;
                if (n_NoiseMode.intValue == 1)
                {
                    indP();
                    n_NoiseLinesAmountY.floatValue = EditorGUILayout.FloatField("Noise Lines Amount Y", n_NoiseLinesAmountY.floatValue);
                    indM();
                }
                n_NoiseSignalProcessing.floatValue = EditorGUILayout.Slider("Noise Signal Processing", n_NoiseSignalProcessing.floatValue, 0.0f, 1.0f);
                EditorGUILayout.Space();

                f_Granularity.boolValue = EditorGUILayout.Toggle("Granularity", f_Granularity.boolValue);
                indP();
                f_GranularityAmount.floatValue = EditorGUILayout.Slider("Granularity Alpha", f_GranularityAmount.floatValue, 0.0f, 0.1f);
                indM();

                f_SignalNoise.boolValue = EditorGUILayout.Toggle("Signal Noise", f_SignalNoise.boolValue);
                indP();
                f_SignalNoiseAmount.floatValue = EditorGUILayout.Slider("Signal Amount", f_SignalNoiseAmount.floatValue, 0f, 1f);
                f_SignalNoisePower.floatValue = EditorGUILayout.Slider("Signal Power", f_SignalNoisePower.floatValue, 0f, 1f);
                indM();

                f_LineNoise.boolValue = EditorGUILayout.Toggle("Line Noise", f_LineNoise.boolValue);
                indP();
                f_LineNoiseAmount.floatValue = EditorGUILayout.Slider("Alpha", f_LineNoiseAmount.floatValue, 0.0f, 10.0f);
                f_LineNoiseSpeed.floatValue = EditorGUILayout.Slider("Speed", f_LineNoiseSpeed.floatValue, 0.0f, 10.0f);
                indM();
                f_TapeNoise.boolValue = EditorGUILayout.Toggle("Tape Noise", f_TapeNoise.boolValue);
                indP();
                f_TapeNoiseTH.floatValue = EditorGUILayout.Slider("Tape Amount", f_TapeNoiseTH.floatValue, 0.0f, 1.5f);
                f_TapeNoiseSpeed.floatValue = EditorGUILayout.Slider("Tape Speed", f_TapeNoiseSpeed.floatValue, 0.0f, 1.5f);
                f_TapeNoiseAmount.floatValue = EditorGUILayout.Slider("Tape Alpha", f_TapeNoiseAmount.floatValue, 0.0f, 1.5f);
                indM();
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
        }
        //
        //Video or image Jitter occurs when the horizontal lines of video image frames are randomly displaced due to the corruption of synchronization signals or electromagnetic interference during video transmission.
        EditorGUILayout.Space();
        f_show_Jitter.boolValue = CustomUI.Foldout("Video Shake Filter", f_show_Jitter.boolValue);
        animFolds[8].target = f_show_Jitter.boolValue;

        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[8].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                JitterInfo = EditorGUILayout.Foldout(JitterInfo, "info", EditorStyles.foldout);
                if (JitterInfo)
                {
                    EditorGUILayout.HelpBox("Video or image shake occurs when the horizontal lines of video image frames are randomly displaced due to the corruption of synchronization signals or electromagnetic interference during video transmission.", MessageType.Info);
                }
                j_ScanLines.boolValue = EditorGUILayout.Toggle("Show Lines", j_ScanLines.boolValue);
                //indP();
                j_ScanLinesWidth.floatValue = EditorGUILayout.Slider("Lines Width", j_ScanLinesWidth.floatValue, 0.0f, 20.0f);
                //indM();
                EditorGUILayout.Space();

                j_LinesFloat.boolValue = EditorGUILayout.Toggle("Floating Lines", j_LinesFloat.boolValue);
                //indP();
                j_LinesSpeed.floatValue = EditorGUILayout.Slider("Lines Speed", j_LinesSpeed.floatValue, -3.0f, 3.0f);
                //indM();
                j_Stretch.boolValue = EditorGUILayout.Toggle("Stretch Noise", j_Stretch.boolValue);
                EditorGUILayout.Space();

                j_JitterHorizontal.boolValue = EditorGUILayout.Toggle("Horizontal Interlacing", j_JitterHorizontal.boolValue);
                //indP();
                j_JitterHorizAmount.floatValue = EditorGUILayout.Slider("Horizontal Amount", j_JitterHorizAmount.floatValue, 0.0f, 5.0f);
                //indM();
                j_JitterVertical.boolValue = EditorGUILayout.Toggle("Shake", j_JitterVertical.boolValue);
                //indP();
                j_VertAmount.floatValue = EditorGUILayout.Slider("Shake Amount", j_VertAmount.floatValue, 0.0f, 15.0f);
                j_VertSpeed.floatValue = EditorGUILayout.Slider("Vertical Shake Speed", j_VertSpeed.floatValue, 0.0f, 5.0f);
                //indM();
                EditorGUILayout.Space();

                j_TwitchHorizontal.boolValue = EditorGUILayout.Toggle("Twitch Horizontal", j_TwitchHorizontal.boolValue);
                //indP();
                j_TwitchHorizFreq.floatValue = EditorGUILayout.Slider("Horizontal Frequency", j_TwitchHorizFreq.floatValue, 0.0f, 5.0f);
                //indM();
                j_TwitchVertical.boolValue = EditorGUILayout.Toggle("Twitch Vertical", j_TwitchVertical.boolValue);
                //indP();
                j_TwitchVertFreq.floatValue = EditorGUILayout.Slider("Vertical Frequency", j_TwitchVertFreq.floatValue, 0.0f, 5.0f);
                //indM();
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
        }
        //
        EditorGUILayout.Space();
        f_show_Signal.boolValue = CustomUI.Foldout("Picture Correction", f_show_Signal.boolValue);
        animFolds[9].target = f_show_Signal.boolValue;

        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[9].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                p_PictureCorrection.boolValue = EditorGUILayout.BeginToggleGroup("Picture correction enable", p_PictureCorrection.boolValue);
                indP();
                p_PictureCorr1.floatValue = EditorGUILayout.Slider("Shift 1", p_PictureCorr1.floatValue, -0.25f, 0.25f);
                p_PictureCorr2.floatValue = EditorGUILayout.Slider("Shift 2", p_PictureCorr2.floatValue, -0.25f, 0.25f);
                p_PictureCorr3.floatValue = EditorGUILayout.Slider("Shift 3", p_PictureCorr3.floatValue, -0.25f, 0.25f);
                p_PictureShift1.floatValue = EditorGUILayout.Slider("Adjust 1", p_PictureShift1.floatValue, -2.0f, 2.0f);
                p_PictureShift2.floatValue = EditorGUILayout.Slider("Adjust 2", p_PictureShift2.floatValue, -2.0f, 2.0f);
                p_PictureShift3.floatValue = EditorGUILayout.Slider("Adjust 3", p_PictureShift3.floatValue, -2.0f, 2.0f);

                p_Gamma.floatValue = EditorGUILayout.Slider("Gamma", p_Gamma.floatValue, 0.0f, 2.0f);
                indM();
                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
        }
        //
        EditorGUILayout.Space();
        f_show_Artefacts.boolValue = CustomUI.Foldout("Artefacts", f_show_Artefacts.boolValue);
        animFolds[10].target = f_show_Artefacts.boolValue;

        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[10].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                a_Artefacts.boolValue = EditorGUILayout.BeginToggleGroup("Enable Artefacts", a_Artefacts.boolValue);
                indP();
                a_ArtefactsThreshold.floatValue = EditorGUILayout.Slider("Input Cutoff", a_ArtefactsThreshold.floatValue, 0.0f, 1.0f);
                a_ArtefactsAmount.floatValue = EditorGUILayout.Slider("Input Amount", a_ArtefactsAmount.floatValue, 0.0f, 3.0f);
                a_ArtefactsFadeAmount.floatValue = EditorGUILayout.Slider("Fade", a_ArtefactsFadeAmount.floatValue, 0.0f, 1.0f);
                a_ArtefactsColor.colorValue = EditorGUILayout.ColorField("Color", a_ArtefactsColor.colorValue);
                indM();
                a_ArtefactsDebug.boolValue = EditorGUILayout.Toggle("Render Artefacts only", a_ArtefactsDebug.boolValue);
                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
        }
        //
        EditorGUILayout.Space();
        f_show_Bypass.boolValue = CustomUI.Foldout("Custom Texture", f_show_Bypass.boolValue);
        animFolds[1].target = f_show_Bypass.boolValue;
        //
        using (var group = new EditorGUILayout.FadeGroupScope(animFolds[1].faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                enableCustomTexture.boolValue = EditorGUILayout.BeginToggleGroup("Enable Custom Texture", enableCustomTexture.boolValue);
                bypassTex.objectReferenceValue = EditorGUILayout.ObjectField("Bypass Texture", bypassTex.objectReferenceValue, typeof(Texture), false);
                spriteTex.objectReferenceValue = EditorGUILayout.ObjectField("Sprite Texture", spriteTex.objectReferenceValue, typeof(Sprite), false);
                EditorGUILayout.Space();
                EditorGUILayout.EndToggleGroup();
                EditorGUI.indentLevel--;
            }
        }
        //
        EditorGUILayout.Space();
        independentTimeOn.boolValue = EditorGUILayout.Toggle("Use unscaled time", independentTimeOn.boolValue);

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        so.ApplyModifiedProperties();
    }
    private void indP()
    {
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
    }
    private void indM()
    {
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }
}
}
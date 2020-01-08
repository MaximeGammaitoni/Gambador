using UnityEngine;
namespace LimitlessDev.RetroLookPro
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Retro Look Pro")]
    public class RetroLookPro : MonoBehaviour
    {
        [HideInInspector]
        public bool developmentMode = true;
        [HideInInspector]
        public PresetScriptableObject referenceScr;
        public Preset tempPreset;
        private bool previous;
        float time_ = 0.0f;
        //
        RenderTexture texBottomNoise;
        RenderTexture texOldF;
        RenderTexture texGl1;
        RenderTexture texGL2;
        RenderTexture texTv;
        RenderTexture texVhs;
        RenderTexture texNeg;
        RenderTexture _trashFrame1;
        RenderTexture _trashFrame2;
        RenderTexture texPass12 = null;
        RenderTexture texPass23 = null;
        RenderTexture texLast = null;
        RenderTexture texFeedback = null;
        RenderTexture texFeedback2 = null;
        RenderTexture tempPass = null;
        RenderTexture zeroPass = null;
        RenderTexture texClear = null;
        RenderTexture texTape = null;
        //
        Texture3D colormapTexture;
        Texture2D colormapPalette;
        Texture2D _noiseTexture;
        int max_curve_length = 50;
        Texture2D texCurves = null;
        Vector4 curvesOffest = new Vector4(0, 0, 0, 0);
        float[,] curvesData = new float[50, 3];
        Material colorMat;
        int m_tempColorPresIndex;
        [HideInInspector]
        public effectPresets presetss;
        public bool turnAllEffectsOff;
        private float T;

        private void CreateMaterials()
        {
            tempPreset.m_1 = newMat(tempPreset.sh_first, "RetroLookPro/Legacy/First_RetroLook");
            tempPreset.m_2 = newMat(tempPreset.sh_second, "RetroLookPro/Legacy/Second_RetroLook");
            tempPreset.m_3 = newMat(tempPreset.sh_third, "RetroLookPro/Legacy/Third_RetroLook");
            tempPreset.m_4 = newMat(tempPreset.sh_fourth, "RetroLookPro/Legacy/Forth_RetroLook");
            tempPreset.m_clear = newMat(tempPreset.sh_clear, "RetroLookPro/Legacy/ResetRetroLook");
            tempPreset.m_tape_noise = newMat(tempPreset.sh_tape, "RetroLookPro/Legacy/Tape_RetroLook");
            if (tempPreset.b_Mode == 3) Curves();
        }
        private void Reset()
        {
            if (referenceScr)
            {

                tempPreset.resolutionMode = ResolutionMode.ConstantPixelSize;
                tempPreset.pixelSize = 3;
                tempPreset.opacity = 1;
                tempPreset.dither = 1;
            }
        }

        private void OnEnable()
        {
            texPass12 = null;
            if (this.isActiveAndEnabled)
            {

                if (referenceScr)
                {
                    if (developmentMode)
                        tempPreset = referenceScr.currPreset;
                    else
                    {
                        tempPreset = new Preset();
                        tempPreset = referenceScr.currPreset.ShallowCopy();
                    }
                    tempPreset._bottomNoiseMat = new Material(Shader.Find("RetroLookPro/Legacy/BottomNoiseEffect"));
                    tempPreset._bottomNoiseMat.SetTexture("_SecondaryTex", tempPreset._VHSNoise);

                    if (tempPreset.b_Mode == 3) Curves();
                    if (tempPreset._enableTVmode)
                    {
                        previous = tempPreset._scan;
                        tempPreset._VHS_Material = new Material(Shader.Find("RetroLookPro/Legacy/VHS_RetroLook"));
                        if (tempPreset._scan)
                            tempPreset._VHS_Material.shader = Shader.Find("RetroLookPro/Legacy/VHSwithLines_RetroLook");
                        tempPreset._VHS_Material.SetFloat("_OffsetPosY", tempPreset._verticalOffset);
                        tempPreset._TV_Material = new Material(Shader.Find("RetroLookPro/Legacy/TV_RetroLook"));
                    }

                    presetss.presetsList[tempPreset.colorPresetIndex].preset.changed = false;
                    string shaderName = "RetroLookPro/Legacy/ColorPalette";
                    Shader colorShader = Shader.Find(shaderName);

                    if (colorShader == null)
                    {
                        Debug.LogWarning("Shader '" + shaderName + "' not found. Was it deleted?");
                        enabled = false;
                    }

                    colorMat = new Material(colorShader);
                    colorMat.hideFlags = HideFlags.DontSave;

                    Texture2D texture = Resources.Load("Noise Textures/blue_noise") as Texture2D;

                    if (texture == null)
                    {
                        Debug.LogWarning("Noise Textures/blue_noise.png not found. Was it moved or deleted?");
                    }

                    m_tempColorPresIndex = tempPreset.colorPresetIndex;
                    colorMat.SetTexture("_BlueNoise", texture);
                }
            }
            if (referenceScr)
            {

                ApplyColormapToMaterial();
                tempPreset.g_GlitchMat2 = null;
            }
        }
        private void Update()
        {
            if (tempPreset.g_enable_glitch2)
            {

                if (Random.value > Mathf.Lerp(0.9f, 0.5f, tempPreset.g_2Intensity))
                {
                    SetUpResources();
                    UpdateNoiseTexture();
                }
            }
        }

        private Material newMat(Shader ref_shader, string shader_path)
        {
            Material temp_mat;
            if (ref_shader)
                temp_mat = new Material(ref_shader);
            else
                temp_mat = new Material(Shader.Find(shader_path)) { hideFlags = HideFlags.DontSave };
            return temp_mat;
        }
        private void CreateTextures(RenderTexture src)
        {
            texBottomNoise = CreateNewTexture(texBottomNoise, src);
            texBottomNoise.Create();
            texGl1 = CreateNewTexture(texGl1, src);
            texGl1.Create();
            texGL2 = CreateNewTexture(texGL2, src);
            texGL2.Create();
            texOldF = CreateNewTexture(texOldF, src);
            texOldF.Create();
            texTv = CreateNewTexture(texTv, src);
            texTv.Create();
            texVhs = CreateNewTexture(texVhs, src);
            texVhs.Create();
            texNeg = CreateNewTexture(texNeg, src);
            texNeg.Create();
            //
            texClear = CreateNewTexture(texClear, src);
            texClear.Create();
            texPass12 = CreateNewTexture(texPass12, src);
            texPass12.Create();
            texPass23 = CreateNewTexture(texPass23, src);
            texPass23.Create();
            texFeedback = CreateNewTexture(texFeedback, src, HideFlags.DontSave);
            texFeedback.Create();
            texFeedback2 = CreateNewTexture(texFeedback2, src, HideFlags.DontSave);
            texFeedback2.Create();
            texLast = CreateNewTexture(texLast, src, HideFlags.DontSave);
            texLast.Create();

            //clear textures
            Graphics.Blit(texClear, texFeedback, tempPreset.m_clear);
            Graphics.Blit(texClear, texFeedback2, tempPreset.m_clear);
            Graphics.Blit(texClear, texLast, tempPreset.m_clear);
        }


        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (!referenceScr && developmentMode)
            {
                Graphics.Blit(src, dest);
                return;
            }
            if (turnAllEffectsOff)
            {
                Graphics.Blit(src, dest);
                return;
            }
            T += Time.deltaTime;
            if (T > 100) T = 0;

            if (tempPreset.o_oldfilm_mat == null && tempPreset.o_Enable_Old_TV_effects)
            {
                tempPreset.o_oldfilm_mat = new Material(Shader.Find("RetroLookPro/Legacy/OldFilmFilterRetroLook"));
            }
            if (tempPreset.o_negative_mat == null && tempPreset.o_Enable_negative_props)
            {
                tempPreset.o_negative_mat = new Material(Shader.Find("RetroLookPro/Legacy/NegativeFilterRetroLook"));
            }

            if (tempPreset.g_GlitchMat1 == null && tempPreset.g_enable_glitch)
            {
                tempPreset.g_GlitchMat1 = new Material(Shader.Find("RetroLookPro/Legacy/Glitch1RetroLook"));
            }

            if (tempPreset.m_1 == null)
            {
                CreateMaterials();
            }
            if (texPass12 == null || (src.width != texPass12.width || src.height != texPass12.height))
            {
                CreateTextures(src);
            }
            if (tempPreset._bottomNoiseMat == null)
            {
                tempPreset._bottomNoiseMat = new Material(Shader.Find("RetroLookPro/Legacy/BottomNoiseEffect"));
                tempPreset._bottomNoiseMat.SetTexture("_SecondaryTex", tempPreset._VHSNoise);
            }
            if (zeroPass == null)
            {
                zeroPass = new RenderTexture(src.width, src.height, src.depth);
            }

            // custom texture
            if (tempPreset.enableCustomTexture)
            {
                if (tempPreset.spriteTex != null) tempPreset.bypassTex = tempPreset.spriteTex.texture;
                if (tempPreset.bypassTex != null) Graphics.Blit(tempPreset.bypassTex, src, tempPreset.m_1);

            }
            //bottom noise
            #region Bottom Noise
            if (tempPreset.useBottomNoise || tempPreset.useBottomStretch)
            {

                tempPreset._bottomNoiseMat.SetFloat("_OffsetNoiseX", Random.Range(0f, 1.0f));
                float offsetNoise1 = tempPreset._bottomNoiseMat.GetFloat("_OffsetNoiseY");
                tempPreset._bottomNoiseMat.SetFloat("_OffsetNoiseY", offsetNoise1 + Random.Range(-0.05f, 0.05f));

                if (tempPreset.useBottomNoise) tempPreset._bottomNoiseMat.EnableKeyword("NOISE_BOTTOM_ON");
                else tempPreset._bottomNoiseMat.DisableKeyword("NOISE_BOTTOM_ON");

                if (tempPreset.useBottomStretch) tempPreset._bottomNoiseMat.EnableKeyword("BOTTOM_STRETCH_ON");
                else tempPreset._bottomNoiseMat.DisableKeyword("BOTTOM_STRETCH_ON");

                tempPreset._bottomNoiseMat.SetFloat("_NoiseBottomHeight", tempPreset.bottomHeight);
                tempPreset._bottomNoiseMat.SetFloat("_NoiseBottomIntensity", tempPreset.bottomIntensity);
                Graphics.Blit(src, texBottomNoise, tempPreset._bottomNoiseMat);
            }
            else
            {
                Graphics.Blit(src, texBottomNoise);
            }
            #endregion

            #region Glitches
            #region Glitch 1
            if (tempPreset.g_enable_glitch)
            {

                tempPreset.g_GlitchMat1.SetFloat("T", T);
                tempPreset.g_GlitchMat1.SetFloat("Speed", tempPreset.g_speed);
                tempPreset.g_GlitchMat1.SetFloat("Strength", tempPreset.g_strength);
                tempPreset.g_GlitchMat1.SetFloat("Fade", tempPreset.g_fade);
                Graphics.Blit(texBottomNoise, texGl1, tempPreset.g_GlitchMat1);
            }
            else
            {
                Graphics.Blit(texBottomNoise, texGl1);
            }
            #endregion
            #region Glitch 2
            if (tempPreset.g_enable_glitch2)
            {
                SetUpResources();

                // Update trash frames.
                int fcount = Time.frameCount;
                if (fcount % 13 == 0) Graphics.Blit(src, _trashFrame1);
                if (fcount % 73 == 0) Graphics.Blit(src, _trashFrame2);

                tempPreset.g_GlitchMat2.SetFloat("_Intensity", tempPreset.g_2Intensity);
                tempPreset.g_GlitchMat2.SetTexture("_NoiseTex", _noiseTexture);
                RenderTexture trashFrame = Random.value > 0.5f ? _trashFrame1 : _trashFrame2;
                tempPreset.g_GlitchMat2.SetTexture("_TrashTex", trashFrame);
                Graphics.Blit(texGl1, texGL2, tempPreset.g_GlitchMat2);
            }
            else
            {
                Graphics.Blit(texGl1, texGL2);
            }
            #endregion
            #endregion

            #region TV mode
            if (tempPreset._enableTVmode)
            {
                if (tempPreset._VHS_Material == null)
                {
                    tempPreset._VHS_Material = new Material(Shader.Find("RetroLookPro/Legacy/VHS_RetroLook"));
                    tempPreset._TV_Material = new Material(Shader.Find("RetroLookPro/Legacy/TV_RetroLook"));
                }
                if (previous != tempPreset._scan)
                {
                    previous = tempPreset._scan;
                    if (tempPreset._scan)
                        tempPreset._VHS_Material.shader = Shader.Find("RetroLookPro/Legacy/VHSwithLines_RetroLook");
                    else
                        tempPreset._VHS_Material.shader = Shader.Find("RetroLookPro/Legacy/VHS_RetroLook");
                }
                if (tempPreset._scan)
                {
                    tempPreset._VHS_Material.SetColor("_ScanLinesColor", tempPreset._scanLinesColor);
                    tempPreset._VHS_Material.SetFloat("_ScanLines", tempPreset._adjustLines);
                }

                if (Random.Range(0, 100 - tempPreset._VerticalOffsetFrequency) <= 5)
                {
                    if (tempPreset._verticalOffset == 0.0f)
                    {
                        tempPreset._VHS_Material.SetFloat("_OffsetPosY", tempPreset._verticalOffset);
                    }
                    if (tempPreset._verticalOffset > 0.0f)
                    {
                        tempPreset._VHS_Material.SetFloat("_OffsetPosY", tempPreset._verticalOffset - Random.Range(0f, tempPreset._verticalOffset));
                    }
                    else if (tempPreset._verticalOffset < 0.0f)
                    {
                        tempPreset._VHS_Material.SetFloat("_OffsetPosY", tempPreset._verticalOffset + Random.Range(0f, -tempPreset._verticalOffset));
                    }
                }
                tempPreset._VHS_Material.SetFloat("_OffsetDistortion", tempPreset._OffsetDistortion);
                tempPreset._VHS_Material.SetFloat("_OffsetColor", tempPreset._offsetColor);
                tempPreset._VHS_Material.SetFloat("_OffsetNoiseX", Random.Range(0f, 0.6f));
                tempPreset._VHS_Material.SetTexture("_SecondaryTex", tempPreset._VHSNoise);
                float offsetNoise = tempPreset._VHS_Material.GetFloat("_OffsetNoiseY");
                tempPreset._VHS_Material.SetFloat("_OffsetNoiseY", offsetNoise + Random.Range(-0.03f, 0.03f));
                tempPreset._VHS_Material.SetFloat("_Intensity", tempPreset._textureIntensity);
                tempPreset._offsetColor = tempPreset._VHS_Material.GetFloat("_OffsetColor");
                tempPreset._TV_Material.SetFloat("hardScan", tempPreset._hardScan);
                tempPreset._TV_Material.SetFloat("resScale", tempPreset._resolution);
                tempPreset._TV_Material.SetFloat("maskDark", tempPreset.maskDark);
                tempPreset._TV_Material.SetFloat("maskLight", tempPreset.maskLight);
                tempPreset._TV_Material.SetVector("warp", tempPreset.warp);
                if (tempPass == null)
                {
                    tempPass = new RenderTexture(src.width, src.height, src.depth);
                }
                Graphics.Blit(texGL2, texVhs, tempPreset._VHS_Material);
                Graphics.Blit(texVhs, texTv, tempPreset._TV_Material);
            }
            else
            {
                Graphics.Blit(texGL2, texTv);
            }
            #endregion

            #region Old TV filter
            if (tempPreset.o_Enable_Old_TV_effects)
            {
                tempPreset.o_oldfilm_mat.SetFloat("T", T);
                tempPreset.o_oldfilm_mat.SetFloat("FPS", tempPreset.o_FPS);
                tempPreset.o_oldfilm_mat.SetFloat("Contrast", tempPreset.o_contrast);
                tempPreset.o_oldfilm_mat.SetFloat("Burn", tempPreset.o_burn);
                tempPreset.o_oldfilm_mat.SetFloat("SceneCut", tempPreset.o_sceneCut);
                tempPreset.o_oldfilm_mat.SetFloat("Fade", tempPreset.o_Fade);
                Graphics.Blit(texTv, texOldF, tempPreset.o_oldfilm_mat);
            }
            else
            {
                Graphics.Blit(texTv, texOldF);
            }
            #endregion

            float screenLinesNum_ = tempPreset.b_ScreenLinesNum;
            if (screenLinesNum_ <= 0) screenLinesNum_ = src.height;
            if (tempPreset.f_TapeNoise || tempPreset.f_Granularity || tempPreset.f_LineNoise)
                if (texTape == null || (texTape.height != Mathf.Min(tempPreset.n_NoiseLinesAmountY, screenLinesNum_)))
                {
                    int texHeight = (int)Mathf.Min(tempPreset.n_NoiseLinesAmountY, screenLinesNum_);
                    int texWidth = (int)(
                          (float)texHeight * (float)src.width / (float)src.height);
                    DestroyImmediate(texTape);
                    texTape = new RenderTexture(texWidth, texHeight, 0);
                    texTape.hideFlags = HideFlags.HideAndDontSave;
                    texTape.filterMode = FilterMode.Point;
                    texTape.Create();
                    Graphics.Blit(texClear, texTape, tempPreset.m_tape_noise);
                }
            if (tempPreset.independentTimeOn) { time_ = Time.unscaledTime; }
            else { time_ = Time.time; }
            tempPreset.m_1.SetFloat("time_", time_);

            tempPreset.m_1.SetFloat("screenLinesNum", screenLinesNum_);
            tempPreset.m_1.SetFloat("noiseLinesNum", tempPreset.n_NoiseLinesAmountY);
            tempPreset.m_1.SetFloat("noiseQuantizeX", tempPreset.n_NoiseSignalProcessing);
            ParamSwitch(tempPreset.m_1, tempPreset.f_Granularity, "VHS_FILMGRAIN_ON");
            ParamSwitch(tempPreset.m_1, tempPreset.f_TapeNoise, "VHS_TAPENOISE_ON");
            ParamSwitch(tempPreset.m_1, tempPreset.f_LineNoise, "VHS_LINENOISE_ON");
            ParamSwitch(tempPreset.m_1, tempPreset.j_JitterHorizontal, "VHS_JITTER_H_ON");
            tempPreset.m_1.SetFloat("jitterHAmount", tempPreset.j_JitterHorizAmount);
            ParamSwitch(tempPreset.m_1, tempPreset.j_JitterVertical, "VHS_JITTER_V_ON");
            tempPreset.m_1.SetFloat("jitterVAmount", tempPreset.j_VertAmount);
            tempPreset.m_1.SetFloat("jitterVSpeed", tempPreset.j_VertSpeed);
            ParamSwitch(tempPreset.m_1, tempPreset.j_LinesFloat, "VHS_LINESFLOAT_ON");
            tempPreset.m_1.SetFloat("linesFloatSpeed", tempPreset.j_LinesSpeed);
            ParamSwitch(tempPreset.m_1, tempPreset.j_TwitchHorizontal, "VHS_TWITCH_H_ON");
            tempPreset.m_1.SetFloat("twitchHFreq", tempPreset.j_TwitchHorizFreq);
            ParamSwitch(tempPreset.m_1, tempPreset.j_TwitchVertical, "VHS_TWITCH_V_ON");
            tempPreset.m_1.SetFloat("twitchVFreq", tempPreset.j_TwitchVertFreq);
            ParamSwitch(tempPreset.m_1, tempPreset.j_ScanLines, "VHS_SCANLINES_ON");
            tempPreset.m_1.SetFloat("scanLineWidth", tempPreset.j_ScanLinesWidth);
            ParamSwitch(tempPreset.m_1, tempPreset.f_SignalNoise, "VHS_YIQNOISE_ON");
            tempPreset.m_1.SetFloat("signalNoisePower", tempPreset.f_SignalNoisePower);
            tempPreset.m_1.SetFloat("signalNoiseAmount", tempPreset.f_SignalNoiseAmount);
            ParamSwitch(tempPreset.m_1, tempPreset.j_Stretch, "VHS_STRETCH_ON");
            ParamSwitch(tempPreset.m_1, tempPreset.f_Fisheye, "VHS_FISHEYE_ON");
            tempPreset.m_1.SetFloat("cutoffX", tempPreset.f_CutoffX);
            tempPreset.m_1.SetFloat("cutoffY", tempPreset.f_CutoffY);
            tempPreset.m_1.SetFloat("cutoffFadeX", tempPreset.f_FadeX);
            tempPreset.m_1.SetFloat("cutoffFadeY", tempPreset.f_FadeY);
            tempPreset.m_2.SetFloat("time_", time_);
            tempPreset.m_2.SetFloat("screenLinesNum", screenLinesNum_);


            #region Bleed
            ParamSwitch(tempPreset.m_2, tempPreset.b_Bleed, "VHS_BLEED_ON");
            tempPreset.m_2.DisableKeyword("VHS_OLD_THREE_PHASE");
            tempPreset.m_2.DisableKeyword("VHS_THREE_PHASE");
            tempPreset.m_2.DisableKeyword("VHS_TWO_PHASE");
            if (tempPreset.b_Mode == 0) { tempPreset.m_2.EnableKeyword("VHS_OLD_THREE_PHASE"); }
            else if (tempPreset.b_Mode == 1) { tempPreset.m_2.EnableKeyword("VHS_THREE_PHASE"); }
            else if (tempPreset.b_Mode == 2) { tempPreset.m_2.EnableKeyword("VHS_TWO_PHASE"); }
            else if (tempPreset.b_Mode == 3) { if (tempPreset.b_BleedCurveEditMode) Curves(); }
            tempPreset.m_2.SetTexture("_CurvesTex", texCurves);
            tempPreset.m_2.SetVector("curvesOffest", curvesOffest);
            tempPreset.m_2.SetInt("bleedLength", tempPreset.b_BleedLength);
            ParamSwitch(tempPreset.m_2, (tempPreset.b_Mode == 3), "VHS_CUSTOM_BLEED_ON");
            ParamSwitch(tempPreset.m_2, tempPreset.b_BleedDebug, "VHS_DEBUG_BLEEDING_ON");
            tempPreset.m_2.SetFloat("bleedAmount", tempPreset.b_BleedAmount);
            #endregion

            #region Fisheye
            ParamSwitch(tempPreset.m_2, tempPreset.f_Fisheye, "VHS_FISHEYE_ON");
            ParamSwitch(tempPreset.m_2, tempPreset.f_FisheyeType == 1, "VHS_FISHEYE_HYPERSPACE");
            tempPreset.m_2.SetFloat("fisheyeBend", tempPreset.f_FisheyeBend);
            tempPreset.m_2.SetFloat("fisheyeSize", tempPreset.f_FisheyeSize);
            #endregion

            #region Vignette
            ParamSwitch(tempPreset.m_2, tempPreset.v_Vignette, "VHS_VIGNETTE_ON");
            tempPreset.m_2.SetFloat("vignetteAmount", tempPreset.v_VignetteAmount);
            tempPreset.m_2.SetFloat("vignetteSpeed", tempPreset.v_VignetteSpeed);
            #endregion

            #region Picture Correction
            ParamSwitch(tempPreset.m_2, tempPreset.p_PictureCorrection, "VHS_SIGNAL_TWEAK_ON");
            tempPreset.m_2.SetFloat("signalAdjustY", tempPreset.p_PictureCorr1);
            tempPreset.m_2.SetFloat("signalAdjustI", tempPreset.p_PictureCorr2);
            tempPreset.m_2.SetFloat("signalAdjustQ", tempPreset.p_PictureCorr3);
            tempPreset.m_2.SetFloat("signalShiftY", tempPreset.p_PictureShift1);
            tempPreset.m_2.SetFloat("signalShiftI", tempPreset.p_PictureShift2);
            tempPreset.m_2.SetFloat("signalShiftQ", tempPreset.p_PictureShift3);
            tempPreset.m_2.SetFloat("gammaCorection", tempPreset.p_Gamma);
            #endregion

            #region Noise
            if (tempPreset.f_TapeNoise || tempPreset.f_Granularity || tempPreset.f_LineNoise)
            {
                tempPreset.m_tape_noise.SetFloat("time_", time_);
                ParamSwitch(tempPreset.m_tape_noise, tempPreset.f_Granularity, "VHS_FILMGRAIN_ON");
                tempPreset.m_tape_noise.SetFloat("filmGrainAmount", tempPreset.f_GranularityAmount);
                ParamSwitch(tempPreset.m_tape_noise, tempPreset.f_TapeNoise, "VHS_TAPENOISE_ON");
                tempPreset.m_tape_noise.SetFloat("tapeNoiseTH", tempPreset.f_TapeNoiseTH);
                tempPreset.m_tape_noise.SetFloat("tapeNoiseAmount", tempPreset.f_TapeNoiseAmount);
                tempPreset.m_tape_noise.SetFloat("tapeNoiseSpeed", tempPreset.f_TapeNoiseSpeed);
                ParamSwitch(tempPreset.m_tape_noise, tempPreset.f_LineNoise, "VHS_LINENOISE_ON");
                tempPreset.m_tape_noise.SetFloat("lineNoiseAmount", tempPreset.f_LineNoiseAmount);
                tempPreset.m_tape_noise.SetFloat("lineNoiseSpeed", tempPreset.f_LineNoiseSpeed);
                Graphics.Blit(texTape, texTape, tempPreset.m_tape_noise);
                tempPreset.m_1.SetTexture("_TapeTex", texTape);
                tempPreset.m_1.SetFloat("tapeNoiseAmount", tempPreset.f_TapeNoiseAmount);
            }
            #endregion

            Graphics.Blit(texOldF, texPass12, tempPreset.m_1);

            #region Color Palette

            if (tempPreset.enableColorPalette)
            {
                if (presetss != null && intHasChanged(tempPreset.colorPresetIndex, m_tempColorPresIndex))
                {
                    ApplyColormapToMaterial();
                }

                ApplyMaterialVariables();

                RenderTexture scaled = RenderTexture.GetTemporary(tempPreset.resolution.x, tempPreset.resolution.y);
                scaled.filterMode = FilterMode.Point;

                if (presetss == null)
                {
                    Graphics.Blit(texPass12, scaled);
                }
                else
                {
                    Graphics.Blit(texPass12, scaled, colorMat);
                }

                Graphics.Blit(scaled, texPass12);
                RenderTexture.ReleaseTemporary(scaled);
            }
            #endregion

            #region Negative Filter
            if (tempPreset.o_Enable_negative_props)
            {
                tempPreset.o_negative_mat.SetFloat("T", T);
                tempPreset.o_negative_mat.SetFloat("Luminosity", 2 - tempPreset.o_Luminosity);
                tempPreset.o_negative_mat.SetFloat("Vignette", 1 - tempPreset.o_Vignette);
                tempPreset.o_negative_mat.SetFloat("Negative", tempPreset.o_Negative);
                Graphics.Blit(texPass12, texNeg, tempPreset.o_negative_mat);
            }
            else
            {
                Graphics.Blit(texPass12, texNeg);
            }
            #endregion

            if (!tempPreset.a_Artefacts)
            {
                Graphics.Blit(texNeg, dest, tempPreset.m_2);
            }
            else
            {
                Graphics.Blit(texNeg, texPass23, tempPreset.m_2);
                tempPreset.m_3.SetTexture("_LastTex", texLast);
                tempPreset.m_3.SetTexture("_FeedbackTex", texFeedback);
                tempPreset.m_3.SetFloat("feedbackThresh", tempPreset.a_ArtefactsThreshold);
                tempPreset.m_3.SetFloat("feedbackAmount", tempPreset.a_ArtefactsAmount);
                tempPreset.m_3.SetFloat("feedbackFade", tempPreset.a_ArtefactsFadeAmount);
                tempPreset.m_3.SetColor("feedbackColor", tempPreset.a_ArtefactsColor);
                Graphics.Blit(texPass23, texFeedback2, tempPreset.m_3);
                Graphics.Blit(texFeedback2, texFeedback);
                tempPreset.m_4.SetFloat("feedbackAmp", 1.0f);
                tempPreset.m_4.SetTexture("_FeedbackTex", texFeedback);
                Graphics.Blit(texPass23, texLast, tempPreset.m_4);

                if (!tempPreset.a_ArtefactsDebug)
                    Graphics.Blit(texLast, dest);
                else
                    Graphics.Blit(texFeedback, dest);
            }
           texBottomNoise.Release();
           texOldF.Release();
           texGl1.Release();
           texGL2.Release();
           texTv.Release();
           texVhs.Release();
           texNeg.Release();
        }
        private void ParamSwitch(Material mat, bool paramValue, string paramName)
        {
            if (paramValue) mat.EnableKeyword(paramName);
            else mat.DisableKeyword(paramName);
        }
        private void Curves()
        {
            if (texCurves == null) texCurves = new Texture2D(max_curve_length, 1, TextureFormat.RGBA32, false);
            curvesOffest[0] = 0.0f;
            curvesOffest[1] = 0.0f;
            curvesOffest[2] = 0.0f;
            float t = 0.0f;
            for (int i = 0; i < tempPreset.b_BleedLength; i++)
            {

                t = ((float)i) / ((float)tempPreset.b_BleedLength);
                curvesData[i, 0] = tempPreset.b_BleedCurve1.Evaluate(t);
                curvesData[i, 1] = tempPreset.b_BleedCurve2.Evaluate(t);
                curvesData[i, 2] = tempPreset.b_BleedCurve3.Evaluate(t);
                if (tempPreset.b_BleedCurveSync) curvesData[i, 2] = curvesData[i, 1];

                if (curvesOffest[0] > curvesData[i, 0]) curvesOffest[0] = curvesData[i, 0];
                if (curvesOffest[1] > curvesData[i, 1]) curvesOffest[1] = curvesData[i, 1];
                if (curvesOffest[2] > curvesData[i, 2]) curvesOffest[2] = curvesData[i, 2];
            };
            curvesOffest[0] = Mathf.Abs(curvesOffest[0]);
            curvesOffest[1] = Mathf.Abs(curvesOffest[1]);
            curvesOffest[2] = Mathf.Abs(curvesOffest[2]);

            for (int i = 0; i < tempPreset.b_BleedLength; i++)
            {
                curvesData[i, 0] += curvesOffest[0];
                curvesData[i, 1] += curvesOffest[1];
                curvesData[i, 2] += curvesOffest[2];
                texCurves.SetPixel(-2 + tempPreset.b_BleedLength - i, 0, new Color(curvesData[i, 0], curvesData[i, 1], curvesData[i, 2]));
            };

            texCurves.Apply();

        }
        private RenderTexture CreateNewTexture(RenderTexture texture, RenderTexture scrT, HideFlags flags)
        {
            DestroyImmediate(texture);
            texture = new RenderTexture(scrT.width, scrT.height, 0) { hideFlags = flags, filterMode = FilterMode.Point };
            return texture;
        }
        private RenderTexture CreateNewTexture(RenderTexture texture, RenderTexture scrT)
        {
            DestroyImmediate(texture);
            texture = new RenderTexture(scrT.width, scrT.height, scrT.depth) { filterMode = FilterMode.Point };
            return texture;
        }
        public void ApplyMaterialVariables()
        {
            switch (tempPreset.resolutionModeIndex)
            {
                case 0:
                    tempPreset.resolutionMode = ResolutionMode.ConstantResolution;
                    break;
                case 1:
                    tempPreset.resolutionMode = ResolutionMode.ConstantPixelSize;
                    break;
                default:
                    break;
            }
            tempPreset.pixelSize = (int)Mathf.Clamp(tempPreset.pixelSize, 1, float.MaxValue);

            if (tempPreset.resolutionMode == ResolutionMode.ConstantPixelSize)
            {
                tempPreset.resolution.x = Screen.width / tempPreset.pixelSize;
                tempPreset.resolution.y = Screen.height / tempPreset.pixelSize;
            }

            tempPreset.resolution.x = Mathf.Clamp(tempPreset.resolution.x, 1, 16384);
            tempPreset.resolution.y = Mathf.Clamp(tempPreset.resolution.y, 1, 16384);

            tempPreset.opacity = Mathf.Clamp01(tempPreset.opacity);
            tempPreset.dither = Mathf.Clamp01(tempPreset.dither);

            colorMat.SetFloat("_Opacity", tempPreset.opacity);
            colorMat.SetFloat("_Dither", tempPreset.dither);
        }

        public void ApplyColormapToMaterial()
        {
            if (presetss != null)
            {
                ApplyPalette();
                ApplyMap();
            }
        }
        void ApplyPalette()
        {
            colormapPalette = new Texture2D(256, 1, TextureFormat.RGB24, false);
            colormapPalette.filterMode = FilterMode.Point;
            colormapPalette.wrapMode = TextureWrapMode.Clamp;

            for (int i = 0; i < presetss.presetsList[tempPreset.colorPresetIndex].preset.numberOfColors; ++i)
            {
                colormapPalette.SetPixel(i, 0, presetss.presetsList[tempPreset.colorPresetIndex].preset.palette[i]);
            }

            colormapPalette.Apply();

            colorMat.SetTexture("_Palette", colormapPalette);
        }
        public void ApplyMap()
        {
            int colorsteps = 64;
            colormapTexture = new Texture3D(colorsteps, colorsteps, colorsteps, TextureFormat.RGB24, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            colormapTexture.SetPixels32(presetss.presetsList[tempPreset.colorPresetIndex].preset.pixels);
            colormapTexture.Apply();
            colorMat.SetTexture("_Colormap", colormapTexture);
        }
        public bool intHasChanged(int A, int B)
        {
            bool result = false;
            if (B != A)
            {
                A = B;
                result = true;
            }
            return result;
        }
        static Color RandomColor()
        {
            return new Color(Random.value, Random.value, Random.value, Random.value);
        }
        void SetUpResources()
        {
            if (tempPreset.g_GlitchMat2 != null) return;

            tempPreset.g_GlitchMat2 = new Material(Shader.Find("RetroLookPro/Legacy/Glitch2RetroLook"))
            {

                hideFlags = HideFlags.DontSave
            };
            Vector2Int texVec = new Vector2Int((int)(tempPreset.g_2Res * 64), (int)(tempPreset.g_2Res * 32));
            _noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false)
            {

                hideFlags = HideFlags.DontSave,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };

            _trashFrame1 = new RenderTexture(Screen.width, Screen.height, 0)
            {
                hideFlags = HideFlags.DontSave
            };
            _trashFrame2 = new RenderTexture(Screen.width, Screen.height, 0)
            {
                hideFlags = HideFlags.DontSave
            };

            UpdateNoiseTexture();
        }

        void UpdateNoiseTexture()
        {
            Color color = RandomColor();

            for (var y = 0; y < _noiseTexture.height; y++)
            {
                for (var x = 0; x < _noiseTexture.width; x++)
                {
                    if (Random.value > 0.89f) color = RandomColor();
                    _noiseTexture.SetPixel(x, y, color);
                }
            }

            _noiseTexture.Apply();
        }
    }
    public enum ResolutionMode
    {
        ConstantResolution,
        ConstantPixelSize,
    }
}
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;





public class CameraControl : MonoBehaviour
{

    public Volume _globalVolume;
    private Bloom _bloom;
    private DepthOfField _depthOfField;
    private ProbeVolumesOptions _probeVolumesOptions;
    private FilmGrain _filmGrain;
    private LensDistortion _lensDistorsion;
    private ShadowsMidtonesHighlights _shadowsMidtonesHighlights;
    private PaniniProjection _paniniProjection;
    private ChromaticAberration _chromaticAberration;
    private Vignette _vignette;
    private MotionBlur _motionblur;
    private Tonemapping _tonemapping;
    private ColorAdjustments _coloradjustements;

    [System.Serializable]
    public class postproSettings
    {


        public bool useComplexLerp = true;
        public bool useBloom = true;
        public bool useLensDistortion = true;
        public bool useChromaticAberration = true;
        public bool useVignette = true;
        public bool useColorAdjustements = true;
        [Space(10)]

        [Header("BLOOM")]
        [Range(0f, 1f)]
        public float scatter = 1f;
        public Vector2 scatterInterpolation = new(0f, 1f);
        public bool useScatter = true; public bool scatterDoInterpolation = false;
        [Space(5)]

        [Min(0f)]
        public float bloomIntensity = 1.46f;
        public Vector2 bloomIntensityInterpolation = new(0f, 1f);
        public bool useBloomIntensity = true; public bool bloomIntensityDoInterpolation = false;
        [Space(5)]

        public Color bloomTint = Color.white;
        public Color[] bloomTintInterpolation = new Color[2];
        public bool useBloomTint = true; public bool bloomTintDoInterpolation = false;

        [Space(5)]

        [Header("LENS DISTORTION")]
        [Range(0f, 1f)]
        public float lensDistortionIntensity = 0.0f;
        public Vector2 lensDistortionIntensityInterpolation = new(0f, 1f);
        public bool useLensDistortionIntensity = true; public bool lensDistortionIntensityDoInterpolation = false;
        [Space(10)]

        [Header("CHROMATIC ABERRATION")]
        [Range(0f, 1f)]
        public float _chromaticAberrationIntensity = 0.195f;
        public Vector2 _chromaticAberrationInterpolation = new(0.195f, 1f);
        public bool useChromaticAberrationIntensity = true; public bool chromaticAberrationIntensityDoInterpolation = false;
        [Space(10)]

        [Header("VIGNETTE")]
        [Min(0f)]
        public float vignetteIntensity = 1.46f;
        public Vector2 vignetteIntensityInterpolation = new(0f, 1f);
        public bool useVignetteIntensity = true; public bool vignetteIntensityDoInterpolation = false;
        [Space(5)]

        public Color vignetteColor = Color.white;
        public Color[] vignetteColorInterpolation = new Color[2];
        public bool useVignetteColor = true; public bool vignetteColorDoInterpolation = false;
        [Space(10)]

        [Header("COLOR ADJUSTEMENTS")]

        [Min(0f)]
        public float postExposure = 0f;
        public Vector2 postExposureInterpolation = new(0, 1f);
        public bool usePostExposure = true; public bool postExposureDoInterpolation = false;
        [Space(5)]

        [Range(-100f, 100f)]
        public float contrast = 0f;
        public Vector2 contrastInterpolation = new(0, 1f);
        public bool useContrast = true; public bool contrastDoInterpolation = false;
        [Space(5)]

        public Color colorFilter = Color.white;
        public Color[] colorFilterInterpolation = new Color[2];
        public bool useColorFilter = true; public bool colorFilterDoInterpolation = false;
        [Space(5)]

        [Range(-180f, 180f)]
        public float hueShift = 0f;
        public Vector2 hueShiftInterpolation = new(0, 1f);
        public bool useHueShift = true; public bool hueShiftDoInterpolation = false;
        [Space(5)]

        [Range(-100f, 100f)]
        public float saturation = 0f;
        public Vector2 saturationInterpolation = new(0, 1f);
        public bool useSaturation = true; public bool saturationDoInterpolation = false;
    }

    public int chill_normal_heavy = 0;
    public postproSettings[] postproModes;
    private void OnValidate()
    {
        setValues();
    }
    private void setValues()
    {
        if (_bloom)
        {
            _bloom.active = postproModes[chill_normal_heavy].useBloom;
            _bloom.scatter.Override(postproModes[chill_normal_heavy].scatter); _bloom.scatter.overrideState = postproModes[chill_normal_heavy].useScatter;
            _bloom.intensity.Override(postproModes[chill_normal_heavy].bloomIntensity); _bloom.intensity.overrideState = postproModes[chill_normal_heavy].useBloomIntensity;
            _bloom.tint.Override(postproModes[chill_normal_heavy].bloomTint); _bloom.tint.overrideState = postproModes[chill_normal_heavy].useBloomTint;
        }
        if (_lensDistorsion)
        {
            _lensDistorsion.active = postproModes[chill_normal_heavy].useLensDistortion;
            _lensDistorsion.intensity.Override(postproModes[chill_normal_heavy].lensDistortionIntensity); _lensDistorsion.intensity.overrideState = postproModes[chill_normal_heavy].useLensDistortionIntensity;
        }
        if (_chromaticAberration)
        {
            _chromaticAberration.active = postproModes[chill_normal_heavy].useChromaticAberration;
            _chromaticAberration.intensity.Override(postproModes[chill_normal_heavy]._chromaticAberrationIntensity); _chromaticAberration.intensity.overrideState = postproModes[chill_normal_heavy].useChromaticAberrationIntensity;
        }
        if (_vignette)
        {
            _vignette.active = postproModes[chill_normal_heavy].useVignette;
            _vignette.intensity.Override(postproModes[chill_normal_heavy].vignetteIntensity); _vignette.intensity.overrideState = postproModes[chill_normal_heavy].useVignetteIntensity;
            _vignette.color.Override(postproModes[chill_normal_heavy].vignetteColor); _vignette.color.overrideState = postproModes[chill_normal_heavy].useVignetteColor;
        }
        if (_coloradjustements)
        {
            _coloradjustements.active = postproModes[chill_normal_heavy].useColorAdjustements;
            _coloradjustements.hueShift.Override(postproModes[chill_normal_heavy].hueShift); _coloradjustements.hueShift.overrideState = postproModes[chill_normal_heavy].useColorAdjustements;
            _coloradjustements.saturation.Override(postproModes[chill_normal_heavy].saturation); _coloradjustements.saturation.overrideState = postproModes[chill_normal_heavy].useSaturation;
            _coloradjustements.colorFilter.Override(postproModes[chill_normal_heavy].colorFilter); _coloradjustements.colorFilter.overrideState = postproModes[chill_normal_heavy].useColorAdjustements;
            _coloradjustements.postExposure.Override(postproModes[chill_normal_heavy].postExposure); _coloradjustements.postExposure.overrideState = postproModes[chill_normal_heavy].usePostExposure;
            _coloradjustements.contrast.Override(postproModes[chill_normal_heavy].contrast); _coloradjustements.contrast.overrideState = postproModes[chill_normal_heavy].useContrast;
        }
    }
    void Update()
    {
        chill_normal_heavy = (int)LevelManager._instance.actualState;
        setValues();

        // Modifica el intervalo entre beats para que tenga una curva exponencial
        float modifier = postproModes[chill_normal_heavy].useComplexLerp ? 1f : 0.5f;
        float beatTime = BeatManager.GetCurrentTime() % BeatManager.GetBeatInterval();
        float t = beatTime / (BeatManager.GetBeatInterval() * modifier);

        // Asegúrate de que t esté en el rango [0, 1]
        t = Mathf.Clamp01(t);

        // Curva exponencial simétrica: va de mínimo a máximo y regresa al mínimo
        float finalTime;
        if (postproModes[chill_normal_heavy].useComplexLerp)
        {
            // Ajustamos para que la interpolación sea simétrica y con comportamiento exponencial ida y vuelta
            if (t < 0.5f)
            {
                finalTime = Mathf.Pow(t * 2f, 2); // Primera mitad (subida)
            }
            else
            {
                float invertedT = 1f - t; // Reflejar para la segunda mitad (bajada)
                finalTime = Mathf.Pow(invertedT * 2f, 2); // Segunda mitad (bajada)
            }
        }
        else
        {
            // Lerp simple
            finalTime = t;
        }
        float value; Color valueColor;
        if (_bloom.active)
        {
            if (postproModes[chill_normal_heavy].useBloomIntensity && postproModes[chill_normal_heavy].bloomIntensityDoInterpolation)
            {
                value = Mathf.Lerp(postproModes[chill_normal_heavy].bloomIntensityInterpolation.x, postproModes[chill_normal_heavy].bloomIntensityInterpolation.y, finalTime);
                _bloom.intensity.Override(value);
            }
            if (postproModes[chill_normal_heavy].useBloomTint && postproModes[chill_normal_heavy].bloomTintDoInterpolation)
            {
                valueColor = Color.Lerp(postproModes[chill_normal_heavy].bloomTintInterpolation[0], postproModes[chill_normal_heavy].bloomTintInterpolation[1], finalTime);
                _bloom.tint.Override(valueColor);
            }
            if (postproModes[chill_normal_heavy].useScatter && postproModes[chill_normal_heavy].scatterDoInterpolation)
            {
                value = Mathf.Lerp(postproModes[chill_normal_heavy].scatterInterpolation.x, postproModes[chill_normal_heavy].scatterInterpolation.y, finalTime);
                _bloom.scatter.Override(value);
            }
        }
        if (_lensDistorsion.active)
        {
            if (postproModes[chill_normal_heavy].useLensDistortionIntensity && postproModes[chill_normal_heavy].lensDistortionIntensityDoInterpolation)
            {
                value = Mathf.Lerp(postproModes[chill_normal_heavy].lensDistortionIntensityInterpolation.x, postproModes[chill_normal_heavy].lensDistortionIntensityInterpolation.y, finalTime);
                _lensDistorsion.intensity.Override(value);
            }

        }
        if (_vignette.active)
        {
            if (postproModes[chill_normal_heavy].useVignetteIntensity && postproModes[chill_normal_heavy].vignetteColorDoInterpolation)
            {
                valueColor = Color.Lerp(postproModes[chill_normal_heavy].vignetteColorInterpolation[0], postproModes[chill_normal_heavy].vignetteColorInterpolation[1], finalTime);
                _vignette.color.Override(valueColor);
            }

            if (postproModes[chill_normal_heavy].useVignetteColor && postproModes[chill_normal_heavy].vignetteIntensityDoInterpolation)
            {
                value = Mathf.Lerp(postproModes[chill_normal_heavy].vignetteIntensityInterpolation.x, postproModes[chill_normal_heavy].vignetteIntensityInterpolation.y, finalTime);
                _vignette.intensity.Override(value);
            }
        }
        if (_chromaticAberration.active)
        {
            if (postproModes[chill_normal_heavy].useChromaticAberrationIntensity && postproModes[chill_normal_heavy].chromaticAberrationIntensityDoInterpolation)
            {
                value = Mathf.Lerp(postproModes[chill_normal_heavy]._chromaticAberrationInterpolation.x, postproModes[chill_normal_heavy]._chromaticAberrationInterpolation.y, finalTime);
                _chromaticAberration.intensity.Override(value);
            }
        }
        if (_coloradjustements.active)
        {
            if (postproModes[chill_normal_heavy].usePostExposure && postproModes[chill_normal_heavy].postExposureDoInterpolation)
            {
                value = Mathf.Lerp(postproModes[chill_normal_heavy].postExposureInterpolation.x, postproModes[chill_normal_heavy].postExposureInterpolation.y, finalTime);
                _coloradjustements.postExposure.Override(value);
            }
            if (postproModes[chill_normal_heavy].useContrast && postproModes[chill_normal_heavy].contrastDoInterpolation)
            {
                value = Mathf.Lerp(postproModes[chill_normal_heavy].contrastInterpolation.x, postproModes[chill_normal_heavy].contrastInterpolation.y, finalTime);
                _coloradjustements.contrast.Override(value);
            }
            if (postproModes[chill_normal_heavy].useHueShift && postproModes[chill_normal_heavy].hueShiftDoInterpolation)
            {
                value = Mathf.Lerp(postproModes[chill_normal_heavy].hueShiftInterpolation.x, postproModes[chill_normal_heavy].hueShiftInterpolation.y, finalTime);
                _coloradjustements.hueShift.Override(value);
            }
            if (postproModes[chill_normal_heavy].useSaturation && postproModes[chill_normal_heavy].saturationDoInterpolation)
            {
                value = Mathf.Lerp(postproModes[chill_normal_heavy].saturationInterpolation.x, postproModes[chill_normal_heavy].saturationInterpolation.y, finalTime);
                _coloradjustements.saturation.Override(value);
            }
            if (postproModes[chill_normal_heavy].useColorFilter && postproModes[chill_normal_heavy].colorFilterDoInterpolation)
            {
                valueColor = Color.Lerp(postproModes[chill_normal_heavy].colorFilterInterpolation[0], postproModes[chill_normal_heavy].colorFilterInterpolation[1], finalTime);
                _coloradjustements.colorFilter.Override(valueColor);
            }
        }

    }



    private void Start()
    {
        setValues();
    }
    private void Awake()
    {
        if (_globalVolume != null && _globalVolume.profile != null)
        {
            VolumeProfile profile = _globalVolume.profile;

            // Modificar Bloom
            profile.TryGet(out _bloom);
            if (!_bloom) Debug.LogError("NO TIENE BLOOM!!!");
            profile.TryGet(out _depthOfField);
            if (!_depthOfField) Debug.LogError("NO TIENE depthOfField!!!");
            profile.TryGet(out _probeVolumesOptions);
            if (!_probeVolumesOptions) Debug.LogError("NO TIENE PROBE VOLUME OPTIONS !!!");
            profile.TryGet(out _filmGrain);
            if (!_filmGrain) Debug.LogError("NO TIENE FILM GRAIN!!!");
            profile.TryGet(out _lensDistorsion);
            if (!_lensDistorsion) Debug.LogError("NO TIENE LENS DISTORTION!!!");
            profile.TryGet(out _shadowsMidtonesHighlights);
            if (!_shadowsMidtonesHighlights) Debug.LogError("NO TIENE SHADOWS MIDTONES HIGHLIGHTS!!!");
            profile.TryGet(out _paniniProjection);
            if (!_paniniProjection) Debug.LogError("NO TIENE PANINI PROJECTIONS!!!");
            profile.TryGet(out _chromaticAberration);
            if (!_chromaticAberration) Debug.LogError("NO TIENE CHROMATIC ABERRATION!!!");
            profile.TryGet(out _vignette);
            if (!_vignette) Debug.LogError("NO TIENE VIGNETE!!!");
            profile.TryGet(out _motionblur);
            if (!_motionblur) Debug.LogError("NO TIENE MOTION BLUR!!!");
            profile.TryGet(out _tonemapping);
            if (!_tonemapping) Debug.LogError("NO TIENE TONE MAPPING!!!");
            profile.TryGet(out _coloradjustements);
            if (!_coloradjustements) Debug.LogError("NO TIENE COLOR ADJUSTEMETS!!!");
        }
        else
        {
            Debug.LogError("peto por el global volume amego");
        }
    }


}

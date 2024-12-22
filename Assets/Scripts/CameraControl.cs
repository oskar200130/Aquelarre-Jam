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

    private void OnValidate()
    {
        if (_bloom)
        {
            _bloom.active = useBloom;
            _bloom.scatter.Override(scatter); _bloom.scatter.overrideState = useScatter;
            _bloom.intensity.Override(bloomIntensity); _bloom.intensity.overrideState = useBloomIntensity;
            _bloom.tint.Override(bloomTint); _bloom.tint.overrideState = useBloomTint;
        }
        if (_lensDistorsion)
        {
            _lensDistorsion.active = useLensDistortion;
            _lensDistorsion.intensity.Override(lensDistortionIntensity); _lensDistorsion.intensity.overrideState = useLensDistortionIntensity;
        }
        if (_chromaticAberration)
        {
            _chromaticAberration.active = useChromaticAberration;
            _chromaticAberration.intensity.Override(_chromaticAberrationIntensity); _chromaticAberration.intensity.overrideState = useChromaticAberrationIntensity;
        }
        if (_vignette)
        {
            _vignette.active = useVignette;
            _vignette.intensity.Override(vignetteIntensity); _vignette.intensity.overrideState = useVignetteIntensity;
            _vignette.color.Override(vignetteColor); _vignette.color.overrideState = useVignetteColor;
        }
        if (_coloradjustements)
        {
            _coloradjustements.active = useColorAdjustements;
            _coloradjustements.hueShift.Override(hueShift); _coloradjustements.hueShift.overrideState = useColorAdjustements;
            _coloradjustements.saturation.Override(saturation); _coloradjustements.saturation.overrideState = useSaturation;
            _coloradjustements.colorFilter.Override(colorFilter); _coloradjustements.colorFilter.overrideState = useColorAdjustements;
            _coloradjustements.postExposure.Override(postExposure); _coloradjustements.postExposure.overrideState = usePostExposure;
            _coloradjustements.contrast.Override(contrast); _coloradjustements.contrast.overrideState = useContrast;
        }
    }

    void Update()
    {
        // Modifica el intervalo entre beats para que tenga una curva exponencial
        float modifier = useComplexLerp ? 1f : 0.5f;
        float beatTime = BeatManager.GetCurrentTime() % BeatManager.GetBeatInterval();
        float t = beatTime / (BeatManager.GetBeatInterval() * modifier);

        // Asegúrate de que t esté en el rango [0, 1]
        t = Mathf.Clamp01(t);

        // Curva exponencial simétrica: va de mínimo a máximo y regresa al mínimo
        float finalTime;
        if (useComplexLerp)
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
            if (useBloomIntensity &&  bloomIntensityDoInterpolation)
            {
                value = Mathf.Lerp(bloomIntensityInterpolation.x, bloomIntensityInterpolation.y, finalTime);
                _bloom.intensity.Override(value);
            }
            if (useBloomTint && bloomTintDoInterpolation)
            {
                valueColor = Color.Lerp(bloomTintInterpolation[0], bloomTintInterpolation[1], finalTime);
                _bloom.tint.Override(valueColor);
            }
            if(useScatter && scatterDoInterpolation)
            {
                value = Mathf.Lerp(scatterInterpolation.x, scatterInterpolation.y, finalTime);
                _bloom.scatter.Override(value);
            }
        }
        if (_lensDistorsion.active)
        {
            if (useLensDistortionIntensity && lensDistortionIntensityDoInterpolation)
            {
                value = Mathf.Lerp(lensDistortionIntensityInterpolation.x, lensDistortionIntensityInterpolation.y, finalTime);
                _lensDistorsion.intensity.Override(value);
            }

        }
        if (_vignette.active)
        {
            if (useVignetteIntensity && vignetteColorDoInterpolation)
            {
                valueColor = Color.Lerp(vignetteColorInterpolation[0], vignetteColorInterpolation[1], finalTime);
                _vignette.color.Override(valueColor);
            }

            if (useVignetteColor &&  vignetteIntensityDoInterpolation)
            {
                value = Mathf.Lerp(vignetteIntensityInterpolation.x, vignetteIntensityInterpolation.y, finalTime);
                _vignette.intensity.Override(value);
            }
        }
        if (_chromaticAberration.active)
        {
            if (useChromaticAberrationIntensity && chromaticAberrationIntensityDoInterpolation)
            {
                value = Mathf.Lerp(_chromaticAberrationInterpolation.x, _chromaticAberrationInterpolation.y, finalTime);
                _chromaticAberration.intensity.Override(value);
            }
        }
        if (_coloradjustements.active)
        {
            if (usePostExposure && postExposureDoInterpolation)
            {
                value = Mathf.Lerp(postExposureInterpolation.x, postExposureInterpolation.y, finalTime);
                _coloradjustements.postExposure.Override(value);
            }
            if (useContrast && contrastDoInterpolation)
            {
                value = Mathf.Lerp(contrastInterpolation.x, contrastInterpolation.y, finalTime);
                _coloradjustements.contrast.Override(value);
            }
            if (useHueShift && hueShiftDoInterpolation)
            {
                value = Mathf.Lerp(hueShiftInterpolation.x, hueShiftInterpolation.y, finalTime);
                _coloradjustements.hueShift.Override(value);
            }
            if (useSaturation && saturationDoInterpolation)
            {
                value = Mathf.Lerp(saturationInterpolation.x, saturationInterpolation.y, finalTime);
                _coloradjustements.saturation.Override(value);
            }
            if (useColorFilter && colorFilterDoInterpolation)
            {
                valueColor = Color.Lerp(colorFilterInterpolation[0], colorFilterInterpolation[1], finalTime);
                _coloradjustements.colorFilter.Override(valueColor);
            }
        }

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

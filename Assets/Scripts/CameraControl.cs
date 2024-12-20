using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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


    //esto lo voy a dejar tal que en el update cambie todo el rato el valor de cada una de estas variables si es que han cambiado,
    //para gestionarlas desde el editor sin necesidad de estar haciendo un script de editor.
    //el update de esta funcion se debe BORRAR antes de hacer la build
    [Range(0f, 1f)]
    public float _bloomScatter = 1f;


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
            Debug.LogError("petó por el global volume amego");
        }
    }
    private void OnValidate()
    {
        if (_bloom) _bloom.scatter.Override(_bloomScatter);
        
    }
    
    
}

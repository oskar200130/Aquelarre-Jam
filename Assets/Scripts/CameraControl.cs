using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;


/*
 
 PARAMETRIZAR:
BLOOM ->
    INTENSITY, SCATTER, TINT
lens distortion
    intensity
chromatic aberration
    intensity
vignette
    color, intensity
Color adjustements
    todo

 
 */


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


    public bool debugpingpong = false;

    [Range(0f, 1f)]
    public float _bloomScatter = 1f;
    //ESTE es el valor que habria que modificar segun las fases del juego.
    //Es decir, si estamos en la fase normal, pues el minimo y el maximo del ping pong es X. Si va mas a tope por los combos,
    //se cambia este minimo y maximo y entonces puede ser mas apoteosico.
    //Ademas, la idea seria bloquear o no algunas de estas variables seg�n esa fase. En plan: solo queremos que el chromatic aberration exista en la fase de tener muchos combos, pues bloqueamos.
    public Vector2 scatterPingPong = new(0f, 1f);

    [Min(0f)]
    public float _bloomItensity = 1.46f;
    [Min(0f)]
    public float _bloomDirtIntensity = 4.1f;

    //DEPTH OF FIELD
    public DepthOfFieldMode depthOfFieldmode = DepthOfFieldMode.Gaussian;
    [Min(0f)]
    public float dofGaussianStart = 20.93f;
    [Range(0.5f, 1.5f)]
    public float dofGaussianMaxRadius = 1.5f;

    //FILM GRAIN
    [Range(0f, 1f)]
    public float _filmGrainIntensity = 1f;

    //CHROMATIC ABERRATION
    [Range(0f, 1f)]
    public float _chromaticAberrationIntensity = 0.195f;
    public Vector2 _chromaticAberrationPingPong = new(0.195f, 1f);

    //etc, ahora me da pereza. en el onvalidate, a�adir cada uno de estos sets en caso de que se quiera toquetear en ejecucion.
    //para cambiar los valores fuera de aqui, simplemente hay que acceder a las variables publicas de arriba (bloom, depth, etc) y hacer override en el valor que toque
    private void OnValidate()
    {
        if (_bloom)
        {
            _bloom.scatter.Override(_bloomScatter);
            _bloom.intensity.Override(_bloomItensity);
            _bloom.dirtIntensity.Override(_bloomDirtIntensity);
        }
        if (_depthOfField)
        {
            _depthOfField.mode.Override(depthOfFieldmode);
            _depthOfField.gaussianStart.Override(dofGaussianStart);
            _depthOfField.gaussianMaxRadius.Override(dofGaussianMaxRadius);
        }
        if (_filmGrain)
        {
            _filmGrain.intensity.Override(_filmGrainIntensity);
        }
        if (_chromaticAberration)
        {
            _chromaticAberration.intensity.Override(_chromaticAberrationIntensity);
        }
    }

    void Update()
    {
        if (debugpingpong)
        {
            // Calcula t en el rango [0, 1]
            float t = Mathf.PingPong(BeatManager.GetCurrentTime() / (BeatManager.GetBeatInterval()), 1f);

            // Aplica una curva exponencial al tiempo t para que llegue r�pidamente a los l�mites
            float curvedT = Mathf.Pow(t, 3) * (t < 0.1f ? 1f : -1f) + Mathf.Clamp01(t);

            // Interpolaci�n exponencial para scatterPingPong
            float value = Mathf.Lerp(scatterPingPong.x, scatterPingPong.y, curvedT);
            _bloom.scatter.Override(value);

            // Interpolaci�n exponencial para _chromaticAberrationPingPong
            value = Mathf.Lerp(_chromaticAberrationPingPong.x, _chromaticAberrationPingPong.y, curvedT);
            _chromaticAberration.intensity.Override(value);
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

            //setea al parametro iniciar en caso de hacer pingpong
            _bloom.scatter.Override(scatterPingPong.x);
            _chromaticAberration.intensity.Override(_chromaticAberrationPingPong.x);
        }
        else
        {
            Debug.LogError("pet� por el global volume amego");
        }
    }


}

using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;





public class BeatManager : MonoBehaviour
{
    public static BeatManager _instance { get; private set; }
    /// <summary>
    /// tiempos y contadores de compas y cancion en general
    /// </summary>
    [HideInInspector] public float beatInterval { get; private set; } //intervalo de tiempo en segundos que dura un pulso
    [HideInInspector] public float measureInterval { get; private set; } //intervalo de tiempo en segundos que dura un compas
    [HideInInspector] public int current_beat { get; private set; } = 0;
    [HideInInspector] public int current_measure { get; private set; } = 0;

    /// <summary>
    /// medidas del compas
    /// </summary>
    [SerializeField] private float _BPM = 120f;
    //al 100%, comprueba el golpe en el pulso exacto. A menos valor, mas dificil, a mas valor, mas permisivo.
    [SerializeField] private float _tolerance = 1f;
    [Tooltip("numero de beats")][SerializeField] private int _numerador = 4;
    [Tooltip("tiempo de beat (corchea, negra, etc ya saben)")][SerializeField] private int _denominador = 4;

    private float current_beat_time = 0f;



    public int denominador
    {
        get { return _denominador; }
        set
        {
            _denominador = value;
            calculateIntervals();
        }
    }
    public int numerador
    {
        get { return _numerador; }
        set
        {
            _numerador = value;
            calculateIntervals();
        }
    }
    public float BPM
    {
        get { return _BPM; }
        set
        {
            _BPM = value;
            calculateIntervals();
        }
    }
    
    private RITMOS current_measure_ritmo = RITMOS.NORMAL; //seguira el ritmo marcado para evaluar clicks en el compas X. Se cambiara segun avance la cancion

    //SCORE: Esto igual tiene mas sentido meterlo en otro lao, donde se gestionen los puntos
    private int queueSize = 10;
    private Queue<SCORE> scores;
    public void AddScore(SCORE newScore)
    {
        if (scores.Count >= 10) scores.Dequeue();
        scores.Enqueue(newScore);
    }
    public SCORE[] GetScores()
    {
        return scores.ToArray();
    }
    public SCORE GetLatestScore()
    {
        return scores.Count > 0 ? scores.Peek() : SCORE.NONE;
    }



    /// <summary>
    /// eventos de beats
    /// </summary>
    [HideInInspector] public UnityEvent OnPulse; //resto de pulsos
    [HideInInspector] public UnityEvent OnMeasure; //inicio de cada compás (no habria por que usar esto, se puede usar solo onPulse y comprobar el numero del compas)
    private FMOD.Studio.EventInstance musicEventInstance;


    private void Awake()
    {
        if (_instance == null)
        {
            Debug.Log("BeatManager instanced");
            _instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        scores = new Queue<SCORE>(queueSize);
        calculateIntervals();
        musicEventInstance = RuntimeManager.CreateInstance("event:/Music");
    }


    private void calculateIntervals()
    {
        beatInterval = (60f / BPM) * (4f / _denominador);
        measureInterval = beatInterval * numerador;
    }
    // Llamado cuando cambian las propiedades en el Inspector
    private void OnValidate()
    {
        calculateIntervals();
    }

    private void Start()
    {
        if (OnPulse == null) OnPulse = new UnityEvent();

        if (OnMeasure == null) OnMeasure = new UnityEvent();

        StartCoroutine(Beat());
    }
    private IEnumerator Beat()
    {
        while (true)
        {
            OnPulse.Invoke();

            if (current_beat == 0)
            {
                if (current_measure == 0) musicEventInstance.start();
                OnMeasure.Invoke();
            }

            current_beat = (current_beat + 1) % numerador;
            if (current_beat == 0) current_measure++;
            current_beat_time = Time.time;

            yield return new WaitForSeconds(beatInterval);
        }
    }

    public SCORE evaluateClick(float clickTime)
    {

        float timeDifference = Mathf.Abs(clickTime - current_beat_time);
              

        float accuracy = 1- (timeDifference / _tolerance);


        SCORE res;
        if (accuracy < 0.3f) res = SCORE.TERRIBLE;
        else if (accuracy < 0.6f) res = SCORE.MID;
        else if (accuracy < 0.80f) res = SCORE.COOL;
        else if (accuracy < 0.95f) res = SCORE.PERFECT;
        else res = SCORE.HEAVY;
        Debug.Log($"Beat {current_beat}: {accuracy} , {res} !! ---- Click on: {clickTime}, beat on: {current_beat_time}");


        AddScore(res);
        return res;
    }

}
public enum SCORE
{
    NONE, //NA
    TERRIBLE, //<0-30%
    MID, //<31-60%
    COOL, //<61-85%
    PERFECT, //<86-90%
    HEAVY //95-100%
}

public enum RITMOS
{
    NONE, //no contaremos puntos en estos compases
    NORMAL, //cada beat
    DOBLES, //dos clicks por beat
    MITAD, //1 click cada dos beats
    TRESILLOS //3 clicks por beat???? de locos este
              //etc

}
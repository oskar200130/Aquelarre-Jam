using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager _instance { get; private set; }
    /// <summary>
    /// tiempos y contadores de compas y cancion en general
    /// </summary>
    [HideInInspector] public float pulseInterval { get; private set; } //intervalo de tiempo en segundos que dura un pulso
    [HideInInspector] public float measureInterval { get; private set; } //intervalo de tiempo en segundos que dura un compas
    [HideInInspector] public int current_beat { get; private set; } = 0;
    [HideInInspector] public int current_measure { get; private set; } = 0;

    /// <summary>
    /// medidas del compas
    /// </summary>
    [SerializeField] private float _BPM = 120f;
    [Tooltip("numero de beats")][SerializeField] private int _numerador = 4;
    [Tooltip("tiempo de beat (corchea, negra, etc ya saben)")][SerializeField] private int _denominador = 4;

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


    /// <summary>
    /// eventos de beats
    /// </summary>
    [HideInInspector]public UnityEvent OnPulse; //resto de pulsos
    [HideInInspector]public UnityEvent OnMeasure; //inicio de cada compás (no habria por que usar esto, se puede usar solo onPulse y comprobar el numero del compas)


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

        calculateIntervals();
    }


    private void calculateIntervals()
    {
        pulseInterval = (60f / BPM) * (4f / _denominador);
        measureInterval = pulseInterval * numerador;
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
                OnMeasure.Invoke();
            }

            current_beat = (current_beat + 1) % numerador;
            if (current_beat == 0) current_measure++;
            yield return new WaitForSeconds(pulseInterval);
        }
    }
}

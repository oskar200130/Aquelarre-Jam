using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager _instance { get; private set; }

    //SCORE: Esto igual tiene mas sentido meterlo en otro lao, donde se gestionen los puntos
    private int queueSize = 10;
    private Queue<SCORE> scores;
    public int puntuacion = 1;
    public Animator beatMarkerContainerAnimator;
    public Animator goatsController;
    public StudioEventEmitter thunder;
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
    //Rangos que multiplicaran la puntuacion / público
    public float constantPeopleProbability = 0.7f;
    public float percentageConstant = 0.00001f;
    public float percentageCool = 0.0003f;
    public float percentagePerfect = 0.0005f;
    public float percentageHeavy = 0.001f;

    [SerializeField] SpeakerParticle[] speakers;
    private int counter_beats = 0; private int counter_measures = 0;
    public TMP_Text pointsText;

    public STATES actualState = STATES.NORMAL;
    bool cabraLoca = false;

    public int nFreestyles = 3; public float freestyleProbability = 0.35f;  private int doneFreestyles = 0; bool canFreestyle = true;
    public GameObject freestyleGO;
    private int freestyleStartMeasure = 0;

    private void Awake()
    {
        if (_instance == null)
        {
            Debug.Log("LevelManager instanced");
            _instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        scores = new Queue<SCORE>(queueSize);
        beatMarkerContainerAnimator.speed = 0;


        BeatManager.onFixedBeat += metronome;
        BeatManager.onTempoChanged += tempoChanged;
    }

    void tempoChanged(float beatInterval)
    {
        if (beatInterval < 0.6) //180 BPM
        {
            beatMarkerContainerAnimator.speed = 2;
        }
        else
        {
            if (counter_measures > 0) //para comprobar que ha empezado la cancion
                beatMarkerContainerAnimator.speed = 1;

        }
    }

    void metronome()
    {

        counter_beats = (counter_beats + 1) % 4; //hardcodeado a 4/4
        if (counter_beats == 0) counter_measures += 1;
        checkSongStates();

        //text.text = $"Compás {counter_measures}, pulso {counter_beats}";
        //Debug.Log($"Compás {counter_measures+1}, pulso {counter_beats+1}");

        //añade de manera aleatoria a gente al escenario de forma constante
        if (Random.Range(0, 1.01f) < constantPeopleProbability)
        {
            //  addPointsByFloat(percentageConstant);
        }

    }
    public bool gameStarted = false;
    private void Update()
    {
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("empezando juego");
            BeatManager._instance.playSong();

            gameStarted = true;
            beatMarkerContainerAnimator.speed = 1;

        }
        if (!BeatManager.isPlayingMusic && gameStarted)
        {
            //para volver a empezar
            gameStarted = false;
            beatMarkerContainerAnimator.speed = 0;
            BeatManager._instance.playMetronome(true);
            doneFreestyles = 0;
            freestyleGO.SetActive(false);
            freestyleStartMeasure = 0; 

        }

        if(gameStarted && counter_measures == 1)
            BeatManager._instance.playMetronome(false);

        ////si esta el texto en pantalla
        //if (!canFreestyle)
        //{
        //    if (counter_measures >= freestyleStartMeasure)
        //    {
        //        canFreestyle = true;
        //        freestyleGO.SetActive(false);
        //    }
        //}

    }
    //checkpoints de la cancion donde cambia la intensidad de las cosas en pantalla
    //esto deberia hacerse con eventos de fmod PERO ME DA UNA PEREZA IMPRESIONANTE
    //EN PLAN
    //IMPRESIONANTE, SON LAS 2:17 NO ME PODRIA DAR MAS PEREZA EFE MOD
    private void checkSongStates()
    {
        float freeprob;
        //cambia de estado de la cancion en compases especificos
        switch (counter_measures +1)
        {
            case 1:
            case 26:
            case 34:
            case 94:
                //if (counter_beats == 0)
                //{
                //    freeprob = Random.Range(0f, 1f);
                //    Debug.Log($"{canFreestyle && doneFreestyles < nFreestyles && freeprob < freestyleProbability}, {freeprob}");
                //    if (canFreestyle && doneFreestyles < nFreestyles && freeprob < freestyleProbability)
                //    {
                //        freestyleGO.SetActive(true);
                //        freestyleStartMeasure = counter_measures;
                //        canFreestyle = false;
                //        doneFreestyles++;
                //    }
                //}
                if (actualState == STATES.CHILL) return;
                actualState = STATES.CHILL;
                Debug.Log("cambio a estado chill, trankilitos");
                break;
            case 10:
            case 30:
            case 38:
            case 54:
                //if (counter_beats == 0)
                //{
                //    freeprob = Random.Range(0f, 1f);
                //    Debug.Log($"{canFreestyle && doneFreestyles < nFreestyles && freeprob < freestyleProbability}, {freeprob}");
                //    if (canFreestyle && doneFreestyles < nFreestyles && freeprob < freestyleProbability)
                //    {
                //        freestyleGO.SetActive(true);
                //        freestyleStartMeasure = counter_measures;
                //        canFreestyle = false;
                //        doneFreestyles++;
                //    }
                //}
                if (actualState == STATES.NORMAL) return;
                actualState = STATES.NORMAL;
                Debug.Log("cambio a estado Normal, se pone intensillo");
                break;

            case 45:
            case 86: //final
                //if (counter_beats == 0)
                //{
                //    freeprob = Random.Range(0f, 1f);
                //    Debug.Log($"{canFreestyle && doneFreestyles < nFreestyles && freeprob < freestyleProbability}, {freeprob}");
                //    if (canFreestyle && doneFreestyles < nFreestyles && freeprob < freestyleProbability)
                //    {
                //        freestyleGO.SetActive(true);
                //        freestyleStartMeasure = counter_measures;
                //        canFreestyle = false;
                //        doneFreestyles++;
                //    }
                //}
                if (actualState == STATES.HEAVY) return;
                mostrarCabra();
                actualState = STATES.HEAVY;
                Debug.Log("cambio a estado HEAVY, WOOOOOOOOOOO");
                break;
            default:
                break;

        }
    }

    public void mostrarCabra()
    {
        if (cabraLoca) return;
        goatsController.SetTrigger("omg");
        thunder.Play();
        Debug.Log("LA CABRAAAAAAAAAAA");
        cabraLoca = true;
    }

    private void Start()
    {
        actualState = STATES.CHILL;
    }

    public void addPointsByFloat(float percentage)
    {
        puntuacion += Mathf.CeilToInt(puntuacion * percentage);
        pointsText.text = $"PEOPLE: {puntuacion}";

    }
    public void addPointsByScore(SCORE s)
    {

        foreach (SpeakerParticle speaker in speakers)
            speaker.playParticle(s);
        //nota, spawmear permite dar al final con todas las notas, ya sean aciertos o fallos y subir siempre, habria qu elimitar el que solo s epudea hacer uan vez cada beat, apra saber cual sera nuestro máximo

        switch (s)
        {
            case SCORE.COOL:
                {

                    //10000 : 1
                    puntuacion += Mathf.CeilToInt(puntuacion * percentageCool);
                }
                break;
            case SCORE.PERFECT:
                {
                    //10000 : 5
                    puntuacion += Mathf.CeilToInt(puntuacion * percentagePerfect);
                }
                break;
            case SCORE.HEAVY:
                {
                    //10000 : 10
                    puntuacion += Mathf.CeilToInt(puntuacion * percentageHeavy);
                }
                break;
            default:
                //Terrible
                //puntuacion ni crece
                break;
        }

        pointsText.text = $"WITCHES {puntuacion}";

        AddScore(s);
    }


    private void OnApplicationQuit()
    {
        BeatManager.onFixedBeat -= metronome;
        BeatManager.onTempoChanged -= tempoChanged;
    }

}
public enum SCORE
{
    NONE, //NA
    TERRIBLE, //<0-30%
    COOL, //<61-85%
    PERFECT, //<86-90%
    HEAVY //95-100%
}

//estados por los que pasa la cancion
public enum STATES
{
    CHILL,
    NORMAL,
    HEAVY
}
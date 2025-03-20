using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager _instance { get; private set; }

    //SCORE: Esto igual tiene mas sentido meterlo en otro lao, donde se gestionen los puntos
    private int queueSize = 10;
    private Queue<SCORE> scores;
    public int puntuacion = 1;
    public Animator beatMarkerContainerAnimator;
    public Animator [] speakers_animators;
    public Animator tree_animator;
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
    private int counter_beats = 0; private int counter_measures = -1;
    public TMP_Text pointsText;
    public GameObject freestyleText;

    public STATES actualState = STATES.NORMAL;
    bool cabraLoca = false;

    [SerializeField]
    Animator lightAnimator;
    [SerializeField]
    GameObject trees;

    [SerializeField]
    float[] milestonesPoints;
    private int idMilestone = 0;
    public bool gameStarted = false;

    [SerializeField]
    public int nHitsClickDown = 0, nHitsClickUp = 0;
    public int nHitsPerBeat = 1;

    [SerializeField]
    Animator HaloIntro;

    public bool tutorial;
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

        BeatManager.onTempoChanged += tempoChanged;
        tutorial = true;
    }
    void StartGame()
    {
        if (!gameStarted)
        {
            Debug.Log("empezando juego");
            HaloIntro.SetTrigger("end");
            BeatManager._instance.playTutorialSong();
            //BeatManager._instance.playSong();

            gameStarted = true;
            counter_measures++;
            beatMarkerContainerAnimator.speed = 1f;
            //beatMarkerContainerAnimator.SetTrigger("Start");
        }
    }

    public void StartSong()
    {
        tutorial = false;
        BeatManager.onFixedBeat += metronome;
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

        nHitsClickDown = 0; nHitsClickUp = 0;
        counter_beats = (counter_beats + 1) % 4; //hardcodeado a 4/4

        if (counter_beats == 0)
        {
            counter_measures += 1;
        }
        checkSongStates();

        //text.text = $"Compás {counter_measures}, pulso {counter_beats}";
        //Debug.Log($"Compás {counter_measures+1}, pulso {counter_beats+1}");

        //añade de manera aleatoria a gente al escenario de forma constante
        if (Random.Range(0, 1.01f) < constantPeopleProbability)
        {
            //  addPointsByFloat(percentageConstant);
        }

    }
    private void Update()
    {
        if (gameStarted)
        {
            if (beatMarkerContainerAnimator.GetCurrentAnimatorStateInfo(0).length <
                beatMarkerContainerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {

                double beatPortion = BeatManager.GetCurrentTime() - BeatManager.lastFixedBeatDSPTime;

                double percentage = beatPortion / BeatManager.beatInterval;
                //no esta playeando la animación
                //la forzamos a empezar de acuerdo a unas normas

                //Debug.Log("toca dar el beat con porcentage: " + percentage);
                tree_animator.Play("Tree Beat", -1, (float)percentage);
                beatMarkerContainerAnimator.Play("Beat Marker", -1, (float)percentage);
                foreach (Animator a in speakers_animators)
                {
                    a.Play("Speaker Beat 1", -1, (float)percentage);
                }

            }
            if (counter_measures == 1)
            {
                BeatManager._instance.playMetronome(false);

            }
        }
        else if (counter_measures < 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
        }
      
       
    }
    //checkpoints de la cancion donde cambia la intensidad de las cosas en pantalla
    //esto deberia hacerse con eventos de fmod PERO ME DA UNA PEREZA IMPRESIONANTE
    //EN PLAN
    //IMPRESIONANTE, SON LAS 2:17 NO ME PODRIA DAR MAS PEREZA EFE MOD
    private void checkSongStates()
    {
        //cambia de estado de la cancion en compases especificos
        switch (counter_measures + 1)
        {
            case 1:
                if (actualState == STATES.CHILL) return;
                actualState = STATES.CHILL;
                Debug.Log("cambio a estado chill, trankilitos");
                //quitarEpilepsia();
                break;
            case 26:
                freestyleText.SetActive(true);
                GetComponent<EventRandomSpawn>().freestyleMode = true;
                if (actualState == STATES.CHILL) return;
                actualState = STATES.CHILL;
                Debug.Log("cambio a estado chill, trankilitos");
                //quitarEpilepsia();
                break;
            case 34:
                freestyleText.SetActive(false);
                GetComponent<EventRandomSpawn>().freestyleMode = false;
                if (actualState == STATES.CHILL) return;
                actualState = STATES.CHILL;
                Debug.Log("cambio a estado chill, trankilitos");
                //quitarEpilepsia();
                break;
            case 94: //final

                if (actualState == STATES.CHILL) return;
                actualState = STATES.CHILL;
                Debug.Log("cambio a estado chill, trankilitos");
                _instance.gameStarted = false;
                GetComponent<EventRandomSpawn>().Stop();
                //quitarEpilepsia();
                break;
            case 10:
            case 30:
            case 38:
                if (actualState == STATES.NORMAL) return;
                actualState = STATES.NORMAL;
                Debug.Log("cambio a estado Normal, se pone intensillo");
                //quitarEpilepsia();
                break;
            case 54:
                freestyleText.SetActive(true);
                GetComponent<EventRandomSpawn>().freestyleMode = true;
                if (actualState == STATES.NORMAL) return;
                actualState = STATES.NORMAL;
                Debug.Log("cambio a estado Normal, se pone intensillo");
                //quitarEpilepsia();
                break;
            case 61: // 60
                freestyleText.SetActive(false);
                GetComponent<EventRandomSpawn>().freestyleMode = false;
                if (actualState == STATES.CHILL) return;
                actualState = STATES.CHILL;
                Debug.Log("cambio a estado chill, trankilitos");
                //quitarEpilepsia();
                break;

            case 45:
            case 85: //86

                if (actualState == STATES.HEAVY) return;
                mostrarCabra();
                actualState = STATES.HEAVY;
                epilepsia();
                Debug.Log("cambio a estado HEAVY, WOOOOOOOOOOO");
                break;
            default:
                break;

        }
        Debug.Log(counter_measures);
    }

    public void mostrarCabra()
    {
        if (cabraLoca) return;
        goatsController.SetTrigger("omg");
        thunder.Play();
        Debug.Log("LA CABRAAAAAAAAAAA");
        cabraLoca = true;
    }

    public void epilepsia()
    {
        lightAnimator.SetTrigger("HEAVY");
        Animator[] childAnimators = trees.GetComponentsInChildren<Animator>();

        foreach (Animator animator in childAnimators)
        {
            animator.enabled = true;
        }
    }
    public void quitarEpilepsia()
    {
        lightAnimator.SetTrigger("ANTIHEAVY");
        Animator[] childAnimators = trees.GetComponentsInChildren<Animator>();

        foreach (Animator animator in childAnimators)
        {
            animator.enabled = false;
        }
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

    public void addPoints(int points)
    {
        puntuacion += points;
        pointsText.text = $"PEOPLE: {puntuacion}";
    }
    public void addPointsByScore(SCORE s, float specialMultiplier)
    {

        foreach (SpeakerParticle speaker in speakers)
            speaker.playParticle(s);
        //nota, spawmear permite dar al final con todas las notas, ya sean aciertos o fallos y subir siempre, habria qu elimitar el que solo s epudea hacer uan vez cada beat, apra saber cual sera nuestro máximo

        switch (s)
        {
            case SCORE.COOL:
                {

                    //10000 : 1
                    puntuacion += Mathf.CeilToInt(puntuacion * specialMultiplier * percentageCool);
                }
                break;
            case SCORE.PERFECT:
                {
                    //10000 : 5
                    puntuacion += Mathf.CeilToInt(puntuacion * specialMultiplier * percentagePerfect);
                }
                break;
            case SCORE.HEAVY:
                {
                    //10000 : 10
                    puntuacion += Mathf.CeilToInt(puntuacion * specialMultiplier * percentageHeavy);
                }
                break;
            default:
                //Terrible
                //puntuacion ni crece
                break;
        }

        pointsText.text = $"WITCHES {puntuacion}";
        if (milestonesPoints[idMilestone] < puntuacion)
        {
            Camera.main.GetComponent<CameraMovement>().StartMoving(1f);
            idMilestone++;
        }
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


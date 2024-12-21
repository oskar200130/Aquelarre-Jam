using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager _instance { get; private set; }

    //SCORE: Esto igual tiene mas sentido meterlo en otro lao, donde se gestionen los puntos
    private int queueSize = 10;
    private Queue<SCORE> scores;
    public int puntuacion = 1;
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
    public float percentageCool = 0.0001f;
    public float percentagePerfect = 0.0005f;
    public float percentageHeavy = 0.001f;

    [SerializeField] SpeakerParticle[] speakers;
    private int counter_beats = 1; private int counter_measures = 0;
    public TMP_Text pointsText;
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
        BeatManager.onFixedBeat += metronome;
    }

    void metronome()
    {
        //text.text = $"Compás {counter_measures}, pulso {counter_beats}";
        //Debug.Log($"Compás {counter_measures}, pulso {counter_beats}");
        counter_beats = (counter_beats + 1) % 4; //hardcodeado a 4/4
        if (counter_beats == 0) counter_measures++;
    }

    private void Start()
    {
        //BeatManager._instance.playSong();
        pointsText.text = $"PEOPLE: {puntuacion}";
    }
    public void updatePoints(SCORE s)
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

        pointsText.text = $"PEOPLE: {puntuacion}";

        AddScore(s);
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

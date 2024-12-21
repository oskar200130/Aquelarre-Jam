using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public static BeatManager _instance { get; private set; }

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

    public int current_beat = 0; public int current_measure = 0;

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
        FMODBeatTracker.onFixedBeat += countBeats;
    }
    bool started = false;
    void countBeats()
    {
        if (!started)
        {
            Debug.Log($"Beat Interval: {FMODBeatTracker.GetBeatInterval()};;; Lenght ms: {""}");
            Debug.Log("------------------------------------------------------------------------------------------");
            started = true;
            //i wanna vomit myself
        }
        current_beat = (current_beat + 1) % 4;
        if (current_beat == 0) current_measure++;
        Debug.Log($"Compas {current_measure+1}, pulso {current_beat +1}");
        Debug.Log($"DSP Delta Time: {FMODBeatTracker.GetDSPDeltaTime()} ;  Current Time {FMODBeatTracker.GetCurrentTime()}   ;");
        Debug.Log($"DSP Delta Time: {FMODBeatTracker.GetLastFixedBeatDSPTime()} ;  Current Time Milliseconds {FMODBeatTracker.GetCurrentTimeInMilliseconds()}");
        Debug.Log("------------------------------------------------------------------------------------------");
    }

}
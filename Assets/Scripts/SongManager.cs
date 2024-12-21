using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    public static SongManager _instance { get; private set; }

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
    public int nBeatsToStart = 2; private int current_beat_before_song = 0;
    private void Awake()
    {
        if (_instance == null)
        {
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
    public bool started = false;
    //hardcodeado a 90 porque la cancion en verdad es a 180
    void countBeats()
    {
        if (!started)
        {
            current_beat_before_song++;
            if (current_beat_before_song > nBeatsToStart)
                started = true;
        }
        else
        {
            //Debug.Log($"Measure {current_measure}, Beat {current_beat}");
            current_beat = (current_beat + 1) % 4;
            if (current_beat == 0) current_measure++;

        }

    }

}
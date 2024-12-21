using UnityEngine;
using UnityEngine.Events;

public class SpecialEvent : MonoBehaviour
{
    [SerializeField]
    int waitForBeats;

    private UnityAction nextBeat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextBeat = NextBeat;
        BeatManager._instance?.OnPulse.AddListener(nextBeat);
    }

    void NextBeat()
    {
        waitForBeats--;
        if (waitForBeats <= 0)
        {
            //TODO: multiplicador para el sistema de puntos

            BeatManager._instance?.OnPulse.RemoveListener(nextBeat);
        }
    }
}

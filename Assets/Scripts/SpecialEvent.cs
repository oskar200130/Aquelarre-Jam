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
        FMODBeatTracker.onFixedBeat += NextBeat;
    }

    void NextBeat()
    {
        waitForBeats--;
        if (waitForBeats <= 0)
        {
            //TODO: multiplicador para el sistema de puntos

            FMODBeatTracker.onFixedBeat -= NextBeat;
        }
    }
}

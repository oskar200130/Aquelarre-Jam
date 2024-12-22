using Unity.Entities;
using UnityEngine;

public class EventRandomSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject[] eventEffect;
    [SerializeField]
    int minBeatsBetweenEvents;
    [SerializeField]
    float probabilitySpawnEffect;

    private int lastSpawn = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BeatManager.onFixedBeat += CreateEvent;
    }

    private void OnApplicationQuit()
    {
        BeatManager.onFixedBeat -= CreateEvent;

    }

    void CreateEvent()
    {
        if (lastSpawn <= 0)
        {
            if (Random.Range(0f, 100f) >= probabilitySpawnEffect) return;

            SpawnEffect((Vector3)World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RandomEntitySystem>().GetRandomEntityPos());
        }
        else
            lastSpawn--;
    }

    public void CreateEventNoRand()
    {
        SpawnEffect((Vector3)World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RandomEntitySystem>().GetRandomEntityPos(), false);
    }
    private void SpawnEffect(Vector3 spawnPos, bool rand = true)
    {
        int id = 0;
        if (rand) id = Random.Range(0, eventEffect.Length);
        
        GameObject instance = Instantiate(eventEffect[id], spawnPos, Quaternion.identity);
        ClickDetector.instance.specialEvents.Add(instance.GetComponentInChildren<SpecialEvent>());
        lastSpawn = minBeatsBetweenEvents;
    }
}

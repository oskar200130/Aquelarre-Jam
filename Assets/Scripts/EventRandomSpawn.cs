using Unity.Entities;
using UnityEngine;

public class EventRandomSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject eventEffect;
    [SerializeField]
    int minBeatsBetweenEvents;
    [SerializeField]
    float probabilitySpawnEffect;

    private int lastSpawn = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FMODBeatTracker.onFixedBeat += CreateEvent;
    }

    void CreateEvent()
    {
        if (lastSpawn <= 0){
            if (Random.Range(0f, 100f) >= probabilitySpawnEffect) return;           

            SpawnEffect((Vector3)World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RandomEntitySystem>().GetRandomEntityPos() + new Vector3(0, 2, 0));
        }
        else
            lastSpawn--;
    }
    private void SpawnEffect(Vector3 spawnPos)
    {
        Instantiate(eventEffect, spawnPos, Quaternion.identity);
        lastSpawn = minBeatsBetweenEvents;
    }
}

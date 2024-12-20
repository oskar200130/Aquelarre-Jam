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
    private BeatManager beatManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        beatManager = BeatManager._instance;
        beatManager?.OnPulse.AddListener(CreateEvent);
    }

    void CreateEvent()
    {
        if (lastSpawn <= 0){
            if (Random.Range(0f, 100f) >= probabilitySpawnEffect) return;


            SpawnEffect(Vector3.zero);
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

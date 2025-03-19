using TMPro;
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
    public int waitBeats = 0;
    public bool freestyleMode = false;
    private  bool tutorialMode = true;
    private int tutorialFase = 0;

    private bool checkClick = false, checkSpace = false;
    private float timeToNextFase = -1;

    [SerializeField]
    GameObject tutorialText;
    [SerializeField]
    Vector3 eventSpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BeatManager.onFixedBeat += CreateEvent;
    }

    private void OnApplicationQuit()
    {
        BeatManager.onFixedBeat -= CreateEvent;

    }

    private void FixedUpdate()
    {
        if((checkClick && Input.GetMouseButtonDown(0)) || (checkSpace && Input.GetKeyDown(KeyCode.Space)))
        {
            Debug.Log("CLICK TO PASS");
            tutorialFase++;
            checkClick = checkSpace = false;
            timeToNextFase = -1;
        }

        if(timeToNextFase > 0)
            timeToNextFase -= Time.deltaTime;
        else if(timeToNextFase > -1)
        {
            Debug.Log("TIME TO PASS");
            checkClick = false;
            timeToNextFase = -1f;
            tutorialFase++;
        }
    }

    void CreateEvent()
    {
        if (freestyleMode) return;

        if (tutorialMode)
        {
            if (waitBeats > 0)
                waitBeats--;
            switch (tutorialFase)
            {
                case 0:
                    tutorialText.SetActive(true);
                    checkClick = true;
                    if(timeToNextFase == -1) timeToNextFase = 6f;
                    return;
                case 1:
                    //tutorialText.SetActive(false);
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "Click over the red circle on the beat";
                    tutorialFase++;
                    Debug.Log("NOW TUTO STARTS");
                    return;
                case 2:
                    checkSpace = true;
                    if(waitBeats == 0)
                    {
                        SpawnEffect(eventSpawn, 0, false, 0);
                        waitBeats = 2;
                    }                    
                    return;
                case 3:
                    BeatManager.currentMusicTrack.setParameterByName("TutorialSteps", 1);
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "";
                    if (timeToNextFase == -1)
                    {
                        timeToNextFase = 2.5f;
                        LevelManager._instance.puntuacion += 100;
                    }
                    return;
                case 4:
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "Click over the blue circle.\n Keep it pressed to load a pogo and then lift";
                    tutorialFase++;
                    Debug.Log("NOW SECOND FASE STARTS");
                    return;
                case 5:
                    checkSpace = true;
                    if (waitBeats == 0)
                    {
                        SpawnEffect(eventSpawn, 0, false, 1);
                        waitBeats = 4;
                    }
                    return;
                case 6:
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "";
                    if (timeToNextFase == -1) 
                    { 
                        LevelManager._instance.puntuacion += 50;
                        timeToNextFase = 2.5f; 
                    }
                    return;
                case 7:
                    LevelManager._instance.freestyleText.SetActive(true);
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "When the freestyle sign appears, you should do actions over the witches keeping the rithm to call more witches.";
                    checkSpace = true;
                    return;
                case 8:
                    LevelManager._instance.puntuacion += 50;
                    BeatManager.currentMusicTrack.setParameterByName("TutorialSteps", 2);
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "";
                    LevelManager._instance.freestyleText.SetActive(false);
                    if (timeToNextFase == -1)
                        timeToNextFase = .5f;
                    return;
                case 9:
                    BeatManager._instance.playSong();
                    break;
            }

            tutorialMode = false;
        }
        //Debug.Log("HOLO");
        if (lastSpawn <= 0)
        {
            if (Random.Range(0f, 100f) >= probabilitySpawnEffect) return;

            SpawnEffect((Vector3)World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RandomEntitySystem>().GetRandomEntityPos());
        }
        else
        {
            if(waitBeats <= 0)
                lastSpawn--;
            else
                waitBeats--;
        }
    }

    public void CreateEventNoRand(int spawns)
    {
        SpawnEffect((Vector3)World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RandomEntitySystem>().GetRandomEntityPos(), spawns, false);
    }
    private void SpawnEffect(Vector3 spawnPos, int spawns = 0, bool rand = true, int eventToSpawn = 0)
    {
        int id = 0;
        if (rand) id = Random.Range(0, eventEffect.Length);
        else id = eventToSpawn;

            Vector3 camPos = Camera.main.transform.position;
        Vector3 dir = (camPos - spawnPos).normalized;
        Vector3 posSpawn = spawnPos + dir * 3.5f;
        posSpawn.y = spawnPos.y;

        GameObject instance = Instantiate(eventEffect[id], posSpawn, Quaternion.identity);
        //GameObject instance = Instantiate(eventEffect[id], spawnPos, Quaternion.identity);
        if (!rand)
            instance.GetComponentInChildren<SpecialEvent>().maxDragSpawns = spawns;
        ClickDetector.instance.specialEvents.Add(instance.GetComponentInChildren<SpecialEvent>());
        lastSpawn = minBeatsBetweenEvents;
    }
}

using FMODUnity;
using TMPro;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

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
    private bool tutorialMode = true;
    private int tutorialFase = 0;

    private bool checkClick = false, checkSpace = false;
    private float timeToNextFase = -1;

    [SerializeField]
    GameObject tutorialText;
    [SerializeField]
    Vector3 eventSpawn;

    private int eventsClicked = 0;
    private bool sound = false, sound2 = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BeatManager.onFixedBeat += CreateEvent;
    }

    private void OnApplicationQuit()
    {
        Stop();
    }

    public void Stop()
    {
        BeatManager.onFixedBeat -= CreateEvent;
    }

    public void AddEventClicked()
    {
        eventsClicked++;
    }
    public void FailedClicked()
    {
        eventsClicked = 0;
    }

    private void Update()
    {
        if ((checkClick && Input.GetMouseButtonDown(0)) || (checkSpace && Input.GetKeyDown(KeyCode.Space)))
        {
            Debug.Log("CLICK TO PASS");
            tutorialFase++;
            checkClick = checkSpace = false;
            timeToNextFase = -1;
        }
    }

    private void FixedUpdate()
    {      
        if (timeToNextFase > 0)
            timeToNextFase -= Time.deltaTime;
        else if (timeToNextFase > -1)
        {
            Debug.Log("TIME TO PASS");
            checkClick = false;
            timeToNextFase = -1f;
            tutorialFase++;
            if (tutorialFase > 8)
            {
                BeatManager._instance.playSong();
                LevelManager._instance.StartSong();
                tutorialMode = false;
            }
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
                    if (timeToNextFase == -1) timeToNextFase = 6f;
                    return;
                case 1:
                    //tutorialText.SetActive(false);
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "Click over the red circle on the beat";
                    tutorialFase++;
                    Debug.Log("NOW TUTO STARTS");
                    return;
                case 2:
                    //checkSpace = true;
                    if (waitBeats == 0)
                    {
                        SpawnEffect(eventSpawn, 0, false, 0);
                        sound = false;
                        waitBeats = 2;
                    }

                    if (!sound && eventsClicked == 1)
                    {
                        RuntimeManager.PlayOneShot("event:/Tutorial/Click 1");
                        sound = true;
                    }
                    else if (!sound && eventsClicked == 2)
                    {
                        RuntimeManager.PlayOneShot("event:/Tutorial/Click 2");
                        sound = true;
                    }
                    else if (!sound && eventsClicked == 3)
                    {
                        RuntimeManager.PlayOneShot("event:/Tutorial/Click 3");
                        sound = true;
                        tutorialFase++;
                    }
                    return;
                case 3:
                    BeatManager.currentMusicTrack.setParameterByName("TutorialSteps", 1);
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "";
                    if (timeToNextFase == -1)
                    {
                        timeToNextFase = 2.5f;
                        LevelManager._instance.addPoints(100);
                    }
                    return;
                case 4:
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "Click over the blue circle.\n Keep it pressed to load a pogo and then lift";
                    LevelManager._instance.GetComponent<EventRandomSpawn>().FailedClicked();
                    tutorialFase++;
                    Debug.Log("NOW SECOND FASE STARTS");
                    return;
                case 5:
                    //checkSpace = true;
                    if (waitBeats == 0)
                    {
                        SpawnEffect(eventSpawn, 0, false, 1);
                        waitBeats = 4;
                        sound = false;
                    }
                    if (!sound && eventsClicked == 1)
                    {
                        RuntimeManager.PlayOneShot("event:/Tutorial/ClickLoop 1");
                        Debug.Log("AAAAAAAAAAAAA");
                        sound = true;
                    }
                    else if (!sound && eventsClicked == 2)
                    {
                        RuntimeManager.PlayOneShot("event:/Tutorial/ClickLoop 2");
                        Debug.Log("BBBBBBBBBBBBBBBB");
                        sound = true;
                    }
                    else if (!sound && eventsClicked == 3)
                    {
                        RuntimeManager.PlayOneShot("event:/Tutorial/ClickLoop 3");
                        Debug.Log("CCCCCCCCCCCCCCC");
                        sound = true;
                        tutorialFase++;
                    }
                    return;
                case 6:
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "";
                    if (timeToNextFase == -1)
                    {
                        timeToNextFase = 2.5f;
                        LevelManager._instance.addPoints(50);
                    }
                    return;
                case 7:
                    LevelManager._instance.freestyleText.SetActive(true);
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "When the freestyle sign appears, you should do actions over the witches keeping the rithm to call more witches.";
                    //checkSpace = true;
                    if (timeToNextFase == -1)
                    {
                        timeToNextFase = 10f;
                    }
                    return;
                case 8:
                    BeatManager.currentMusicTrack.setParameterByName("TutorialSteps", 2);
                    tutorialText.GetComponent<TextMeshProUGUI>().text = "";
                    LevelManager._instance.freestyleText.SetActive(false);
                    if (timeToNextFase == -1)
                    {
                        LevelManager._instance.addPoints(50);
                        timeToNextFase = 6.5f;
                    }
                    return;
            }
        }
        //Debug.Log("HOLO");
        if (lastSpawn <= 0)
        {
            if (Random.Range(0f, 100f) >= probabilitySpawnEffect) return;

            SpawnEffect((Vector3)World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RandomEntitySystem>().GetRandomEntityPos());
        }
        else
        {
            if (waitBeats <= 0)
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

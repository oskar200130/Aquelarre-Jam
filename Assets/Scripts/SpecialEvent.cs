using UnityEngine;
using UnityEngine.Events;

public class SpecialEvent : MonoBehaviour
{
    [SerializeField]
    float radiusClick;

    //private UnityAction nextBeat;
    private Animator animator;

    public float multiplier;
    public bool drag;

    public int maxDragSpawns;
    public int beatsToWaitInHold = 4;

    private bool failedClick;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        failedClick = true;
        animator = GetComponent<Animator>();
    }

    public bool CheckClick()
    {
        if ((ClickDetector.instance.specialDetectorHitPoint - new Vector3(transform.position.x, 0, transform.position.z)).magnitude < radiusClick)
        {
            animator.SetTrigger("Clicked");
            failedClick = false;
            if(drag)
            {
                InstanciateDrag();
            }
            return true;
        }
        return false;
    }

    public void SetNotFailedClick()
    {
        failedClick = false;
    }
    public void DestroyMyself()
    {
        if (failedClick)
            LevelManager._instance.GetComponent<EventRandomSpawn>().FailedClicked();
        ClickDetector.instance.specialEvents.Remove(this);
        Destroy(transform.parent.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, 0, transform.position.z), radiusClick);
    }

    public void InstanciateDrag()
    {
        if(maxDragSpawns > 0)
            LevelManager._instance.gameObject.GetComponent<EventRandomSpawn>().CreateEventNoRand(maxDragSpawns-1);
    }

    //Llamado por animacion
    public void AddBeatsForHold()
    {
        LevelManager._instance.gameObject.GetComponent<EventRandomSpawn>().waitBeats = beatsToWaitInHold;
    }
}

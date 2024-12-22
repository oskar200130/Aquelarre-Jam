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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public bool CheckClick()
    {
        if ((ClickDetector.instance.specialDetectorHitPoint - new Vector3(transform.position.x, 0, transform.position.z)).magnitude < radiusClick)
        {
            animator.SetTrigger("Clicked");
            if(drag)
            {
                InstanciateDrag();
            }
            return true;
        }
        return false;
    }

    public void DestroyMyself()
    {
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
        LevelManager._instance.gameObject.GetComponent<EventRandomSpawn>().CreateEventNoRand();
    }
}

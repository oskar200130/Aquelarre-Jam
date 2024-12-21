using UnityEngine;
using UnityEngine.Events;

public class SpecialEvent : MonoBehaviour
{
    public enum SpecialEventType { JUMP, HOLD, POGO}    //Si se añaden nuevos eventos, hacerlo antes de pogo o modificar el valor que inicializa type de forma random
    [SerializeField]
    int waitForBeats;

    [SerializeField]
    float radiusClick;

    public Material mat;    //Para debug

    private UnityAction nextBeat;
    private Animator animator;

    private SpecialEventType type;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextBeat = NextBeat;
        BeatManager.onFixedBeat += NextBeat;
        animator = GetComponent<Animator>();
        type = (SpecialEventType)Random.Range(0, (int)SpecialEventType.POGO +1);   
    }

    void NextBeat()
    {
        waitForBeats--;
        //animator.SetTrigger("NextAnim");
        if (waitForBeats == 2) GetComponent<MeshRenderer>().material = mat;
        else if (waitForBeats == 0)
        {
            if ((ClickDetector.instance.worldMousePosWhenDown - new Vector3(transform.position.x, 0, transform.position.z)).magnitude < radiusClick)
                Debug.Log("MIHOME " + type);
                //TODO: multiplicador para el sistema de puntos y tipo de evento
        }
        else if (waitForBeats < 0)
        {
            BeatManager.onFixedBeat -= NextBeat;
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, 0, transform.position.z), radiusClick);
    }
}

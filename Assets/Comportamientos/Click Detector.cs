using NUnit.Framework;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour
{
    public static ClickDetector instance;

    [Tooltip("Arbitary text message")]
    //Unidades de distacia, actualemten se comprueban unicamente con la pantalla (yo lo dejaria asi )
    public float pogoMargin = 25.0f;
    public float timeMarginForActions = 0.6f;
    public bool canNotCancelPogo = false;

    public bool salto, pogo, arrastre, pogoEnd, rePogo;

    public float TimeEndedPogo = -1.0f; //si puede haber varios pogos, pues haremos un array que los checke y lance el nuevo evento

    public bool up, down;

    public Vector3 worldMousePosWhenDown;
    public Vector3 worldMousePosPOGOCOMENCE;
    public Vector2 screenMousePosWhenDown;
    public Vector3 specialDetectorHitPoint;

    //para hacer solo casting a este y no mil veces a la llamada de Input. que es mas costoso que lañ haga cada objeto.
    public Vector3 worldMousePos;
    public Vector2 screenMousePos;

    [SerializeField] Collider planeSpecialDetector;

    float timeClickedDown = 0.0f;
    SCORE clickDownLastScore = SCORE.NONE;
    SCORE clickUpLastScore = SCORE.NONE;

    public List<SpecialEvent> specialEvents;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            specialEvents = new List<SpecialEvent>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Update once per frame
    void Update()
    {
        float t;

        if (TimeEndedPogo != 0.0f)
        {
            if (TimeEndedPogo + 2.5f <= BeatManager.GetCurrentTime() )
            {
                rePogo = true;
                TimeEndedPogo = 0.0f;
            }
        }
        else
        {
            rePogo = false;
        }

        if (timeClickedDown == 0.0f)
        {
            down = Input.GetMouseButtonDown(0);
            if (down)
            {
                pogoEnd = false;
                screenMousePosWhenDown = Input.mousePosition;
                timeClickedDown = BeatManager.GetCurrentTime();

                //se puede restringir el objeto con el que colisiona, actualemte lo hago asi por pereza //TODO
                Ray ray2 = Camera.main.ScreenPointToRay(screenMousePosWhenDown);

                // Calculate the intersection with the (x, 0, z) plane
                t = -ray2.origin.y / ray2.direction.y;
                worldMousePosWhenDown = ray2.origin + ray2.direction * t;

                //Posicion para detectar los eventos especiales
                RaycastHit hit;                
                if(planeSpecialDetector.Raycast(ray2, out hit, 1000000000f))
                    specialDetectorHitPoint = hit.point;

                float multiplier = 1;       //Puntos para todos los eventos al click
                for (int i = 0; i < specialEvents.Count; i++)
                {
                    if (specialEvents[i].CheckClick())
                    {
                        multiplier = specialEvents[i].multiplier;
                        break;
                    }
                }
                clickUpLastScore = BeatManager._instance.evaluateClick(multiplier);
                if (multiplier != 1) Debug.Log("Multiplicador click: " + multiplier);
            }
        }

        up = Input.GetMouseButtonUp(0);
        if (up)
        {
            timeClickedDown = 0.0f;
        }

        //actualizar la posicióna actual del raton, World y screen
        screenMousePos = Input.mousePosition;

        //se puede restringir el objeto con el que colisiona, actualemte lo hago asi por pereza //TODO
        Ray ray = Camera.main.ScreenPointToRay(screenMousePos);

        // Calculate the intersection with the (x, 0, z) plane
        t = -ray.origin.y / ray.direction.y;
        worldMousePos = ray.origin + ray.direction * t;

        //hayq ue pòner un limite de timepo, lo ahgop al final porque me amreo //TODO
        if (!(pogo || arrastre) && up && down)  //BEAT
        {
            salto = true;
            // Debug.Log("Salto");            
        }
        else
        {
            salto = false;
        }

        //primero el arrastre, imaginemos que el jugador mueve rapido el raton, entoces el timepo de acciones, que sirve apra diferenciar entre click o Pogo no importa. sabemos que e sun arrastre.
        if (down && !up)
        {
            if (!arrastre && Vector2.Distance(screenMousePos, screenMousePosWhenDown) < pogoMargin) //not arrastre, para evitar de que entren a un ciclo de entrar y salir de la zona
            {
                if (timeClickedDown + timeMarginForActions < BeatManager.GetCurrentTime())
                {
                    if (!pogo)
                        Debug.Log("Pogo");
                    pogo = true;
                    worldMousePosPOGOCOMENCE = worldMousePosWhenDown;
                }

            }
            else if (!canNotCancelPogo || !pogo) //podemos hacer esto, para que una vez comenzado un pogo no se pueda comenzar el arrastre, lo comento para tenerlo en cuenta, de hecho loo voy a poner como eleccion en 
            {

                //TODO a lo mejor no queremos que se cancelle un pogo si movemos el raton, si no que una vez empezado nos da igual, seguramente sea mas satisfacctorio.
                if (pogo)
                {
                    Debug.Log("Pogo cancelled");
                    pogoEnd = true;
                }
                pogo = false;
                if (!arrastre)
                    Debug.Log("Arrastre");
                arrastre = true;
            }
        }
        else
        {
            if (pogo)       //POGO CIERRA
            {
                Debug.Log("Pogo ended");
                pogoEnd = true;
                float multiplier = 1;
                for (int i = 0; i < specialEvents.Count; i++)
                {
                    if (specialEvents[i].CheckClick())
                    {
                        multiplier = specialEvents[i].multiplier;
                        break;
                    }
                }
                clickUpLastScore = BeatManager._instance.evaluateClick(multiplier);
                if(multiplier!=1) Debug.Log("Multiplicador finPogo: " + multiplier);

                TimeEndedPogo = BeatManager.GetCurrentTime();
            }
            if (arrastre)       //ARRASTE FIN
            {
                Debug.Log("Arrastre ended");
                //ela rrastre al momento de detectar el click desde que lo suelta
                //float multiplier = 1;
                //for (int i = 0; i < specialEvents.Count; i++)
                //{
                //    if (specialEvents[i].CheckClick())
                //    {
                //        multiplier = specialEvents[i].multiplier;
                //        break;
                //    }
                //}
                //clickUpLastScore = BeatManager._instance.evaluateClick(multiplier);
            }
            pogo = false;
            arrastre = false;
        }
    }
}

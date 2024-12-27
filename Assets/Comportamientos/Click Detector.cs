using System.Collections.Generic;
using UnityEngine;

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
    private float dragMulti;

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
    double clickTime;
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
                clickTime = BeatManager.GetCurrentTime();
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
                        if (specialEvents[i].drag)
                            dragMulti += specialEvents[i].multiplier;
                        else
                            multiplier = specialEvents[i].multiplier;
                        break;
                    }
                }

                if (LevelManager._instance.nHitsClickDown < LevelManager._instance.nHitsPerBeat)
                {
                    clickUpLastScore = BeatManager._instance.evaluateClick(clickTime, multiplier);
                    LevelManager._instance.nHitsClickDown++;
                }
            }
        }

        up = Input.GetMouseButtonUp(0);
        if (up)
        {
            timeClickedDown = 0.0f;
            clickTime = BeatManager.GetCurrentTime();
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
                    pogo = true;
                    worldMousePosPOGOCOMENCE = worldMousePosWhenDown;
                }

            }
            else if (arrastre)
            {
                //Posicion para detectar los eventos especiales
                RaycastHit hit;
                if (planeSpecialDetector.Raycast(ray, out hit, 1000000000f))
                    specialDetectorHitPoint = hit.point;
                for (int i = 0; i < specialEvents.Count; i++)
                {
                    if (specialEvents[i].CheckClick())
                    {
                        dragMulti += specialEvents[i].multiplier;
                        specialEvents[i].DestroyMyself();
                        break;
                    }
                }
            }
            else if (!canNotCancelPogo || !pogo) //podemos hacer esto, para que una vez comenzado un pogo no se pueda comenzar el arrastre, lo comento para tenerlo en cuenta, de hecho loo voy a poner como eleccion en 
            {

                //TODO a lo mejor no queremos que se cancelle un pogo si movemos el raton, si no que una vez empezado nos da igual, seguramente sea mas satisfacctorio.
                if (pogo)
                {
                    pogoEnd = true;
                }
                pogo = false;
                arrastre = true;
            }
        }
        else
        {
            if (pogo)       //POGO CIERRA
            {
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
                if (LevelManager._instance.nHitsClickUp < LevelManager._instance.nHitsPerBeat)
                {
                    clickUpLastScore = BeatManager._instance.evaluateClick(clickTime, multiplier);
                    LevelManager._instance.nHitsClickUp++;
                }

                TimeEndedPogo = BeatManager.GetCurrentTime();
            }
            if (arrastre)       //ARRASTE FIN
            {
                if (LevelManager._instance.nHitsClickUp < LevelManager._instance.nHitsPerBeat)
                {
                    clickUpLastScore = BeatManager._instance.evaluateClick(clickTime, dragMulti);
                    LevelManager._instance.nHitsClickUp++;
                }
                dragMulti = 1;
            }
            pogo = false;
            arrastre = false;
        }
    }
}

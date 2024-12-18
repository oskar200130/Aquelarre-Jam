using UnityEngine;

public class ClickDetector : MonoBehaviour
{
   
    [Tooltip("Arbitary text message")]
    //Unidades de distacia, actualemten se comprueban unicamente con la pantalla (yo lo dejaria asi )
    public float pogoMargin = 25.0f; 
    public float timeMarginForActions = 0.6f;
    public bool canNotCancelPogo = false;



    public bool salto, pogo, arrastre, pogoEnd;



    public bool up, down;

    public Vector3 worldMousePosWhenDown;
    public Vector2 screenMousePosWhenDown;

    //para hacer solo casting a este y no mil veces a la llamada de Input. que es mas costoso que lañ haga cada objeto.
    public Vector3 worldMousePos;
    public Vector2 screenMousePos;

    float timeClickedDown = 0.0f;

    // Update once per frame
    void Update()
    {

        float t;

        if (timeClickedDown == 0.0f)
        {
            down = Input.GetMouseButtonDown(0);

            if (down)
            {
                pogoEnd = false;
                screenMousePosWhenDown = Input.mousePosition;
                timeClickedDown = Time.time;

                //se puede restringir el objeto con el que colisiona, actualemte lo hago asi por pereza //TODO
                Ray ray2 = Camera.main.ScreenPointToRay(screenMousePosWhenDown);

                // Calculate the intersection with the (x, 0, z) plane
                t = -ray2.origin.y / ray2.direction.y;

                worldMousePosWhenDown = ray2.origin + ray2.direction * t;

                //en caso de que se quiera hacer con la intersecciñoin de un plano y no ocon el plano (x,y = 0, z)
                //if (Physics.Raycast(ray2, out RaycastHit hitData2))
                //{
                //    worldMousePosWhenDown = hitData2.point;
                //}

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

        //por si se quiere hacer con collision de algun plano
        //if (Physics.Raycast(ray, out RaycastHit hitData))
        //{
        //    worldMousePos = hitData.point;
        //}


        //hayq ue pòner un limite de timepo, lo ahgop al final porque me amreo //TODO
        if (!(pogo || arrastre) && up && down)
        {
            salto = true;
            Debug.Log("Salto");
        }
        else
        {
            salto = false;
        }


        //primero el arrastre, imaginemos que el jugador mueve rapido el raton, entoces el timepo de acciones, que sirve apra diferenciar entre click o Pogo no importa. sabemos que e sun arrastre.




        
            if (down && !up)
            {
                if (!arrastre && Vector2.Distance(screenMousePos, screenMousePosWhenDown) < pogoMargin ) //not arrastre, para evitar de que entren a un ciclo de entrar y salir de la zona
                {
                    if (timeClickedDown + timeMarginForActions < Time.time)
                    {
                        if (!pogo)
                        Debug.Log("Pogo");
                        pogo = true;
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

                if (pogo)
                {
                    Debug.Log("Pogo ended");
                    pogoEnd = true;
                }
                if (arrastre)
                    Debug.Log("Arrastre ended");
                pogo = false;
                arrastre = false;
            }

        
    }
}

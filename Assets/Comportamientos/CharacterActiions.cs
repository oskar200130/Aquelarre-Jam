using UnityEngine;

public class CharacterActiions : MonoBehaviour
{


    public ClickDetector detector;
    public float distanceMarginActions = 10.0f;

    private float currentSpeed = 0.0f;

    private Vector3 crowdPoint;

    private enum charStates { IDLE, JUMP, POGO, POGOEXIT, POGOEND, ARRASTE }
    private charStates estado = charStates.IDLE;

    public float jumpForce = 20 * 2;
    public float gravity = -9.81f * 2;
    
    public float velocity;
    public float distanceToMouseDown;
    public float pogoForce;
    private void Start()
    {
        crowdPoint = transform.position; //cuandos e generen de verdad tal vez no se encuentren en este punto de referenceia,
                                         //el cual va a ser su sitoi entre todo el publico y al que van a intentra ir cada uno de los agentes.
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (estado == charStates.IDLE || estado == charStates.POGOEXIT)
        {

            if (detector.salto)
            {
                distanceToMouseDown = Vector2.Distance(detector.screenMousePosWhenDown, screenPos);
                if (distanceToMouseDown < distanceMarginActions)
                {
                    estado = charStates.JUMP;
                    //fuerzad de salto proporcional a la distancia del click
                    velocity = jumpForce * ((distanceMarginActions - distanceToMouseDown) / distanceMarginActions);
                }
            }
            else if (detector.pogo)
            {
                distanceToMouseDown = Vector2.Distance(detector.screenMousePos, screenPos);
                if (distanceToMouseDown < distanceMarginActions)
                {
                    //startPosition = transform.position;
                    //hacemos el pogo, añadiendo distancia segun el tiempo que lleve pulsado.

                    estado = charStates.POGO;

                }
            }
            else if (detector.arrastre)
            {
                if (Vector2.Distance(detector.screenMousePos, screenPos) < distanceMarginActions)
                {
                    //startPosition = transform.position;
                    //hacemos el arrastre que este tocando
                    estado = charStates.POGO;

                }
            }
        }
        if (estado == charStates.POGO)
        {
            if (detector.pogoEnd)
            {
                estado = charStates.POGOEND;
            }
        }
    }

    private void FixedUpdate()
    {
        switch (estado)
        {
            case charStates.JUMP:
                {
                    velocity += gravity * (Time.deltaTime*2);
                    transform.Translate(new Vector3(0, 0, velocity) * Time.deltaTime);
                    if (velocity <= 0)
                    {
                        //chekear que ha llegado al suelo //me da pereza
                        if (crowdPoint.y >= transform.position.y)
                        {
                            velocity = 0.0f;

                            estado = charStates.IDLE;

                            transform.position = crowdPoint;
                        }
                    }
                }
                break;
            case charStates.POGO:

                {
                    //si en vez de screenPosWhen Down s eahce con solo POS, el Pogo seguira donde sea que se este formando, puede ser mñas natural
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                    distanceToMouseDown = Vector2.Distance(detector.screenMousePos, screenPos);

                    //fuerza de salto proporcional a la distancia del click
                    pogoForce = Mathf.Abs((distanceMarginActions - distanceToMouseDown) / distanceMarginActions);
                    velocity = jumpForce * pogoForce;

                    Vector3 direccion = (crowdPoint - detector.worldMousePos).normalized * pogoForce;

                    // The step size is equal to speed times frame time.
                    float step = velocity * Time.deltaTime;

                   


                    if (distanceToMouseDown >= distanceMarginActions)
                    {
                        estado = charStates.POGOEXIT;
                    }
                    else
                    {
                        // Move our position a step closer to the target.
                        transform.position = Vector3.MoveTowards(transform.position, transform.position + direccion, step);
                    }
                }
                break;

            case charStates.POGOEXIT:

                {
                    velocity = 20.0f;
                    transform.position = Vector3.MoveTowards(transform.position, crowdPoint, velocity * Time.deltaTime);

                    if (Vector3.Distance(transform.position, crowdPoint) <= 0.1f)
                    {
                        transform.position = crowdPoint;
                        estado = charStates.IDLE;
                    }
                }
                break;

            case charStates.POGOEND:
                {
                    //no se como hacerle para que quede guay ajajajaja (joder vamos espesos a estas horas 0:16)
                    velocity = Random.Range(20.0f,100.0f);
                    transform.position = Vector3.MoveTowards(transform.position, crowdPoint, velocity * Time.deltaTime);

                    if (Vector3.Distance(transform.position, crowdPoint) <= 0.1f)
                    {
                        transform.position = crowdPoint;
                        estado = charStates.IDLE;
                    }
                }
                break;
            case charStates.ARRASTE:
                break;
            default:
                //animacion de Idle, puede ser un up & down o una animación de sprite
                {

                }
                break;
        }


    }
}



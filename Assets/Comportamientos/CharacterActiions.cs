using UnityEngine;

public class CharacterActiions : MonoBehaviour
{


    public ClickDetector detector;
    public float distanceMarginActions = 10.0f;

    public float speed = 5f; // Velocidad de movimiento
    private float currentSpeed = 0.0f;

    private Vector3 crowdPoint;

    private enum charStates { IDLE, JUMP, POGO, ARRASTE }
    private charStates estado = charStates.IDLE;

    public float jumpForce = 20 * 2;
    public float gravity = -9.81f * 2;
    
    public float velocity;
    public float distanceToMouseDown;

    private void Start()
    {
        crowdPoint = transform.position; //cuandos e generen de verdad tal vez no se encuentren en este punto de referenceia,
                                         //el cual va a ser su sitoi entre todo el publico y al que van a intentra ir cada uno de los agentes.
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (estado == charStates.IDLE)
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
                if (Vector2.Distance(detector.screenMousePos, screenPos) < distanceMarginActions)
                {
                    //startPosition = transform.position;
                    //hacemos el pogo, añadiendo distancia segun el tiempo que lleve pulsado.
                }
            }
            else if (detector.arrastre)
            {
                if (Vector2.Distance(detector.screenMousePos, screenPos) < distanceMarginActions)
                {
                    //startPosition = transform.position;
                    //hacemos el arrastre que este tocando
                }
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
                break;
            case charStates.ARRASTE:
                break;
            default:
                break;
        }


    }
}



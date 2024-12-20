using Unity.Entities;
using Unity.Mathematics;

public struct EspectadorVariables : IComponentData, IEnableableComponent
{
    public float distanceMarginActions; // = 10.0f;

    public float currentSpeed;// = 0.0f;

    public float3 crowdPoint;

    public enum espectatorStates { IDLE, JUMP, POGO, POGOEXIT, POGOEND, ARRASTE }
    public espectatorStates estado;// = charStates.IDLE;

    public float jumpForce;// = 20.0f;
    public float gravity;// = -21f;

    public float velocity;
    public float jumpVel;
    public float aceleration;
    public float3 directionalVel;
    public float distanceToMouseDown;
    public float pogoForce;

    public float minJumpForce;// = 1.0f;
    public float maxJumpForce;// = 10.0f;
}

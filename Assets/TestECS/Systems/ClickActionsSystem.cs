using NSprites;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class ClickActionsSystem : SystemBase
{
    public partial struct CheckClick : IJobEntity
    {
        public float time;
        public Unity.Mathematics.Random RandomGenerator; // Generador de aleatoriedad

        private void Execute(ref ChangeAnimTag e, ref LocalTransform tr, ref EspectadorVariables ev)
        {
            if (ev.estado == EspectadorVariables.espectatorStates.IDLE || ev.estado == EspectadorVariables.espectatorStates.POGOEND)
            {
                if (ClickDetector.instance.salto)
                {
                    float3 dist = math.abs(tr.Position - (float3)ClickDetector.instance.worldMousePosWhenDown);
                    if (math.length(dist) < ev.distanceMarginActions)
                    {
                        e.nextAnim = Animator.StringToHash("Arms"); //el de salto

                        //Debug.Log("posiciones y transform : " + tr.Position.y + " crowd : " + ev.crowdPoint.y);
                        ev.estado = EspectadorVariables.espectatorStates.JUMP;
                        ev.velocity = ev.jumpForce * ((ev.distanceMarginActions - math.length(dist)) / ev.distanceMarginActions);
                    }
                }
                else if (ClickDetector.instance.arrastre)
                {
                    float3 dist = math.abs(tr.Position - (float3)ClickDetector.instance.worldMousePos);
                    if (math.length(dist) < ev.distanceMarginActions)
                    {
                        ev.estado = EspectadorVariables.espectatorStates.ARRASTE;
                        ev.velocity = RandomGenerator.NextFloat(ev.minJumpForce, ev.maxJumpForce);
                        e.nextAnim = Animator.StringToHash("Run");
                    }
                }
                else if (ClickDetector.instance.pogo)
                {
                    float3 dist = math.abs(tr.Position - (float3)ClickDetector.instance.worldMousePosWhenDown);
                    if (math.length(dist) < ev.distanceMarginActions)
                    {
                        ev.estado = EspectadorVariables.espectatorStates.POGO;
                        //ev.velocity = RandomGenerator.NextFloat(ev.minJumpForce, ev.maxJumpForce);
                        e.nextAnim = Animator.StringToHash("Run");
                    }
                }
                else if (ClickDetector.instance.rePogo)
                {
                    float3 dist = math.abs(tr.Position - (float3)ClickDetector.instance.worldMousePosPOGOCOMENCE);
                    if (math.length(dist) < ev.distanceMarginActions)
                    {
                        e.nextAnim = Animator.StringToHash("Arms"); //el de salto

                        //Debug.Log("posiciones y transform : " + tr.Position.y + " crowd : " + ev.crowdPoint.y);
                        ev.estado = EspectadorVariables.espectatorStates.JUMP;
                        ev.velocity = ev.jumpForce * ((ev.distanceMarginActions - math.length(dist)) / ev.distanceMarginActions);
                    }
                }
            }
            else if (ev.estado == EspectadorVariables.espectatorStates.ARRASTE)
            {
                if (!ClickDetector.instance.arrastre)
                {
                    ev.estado = EspectadorVariables.espectatorStates.IDLE;
                }
            }
            else if (ev.estado == EspectadorVariables.espectatorStates.POGO)
            {
                if (ClickDetector.instance.pogoEnd)
                {
                    ev.aceleration = RandomGenerator.NextFloat(5.0f, 15.0f);
                    ev.jumpVel = RandomGenerator.NextFloat(ev.minJumpForce, ev.maxJumpForce);
                    ev.estado = EspectadorVariables.espectatorStates.POGOEND;
                }
            }

            /*
            Aqui se calculan los movimientos
            
            */
            if (ev.estado == EspectadorVariables.espectatorStates.JUMP)
            {
                tr.Position.y += (ev.velocity * time);
                ev.velocity += (ev.gravity * (time * 2));

                //chekear que ha llegado al suelo 
                if (ev.crowdPoint.y >= tr.Position.y)
                {
                    ev.velocity = 0.0f;

                    ev.estado = EspectadorVariables.espectatorStates.IDLE;
                    e.nextAnim = Animator.StringToHash("Idle"); //el de idle
                    tr.Position = ev.crowdPoint;
                }
            }
            else if (ev.estado == EspectadorVariables.espectatorStates.ARRASTE)
            {
                tr.Position.y += (ev.velocity * time);
                ev.velocity += (ev.gravity * (time * 2));

                //si se sale del rango de arrastre, devolver a Idle.
                //distanceToMouseDown = Vector3.Distance(detector.worldMousePos, crowdPoint);
                //if (distanceToMouseDown >= distanceMarginActions)

                float3 dist = math.abs(tr.Position - (float3)ClickDetector.instance.worldMousePos);
                if (math.length(dist) >= ev.distanceMarginActions)
                {
                    //chekear que ha llegado al suelo 
                    if (ev.crowdPoint.y >= tr.Position.y)
                    {
                        ev.velocity = 0.0f;
                        ev.estado = EspectadorVariables.espectatorStates.IDLE;
                        e.nextAnim = Animator.StringToHash("Idle"); //el de idle
                        tr.Position = ev.crowdPoint;
                        return;
                    }
                }

                //chekear que ha llegado al suelo 
                if (ev.crowdPoint.y >= tr.Position.y)
                {
                    e.nextAnim = Animator.StringToHash("Arms"); //el de idle
                    ev.velocity = RandomGenerator.NextFloat(ev.minJumpForce, ev.maxJumpForce);
                    tr.Position = ev.crowdPoint;
                }

            }
            else if (ev.estado == EspectadorVariables.espectatorStates.POGO)
            {
                float3 dir = tr.Position - (float3)ClickDetector.instance.worldMousePosWhenDown; //tambien es la dirección, cunado lo normalicemos

                float dist = math.length(math.abs(dir)) + ev.pogoDistantVariation;
                ev.pogoForce = (ev.distanceMarginActions - dist) / ev.distanceMarginActions;
                ev.velocity = ev.jumpForce * ev.pogoForce;


                dir = math.normalize(dir);

                float step = ev.velocity * time;

                //if (dist >= ev.distanceMarginActions)
                //{

                //}
                //else
                //{
                tr.Position += dir * step;
                //}

            }
            else if (ev.estado == EspectadorVariables.espectatorStates.POGOEND)
            {

                //imaginemos que se acabo un pogo, vuelve a su estado de idle, pues hacer que caminen a su posición

                float3 dir = ev.crowdPoint - tr.Position; //tambien es la dirección, cunado lo normalicemos

                float dist = math.length(math.abs(dir));

                dir.y = 0.0f; //para que el salto sea independiente
                dir = math.normalize(dir);

                ev.directionalVel += dir * ev.aceleration * (time * 2);
                //si esta muy cerca, se activa el damping, para impedir que se valla por las ramas

                if (dist < 4.0f)
                {
                    ev.directionalVel *= math.pow(0.5f, time); //damping del 0,5f a dos unidades de distancia
                }

                //adegurarse de que si esta saltando, o en pogo, llegue al suelo
                tr.Position.y += (ev.jumpVel * time);
                ev.jumpVel += (ev.gravity * (time * 2));

                //chekear que ha llegado al suelo 
                if (ev.crowdPoint.y >= tr.Position.y)
                {
                    ev.jumpVel = RandomGenerator.NextFloat(ev.minJumpForce, ev.maxJumpForce); // que salte otra vez
                    tr.Position.y = ev.crowdPoint.y;
                }

                tr.Position += ev.directionalVel * time; //velocidad de caminado puesot a apelo, //TODO

                if (math.length(ev.directionalVel) < 1.0f) //cuando la velocidad concuerde de que ha llegado a su fin, y que deje de saltar
                {
                    if (ev.crowdPoint.y >= tr.Position.y)
                    {
                        ev.jumpVel = 0.0f;
                        ev.directionalVel = float3.zero;
                        //ev.aceleration = 0.0f; //esto noe s necesario, ya qeu se calcula cada vez
                        tr.Position = ev.crowdPoint;
                        e.nextAnim = Animator.StringToHash("Idle"); //el de idle
                        ev.estado = EspectadorVariables.espectatorStates.IDLE;

                    }
                }

            }
            else if (ev.estado == EspectadorVariables.espectatorStates.IDLE && ev.crowdPoint.y != tr.Position.y)
            {
                //adegurarse de que si esta saltando, o en pogo, llegue al suelo
                tr.Position.y += (ev.velocity * time);
                ev.velocity += (ev.gravity * (time * 2));

                //chekear que ha llegado al suelo 
                if (ev.crowdPoint.y >= tr.Position.y)
                {
                    ev.velocity = 0.0f;

                    ev.estado = EspectadorVariables.espectatorStates.IDLE;
                    e.nextAnim = Animator.StringToHash("Idle"); //el de idle
                    tr.Position = ev.crowdPoint;
                }

                ////imaginemos que se acabo un pogo, vuelve a su estado de idle, pues hacer que caminen a su posición

                //float3 dir = tr.Position - ev.crowdPoint; //tambien es la dirección, cunado lo normalicemos

                //float dist = math.length(math.abs(dir));

                //if (dist > 0.5f)
                //{
                //    tr.Position += dir * 5.0f * time; //velocidad de caminado Idle puesot a apelo, //TODO
                //}
                //else
                //{
                //    //no voy a poner que se teletrasnporte a la posición porque leugo al lio, pero habria que ahcer algo si no esta cerca claro esta
                //    //tr.Position
                //}

            }
            else if (ev.estado == EspectadorVariables.espectatorStates.CAMINANDO)
            {
                float3 dir = ev.crowdPoint - tr.Position; //tambien es la dirección, cunado lo normalicemos
                float dist = math.length(math.abs(dir));

                dir.y = 0.0f; //para que el salto sea independiente
                dir = math.normalize(dir);

                tr.Position += dir * ev.velocity * time; //velocidad de caminado puesot a apelo, //TODO

                if (dist <= 0.5f)
                {
                    tr.Position = ev.crowdPoint;
                    ev.estado = EspectadorVariables.espectatorStates.IDLE;
                    e.nextAnim = Animator.StringToHash("Idle");
                }
            }
        }
    }

    private struct SystemData : IComponentData
    {
        public EntityQuery MovableQuery;
    }

    [BurstCompile]
    protected override void OnCreate()
    {
        var systemData = new SystemData();

        var queryBuilder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ChangeAnimTag>()
            .WithAll<EspectadorVariables>()
            .WithAll<LocalTransform>()
            .WithAspect<AnimatorAspect>()
            .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState);

        var movableQuery = GetEntityQuery(queryBuilder);

        systemData.MovableQuery = movableQuery;

        _ = EntityManager.AddComponentData(SystemHandle, systemData);

        queryBuilder.Dispose();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var systemData = SystemAPI.GetComponent<SystemData>(SystemHandle);
        if (!SystemAPI.TryGetSingleton<PonchoAnimSettings>(out var animationSettings))
            return;
        var time = SystemAPI.Time.DeltaTime;
        uint seed = (uint)UnityEngine.Random.Range(1, int.MaxValue);

        EntityCommandBuffer entityCB = new(Allocator.Temp);
        var animationSwitchJob = new CheckClick
        {
            time = time,
            RandomGenerator = new Unity.Mathematics.Random(seed)
        };
        Dependency = animationSwitchJob.ScheduleParallelByRef(systemData.MovableQuery, Dependency);

        entityCB.Playback(EntityManager);
    }
}
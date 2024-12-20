using NSprites;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class ClickActionsSystem : SystemBase
{    public partial struct CheckClick : IJobEntity
    {
        public float time;
        public Unity.Mathematics.Random RandomGenerator; // Generador de aleatoriedad

        private void Execute(ref ChangeAnimTag e, ref LocalTransform tr, ref EspectadorVariables ev)
        {
            if (ev.estado == EspectadorVariables.espectatorStates.IDLE)
            {
                if(ClickDetector.instance.salto)
                {
                    float3 dist = math.abs(tr.Position - (float3)ClickDetector.instance.worldMousePosWhenDown);
                    if (math.length(dist) < ev.distanceMarginActions)
                    {
                        e.nextAnim = Animator.StringToHash("Death"); //el de salto

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
                        e.nextAnim = Animator.StringToHash("WalkBack");
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
                        tr.Position = ev.crowdPoint;
                        return;
                    }
                }

                //chekear que ha llegado al suelo 
                if (ev.crowdPoint.y >= tr.Position.y)
                {
                    ev.velocity = RandomGenerator.NextFloat(ev.minJumpForce, ev.maxJumpForce);
                    tr.Position = ev.crowdPoint;
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
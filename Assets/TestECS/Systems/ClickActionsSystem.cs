using NSprites;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(SpriteUVAnimationSystem))]
public partial class ClickActionsSystem : SystemBase
{
    //private ClickDetector click;

    //[BurstCompile]
    public partial struct CheckClick : IJobEntity
    {
        public double Time;

        private void Execute(ref ChangeAnimTag e, LocalTransform tr)
        {
            if(ClickDetector.instance.salto || ClickDetector.instance.pogo)
            {
                float3 dist = math.abs(tr.Position - (float3)ClickDetector.instance.worldMousePosWhenDown);
                if (math.length(dist) < ClickDetector.instance.distanceMarginActions)
                {
                    e.nextAnim = Animator.StringToHash("Death");
                    Debug.Log("Holo " + dist);
                }
            }
        }
    }

    private struct SystemData : IComponentData
    {
        public EntityQuery MovableQuery;
    }

    //[BurstCompile]
    protected override void OnCreate()
    {
        //click = ClickDetector.instance;
        //if (click == null)
        //    UnityEngine.Debug.LogError("No click detector found");

        var systemData = new SystemData();

        var queryBuilder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ChangeAnimTag>()
            .WithAll<LocalTransform>()
            .WithAspect<AnimatorAspect>()
            .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState);

        var movableQuery = GetEntityQuery(queryBuilder);

        systemData.MovableQuery = movableQuery;

        _ = EntityManager.AddComponentData(SystemHandle, systemData);

        queryBuilder.Dispose();
    }

    //[BurstCompile]
    protected override void OnUpdate()
    {
        var systemData = SystemAPI.GetComponent<SystemData>(SystemHandle);
        if (!SystemAPI.TryGetSingleton<PonchoAnimSettings>(out var animationSettings))
            return;
        var time = SystemAPI.Time.ElapsedTime;

        EntityCommandBuffer entityCB = new(Allocator.Temp);
        var animationSwitchJob = new CheckClick
        {
            Time = time
        };
        Dependency = animationSwitchJob.ScheduleParallelByRef(systemData.MovableQuery, Dependency);

        entityCB.Playback(EntityManager);
    }
}
using NSprites;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct ClickActionsSystem : ISystem
{
    //private ClickDetector click;

    [BurstCompile]
    public partial struct CheckClick : IJobEntity
    {
        public double Time;
        public EntityCommandBuffer ecb;

        private void Execute(Entity e, LocalToWorld tr)
        {
            float3 dist = math.abs(tr.Position - (float3)ClickDetector.instance.worldMousePosWhenDown);
            if (math.length(dist) < ClickDetector.instance.distanceMarginActions)
            {
                ecb.AddComponent(e, new ChangeAnimTag { nextAnim = Animator.StringToHash("Death") });
            }
        }
    }

    private struct SystemData : IComponentData
    {
        public EntityQuery MovableQuery;
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //click = ClickDetector.instance;
        //if (click == null)
        //    UnityEngine.Debug.LogError("No click detector found");

        var systemData = new SystemData();

        var queryBuilder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ChangeAnimTag>()
            .WithAspect<AnimatorAspect>()
            .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState);

        var movableQuery = state.GetEntityQuery(queryBuilder);

        movableQuery.AddChangedVersionFilter(ComponentType.ReadOnly<ChangeAnimTag>());
        systemData.MovableQuery = movableQuery;

        _ = state.EntityManager.AddComponentData(state.SystemHandle, systemData);

        queryBuilder.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var systemData = SystemAPI.GetComponent<SystemData>(state.SystemHandle);
        if (!SystemAPI.TryGetSingleton<PonchoAnimSettings>(out var animationSettings))
            return;
        var time = SystemAPI.Time.ElapsedTime;

        EntityCommandBuffer entityCB = new(Allocator.Temp);
        var animationSwitchJob = new CheckClick
        {
            Time = time,
            ecb = entityCB
        };
        state.Dependency = animationSwitchJob.ScheduleParallelByRef(systemData.MovableQuery, state.Dependency);

        entityCB.Playback(World.DefaultGameObjectInjectionWorld.EntityManager);
    }
}
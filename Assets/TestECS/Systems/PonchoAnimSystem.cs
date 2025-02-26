using NSprites;
using Unity.Entities;
using Unity.Burst;

[BurstCompile]
[UpdateBefore(typeof(SpriteUVAnimationSystem))]
public partial struct PonchoAnimSystem : ISystem
{
    [BurstCompile]
    private partial struct ChangeAnimationJob : IJobEntity
    {
        public double Time;

        private void Execute(AnimatorAspect animator, ChangeAnimTag tag)
        {            
            animator.SetAnimation(tag.nextAnim, Time);
        }
    }

    private struct SystemData : IComponentData
    {
        public EntityQuery MovableQuery;
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var systemData = new SystemData();

        var queryBuilder = new EntityQueryBuilder(Unity.Collections.Allocator.Temp)
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

        var animationSwitchJob = new ChangeAnimationJob
        {
            Time = time
        };
        state.Dependency = animationSwitchJob.ScheduleParallelByRef(systemData.MovableQuery, state.Dependency);
    }
}
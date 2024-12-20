using NSprites;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

[BurstCompile]
[CreateAfter(typeof(SpawnPublicSystem))]
public partial class RandomEntitySystem : SystemBase
{
    private NativeArray<Entity> entities;
    private Unity.Mathematics.Random RandomGenerator;

    [BurstCompile]
    private partial struct GetRandomEntity : IJobEntity
    {
        public double Time;

        private void Execute(AnimatorAspect animator, ChangeAnimTag tag)
        {
            animator.SetAnimation(tag.nextAnim, Time);
        }
    }
    [BurstCompile]
    protected override void OnStartRunning()
    {
        var queryBuilder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<LocalTransform>()
            .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState);
        entities = GetEntityQuery(queryBuilder).ToEntityArray(Allocator.Persistent);

        queryBuilder.Dispose();

        uint seed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
        RandomGenerator = new Unity.Mathematics.Random(seed);
    }

    public float3 GetRandomEntityPos()
    {
        if (entities == null) return new float3(0, 0, 0);
        return SystemAPI.GetComponent<LocalTransform>(entities[RandomGenerator.NextInt(entities.Length - 1)]).Position;
    }
    protected override void OnDestroy()
    {
        entities.Dispose();
    }
    protected override void OnUpdate()
    {

    }
}
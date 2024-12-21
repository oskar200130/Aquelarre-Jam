using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

[BurstCompile]
public partial class SpawnPublicSystem : SystemBase
{
    private Unity.Mathematics.Random RandomGenerator; // Generador de aleatoriedad

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<SpawnerComponent>();
        uint seed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
        RandomGenerator = new Unity.Mathematics.Random(seed);
    }
    [BurstCompile]
    protected override void OnStartRunning()
    {
        SpawnerComponent spawner = SystemAPI.GetSingleton<SpawnerComponent>();

        EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp);
        for (int i = 0; i < spawner.spawnNumberPeople; i++)
        {
            Entity newEntity = ecb.Instantiate(spawner.prefab);
            float x = (i % spawner.spawnLengthNumber) * spawner.spawnInitialSeparation + RandomGenerator.NextFloat(-spawner.spawnVariationPos, spawner.spawnVariationPos);
            float z = (i / (int)spawner.spawnLengthNumber) * spawner.spawnInitialSeparation + RandomGenerator.NextFloat(-spawner.spawnVariationPos, spawner.spawnVariationPos);
            ecb.SetComponent(newEntity, LocalTransform.FromPosition(new float3(x, 0, z)));

            if (RandomGenerator.NextBool())
                ecb.SetComponent(newEntity, LocalTransform.FromRotation(new quaternion(0, 1, 0, 0)));
            else
                ecb.SetComponent(newEntity, LocalTransform.FromRotation(new quaternion(0, 0, 0, 1)));
            ecb.AddComponent(newEntity, new ChangeAnimTag { nextAnim = Animator.StringToHash("Idle") });
            ecb.AddComponent(newEntity, new EspectadorVariables
            {
                distanceMarginActions = 10.0f,
                currentSpeed = 0.0f,
                crowdPoint = new float3(x, 0, z),
                estado = EspectadorVariables.espectatorStates.IDLE,
                jumpForce = 20.0f,
                gravity = -21.0f,
                velocity = 0.0f,
                jumpVel = 0.0f,
                aceleration = 0.0f,
                directionalVel = new float3(0.0f, 0.0f, 0.0f),
                distanceToMouseDown = 0.0f,
                pogoForce = 0.0f,
                pogoDistantVariation = RandomGenerator.NextFloat(-spawner.pogoVariationPos, spawner.pogoVariationPos),
                minJumpForce = 1.0f,
                maxJumpForce = 10.0f
            });
        }
        ecb.Playback(EntityManager);
    }
    protected override void OnUpdate()
    {

    }
}

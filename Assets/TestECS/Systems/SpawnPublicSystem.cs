using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using System.Diagnostics;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;

[BurstCompile]
public partial class SpawnPublicSystem : SystemBase
{
    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<SpawnerComponent>();
    }
    [BurstCompile]
    protected override void OnStartRunning()
    {
        SpawnerComponent spawner = SystemAPI.GetSingleton<SpawnerComponent>();

        EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp);
        for (int i = 0; i < spawner.spawnNumberPeople; i++)
        {
            Entity newEntity = ecb.Instantiate(spawner.prefab);
            ecb.AddComponent(newEntity, new MovePeopleComponent { destiny = new float3((i % spawner.spawnLengthNumber)*1.1f, (i / (int)spawner.spawnLengthNumber)*1.1f, 0) });
            //ecb.AddComponent(newEntity, new ChangeAnimTag { nextAnim = Animator.StringToHash("Death") });
        }
        ecb.Playback(EntityManager);
    }
    protected override void OnUpdate()
    {
        
    }
}

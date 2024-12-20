using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

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

        //EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        SpawnerComponent spawner = SystemAPI.GetSingleton<SpawnerComponent>();

        EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp);
        for (int i = 0; i < spawner.spawnNumberPeople; i++)
        {
            Entity newEntity = ecb.Instantiate(spawner.prefab);

            //tal vez asi funcione directamente para spawnearlos
            //ecb.set<LocalTransform>(newEntity, new LocalTransform { Position = new float3((i % spawner.spawnLengthNumber) * 1.1f, 0, (i / (int)spawner.spawnLengthNumber) * 1.1f) });
            //no funciono

            //quito esta barbaridad porque crea un job que no apra de teletrasnportar a los pibes al mismo sitio
            //ecb.AddComponent(newEntity, new MovePeopleComponent { destiny = new float3((i % spawner.spawnLengthNumber)*1.1f, 0, (i / (int)spawner.spawnLengthNumber)*1.1f) });            

            //este si que vale
            //ecb.SetComponent(newEntity, new TargetPositionComponent
            //{
            //    Value = new float3((i % spawner.spawnLengthNumber) * 1.1f, 0, (i / (int)spawner.spawnLengthNumber) * 1.1f)
            //});


            //esto funciona siquiera porque???
            //entityManager.SetComponentData(newEntity, LocalTransform.FromPosition(new float3((i % spawner.spawnLengthNumber) * 1.1f, 0, (i / (int)spawner.spawnLengthNumber) * 1.1f)));
            //entity manager no se puede uitilizar

            //esta es la solución! pero creo que peirden al padre, lo cual es putadon? o no? seguramente no.
            ecb.SetComponent(newEntity, LocalTransform.FromPosition(new float3((i % spawner.spawnLengthNumber) * 1.1f, 0, (i / (int)spawner.spawnLengthNumber) * 1.1f)));
            
            ecb.AddComponent(newEntity, new ChangeAnimTag { nextAnim = Animator.StringToHash("Idle") });            
            ecb.AddComponent(newEntity, new EspectadorVariables
            {   distanceMarginActions = 10.0f,
                currentSpeed = 0.0f,
                crowdPoint = new float3((i % spawner.spawnLengthNumber) * 1.1f, 0, (i / (int)spawner.spawnLengthNumber) * 1.1f),
                estado = EspectadorVariables.espectatorStates.IDLE,
                jumpForce = 20.0f,
                gravity = -21.0f,
                velocity = 0.0f,
                distanceToMouseDown = 0.0f,
                pogoForce = 0.0f,
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

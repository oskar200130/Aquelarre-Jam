using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using NSprites;

[BurstCompile]
public partial class SpawnPublicSystem : SystemBase
{
    private Unity.Mathematics.Random RandomGenerator; // Generador de aleatoriedad
    private int numberEntities;

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<SpawnerComponent>();
        uint seed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
        RandomGenerator = new Unity.Mathematics.Random(seed);
        numberEntities = 0;
    }


    private SpawnerComponent spawner;
    private LevelManager beatManageador;
    [BurstCompile]
    protected override void OnStartRunning()
    {

        spawner = SystemAPI.GetSingleton<SpawnerComponent>();
        beatManageador = LevelManager._instance;
    }


    [BurstCompile]
    protected override void OnUpdate()
    {
        if (numberEntities != beatManageador.puntuacion)
        {
            EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp);

            for (int i = numberEntities; i < beatManageador.puntuacion; i++, numberEntities++)
            {

                //replantear las posiciones de spawn para que sean en un semi circulo desde el escenario (a saber como son esas mates)
                Entity newEntity = ecb.Instantiate(spawner.prefab);
                ecb.SetComponent(newEntity, new SortingData(1, 1));
                int rowPos = (int)spawner.spawnLengthNumber / 2 + (i % 2 == 0 ? -(i % (int)spawner.spawnLengthNumber) / 2 : ((i % (int)spawner.spawnLengthNumber) + 1) / 2);
                float x = rowPos * spawner.spawnInitialSeparation + RandomGenerator.NextFloat(-spawner.spawnVariationPos, spawner.spawnVariationPos);
                //lo he puesto en negativo, apra que el publico crezca hacia atras y no hacia el escenario jjjj
                float z = -(i / (int)spawner.spawnLengthNumber) * spawner.spawnInitialSeparation + RandomGenerator.NextFloat(-spawner.spawnVariationPos, spawner.spawnVariationPos);

                int color = RandomGenerator.NextInt(1, 4);

                float3 SpawnPoint = GetNearestOutsidePosition(new float3(x, 0, z));

                quaternion rotation;
                if (RandomGenerator.NextBool()) 
                    rotation = new quaternion(0.34f, 0f, 0f, 0.94f);
                else
                    rotation = new quaternion(0f, 0.94f, 0.34f, 0f);
                
                ecb.SetComponent(newEntity, LocalTransform.FromPositionRotation(SpawnPoint, rotation));

                ecb.AddComponent(newEntity, new ChangeAnimTag { nextAnim = Animator.StringToHash("Idle" + color) });
                ecb.AddComponent(newEntity, new EspectadorVariables
                {
                    distanceMarginActions = 10.0f,
                    currentSpeed = 0.0f,
                    crowdPoint = new float3(x, 0, z),
                    estado = EspectadorVariables.espectatorStates.CAMINANDO,
                    jumpForce = 20.0f,
                    gravity = -21.0f,
                    velocity = 20.0f,
                    jumpVel = 0.0f,
                    aceleration = 0.0f,
                    directionalVel = new float3(0.0f, 0.0f, 0.0f),
                    distanceToMouseDown = 0.0f,
                    pogoForce = 0.0f,
                    pogoDistantVariation = RandomGenerator.NextFloat(-spawner.pogoVariationPos, spawner.pogoVariationPos),
                    minJumpForce = 1.0f,
                    maxJumpForce = 10.0f,
                    colorid = color

                }); ;
            }
            ecb.Playback(EntityManager);

        }
    }



    float3 GetNearestOutsidePosition(float3 insidePos)
    {
        //direccion, centro camara mundo con el punto, y calcular luego distancia de centro mundo a esquina camara mundo


        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));


        // Calculate the intersection with the (x, 0, z) plane
        float t = -ray.origin.y / ray.direction.y;

        float3 centro = ray.origin + ray.direction * t;

        float3 dir = math.normalize(insidePos - centro);
        if (dir.z > 0) dir.z = 0;

        //vamos a calcular la distancia con el punto de la esquina inferior izquierda, y usar esa distancia como punto de spawneo
        Ray ray2 = Camera.main.ScreenPointToRay(new Vector3(0, 0, 0));
        t = -ray2.origin.y / ray2.direction.y;
        float3 esquina = ray2.origin + ray2.direction * t;
        float3 distancia = math.abs(math.length(centro - esquina));

        return centro + (dir * distancia);

    }
    //[BurstCompile]
    //protected override void OnDestroy()
    //{
    //}

}

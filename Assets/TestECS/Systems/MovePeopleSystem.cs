using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct MovePeopleSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityMng = state.EntityManager;

        NativeArray<Entity> entities = entityMng.GetAllEntities(Allocator.Temp);

        foreach (Entity entity in entities)
        {
            if (entityMng.HasComponent<MovePeopleComponent>(entity))
            {
                MovePeopleComponent move = entityMng.GetComponentData<MovePeopleComponent>(entity);
                LocalTransform localTrf = entityMng.GetComponentData<LocalTransform>(entity);
                if (localTrf.Position.x != move.destiny.x || localTrf.Position.y != move.destiny.y)
                {
                    localTrf.Position = move.destiny;
                }
                entityMng.SetComponentData(entity, localTrf);
            }
        }
    }
}

using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct MovePeopleSystem : ISystem
{

    public void OnEnable(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //new MovePersonJob{}.ScheduleParallel(state.Dependency).Complete();
    }
}

[BurstCompile]
public partial struct MovePersonJob : IJobEntity
{
    [BurstCompile]
    private void Execute(MoveAspect aspect)
    {
        aspect.Move();
    }
}

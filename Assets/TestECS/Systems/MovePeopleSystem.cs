using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.VisualScripting;

[BurstCompile]
public partial struct MovePeopleSystem : ISystem
{

    public void OnEnable(ref SystemState state)
    {
        
    }
    
    public void OnUpdate(ref SystemState state)
    {
        new MovePersonJob{}.ScheduleParallel(state.Dependency).Complete();
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

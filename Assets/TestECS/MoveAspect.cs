using Unity.Entities;
using Unity.Transforms;

public readonly partial struct MoveAspect : IAspect
{
    private readonly RefRW<LocalTransform> _localTransform;
    private readonly RefRO<MovePeopleComponent> _targetPosition;

    public void Move()
    {
        _localTransform.ValueRW.Position = _targetPosition.ValueRO.destiny;
    }
}
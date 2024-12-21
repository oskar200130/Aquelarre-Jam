using Unity.Entities;
using Unity.Mathematics;

public struct SpawnerComponent : IComponentData
{
    public Entity prefab;
    public float3 spawnZone;
    public float spawnNumberPeople;
    public float spawnLengthNumber;
    public float spawnVariationPos;
    public float spawnInitialSeparation;
    public float pogoVariationPos;
}

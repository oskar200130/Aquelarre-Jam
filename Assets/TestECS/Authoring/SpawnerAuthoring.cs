using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float spawnNumber;
    public float spawnLenght;
    public float spawnSeparation;
    public float spawnVariationPos;
    public float pogoVariationPos;

    class SpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnerComponent{
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.None),
                spawnZone = authoring.transform.position,
                spawnNumberPeople = authoring.spawnNumber,
                spawnLengthNumber = authoring.spawnLenght,
                spawnVariationPos = authoring.spawnVariationPos,
                spawnInitialSeparation = authoring.spawnSeparation,
                pogoVariationPos = authoring.pogoVariationPos
            });
        }
    }
}

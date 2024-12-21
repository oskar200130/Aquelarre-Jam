using Unity.Entities;
using UnityEngine;

public class PonchoAnimAuthoring : MonoBehaviour
{
    private class PonchoAnimBaker : Baker<PonchoAnimAuthoring>
    {
        public override void Bake(PonchoAnimAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new PonchoAnimSettings
            {
                IdleHash = Animator.StringToHash("Idle"),
                RunHash = Animator.StringToHash("Run"),
                ArmsHash = Animator.StringToHash("Arms")
            });
        }
    }
}
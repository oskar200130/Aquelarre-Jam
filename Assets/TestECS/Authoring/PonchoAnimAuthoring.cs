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
                IdleHash = Animator.StringToHash("idle"),
                WalkHash = Animator.StringToHash("walk"),
                WalkBackHash = Animator.StringToHash("walkBack"),
                HitHash = Animator.StringToHash("hit"),
                DeathHash = Animator.StringToHash("death")
            });
        }
    }
}
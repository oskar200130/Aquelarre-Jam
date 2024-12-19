using Unity.Entities;

public struct ChangeAnimTag : IComponentData, IEnableableComponent
{
    public int nextAnim;
}

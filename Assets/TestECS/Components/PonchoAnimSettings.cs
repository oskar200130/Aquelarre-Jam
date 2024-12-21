using Unity.Entities;

public struct PonchoAnimSettings : IComponentData
{
    public int IdleHash;
    public int RunHash;
    public int ArmsHash;
}
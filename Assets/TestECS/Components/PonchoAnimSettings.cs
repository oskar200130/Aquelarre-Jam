using Unity.Entities;

public struct PonchoAnimSettings : IComponentData
{
    public int IdleHash;
    public int WalkHash;
    public int WalkBackHash;
    public int HitHash;
    public int DeathHash;
}
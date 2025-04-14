using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct AISystem : ISystem
{
    public void OnCreate(ref SystemState state) { }
    public void OnUpdate(ref SystemState state)
    {
        // TODO: AI 决策逻辑（移动、攻击）
    }
    public void OnDestroy(ref SystemState state) { }
}

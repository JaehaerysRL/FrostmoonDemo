using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct TurnSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }
    public void OnUpdate(ref SystemState state)
    {
        // TODO: 管理回合队列，切换当前角色
    }
    public void OnDestroy(ref SystemState state) { }
}

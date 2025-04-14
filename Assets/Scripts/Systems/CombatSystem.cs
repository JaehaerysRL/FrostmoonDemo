using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct CombatSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }
    public void OnUpdate(ref SystemState state)
    {
        // TODO: 攻击判定、命中、伤害计算
    }
    public void OnDestroy(ref SystemState state) { }
}

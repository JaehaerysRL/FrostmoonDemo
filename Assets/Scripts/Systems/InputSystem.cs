using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct InputSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }
    public void OnUpdate(ref SystemState state)
    {
        // TODO: 获取鼠标点击、选中单位等输入处理
    }
    public void OnDestroy(ref SystemState state) { }
}

using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct BiomeCenter : IBufferElementData
{
    public int2 Position;
}

[InternalBufferCapacity(4)]
public struct FrozenLakeCenter : IBufferElementData
{
    public int2 Position;
}

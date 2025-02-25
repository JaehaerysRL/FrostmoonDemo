#nullable enable
using System;
using Cysharp.Threading.Tasks;

#pragma warning disable CA2255

// 文件范围的命名空间
namespace PolySharp.Tests;

internal class TestClass
{
    TestClass(string id, string? name)
    {
        ID = id;
        this.name = name;
    }

    // 可空引用类型
    private string? name;
    // 初始化必须与初始化不可变
    public required string ID { get; init; }

    internal void Test()
    {
        // record
        Person person = new("Tom");
        Console.WriteLine(person);
        // output: Person { Name = Tom, PhoneNumbers = [] }

        // 模式匹配
        // switch 表达式
        static Point Transform(Point point) => point switch
        {
            { X: 0, Y: 0 } => new Point(0, 0),
            { X: var x, Y: var y } when x < y => new Point(x + y, y),
            { X: var x, Y: var y } when x > y => new Point(x - y, y),
            { X: var x, Y: var y } => new Point(2 * x, 2 * y),
        };
        // 属性模式
        static bool IsOrigin(Point point) => point is { X: 0, Y: 0 };
        // 位置模式
        static string Classify(Point point) => point switch
        {
            (0, 0) => "Origin",
            (var x, var y) when x == y => "Diagonal",
            (var x, var y) when x > y => "Above the diagonal",
            (var x, var y) => "Below the diagonal",
        };

    }

    // 异步方法（UniTask）
    async UniTask<string> DemoAsync()
    {
        await UniTask.Delay(1000);
        return "Hello";
    }
}

// 记录,用于定义不可变的数据结构,不需要声明任何位置属性,也可以声明其他属性
public record Person(string Name)
{
    public long SomeNumber { get; init; } = 0;
};

interface ISampleInterface
{
    // 默认接口方法
    void SampleMethod()
    {
        Console.WriteLine("SampleMethod");
    }
}

public readonly struct Point
{
    public int X { get; }
    public int Y { get; }
    public Point(int x, int y) => (X, Y) = (x, y);
    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
}



using UnityEngine;
using System.Text;

public static class SmartLog
{
    // 颜色配置
    private static readonly Color tagColor = new Color(1f, 1f, 1f); // 白色
    private static readonly Color debugColor = new Color(0.5f, 0.5f, 1f); // 蓝色
    private static readonly Color infoColor = new Color(0.5f, 1f, 0.5f); // 绿色
    private static readonly Color warningColor = new Color(1f, 1f, 0.5f); // 黄色
    private static readonly Color errorColor = new Color(1f, 0.5f, 0.5f); // 红色

    // 日志格式化
    private static void FormatMessage(Color paramColor, StringBuilder sb, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (i == 0 && args[0] is string str)
            {
                sb.Append($"<color=#{ColorUtility.ToHtmlStringRGB(tagColor)}>");
                sb.Append($"[{args[0]}] ");
                sb.Append("</color>");
                continue;
            }

            var obj = args[i];
            sb.Append($"<color=#{ColorUtility.ToHtmlStringRGB(paramColor)}>");
            sb.Append(obj as string ?? obj.ToString());
            sb.Append("</color>");

            if (ShouldAddColon(i, args)) sb.Append(":");
        }
    }

    private static bool ShouldAddColon(int index, object[] args)
    {
        return index < args.Length - 1
            && args[index] is string current
            && current.EndsWith(":")
            && !(args[index + 1] is string next && next.EndsWith(":"));
    }

    // 日志输出
    public static void Log(params object[] args)
    {
        StringBuilder sb = new StringBuilder();
        FormatMessage(debugColor, sb, args);
        Debug.Log(sb.ToString());
    }

    public static void Info(params object[] args)
    {
        StringBuilder sb = new StringBuilder();
        FormatMessage(infoColor, sb, args);
        Debug.Log(sb.ToString());
    }

    public static void Warning(params object[] args)
    {
        StringBuilder sb = new StringBuilder();
        FormatMessage(warningColor, sb, args);
        Debug.LogWarning(sb.ToString());
    }

    public static void Error(params object[] args)
    {
        StringBuilder sb = new StringBuilder();
        FormatMessage(errorColor, sb, args);
        Debug.LogError(sb.ToString());
    }
}
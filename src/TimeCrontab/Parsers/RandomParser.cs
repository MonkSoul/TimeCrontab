// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using System;

namespace TimeCrontab;

/// <summary>
/// Cron 字段值含 R 字符解析器
/// </summary>
/// <remarks>
/// <para>R 表示随机生成的时刻，仅在 <see cref="CrontabFieldKind.Second"/>、<see cref="CrontabFieldKind.Minute"/> 或 <see cref="CrontabFieldKind.Hour"/> 字段域中使用。</para>
/// <para>参考文献：https://help.eset.com/protect_admin/10.0/zh-CN/cron_expression.html。</para>
/// </remarks>
internal sealed class RandomParser : ICronParser, ITimeParser
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="TimeCrontabException"></exception>
    public RandomParser(CrontabFieldKind kind)
    {
        // 验证 R 字符是否在 Second、Minute 或 Hour 字段域中使用
        if (kind != CrontabFieldKind.Second &&
            kind != CrontabFieldKind.Minute &&
            kind != CrontabFieldKind.Hour)
        {
            throw new TimeCrontabException("The <R> parser can only be used with the Second, Minute, or Hour fields.");
        }

        Kind = kind;
    }

    /// <summary>
    /// Cron 字段种类
    /// </summary>
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// 判断当前时间是否符合 Cron 字段种类解析规则
    /// </summary>
    /// <param name="datetime">当前时间</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsMatch(DateTime datetime)
    {
        return true;
    }

    /// <summary>
    /// 获取 Cron 字段种类当前值的下一个发生值
    /// </summary>
    /// <param name="currentValue">时间值</param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    public int? Next(int currentValue)
    {
        return GenerateRandomValue();
    }

    /// <summary>
    /// 获取 Cron 字段种类字段起始值
    /// </summary>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    public int First()
    {
        return 0;
    }

    /// <summary>
    /// 将解析器转换成字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return "R";
    }

    /// <summary>
    /// 随机对象
    /// </summary>
    private static readonly Random random = new();

    /// <summary>
    /// 生成指定范围的随机值
    /// </summary>
    /// <returns><see cref="int"/></returns>
    private int GenerateRandomValue()
    {
        // 获取 Cron 字段种类最小值和最大值
        var minimum = Constants.MinimumDateTimeValues[Kind];
        var maximum = Constants.MaximumDateTimeValues[Kind];

        // 生成最小值和最大值之间的随机数
        return random.Next(minimum, maximum + 1);
    }
}
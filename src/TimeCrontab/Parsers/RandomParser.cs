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
/// <para>支持区间随机：Rmin-max，例如 R30-59 表示在 30 到 59 之间随机。</para>
/// <para>参考文献：https://help.eset.com/protect_admin/13.0/zh-CN/cron_expression.html。</para>
/// </remarks>
internal sealed class RandomParser : ICronParser, ITimeParser
{
    /// <summary>
    /// 全局随机种子生成器（线程安全锁）
    /// </summary>
    private static readonly Random _globalRandom = new();

    /// <summary>
    /// 线程独立的随机实例
    /// </summary>
    [ThreadStatic]
    private static Random _localRandom;

    /// <summary>
    /// 获取当前线程的随机实例
    /// </summary>
    private static Random GetRandom()
    {
        if (_localRandom == null)
        {
            int seed;
            lock (_globalRandom)
            {
                seed = _globalRandom.Next();
            }
            _localRandom = new Random(seed);
        }

        return _localRandom;
    }

    /// <summary>
    /// 随机范围最小值（包含）
    /// </summary>
    private readonly int _minValue;

    /// <summary>
    /// 随机范围最大值（包含）
    /// </summary>
    private readonly int _maxValue;

    /// <summary>
    /// 构造函数（全范围随机）
    /// </summary>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="TimeCrontabException"></exception>
    public RandomParser(CrontabFieldKind kind)
        : this(kind, Constants.MinimumDateTimeValues[kind], Constants.MaximumDateTimeValues[kind])
    {
    }

    /// <summary>
    /// 构造函数（指定随机区间）
    /// </summary>
    /// <param name="kind">Cron 字段种类</param>
    /// <param name="minValue">最小值（包含）</param>
    /// <param name="maxValue">最大值（包含）</param>
    /// <exception cref="TimeCrontabException"></exception>
    public RandomParser(CrontabFieldKind kind, int minValue, int maxValue)
    {
        // 验证 R 字符是否在 Second、Minute 或 Hour 字段域中使用
        if (kind != CrontabFieldKind.Second &&
            kind != CrontabFieldKind.Minute &&
            kind != CrontabFieldKind.Hour)
        {
            throw new TimeCrontabException("The <R> parser can only be used with the Second, Minute, or Hour fields.");
        }

        var fieldMin = Constants.MinimumDateTimeValues[kind];
        var fieldMax = Constants.MaximumDateTimeValues[kind];

        // 验证区间有效性
        if (minValue < fieldMin || minValue > fieldMax)
        {
            throw new TimeCrontabException($"The minimum value {minValue} is out of bounds for the {kind} field.");
        }

        if (maxValue < fieldMin || maxValue > fieldMax)
        {
            throw new TimeCrontabException($"The maximum value {maxValue} is out of bounds for the {kind} field.");
        }

        if (minValue > maxValue)
        {
            throw new TimeCrontabException($"The minimum value {minValue} cannot be greater than the maximum value {maxValue}.");
        }

        Kind = kind;
        _minValue = minValue;
        _maxValue = maxValue;
    }

    /// <summary>
    /// Cron 字段种类
    /// </summary>
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// 判断当前时间是否符合 Cron 字段种类解析规则
    /// </summary>
    /// <remarks>
    /// <para>由于 R 字段的值是在运行时动态生成的，固定的时间值无法匹配，因此总是返回 false。</para>
    /// <para>实际调度由 <see cref="Crontab.InternalGetNextOccurence"/> 中的特判逻辑处理。</para>
    /// </remarks>
    /// <param name="datetime">当前时间</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsMatch(DateTime datetime)
    {
        return false;
    }

    /// <summary>
    /// 获取 Cron 字段种类当前值的下一个发生值
    /// </summary>
    /// <param name="currentValue">时间值</param>
    /// <returns><see cref="int"/></returns>
    public int? Next(int currentValue)
    {
        // 在指定区间内生成随机数
        return GetRandom().Next(_minValue, _maxValue + 1);
    }

    /// <summary>
    /// 获取 Cron 字段种类当前值的上一个发生值
    /// </summary>
    /// <param name="currentValue">时间值</param>
    /// <returns><see cref="int"/></returns>
    public int? Previous(int currentValue)
    {
        return GetRandom().Next(_minValue, _maxValue + 1);
    }

    /// <summary>
    /// 获取 Cron 字段种类字段起始值（区间最小值）
    /// </summary>
    /// <returns><see cref="int"/></returns>
    public int First()
    {
        return _minValue;
    }

    /// <summary>
    /// 获取 Cron 字段种类字段末尾值（区间最大值）
    /// </summary>
    /// <returns><see cref="int"/></returns>
    public int Last()
    {
        return _maxValue;
    }

    /// <summary>
    /// 将解析器转换成字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        // 全范围则输出 "R"，否则输出 "Rmin-max"
        var fieldMin = Constants.MinimumDateTimeValues[Kind];
        var fieldMax = Constants.MaximumDateTimeValues[Kind];

        return (_minValue == fieldMin && _maxValue == fieldMax) ? "R" : $"R{_minValue}-{_maxValue}";
    }
}
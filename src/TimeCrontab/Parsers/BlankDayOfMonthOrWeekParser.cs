// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using System;

namespace TimeCrontab;

/// <summary>
/// Cron 字段值含 ? 字符解析器
/// </summary>
/// <remarks>
/// <para>只能用在 Day 和 DayOfWeek 两个域使用。它也匹配域的任意值，但实际不会。因为 Day 和 DayOfWeek 会相互影响</para>
/// <para>例如想在每月的 20 日触发调度，不管 20 日到底是星期几，则只能使用如下写法：13 15 20 * ?</para>
/// <para>其中最后一位只能用 ?，而不能使用 *，如果使用 * 表示不管星期几都会触发，实际上并不是这样</para>
/// <para>所以 ? 起着 Day 和 DayOfWeek 互斥性作用</para>
/// <para>仅在 <see cref="CrontabFieldKind.Day"/> 或 <see cref="CrontabFieldKind.DayOfWeek"/> 字段域中使用</para>
/// </remarks>
internal sealed class BlankDayOfMonthOrWeekParser : ICronParser
{
    /// <summary>
    ///  构造函数
    /// </summary>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="TimeCrontabException"></exception>
    public BlankDayOfMonthOrWeekParser(CrontabFieldKind kind)
    {
        // 验证 ? 字符是否在 DayOfWeek 和 Day 字段域中使用
        if (kind != CrontabFieldKind.DayOfWeek && kind != CrontabFieldKind.Day)
        {
            throw new TimeCrontabException("The <?> parser can only be used in the Day-of-Week or Day-of-Month fields.");
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
    /// 将解析器转换成字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return "?";
    }
}
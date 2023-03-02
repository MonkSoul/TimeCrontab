﻿// MIT 许可证
//
// 版权 (c) 2020-present 百小僧, 百签科技（广东）有限公司 和所有贡献者
//
// 特此免费授予任何获得本软件副本和相关文档文件（下称“软件”）的人不受限制地处置该软件的权利，
// 包括不受限制地使用、复制、修改、合并、发布、分发、转授许可和/或出售该软件副本，
// 以及再授权被配发了本软件的人如上的权利，须在下列条件下：
//
// 上述版权声明和本许可声明应包含在该软件的所有副本或实质成分中。
//
// 本软件是“如此”提供的，没有任何形式的明示或暗示的保证，包括但不限于对适销性、特定用途的适用性和不侵权的保证。
// 在任何情况下，作者或版权持有人都不对任何索赔、损害或其他责任负责，无论这些追责来自合同、侵权或其它行为中，
// 还是产生于、源于或有关于本软件以及本软件的使用或其它处置。

using System;

namespace TimeCrontab;

/// <summary>
/// Cron 字段值含 LW 字符解析器
/// </summary>
/// <remarks>
/// <para>表示月中最后一个工作日，即最后一个非周六周末的日期，仅在 <see cref="CrontabFieldKind.Day"/> 字段域中使用</para>
/// </remarks>
internal sealed class LastWeekdayOfMonthParser : ICronParser
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="TimeCrontabException"></exception>
    public LastWeekdayOfMonthParser(CrontabFieldKind kind)
    {
        // 验证 LW 字符是否在 Day 字段域中使用
        if (kind != CrontabFieldKind.Day)
        {
            throw new TimeCrontabException("The <LW> parser can only be used in the Day field.");
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
        /*
         * W：表示有效工作日(周一到周五),只能出现在 Day 域，系统将在离指定日期的最近的有效工作日触发事件
         * 例如：在 Day 使用 5W，如果 5 日是星期六，则将在最近的工作日：星期五，即 4 日触发
         * 如果 5 日是星期天，则在 6 日(周一)触发；如果 5 日在星期一到星期五中的一天，则就在 5 日触发
         * 另外一点，W 的最近寻找不会跨过月份
         */

        // 获取当前时间所在月最后一天
        var specificValue = DateTime.DaysInMonth(datetime.Year, datetime.Month);
        var specificDay = new DateTime(datetime.Year, datetime.Month, specificValue);

        // 最靠近的工作日时间
        DateTime closestWeekday;

        // 处理月中最后一天的不同情况
        switch (specificDay.DayOfWeek)
        {
            // 如果最后一天是周六，则退一天
            case DayOfWeek.Saturday:
                closestWeekday = specificDay.AddDays(-1);

                break;

            // 如果最后一天是周天，则进一天
            case DayOfWeek.Sunday:
                closestWeekday = specificDay.AddDays(1);

                // 如果进一天不在本月，则退到上周五
                if (closestWeekday.Month != specificDay.Month)
                {
                    closestWeekday = specificDay.AddDays(-2);
                }

                break;

            // 处理恰好是工作日情况，直接使用
            default:
                closestWeekday = specificDay;
                break;
        }

        return datetime.Day == closestWeekday.Day;
    }

    /// <summary>
    /// 将解析器转换成字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return "LW";
    }
}
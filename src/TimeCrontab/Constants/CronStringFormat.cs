// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// TimeCrontab is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/TimeCrontab/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TimeCrontab;

/// <summary>
/// Cron 表达式格式化类型
/// </summary>
public enum CronStringFormat
{
    /// <summary>
    /// 默认格式
    /// </summary>
    /// <remarks>书写顺序：分 时 天 月 周</remarks>
    Default = 0,

    /// <summary>
    /// 带年份格式
    /// </summary>
    /// <remarks>书写顺序：分 时 天 月 周 年</remarks>
    WithYears = 1,

    /// <summary>
    /// 带秒格式
    /// </summary>
    /// <remarks>书写顺序：秒 分 时 天 月 周</remarks>
    WithSeconds = 2,

    /// <summary>
    /// 带秒和年格式
    /// </summary>
    /// <remarks>书写顺序：秒 分 时 天 月 周 年</remarks>
    WithSecondsAndYears = 3
}
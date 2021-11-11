// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// TimeCrontab is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/TimeCrontab/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TimeCrontab;

/// <summary>
/// Cron 字段种类
/// </summary>
internal enum CrontabFieldKind
{
    /// <summary>
    /// 秒
    /// </summary>
    Second = 0,

    /// <summary>
    /// 分
    /// </summary>
    Minute = 1,

    /// <summary>
    /// 时
    /// </summary>
    Hour = 2,

    /// <summary>
    /// 天
    /// </summary>
    Day = 3,

    /// <summary>
    /// 月
    /// </summary>
    Month = 4,

    /// <summary>
    /// 星期
    /// </summary>
    DayOfWeek = 5,

    /// <summary>
    /// 年
    /// </summary>
    Year = 6
}

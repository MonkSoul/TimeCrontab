// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// TimeCrontab is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/TimeCrontab/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TimeCrontab
{
    /// <summary>
    /// Cron 表达式抽象类
    /// </summary>
    /// <remarks>主要将 Cron 表达式转换成 OOP 类进行操作</remarks>
    public sealed partial class Crontab
    {
        /// <summary>
        /// 表示每秒的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Secondly = Parse("* * * * * *", CronStringFormat.WithSeconds);

        /// <summary>
        /// 表示每分钟的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Minutely = Parse("* * * * *", CronStringFormat.Default);

        /// <summary>
        /// 表示每小时开始 的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Hourly = Parse("0 * * * *", CronStringFormat.Default);

        /// <summary>
        /// 表示每天（午夜）开始的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Daily = Parse("0 0 * * *", CronStringFormat.Default);

        /// <summary>
        /// 表示每月1号（午夜）开始的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Monthly = Parse("0 0 1 * *", CronStringFormat.Default);

        /// <summary>
        /// 表示每周日（午夜）开始的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Weekly = Parse("0 0 * * 0", CronStringFormat.Default);

        /// <summary>
        /// 表示每年1月1号（午夜）开始的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Yearly = Parse("0 0 1 1 *", CronStringFormat.Default);
    }
}
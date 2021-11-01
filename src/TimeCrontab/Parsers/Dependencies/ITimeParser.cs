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
    /// DateTime 时间解析器依赖接口
    /// </summary>
    /// <remarks>主要用于计算 DateTime 主要组成部分（秒，分，时，年）的下一个取值</remarks>
    internal interface ITimeParser
    {
        /// <summary>
        /// 获取 Cron 字段种类当前值的下一个发生值
        /// </summary>
        /// <param name="currentValue">时间值</param>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        int? Next(int currentValue);

        /// <summary>
        /// 获取 Cron 字段种类字段起始值
        /// </summary>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        int First();
    }
}
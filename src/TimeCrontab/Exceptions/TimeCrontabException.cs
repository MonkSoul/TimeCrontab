// Copyright (c) 2020-present ��Сɮ, Baiqian Co.,Ltd.
// TimeCrontab is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/TimeCrontab/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TimeCrontab;

/// <summary>
/// TimeCrontab ģ���쳣��
/// </summary>
public sealed class TimeCrontabException : Exception
{
    /// <summary>
    /// ���캯��
    /// </summary>
    public TimeCrontabException()
        : base()
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="message">�쳣��Ϣ</param>
    public TimeCrontabException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="message">�쳣��Ϣ</param>
    /// <param name="innerException">�ڲ��쳣</param>
    public TimeCrontabException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
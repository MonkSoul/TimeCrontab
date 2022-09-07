# TimeCrontab

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/TimeCrontab/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/TimeCrontab.svg?cacheSeconds=10800)](https://www.nuget.org/packages/TimeCrontab) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

.NET 全能 Cron 表达式解析库，支持 Cron 所有特性。

![TimeCrontab.drawio](https://gitee.com/dotnetchina/TimeCrontab/raw/master/drawio/TimeCrontab.drawio.png "TimeCrontab.drawio.png")

## 特性

- 支持 Cron 所有特性
- 超高性能
- 易拓展
- 很小，仅 `4KB`
- 无第三方依赖
- 跨平台
- 高质量代码和良好单元测试
- 支持 `.NET Framework 3.5+` 及后续版本

## 安装

- [Package Manager](https://www.nuget.org/packages/TimeCrontab)

```powershell
Install-Package TimeCrontab
```

- [.NET CLI](https://www.nuget.org/packages/TimeCrontab)

```powershell
dotnet add package TimeCrontab
```

## 快速入门

我们在[主页](./samples)上有不少例子，这是让您入门的第一个：

**常规格式**：分 时 天 月 周

```cs
var crontab = Crontab.Parse("* * * * *");
var nextOccurrence = crontab.GetNextOccurrence(DateTime.Now);
```

**支持年份**：分 时 天 月 周 年

```cs
var crontab = Crontab.Parse("* * * * * *", CronStringFormat.WithYears);
var nextOccurrence = crontab.GetNextOccurrence(DateTime.Now);
```

**支持秒数**：秒 分 时 天 月 周

```cs
var crontab = Crontab.Parse("* * * * * *", CronStringFormat.WithSeconds);
var nextOccurrence = crontab.GetNextOccurrence(DateTime.Now);
```

**支持秒和年**：秒 分 时 天 月 周 年

```cs
var crontab = Crontab.Parse("* * * * * * *", CronStringFormat.WithSecondsAndYears);
var nextOccurrence = crontab.GetNextOccurrence(DateTime.Now);
```

**获取休眠差实现定时任务**

```cs
// 阻塞方式
var crontab = Crontab.Parse("* * * * * *", CronStringFormat.WithSeconds);
while(true)
{
    Thread.Sleep(crontab.GetSleepMilliseconds(DateTime.UtcNow));
    Console.WriteLine(DateTime.Now.ToString("G"));
}

// 无阻塞方式
var crontab = Crontab.Parse("* * * * * *", CronStringFormat.WithSeconds);
Task.Factory.StartNew(() =>
{
    while (true)
    {
        Thread.Sleep(crontab.GetSleepMilliseconds(DateTime.UtcNow));
        Console.WriteLine(DateTime.Now.ToString("G"));
    }
}, TaskCreationOptions.LongRunning);
```

**结合 `BackgroundService` 方式**

```cs
using TimeCrontab;

namespace WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly Crontab _crontab;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _crontab = Crontab.Parse("* * * * * *", CronStringFormat.WithSeconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var taskFactory = new TaskFactory(System.Threading.Tasks.TaskScheduler.Current);

            await taskFactory.StartNew(async () =>
            {
                // 你的业务代码写到这里面

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Task.CompletedTask;
            }, stoppingToken);

            await Task.Delay(_crontab.GetSleepMilliseconds(DateTime.UtcNow), stoppingToken);
        }
    }
}
```

**Macro 标识符**

```cs
var secondly = Crontab.Secondly;    // 每秒
var minutely = Crontab.Minutely;    // 每分钟
var hourly = Crontab.Hourly;    // 每小时
var daily = Crontab.Daily;  // 每天 00:00:00
var monthly = Crontab.Monthly;  // 每月 1 号 00:00:00
var weekly = Crontab.Weekly;    // 每周日 00：00：00
var yearly = Crontab.Yearly;    // 每年 1 月 1 号 00:00:00
```

[更多文档](./docs)

## 文档

您可以在[主页](./docs)找到 TimeCrontab 文档。

## 测试

```cs
public class TimeCrontabUnitTests
{
    [Theory]
    [InlineData("* * * * *", "* * * * *", CronStringFormat.Default)]
    [InlineData("0 0 31W * *", "0 0 31W * *", CronStringFormat.Default)]
    [InlineData("0 23 ? * MON-FRI", "0 23 ? * 1-5", CronStringFormat.Default)]
    [InlineData("*/5 * * * *", "*/5 * * * *", CronStringFormat.Default)]
    [InlineData("30 11 * * 1-5", "30 11 * * 1-5", CronStringFormat.Default)]
    [InlineData("23 12 * JAN *", "23 12 * 1 *", CronStringFormat.Default)]
    [InlineData("* * * * MON#3", "* * * * 1#3", CronStringFormat.Default)]
    [InlineData("*/5 * L JAN *", "*/5 * L 1 *", CronStringFormat.Default)]
    [InlineData("0 0 ? 1 MON#1", "0 0 ? 1 1#1", CronStringFormat.Default)]
    [InlineData("0 0 LW * *", "0 0 LW * *", CronStringFormat.Default)]
    [InlineData("0 30 10-13 ? * WED,FRI", "0 30 10-13 ? * 3,5", CronStringFormat.WithSeconds)]
    [InlineData("0 */5 * * * *", "0 */5 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("0 0/1 * * * ?", "0 */1 * * * ?", CronStringFormat.WithSeconds)]
    [InlineData("5-10 30-35 10-12 * * *", "5-10 30-35 10-12 * * *", CronStringFormat.WithSeconds)]
    public void TestParse(string expression, string outputString, CronStringFormat format)
    {
        var output = Crontab.Parse(expression, format).ToString();
        Assert.Equal(outputString, output);
    }

    [Theory]
    [InlineData("* * * * *", "2021-01-01 00:01:00", CronStringFormat.Default)]
    [InlineData("0 0 31W * *", "2021-01-29 00:00:00", CronStringFormat.Default)]
    [InlineData("0 23 ? * MON-FRI", "2021-01-01 23:00:00", CronStringFormat.Default)]
    [InlineData("*/5 * * * *", "2021-01-01 00:05:00", CronStringFormat.Default)]
    [InlineData("30 11 * * 1-5", "2021-01-01 11:30:00", CronStringFormat.Default)]
    [InlineData("23 12 * JAN *", "2021-01-01 12:23:00", CronStringFormat.Default)]
    [InlineData("* * * * MON#3", "2021-01-18 00:00:00", CronStringFormat.Default)]
    [InlineData("*/5 * L JAN *", "2021-01-31 00:00:00", CronStringFormat.Default)]
    [InlineData("0 0 ? 1 MON#1", "2021-01-04 00:00:00", CronStringFormat.Default)]
    [InlineData("0 0 LW * *", "2021-01-29 00:00:00", CronStringFormat.Default)]
    [InlineData("0 30 10-13 ? * WED,FRI", "2021-01-01 10:30:00", CronStringFormat.WithSeconds)]
    [InlineData("0 */5 * * * *", "2021-01-01 00:05:00", CronStringFormat.WithSeconds)]
    [InlineData("0 0/1 * * * ?", "2021-01-01 00:01:00", CronStringFormat.WithSeconds)]
    [InlineData("5-10 30-35 10-12 * * *", "2021-01-01 10:30:05", CronStringFormat.WithSeconds)]
    public void TestGetNextOccurence(string expression, string nextOccurenceString, CronStringFormat format)
    {
        var beginTime = new DateTime(2021, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var nextOccurence = crontab.GetNextOccurrence(beginTime);
        Assert.Equal(nextOccurenceString, nextOccurence.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}
```

## 贡献

该存储库的主要目的是继续发展 TimeCrontab 核心，使其更快、更易于使用。TimeCrontab 的开发在 [Gitee](https://gitee.com/dotnetchina/TimeCrontab) 上公开进行，我们感谢社区贡献错误修复和改进。

## 许可证

TimeCrontab 采用 [MIT](./LICENSE) 开源许可证。

```
MIT License

Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

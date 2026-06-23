# TimeCrontab

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/TimeCrontab/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/TimeCrontab.svg?cacheSeconds=10800)](https://www.nuget.org/packages/TimeCrontab) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

.NET 全能 [Cron](http://crontab.org/) 表达式解析库，支持 [Cron](http://crontab.org/) 所有特性。

![TimeCrontab.drawio](https://gitee.com/dotnetchina/TimeCrontab/raw/net6/drawio/TimeCrontab.drawio.png "TimeCrontab.drawio.png")

## 特性

- 支持 [Cron](http://crontab.org/) 所有特性
- 超高性能
- 易扩展
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

我们在[主页](./samples)上有不少例子，这是让您入门的第一个。

### 解析 Cron 表达式

`TimeCrontab` 支持四种 Cron 格式，通过 `CronStringFormat` 指定：

```cs
// 常规格式：分 时 天 月 周
var crontab = Crontab.Parse("* * * * *");

// 支持年份：分 时 天 月 周 年
var crontab = Crontab.Parse("* * * * * *", CronStringFormat.WithYears);

// 支持秒数：秒 分 时 天 月 周
var crontab = Crontab.Parse("* * * * * *", CronStringFormat.WithSeconds);

// 支持秒和年：秒 分 时 天 月 周 年
var crontab = Crontab.Parse("* * * * * * *", CronStringFormat.WithSecondsAndYears);
```

### 获取发生时间

解析成功后，可通过以下方法获取下一个或上一个发生时间：

#### 单个发生时间

```cs
var next = crontab.GetNextOccurrence(DateTime.Now);           // 下一个发生时间
var previous = crontab.GetPreviousOccurrence(DateTime.Now);   // 上一个发生时间
```

#### 指定时间范围内的所有发生时间

```cs
// 从现在开始，未来 30 分钟内的所有发生时间
var nextOccurrences = crontab.GetNextOccurrences(DateTime.Now, DateTime.Now.AddMinutes(30));

// 从现在开始，过去 30 分钟内的所有发生时间
var previousOccurrences = crontab.GetPreviousOccurrences(DateTime.Now, DateTime.Now.AddMinutes(-30));
```

#### 指定数量的发生时间

```cs
// 接下来的 10 次发生时间
var next10 = crontab.GetNextOccurrences(DateTime.Now, 10);

// 之前的 10 次发生时间
var previous10 = crontab.GetPreviousOccurrences(DateTime.Now, 10);
```

### 实现定时任务

利用获取到的发生时间，可以轻松实现定时任务。

#### 阻塞方式

```cs
var crontab = Crontab.Parse("* * * * *", CronStringFormat.Default);
while(true)
{
    Thread.Sleep(crontab.GetSleepTimeSpan(DateTime.Now));
    Console.WriteLine(DateTime.Now.ToString("G"));
}
```

#### 无阻塞方式

```cs
var crontab = Crontab.Parse("* * * * *", CronStringFormat.Default);
Task.Factory.StartNew(async () =>
{
    while (true)
    {
        await Task.Delay(crontab.GetSleepTimeSpan(DateTime.Now));
        Console.WriteLine(DateTime.Now.ToString("G"));
    }
}, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
```

#### 在 `BackgroundService` 中使用

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
        // 示例：每分钟执行一次（可根据需要修改表达式）
        _crontab = Crontab.Parse("* * * * *", CronStringFormat.Default);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // 计算距离下一次执行需要等待的时间
            var sleepTimeSpan = _crontab.GetSleepTimeSpan(DateTime.Now);
            await Task.Delay(sleepTimeSpan, stoppingToken);

            // 执行业务逻辑（直接在此处编写或调用方法）
            _logger.LogInformation("Worker running at: {time}", DateTime.Now);
        }
    }
}
```

### Macro 标识符

`TimeCrontab` 提供了一些内置的常用宏，方便快速创建常见的 Cron 表达式。

```cs
// 通过宏字符串解析
var secondly = Crontab.Parse("@secondly");    // 每秒
var minutely = Crontab.Parse("@minutely");    // 每分钟
var hourly = Crontab.Parse("@hourly");    // 每小时
var daily = Crontab.Parse("@daily");  // 每天 00:00:00
var monthly = Crontab.Parse("@monthly");  // 每月 1 号 00:00:00
var weekly = Crontab.Parse("@weekly");    // 每周日 00：00：00
var yearly = Crontab.Parse("@yearly");    // 每年 1 月 1 号 00:00:00
var workday = Crontab.Parse("@workday");    // 每周一至周五 00:00:00

// 通过静态属性直接获取
var secondly = Crontab.Secondly;    // 每秒
var minutely = Crontab.Minutely;    // 每分钟
var hourly = Crontab.Hourly;    // 每小时
var daily = Crontab.Daily;  // 每天 00:00:00
var monthly = Crontab.Monthly;  // 每月 1 号 00:00:00
var weekly = Crontab.Weekly;    // 每周日 00：00：00
var yearly = Crontab.Yearly;    // 每年 1 月 1 号 00:00:00
var workday = Crontab.Workday;    // 每周一至周五 00:00:00
```

### Macro At 标识符

允许在宏基础上指定具体的秒、分、时等值，进一步定制触发时间。

```cs
// 每第 3 秒
var crontab = Crontab.SecondlyAt(3);
// 每第 3，5，6 秒
var crontab = Crontab.SecondlyAt(3, 5, 6);

// 每分钟第 3 秒
var crontab = Crontab.MinutelyAt(3);
// 每分钟第 3，5，6 秒
var crontab = Crontab.MinutelyAt(3, 5, 6);

// 每小时第 3 分钟
var crontab = Crontab.HourlyAt(3);
// 每小时第 3，5，6 分钟
var crontab = Crontab.HourlyAt(3, 5, 6);

// 每天第 3 小时正（点）
var crontab = Crontab.DailyAt(3);
// 每天第 3，5，6 小时正（点）
var crontab = Crontab.DailyAt(3, 5, 6);

// 每月第 3 天零点正
var crontab = Crontab.MonthlyAt(3);
// 每月第 3，5，6 天零点正
var crontab = Crontab.MonthlyAt(3, 5, 6);

// 每周星期 3 零点正
var crontab = Crontab.WeeklyAt(3);
var crontab = Crontab.WeeklyAt("WED");  // SUN（星期天），MON，TUE，WED，THU，FRI，SAT
// 每周星期 3，5，6 零点正
var crontab = Crontab.WeeklyAt(3, 5, 6);
var crontab = Crontab.WeeklyAt("WED", "FRI", "SAT");
// 还支持混合
var crontab = Crontab.WeeklyAt(3, "FRI", 6);

// 每年第 3 月 1 日零点正
var crontab = Crontab.YearlyAt(3);
var crontab = Crontab.YearlyAt("MAR");  // JAN（一月），FEB，MAR，APR，MAY，JUN，JUL，AUG，SEP，OCT，NOV，DEC
// 每年第 3，5，6 月 1 日零点正
var crontab = Crontab.YearlyAt(3);
var crontab = Crontab.YearlyAt(3, 5, 6);
var crontab = Crontab.YearlyAt("MAR", "MAY", "JUN");
// 还支持混合
var crontab = Crontab.YearlyAt(3, "MAY", 6);
```

### 支持 `R` 随机时刻

`R` 是一个特殊的 `CRON` 表达式字符，允许您指定随机生成的时刻。例如，`R 0 0 * * ? *` 表示在每天 `00:00` 的随机秒数 (`0-59`) 时刻引发触发器。`R R R 15W * ? *` 表示在每月 `15` 日的随机时刻（秒、分钟、小时）引发。如果 `15` 日为星期六，则会在星期五（即 `14` 日）引发触发器。如果 `15` 日为星期天，则会在星期一（即 `16` 日）引发触发器。[参考文献](https://help.eset.com/protect_admin/13.0/zh-CN/cron_expression.html)

```cs
// 全范围随机（秒 0-59）
var crontab = Crontab.Parse("R 0 0 * * ? *", CronStringFormat.WithSecondsAndYears);
```

`R` 也支持指定随机区间，格式为 `Rmin-max`。这对于将大量定时任务错峰执行非常有用，可以避免同时触发造成系统压力。

```cs
// 秒数在 30~59 之间随机
var crontab = Crontab.Parse("R30-59 * * * * *", CronStringFormat.WithSeconds);

// 分钟在 1~5 之间随机
var crontab = Crontab.Parse("* R1-5 * * * *", CronStringFormat.WithSeconds);

// 小时在 10~20 之间随机
var crontab = Crontab.Parse("* * R10-20 * * *", CronStringFormat.WithSeconds);
```

`R` 还支持带步长的区间随机，格式为 `Rmin-max/step` 或 `R/step`，在给定的区间内按步长筛选候选值后随机选取。

```cs
// 秒数在 0~59 之间，每 5 秒随机一个值（0,5,10,...,55）
var crontab = Crontab.Parse("R0-59/5 * * * * *", CronStringFormat.WithSeconds);

// 分钟在 0~59 之间，每 10 分钟随机一个值（0,10,20,30,40,50）
var crontab = Crontab.Parse("* R0-59/10 * * * *", CronStringFormat.WithSeconds);

// 小时在 0~23 之间，每 6 小时随机一个值（0,6,12,18）
var crontab = Crontab.Parse("* * R0-23/6 * * *", CronStringFormat.WithSeconds);

// 秒数在 1~5 之间，步长为 2（1,3,5）
var crontab = Crontab.Parse("R1-5/2 * * * * *", CronStringFormat.WithSeconds);

// 全范围带步长：秒每 10 秒随机一个值（0,10,20,30,40,50）
var crontab = Crontab.Parse("R/10 * * * * *", CronStringFormat.WithSeconds);
```

[更多文档](https://furion.net/docs/cron)

## 文档

您可以在[主页](https://furion.net/docs/cron)找到 TimeCrontab 文档。

## 测试

```cs
public class TimeCrontabUnitTests
{
    private readonly ITestOutputHelper _testOutput;
    public TimeCrontabUnitTests(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
    }

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
    [InlineData("20/10 * * * * ?", "20/10 * * * * ?", CronStringFormat.WithSeconds)]
    public void TestParse(string expression, string outputString, CronStringFormat format)
    {
        var output = Crontab.Parse(expression, format).ToString();
        Assert.Equal(outputString, output);
    }

    [Theory]
    [InlineData("* * * * *", "2022-01-01 00:01:00", CronStringFormat.Default)]
    [InlineData("0 0 31W * *", "2022-01-31 00:00:00", CronStringFormat.Default)]
    [InlineData("0 23 ? * MON-FRI", "2022-01-03 23:00:00", CronStringFormat.Default)]
    [InlineData("*/5 * * * *", "2022-01-01 00:05:00", CronStringFormat.Default)]
    [InlineData("30 11 * * 1-5", "2022-01-03 11:30:00", CronStringFormat.Default)]
    [InlineData("23 12 * JAN *", "2022-01-01 12:23:00", CronStringFormat.Default)]
    [InlineData("* * * * MON#3", "2022-01-17 00:00:00", CronStringFormat.Default)]
    [InlineData("*/5 * L JAN *", "2022-01-31 00:00:00", CronStringFormat.Default)]
    [InlineData("0 0 ? 1 MON#1", "2022-01-03 00:00:00", CronStringFormat.Default)]
    [InlineData("0 0 LW * *", "2022-01-31 00:00:00", CronStringFormat.Default)]
    [InlineData("0 30 10-13 ? * WED,FRI", "2022-01-05 10:30:00", CronStringFormat.WithSeconds)]
    [InlineData("0 */5 * * * *", "2022-01-01 00:05:00", CronStringFormat.WithSeconds)]
    [InlineData("0 0/1 * * * ?", "2022-01-01 00:01:00", CronStringFormat.WithSeconds)]
    [InlineData("5-10 30-35 10-12 * * *", "2022-01-01 10:30:05", CronStringFormat.WithSeconds)]
    [InlineData("20/10 * * * * ?", "2022-01-01 00:00:20", CronStringFormat.WithSeconds)]
    [InlineData("20/30 * * * * ?", "2022-01-01 00:00:20", CronStringFormat.WithSeconds)]
    public void TestGetNextOccurence(string expression, string nextOccurenceString, CronStringFormat format)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var nextOccurence = crontab.GetNextOccurrence(beginTime);
        Assert.Equal(nextOccurenceString, nextOccurence.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    [Theory]
    [InlineData("* * * * *", "2021-12-31 23:59:00", CronStringFormat.Default)]
    [InlineData("0 0 31W * *", "2021-12-31 00:00:00", CronStringFormat.Default)]
    [InlineData("0 23 ? * MON-FRI", "2021-12-31 23:00:00", CronStringFormat.Default)]
    [InlineData("*/5 * * * *", "2021-12-31 23:55:00", CronStringFormat.Default)]
    [InlineData("30 11 * * 1-5", "2021-12-31 11:30:00", CronStringFormat.Default)]
    [InlineData("23 12 * JAN *", "2021-01-31 12:23:00", CronStringFormat.Default)]
    [InlineData("* * * * MON#3", "2021-12-20 23:59:00", CronStringFormat.Default)]
    [InlineData("*/5 * L JAN *", "2021-01-31 23:55:00", CronStringFormat.Default)]
    [InlineData("0 0 ? 1 MON#1", "2021-01-04 00:00:00", CronStringFormat.Default)]
    [InlineData("0 0 LW * *", "2021-12-31 00:00:00", CronStringFormat.Default)]
    [InlineData("0 30 10-13 ? * WED,FRI", "2021-12-31 13:30:00", CronStringFormat.WithSeconds)]
    [InlineData("0 */5 * * * *", "2021-12-31 23:55:00", CronStringFormat.WithSeconds)]
    [InlineData("0 0/1 * * * ?", "2021-12-31 23:59:00", CronStringFormat.WithSeconds)]
    [InlineData("5-10 30-35 10-12 * * *", "2021-12-31 12:35:10", CronStringFormat.WithSeconds)]
    [InlineData("20/10 * * * * ?", "2021-12-31 23:59:50", CronStringFormat.WithSeconds)]
    [InlineData("20/30 * * * * ?", "2021-12-31 23:59:50", CronStringFormat.WithSeconds)]
    public void GetPreviousOccurrence(string expression, string previousOccurenceString, CronStringFormat format)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var previous = crontab.GetPreviousOccurrence(beginTime);
        Assert.Equal(previousOccurenceString, previous.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    [Theory]
    [InlineData("R 0 0 * * ? *", "R 0 0 * * ? *", CronStringFormat.WithSecondsAndYears)]
    [InlineData("R R R 15W * ? *", "R R R 15W * ? *", CronStringFormat.WithSecondsAndYears)]
    [InlineData("R * * * * *", "R * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R * * * *", "* R * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R * * *", "* * R * * *", CronStringFormat.WithSeconds)]
    public void TestParse_Random(string expression, string outputString, CronStringFormat format)
    {
        var output = Crontab.Parse(expression, format).ToString();
        Assert.Equal(outputString, output);
    }

    [Theory]
    [InlineData("R30-59 * * * * *", "R30-59 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R1-5 * * * *", "* R1-5 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R5-10 * * *", "* * R5-10 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R0-59 * * * * *", "R * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R10-10 * * * * *", "R10-10 * * * * *", CronStringFormat.WithSeconds)]
    public void TestParse_RandomRange(string expression, string outputString, CronStringFormat format)
    {
        var output = Crontab.Parse(expression, format).ToString();
        Assert.Equal(outputString, output);
    }

    [Theory]
    [InlineData("R 0 0 * * ? *", CronStringFormat.WithSecondsAndYears, 0, 59)]
    [InlineData("* R 0 * * ? *", CronStringFormat.WithSecondsAndYears, 0, 59)]
    [InlineData("* * R * * ? *", CronStringFormat.WithSecondsAndYears, 0, 23)]
    public void TestNextOccurrence_RandomValueInRange(string expression, CronStringFormat format, int min, int max)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var next = crontab.GetNextOccurrence(beginTime);

        int actualValue = GetRandomFieldValue(next, expression);
        Assert.InRange(actualValue, min, max);
        _testOutput.WriteLine($"Random value: {actualValue}");
    }

    [Theory]
    [InlineData("R30-59 * * * * *", CronStringFormat.WithSeconds, 30, 59)]
    [InlineData("* R10-20 * * * *", CronStringFormat.WithSeconds, 10, 20)]
    [InlineData("* * R5-10 * * *", CronStringFormat.WithSeconds, 5, 10)]
    [InlineData("R10-10 * * * * *", CronStringFormat.WithSeconds, 10, 10)]
    public void TestNextOccurrence_RandomRange(string expression, CronStringFormat format, int min, int max)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var next = crontab.GetNextOccurrence(beginTime);

        int actualValue = GetRandomFieldValue(next, expression);
        Assert.InRange(actualValue, min, max);
        _testOutput.WriteLine($"Random range value: {actualValue}");
    }

    [Fact]
    public void TestMultiRandomFieldNextOccurrence()
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse("R R R 15W * ? *", CronStringFormat.WithSecondsAndYears);
        var next = crontab.GetNextOccurrence(beginTime);

        Assert.InRange(next.Second, 0, 59);
        Assert.InRange(next.Minute, 0, 59);
        Assert.InRange(next.Hour, 0, 23);

        Assert.Equal(14, next.Day);
        Assert.Equal(1, next.Month);
        Assert.Equal(2022, next.Year);

        _testOutput.WriteLine($"Random multi-field: {next:yyyy-MM-dd HH:mm:ss}");
    }

    [Theory]
    [InlineData("R,30 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R,5 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R,10 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R30-59,20 * * * * *", CronStringFormat.WithSeconds)]
    public void TestRandomCombinedWithOtherValuesThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Theory]
    [InlineData("R60-30 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R-1-5 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R0-60 * * *", CronStringFormat.WithSeconds)]
    [InlineData("Rabc-def * * * * *", CronStringFormat.WithSeconds)]
    public void TestInvalidRandomRangeThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Theory]
    [InlineData("R0-59/5 * * * * *", "R0-59/5 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R0-59/10 * * * *", "* R0-59/10 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R0-23/2 * * *", "* * R0-23/2 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R1-5/2 * * * * *", "R1-5/2 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R1-5/1 * * * * *", "R1-5/1 * * * * *", CronStringFormat.WithSeconds)]
    public void TestParse_RandomStep(string expression, string outputString, CronStringFormat format)
    {
        var output = Crontab.Parse(expression, format).ToString();
        Assert.Equal(outputString, output);
    }

    [Theory]
    [InlineData("R0-59/10 * * * * *", CronStringFormat.WithSeconds, new int[] { 0, 10, 20, 30, 40, 50 })]
    [InlineData("* R0-59/15 * * * *", CronStringFormat.WithSeconds, new int[] { 0, 15, 30, 45 })]
    [InlineData("* * R0-23/6 * * *", CronStringFormat.WithSeconds, new int[] { 0, 6, 12, 18 })]
    [InlineData("R1-5/2 * * * * *", CronStringFormat.WithSeconds, new int[] { 1, 3, 5 })]
    public void TestNextOccurrence_RandomStep(string expression, CronStringFormat format, int[] validValues)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var next = crontab.GetNextOccurrence(beginTime);

        int actualValue = GetRandomFieldValue(next, expression);
        Assert.Contains(actualValue, validValues);
        _testOutput.WriteLine($"Random step value: {actualValue}");
    }

    [Theory]
    [InlineData("R0-59/0 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R0-59/-5 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R0-59/abc * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R5-1/2 * * * * *", CronStringFormat.WithSeconds)]
    public void TestInvalidRandomStepThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Theory]
    [InlineData("* * * * *", CronStringFormat.Default, 5)]
    [InlineData("*/5 * * * *", CronStringFormat.Default, 10)]
    [InlineData("0 0/1 * * * ?", CronStringFormat.WithSeconds, 3)]
    public void TestGetNextOccurrencesCount(string expression, CronStringFormat format, int count)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var occurrences = crontab.GetNextOccurrences(beginTime, count).ToList();

        Assert.Equal(count, occurrences.Count);

        for (int i = 0; i < occurrences.Count - 1; i++)
        {
            Assert.True(occurrences[i] < occurrences[i + 1]);
        }

        Assert.All(occurrences, dt => Assert.True(dt > beginTime));
    }

    [Theory]
    [InlineData("* * * * *", CronStringFormat.Default, 5)]
    [InlineData("*/5 * * * *", CronStringFormat.Default, 10)]
    [InlineData("0 0/1 * * * ?", CronStringFormat.WithSeconds, 3)]
    public void TestGetPreviousOccurrencesCount(string expression, CronStringFormat format, int count)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var occurrences = crontab.GetPreviousOccurrences(beginTime, count).ToList();

        Assert.Equal(count, occurrences.Count);

        for (int i = 0; i < occurrences.Count - 1; i++)
        {
            Assert.True(occurrences[i] > occurrences[i + 1]);
        }

        Assert.All(occurrences, dt => Assert.True(dt < beginTime));
    }

    [Fact]
    public void TestGetNextOccurrencesCountWithRandomExpression()
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse("R30-59 * * * * *", CronStringFormat.WithSeconds);
        var occurrences = crontab.GetNextOccurrences(beginTime, 5).ToList();

        Assert.Equal(5, occurrences.Count);
        Assert.All(occurrences, dt => Assert.InRange(dt.Second, 30, 59));

        for (int i = 0; i < 4; i++)
        {
            Assert.True(occurrences[i] < occurrences[i + 1]);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TestGetNextOccurrencesCountInvalidCount(int invalidCount)
    {
        var crontab = Crontab.Parse("* * * * *");
        Assert.Throws<ArgumentOutOfRangeException>(() => crontab.GetNextOccurrences(DateTime.Now, invalidCount).ToList());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TestGetPreviousOccurrencesCountInvalidCount(int invalidCount)
    {
        var crontab = Crontab.Parse("* * * * *");
        Assert.Throws<ArgumentOutOfRangeException>(() => crontab.GetPreviousOccurrences(DateTime.Now, invalidCount).ToList());
    }

    private static int GetRandomFieldValue(DateTime dateTime, string expression)
    {
        var parts = expression.Split(' ');

        for (int i = 0; i < 3; i++)
        {
            if (parts[i].StartsWith("R"))
            {
                return i switch
                {
                    0 => dateTime.Second,
                    1 => dateTime.Minute,
                    2 => dateTime.Hour,
                    _ => throw new InvalidOperationException()
                };
            }
        }

        throw new ArgumentException("No random field found in expression");
    }
}
```

## 贡献

该存储库的主要目的是继续发展 TimeCrontab 核心，使其更快、更易于使用。TimeCrontab 的开发在 [Gitee](https://gitee.com/dotnetchina/TimeCrontab) 上公开进行，我们感谢社区贡献错误修复和改进。

## 许可证

TimeCrontab 采用 [MIT](./LICENSE) 开源许可证。

[![](./assets/baiqian.svg)](https://baiqian.com)
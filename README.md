# TimeCrontab

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/TimeCrontab/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/TimeCrontab.svg?cacheSeconds=10800)](https://www.nuget.org/packages/TimeCrontab) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

A comprehensive .NET [Cron](http://crontab.org/) expression parsing library that supports all [Cron](http://crontab.org/) features.

![TimeCrontab.drawio](https://gitee.com/dotnetchina/TimeCrontab/raw/net6/drawio/TimeCrontab.drawio.png "TimeCrontab.drawio.png")

## Features

- Supports all [Cron](http://crontab.org/) features
- High performance
- Easy to extend
- Very small, only `4KB`
- No third-party dependencies
- Cross-platform
- High-quality code and good unit tests
- Supports `.NET Framework 3.5+` and later versions

## Installation

- [Package Manager](https://www.nuget.org/packages/TimeCrontab)

```powershell
Install-Package TimeCrontab
```

- [.NET CLI](https://www.nuget.org/packages/TimeCrontab)

```powershell
dotnet add package TimeCrontab
```

## Quick Start

We have many examples on the [home page](./samples). Here is the first one to get you started.

### Parsing Cron Expressions

`TimeCrontab` supports four Cron formats, specified via `CronStringFormat`:

```cs
// Default format: Minute Hour Day Month DayOfWeek
var crontab = Crontab.Parse("* * * * *");

// With year: Minute Hour Day Month DayOfWeek Year
var crontab = Crontab.Parse("* * * * * *", CronStringFormat.WithYears);

// With seconds: Second Minute Hour Day Month DayOfWeek
var crontab = Crontab.Parse("* * * * * *", CronStringFormat.WithSeconds);

// With seconds and year: Second Minute Hour Day Month DayOfWeek Year
var crontab = Crontab.Parse("* * * * * * *", CronStringFormat.WithSecondsAndYears);
```

### Getting Occurrence Times

After parsing, you can get the next or previous occurrence time with the following methods:

#### Single Occurrence

```cs
var next = crontab.GetNextOccurrence(DateTime.Now);           // Next occurrence time
var previous = crontab.GetPreviousOccurrence(DateTime.Now);   // Previous occurrence time
```

#### All Occurrences within a Time Range

```cs
// All occurrences in the next 30 minutes
var nextOccurrences = crontab.GetNextOccurrences(DateTime.Now, DateTime.Now.AddMinutes(30));

// All occurrences in the past 30 minutes
var previousOccurrences = crontab.GetPreviousOccurrences(DateTime.Now, DateTime.Now.AddMinutes(-30));
```

#### Specified Number of Occurrences

```cs
// Next 10 occurrences
var next10 = crontab.GetNextOccurrences(DateTime.Now, 10);

// Previous 10 occurrences
var previous10 = crontab.GetPreviousOccurrences(DateTime.Now, 10);
```

### Implementing Simple Scheduled Tasks

You can easily implement scheduled tasks using the obtained occurrence times.

#### Blocking Approach

```cs
var crontab = Crontab.Parse("* * * * *", CronStringFormat.Default);
while(true)
{
    Thread.Sleep(crontab.GetSleepTimeSpan(DateTime.Now));
    Console.WriteLine(DateTime.Now.ToString("G"));
}
```

#### Non-blocking Approach

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

#### Using `BackgroundService`

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
        // Example: every minute (adjust the expression as needed)
        _crontab = Crontab.Parse("* * * * *", CronStringFormat.Default);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Calculate the time to sleep until the next occurrence
            var sleepTimeSpan = _crontab.GetSleepTimeSpan(DateTime.Now);
            await Task.Delay(sleepTimeSpan, stoppingToken);

            // Execute your business logic here
            _logger.LogInformation("Worker running at: {time}", DateTime.Now);
        }
    }
}
```

### Macro Identifiers

`TimeCrontab` provides built-in macros for quickly creating common Cron expressions.

```cs
// Parse via macro strings
var secondly = Crontab.Parse("@secondly");    // Every second
var minutely = Crontab.Parse("@minutely");    // Every minute
var hourly = Crontab.Parse("@hourly");    // Every hour
var daily = Crontab.Parse("@daily");  // Every day at 00:00:00
var monthly = Crontab.Parse("@monthly");  // Every 1st day of month at 00:00:00
var weekly = Crontab.Parse("@weekly");    // Every Sunday at 00:00:00
var yearly = Crontab.Parse("@yearly");    // Every 1st day of year at 00:00:00
var weekdays = Crontab.Parse("@weekdays");    // Every Monday to Friday at 00:00:00
var weekends = Crontab.Parse("@weekends");    // Every Saturday and Sunday at 00:00:00

// Static properties
var secondly = Crontab.Secondly;    // Every second
var minutely = Crontab.Minutely;    // Every minute
var hourly = Crontab.Hourly;    // Every hour
var daily = Crontab.Daily;  // Every day at 00:00:00
var monthly = Crontab.Monthly;  // Every 1st day of month at 00:00:00
var weekly = Crontab.Weekly;    // Every Sunday at 00:00:00
var yearly = Crontab.Yearly;    // Every 1st day of year at 00:00:00
var weekdays = Crontab.Weekdays;    // Every Monday to Friday at 00:00:00
var weekends = Crontab.Weekends;    // Every Saturday and Sunday at 00:00:00
```

### Macro At Identifiers

Allows specifying exact second, minute, hour etc. on top of macros.

```cs
// Every 3rd second
var crontab = Crontab.SecondlyAt(3);
// Every 3,5,6 seconds
var crontab = Crontab.SecondlyAt(3, 5, 6);

// Every minute at the 3rd second
var crontab = Crontab.MinutelyAt(3);
// Every minute at the 3rd,5th,6th second
var crontab = Crontab.MinutelyAt(3, 5, 6);

// Every hour at the 3rd minute
var crontab = Crontab.HourlyAt(3);
// Every hour at the 3rd,5th,6th minute
var crontab = Crontab.HourlyAt(3, 5, 6);

// Every day at the 3rd hour
var crontab = Crontab.DailyAt(3);
// Every day at the 3rd,5th,6th hour
var crontab = Crontab.DailyAt(3, 5, 6);

// Every month on the 3rd day at midnight
var crontab = Crontab.MonthlyAt(3);
// Every month on the 3rd,5th,6th day at midnight
var crontab = Crontab.MonthlyAt(3, 5, 6);

// Every week on Wednesday at midnight
var crontab = Crontab.WeeklyAt(3);
var crontab = Crontab.WeeklyAt("WED");  // SUN, MON, TUE, WED, THU, FRI, SAT
// Every week on Wednesday, Friday, Saturday at midnight
var crontab = Crontab.WeeklyAt(3, 5, 6);
var crontab = Crontab.WeeklyAt("WED", "FRI", "SAT");
// Mixed
var crontab = Crontab.WeeklyAt(3, "FRI", 6);

// Every year in March at midnight
var crontab = Crontab.YearlyAt(3);
var crontab = Crontab.YearlyAt("MAR");  // JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC
// Every year in March, May, June at midnight
var crontab = Crontab.YearlyAt(3);
var crontab = Crontab.YearlyAt(3, 5, 6);
var crontab = Crontab.YearlyAt("MAR", "MAY", "JUN");
// Mixed
var crontab = Crontab.YearlyAt(3, "MAY", 6);
```

### Supporting `R` Random Moment

`R` is a special `CRON` expression character that allows you to specify a randomly generated moment. For example, `R 0 0 * * ? *` means triggering at a random second (0‑59) every day at 00:00. `R R R 15W * ? *` means triggering at a random moment (seconds, minutes, hours) on the 15th day of each month. If the 15th is a Saturday, it fires on Friday (the 14th); if it is a Sunday, it fires on Monday (the 16th). [Reference](https://help.eset.com/protect_admin/13.0/zh-CN/cron_expression.html)

```cs
// Full-range random (seconds 0-59)
var crontab = Crontab.Parse("R 0 0 * * ? *", CronStringFormat.WithSecondsAndYears);
```

`R` also supports specifying a random interval in the format `Rmin-max`. This is very useful for staggering a large number of scheduled tasks to avoid system pressure caused by simultaneous triggering.

```cs
// Random seconds between 30~59
var crontab = Crontab.Parse("R30-59 * * * * *", CronStringFormat.WithSeconds);

// Random minutes between 1~5
var crontab = Crontab.Parse("* R1-5 * * * *", CronStringFormat.WithSeconds);

// Random hours between 10~20
var crontab = Crontab.Parse("* * R10-20 * * *", CronStringFormat.WithSeconds);
```

`R` also supports interval random with a step, in the formats `Rmin-max/step` or `R/step`. Candidates are filtered by the step within the given range, and one value is randomly selected.

```cs
// Every 5 seconds between 0~59 (0,5,10,...,55)
var crontab = Crontab.Parse("R0-59/5 * * * * *", CronStringFormat.WithSeconds);

// Every 10 minutes between 0~59 (0,10,20,30,40,50)
var crontab = Crontab.Parse("* R0-59/10 * * * *", CronStringFormat.WithSeconds);

// Every 6 hours between 0~23 (0,6,12,18)
var crontab = Crontab.Parse("* * R0-23/6 * * *", CronStringFormat.WithSeconds);

// Step 2 between 1~5 (1,3,5)
var crontab = Crontab.Parse("R1-5/2 * * * * *", CronStringFormat.WithSeconds);

// Full-range step: random every 10 seconds (0,10,20,30,40,50)
var crontab = Crontab.Parse("R/10 * * * * *", CronStringFormat.WithSeconds);
```

`R` also supports discrete value random, in the format `Rvalue1,value2,value3`, which randomly selects from the specified values.

```cs
// Random seconds among 1,5,10,12
var crontab = Crontab.Parse("R1,5,10,12 * * * * *", CronStringFormat.WithSeconds);

// Random minutes among 0,15,30,45
var crontab = Crontab.Parse("* R0,15,30,45 * * * *", CronStringFormat.WithSeconds);

// Random hours among 8,12,18
var crontab = Crontab.Parse("* * R8,12,18 * * *", CronStringFormat.WithSeconds);
```

[More Documentation](https://furion.net/docs/cron)

## Documentation

You can find the TimeCrontab documentation on the [home page](https://furion.net/docs/cron).

## Tests

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
    [InlineData("R1,5,10,12 * * * * *", "R1,5,10,12 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R0,15,30,45 * * * *", "* R0,15,30,45 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R8,12,18 * * *", "* * R8,12,18 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R5,10,15 * * * * *", "R5,10,15 * * * * *", CronStringFormat.WithSeconds)]
    public void TestParse_RandomDiscrete(string expression, string outputString, CronStringFormat format)
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

    [Theory]
    [InlineData("R1,5,10,12 * * * * *", CronStringFormat.WithSeconds, new int[] { 1, 5, 10, 12 })]
    [InlineData("* R0,15,30,45 * * * *", CronStringFormat.WithSeconds, new int[] { 0, 15, 30, 45 })]
    [InlineData("* * R8,12,18 * * *", CronStringFormat.WithSeconds, new int[] { 8, 12, 18 })]
    [InlineData("R10,10,10 * * * * *", CronStringFormat.WithSeconds, new int[] { 10 })]
    public void TestNextOccurrence_RandomDiscrete(string expression, CronStringFormat format, int[] validValues)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var next = crontab.GetNextOccurrence(beginTime);

        int actualValue = GetRandomFieldValue(next, expression);
        Assert.Contains(actualValue, validValues);
        _testOutput.WriteLine($"Random discrete value: {actualValue}");
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
    [InlineData("R61 * * * * *", CronStringFormat.WithSeconds)] 
    [InlineData("R1,abc,10 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R25 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R, * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R1,,5 * * * * *", CronStringFormat.WithSeconds)]
    public void TestInvalidRandomDiscreteThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Theory]
    [InlineData("R1,5,10,12,30 * * * * *", CronStringFormat.WithSeconds)]
    public void TestRandomDiscreteCombinedWithOtherValuesNotThrow(string expression, CronStringFormat format)
    {
        var crontab = Crontab.Parse(expression, format);
        Assert.NotNull(crontab);
    }

    [Theory]
    [InlineData("* R0,60 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R0,24 * * *", CronStringFormat.WithSeconds)]
    public void TestDiscreteValueOutOfRangeThrows(string expression, CronStringFormat format)
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

## Contributing

The primary goal of this repository is to continue developing TimeCrontab, making it faster and easier to use. Development of TimeCrontab is publicly conducted on [Gitee](https://gitee.com/dotnetchina/TimeCrontab), and we appreciate community contributions for bug fixes and improvements.

## License

TimeCrontab is released under the [MIT](./LICENSE) open source license.

[![](./assets/baiqian.svg)](https://baiqian.com)
using System;
using Xunit;

namespace TimeCrontab.UnitTests;

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

    /// <summary>
    /// 测试全范围 R 的解析与字符串输出
    /// </summary>
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

    /// <summary>
    /// 测试区间随机 Rmin-max 的解析与字符串输出
    /// </summary>
    [Theory]
    [InlineData("R30-59 * * * * *", "R30-59 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R1-5 * * * *", "* R1-5 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R5-10 * * *", "* * R5-10 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R0-59 * * * * *", "R * * * * *", CronStringFormat.WithSeconds)] // 全范围等价于 R
    [InlineData("R10-10 * * * * *", "R10-10 * * * * *", CronStringFormat.WithSeconds)] // 固定值区间
    public void TestParse_RandomRange(string expression, string outputString, CronStringFormat format)
    {
        var output = Crontab.Parse(expression, format).ToString();
        Assert.Equal(outputString, output);
    }

    /// <summary>
    /// 验证全范围 R 下一次发生时间的随机值在合法范围内
    /// </summary>
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

    /// <summary>
    /// 验证区间随机 Rmin-max 下一次发生时间的随机值在指定区间内
    /// </summary>
    [Theory]
    [InlineData("R30-59 * * * * *", CronStringFormat.WithSeconds, 30, 59)]
    [InlineData("* R10-20 * * * *", CronStringFormat.WithSeconds, 10, 20)]
    [InlineData("* * R5-10 * * *", CronStringFormat.WithSeconds, 5, 10)]
    [InlineData("R10-10 * * * * *", CronStringFormat.WithSeconds, 10, 10)] // 固定值
    public void TestNextOccurrence_RandomRange(string expression, CronStringFormat format, int min, int max)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var next = crontab.GetNextOccurrence(beginTime);

        int actualValue = GetRandomFieldValue(next, expression);
        Assert.InRange(actualValue, min, max);
        _testOutput.WriteLine($"Random range value: {actualValue}");
    }

    /// <summary>
    /// 测试多个随机字段组合（R R R 15W * ? *）
    /// 确保秒、分、时三个字段均为随机且合法
    /// </summary>
    [Fact]
    public void TestMultiRandomFieldNextOccurrence()
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse("R R R 15W * ? *", CronStringFormat.WithSecondsAndYears);
        var next = crontab.GetNextOccurrence(beginTime);

        // 秒 0-59
        Assert.InRange(next.Second, 0, 59);
        // 分 0-59
        Assert.InRange(next.Minute, 0, 59);
        // 时 0-23
        Assert.InRange(next.Hour, 0, 23);

        // 因为 2022-01-15 是周六，15W 会调整到 14 号（周五）
        Assert.Equal(14, next.Day);
        Assert.Equal(1, next.Month);
        Assert.Equal(2022, next.Year);

        _testOutput.WriteLine($"Random multi-field: {next:yyyy-MM-dd HH:mm:ss}");
    }

    /// <summary>
    /// 测试 R 与其他值混用应抛出异常
    /// </summary>
    [Theory]
    [InlineData("R,30 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R,5 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R,10 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R30-59,20 * * * * *", CronStringFormat.WithSeconds)]
    public void TestRandomCombinedWithOtherValuesThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    /// <summary>
    /// 测试无效的 R 区间
    /// </summary>
    [Theory]
    [InlineData("R60-30 * * * * *", CronStringFormat.WithSeconds)] // min > max
    [InlineData("* R-1-5 * * * *", CronStringFormat.WithSeconds)] // 负数
    [InlineData("* * R0-60 * * *", CronStringFormat.WithSeconds)] // 超出小时最大值
    [InlineData("Rabc-def * * * * *", CronStringFormat.WithSeconds)] // 非数字
    public void TestInvalidRandomRangeThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    /// <summary>
    /// 测试带步长的随机区间 Rmin-max/step
    /// </summary>
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

    /// <summary>
    /// 验证带步长随机区间下一次发生值在候选集中
    /// </summary>
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

    /// <summary>
    /// 测试无效步长抛出异常
    /// </summary>
    [Theory]
    [InlineData("R0-59/0 * * * * *", CronStringFormat.WithSeconds)]  // 步长为0
    [InlineData("R0-59/-5 * * * * *", CronStringFormat.WithSeconds)] // 负数步长
    [InlineData("R0-59/abc * * * * *", CronStringFormat.WithSeconds)] // 非数字步长
    [InlineData("R5-1/2 * * * * *", CronStringFormat.WithSeconds)]   // min>max
    public void TestInvalidRandomStepThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    /// <summary>
    /// 根据表达式判断 R 所在字段，并提取 DateTime 对应值
    /// </summary>
    private static int GetRandomFieldValue(DateTime dateTime, string expression)
    {
        var parts = expression.Split(' ');

        // R 一定在表达式的前三个字段之一（秒、分、时）
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
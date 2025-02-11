using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

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
    public void GetPreviousOccurrence(string expression, string nextOccurenceString, CronStringFormat format)
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse(expression, format);
        var previous = crontab.GetPreviousOccurrence(beginTime);
        Assert.Equal(nextOccurenceString, previous.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    [Fact]
    public void TestRandownInSecondOrMinuteOrHour()
    {
        var beginTime = new DateTime(2022, 1, 1, 0, 0, 0);
        var crontab = Crontab.Parse("R 0 0 * * ? *", CronStringFormat.WithSecondsAndYears);
        Assert.Equal("R 0 0 * * ? *", crontab.ToString());
        var nextOccurence = crontab.GetNextOccurrence(beginTime);
        Assert.True(nextOccurence.Second >= 0 && nextOccurence.Second <= 59);
        _testOutput.WriteLine(nextOccurence.Second.ToString());

        Assert.Throws<TimeCrontabException>(() => Crontab.Parse("* 0 0 R * ? *", CronStringFormat.WithSecondsAndYears));
    }
}
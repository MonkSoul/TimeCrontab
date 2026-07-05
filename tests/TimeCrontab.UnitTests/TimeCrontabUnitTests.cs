using System;
using System.Linq;
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
    [InlineData("R(30-59) * * * * *", "R(30-59) * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R(1-5) * * * *", "* R(1-5) * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R(5-10) * * *", "* * R(5-10) * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(0-59) * * * * *", "R * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(10-10) * * * * *", "R(10-10) * * * * *", CronStringFormat.WithSeconds)]
    public void TestParse_RandomRange(string expression, string outputString, CronStringFormat format)
    {
        var output = Crontab.Parse(expression, format).ToString();
        Assert.Equal(outputString, output);
    }

    [Theory]
    [InlineData("R(1,5,10,12) * * * * *", "R(1,5,10,12) * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R(0,15,30,45) * * * *", "* R(0,15,30,45) * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R(8,12,18) * * *", "* * R(8,12,18) * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(5,10,15) * * * * *", "R(5,10,15) * * * * *", CronStringFormat.WithSeconds)]
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
    [InlineData("R(30-59) * * * * *", CronStringFormat.WithSeconds, 30, 59)]
    [InlineData("* R(10-20) * * * *", CronStringFormat.WithSeconds, 10, 20)]
    [InlineData("* * R(5-10) * * *", CronStringFormat.WithSeconds, 5, 10)]
    [InlineData("R(10-10) * * * * *", CronStringFormat.WithSeconds, 10, 10)]
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
    [InlineData("R(1,5,10,12) * * * * *", CronStringFormat.WithSeconds, new int[] { 1, 5, 10, 12 })]
    [InlineData("* R(0,15,30,45) * * * *", CronStringFormat.WithSeconds, new int[] { 0, 15, 30, 45 })]
    [InlineData("* * R(8,12,18) * * *", CronStringFormat.WithSeconds, new int[] { 8, 12, 18 })]
    [InlineData("R(10,10,10) * * * * *", CronStringFormat.WithSeconds, new int[] { 10 })]
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
    }

    [Theory]
    [InlineData("R,30 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R,5 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R,10 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(30-59),20 * * * * *", CronStringFormat.WithSeconds)]
    public void TestRandomCombinedWithOtherValuesThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Theory]
    [InlineData("R(60-30) * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R(-1-5) * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R(0-60) * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(abc-def) * * * * *", CronStringFormat.WithSeconds)]
    public void TestInvalidRandomRangeThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Theory]
    [InlineData("R(61) * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(1,abc,10) * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R(25) * * *", CronStringFormat.WithSeconds)]
    [InlineData("R() * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(1,,5) * * * * *", CronStringFormat.WithSeconds)]
    public void TestInvalidRandomDiscreteThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Theory]
    [InlineData("R(1,5,10,12,30) * * * * *", CronStringFormat.WithSeconds)]
    public void TestRandomDiscreteCombinedWithOtherValuesNotThrow(string expression, CronStringFormat format)
    {
        var crontab = Crontab.Parse(expression, format);
        Assert.NotNull(crontab);
    }

    [Theory]
    [InlineData("* R(0,60) * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R(0,24) * * *", CronStringFormat.WithSeconds)]
    public void TestDiscreteValueOutOfRangeThrows(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Theory]
    [InlineData("R(0-59)/5 * * * * *", "R(0-59)/5 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R(0-59)/10 * * * *", "* R(0-59)/10 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R(0-23)/2 * * *", "* * R(0-23)/2 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(1-5)/2 * * * * *", "R(1-5)/2 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(1-5)/1 * * * * *", "R(1-5)/1 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R/5 * * * * *", "R/5 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* R/10 * * * *", "* R/10 * * * *", CronStringFormat.WithSeconds)]
    [InlineData("* * R/2 * * *", "* * R/2 * * *", CronStringFormat.WithSeconds)]
    public void TestParse_RandomStep(string expression, string outputString, CronStringFormat format)
    {
        var output = Crontab.Parse(expression, format).ToString();
        Assert.Equal(outputString, output);
    }

    [Theory]
    [InlineData("R(0-59)/10 * * * * *", CronStringFormat.WithSeconds, new int[] { 0, 10, 20, 30, 40, 50 })]
    [InlineData("* R(0-59)/15 * * * *", CronStringFormat.WithSeconds, new int[] { 0, 15, 30, 45 })]
    [InlineData("* * R(0-23)/6 * * *", CronStringFormat.WithSeconds, new int[] { 0, 6, 12, 18 })]
    [InlineData("R(1-5)/2 * * * * *", CronStringFormat.WithSeconds, new int[] { 1, 3, 5 })]
    [InlineData("R/10 * * * * *", CronStringFormat.WithSeconds, new int[] { 0, 10, 20, 30, 40, 50 })]
    [InlineData("* R/15 * * * *", CronStringFormat.WithSeconds, new int[] { 0, 15, 30, 45 })]
    [InlineData("* * R/6 * * *", CronStringFormat.WithSeconds, new int[] { 0, 6, 12, 18 })]
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
    [InlineData("R(0-59)/0 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(0-59)/-5 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(0-59)/abc * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R(5-1)/2 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R/0 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R/-5 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("R/abc * * * * *", CronStringFormat.WithSeconds)]
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
        var crontab = Crontab.Parse("R(30-59) * * * * *", CronStringFormat.WithSeconds);
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

    [Theory]
    [InlineData("* * * * R#3", CronStringFormat.Default)]
    [InlineData("* * * * H#5", CronStringFormat.Default)]
    [InlineData("0 0 * * R#1", CronStringFormat.Default)]
    [InlineData("0 0 * * H#2", CronStringFormat.Default)]
    public void TestParse_RandomHash_Success(string expression, CronStringFormat format)
    {
        var crontab = Crontab.Parse(expression, format);
        Assert.NotNull(crontab);
    }

    [Theory]
    [InlineData("* * R#3 * * *", CronStringFormat.WithSeconds)]
    [InlineData("R#3 * * * * *", CronStringFormat.WithSeconds)]
    [InlineData("0 0 * * * H#2", CronStringFormat.WithYears)]
    public void TestParse_RandomHash_InvalidField_Throws(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Theory]
    [InlineData("* * * * R#0", CronStringFormat.Default)]
    [InlineData("* * * * R#6", CronStringFormat.Default)]
    [InlineData("* * * * H#-1", CronStringFormat.Default)]
    [InlineData("* * * * H#abc", CronStringFormat.Default)]
    public void TestParse_RandomHash_InvalidNumber_Throws(string expression, CronStringFormat format)
    {
        Assert.Throws<TimeCrontabException>(() => Crontab.Parse(expression, format));
    }

    [Fact]
    public void TestRandomHash_Occurrence_IsValid()
    {
        var crontab = Crontab.Parse("* * * * R#3");
        var next = crontab.GetNextOccurrence(new DateTime(2022, 1, 1, 0, 0, 0));
        Assert.InRange((int)next.DayOfWeek, 0, 6);
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
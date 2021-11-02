using System;
using Xunit;

namespace TimeCrontab.UnitTests
{
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
        public void TestGetNextOccurence(string expression, string nextOccurenceString, CronStringFormat format)
        {
            var beginTime = new DateTime(2021, 1, 1, 0, 0, 0);
            var crontab = Crontab.Parse(expression, format);
            var nextOccurence = crontab.GetNextOccurrence(beginTime);
            Assert.Equal(nextOccurenceString, nextOccurence.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
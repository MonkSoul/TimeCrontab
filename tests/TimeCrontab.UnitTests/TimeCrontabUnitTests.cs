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
        public void TestParse(string expression, string outputString, CronStringFormat format)
        {
            var output = Crontab.Parse(expression, format).ToString();
            Assert.Equal(outputString, output);
        }
    }
}
using Xunit;

namespace TimeCrontab.UnitTests
{
    public class TimeCrontabUnitTests
    {
        [Theory]
        [InlineData("* * * * *", "* * * * *", CronStringFormat.Default)]
        public void TestParse(string expression, string outputString, CronStringFormat format)
        {
            var output = Crontab.Parse(expression, format).ToString();
            Assert.Equal(outputString, output);
        }
    }
}
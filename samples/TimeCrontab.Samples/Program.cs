using TimeCrontab;

var now = DateTime.Now;
Console.WriteLine("当前时间：" + now.ToString());

Console.WriteLine("--------------------------------------------------");

var crontab = Crontab.Parse("R(30-59) * * * * *", CronStringFormat.WithSeconds);
var nextOccurrences = crontab.GetNextOccurrences(now, now.AddMinutes(5)).ToList();

foreach (var occurrence in nextOccurrences)
{
    Console.WriteLine(occurrence);
}

Console.WriteLine("--------------------------------------------------");

var crontab2 = Crontab.Parse("* R(10-20) * * * *", CronStringFormat.WithSeconds);
var nextOccurrences2 = crontab2.GetNextOccurrences(now, now.AddHours(1)).ToList();

foreach (var occurrence in nextOccurrences2)
{
    Console.WriteLine(occurrence);
}

var crontab3 = Crontab.Parse("R(0-59)/5 * * * * *", CronStringFormat.WithSeconds);
var nextOccurrences3 = crontab3.GetNextOccurrences(now, now.AddMinutes(5)).ToList();

Console.WriteLine("--------------------------------------------------");

foreach (var occurrence in nextOccurrences3)
{
    Console.WriteLine(occurrence);
}

var crontab4 = Crontab.Parse("R/5 * * * * *", CronStringFormat.WithSeconds);
var nextOccurrences4 = crontab4.GetNextOccurrences(now, now.AddMinutes(5)).ToList();

Console.WriteLine("--------------------------------------------------");

foreach (var occurrence in nextOccurrences4)
{
    Console.WriteLine(occurrence);
}

var crontab5 = Crontab.Parse("R(30-59) * * * * *", CronStringFormat.WithSeconds);
var nextOccurrences5 = crontab5.GetNextOccurrences(now, 10).ToList();

Console.WriteLine("--------------------------------------------------");

foreach (var occurrence in nextOccurrences5)
{
    Console.WriteLine(occurrence);
}

var crontab6 = Crontab.Parse("R(1,5,10,12) * * * * *", CronStringFormat.WithSeconds);
var nextOccurrences6 = crontab6.GetNextOccurrences(now, 10).ToList();

Console.WriteLine("--------------------------------------------------");

foreach (var occurrence in nextOccurrences6)
{
    Console.WriteLine(occurrence);
}
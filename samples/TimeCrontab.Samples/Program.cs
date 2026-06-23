using TimeCrontab;

var crontab = Crontab.Parse("R30-59 * * * * *", CronStringFormat.WithSeconds);
var nextOccurrences = crontab.GetNextOccurrences(DateTime.Now, DateTime.Now.AddMinutes(5)).ToList();

foreach (var occurrence in nextOccurrences)
{
    Console.WriteLine(occurrence);
}

Console.WriteLine("--------------------------------------------------");

var crontab2 = Crontab.Parse("* R10-20 * * * *", CronStringFormat.WithSeconds);
var nextOccurrences2 = crontab2.GetNextOccurrences(DateTime.Now, DateTime.Now.AddHours(1)).ToList();

foreach (var occurrence in nextOccurrences2)
{
    Console.WriteLine(occurrence);
}

var crontab3 = Crontab.Parse("R0-59/5 * * * * *", CronStringFormat.WithSeconds);
var nextOccurrences3 = crontab3.GetNextOccurrences(DateTime.Now, DateTime.Now.AddMinutes(5)).ToList();

Console.WriteLine("--------------------------------------------------");

foreach (var occurrence in nextOccurrences3)
{
    Console.WriteLine(occurrence);
}

var crontab4 = Crontab.Parse("R30-59 * * * * *", CronStringFormat.WithSeconds);
var nextOccurrences4 = crontab4.GetNextOccurrences(DateTime.Now, 10).ToList();

Console.WriteLine("--------------------------------------------------");

foreach (var occurrence in nextOccurrences4)
{
    Console.WriteLine(occurrence);
}
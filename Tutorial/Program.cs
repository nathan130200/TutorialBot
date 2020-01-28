using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new TutorialBot();
            await bot.StartAsync();

            while (Console.ReadKey(true).Key != ConsoleKey.Q)
                await Task.Delay(1);

            await bot.StopAsync();
        }
    }
}

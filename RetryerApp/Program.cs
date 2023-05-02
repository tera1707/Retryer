using System.Diagnostics;
using SharedRetryProject;

namespace RetryerApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //var retryer = new Retryer();

            //await retryer.TryAndThrowWhenRetryMaxOver<InvalidOperationException>(5, TimeSpan.FromSeconds(1), async () =>
            //{
            //    await ErrorFunc();
            //});

            //Debug.WriteLine("プログラムを終わります。");

            Console.ReadLine();            
        }






        static int cnt = 0;
        private static async Task ErrorFunc()
        {
            await Task.Delay(3000).ConfigureAwait(false);

            cnt++;
            if (cnt < 3)
            {
                throw new InvalidOperationException();
            }
        }
    }













}
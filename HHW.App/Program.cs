using System;
using System.Threading.Tasks;

namespace HHW.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            test();
            call(10);
        }

        static async void test()
        {
            int i = 0;
            while(true)
            {
                i = await test2(i);
                Console.Write(i);
            }
        }

        static Action<int> call;
        static System.Threading.Tasks.Task<int> test2(int i)
        {
            var tcs = new TaskCompletionSource<int>();
            call = (v) =>
            {
                tcs.SetResult(v);
            };

            return tcs.Task;
        }
    }
}

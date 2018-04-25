using System;

namespace HHW.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            test();
        }

        static async void test()
        {
            int i = 0;
            while(true)
            {
                i = await test2(i);
            }
        }

        static async System.Threading.Tasks.Task<int> test2(int i)
        {
            await System.Threading.Tasks.Task.CompletedTask;
            return i + 1; 
        }
    }
}

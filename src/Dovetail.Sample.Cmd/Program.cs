using System;

namespace Dovetail.Sample.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new SomeAppClass().GetSampleKey());
        }
    }
}

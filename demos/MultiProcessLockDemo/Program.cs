using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MultiProcessLockDemo
{
    class Program
    {
        

        static void Main(string[] args)
        {
            var processCount = 4;
            Console.Write("Starting {0} processes - testing lock contention", processCount);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MultiProcessLockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var processCount = 20;
            Console.Write("Starting {0} processes - testing lock contention", processCount);
            var processes = new List<Process>();
            for (var i = 0; i <= processCount; i++)
            {
                processes.Add(StartProcess());
            }

            while (!processes.All(x => x.HasExited))
            {
                Console.WriteLine("Waiting for all processes to acquire lock...");
                Thread.Sleep(1000);
            }
            
        }

        static Process StartProcess()
        {
            var p = new Process();
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.FileName = "SingleProcessLockDemo.exe";
            p.Start();
            return p;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using FileLock;

namespace TimeoutLockDemo
{
    class Program
    {
        private static readonly TimeSpan LockTimeout = new TimeSpan(0, 0, 0, 10);

        private static readonly string LockName = "SampleLock";

        static void Main(string[] args)
        {
            var fileLock = SimpleFileLock.Create(LockName, LockTimeout);
            Console.WriteLine("Acquiring lock");
            var lockAcquired = fileLock.TryAcquireLock();
            if (lockAcquired)
            {
                Console.WriteLine("Lock Acquired");
                Console.WriteLine("Spawning other lock-contending process...");
                var p = StartProcess();
                Console.WriteLine("Sleeping until lock times out (and not releasing it)...");
                Thread.Sleep(TimeSpan.FromSeconds(20));
                Console.WriteLine("Waking up...");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Failed to acquire lock.");
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

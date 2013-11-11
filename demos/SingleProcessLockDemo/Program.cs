using System;
using System.CodeDom;
using System.Diagnostics;
using System.Threading;
using FileLock;

namespace SingleProcessLockDemo
{
    class Program
    {
        private static readonly TimeSpan LockTimeout = new TimeSpan(0, 0, 0, 10);

        private static readonly string LockName = "SampleLock";

        static void Main(string[] args)
        {
            var process = Process.GetCurrentProcess();
            var fileLock = SimpleFileLock.Create(LockName, LockTimeout);

            for (var i = 0; i < 20; i++)
            {
                Console.WriteLine("{0}: PID {1} attempting to acquire FileLock {2} (attempt {3}", DateTime.Now, process.Id, fileLock.LockName, i);
                var acquired = fileLock.TryAcquireLock();
                if (acquired)
                {
                    Console.WriteLine("{0}: PID {1} ACQUIRED FileLock {2} - releasing", DateTime.Now, process.Id,
                        fileLock.LockName);
                    fileLock.ReleaseLock();
                    Console.WriteLine("{0}: PID {1} RELEASED FileLock {2} - releasing", DateTime.Now, process.Id,
                        fileLock.LockName);
                    Console.WriteLine("{0}: PID {1} EXITING", DateTime.Now, process.Id);
                    return;
                }
                else
                {
                    Console.WriteLine("{0}: PID {1} UNABLE TO ACQUIRE FileLock {2} - sleeping", DateTime.Now, process.Id,
                        fileLock.LockName);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}

using System;
using System.Diagnostics;
using FileLock.FileSys;

namespace FileLock
{
    public class SimpleFileLock : IFileLock
    {
        protected SimpleFileLock(string lockName, TimeSpan lockTimeout)
        {
            LockName = lockName;
            LockTimeout = lockTimeout;
        }

        public TimeSpan LockTimeout { get; private set; }

        public string LockName { get; private set; }

        private string LockFilePath { get; set; }

        public bool TryAcquireLock()
        {
            if (LockIO.LockExists(LockFilePath))
            {
                var lockContent = LockIO.ReadLock(LockFilePath);

                //Someone else owns the lock
                if (lockContent.GetType() == typeof(OtherProcessOwnsFileLockContent))
                {
                    return false;
                }

                //the file no longer exists
                if (lockContent.GetType() == typeof(MissingFileLockContent))
                {
                    return AcquireLock();
                }


                var lockWriteTime = new DateTime(lockContent.Timestamp);

                //This lock belongs to this process - we can reacquire the lock
                var currentProcess = Process.GetCurrentProcess();
                if (lockContent.PID == currentProcess.Id && lockContent.MachineName == currentProcess.MachineName)
                {
                    return AcquireLock();
                }

                //The lock has not timed out - we can't acquire it
                if (!(Math.Abs((DateTime.Now - lockWriteTime).TotalSeconds) > LockTimeout.TotalSeconds)) return false;
            }

            //Acquire the lock
            
            return AcquireLock();
        }



        public bool ReleaseLock()
        {
            //Need to own the lock in order to release it (and we can reacquire the lock inside the current process)
            if (LockIO.LockExists(LockFilePath) && TryAcquireLock())
                LockIO.DeleteLock(LockFilePath);
            return true;
        }

        #region Internal methods

        protected FileLockContent CreateLockContent()
        {
            var process = Process.GetCurrentProcess();
            return new FileLockContent
            {
                PID = process.Id,
                ProcessName = process.ProcessName,
                MachineName = process.MachineName,
                Timestamp = DateTime.Now.Ticks
            };
        }

        private bool AcquireLock()
        {
            return LockIO.WriteLock(LockFilePath, CreateLockContent());
        }

        #endregion

        #region Create methods

        public static SimpleFileLock Create(string lockName, TimeSpan lockTimeout)
        {
            if (string.IsNullOrEmpty(lockName))
                throw new ArgumentNullException("lockName", "lockName cannot be null or emtpy.");

            return new SimpleFileLock(lockName, lockTimeout) { LockFilePath = LockIO.GetFilePath(lockName) };
        }

        #endregion
    }
}
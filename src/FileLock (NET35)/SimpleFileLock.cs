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

                if (lockContent.GetType() == typeof(MissingFileLockContent))
                {
                    AcquireLock();
                    return true;
                }


                var lockWriteTime = new DateTime(lockContent.Timestamp);

                //This lock belongs to this process - we can reacquire the lock
                if (lockContent.PID == Process.GetCurrentProcess().Id)
                {
                    AcquireLock();
                    return true;
                }

                //The lock has not timed out - we can't acquire it
                if (!(Math.Abs((DateTime.Now - lockWriteTime).TotalSeconds) > LockTimeout.TotalSeconds)) return false;
            }

            //Acquire the lock
            AcquireLock();
            return true;
        }

        

        public bool ReleaseLock()
        {
            if(LockIO.LockExists(LockFilePath))
                LockIO.DeleteLock(LockFilePath);
            return true;
        }

        #region Internal methods

        protected FileLockContent CreateLockContent()
        {
            var process = Process.GetCurrentProcess();
            return new FileLockContent()
            {
                PID = process.Id,
                Timestamp = DateTime.Now.Ticks,
                ProcessName = process.ProcessName
            };
        }

        private void AcquireLock()
        {
            LockIO.WriteLock(LockFilePath, CreateLockContent());
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
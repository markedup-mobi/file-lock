using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FileLock.FileSys
{
    internal static class LockIO
    {
        private static readonly DataContractJsonSerializer JsonSerializer;

        static LockIO()
        {
            JsonSerializer = new DataContractJsonSerializer(typeof(FileLockContent), new[] { typeof(FileLockContent) }, int.MaxValue, true, null, true);
        }

        public static string GetFilePath(string lockName)
        {
            return Path.Combine(Path.GetTempPath(), lockName);
        }

        public static bool LockExists(string lockFilePath)
        {
            return File.Exists(lockFilePath);
        }

        public static FileLockContent ReadLock(string lockFilePath)
        {
            try
            {
                using (var stream = File.OpenRead(lockFilePath))
                {
                    var obj = JsonSerializer.ReadObject(stream);
                    return (FileLockContent) obj;
                }
            }
            catch (FileNotFoundException)
            {
                return new MissingFileLockContent();
            }
            catch (IOException)
            {
                return new OtherProcessOwnsFileLockContent();
            }
        }

        public static bool WriteLock(string lockFilePath, FileLockContent lockContent)
        {
            try
            {
                using (var stream = File.Create(lockFilePath))
                {
                    JsonSerializer.WriteObject(stream, lockContent);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DeleteLock(string lockFilePath)
        {
            File.Delete(lockFilePath);
        }
    }
}

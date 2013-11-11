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
            using (var stream = File.OpenRead(lockFilePath))
            {
                var obj = JsonSerializer.ReadObject(stream);
                return (FileLockContent)obj;
            }
        }

        public static void WriteLock(string lockFilePath, FileLockContent lockContent)
        {
            using (var stream = File.Create(lockFilePath))
            {
                JsonSerializer.WriteObject(stream, lockContent);
            }
        }

        public static void DeleteLock(string lockFilePath)
        {
            File.Delete(lockFilePath);
        }
    }
}

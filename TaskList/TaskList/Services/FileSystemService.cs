using System.IO;
using TaskList.Bootstraping;

namespace TaskList.Services
{
    public interface IFileSystemService
    {
        void CreateDirectory(string path);
    }
    
    public class FileSystemService : IFileSystemService
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
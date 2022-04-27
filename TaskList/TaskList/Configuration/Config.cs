using System;
using System.IO;

namespace TaskList.Configuration
{
    public interface IConfig
    {
        string SqliteDbDirectory { get; }
        string SqliteDbFilePath { get; }
    }
    
    public class Config : IConfig
    {
        private readonly string _rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); 
        
        public Config()
        {
            SqliteDbName = "Tasks.db3";
        }
        
        public string SqliteDbName { get; }
        public string SqliteDbDirectory => Path.Combine(_rootDirectory, "Database");
        public string SqliteDbFilePath => Path.Combine(SqliteDbDirectory, SqliteDbName);
    }
}
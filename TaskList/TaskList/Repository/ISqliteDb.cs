using SQLite;
using TaskList.Configuration;
using TaskList.Services;

namespace TaskList.Repository
{
    public interface ISqliteDb
    {
        SQLiteAsyncConnection GetDb();
    }
    
    public class SqliteDb : ISqliteDb
    {
        private readonly SQLiteAsyncConnection _connection;

        public SqliteDb(IConfig config, IFileSystemService fileSystemService)
        {
            fileSystemService.CreateDirectory(config.SqliteDbDirectory);
            _connection = new SQLiteAsyncConnection(config.SqliteDbFilePath);   
        }
        
        public SQLiteAsyncConnection GetDb()
        {
            return _connection;
        }
    }
}
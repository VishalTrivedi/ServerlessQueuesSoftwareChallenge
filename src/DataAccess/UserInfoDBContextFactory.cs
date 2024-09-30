using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class UserInfoDBContextFactory : IDisposable
{
    private DbConnection _connection;
    private readonly string _connectionString;

    public UserInfoDBContextFactory()
    {
        _connectionString = Environment.GetEnvironmentVariable("DBConnectionString");
    }

    private DbContextOptions<UserInfoDBContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<UserInfoDBContext>()
            .UseSqlite(_connection)
            .Options;            
    }

    public UserInfoDBContext CreateContext()
    {
        if (_connection == null)
        {
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();

            var options = CreateOptions();
            using (var context = new UserInfoDBContext(options))
            {
                context.Database.EnsureCreated();
            }
        }

        return new UserInfoDBContext(CreateOptions());
    }

    public void Dispose()
    {
        if (_connection != null)
        {
            _connection.Dispose();
            _connection = null;
        }
    }
}

using BusinessLogic.Models;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace InMemoryDB.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        // The database name allows the scope of the in-memory database
        // to be controlled independently of the context. The in-memory database is shared
        // anywhere the same name is used.
        var options = new DbContextOptionsBuilder<UserInfoDBContext>()
            .UseInMemoryDatabase(databaseName: "Test1")
            .Options;

        using (var context = new UserInfoDBContext(options))
        {
            var user = new User() { Id=1, FirstName="Jerry", LastName="Nettleton", SVGData="abcd" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // New context with the data as the database name is the same
        using (var context = new UserInfoDBContext(options))
        {
            var count = await context.Users.CountAsync();
            

            var u = await context.Users.FirstOrDefaultAsync(user => user.FirstName == "Jerry");
        }
    }
}
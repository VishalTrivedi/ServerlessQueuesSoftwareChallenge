using BusinessLogic.Models;

namespace DataAccess;

public class UserRepository : IUserRepository, IDisposable
{
    private readonly UserInfoDBContext _dbContext;

    public UserRepository(
        UserInfoDBContextFactory userInfoDBContextFactory)
    {
        _dbContext = userInfoDBContextFactory.CreateContext();
    }

    public async Task<User?> CreateUser(
        string firstName,
        string lastName)
    {
        if (FindUserByName(firstName, lastName) is not null)
        {
            return null;
        }

        var user = new User()
        {
            FirstName = firstName,
            LastName = lastName,
            SVGData = "ABC"
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User?> UpdateUser(
        int id,
        string svgData)
    {
        var user = FindUserById(id);

        if (user == null)
        {
            return null;
        }

        user.SVGData = svgData;

        _dbContext.Users.Update(user);

        await _dbContext.SaveChangesAsync();

        return user;
    }

    private User? FindUserByName(
        string firstName,
        string lastName)
    {
        return _dbContext.Users
            .Where(u => u.FirstName == firstName
                && u.LastName == lastName)
            .FirstOrDefault();
    }

    private User? FindUserById(
        int id)
    {
        return _dbContext.Users
            .Where(u => u.Id == id)
            .FirstOrDefault();
    }

    public void Dispose()
    {
        if (_dbContext is not null)
        {
            _dbContext.Dispose();
        }

        if (_userInfoDBContextFactory is not null)
        {
            _userInfoDBContextFactory.Dispose();
        }
    }
}

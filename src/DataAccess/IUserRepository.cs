using BusinessLogic.Models;

namespace DataAccess;

public interface IUserRepository
{
    Task<User?> CreateUser(
        string firstName, 
        string lastName);
    
    Task<User?> UpdateUser(
        int id, 
        string svgData);
}
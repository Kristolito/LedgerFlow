using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Abstractions.Security;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
}

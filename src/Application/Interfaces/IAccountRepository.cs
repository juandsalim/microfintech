using Domain.Entities;

namespace Application.Interfaces;

public interface IAccountRepository
{
    Task AddAsync(Account account);
    Task SaveChangesAsync();
}
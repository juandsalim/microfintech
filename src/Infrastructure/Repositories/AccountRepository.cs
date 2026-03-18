using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
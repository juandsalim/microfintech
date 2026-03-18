// src/Domain/Entities/Account.cs
using System;

namespace Domain.Entities;

public class Account
{
    // Propiedades con 'init' o 'private set' para evitar modificaciones externas ilegales
    public Guid Id { get; init; }
    public string Currency { get; init; }
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; init; }

    // Constructor privado requerido por Entity Framework
    private Account() { }

    public Account(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty");

        Id = Guid.NewGuid();
        Currency = currency.ToUpper();
        Balance = 0;
        CreatedAt = DateTime.UtcNow;
    }

    // La lógica de negocio ocurre ADENTRO de la entidad
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be strictly positive.");

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be strictly positive.");

        // Nuestro invariante: el balance no puede ser negativo
        if (Balance - amount < 0)
            throw new InvalidOperationException("Insufficient funds.");

        Balance -= amount;
    }
}
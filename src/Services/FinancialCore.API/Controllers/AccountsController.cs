using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinancialCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository _repository;

    public AccountsController(IAccountRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> OpenAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            // 1. Instanciamos nuestra entidad de dominio (la que tiene la lógica)
            var account = new Account(request.Currency);

            // 2. Usamos el repositorio para guardarla
            await _repository.AddAsync(account);
            await _repository.SaveChangesAsync();

            // 3. Retornamos un 201 Created con el ID generado
            return CreatedAtAction(nameof(OpenAccount), new { id = account.Id }, account);
        }
        catch (ArgumentException ex)
        {
            // Si la regla de negocio de la moneda vacía falla, devolvemos un 400 Bad Request
            return BadRequest(new { error = ex.Message });
        }
    }
}
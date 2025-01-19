using API.Dtos.Transaction;
using API.Services.Stripe;
using API.Services.Transactions;
using API.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/transaction")]
public class TransactionController( ITransactionService transactionService) : ControllerBase
{
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequestDto transactionRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        transactionRequestDto.UserId = Constants.GetUserId(HttpContext);
        var response = await transactionService.CreateTransaction(transactionRequestDto);
        return StatusCode(response.StatusCode, response);
        
        
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllTransactions()
    {
        var response = await transactionService.GetAllTransactions();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        var response = await transactionService.GetTransactionById(id);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("users")]
    public async Task<IActionResult> GetAllTransactionsByUserId()
    {
        
        var response = await transactionService.GetAllTransactionsByUserId(Constants.GetUserId(HttpContext));
        return StatusCode(response.StatusCode, response);
    }
    
}
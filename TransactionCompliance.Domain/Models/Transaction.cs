using System.ComponentModel.DataAnnotations;

namespace TransactionCompliance.Domain.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int StatusId { get; set; }

    [Required]
    public int TransactionTypeId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string AccountNumberMasked { get; set; }

    [Required]
    public string CardNumberMasked { get; set; }

    [Required]
    public string AccountNumber { get; set; }

    [Required]
    public string CardNumber { get; set; }

    [Required]
    public string ReferenceNumber { get; set; }

    [Required]
    public string CustomerName { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public Status Status { get; set; }

    public TransactionType TransactionType { get; set; }
    public UserAccount UserAccount { get; set; }

}
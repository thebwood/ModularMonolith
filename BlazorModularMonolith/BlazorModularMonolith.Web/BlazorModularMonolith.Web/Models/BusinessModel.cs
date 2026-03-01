namespace BlazorModularMonolith.Web.Models;

public class BusinessModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public BusinessType Type { get; set; }
    public List<Guid> AddressIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum BusinessType
{
    SoleProprietorship = 1,
    Partnership = 2,
    Corporation = 3,
    LLC = 4,
    NonProfit = 5,
    Other = 6
}

public class CreateBusinessModel
{
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public BusinessType Type { get; set; }
}

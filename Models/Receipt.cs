using System.Text.Json.Serialization;

namespace ocr_receipt.Models;

public class Receipt
{
    public int Id { get; init; }
    public string StoreName { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = [];
    public DateOnly? BuyDate { get; set; } = null;
    public decimal TotalPrice { get; set; }
    public decimal AmountPaid { get; set; } = default;
    public string? PaymentMethod { get; set; } = string.Empty;
    public decimal? Changes { get; set;} = default;
}

public class Product
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; } = default;
    public decimal PricePerItem { get; set; } = default;
    public int ReceiptId { get; set; }
}

public record ProductDTO(
    [property: JsonPropertyName("product_name")] string ProductName,
    [property: JsonPropertyName("quantity")] int Quantity,
    [property: JsonPropertyName("price_per_item")] decimal PricePerItem
);
public record ReceiptDTO(
    [property: JsonPropertyName("store_name")] string StoreName,
    [property: JsonPropertyName("products")] List<ProductDTO> Products,
    [property: JsonPropertyName("buy_date")] string BuyDate,
    [property: JsonPropertyName("total_price")] decimal TotalPrice,
    [property: JsonPropertyName("amount_paid")] decimal AmountPaid,
    [property: JsonPropertyName("payment_method")] string? PaymentMethod,
    [property: JsonPropertyName("changes")] decimal? Changes
);

public record ReceiptPatchDTO(
    [property: JsonPropertyName("store_name")] string? StoreName = null,
    [property: JsonPropertyName("products")] List<ProductDTO>? Products = null,
    [property: JsonPropertyName("buy_date")] string? BuyDate = null,
    [property: JsonPropertyName("total_price")] decimal? TotalPrice = null,
    [property: JsonPropertyName("amount_paid")] decimal? AmountPaid = null,
    [property: JsonPropertyName("payment_method")] string? PaymentMethod = null,
    [property: JsonPropertyName("changes")] decimal? Changes = null
);
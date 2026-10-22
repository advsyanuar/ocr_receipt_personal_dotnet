namespace ocr_receipt.Models;

public class OCRProcess
{
    public int Id { get; init;}
    public string ImagePath { get; set; } = string.Empty;
    public bool IsProcessed { get; set; }
    public string? RawTextPath { get; set; }
    public string? JSONText { get; set; }
}

public record OCRProcessDTO(string? RawTextPath = null, string? JSONText = null, string? ImagePath = null, bool? IsProcessed = null);
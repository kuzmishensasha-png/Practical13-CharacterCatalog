namespace Practical1.Infra.Data;

public class TextElementData
{
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";

    public string? Content { get; set; }
    public string? Url { get; set; }
    public string? Source { get; set; }

    public int Level { get; set; }

    public List<TextElementData> Children { get; set; } = new();
}
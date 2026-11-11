namespace Practical1.TextSystem;

public class ImageElement : TextElement
{
    public override string Name =>
        Description;

    public string Source { get; set; }

    public string Description { get; set; }

    public override TextElement DeepClone() => new ImageElement(Source, Description);

    public ImageElement(
        string source,
        string description)
    {
        Source = source;
        Description = description;
    }
}
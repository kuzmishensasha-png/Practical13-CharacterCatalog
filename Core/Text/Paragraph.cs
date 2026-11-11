namespace Practical1.TextSystem;

public class Paragraph : TextElement
{
    public override string Name => Content;

    public string Content { get; set; }

    public override TextElement DeepClone() => new Paragraph(Content);

    public Paragraph(string content)
    {
        Content = content;
    }
}
namespace Practical1.TextSystem;

public class Heading : ContainerElement
{
    public override string Name => Content;

    public string Content { get; set; }

    public int Level { get; set; }

    public override TextElement DeepClone()
    {
        Heading copy = new Heading(Content, Level);
        foreach (TextElement child in Children)
        {
            copy.Add(child.DeepClone());
        }
        return copy;
    }

    public Heading(
        string content,
        int level)
    {
        Content = content;
        Level = level;
    }
}
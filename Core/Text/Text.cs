namespace Practical1.TextSystem;

public class Text : ContainerElement
{
    public override string Name => "";

    public override TextElement DeepClone()
    {
        Text copy = new Text();
        foreach (TextElement child in Children)
        {
            copy.Add(child.DeepClone());
        }
        return copy;
    }

    public string GetTableOfContents()
    {
        List<string> lines = new();
        CollectHeadings(this, lines);
        return string.Join(Environment.NewLine, lines);
    }

    private static void CollectHeadings(ContainerElement container, List<string> lines)
    {
        foreach (TextElement element in container.Children)
        {
            if (element is Heading heading)
            {
                string indentation = new string(' ', Math.Max(0, heading.Level - 1) * 2);
                lines.Add(indentation + heading.Content);
            }
            if (element is ContainerElement nested)
            {
                CollectHeadings(nested, lines);
            }
        }
    }
}

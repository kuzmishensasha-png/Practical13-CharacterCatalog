namespace Practical1.TextSystem;

public abstract class TextElement
{
    public string Id { get; } = Guid.NewGuid()
        .ToString("N")[..8];

    public ContainerElement? Parent { get; internal set; }

    public abstract string Name { get; }
    public abstract TextElement DeepClone();
}
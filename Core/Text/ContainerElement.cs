namespace Practical1.TextSystem;

public abstract class ContainerElement : TextElement
{
    private readonly List<TextElement> children = new();

    public IReadOnlyList<TextElement> Children => children;

    public void Add(TextElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        if (element.Parent != null)
            throw new InvalidOperationException("Element already belongs to a container.");

        for (ContainerElement? ancestor = this;
             ancestor != null;
             ancestor = ancestor.Parent)
        {
            if (ReferenceEquals(ancestor, element))
                throw new InvalidOperationException("Adding this element would create a cycle.");
        }

        element.Parent = this;
        children.Add(element);
    }

    public bool Remove(TextElement element)
    {
        if (!children.Remove(element))
        {
            return false;
        }

        element.Parent = null;
        return true;
    }
        public void RemoveAt(int index)
    {
        TextElement element = children[index];

        children.RemoveAt(index);
        element.Parent = null;
    }

    public void Move(int fromIndex, int toIndex)
    {
        TextElement element = children[fromIndex];

        children.RemoveAt(fromIndex);
        children.Insert(toIndex, element);
    }

    public TextElement? FindChild(string name)
    {
        return children.FirstOrDefault(
            element => string.Equals(
                element.Name,
                name,
                StringComparison.OrdinalIgnoreCase
            )
        );
    }
}
namespace Practical1.TextSystem;

public sealed class DocumentSession
{
    private static readonly Lazy<DocumentSession> lazy =
        new(() => new DocumentSession());

    private Text document = new Text();

    private DocumentSession() { }

    public static DocumentSession Instance => lazy.Value;

    public Text Document => document;

    public void Replace(Text newDocument)
    {
        ArgumentNullException.ThrowIfNull(newDocument);
        if (newDocument.Parent != null)
        {
            throw new InvalidOperationException("The document root cannot have a parent.");
        }
        document = newDocument;
    }

    public void Clear() => document = new Text();
}

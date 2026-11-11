using Practical1.CLI;
using Practical1.TextSystem;

namespace Practical1.Tests.Core;

public class CreationalPatternsTests
{
    [Fact]
    public void Singleton_ReturnsSameSessionAndReplacesRoot()
    {
        DocumentSession session = DocumentSession.Instance;
        Text previous = session.Document;
        try
        {
            Assert.Same(session, DocumentSession.Instance);
            Text replacement = new Text();
            replacement.Add(new Paragraph("new"));
            session.Replace(replacement);
            Assert.Same(replacement, DocumentSession.Instance.Document);
            Assert.Single(session.Document.Children);
        }
        finally
        {
            session.Replace(previous);
        }
    }

    [Fact]
    public void Singleton_RejectsNullAndAttachedRoot()
    {
        DocumentSession session = DocumentSession.Instance;
        Assert.Throws<ArgumentNullException>(() => session.Replace(null!));

        Text invalid = new Text();
        Text other = new Text();
        other.Add(invalid);
        Assert.Throws<InvalidOperationException>(() => session.Replace(invalid));
    }

    [Fact]
    public void Builder_CreatesNestedTreeAndRendersCorrectly()
    {
        Text document = new TextBuilder()
            .BeginSection("Chapter", 1)
                .AddParagraph("First paragraph")
                .BeginSection("Subchapter", 2)
                    .AddParagraph("Second paragraph")
                    .AddLink("Site", "https://example.org")
                .EndSection()
            .EndSection()
            .Build();

        Heading chapter = Assert.IsType<Heading>(Assert.Single(document.Children));
        Assert.Same(document, chapter.Parent);
        Assert.Equal(2, chapter.Children.Count);
        Heading sub = Assert.IsType<Heading>(chapter.Children[1]);
        Assert.Same(chapter, sub.Parent);
        Assert.Equal(2, sub.Children.Count);
        Assert.Equal("Chapter" + Environment.NewLine + "  Subchapter", document.GetTableOfContents());

        string rendered = new TextRenderer().Render(document);
        Assert.Contains("# Chapter", rendered);
        Assert.Contains("## Subchapter", rendered);
        Assert.Contains("Second paragraph", rendered);
    }

    [Fact]
    public void Builder_RejectsUnclosedAndUnopenedSections()
    {
        TextBuilder builder = new TextBuilder();
        Assert.Throws<InvalidOperationException>(() => builder.EndSection());
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.BeginSection("Invalid", 0));
        builder.BeginSection("Open", 1);
        Assert.Throws<InvalidOperationException>(() => builder.Build());
        builder.EndSection();
        Assert.Single(builder.Build().Children);
    }

    [Fact]
    public void Builder_OldMethodsRemainCompatible()
    {
        TextBuilder builder = new TextBuilder();
        Heading heading = builder.CreateHeading("Old", 1);
        Paragraph paragraph = builder.CreateParagraph("Paragraph");
        Assert.Collection(builder.Build().Children,
            element => Assert.Same(heading, element),
            element => Assert.Same(paragraph, element));
    }

    [Fact]
    public void Prototype_CopiesNestedTreeWithIndependentNodesAndIds()
    {
        Text original = new Text();
        Heading heading = new Heading("Chapter", 1);
        Heading nested = new Heading("Nested", 2);
        nested.Add(new Paragraph("Unchanged"));
        heading.Add(nested);
        original.Add(heading);

        Text copy = Assert.IsType<Text>(original.DeepClone());
        Heading copyHeading = Assert.IsType<Heading>(Assert.Single(copy.Children));
        Heading copyNested = Assert.IsType<Heading>(Assert.Single(copyHeading.Children));
        Paragraph copyParagraph = Assert.IsType<Paragraph>(Assert.Single(copyNested.Children));

        Assert.NotSame(original, copy);
        Assert.NotEqual(original.Id, copy.Id);
        Assert.NotEqual(heading.Id, copyHeading.Id);
        Assert.NotEqual(nested.Id, copyNested.Id);
        Assert.Same(copy, copyHeading.Parent);
        Assert.Same(copyHeading, copyNested.Parent);
        Assert.Same(copyNested, copyParagraph.Parent);

        copyParagraph.Content = "Changed";
        Paragraph originalParagraph = Assert.IsType<Paragraph>(Assert.Single(nested.Children));
        Assert.Equal("Unchanged", originalParagraph.Content);
    }

    [Fact]
    public void Tree_RejectsCyclesAndMultipleParents()
    {
        Text root = new Text();
        Heading section = new Heading("Section", 1);
        root.Add(section);

        Assert.Throws<InvalidOperationException>(() => section.Add(root));
        Assert.Throws<InvalidOperationException>(() => root.Add(section));

        Text another = new Text();
        Assert.Throws<InvalidOperationException>(() => another.Add(section));

        Assert.True(root.Remove(section));
        another.Add(section);
        Assert.Same(another, section.Parent);
    }

    [Fact]
    public void Prototype_CopiesLeafPropertiesWithoutRetainingParent()
    {
        Link link = new Link("Search", "https://example.org");
        ImageElement image = new ImageElement("picture.png", "Illustration");
        Text parent = new Text();
        parent.Add(link);
        parent.Add(image);

        Link copyLink = Assert.IsType<Link>(link.DeepClone());
        ImageElement copyImage = Assert.IsType<ImageElement>(image.DeepClone());

        Assert.Equal(link.Text, copyLink.Text);
        Assert.Equal(link.Url, copyLink.Url);
        Assert.Equal(image.Source, copyImage.Source);
        Assert.Equal(image.Description, copyImage.Description);
        Assert.NotEqual(link.Id, copyLink.Id);
        Assert.NotEqual(image.Id, copyImage.Id);
        Assert.Null(copyLink.Parent);
        Assert.Null(copyImage.Parent);
    }
}

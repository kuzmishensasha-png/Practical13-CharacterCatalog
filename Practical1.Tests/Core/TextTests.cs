using Practical1.TextSystem;

namespace Practical1.Tests.Core;

public class TextTests
{
    [Fact]
    public void TextBuilder_CreateParagraph_AddsParagraph()
    {
        TextBuilder builder =
            new TextBuilder();

        Paragraph paragraph =
            builder.CreateParagraph(
                "Hello world"
            );

        Text document =
            builder.Build();

        Assert.Single(
            document.Children
        );

        Assert.Same(
            paragraph,
            document.Children[0]
        );

        Assert.Equal(
            "Hello world",
            paragraph.Content
        );
    }

    [Fact]
    public void TextBuilder_CreateHeading_AddsHeading()
    {
        TextBuilder builder =
            new TextBuilder();

        Heading heading =
            builder.CreateHeading(
                "Main title",
                1
            );

        Text document =
            builder.Build();

        Assert.Single(
            document.Children
        );

        Assert.Same(
            heading,
            document.Children[0]
        );

        Assert.Equal(
            "Main title",
            heading.Content
        );

        Assert.Equal(
            1,
            heading.Level
        );
    }

    [Fact]
    public void Container_Add_SetsParent()
    {
        Text document =
            new Text();

        Heading heading =
            new Heading(
                "Chapter",
                1
            );

        document.Add(
            heading
        );

        Assert.Same(
            document,
            heading.Parent
        );
    }

    [Fact]
    public void GetTableOfContents_ReturnsHeadings()
    {
        Text document =
            new Text();

        document.Add(
            new Heading(
                "First",
                1
            )
        );

        document.Add(
            new Heading(
                "Second",
                2
            )
        );

        string result =
            document.GetTableOfContents();

        string expected =
            "First"
            + Environment.NewLine
            + "  Second";

        Assert.Equal(
            expected,
            result
        );
    }
}
using Practical1.CLI;
using Practical1.TextSystem;

namespace Practical1.Tests;

public class TextRendererTests
{
    [Fact]
    public void Render_Paragraph_ReturnsContent()
    {

        TextRenderer renderer =
            new TextRenderer();

        Paragraph paragraph =
            new Paragraph("Hello world");


        string result =
            renderer.Render(paragraph);


        Assert.Equal(
            "Hello world",
            result
        );
    }

    [Fact]
    public void Render_Heading_ReturnsFormattedHeading()
    {

        TextRenderer renderer =
            new TextRenderer();

        Heading heading =
            new Heading(
                "Title",
                2
            );


        string result =
            renderer.Render(heading);


        Assert.Equal(
            "## Title",
            result
        );
    }

    [Fact]
    public void Render_Link_ReturnsTextAndUrl()
    {

        TextRenderer renderer =
            new TextRenderer();

        Link link =
            new Link(
                "Google",
                "https://google.com"
            );


        string result =
            renderer.Render(link);


        Assert.Equal(
            "Google (https://google.com)",
            result
        );
    }

    [Fact]
    public void Render_TextWithChildren_ReturnsWholeDocument()
    {

        TextRenderer renderer =
            new TextRenderer();

        Text document =
            new Text();

        document.Add(
            new Heading(
                "Title",
                1
            )
        );

        document.Add(
            new Paragraph(
                "Some text"
            )
        );


        string result =
            renderer.Render(document);

        string expected =
            "# Title"
            + Environment.NewLine
            + "Some text";


        Assert.Equal(
            expected,
            result
        );
    }
}
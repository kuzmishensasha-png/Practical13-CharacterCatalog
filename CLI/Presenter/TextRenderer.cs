using Practical1.TextSystem;

namespace Practical1.CLI;

public class TextRenderer
{
    public string Render(TextElement element)
    {
        if (element is Text text)
        {
            return RenderContainer(text);
        }

        if (element is Heading heading)
        {
            string result =
                new string('#', heading.Level)
                + " "
                + heading.Content;

            if (heading.Children.Count > 0)
            {
                result +=
                    Environment.NewLine
                    + RenderContainer(heading);
            }

            return result;
        }

        if (element is Paragraph paragraph)
        {
            return paragraph.Content;
        }

        if (element is Link link)
        {
            return $"{link.Text} ({link.Url})";
        }

        if (element is ImageElement image)
        {
            return
                $"[Image: {image.Description}] ({image.Source})";
        }

        return "";
    }

    private string RenderContainer(
        ContainerElement container)
    {
        return string.Join(
            Environment.NewLine,
            container.Children.Select(Render)
        );
    }
}
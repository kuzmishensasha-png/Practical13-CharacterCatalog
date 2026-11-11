using System.Text;

namespace Practical1.CLI.Catalog;

public class TextCatalogRenderer : ICatalogRenderer
{
    public string RenderList(IReadOnlyList<CatalogCharacter> characters)
    {
        if (characters.Count == 0)
        {
            return "Catalog is empty. Use update or create.";
        }

        StringBuilder result = new();

        foreach (CatalogCharacter character in characters
                     .OrderBy(character => character.Name))
        {
            string source = character.IsCustom ? "user" : "API";
            result.AppendLine($"[{source}] {character.Name} (id: {character.Id})");
        }

        result.AppendLine($"Total: {characters.Count}");
        return result.ToString().TrimEnd();
    }

    public string RenderDetails(CatalogCharacter character)
    {
        StringBuilder result = new();
        result.AppendLine($"ID: {character.Id}");
        result.AppendLine($"Name: {character.Name}");
        result.AppendLine($"Source: {(character.IsCustom ? "user" : "API")}");
        result.AppendLine($"Rarity: {(character.Rarity > 0 ? character.Rarity.ToString() : "unknown")}");
        result.AppendLine($"Element: {ShowOrUnknown(character.Vision)}");
        result.AppendLine($"Weapon: {ShowOrUnknown(character.Weapon)}");
        result.AppendLine($"Description: {ShowOrUnknown(character.Description)}");
        result.AppendLine($"Image: {ShowOrUnknown(character.ImageUrl)}");
        return result.ToString().TrimEnd();
    }

    private static string ShowOrUnknown(string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? "not specified" : text;
    }
}

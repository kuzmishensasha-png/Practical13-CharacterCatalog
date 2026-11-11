namespace Practical1.CLI.Catalog;

public interface ICatalogRenderer
{
    string RenderList(IReadOnlyList<CatalogCharacter> characters);
    string RenderDetails(CatalogCharacter character);
}

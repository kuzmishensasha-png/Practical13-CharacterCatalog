namespace Practical1.CLI.Catalog;

public class CatalogCharacter
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Vision { get; set; } = "";
    public string Weapon { get; set; } = "";
    public int Rarity { get; set; }
    public string ImageUrl { get; set; } = "";
    public bool IsCustom { get; set; }
    public bool DetailsLoaded { get; set; }
}

public class CatalogData
{
    public List<CatalogCharacter> Characters { get; set; } = new();
}

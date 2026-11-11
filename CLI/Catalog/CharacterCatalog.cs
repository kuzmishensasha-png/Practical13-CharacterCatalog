using Practical1.Infra.Data;
using Practical1.Infra.Remote;

namespace Practical1.CLI.Catalog;

public class CharacterCatalog
{
    private readonly IGenshinApiClient api;
    private readonly IDataStorage storage;
    private readonly List<CatalogCharacter> characters = new();

    public IReadOnlyList<CatalogCharacter> Characters => characters.AsReadOnly();

    public CharacterCatalog(IGenshinApiClient api, IDataStorage storage)
    {
        this.api = api;
        this.storage = storage;
    }

    public async Task<int> UpdateAsync()
    {
        IReadOnlyList<string> remoteIds = await api.GetCharacterIdsAsync();
        if (remoteIds.Count == 0)
        {
            throw new HttpRequestException("API returned an empty character list.");
        }

        HashSet<string> ids = remoteIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Залишаємо власних персонажів, оновлюємо лише дані з API.
        characters.RemoveAll(character =>
            !character.IsCustom && !ids.Contains(character.Id));

        foreach (string id in ids.OrderBy(id => id))
        {
            if (characters.Any(character =>
                string.Equals(character.Id, id, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            characters.Add(new CatalogCharacter
            {
                Id = id,
                Name = id,
                ImageUrl = RemoteImageUrl(id)
            });
        }

        return ids.Count;
    }

    public async Task<CatalogCharacter?> FindAsync(string idOrName)
    {
        CatalogCharacter? character = characters.FirstOrDefault(character =>
            string.Equals(character.Id, idOrName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(character.Name, idOrName, StringComparison.OrdinalIgnoreCase));

        if (character == null || character.IsCustom || character.DetailsLoaded)
        {
            return character;
        }

        RemoteCharacterData? data = await api.GetCharacterAsync(character.Id);

        if (data != null)
        {
            character.Name = string.IsNullOrWhiteSpace(data.Name)
                ? character.Id : data.Name;
            character.Description = data.Description ?? "";
            character.Vision = data.Vision ?? "";
            character.Weapon = data.Weapon ?? "";
            character.Rarity = data.Rarity;
            character.DetailsLoaded = true;
        }

        return character;
    }

    public CatalogCharacter CreateCustom(
        string name,
        string description,
        string vision,
        string weapon,
        int rarity,
        string imageUrl)
    {
        if (characters.Any(character =>
            string.Equals(character.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A character with this name already exists.");
        }

        CatalogCharacter character = new()
        {
            Id = "user-" + Guid.NewGuid().ToString("N")[..8],
            Name = name,
            Description = description,
            Vision = vision,
            Weapon = weapon,
            Rarity = rarity,
            ImageUrl = imageUrl,
            IsCustom = true,
            DetailsLoaded = true
        };

        characters.Add(character);
        return character;
    }

    public void Save(string fileName)
    {
        storage.Save(fileName, new CatalogData
        {
            Characters = characters.ToList()
        });
    }

    public bool Load(string fileName)
    {
        CatalogData? data = storage.Load<CatalogData>(fileName);

        if (data == null)
        {
            return false;
        }

        List<CatalogCharacter> loaded = (data.Characters ?? new())
            .Where(character => character != null
                && !string.IsNullOrWhiteSpace(character.Id)
                && !string.IsNullOrWhiteSpace(character.Name))
            .GroupBy(character => character.Id, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();

        characters.Clear();
        characters.AddRange(loaded);
        return true;
    }

    private static string RemoteImageUrl(string id)
    {
        return "https://genshin.jmp.blue/characters/"
            + Uri.EscapeDataString(id) + "/portrait";
    }
}

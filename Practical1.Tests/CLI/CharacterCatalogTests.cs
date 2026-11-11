using Practical1.CLI.Catalog;
using Practical1.Infra.Data;
using Practical1.Infra.Remote;

namespace Practical1.Tests.CLI;

public class CharacterCatalogTests
{
    private sealed class FakeApi : IGenshinApiClient
    {
        public IReadOnlyList<string> Ids { get; set; } = new[] { "amber", "lisa" };

        public Task<IReadOnlyList<string>> GetCharacterIdsAsync()
            => Task.FromResult(Ids);

        public Task<RemoteCharacterData?> GetCharacterAsync(string id)
            => Task.FromResult<RemoteCharacterData?>(new RemoteCharacterData
            {
                Name = id == "amber" ? "Amber" : "Lisa",
                Description = "Character details",
                Rarity = 4,
                Vision = "Pyro",
                Weapon = "Bow"
            });

        public Task<IReadOnlyList<string>> GetWeaponIdsAsync()
            => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

        public Task<RemoteWeaponData?> GetWeaponAsync(string id)
            => Task.FromResult<RemoteWeaponData?>(null);
    }

    [Fact]
    public async Task UpdateAndShow_LoadsRemoteInformationAndImageLink()
    {
        CharacterCatalog catalog = new(new FakeApi(), new JsonDataStorage());

        int count = await catalog.UpdateAsync();
        CatalogCharacter? amber = await catalog.FindAsync("amber");

        Assert.Equal(2, count);
        Assert.NotNull(amber);
        Assert.Equal("Amber", amber.Name);
        Assert.Equal("Pyro", amber.Vision);
        Assert.Contains("/characters/amber/portrait", amber.ImageUrl);
    }

    [Fact]
    public async Task Update_KeepsCustomCharacters()
    {
        FakeApi api = new();
        CharacterCatalog catalog = new(api, new JsonDataStorage());
        catalog.CreateCustom("My Hero", "Story", "Cryo", "Sword", 5, "");

        await catalog.UpdateAsync();
        api.Ids = new[] { "amber" };
        await catalog.UpdateAsync();

        Assert.Contains(catalog.Characters, character => character.Name == "My Hero");
        Assert.DoesNotContain(catalog.Characters, character => character.Id == "lisa");
    }

    [Fact]
    public void SaveAndLoad_RestoresCustomCharacter()
    {
        string file = Path.Combine(Path.GetTempPath(), $"catalog_{Guid.NewGuid():N}.json");
        try
        {
            CharacterCatalog original = new(new FakeApi(), new JsonDataStorage());
            original.CreateCustom("My Hero", "Story", "Cryo", "Sword", 5,
                "https://example.com/hero.png");
            original.Save(file);

            CharacterCatalog restored = new(new FakeApi(), new JsonDataStorage());
            Assert.True(restored.Load(file));
            CatalogCharacter? character = Assert.Single(restored.Characters);
            Assert.Equal("My Hero", character.Name);
            Assert.Equal(5, character.Rarity);
            Assert.True(character.IsCustom);
        }
        finally
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}

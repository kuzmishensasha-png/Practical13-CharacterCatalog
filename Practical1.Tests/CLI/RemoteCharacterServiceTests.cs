using Practical1.CLI;
using Practical1.GameSystem;
using Practical1.Infra.Remote;

namespace Practical1.Tests.CLI;

public class RemoteCharacterServiceTests
{
    private class FakeLogger : ILogger
    {
        public void Log(string message)
        {
        }
    }

    private class FakeApiClient : IGenshinApiClient
    {
        public Task<IReadOnlyList<string>>
            GetCharacterIdsAsync()
        {
            return Task.FromResult<
                IReadOnlyList<string>
            >(new List<string>());
        }

        public Task<RemoteCharacterData?>
            GetCharacterAsync(
                string id)
        {
            RemoteCharacterData data =
                new RemoteCharacterData
                {
                    Name = "Amber",
                    Description = "Energetic Outrider.",
                    Rarity = 4,
                    Vision = "Pyro",
                    Weapon = "Bow"
                };

            return Task.FromResult<
                RemoteCharacterData?
            >(data);
        }

        public Task<IReadOnlyList<string>>
            GetWeaponIdsAsync()
        {
            return Task.FromResult<
                IReadOnlyList<string>
            >(new List<string>());
        }

        public Task<RemoteWeaponData?>
            GetWeaponAsync(
                string id)
        {
            RemoteWeaponData data =
                new RemoteWeaponData
                {
                    Name = "Cool Steel",
                    Type = "Sword",
                    Rarity = 3,
                    BaseAttack = 39
                };

            return Task.FromResult<
                RemoteWeaponData?
            >(data);
        }
    }

    [Fact]
    public async Task CreateCharacterAsync_CreatesCharacter()
    {
        Game game =
            new Game(
                new FakeLogger()
            );

        RemoteCharacterService service =
            new RemoteCharacterService(
                new FakeApiClient(),
                new CharacterMapper(game),
                new WeaponMapper(),
                game
            );

        Character? result =
            await service.CreateCharacterAsync(
                "amber"
            );

        Assert.NotNull(result);

        Assert.Equal(
            "Amber",
            result.Name
        );
    }

    [Fact]
    public async Task EquipWeaponAsync_EquipsWeapon()
    {
        Game game =
            new Game(
                new FakeLogger()
            );

        RemoteCharacterService service =
            new RemoteCharacterService(
                new FakeApiClient(),
                new CharacterMapper(game),
                new WeaponMapper(),
                game
            );

        await service.CreateCharacterAsync(
            "amber"
        );

        await service.EquipWeaponAsync(
            "cool-steel"
        );

        Assert.NotNull(
            service.CurrentCharacter
        );

        Assert.Single(
            service.CurrentCharacter.Items
        );

        Assert.Equal(
            "Cool Steel",
            service.CurrentCharacter.Items[0].Name
        );
    }
}
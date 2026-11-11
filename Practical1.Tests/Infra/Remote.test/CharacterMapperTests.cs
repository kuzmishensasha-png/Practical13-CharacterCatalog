using Practical1.GameSystem;
using Practical1.Infra.Remote;

namespace Practical1.Tests.Infra.Remote;

public class CharacterMapperTests
{
    private class FakeLogger : ILogger
    {
        public void Log(string message)
        {
        }
    }

    [Fact]
    public void Map_CreatesCharacterWithAbilities()
    {
        Game game =
            new Game(
                new FakeLogger()
            );

        CharacterMapper mapper =
            new CharacterMapper(
                game
            );

        RemoteCharacterData data =
            new RemoteCharacterData
            {
                Name = "Amber",
                Description = "Always energetic and full of life.",
                Rarity = 4,
                Vision = "Pyro",
                Weapon = "Bow",
                SkillTalents =
                {
                    new RemoteTalentData
                    {
                        Name = "Sharpshooter",
                        Description = "Performs normal attacks."
                    },
                    new RemoteTalentData
                    {
                        Name = "Explosive Puppet",
                        Description = "Deploys Baron Bunny."
                    }
                }
            };

        Character result =
            mapper.Map(data);

        Assert.Equal(
            "Amber",
            result.Name
        );

        Assert.Equal(
            "Always energetic and full of life.",
            result.Description
        );

        Assert.Equal(
            4,
            result.Rarity
        );

        Assert.Equal(
            "Pyro",
            result.Vision
        );

        Assert.Equal(
            "Bow",
            result.WeaponType
        );

        Assert.Equal(
            180,
            result.Health
        );

        Assert.Equal(
            8,
            result.Armor
        );

        Assert.Equal(
            40,
            result.AttackPower
        );

        Assert.Equal(
            2,
            result.Abilities.Count
        );

        Assert.Equal(
            "Sharpshooter",
            result.Abilities[0].Name
        );

        Assert.Equal(
            "Performs normal attacks.",
            result.Abilities[0].Description
        );

        Assert.Equal(
            "Explosive Puppet",
            result.Abilities[1].Name
        );

        Assert.Equal(
            "Deploys Baron Bunny.",
            result.Abilities[1].Description
        );
    }
}
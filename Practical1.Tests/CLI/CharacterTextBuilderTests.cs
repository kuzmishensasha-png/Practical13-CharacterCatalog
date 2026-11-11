using Practical1.CLI;
using Practical1.GameSystem;

namespace Practical1.Tests.CLI;

public class CharacterTextBuilderTests
{
    private class FakeLogger : ILogger
    {
        public void Log(string message)
        {
        }
    }

    [Fact]
    public void Build_CreatesCharacterText()
    {
        Game game =
            new Game(
                new FakeLogger()
            );

        Warrior character =
            new Warrior(
                "Amber",
                180,
                8,
                40
            );

        character.Description =
            "Always energetic and full of life.";

        character.Rarity = 4;
        character.Vision = "Pyro";
        character.WeaponType = "Bow";

        Ability ability =
            new Ability(
                "Sharpshooter",
                20,
                "Performs normal attacks."
            );

        Item weapon =
            new Item(
                "Cool Steel",
                0,
                39,
                "Sword",
                3,
                "Bane of Water and Ice",
                "Increases damage."
            );

        game.AddAbility(
            character,
            ability
        );

        game.Equip(
            character,
            weapon
        );

        CharacterTextBuilder builder =
            new CharacterTextBuilder();

        TextRenderer renderer =
            new TextRenderer();

        string result =
            renderer.Render(
                builder.Build(character)
            );

        Assert.Contains(
            "# Amber",
            result
        );

        Assert.Contains(
            "Always energetic and full of life.",
            result
        );

        Assert.Contains(
            "Rarity: 4",
            result
        );

        Assert.Contains(
            "Vision: Pyro",
            result
        );

        Assert.Contains(
            "Health: 180",
            result
        );

        Assert.Contains(
            "Sharpshooter",
            result
        );

        Assert.Contains(
            "Cool Steel",
            result
        );
    }
}
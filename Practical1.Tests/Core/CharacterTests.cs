using Practical1.GameSystem;

namespace Practical1.Tests.Core;

public class CharacterTests
{
    private class FakeLogger : ILogger
    {
        public void Log(string message)
        {

        }
    }

    [Fact]
    public void Attack_ReducesTargetHealth()
    {

        ILogger logger = new FakeLogger();
        Game game = new Game(logger);

        Warrior attacker =
            new Warrior(
                "Arthur",
                100,
                0,
                30
            );

        Mage target =
            new Mage(
                "Merlin",
                100,
                0,
                20
            );


        game.Attack(
            attacker,
            target
        );


        Assert.Equal(
            70,
            target.Health
        );
    }

    [Fact]
    public void Heal_DoesNotExceedMaxHealth()
    {

        ILogger logger = new FakeLogger();
        Game game = new Game(logger);

        Warrior attacker =
            new Warrior(
                "Arthur",
                100,
                0,
                30
            );

        Mage target =
            new Mage(
                "Merlin",
                100,
                0,
                20
            );

        game.Attack(
            attacker,
            target
        );


        game.Heal(
            target,
            100
        );


        Assert.Equal(
            100,
            target.Health
        );
    }

    [Fact]
    public void Equip_AddsItemBonuses()
    {

        ILogger logger = new FakeLogger();
        Game game = new Game(logger);

        Warrior warrior =
            new Warrior(
                "Arthur",
                100,
                10,
                20
            );

        Item sword =
            new Item(
                "Sword",
                5,
                10
            );

        game.Equip(
            warrior,
            sword
        );

        Assert.Equal(
            15,
            warrior.Armor
        );

        Assert.Equal(
            30,
            warrior.AttackPower
        );

        Assert.Contains(
            sword,
            warrior.Items
        );
    }

    [Fact]
    public void Defend_ReducesNextAttackDamage()
    {

        ILogger logger = new FakeLogger();
        Game game = new Game(logger);

        Warrior attacker =
            new Warrior(
                "Arthur",
                100,
                0,
                40
            );

        Mage target =
            new Mage(
                "Merlin",
                100,
                0,
                20
            );


        game.Defend(target);

        game.Attack(
            attacker,
            target
        );


        Assert.Equal(
            80,
            target.Health
        );
    }
}
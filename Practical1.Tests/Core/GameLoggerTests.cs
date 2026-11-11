using Practical1.GameSystem;

namespace Practical1.Tests.Core;

public class GameLoggerTests
{
    private class MockLogger : ILogger
    {
        public int CallCount { get; private set; }

        public List<string> Messages { get; } =
            new List<string>();

        public void Log(string message)
        {
            CallCount++;
            Messages.Add(message);
        }
    }

    [Fact]
    public void Attack_LogsTwoMessages()
    {
        MockLogger logger =
            new MockLogger();

        Game game =
            new Game(logger);

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
            2,
            logger.CallCount
        );

        Assert.Contains(
            "Arthur attacks Merlin!",
            logger.Messages
        );
    }

    [Fact]
    public void Heal_LogsOneMessage()
    {
        MockLogger logger =
            new MockLogger();

        Game game =
            new Game(logger);

        Mage mage =
            new Mage(
                "Merlin",
                100,
                0,
                20
            );

        game.Heal(
            mage,
            10
        );

        Assert.Equal(
            1,
            logger.CallCount
        );

        Assert.Contains(
            logger.Messages,
            message =>
                message.Contains("Merlin heals for 10")
        );
    }

    [Fact]
    public void Equip_LogsOneMessage()
    {
        MockLogger logger =
            new MockLogger();

        Game game =
            new Game(logger);

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
            1,
            logger.CallCount
        );

        Assert.Contains(
            logger.Messages,
            message =>
                message.Contains("Arthur equips Sword")
        );
    }
}
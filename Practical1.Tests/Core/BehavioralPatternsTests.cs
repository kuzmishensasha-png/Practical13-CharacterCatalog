using Practical1.CLI;
using Practical1.GameSystem;

namespace Practical1.Tests.Core;

public class BehavioralPatternsTests
{
    private sealed class TestLogger : ILogger
    {
        public List<string> Messages { get; } = new();
        public void Log(string message) => Messages.Add(message);
    }

    private sealed class TestPresenter : IPresenter
    {
        private readonly Queue<string> inputs;

        public TestPresenter(params string[] inputs)
        {
            this.inputs = new Queue<string>(inputs);
        }

        public void Show(string text) { }
        public void ShowLine(string text) { }
        public string? Read() => inputs.Dequeue();
    }

    [Fact]
    public void Command_ExecutesRegisteredObjectsAndOldDelegates()
    {
        var interpreter = new CommandInterpreter(
            new CommandParser(), new TestPresenter("first", "second", "exit"));
        List<string> calls = new();

        interpreter.Register("first", new ActionCommand(request => calls.Add(request.Name)));
        interpreter.Register("second", request => calls.Add(request.Name));
        interpreter.Register("exit", request => interpreter.Stop());

        interpreter.Run();

        Assert.Equal(new[] { "first", "second" }, calls);
    }

    [Fact]
    public void Strategy_ChangesOnlyTheChosenAttackRule()
    {
        var game = new Game(new TestLogger());
        var attacker = new Warrior("Arthur", 100, 0, 30);
        var target = new Mage("Merlin", 100, 10, 20);

        game.Attack(attacker, target);
        Assert.Equal(80, target.Health);

        attacker.SetAttackStrategy(new PiercingDamageStrategy());
        game.Attack(attacker, target);
        Assert.Equal(50, target.Health);

        game.Defend(target);
        game.Attack(attacker, target);
        Assert.Equal(35, target.Health);
    }

    [Fact]
    public void Memento_RestoresHealthEquipmentAbilitiesAndDefense()
    {
        var game = new Game(new TestLogger());
        var hero = new Warrior("Arthur", 100, 5, 20);
        var enemy = new Mage("Merlin", 100, 0, 25);
        var sword = new Item("Sword", 2, 5);
        var ability = new Ability("Slash", 10);

        game.Defend(hero);
        CharacterMemento snapshot = hero.CreateMemento();

        game.Equip(hero, sword);
        game.AddAbility(hero, ability);
        hero.SetAttackStrategy(new PiercingDamageStrategy());
        game.Attack(enemy, hero);

        hero.RestoreMemento(snapshot);

        Assert.Equal(100, hero.Health);
        Assert.Equal(5, hero.Armor);
        Assert.Equal(20, hero.AttackPower);
        Assert.True(hero.IsDefending);
        Assert.Empty(hero.Items);
        Assert.Empty(hero.Abilities);
        Assert.IsType<NormalDamageStrategy>(hero.AttackStrategy);
        Assert.Throws<ArgumentException>(() => enemy.RestoreMemento(snapshot));
    }

    [Fact]
    public void Observer_NotifiesListenersAndPreservesOriginalLogger()
    {
        var logger = new TestLogger();
        var game = new Game(logger);
        var attacker = new Warrior("Arthur", 100, 0, 30);
        var target = new Mage("Merlin", 100, 0, 20);
        List<string> events = new();
        Action<string> listener = events.Add;

        game.MessagePublished += listener;
        game.Attack(attacker, target);
        game.MessagePublished -= listener;
        game.Heal(target, 10);

        Assert.Equal(2, events.Count);
        Assert.Equal(3, logger.Messages.Count);
        Assert.Contains("Arthur attacks Merlin!", events);
    }
}

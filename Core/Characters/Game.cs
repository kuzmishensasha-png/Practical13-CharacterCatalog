namespace Practical1.GameSystem;

public class Game
{
    // Observer: підписники отримують повідомлення про те, що сталося в грі.
    public event Action<string>? MessagePublished;

    public Game(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        MessagePublished += logger.Log;
    }

    private void Publish(string message) => MessagePublished?.Invoke(message);

    public void Attack(Character attacker, Character target)
    {
        Publish($"{attacker.Name} attacks {target.Name}!");

        int damage = attacker.Attack(target);

        Publish(
            $"{target.Name} takes {damage} damage. Health: {target.Health}/{target.MaxHealth}"
        );
    }

    public void UseAbility(Character user, Character target)
    {
        Publish(user.GetAbilityMessage(target));

        int damage = user.UseAbility(target);

        Publish(
            $"{target.Name} takes {damage} damage. Health: {target.Health}/{target.MaxHealth}"
        );
    }

    public void Defend(Character character)
    {
        Publish($"{character.Name} is defending.");
        character.Defend();
    }

    public void Heal(Character character, int amount)
    {
        character.Heal(amount);

        Publish(
            $"{character.Name} heals for {amount}. Health: {character.Health}/{character.MaxHealth}"
        );
    }

    public void Equip(Character character, Item item)
    {
        character.Equip(item);

        Publish(
            $"{character.Name} equips {item.Name}. Armor: {character.Armor}, Attack: {character.AttackPower}"
        );
    }

    public void AddAbility(Character character, Ability ability)
    {
        character.LearnAbility(ability);

        Publish(
            $"{character.Name} learned {ability.Name}."
        );
    }

    public void UseAbility(
        Character user,
        Character target,
        Ability ability)
    {
        int damage = ability.Use(user, target);

        Publish(
            $"{user.Name} uses {ability.Name} on {target.Name}!"
        );

        Publish(
            $"{target.Name} takes {damage} damage. Health: {target.Health}/{target.MaxHealth}"
        );
    }

    public void RestoreHealth(
        Character character,
        int health)
    {
        character.RestoreHealth(health);
    }
}

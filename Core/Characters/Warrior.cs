namespace Practical1.GameSystem;

public class Warrior : Character
{
    public Warrior(string name, int health, int armor, int attackPower)
        : base(name, health, armor, attackPower)
    {
    }

    internal override int UseAbility(Character target)
    {
        int damage = AttackPower + 15;

        return target.TakeDamage(damage);
    }

    internal override string GetAbilityMessage(Character target)
    {
        return $"{Name} uses Power Strike on {target.Name}!";
    }
}
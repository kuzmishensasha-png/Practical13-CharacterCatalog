namespace Practical1.GameSystem;

public class Mage : Character
{
    public Mage(string name, int health, int armor, int attackPower)
        : base(name, health, armor, attackPower)
    {
    }

    internal override int UseAbility(Character target)
    {
        int damage = AttackPower + 25;

        return target.TakeDamage(damage);
    }

    internal override string GetAbilityMessage(Character target)
    {
        return $"{Name} casts Fireball on {target.Name}!";
    }
}
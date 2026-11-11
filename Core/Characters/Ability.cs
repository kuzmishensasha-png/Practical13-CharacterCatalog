namespace Practical1.GameSystem;

public class Ability
{
    public string Id { get; } = Guid.NewGuid()
        .ToString("N")[..8];

    public string Name { get; }

    public int BonusDamage { get; }

    public string Description { get; }

    public Ability(
        string name,
        int bonusDamage,
        string description = "")
    {
        Name = name;
        BonusDamage = bonusDamage;
        Description = description;
    }

    internal int Use(
        Character user,
        Character target)
    {
        int damage =
            user.AttackPower + BonusDamage;

        return target.TakeDamage(
            damage
        );
    }
}
namespace Practical1.GameSystem;

public sealed class CharacterMemento
{
    internal Character Owner { get; }
    internal int Health { get; }
    internal int Armor { get; }
    internal int AttackPower { get; }
    internal bool IsDefending { get; }
    internal string Description { get; }
    internal int Rarity { get; }
    internal string Vision { get; }
    internal string WeaponType { get; }
    internal IDamageStrategy AttackStrategy { get; }
    internal IReadOnlyList<Item> Items { get; }
    internal IReadOnlyList<Ability> Abilities { get; }

    internal CharacterMemento(Character character)
    {
        Owner = character;
        Health = character.Health;
        Armor = character.Armor;
        AttackPower = character.AttackPower;
        IsDefending = character.IsDefending;
        Description = character.Description;
        Rarity = character.Rarity;
        Vision = character.Vision;
        WeaponType = character.WeaponType;
        AttackStrategy = character.AttackStrategy;
        Items = character.Items.ToArray();
        Abilities = character.Abilities.ToArray();
    }
}

namespace Practical1.GameSystem;

public class Item
{
    public string Id { get; } = Guid.NewGuid()
        .ToString("N")[..8];

    public string Name { get; }

    public int ArmorBonus { get; }

    public int AttackBonus { get; }

    public string WeaponType { get; }

    public int Rarity { get; }

    public string PassiveName { get; }

    public string Description { get; }

    public Item(
        string name,
        int armorBonus,
        int attackBonus,
        string weaponType = "",
        int rarity = 0,
        string passiveName = "",
        string description = "")
    {
        Name = name;
        ArmorBonus = armorBonus;
        AttackBonus = attackBonus;
        WeaponType = weaponType;
        Rarity = rarity;
        PassiveName = passiveName;
        Description = description;
    }
}
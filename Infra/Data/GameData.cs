namespace Practical1.Infra.Data;

public class GameData
{
    public List<CharacterData> Characters { get; set; } = new();
    public List<ItemData> Items { get; set; } = new();
    public List<AbilityData> Abilities { get; set; } = new();
}

public class CharacterData
{
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";

    public string AttackStyle { get; set; } = "normal";

    public int Health { get; set; }
    public int MaxHealth { get; set; }

    public int BaseArmor { get; set; }
    public int BaseAttackPower { get; set; }

    public List<string> Items { get; set; } = new();
    public List<string> Abilities { get; set; } = new();
}

public class ItemData
{
    public string Name { get; set; } = "";

    public int ArmorBonus { get; set; }
    public int AttackBonus { get; set; }
}

public class AbilityData
{
    public string Name { get; set; } = "";

    public int BonusDamage { get; set; }
}

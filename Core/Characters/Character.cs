namespace Practical1.GameSystem;

public abstract class Character
{
    private readonly List<Item> items = new();
    private readonly List<Ability> abilities = new();

    public string Id { get; } = Guid.NewGuid()
        .ToString("N")[..8];

    public IReadOnlyList<Item> Items => items;
    public IReadOnlyList<Ability> Abilities => abilities;

    public IDamageStrategy AttackStrategy { get; private set; } = new NormalDamageStrategy();
    public string Name { get; }

    public string Description { get; set; } = "";

    public int Rarity { get; set; }

    public string Vision { get; set; } = "";

    public string WeaponType { get; set; } = "";

    public int Health { get; protected set; }

    public int MaxHealth { get; }

    public int Armor { get; protected set; }

    public int AttackPower { get; protected set; }
    

    public bool IsDefending { get; private set; }
    
    protected Character(string name, int health, int armor, int attackPower)
    {
        Name = name;
        Health = health;
        MaxHealth = health;
        Armor = armor;
        AttackPower = attackPower;
    }

    internal int Attack(Character target)
    {
        return target.TakeDamage(AttackPower, AttackStrategy);
    }

    internal int TakeDamage(int damage, IDamageStrategy? strategy = null)
    {
        // Strategy: залежно від типу атаки шкода рахується по-різному.
        int finalDamage = (strategy ?? new NormalDamageStrategy())
            .Calculate(damage, Armor, IsDefending);

        if (IsDefending)
        {
            IsDefending = false;
        }

        Health = Math.Max(0, Health - finalDamage);

        return finalDamage;
    }

    internal void Heal(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }

    internal void Defend()
    {
        IsDefending = true;
    }

    internal abstract int UseAbility(Character target);
    internal abstract string GetAbilityMessage(Character target);

    internal void Equip(Item item)
    {
        if (items.Contains(item))
        {
            return;
        }

        items.Add(item);

        Armor += item.ArmorBonus;
        AttackPower += item.AttackBonus;
    }

    internal void LearnAbility(Ability ability)
    {
        if (!abilities.Contains(ability))
        {
            abilities.Add(ability);
        }
    }

    internal void RestoreHealth(int health)
    {
        Health = Math.Clamp(
            health,
            0,
            MaxHealth
        );
    }  

    public void SetAttackStrategy(IDamageStrategy strategy)
    {
        AttackStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    // Роблю знімок стану, щоб потім можна було все повернути назад.
    public CharacterMemento CreateMemento() => new(this);

    public void RestoreMemento(CharacterMemento memento)
    {
        ArgumentNullException.ThrowIfNull(memento);
        if (!ReferenceEquals(memento.Owner, this))
        {
            throw new ArgumentException("Snapshot belongs to another character.", nameof(memento));
        }

        Health = memento.Health;
        Armor = memento.Armor;
        AttackPower = memento.AttackPower;
        IsDefending = memento.IsDefending;
        Description = memento.Description;
        Rarity = memento.Rarity;
        Vision = memento.Vision;
        WeaponType = memento.WeaponType;
        AttackStrategy = memento.AttackStrategy;

        items.Clear();
        items.AddRange(memento.Items);
        abilities.Clear();
        abilities.AddRange(memento.Abilities);
    }
}

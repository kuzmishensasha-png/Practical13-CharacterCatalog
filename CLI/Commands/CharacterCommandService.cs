using Practical1.GameSystem;
using Practical1.Infra.Data;

namespace Practical1.CLI;

public class CharacterCommandService
{
    private readonly List<Character>
        characters = new();

    private readonly List<Item>
        items = new();

    private readonly List<Ability>
        abilities = new();

    // Memento: тут тримаю останній знімок кожного персонажа.
    private readonly Dictionary<Character, CharacterMemento> snapshots = new();

    private readonly Game game;

    private readonly IPresenter presenter;

    private readonly IDataStorage storage;

    public CharacterCommandService(
        IPresenter presenter,
        IDataStorage storage,
        Game game)
    {
        this.presenter = presenter;
        this.storage = storage;
        this.game = game;
    }

    public void Create(
        CommandRequest request)
    {
        if (request.Arguments.Count == 0)
        {
            presenter.ShowLine(
                "Usage: create <char|item|ability>"
            );

            return;
        }

        string type =
            request.Arguments[0].ToLower();

        if (type == "char")
        {
            CreateCharacter();
        }
        else if (type == "item")
        {
            CreateItem();
        }
        else if (type == "ability")
        {
            CreateAbility();
        }
        else
        {
            presenter.ShowLine(
                "Unknown object type."
            );
        }
    }

    public void Add(
        CommandRequest request)
    {
        if (request.Arguments.Count == 0)
        {
            presenter.ShowLine(
                "Usage: add <item|ability> --char_id <id/name> --id <id/name>"
            );

            return;
        }

        if (!request.Options.TryGetValue(
            "char_id",
            out string? charId)
            || string.IsNullOrWhiteSpace(
                charId))
        {
            presenter.ShowLine(
                "--char_id is required."
            );

            return;
        }

        if (!request.Options.TryGetValue(
            "id",
            out string? objectId)
            || string.IsNullOrWhiteSpace(
                objectId))
        {
            presenter.ShowLine(
                "--id is required."
            );

            return;
        }

        Character? character =
            FindCharacter(charId);

        if (character == null)
        {
            presenter.ShowLine(
                "Character not found."
            );

            return;
        }

        string type =
            request.Arguments[0].ToLower();

        if (type == "item")
        {
            Item? item =
                FindItem(objectId);

            if (item == null)
            {
                presenter.ShowLine(
                    "Item not found."
                );

                return;
            }

            game.Equip(
                character,
                item
            );
        }
        else if (type == "ability")
        {
            Ability? ability =
                FindAbility(objectId);

            if (ability == null)
            {
                presenter.ShowLine(
                    "Ability not found."
                );

                return;
            }

            game.AddAbility(
                character,
                ability
            );
        }
        else
        {
            presenter.ShowLine(
                "Use: add item ... or add ability ..."
            );
        }
    }

    public void Act(
        CommandRequest request)
    {
        if (request.Arguments.Count < 3)
        {
            presenter.ShowLine(
                "Usage: act <attack|heal|ability> <actor> <target> [--id <ability>]"
            );

            return;
        }

        string action =
            request.Arguments[0];

        Character? actor =
            FindCharacter(
                request.Arguments[1]
            );

        Character? target =
            FindCharacter(
                request.Arguments[2]
            );

        if (actor == null ||
            target == null)
        {
            presenter.ShowLine(
                "Character not found."
            );

            return;
        }

        if (action == "attack")
        {
            game.Attack(
                actor,
                target
            );
        }
        else if (action == "heal")
        {
            presenter.Show(
                "Heal amount: "
            );

            if (!int.TryParse(
                presenter.Read(),
                out int amount))
            {
                presenter.ShowLine(
                    "Invalid amount."
                );

                return;
            }

            game.Heal(
                target,
                amount
            );
        }
        else if (action == "ability")
        {
            if (!request.Options.TryGetValue(
                "id",
                out string? abilityId)
                || string.IsNullOrWhiteSpace(
                    abilityId))
            {
                presenter.ShowLine(
                    "--id <ability> is required."
                );

                return;
            }

            Ability? ability =
                FindAbility(abilityId);

            if (ability == null)
            {
                presenter.ShowLine(
                    "Ability not found."
                );

                return;
            }

            if (!actor.Abilities.Contains(
                ability))
            {
                presenter.ShowLine(
                    "This character does not have that ability."
                );

                return;
            }

            game.UseAbility(
                actor,
                target,
                ability
            );
        }
        else
        {
            presenter.ShowLine(
                "Unknown action."
            );
        }
    }

    public void List(
        CommandRequest request)
    {
        if (request.Arguments.Count == 0)
        {
            presenter.ShowLine(
                "Usage: ls <char|item|ability> [--id <id/name>]"
            );

            return;
        }

        string type =
            request.Arguments[0].ToLower();

        request.Options.TryGetValue(
            "id",
            out string? id
        );

        if (type == "char")
        {
            ListCharacters(id);
        }
        else if (type == "item")
        {
            ListItems(id);
        }
        else if (type == "ability")
        {
            ListAbilities(id);
        }
        else
        {
            presenter.ShowLine(
                "Unknown category."
            );
        }
    }

    public void Snapshot(CommandRequest request)
    {
        if (request.Arguments.Count == 0)
        {
            presenter.ShowLine("Usage: snapshot <character>");
            return;
        }

        Character? character = FindCharacter(request.Arguments[0]);
        if (character == null)
        {
            presenter.ShowLine("Character not found.");
            return;
        }

        snapshots[character] = character.CreateMemento();
        presenter.ShowLine($"Snapshot created for {character.Name}.");
    }

    public void Restore(CommandRequest request)
    {
        if (request.Arguments.Count == 0)
        {
            presenter.ShowLine("Usage: restore <character>");
            return;
        }

        Character? character = FindCharacter(request.Arguments[0]);
        if (character == null)
        {
            presenter.ShowLine("Character not found.");
            return;
        }

        if (!snapshots.TryGetValue(character, out CharacterMemento? snapshot))
        {
            presenter.ShowLine("No snapshot for this character.");
            return;
        }

        character.RestoreMemento(snapshot);
        presenter.ShowLine($"{character.Name} restored from snapshot.");
    }

    public void Strategy(CommandRequest request)
    {
        if (request.Arguments.Count < 2)
        {
            presenter.ShowLine("Usage: strategy <character> <normal|piercing>");
            return;
        }

        Character? character = FindCharacter(request.Arguments[0]);
        if (character == null)
        {
            presenter.ShowLine("Character not found.");
            return;
        }

        IDamageStrategy strategy;
        if (request.Arguments[1].Equals("normal", StringComparison.OrdinalIgnoreCase))
        {
            strategy = new NormalDamageStrategy();
        }
        else if (request.Arguments[1].Equals("piercing", StringComparison.OrdinalIgnoreCase))
        {
            strategy = new PiercingDamageStrategy();
        }
        else
        {
            presenter.ShowLine("Use normal or piercing.");
            return;
        }

        character.SetAttackStrategy(strategy);
        presenter.ShowLine($"{character.Name} attack strategy updated.");
    }

    private void CreateCharacter()
    {
        string type =
            ReadRequired(
                "Type (warrior/mage): "
            );

        string name =
            ReadRequired("Name: ");

        int health =
            ReadInt("Health: ");

        int armor =
            ReadInt("Armor: ");

        int attack =
            ReadInt("Attack power: ");

        Character character;

        if (type.Equals(
            "mage",
            StringComparison.OrdinalIgnoreCase))
        {
            character =
                new Mage(
                    name,
                    health,
                    armor,
                    attack
                );
        }
        else
        {
            character =
                new Warrior(
                    name,
                    health,
                    armor,
                    attack
                );
        }

        characters.Add(character);

        presenter.ShowLine(
            $"Character created. ID: {character.Id}"
        );
    }

    private void CreateItem()
    {
        string name =
            ReadRequired("Name: ");

        int armor =
            ReadInt("Armor bonus: ");

        int attack =
            ReadInt("Attack bonus: ");

        Item item =
            new Item(
                name,
                armor,
                attack
            );

        items.Add(item);

        presenter.ShowLine(
            $"Item created. ID: {item.Id}"
        );
    }

    private void CreateAbility()
    {
        string name =
            ReadRequired("Name: ");

        int damage =
            ReadInt("Bonus damage: ");

        Ability ability =
            new Ability(
                name,
                damage
            );

        abilities.Add(ability);

        presenter.ShowLine(
            $"Ability created. ID: {ability.Id}"
        );
    }

    private void ListCharacters(
        string? id)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            Character? character =
                FindCharacter(id);

            if (character == null)
            {
                presenter.ShowLine(
                    "Character not found."
                );

                return;
            }

            presenter.ShowLine(
                $"ID: {character.Id}"
            );

            presenter.ShowLine(
                $"Name: {character.Name}"
            );

            presenter.ShowLine(
                $"Health: {character.Health}/{character.MaxHealth}"
            );

            presenter.ShowLine(
                $"Armor: {character.Armor}"
            );

            presenter.ShowLine(
                $"Attack: {character.AttackPower}"
            );

            presenter.ShowLine(
                "Items: "
                + string.Join(
                    ", ",
                    character.Items.Select(
                        item => item.Name
                    )
                )
            );

            presenter.ShowLine(
                "Abilities: "
                + string.Join(
                    ", ",
                    character.Abilities.Select(
                        ability =>
                            ability.Name
                    )
                )
            );

            return;
        }

        foreach (Character character
                 in characters)
        {
            presenter.ShowLine(
                $"[{character.Id}] {character.Name} - HP {character.Health}/{character.MaxHealth}"
            );
        }
    }

    private void ListItems(
        string? id)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            Item? item =
                FindItem(id);

            if (item == null)
            {
                presenter.ShowLine(
                    "Item not found."
                );

                return;
            }

            presenter.ShowLine(
                $"ID: {item.Id}"
            );

            presenter.ShowLine(
                $"Name: {item.Name}"
            );

            presenter.ShowLine(
                $"Armor bonus: {item.ArmorBonus}"
            );

            presenter.ShowLine(
                $"Attack bonus: {item.AttackBonus}"
            );

            presenter.ShowLine(
                "Used by: "
                + string.Join(
                    ", ",
                    characters
                        .Where(
                            c =>
                                c.Items
                                    .Contains(item)
                        )
                        .Select(
                            c => c.Name
                        )
                )
            );

            return;
        }

        foreach (Item item in items)
        {
            string users =
                string.Join(
                    ", ",
                    characters
                        .Where(
                            c =>
                                c.Items
                                    .Contains(item)
                        )
                        .Select(
                            c => c.Name
                        )
                );

            presenter.ShowLine(
                $"[{item.Id}] {item.Name} - users: {users}"
            );
        }
    }

    private void ListAbilities(
        string? id)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            Ability? ability =
                FindAbility(id);

            if (ability == null)
            {
                presenter.ShowLine(
                    "Ability not found."
                );

                return;
            }

            presenter.ShowLine(
                $"ID: {ability.Id}"
            );

            presenter.ShowLine(
                $"Name: {ability.Name}"
            );

            presenter.ShowLine(
                $"Bonus damage: {ability.BonusDamage}"
            );

            presenter.ShowLine(
                "Used by: "
                + string.Join(
                    ", ",
                    characters
                        .Where(
                            c =>
                                c.Abilities
                                    .Contains(ability)
                        )
                        .Select(
                            c => c.Name
                        )
                )
            );

            return;
        }

        foreach (Ability ability
                 in abilities)
        {
            string users =
                string.Join(
                    ", ",
                    characters
                        .Where(
                            c =>
                                c.Abilities
                                    .Contains(ability)
                        )
                        .Select(
                            c => c.Name
                        )
                );

            presenter.ShowLine(
                $"[{ability.Id}] {ability.Name} - users: {users}"
            );
        }
    }

    private Character? FindCharacter(
        string value)
    {
        return characters.FirstOrDefault(
            character =>
                string.Equals(
                    character.Id,
                    value,
                    StringComparison
                        .OrdinalIgnoreCase
                )
                ||
                string.Equals(
                    character.Name,
                    value,
                    StringComparison
                        .OrdinalIgnoreCase
                )
        );
    }

    private Item? FindItem(
        string value)
    {
        return items.FirstOrDefault(
            item =>
                string.Equals(
                    item.Id,
                    value,
                    StringComparison
                        .OrdinalIgnoreCase
                )
                ||
                string.Equals(
                    item.Name,
                    value,
                    StringComparison
                        .OrdinalIgnoreCase
                )
        );
    }

    private Ability? FindAbility(
        string value)
    {
        return abilities.FirstOrDefault(
            ability =>
                string.Equals(
                    ability.Id,
                    value,
                    StringComparison
                        .OrdinalIgnoreCase
                )
                ||
                string.Equals(
                    ability.Name,
                    value,
                    StringComparison
                        .OrdinalIgnoreCase
                )
        );
    }

    private string ReadRequired(
        string message)
    {
        while (true)
        {
            presenter.Show(message);

            string? value =
                presenter.Read();

            if (!string.IsNullOrWhiteSpace(
                value))
            {
                return value;
            }
        }
    }

    private int ReadInt(
        string message)
    {
        while (true)
        {
            presenter.Show(message);

            if (int.TryParse(
                presenter.Read(),
                out int value))
            {
                return value;
            }

            presenter.ShowLine(
                "Enter a number."
            );
        }
    }

    public void Save(CommandRequest request)
    {
        string fileName = "game.json";

        if (request.Arguments.Count > 0)
        {
            fileName = request.Arguments[0];
        }

        GameData data = new GameData();

        foreach (Item item in items)
        {
            data.Items.Add(
                new ItemData
                {
                    Name = item.Name,
                    ArmorBonus = item.ArmorBonus,
                    AttackBonus = item.AttackBonus
                }
            );
        }

        foreach (Ability ability in abilities)
        {
            data.Abilities.Add(
                new AbilityData
                {
                    Name = ability.Name,
                    BonusDamage = ability.BonusDamage
                }
            );
        }

        foreach (Character character in characters)
        {
            int itemArmor =
                character.Items.Sum(
                    item => item.ArmorBonus
                );

            int itemAttack =
                character.Items.Sum(
                    item => item.AttackBonus
                );

            CharacterData characterData =
                new CharacterData
                {
                    Type = character is Mage
                        ? "mage"
                        : "warrior",

                    Name = character.Name,

                    Health = character.Health,
                    MaxHealth = character.MaxHealth,

                    AttackStyle = character.AttackStrategy is PiercingDamageStrategy
                        ? "piercing" : "normal",

                    BaseArmor =
                        character.Armor - itemArmor,

                    BaseAttackPower =
                        character.AttackPower - itemAttack
                };

            foreach (Item item in character.Items)
            {
                characterData.Items.Add(
                    item.Name
                );
            }

            foreach (Ability ability
                     in character.Abilities)
            {
                characterData.Abilities.Add(
                    ability.Name
                );
            }

            data.Characters.Add(
                characterData
            );
        }

        storage.Save(
            fileName,
            data
        );

        presenter.ShowLine(
            $"Game saved to {fileName}"
        );
    }

    public void Load(CommandRequest request)
    {
        string fileName = "game.json";

        if (request.Arguments.Count > 0)
        {
            fileName = request.Arguments[0];
        }

        GameData? data =
            storage.Load<GameData>(
                fileName
            );

        if (data == null)
        {
            presenter.ShowLine(
                "Save file not found."
            );

            return;
        }

        characters.Clear();
        items.Clear();
        abilities.Clear();
        snapshots.Clear();

        foreach (ItemData itemData
                 in data.Items)
        {
            Item item =
                new Item(
                    itemData.Name,
                    itemData.ArmorBonus,
                    itemData.AttackBonus
                );

            items.Add(item);
        }

        foreach (AbilityData abilityData
                 in data.Abilities)
        {
            Ability ability =
                new Ability(
                    abilityData.Name,
                    abilityData.BonusDamage
                );

            abilities.Add(ability);
        }

        foreach (CharacterData characterData
                 in data.Characters)
        {
            Character character;

            if (characterData.Type == "mage")
            {
                character =
                    new Mage(
                        characterData.Name,
                        characterData.MaxHealth,
                        characterData.BaseArmor,
                        characterData.BaseAttackPower
                    );
            }
            else
            {
                character =
                    new Warrior(
                        characterData.Name,
                        characterData.MaxHealth,
                        characterData.BaseArmor,
                        characterData.BaseAttackPower
                    );
            }

            characters.Add(character);

            if (characterData.AttackStyle == "piercing")
            {
                character.SetAttackStrategy(new PiercingDamageStrategy());
            }

            foreach (string itemName
                     in characterData.Items)
            {
                Item? item =
                    FindItem(itemName);

                if (item != null)
                {
                    game.Equip(
                        character,
                        item
                    );
                }
            }

            foreach (string abilityName
                     in characterData.Abilities)
            {
                Ability? ability =
                    FindAbility(abilityName);

                if (ability != null)
                {
                    game.AddAbility(
                        character,
                        ability
                    );
                }
            }

            game.RestoreHealth(
                character,
                characterData.Health
            );
        }

        presenter.ShowLine(
            $"Game loaded from {fileName}"
        );
    }
}

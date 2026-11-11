using Practical1.GameSystem;
using Practical1.TextSystem;

namespace Practical1.CLI;

public class CharacterTextBuilder
{
    public Text Build(
        Character character)
    {
        Text document =
            new Text();

        document.Add(
            new Heading(
                character.Name,
                1
            )
        );

        document.Add(
            new Paragraph(
                character.Description
            )
        );

        document.Add(
            new Heading(
                "Character information",
                2
            )
        );

        document.Add(
            new Paragraph(
                $"Rarity: {character.Rarity}"
            )
        );

        document.Add(
            new Paragraph(
                $"Vision: {character.Vision}"
            )
        );

        document.Add(
            new Paragraph(
                $"Weapon type: {character.WeaponType}"
            )
        );

        document.Add(
            new Paragraph(
                $"Health: {character.Health}/{character.MaxHealth}"
            )
        );

        document.Add(
            new Paragraph(
                $"Armor: {character.Armor}"
            )
        );

        document.Add(
            new Paragraph(
                $"Attack: {character.AttackPower}"
            )
        );

        document.Add(
            new Heading(
                "Abilities",
                2
            )
        );

        foreach (
            Ability ability
            in character.Abilities)
        {
            document.Add(
                new Heading(
                    ability.Name,
                    3
                )
            );

            document.Add(
                new Paragraph(
                    ability.Description
                )
            );
        }

        document.Add(
            new Heading(
                "Equipment",
                2
            )
        );

        foreach (
            Item item
            in character.Items)
        {
            document.Add(
                new Paragraph(
                    $"{item.Name} | "
                    + $"{item.WeaponType} | "
                    + $"Rarity: {item.Rarity} | "
                    + $"Attack bonus: {item.AttackBonus}"
                )
            );
        }

        return document;
    }
}
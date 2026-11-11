using Practical1.GameSystem;

namespace Practical1.Infra.Remote;

public class CharacterMapper
{
    private readonly Game game;

    public CharacterMapper(
        Game game)
    {
        this.game = game;
    }

    public Character Map(
        RemoteCharacterData data)
    {
        int health =
            100 + data.Rarity * 20;

        int armor =
            data.Rarity * 2;

        int attackPower =
            20 + data.Rarity * 5;

        Character character;

        if (data.Weapon == "Catalyst")
        {
            character =
                new Mage(
                    data.Name,
                    health,
                    armor,
                    attackPower
                );
        }
        else
        {
            character =
                new Warrior(
                    data.Name,
                    health,
                    armor,
                    attackPower
                );
        }

        character.Description =
            data.Description;

        character.Rarity =
            data.Rarity;

        character.Vision =
            data.Vision;

        character.WeaponType =
            data.Weapon;

        foreach (
            RemoteTalentData talent
            in data.SkillTalents)
        {
            Ability ability =
                new Ability(
                    talent.Name,
                    data.Rarity * 5,
                    talent.Description
                );

            game.AddAbility(
                character,
                ability
            );
        }

        return character;
    }
}
using Practical1.GameSystem;
using Practical1.Infra.Remote;

namespace Practical1.CLI;

public class RemoteCharacterService
{
    private readonly IGenshinApiClient api;
    private readonly CharacterMapper characterMapper;
    private readonly WeaponMapper weaponMapper;
    private readonly Game game;

    public Character? CurrentCharacter
    {
        get;
        private set;
    }

    public RemoteCharacterService(
        IGenshinApiClient api,
        CharacterMapper characterMapper,
        WeaponMapper weaponMapper,
        Game game)
    {
        this.api = api;
        this.characterMapper = characterMapper;
        this.weaponMapper = weaponMapper;
        this.game = game;
    }

    public async Task<Character?>
        CreateCharacterAsync(
            string id)
    {
        RemoteCharacterData? data =
            await api.GetCharacterAsync(
                id
            );

        if (data == null)
        {
            return null;
        }

        CurrentCharacter =
            characterMapper.Map(
                data
            );

        return CurrentCharacter;
    }

    public async Task<Item?>
        EquipWeaponAsync(
            string id)
    {
        if (CurrentCharacter == null)
        {
            return null;
        }

        RemoteWeaponData? data =
            await api.GetWeaponAsync(
                id
            );

        if (data == null)
        {
            return null;
        }

        Item item =
            weaponMapper.Map(
                data
            );

        game.Equip(
            CurrentCharacter,
            item
        );

        return item;
    }
}
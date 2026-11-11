using Practical1.GameSystem;

namespace Practical1.Infra.Remote;

public class WeaponMapper
{
    public Item Map(
        RemoteWeaponData data)
    {
        return new Item(
            data.Name,
            0,
            data.BaseAttack,
            data.Type,
            data.Rarity,
            data.PassiveName,
            data.PassiveDesc
        );
    }
}
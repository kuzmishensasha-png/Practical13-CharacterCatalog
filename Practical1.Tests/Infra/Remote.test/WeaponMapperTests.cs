using Practical1.GameSystem;
using Practical1.Infra.Remote;

namespace Practical1.Tests.Infra.Remote;

public class WeaponMapperTests
{
    [Fact]
    public void Map_CreatesItemFromWeaponData()
    {
        RemoteWeaponData data =
            new RemoteWeaponData
            {
                Name = "Cool Steel",
                Type = "Sword",
                Rarity = 3,
                BaseAttack = 39,
                PassiveName = "Bane of Water and Ice",
                PassiveDesc =
                    "Increases damage against enemies affected by Hydro or Cryo."
            };

        WeaponMapper mapper =
            new WeaponMapper();

        Item result =
            mapper.Map(data);

        Assert.Equal(
            "Cool Steel",
            result.Name
        );

        Assert.Equal(
            "Sword",
            result.WeaponType
        );

        Assert.Equal(
            3,
            result.Rarity
        );

        Assert.Equal(
            39,
            result.AttackBonus
        );

        Assert.Equal(
            0,
            result.ArmorBonus
        );

        Assert.Equal(
            "Bane of Water and Ice",
            result.PassiveName
        );
    }
}
namespace Practical1.Infra.Remote;

public interface IGenshinApiClient
{
    Task<IReadOnlyList<string>>
        GetCharacterIdsAsync();

    Task<RemoteCharacterData?>
        GetCharacterAsync(
            string id
        );

    Task<IReadOnlyList<string>>
        GetWeaponIdsAsync();

    Task<RemoteWeaponData?>
        GetWeaponAsync(
            string id
        );
}
using System.Net.Http.Json;

namespace Practical1.Infra.Remote;

public class GenshinApiClient
    : IGenshinApiClient
{
    private readonly HttpClient httpClient;

    public GenshinApiClient(
        HttpClient httpClient)
    {
        this.httpClient =
            httpClient;
    }

    public async Task<IReadOnlyList<string>>
        GetCharacterIdsAsync()
    {
        List<string>? characters =
            await httpClient
                .GetFromJsonAsync<List<string>>(
                    "characters"
                );

        return characters
            ?? new List<string>();
    }

    public async Task<RemoteCharacterData?>
        GetCharacterAsync(
            string id)
    {
        return await httpClient
            .GetFromJsonAsync<RemoteCharacterData>(
                $"characters/{id}"
            );
    }

    public async Task<IReadOnlyList<string>>
        GetWeaponIdsAsync()
    {
        List<string>? weapons =
            await httpClient
                .GetFromJsonAsync<List<string>>(
                    "weapons"
                );

        return weapons
            ?? new List<string>();
    }

    public async Task<RemoteWeaponData?>
        GetWeaponAsync(
            string id)
    {
        return await httpClient
            .GetFromJsonAsync<RemoteWeaponData>(
                $"weapons/{id}"
            );
    }
}
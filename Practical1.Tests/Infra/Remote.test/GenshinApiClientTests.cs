using System.Net;
using System.Net.Http;
using System.Text;
using Practical1.Infra.Remote;

namespace Practical1.Tests.Infra.Remote;

public class GenshinApiClientTests
{
    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string json;

        public FakeHttpMessageHandler(string json)
        {
            this.json = json;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response =
                new HttpResponseMessage(
                    HttpStatusCode.OK
                );

            response.Content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

            return Task.FromResult(
                response
            );
        }
    }

    [Fact]
    public async Task GetCharacterIdsAsync_ReturnsCharacters()
    {
        HttpClient httpClient =
            new HttpClient(
                new FakeHttpMessageHandler(
                    "[\"amber\",\"kaeya\"]"
                )
            );

        httpClient.BaseAddress =
            new Uri(
                "https://genshin.jmp.blue/"
            );

        GenshinApiClient client =
            new GenshinApiClient(
                httpClient
            );

        IReadOnlyList<string> result =
            await client.GetCharacterIdsAsync();

        Assert.Equal(
            2,
            result.Count
        );

        Assert.Equal(
            "amber",
            result[0]
        );

        Assert.Equal(
            "kaeya",
            result[1]
        );
    }

    [Fact]
    public async Task GetCharacterAsync_ReturnsCharacterData()
    {
        string json =
            """
            {
                "name": "Amber",
                "description": "Always energetic and full of life.",
                "rarity": 4,
                "vision": "Pyro",
                "weapon": "Bow"
            }
            """;

        HttpClient httpClient =
            new HttpClient(
                new FakeHttpMessageHandler(
                    json
                )
            );

        httpClient.BaseAddress =
            new Uri(
                "https://genshin.jmp.blue/"
            );

        GenshinApiClient client =
            new GenshinApiClient(
                httpClient
            );

        var result =
            await client.GetCharacterAsync(
                "amber"
            );

        Assert.NotNull(result);

        Assert.Equal(
            "Amber",
            result.Name
        );

        Assert.Equal(
            4,
            result.Rarity
        );

        Assert.Equal(
            "Pyro",
            result.Vision
        );

        Assert.Equal(
            "Bow",
            result.Weapon
        );
    }

    [Fact]
    public async Task GetCharacterAsync_ReturnsSkillTalents()
    {
        string json =
            """
            {
                "name": "Amber",
                "description": "Always energetic and full of life.",
                "rarity": 4,
                "vision": "Pyro",
                "weapon": "Bow",
                "skillTalents": [
                    {
                        "name": "Sharpshooter",
                        "description": "Performs normal attacks."
                    },
                    {
                        "name": "Explosive Puppet",
                        "description": "Deploys Baron Bunny."
                    }
                ]
            }
            """;

        HttpClient httpClient =
            new HttpClient(
                new FakeHttpMessageHandler(
                    json
                )
            );

        httpClient.BaseAddress =
            new Uri(
                "https://genshin.jmp.blue/"
            );

        GenshinApiClient client =
            new GenshinApiClient(
                httpClient
            );

        RemoteCharacterData? result =
            await client.GetCharacterAsync(
                "amber"
            );

        Assert.NotNull(result);

        Assert.Equal(
            2,
            result.SkillTalents.Count
        );

        Assert.Equal(
            "Sharpshooter",
            result.SkillTalents[0].Name
        );

        Assert.Equal(
            "Explosive Puppet",
            result.SkillTalents[1].Name
        );
    }

    [Fact]
    public async Task GetWeaponIdsAsync_ReturnsWeapons()
    {
        HttpClient httpClient =
            new HttpClient(
                new FakeHttpMessageHandler(
                    "[\"cool-steel\",\"amos-bow\"]"
                )
            );

        httpClient.BaseAddress =
            new Uri(
                "https://genshin.jmp.blue/"
            );

        GenshinApiClient client =
            new GenshinApiClient(
                httpClient
            );

        IReadOnlyList<string> result =
            await client.GetWeaponIdsAsync();

        Assert.Equal(
            2,
            result.Count
        );

        Assert.Equal(
            "cool-steel",
            result[0]
        );
    }

    [Fact]
    public async Task GetWeaponAsync_ReturnsWeaponData()
    {
        string json =
            """
            {
                "name": "Cool Steel",
                "type": "Sword",
                "rarity": 3,
                "baseAttack": 39,
                "subStat": "ATK",
                "passiveName": "Bane of Water and Ice",
                "passiveDesc": "Increases damage against enemies affected by Hydro or Cryo."
            }
            """;

        HttpClient httpClient =
            new HttpClient(
                new FakeHttpMessageHandler(
                    json
                )
            );

        httpClient.BaseAddress =
            new Uri(
                "https://genshin.jmp.blue/"
            );

        GenshinApiClient client =
            new GenshinApiClient(
                httpClient
            );

        RemoteWeaponData? result =
            await client.GetWeaponAsync(
                "cool-steel"
            );

        Assert.NotNull(result);

        Assert.Equal(
            "Cool Steel",
            result.Name
        );

        Assert.Equal(
            "Sword",
            result.Type
        );

        Assert.Equal(
            3,
            result.Rarity
        );

        Assert.Equal(
            39,
            result.BaseAttack
        );
    }
}
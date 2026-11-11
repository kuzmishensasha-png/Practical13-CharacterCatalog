using Practical1.CLI;
using Practical1.Infra.Remote;

namespace Practical1.Tests.CLI;

public class RemoteCommandServiceTests
{
    private class FakeApiClient : IGenshinApiClient
    {
        public Task<IReadOnlyList<string>>
            GetCharacterIdsAsync()
        {
            IReadOnlyList<string> result =
                new List<string>
                {
                    "amber",
                    "kaeya"
                };

            return Task.FromResult(
                result
            );
        }

        public Task<RemoteCharacterData?>
            GetCharacterAsync(
                string id)
        {
            return Task.FromResult<
                RemoteCharacterData?
            >(null);
        }

        public Task<IReadOnlyList<string>>
            GetWeaponIdsAsync()
        {
            IReadOnlyList<string> result =
                new List<string>
                {
                    "cool-steel",
                    "amos-bow"
                };

            return Task.FromResult(
                result
            );
        }

        public Task<RemoteWeaponData?>
            GetWeaponAsync(
                string id)
        {
            return Task.FromResult<
                RemoteWeaponData?
            >(null);
        }
    }

    private class FakePresenter : IPresenter
    {
        public List<string> Lines { get; } =
            new List<string>();

        public void Show(string text)
        {
            Lines.Add(text);
        }

        public void ShowLine(string text)
        {
            Lines.Add(text);
        }

        public string? Read()
        {
            return null;
        }
    }

    [Fact]
    public async Task ListCharactersAsync_ShowsCharacters()
    {
        FakeApiClient api =
            new FakeApiClient();

        FakePresenter presenter =
            new FakePresenter();

        RemoteCommandService service =
            new RemoteCommandService(
                api,
                presenter
            );

        await service.ListCharactersAsync();

        Assert.Contains(
            "amber",
            presenter.Lines
        );

        Assert.Contains(
            "kaeya",
            presenter.Lines
        );
    }

    [Fact]
    public async Task ListWeaponsAsync_ShowsWeapons()
    {
        FakeApiClient api =
            new FakeApiClient();

        FakePresenter presenter =
            new FakePresenter();

        RemoteCommandService service =
            new RemoteCommandService(
                api,
                presenter
            );

        await service.ListWeaponsAsync();

        Assert.Contains(
            "cool-steel",
            presenter.Lines
        );

        Assert.Contains(
            "amos-bow",
            presenter.Lines
        );
    }
}
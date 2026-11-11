using Practical1.Infra.Remote;

namespace Practical1.CLI;

public class RemoteCommandService
{
    private readonly IGenshinApiClient api;
    private readonly IPresenter presenter;

    public RemoteCommandService(
        IGenshinApiClient api,
        IPresenter presenter)
    {
        this.api = api;
        this.presenter = presenter;
    }

    public async Task ListCharactersAsync()
    {
        IReadOnlyList<string> characters =
            await api.GetCharacterIdsAsync();

        foreach (
            string character
            in characters)
        {
            presenter.ShowLine(
                character
            );
        }
    }

    public async Task ListWeaponsAsync()
    {
        IReadOnlyList<string> weapons =
            await api.GetWeaponIdsAsync();

        foreach (
            string weapon
            in weapons)
        {
            presenter.ShowLine(
                weapon
            );
        }
    }
}
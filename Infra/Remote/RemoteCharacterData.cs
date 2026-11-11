namespace Practical1.Infra.Remote;

public class RemoteCharacterData
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public int Rarity { get; set; }

    public string Vision { get; set; } = "";

    public string Weapon { get; set; } = "";

    public List<RemoteTalentData> SkillTalents { get; set; } =
        new List<RemoteTalentData>();
}
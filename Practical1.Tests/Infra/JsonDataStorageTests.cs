using Practical1.Infra.Data;

namespace Practical1.Tests.Infra;

public class JsonDataStorageTests
{
    [Fact]
    public void SaveAndLoad_GameData_RestoresData()
    {
        JsonDataStorage storage =
            new JsonDataStorage();

        string fileName =
            Path.Combine(
                Path.GetTempPath(),
                $"game_{Guid.NewGuid():N}.json"
            );

        GameData data =
            new GameData();

        data.Items.Add(
            new ItemData
            {
                Name = "Sword",
                ArmorBonus = 5,
                AttackBonus = 10
            }
        );

        try
        {
            storage.Save(
                fileName,
                data
            );

            GameData? loaded =
                storage.Load<GameData>(
                    fileName
                );

            Assert.NotNull(loaded);

            Assert.Single(
                loaded.Items
            );

            Assert.Equal(
                "Sword",
                loaded.Items[0].Name
            );

            Assert.Equal(
                5,
                loaded.Items[0].ArmorBonus
            );

            Assert.Equal(
                10,
                loaded.Items[0].AttackBonus
            );
        }
        finally
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }

    [Fact]
    public void SaveAndLoad_TextData_RestoresChildren()
    {
        JsonDataStorage storage =
            new JsonDataStorage();

        string fileName =
            Path.Combine(
                Path.GetTempPath(),
                $"text_{Guid.NewGuid():N}.json"
            );

        TextElementData data =
            new TextElementData
            {
                Type = "text"
            };

        data.Children.Add(
            new TextElementData
            {
                Type = "paragraph",
                Content = "Hello"
            }
        );

        try
        {
            storage.Save(
                fileName,
                data
            );

            TextElementData? loaded =
                storage.Load<TextElementData>(
                    fileName
                );

            Assert.NotNull(loaded);

            Assert.Single(
                loaded.Children
            );

            Assert.Equal(
                "paragraph",
                loaded.Children[0].Type
            );

            Assert.Equal(
                "Hello",
                loaded.Children[0].Content
            );
        }
        finally
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }

    [Fact]
    public void Load_MissingFile_ReturnsNull()
    {
        JsonDataStorage storage =
            new JsonDataStorage();

        string fileName =
            Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.json"
            );

        GameData? result =
            storage.Load<GameData>(
                fileName
            );

        Assert.Null(result);
    }
}
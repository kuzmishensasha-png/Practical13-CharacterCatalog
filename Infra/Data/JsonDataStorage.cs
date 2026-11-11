using System.Text.Json;

namespace Practical1.Infra.Data;

public class JsonDataStorage : IDataStorage
{
    private readonly JsonSerializerOptions options =
        new JsonSerializerOptions
        {
            WriteIndented = true
        };

    public void Save<T>(
        string fileName,
        T data)
    {
        string json =
            JsonSerializer.Serialize(
                data,
                options
            );

        File.WriteAllText(
            fileName,
            json
        );
    }

    public T? Load<T>(
        string fileName)
    {
        if (!File.Exists(fileName))
        {
            return default;
        }

        string json =
            File.ReadAllText(fileName);

        return JsonSerializer.Deserialize<T>(
            json,
            options
        );
    }
}
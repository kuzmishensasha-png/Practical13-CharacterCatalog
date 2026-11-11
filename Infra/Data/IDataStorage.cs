namespace Practical1.Infra.Data;

public interface IDataStorage
{
    void Save<T>(string fileName, T data);

    T? Load<T>(string fileName);
}


namespace DalApi;

/// <summary>
/// Crud interface
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICrud<T> where T : class
{
    int Create(T entity);

    T? Read(int id);

    //List<T> ReadAll();

    T? Read(Func<T, bool> filter); // stage 2

    void Update(T entity);

    void Delete(int id);

    void PermanentDelete(int id); // stage 2

    IEnumerable<T?> ReadAll(Func<T, bool>? filter = null); // stage 2

    void Reset(); //erase all data values (in memory) and erase all data files (in xml)

}

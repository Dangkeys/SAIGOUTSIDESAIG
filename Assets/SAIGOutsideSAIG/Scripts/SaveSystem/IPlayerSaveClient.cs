using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaveSystem
{
    public interface IPlayerSaveClient
    {
        Task Save(string key, object value);

        Task Save(params (string key, object value)[] values);

        Task<T> Load<T>(string key);

        Task<IEnumerable<T>> Load<T>(params string[] keys);

        Task Delete(string key);

        Task DeleteAll();
    }
}
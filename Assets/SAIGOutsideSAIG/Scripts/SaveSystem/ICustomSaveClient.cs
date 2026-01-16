using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Models;

namespace SaveSystem
{
    public interface ICustomSaveClient
    {
        Task<T> Load<T>(string customItemID, params string[] keys);

        Task<List<T>> Query<T>(List<FieldFilter> filters, HashSet<string> returnKeys = null) where T : EntityData;
    }
}
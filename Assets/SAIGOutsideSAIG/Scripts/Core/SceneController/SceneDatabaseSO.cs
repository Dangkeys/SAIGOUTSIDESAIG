using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace SceneController
{
    [CreateAssetMenu(fileName = "SceneDatabaseSO", menuName = "Scriptable Objects/SceneDatabaseSO")]
    public class SceneDatabaeSO : ScriptableObject
    {
        [field: SerializeField] public List<SceneSO> SceneSOList { get; private set; }
        public SceneSO FindSceneSOById(string id)
        {
            return SceneSOList.FirstOrDefault(s => s.ID == id);
        }
        public SceneSO FindSceneSOByName(string name)
        {
            return SceneSOList.FirstOrDefault(s => s.Name == name);
        }
    }

}

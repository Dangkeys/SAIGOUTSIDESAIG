using System.Collections.Generic;
using UnityEngine;
namespace Scene
{
    [CreateAssetMenu(fileName = "SceneDatabaseSO", menuName = "Scriptable Objects/SceneDatabaseSO")]
    public class SceneDatabaeSO : ScriptableObject
    {
        [field: SerializeField] private List<SceneSO> SceneSOList;
    }

}

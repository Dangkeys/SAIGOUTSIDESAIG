using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseSO : ScriptableObject
{
    [field: SerializeField, ReadOnly] public string ID { get; private set; }
    [field: SerializeField] public string Name { get; private set; }

    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            Debug.Log($"[BaseSO] Generating new ID for {name} (was empty or null)");
            ID = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
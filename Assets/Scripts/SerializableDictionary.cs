using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private SyncList<TKey> keys = new SyncList<TKey>();
    [SerializeField] private SyncList<TValue> values = new SyncList<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            if (!this.ContainsKey(keys[i])) // 중복 방지
            {
                this.Add(keys[i], values[i]);
            }
        }
    }
}

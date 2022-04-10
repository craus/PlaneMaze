using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

/// <summary>
/// Same as Dictionary, but with default value when no key found
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="V"></typeparam>
[Serializable]
public class Map<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    public Func<V, bool> removeDefaultValues = v => false;
    private Func<V> defaultValueProvider = null;

    public new V this[K key] {
        get {
            if (!ContainsKey(key)) {
                return this[key] = GetDefaultValue();
            }
            return base[key];
        }
        set {
            if (removeDefaultValues(value)) {
                base.Remove(key);
                return;
            }
            base[key] = value;
        }
    }

    private V GetDefaultValue() {
        if (defaultValueProvider == null) {
            return default(V);
        } else {
            return defaultValueProvider();
        }
    }

    public Map(Func<V> defaultValueProvider = null) {
        this.defaultValueProvider = defaultValueProvider;
    }

    public Map() {
    }

    public override string ToString() {
        string result = "";
        foreach (K key in Keys) {
            if (result != "") {
                result += "\n";
            }
            string value = this[key] != null ? this[key].ToString() : "null";
            result += key.ToString() + ": " + value;
        }
        return result;
    }

    [SerializeField]
    private List<K> keys = new List<K>();

    [SerializeField]
    private List<V> values = new List<V>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<K, V> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}
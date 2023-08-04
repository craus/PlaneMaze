using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;

public class Saver : MonoBehaviour
{
    public string key;

    //public 

    public Map<string, object> Save() {
        //GetComponentsInChildren<ISaver>().ForEach(saver => saver.OnSave());
        return null;
    }

    public Saver Load(Map<string, object> data) {
        var result = Instantiate(this);
        //GetComponentsInChildren<ISaver>().ForEach(saver => saver.OnLoad());
        return result;
    }
}

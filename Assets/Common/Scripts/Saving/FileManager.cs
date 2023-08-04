using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;

public static class FileManager
{
    static BinaryFormatter GetFormatter() {
        var bf = new BinaryFormatter();

        SurrogateSelector ss = new SurrogateSelector();

        bf.SurrogateSelector = ss;

        return bf;
    }

    public static T LoadFromFile<T>(string filename) where T : class {
        filename = Application.persistentDataPath + "/" + filename;
        Debug.Log("Loading from: " + filename);
        T result = null;
        FileStream fs = null;
        try {
            var bf = GetFormatter();
            fs = new FileStream(filename, FileMode.Open);
            result = bf.Deserialize(fs) as T;
        } catch (Exception e) {
            Debug.LogException(e);
            Debug.Log(e.StackTrace);
        } finally {
            if (fs != null) fs.Close();
        }
        return result;
    }

    public static void SaveToFile<T>(T data, string filename) {
        filename = Application.persistentDataPath + "/" + filename;
        Debug.Log("Saving to: " + filename);
        FileStream fs = null;
        try {
            var bf = GetFormatter();

            fs = new FileStream(filename, FileMode.Create);
            bf.Serialize(fs, data);
        } catch (Exception e) {
            Debug.LogException(e);
            Debug.Log(e.StackTrace);
        } finally {
            if (fs != null) fs.Close();
        }
    }

    public static void RemoveFile(string filename) {
        filename = Application.persistentDataPath + "/" + filename;
        Debug.Log("Removing file: " + filename);
        try {
            var bf = GetFormatter();
            File.Delete(filename);
        } catch (Exception e) {
            Debug.LogException(e);
            Debug.Log(e.StackTrace);
        } finally {
        }
    }
}

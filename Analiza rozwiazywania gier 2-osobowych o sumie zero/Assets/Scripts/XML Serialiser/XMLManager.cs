using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XMLManager : MonoBehaviour
{
    public static XMLManager ins;

    private void Awake()
    {
        ins = this;
    }

    public void SaveData(GameData gD, string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameData));
        FileStream stream = new FileStream(string.Format("{0}/StreamingAssets/XML/{1}.xml", Application.dataPath, fileName), FileMode.Create);

        serializer.Serialize(stream, gD);
        stream.Close();

        Debug.Log("XML saved!");
    }

    public void SaveData(Scripts.GameData gD, string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Scripts.GameData));
        FileStream stream = new FileStream(string.Format("{0}/StreamingAssets/XML/{1}.xml", Application.dataPath, fileName), FileMode.Create);

        serializer.Serialize(stream, gD);
        stream.Close();

        Debug.Log("XML saved!");
    }

    public GameData LoadData(string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameData));
        FileStream stream = new FileStream(string.Format("{0}/StreamingAssets/XML/{1}.xml", Application.dataPath, fileName), FileMode.Open);

        GameData gD = (GameData)serializer.Deserialize(stream);
        stream.Close();

        Debug.Log("XML loaded");
        return gD;
    }

    public Scripts.GameData LoadData2(string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Scripts.GameData));
        FileStream stream = new FileStream(string.Format("{0}/XML/{1}.xml", Application.streamingAssetsPath, fileName), FileMode.Open);

        Scripts.GameData gD = (Scripts.GameData)serializer.Deserialize(stream);
        stream.Close();

        Debug.Log("XML loaded");
        return gD;
    }
}

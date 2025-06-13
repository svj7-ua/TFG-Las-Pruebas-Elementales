using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    

    public static void SavePlayerData(PlayerGameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerdata.toe";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerGameData LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/playerdata.toe";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerGameData data = formatter.Deserialize(stream) as PlayerGameData;
            stream.Close();
            return data;
        }
        else
        {
            return new PlayerGameData(); // Return a new instance if no data exists
        }
    }

}
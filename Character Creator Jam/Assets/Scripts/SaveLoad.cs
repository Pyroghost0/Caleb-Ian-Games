using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveLoad
{
    public static void Save(int[] data)
	{
		//save data
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/saveData.gd");
		bf.Serialize(file, data);
		file.Close();
	}
	public static void ClearData()
	{
		if (File.Exists(Application.persistentDataPath + "/saveData.gd"))
		{
			File.Delete(Application.persistentDataPath + "/saveData.gd");
		}
	}
	public static void FullUnlock()
	{
		int[] data = new int[25];
		data[0] = 0;
		data[1] = 1;
		data[2] = 0;
		data[3] = 0;
		for (int i = 4; i < 25; i++)
		{
			data[i] = 1;
		}
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/saveData.gd");
		bf.Serialize(file, data);
		file.Close();
	}
	public static int[] Load()
	{
		if (File.Exists(Application.persistentDataPath + "/saveData.gd"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/saveData.gd", FileMode.Open);
			int[] data = (int[])bf.Deserialize(file);
			file.Close();
			return data;
		}
		else
		{
			return null;
		}
	}
}

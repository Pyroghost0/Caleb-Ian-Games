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
		int[] data = new int[25];
		for (int i = 0; i < 25; i++)
		{
			data[i] = 0;
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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class Data
{
	//data[0] = level 1 completed?		data[1] = level 1 available?		data[2] = level 2...		data[12] = alt level 1 completed?		data[13] = alt level 1 available?		data[14] = alt level 2...
	public static void Save(bool[] data)
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

		bool[] data = Data.Load() == null ? new bool[24] : Data.Load();
		for (int i = 1; i < 24; i+=2)
		{
			data[i] = true;
		}
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/saveData.gd");
		bf.Serialize(file, data);
		file.Close();
	}
	public static bool[] Load()
	{
		if (File.Exists(Application.persistentDataPath + "/saveData.gd"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/saveData.gd", FileMode.Open);
			bool[] data = (bool[])bf.Deserialize(file);
			file.Close();
			return data;
		}
		else
		{
			return null;
		}
	}
}

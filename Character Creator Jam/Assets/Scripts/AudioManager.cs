/* Coded by Ian Connors
 * Qualms
 * Manages the audio levels in audio objects
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgm;
    public AudioEchoFilter bgmEcho;
	private AudioSource[] sounds;

	private float bgmVolume = 1;
	private float bgmVolumeMultiplier = 1;
	private float soundVolume = 1;

	public void Start()
	{
		sounds = FindObjectsOfType<AudioSource>(true);
	}
	public void BgmChangePitch(float pitch)
	{
        bgm.pitch = pitch;
	}
	public void BgmRestart()
	{
		bgm.time = 0;
	}

	public void BgmTime(float time)
	{
		bgm.time = time;
	}

    public void BgmChangeVolume(float volume)
	{
        bgm.volume = volume * bgmVolumeMultiplier;
		bgmVolume = volume;
	}
	public void BgmVolumeSetting(float volume)
	{
		bgm.volume = bgmVolume * volume;
		bgmVolumeMultiplier = volume;
	}
	public void SoundChangeVolume(float volume)
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			if (sounds[i]) sounds[i].volume = volume;
		}
		bgm.volume = bgmVolume * bgmVolumeMultiplier;
		soundVolume = volume;
	}
	public void ToggleBgmEcho(bool enable)
	{
		if (enable)
		{
			bgmEcho.enabled = true;
		}
		else
		{
			bgmEcho.enabled = false;
		}
	}

	public void BgmEchoSettings(int delay, float decayRatio)
	{
		bgmEcho.delay = delay;
		bgmEcho.decayRatio = decayRatio;
	}
	public void SpawnSlime()
	{
		sounds = FindObjectsOfType<AudioSource>(true);
		SoundChangeVolume(soundVolume);
	}
	public void ChangeScene(string sceneName)
	{
		ToggleBgmEcho(false);
		BgmChangePitch(1f);
		BgmChangeVolume(0.5f);
		sounds = FindObjectsOfType<AudioSource>(true);
		SoundChangeVolume(soundVolume);
		if (sceneName == "Dress Up Room")
		{
			BgmChangeVolume(0.1f);
		}
		else if (sceneName == "Tutorial")
		{
			BgmChangeVolume(0.3f);
			BgmChangePitch(0.2f);
		}
		else if (sceneName == "Mech Level")
		{
			BgmRestart();
			BgmChangePitch(1f);
		}
		else if (sceneName == "Deer Level")
		{
			BgmRestart();
			BgmChangePitch(0.8f);
		}
		else if (sceneName == "Baker Level")
		{
			BgmRestart();
			BgmChangePitch(0.6f);
		}
		else if (sceneName == "POTUS Level")
		{
			BgmRestart();
			BgmChangeVolume(0.3f);
			BgmChangePitch(1.1f);
		}
		else if (sceneName == "Boss Battle")
		{
			BgmChangeVolume(0f);
			BgmChangePitch(1f);
		}
	}
}

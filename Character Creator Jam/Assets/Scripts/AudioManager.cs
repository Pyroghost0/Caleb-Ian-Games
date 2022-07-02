using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgm;
    public AudioEchoFilter bgmEcho;
 
    public void BgmChangePitch(float pitch)
	{
        bgm.pitch = pitch;
	}
	public void BgmRestart()
	{
		bgm.time = 0;
	}

    public void BgmChangeVolume(float volume)
	{
        bgm.volume = volume;
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
	public void ChangeScene(string sceneName)
	{
		BgmChangeVolume(1f);
		if (sceneName == "Dress Up Room")
		{
			BgmChangeVolume(0.2f);
		}
		if (sceneName == "Tutorial")
		{
			BgmChangeVolume(0.5f);
			BgmChangePitch(0.2f);
		}
		if (sceneName == "Mech Level")
		{
			BgmRestart();
			BgmChangePitch(1f);
		}
		if (sceneName == "Deer Level")
		{
			BgmRestart();
			BgmChangePitch(0.8f);
		}
		if (sceneName == "Baker Level")
		{
			BgmRestart();
			BgmChangePitch(0.6f);
		}
		if (sceneName == "POTUS Level")
		{
			BgmRestart();
			BgmChangePitch(1.1f);
		}
		if (sceneName == "Boss Battle")
		{
			BgmChangeVolume(0f);
			BgmChangePitch(1f);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject source;

    public string currentBankName;
    public uint bankId;

    [Range(-50, 12)] public float audioVolume;
    [Range(-50, 12)] public float musicVolume;

    [Range(0, 1)] public float valence;

    public bool usePseudoValence;
    [Range(0, 1)] public float pseudoValence;

    private void Start()
    {
        if (usePseudoValence == true)
        {
            GetComponent<PseudoValenceRTPC>().enabled = true;
        }

        else
        {
            GetComponent<PseudoValenceRTPC>().enabled = false;
        }
    }


    public void LoadSoundBank(string soundBankName)
    {
        if(currentBankName != soundBankName)
        {
            UnloadSoundBank();
            currentBankName = soundBankName;
            AkBankManager.LoadBank(soundBankName, false, false);
        }
       
    }

    public void UnloadSoundBank()
    {
        currentBankName = string.Empty;
        AkSoundEngine.StopAll();
        AkBankManager.UnloadBank(currentBankName);
    }

    public void SetEvent(string evtName, float delay = 0)
    {
        AkSoundEngine.SetRTPCValue(AkSoundEngine.GetIDFromString("startDelay"), delay);
        AkSoundEngine.PostEvent(evtName, source);  
    }

    public void SetNewValenceValue(float newValence)
    {
        valence = newValence;
    }


    public void ChangeVolumeParameters(float newAudioVol, float newMusicVol)
    {
        audioVolume = newAudioVol;
        musicVolume = newMusicVol;
    }

    public void Pause()
    {
        AkSoundEngine.PostEvent("PauseAll", source);
    }

    public void Resume()
    {
        AkSoundEngine.PostEvent("ResumeAll", source);
    }

    private void Update()
    {
        AkSoundEngine.SetRTPCValue(AkSoundEngine.GetIDFromString("AudioVolume"), audioVolume);
        AkSoundEngine.SetRTPCValue(AkSoundEngine.GetIDFromString("MusicVolume"), musicVolume);

        if (usePseudoValence == false)
        {
            AkSoundEngine.SetRTPCValue(AkSoundEngine.GetIDFromString("ValenceLevel"), valence);
        }

    }
}

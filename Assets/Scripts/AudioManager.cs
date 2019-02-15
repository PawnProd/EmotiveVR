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
        AkSoundEngine.StopAll();
        AkBankManager.UnloadBank(currentBankName);
    }

    public void SetEvent(string evtName)
    {
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

    private void Update()
    {
        AkSoundEngine.SetRTPCValue("AudioVolume", audioVolume);
        AkSoundEngine.SetRTPCValue("MusicVolume", musicVolume);

        AkSoundEngine.SetRTPCValue("ValenceLevel", valence);
    }
}

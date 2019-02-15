using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Sequence")]
public class Sequence : ScriptableObject
{
    [Header("Video Data")]
    public VideoClip clip;
    public RenderTexture rt;

    [Header("Parameters")]
    public bool showEmotionalBar;
    public bool addScene;
    public bool waitInteraction;

    [Header("Additional Behaviors")]
    public string sceneNameToLoad;
    public int delayBeforeNextSequence;

    [Header("Audio")]
    public string sounBankName;
    public string audioEvtName;

}

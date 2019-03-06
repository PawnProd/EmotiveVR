using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "Sequence")]
public class Sequence : ScriptableObject
{

    [System.Serializable]
    public struct BarPositionInfo
    {
        public Vector3 position;
        public Vector3 rotation;
        public float keyTime;
        public bool changeText;
        public bool hide;
    }


    [Header("Video Data")]
    public VideoClip clip;
    public RenderTexture rt;

    [Header("Parameters")]
    public bool cutSequence;
    public bool showEmotionalBar;
    public bool addScene;
    public bool waitInteraction;
    public bool clearVideo;
    public bool usePostProcess;

    [Header("Additional Behaviors")]
    public string sceneNameToLoad;
    public int delayBeforeNextSequence;

    [Header("Audio")]
    public string soundBankName;
    public string audioEvtName;
    public float delay;

    [Header("Emotional Bar Param")]
    public List<BarPositionInfo> barInfo;

    [Header("Post Process")]
    public PostProcessProfile profile;
    public bool updateColorFromValence;

    [Header("Synchronization")]
    public bool forceSynchronize;
    public float timeValue;

    [Header("Cut Interaction")]
    public bool activeCut;
    public float timestampCut;
    public float time;

}


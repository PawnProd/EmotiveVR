using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Sequence")]
public class Sequence : ScriptableObject
{
    public VideoClip clip;
    public RenderTexture rt;

    public bool showEmotionalBar;
    public bool addScene;
    public bool waitInteraction;

    public string sceneNameToLoad;
    public int delayBeforeNextSequence;
}

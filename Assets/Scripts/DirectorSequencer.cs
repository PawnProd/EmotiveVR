using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Valve.VR;

public class DirectorSequencer : MonoBehaviour
{
    public static DirectorSequencer Instance { private set; get; }

    
    [Header("Control Sequences")]
    public int indexSequence = 0;
    public List<Sequence> sequences;
    public Sequence currentSequence;

    [Header("Parameters")]
    public float timeToChoice = 5;

    public float timer = 0;
    public float delay;
    public float updateValenceTime = 1;
    public float synchronizeTimer = 0;
    public float timeToFade = 2;

    public string loadedSceneName;

    public bool activeSubtitle = true;
    public bool vr = false;

    [Header("References")]
    public List<Camera> allCameras;
    public SRTManager srtManager;
    public VideoPlayer player;
    public VideoPlayer cutPlayer;
    public GameObject emotionalBar;
    public AudioManager audioManager;
    public Animator fadeAnimator;
    public RenderTexture cutRt;
    public Canvas canvasSubtitle;
	public Animator sphereFade;

    [HideInInspector] public Camera cam;

    [Header("States")]
    public bool play = false;
    public bool fadeDone = false;
    public bool waitEndScene = false;
    public bool activeRaycast = false;
    public bool showEpilogue = false;
    public bool updateBarPos = false;

    private readonly float _timerChoice = 0;

    private Quaternion startRotation;
    private GameObject _hitObject;

    private SteamVR_LoadLevel vrSceneManager;


    private void Awake()
    {
        Instance = this;

        if(vr)
        {
            cam = allCameras[1];
            cam.transform.parent.gameObject.SetActive(true);
            fadeAnimator.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            cam = allCameras[0];
            cam.gameObject.SetActive(true);
        }
        startRotation = cam.transform.rotation;
        canvasSubtitle.worldCamera = cam;
    }

    private void Start()
    {
        vrSceneManager = GetComponent<SteamVR_LoadLevel>();
        vrSceneManager.fadeInTime = timeToFade;
        vrSceneManager.fadeOutTime = timeToFade;

        if(activeSubtitle)
            srtManager.Init(sequences);

        // Add a callback when the video is finished
        player.loopPointReached += EndVideo;

        player.EnableAudioTrack(0, false);

        // Read all the valence data in the csv file
        DataReader.Init("Data_Valence.csv");
        player.playOnAwake = false;
        PrepareVideo();
    }

    private void Update()
    {
        if(currentSequence != null)
        {
            // If we load an additional scene and we didn't wait an interaction
            if (waitEndScene && !activeRaycast)
            {
                timer += Time.deltaTime;
                if (timer >= delay)
                {
                    waitEndScene = false;
                    timer = 0;
                    RemoveScene();

                    if(!vr)
                        StartCoroutine(CO_FadeIn());
                    else
                        StartCoroutine(CO_FadeInVR());
                }
            }

            if (currentSequence.cutSequence && fadeDone)
            {
                timer += Time.deltaTime;
                if (timer >= delay)
                {
                    timer = 0;
                    fadeDone = false;

                    if (!vr)
                        StartCoroutine(CO_FadeIn());
                    else
                        StartCoroutine(CO_FadeInVR());
                }
            }

            if (activeRaycast)
            {
                RaycastHit hit;
                // We fire a raycast from the camera position to the camera forward
                Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.red);
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity))
                {
                    if (hit.collider != null)
                    {
                        Debug.Log("Hit with " + hit.collider.name);
                        if (_hitObject != null && _hitObject != hit.collider.gameObject)
                        {
                            _hitObject.GetComponent<ChoiceSequence>().OnRaycastExit();
                        }

                        _hitObject = hit.collider.gameObject;
                        _hitObject.GetComponent<ChoiceSequence>().OnRaycastEnter();
                    }
                   
                }
                else
                {
                    if(_hitObject != null)
                        _hitObject.GetComponent<ChoiceSequence>().OnRaycastExit();

                    _hitObject = null;
                }
            }
        }

        if(updateBarPos)
        {
            
            emotionalBar.GetComponent<EmotionBar>().UpdateBar(player.frame);
        }
       
    }

    public void ValidateChoice(ChoiceSequence choice)
    {
        if (!choice.nextSequence)
        {
            AddSequences(choice.GetSequence());
            srtManager.AddSubtitles(choice.GetSequence());
        }

        activeRaycast = false;
        showEpilogue = true;
        RemoveScene();

        if (!vr)
            StartCoroutine(CO_FadeIn());
        else
            StartCoroutine(CO_FadeInVR());
    }


    private void PrepareVideo()
    {
        play = false;
        if (indexSequence < sequences.Count)
        {
            currentSequence = sequences[indexSequence];
            if (currentSequence.clip != null)
            {
                SetupSequence(currentSequence);
                player.Prepare();
                player.prepareCompleted += SetNextVideo;
            }
            else
            {
                SetNextVideo(player);
            }
           
        }
    }

    private void SetNextVideo(VideoPlayer vp)
    {

        player.prepareCompleted -= SetNextVideo;
        // We reset the camera rotation to avoid some bug in the choice scene
        cam.transform.rotation = startRotation;

        emotionalBar.SetActive(currentSequence.showEmotionalBar);

        // SETUP ADDITIONAL SCENE
        if (currentSequence.addScene)
        {
            AddScene(currentSequence);

            if(currentSequence.waitInteraction)
            {
                activeRaycast = true;
            }
        }

        if(currentSequence.delayBeforeNextSequence != 0)
        {
            delay = currentSequence.delayBeforeNextSequence;
        }

        if(currentSequence.clearVideo)
        {
            player.Stop();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            audioManager.UnloadSoundBank();
        }
        else
        {
            cam.clearFlags = CameraClearFlags.Skybox;
        }

        // SETUP EMOTIONAL BAR
        if(currentSequence.showEmotionalBar)
        {
            audioManager.SetNewValenceValue(DataReader.GetValence());
            StartCoroutine(CO_UpdateValenceTime());

            if(currentSequence.barInfo.Count > 0)
            {
                emotionalBar.GetComponent<EmotionBar>().MapBarInfo(currentSequence.barInfo);
                updateBarPos = true;
            }

            emotionalBar.GetComponent<EmotionBar>().ShowOrHideText(currentSequence.hideText);
        }

        // SETUP AUDIO
        if (!string.IsNullOrEmpty(currentSequence.soundBankName))
        {
            audioManager.LoadSoundBank(currentSequence.soundBankName);
        }

        if(currentSequence.audioEvtNames.Count > 0)
        {
            StartCoroutine(CO_WaitVideoToLaunchAudio());
        }
       

        if(currentSequence.usePostProcess)
        {
            cam.GetComponent<CameraManager>().SetPostProcess(true, currentSequence.profile);
        }
        else
        {
            cam.GetComponent<CameraManager>().SetPostProcess(false, null);
        }

        if(activeSubtitle)
        {
            srtManager.SetSubtitles(currentSequence.name);
        }

        if(vr)
        {
            StartCoroutine(CO_FadeOutVR());
        }
        else
        {
            StartCoroutine(CO_FadeOut());
        }

        ++indexSequence;
    }

    // Callback when the video is finish
    private void EndVideo(VideoPlayer vp)
    {
        player.Stop();
        // Check if we can go to the next video
        if (!currentSequence.waitInteraction && currentSequence.delayBeforeNextSequence == 0)
        {
            if (!vr)
                StartCoroutine(CO_FadeIn());
            else
                StartCoroutine(CO_FadeInVR());
        }

    }

    #region Sequences
    // Set the sequence to the scene. Update video and render texture and set to the skybox material. Active the bar if it's necessary
    private void SetupSequence(Sequence sequence)
    {
        player.clip = sequence.clip;
        player.targetTexture = sequence.rt;
        RenderSettings.skybox.mainTexture = sequence.rt;

    }

    // Add a range of new sequences in the list (for the choice scene)
    private void AddSequences(List<Sequence> newSequences)
    {
        sequences.InsertRange(indexSequence, newSequences);
    }


    // Check if the sequence list contains a sequence
    public bool ContainSequence(Sequence sequence)
    {
        return sequences.Contains(sequence);
    }

    #endregion


    #region Fade Event

    public void EndFadeIn()
    {
        PrepareVideo();
    }

    public void EndFadeOut()
    {
        play = true;
        player.Play();
    }
    #endregion

    #region Scene Management
    // Load an additional scene in Additive Mode
    private void AddScene(Sequence sequence)
    {
        SceneManager.LoadSceneAsync(sequence.sceneNameToLoad, LoadSceneMode.Additive);
        waitEndScene = true;
        loadedSceneName = sequence.sceneNameToLoad;
        
    }

    // Unload an scene
    private void RemoveScene()
    {
        SceneManager.UnloadSceneAsync(loadedSceneName);
    }
    #endregion

    #region Coroutines
    // Update every second the emotional bar if it is active
    IEnumerator CO_UpdateValenceTime()
    {
        while(currentSequence.showEmotionalBar)
        {
            yield return new WaitForSeconds(updateValenceTime);
            DataReader.UpTime();
            float valence = DataReader.GetValence();
            audioManager.SetNewValenceValue(valence);
            emotionalBar.GetComponent<EmotionBar>().UpdateEmotionBar(valence);
            if (currentSequence.updateColorFromValence)
            {
                cam.GetComponent<CameraManager>().UpdateFilterColor(valence);
            }
        }

        yield return null;
    }

    IEnumerator CO_WaitVideoToLaunchAudio()
    {
        while(!player.isPlaying)
        {
            yield return null;
        }

        if(currentSequence.forceSynchronize)
        {
            while(synchronizeTimer < currentSequence.timeValue)
            {
                synchronizeTimer += Time.deltaTime;
                yield return null;
            }
        }
        synchronizeTimer = 0;
        foreach (string evtName in currentSequence.audioEvtNames)
        {
            if (!string.IsNullOrEmpty(evtName))
            {
                audioManager.SetEvent(evtName, currentSequence.delay);
				StartCoroutine(srtManager.Begin());
            }
        }
    }

    IEnumerator CO_FadeIn()
    {
        fadeDone = false;
        fadeAnimator.SetTrigger("FadeIn");

        yield return new WaitForSeconds(fadeAnimator.GetCurrentAnimatorStateInfo(0).length);

        EndFadeIn();
        yield return null;
    }

    IEnumerator CO_FadeOut()
    {
        fadeAnimator.SetTrigger("FadeOut");

        yield return new WaitForSeconds(fadeAnimator.GetCurrentAnimatorStateInfo(0).length);

        fadeDone = true;
        EndFadeOut();
        yield return null;
    }

    IEnumerator CO_FadeInVR()
    {
        Debug.Log("Fade In VR!");

        sphereFade.SetTrigger("FadeIn");

        yield return new WaitForSeconds(sphereFade.GetCurrentAnimatorStateInfo(0).length);

        EndFadeIn();
        yield return null;
    }

    IEnumerator CO_FadeOutVR()
    {
        yield return new WaitForSeconds(1);

        Debug.Log("Fade Out VR!");

        sphereFade.SetTrigger("FadeOut");
        yield return new WaitForSeconds(sphereFade.GetCurrentAnimatorStateInfo(0).length);

        fadeDone = true;
        EndFadeOut();
        yield return null;
    }
    #endregion
}

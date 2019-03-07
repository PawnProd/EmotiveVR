using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class DirectorSequencer : MonoBehaviour
{
    public static DirectorSequencer Instance { private set; get; }

    
    [Header("Control Sequences")]
    public int indexSequence = 0;

    public List<Sequence> sequences;

    public Sequence currentSequence;

    public float timeToChoice = 5;

    public RenderTexture cutRt;

    [Header("References")]
    public Camera cam;
    public SRTManager srtManager;
    public VideoPlayer player;
    public VideoPlayer cutPlayer;
    public GameObject emotionalBar;
    public AudioManager audioManager;
    public Animator fadeAnimator;

    public bool useVR = false;

    public bool play = false;
    public bool fadeDone = false;
    public bool waitEndScene = false;
    public bool activeRaycast = false;
    public bool showEpilogue = false;

    public string loadedSceneName;

    public float timer = 0;
    public float delay;
    public float updateValenceTime = 1;
    public float synchronizeTimer = 0;

    private float _timerChoice = 0;
    public float _timerCut = 0;
    private Quaternion startRotation;
    private GameObject _hitObject;

    private bool _cut;
    private bool _resumeVideo;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        srtManager.Init(sequences);

        // Add a callback when the video is finished
        player.loopPointReached += EndVideo;
        startRotation = cam.transform.rotation;

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
                    StartCoroutine(CO_FadeIn());
                }
            }

            if (currentSequence.cutSequence && fadeDone)
            {
                timer += Time.deltaTime;
                if (timer >= delay)
                {
                    timer = 0;
                    fadeDone = false;
                    StartCoroutine(CO_FadeIn());
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
                        if (hit.collider.CompareTag("Choice") && _hitObject != hit.collider.gameObject)
                        {
                            if (_hitObject != null)
                                _hitObject.GetComponent<ChoiceSequence>().FadeOutSequence();

                            _timerChoice = 0;
                            _hitObject = hit.collider.gameObject;
                            _hitObject.GetComponent<AudioSource>().Play();
                        }
                        else if (_hitObject == hit.collider.gameObject)
                        {
                            _timerChoice += Time.deltaTime;

                            if (_timerChoice >= timeToChoice)
                            {
                                ValidateChoice(_hitObject.GetComponent<ChoiceSequence>());
                            }
                        }
                    }

                }
                else if (_hitObject != null)
                {
                    _hitObject.GetComponent<ChoiceSequence>().FadeInSequence();
                    _hitObject = null;
                }
            }

            if (currentSequence.activeCut && fadeDone)
            {
                _timerCut += Time.deltaTime;

                if (_timerCut >= currentSequence.timestampCut)
                {
                    fadeDone = false;
                    _cut = true;
                    StartCoroutine(CO_FadeIn());
                    _timerCut = 0;
                }

                if (cutPlayer.isPlaying && _timerCut >= currentSequence.time)
                {
                    fadeDone = false;
                    EndCut();
                    _timerCut = 0;
                }
            }
        }
       
    }

    public void ValidateChoice(ChoiceSequence choice)
    {
        if (!choice.nextSequence)
        {
            AddSequences(choice.GetSequence());
        }

        activeRaycast = false;
        showEpilogue = true;
        RemoveScene();
        StartCoroutine(CO_FadeIn());
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

        if(currentSequence.cutSequence)
        {
            SetCut();
        }

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
            }
        }

        // SETUP AUDIO
        if (!string.IsNullOrEmpty(currentSequence.soundBankName))
        {
            audioManager.LoadSoundBank(currentSequence.soundBankName);
        }

        if (!string.IsNullOrEmpty(currentSequence.audioEvtName))
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

        if(currentSequence.activeCut)
        {
            emotionalBar.GetComponent<EmotionBar>().ShowOrHideText(false);
        }

        srtManager.SetSubtitles(currentSequence.name);
        StartCoroutine(CO_FadeOut());
        ++indexSequence;
    }

    // Callback when the video is finish
    private void EndVideo(VideoPlayer vp)
    {
        player.Stop();
        // Check if we can go to the next video
        if (!currentSequence.waitInteraction && currentSequence.delayBeforeNextSequence == 0)
        {
            StartCoroutine(CO_FadeIn());
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

    #region Cut

    private void SetCut()
    {
        emotionalBar.GetComponent<EmotionBar>().ResetPosition();
        emotionalBar.GetComponent<EmotionBar>().ShowOrHideText(true);
        emotionalBar.GetComponent<EmotionBar>().SetText("SEQUENCE INTERACTIVE", "Le feedback visuel de vos émotions en temps réel apparaîtra sur le miroir de la chambre.");
    }

    private void CutVideo()
    {
        emotionalBar.SetActive(false);
        player.Pause();
        audioManager.Pause();
        RenderSettings.skybox.mainTexture = cutRt;
        StartCoroutine(CO_FadeOut());
    }

    private void EndCut()
    {
        cutPlayer.Stop();
        _resumeVideo = true;
        StartCoroutine(CO_FadeIn());
    }

    private void ResumeVideo()
    {
        RenderSettings.skybox.mainTexture = currentSequence.rt;
        StartCoroutine(CO_FadeOut());
    }
    #endregion

    #region Fade Event

    public void EndFadeIn()
    {
        if(_cut)
        {
            CutVideo();
        }
        else if(_resumeVideo)
        {
            ResumeVideo();
        }
        else
        {
            PrepareVideo();
        }
    }

    public void EndFadeOut()
    {
        if (_cut)
        {
            _cut = false;
            cutPlayer.Play();
        }
        else if (_resumeVideo)
        {
            _resumeVideo = false;
            player.Play();
            audioManager.Resume();
        }
        else
        {
            StartCoroutine(srtManager.Begin());
            play = true;
            player.Play();
        }
    }
    #endregion

    #region Scene Management
    // Load an additional scene in Additive Mode
    private void AddScene(Sequence sequence)
    {
        SceneManager.LoadScene(sequence.sceneNameToLoad, LoadSceneMode.Additive);
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
        audioManager.SetEvent(currentSequence.audioEvtName, currentSequence.delay);
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
    #endregion
}

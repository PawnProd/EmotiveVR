using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class DirectorSequencer : MonoBehaviour
{
    public static DirectorSequencer Instance { private set; get; }

    public int indexSequence = 0;

    public List<Sequence> sequences;

    public Sequence currentSequence;

    [Header("References")]
    public Camera cam;
    public VideoPlayer player;
    public GameObject emotionalBar;
    public AudioManager audioManager;

    public bool play = false;
    public bool waitEndScene = false;
    public bool activeRaycast = false;
    public bool showEpilogue = false;

    public string loadedSceneName;

    public float timer = 0;
    public float delay;
    public float updateValenceTime = 1;

    private Quaternion startRotation;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Add a callback when the video is finished
        player.loopPointReached += EndVideo;
        startRotation = cam.transform.rotation;

        // Read all the valence data in the csv file
        DataReader.Init("Data_Valence.csv");
        SetNextVideo();
    }

    private void Update()
    {
        // If we load an additional scene and we didn't wait an interaction
        if(waitEndScene && !activeRaycast)
        {
            timer += Time.deltaTime;
            if(timer >= delay)
            {
                waitEndScene = false;
                timer = 0;
                RemoveScene();
                SetNextVideo();
            }
        }

        if(activeRaycast)
        {
            RaycastHit hit;
            // We fire a raycast from the camera position to the camera forward
            Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.red);
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity))
            {
                if(hit.collider != null)
                {
                    if(hit.collider.CompareTag("Choice"))
                    {
                        if(!hit.collider.GetComponent<ChoiceSequence>().nextSequence)
                        {
                            AddSequences(hit.collider.GetComponent<ChoiceSequence>().GetSequence());
                        }
                        
                        activeRaycast = false;
                        showEpilogue = true;
                        RemoveScene();
                        SetNextVideo();
                    }
                }
            }
        }
    }

    private void SetNextVideo()
    {
        // We reset the camera rotation to avoid some bug in the choice scene
        cam.transform.rotation = startRotation;

        if(indexSequence < sequences.Count)
        {
            currentSequence = sequences[indexSequence];

            // SETUP ADDITIONAL SCENE
            if (currentSequence.addScene)
            {
                AddScene(currentSequence);

                if(currentSequence.waitInteraction)
                {
                    activeRaycast = true;
                }
            }

            // SETUP VIDEO
            if (currentSequence.clip != null)
            {
                SetupSequence(currentSequence);
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

            ++indexSequence;
        }
        
        
    }

    // Set the sequence to the scene. Update video and render texture and set to the skybox material. Active the bar if it's necessary
    private void SetupSequence(Sequence sequence)
    {
        player.clip = sequence.clip;
        player.targetTexture = sequence.rt;
        RenderSettings.skybox.mainTexture = sequence.rt;

        emotionalBar.SetActive(sequence.showEmotionalBar);
    }

    // Callback when the video is finish
    private void EndVideo(VideoPlayer vp)
    {
        // Check if we can go to the next video
        if(!currentSequence.waitInteraction && currentSequence.delayBeforeNextSequence == 0)
        {
            SetNextVideo();
        }
        
    }

    // Add a range of new sequences in the list (for the choice scene)
    private void AddSequences(List<Sequence> newSequences)
    {
        sequences.InsertRange(indexSequence, newSequences);
    }

    // Load an additional scene in Additive Mode
    private void AddScene(Sequence sequence)
    {
        SceneManager.LoadScene(sequence.sceneNameToLoad, LoadSceneMode.Additive);
        waitEndScene = true;
        loadedSceneName = sequence.sceneNameToLoad;
        delay = sequence.delayBeforeNextSequence;
    }

    // Unload an scene
    private void RemoveScene()
    {
        SceneManager.UnloadSceneAsync(loadedSceneName);
    }

    // Check if the sequence list contains a sequence
    public bool ContainSequence(Sequence sequence)
    {
        return sequences.Contains(sequence);
    }

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
        }

        yield return null;
    }

    IEnumerator CO_WaitVideoToLaunchAudio()
    {
        while(!player.isPlaying)
        {
            yield return null;
        }

        audioManager.SetEvent(currentSequence.audioEvtName, currentSequence.delay);
    }
}

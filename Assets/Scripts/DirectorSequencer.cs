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

    public bool play = false;
    public bool waitEndScene = false;
    public bool activeRaycast = false;
    public bool showEpilogue = false;

    public string loadedSceneName;

    public float timer = 0;
    public float delay;

    private Quaternion startRotation;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player.loopPointReached += EndVideo;
        startRotation = cam.transform.rotation;
        DataReader.Init("Data_Valence.csv");
    }

    private void Update()
    {
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
        cam.transform.rotation = startRotation;
        if(indexSequence < sequences.Count)
        {
            currentSequence = sequences[indexSequence];

            if (currentSequence.addScene)
            {
                AddScene(currentSequence);

                if(currentSequence.waitInteraction)
                {
                    activeRaycast = true;
                }

            }

            if (currentSequence.clip != null)
            {
                SetupSequence(currentSequence);
            }

            if(currentSequence.showEmotionalBar)
            {
                StartCoroutine(CO_UpdateValenceTime());
            }

            ++indexSequence;
        }
        
        
    }

    private void SetupSequence(Sequence sequence)
    {
        player.clip = sequence.clip;
        player.targetTexture = sequence.rt;
        RenderSettings.skybox.mainTexture = sequence.rt;

        emotionalBar.SetActive(sequence.showEmotionalBar);
    }

    private void EndVideo(VideoPlayer vp)
    {
        if(!currentSequence.waitInteraction && currentSequence.delayBeforeNextSequence == 0)
        {
            SetNextVideo();
        }
        
    }

    private void AddSequences(List<Sequence> newSequences)
    {
        sequences.InsertRange(indexSequence, newSequences);
    }

    private void AddScene(Sequence sequence)
    {
        SceneManager.LoadScene(sequence.sceneNameToLoad, LoadSceneMode.Additive);
        waitEndScene = true;
        loadedSceneName = sequence.sceneNameToLoad;
        delay = sequence.delayBeforeNextSequence;
    }

    private void RemoveScene()
    {
        SceneManager.UnloadSceneAsync(loadedSceneName);
    }

    public bool ContainSequence(Sequence sequence)
    {
        return sequences.Contains(sequence);
    }

    IEnumerator CO_UpdateValenceTime()
    {
        while(currentSequence.showEmotionalBar)
        {
            yield return new WaitForSeconds(1);
            DataReader.UpTime();
        }

        yield return null;
    }
}

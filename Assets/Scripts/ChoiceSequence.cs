using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceSequence : MonoBehaviour
{
    public List<Sequence> sequences;
    public bool nextSequence = false;
    public bool blockHide = false;
    public bool epilogue = false;

    private float _fadePercent = 1;

    private void Start()
    {
        if(epilogue)
        {
            if(!DirectorSequencer.Instance.showEpilogue)
            {

                gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
        else if(DirectorSequencer.Instance.ContainSequence(sequences[0]) && !blockHide)
        {
            gameObject.SetActive(false);
        }
    }

    public List<Sequence> GetSequence()
    {
        return sequences;
    }

    public void FadeOutSequence()
    {
        StopAllCoroutines();
        StartCoroutine(CO_FadeOutChoiceSequence());
    }

    public void FadeInSequence()
    {
        StopAllCoroutines();
        StartCoroutine(CO_FadeInChoiceSequence());
    }

    IEnumerator CO_FadeOutChoiceSequence()
    {
        while (_fadePercent > 0)
        {
            _fadePercent -= Time.deltaTime / 3;
            GetComponent<AudioSource>().volume = _fadePercent;
            yield return null;
        }
        _fadePercent = 0;
        GetComponent<AudioSource>().volume = 0;
        DirectorSequencer.Instance.ValidateChoice(this);

    }

    IEnumerator CO_FadeInChoiceSequence()
    {
        while (_fadePercent < 1)
        {
            _fadePercent += Time.deltaTime;
            GetComponent<AudioSource>().volume = _fadePercent;
            yield return null;
        }

        _fadePercent = 1;
        GetComponent<AudioSource>().volume = 1;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceSequence : MonoBehaviour
{
    public List<Sequence> sequences;
    public bool nextSequence = false;
    public bool blockHide = false;

    private void Start()
    {
        if(sequences.Count == 0)
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
}

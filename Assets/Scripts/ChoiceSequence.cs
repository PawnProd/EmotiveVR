using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceSequence : MonoBehaviour
{
    public List<Sequence> sequences;
    public bool nextSequence = false;

    private void Start()
    {
        if(sequences.Count == 0)
        {
            if(DirectorSequencer.Instance.showEpilogue)
            {
                Debug.Log("Coucou");
                gameObject.transform.parent.gameObject.SetActive(true);
            }
        }
        else if(DirectorSequencer.Instance.ContainSequence(sequences[0]))
        {
            gameObject.SetActive(false);
        }
    }

    public List<Sequence> GetSequence()
    {
        return sequences;
    }
}

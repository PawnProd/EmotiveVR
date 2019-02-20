using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionBar : MonoBehaviour
{
    public Image positiveFill;
    public Image negativeFill;

    public void UpdateBar(float valence)
    {
        if(valence < 0.5f)
        {
            if(!negativeFill.gameObject.activeSelf)
            {
                negativeFill.gameObject.SetActive(true);
            }

            positiveFill.gameObject.SetActive(false);
            negativeFill.fillAmount = valence * 2;
        }
        else
        {
            if (!positiveFill.gameObject.activeSelf)
            {
                positiveFill.gameObject.SetActive(true);
            }

            negativeFill.gameObject.SetActive(false);
            positiveFill.fillAmount = valence * 2;
        }
    }

    private void Update()
    {
        if(gameObject.activeSelf)
            UpdateBar(DataReader.GetValence());
    }
}

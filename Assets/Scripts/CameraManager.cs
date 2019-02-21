using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager : MonoBehaviour
{
    public PostProcessLayer layer;
    public PostProcessVolume volume;

    private void Start()
    {
        layer = GetComponent<PostProcessLayer>();
        volume = GetComponent<PostProcessVolume>();
    }

    public void SetPostProcess(bool active)
    {
        layer.enabled = active;
        volume.enabled = active;
    }

    public void ChangeLuminosity(float valence)
    {
        PostProcessProfile profile = volume.profile;

        profile.TryGetSettings(out ColorGrading colorGrad);
        colorGrad.enabled.value = true;
        colorGrad.postExposure.value = (((valence - 0) * (3 - 1)) / (1 - 0)) + 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cigar : MonoBehaviour
{
    public List<float> timestampCigare;
    public float maxBurn;
    public float minBurn;

    private Material _burnMat;
    private float _time = 0;
    private int _indexTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        _burnMat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(_indexTime < timestampCigare.Count)
        {
            _time += Time.deltaTime;

            if (_time >= timestampCigare[_indexTime])
            {
                _indexTime++;
                _burnMat.SetInt("_Bool_Burn", 0);
                _burnMat.SetFloat("_Mask_Range", maxBurn);
                StartCoroutine(CO_Wait());
                
            }
        }
    }

    IEnumerator CO_Wait()
    {
        yield return new WaitForSeconds(3);
        _burnMat.SetInt("_Bool_Burn", 1);
    }
}

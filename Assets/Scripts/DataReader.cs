using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class DataReader
{
    private static string _path;
    private static string _filename;

    private static List<float> _valenceValues = new List<float>();
    private static int _currentTime = 0;

    public static void Init(string filename)
    {
        _filename = filename;
        
        _path = Application.streamingAssetsPath + "/DataValence/" + _filename;
        Debug.Log(_path);
        ReadDataFromCSV();

    }

    public static void ReadDataFromCSV()
    {
        string fileData = File.ReadAllText(_path);
        string[] values = fileData.Split('\n');
        
        float[] floatValues = Array.ConvertAll(values, x => float.Parse(x));

        _valenceValues.AddRange(floatValues);
    }

    public static float GetValence()
    {
        return _valenceValues[_currentTime];
    }

    public static void UpTime()
    {
        _currentTime++;
        _currentTime = Mathf.Clamp(_currentTime, 0, _valenceValues.Count - 1);
    }
}

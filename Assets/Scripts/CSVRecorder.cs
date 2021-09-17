using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class CSVRecorder
{

    StringBuilder sb = new StringBuilder();
    string filepath;
    StreamWriter outStream;

    // Start is called before the first frame update
    public void Start()
    {
        string[] rowDataTemp = new string[9];
        rowDataTemp[0] = "No.";
        rowDataTemp[1] = "x";
        rowDataTemp[2] = "y"; 
        rowDataTemp[3] = "z";
        rowDataTemp[4] = "rx";
        rowDataTemp[5] = "ry";
        rowDataTemp[6] = "rz";
        rowDataTemp[7] = "ppm";
        rowDataTemp[8] = "distance";

        filepath = getPath();

        sb.AppendLine(string.Join(",", rowDataTemp));

        outStream = System.IO.File.CreateText(filepath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    // Update is called once per frame
    public void Update(string[] line)
    {
        sb = new StringBuilder();
        sb.AppendLine(string.Join(",", line));
        outStream.WriteLine(sb);
        outStream.Close();
    }

    // Following method is used to retrive the relative path as device platform
    private string getPath()
    {
        #if UNITY_EDITOR
                return Application.dataPath + "/CSV/" + "Saved_data.csv";
        #elif UNITY_ANDROID
                return Application.persistentDataPath+"Saved_data.csv";
        #elif UNITY_IPHONE
                return Application.persistentDataPath+"/"+"Saved_data.csv";
        #else
                return Application.dataPath +"/"+"Saved_data.csv";
        #endif
    }
}

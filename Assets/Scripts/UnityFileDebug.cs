using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityFileDebug : MonoBehaviour
{
    public bool useAbsolutePath = false;
    public string fileName = "MyGame";

    public string absolutePath = "/home/yourUsername/UnityLogs";

    public string filePath;
    public string filePathFull;
    public int count = 0;

    System.IO.StreamWriter fileWriter;

    void OnEnable()
    {
        UpdateFilePath();
        if (Application.isPlaying)
        {
            count = 0;
            fileWriter = new System.IO.StreamWriter(filePathFull, false);
            fileWriter.AutoFlush = true;
           // fileWriter.WriteLine("[");
            Application.logMessageReceived += HandleLog;
        }
    }

    void OnDisable()
    {
        if (Application.isPlaying)
        {
            Application.logMessageReceived -= HandleLog;
          //  fileWriter.WriteLine("\n]");
            fileWriter.Close();
        }
    }

    public void UpdateFilePath()
    {
        filePath = useAbsolutePath ? absolutePath : Application.persistentDataPath;
        filePathFull = System.IO.Path.Combine(filePath, fileName + "." +
            System.DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".csv");
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        //... same as above
        if(type == LogType.Log)
        {
            fileWriter.Write((count == 0 ? "" : logString + "\n"));
           // Debug.Log(logString);
        }
        
            //+ JsonUtility.ToJson(j));
        count++;
    }
}
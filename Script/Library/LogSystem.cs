using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public static class LogSystem
{
    public static void Init()
    {
#if !UNITY_EDITOR
        Application.logMessageReceived += Log;
        if (!Directory.Exists(Application.persistentDataPath + "/Cache/Log"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Cache/Log");
#endif
    }

    public static StringBuilder GetTime()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(string.Format("{0:D2}", System.DateTime.Now.Hour));
        builder.Append(string.Format("{0:D2}", System.DateTime.Now.Minute));
        builder.Append(string.Format("{0:D2}", System.DateTime.Now.Second));
        return builder;
    }
    public static StringBuilder GetDate()
    {
        StringBuilder builder = new StringBuilder(); 
        builder.Append(System.DateTime.Now.Date.Year);
        builder.Append(string.Format("{0:D2}", System.DateTime.Now.Month));
        builder.Append(string.Format("{0:D2}", System.DateTime.Now.Day));
        return builder;
    }
    public static System.TimeSpan DeltaTime(string time)
    {
        System.DateTime prev = System.DateTime.Parse(time);
        System.DateTime curr = System.DateTime.Now;
        string[] strs = (prev - curr).ToString().Split(':');
        return curr - prev;
    }
    public static void Log(string condition, string stackTrace, LogType type)
    {
        #if !UNITY_EDITOR
        string msg = GetTime().ToString() + " " + type + " " + condition + " " + stackTrace + "\r\n";

        FileStream fs;
        fs = new FileStream(Application.persistentDataPath + "/Cache/Log/" +GetDate() + ".txt", FileMode.Append);

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(msg);
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
#endif
    }

    public static void Log(LogType type, string log)
    {
        #if !UNITY_EDITOR
        string msg = GetTime() + " " + type + " " + log + "\r\n";

        FileStream fs;
        fs = new FileStream(Application.persistentDataPath + "/Cache/Log/" +GetDate() + ".txt", FileMode.Append);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(msg);
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
#endif
    }
}

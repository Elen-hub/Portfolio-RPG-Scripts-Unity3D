using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameSystem
{
    public const float NormalMonsterColliderRange = 5f;
    public const float NormalMonsterChaseRangeToTarget = 10f;
    public const float BossMonsterColliderRange = 10f;
    public const float BossMonsterChaseRangeToTarget = 15f;
    public const float NormalMonsterChaseRangeToSpawn = 10f;

    public const float NPCInteractionColliderRange = 1.5f;

    public const float ItemColliderRange = 1;

    public const float PlayerCameraMoveTime = 0.5f;
    public const float PlayerCameraMoveDistance = 0.3f;

    public static Color ColorNormal = new Color(0, 0, 0);
    public static Color ColorMagic = new Color(0.22f, 0.41f, 1);
    public static Color ColorUnique = new Color(1, 0.4f, 1);
    public static Color ColorRelic = new Color(0.4f, 0, 1);
    public static Color ColorLegend = new Color(1, 1, 0.2f);
    public static Color ColorInfinity = new Color(1, 0.2f, 0.2f);
    public static string[] ColorRarity = new string[6] { "#ffffffff", "#0040ffff", "#ff00a7ff", "#00ffffff", "#ffff00ff", "#ff0000ff" };

    // Custom System
    public static string Path;
    public static float DoubleTouchTime = 0.3f;
    public static float MusicVolume = 0.8f;
    public static bool UseMusic = true;

    public static float SoundVolume = 0.8f;
    public static bool UseSound = true;

    public static bool PlayerCameraHoldRot = true;
    public static float PlayerCameraHeight = 5;
    public static float PlayerCameraWidth = 4.5f;
    public static float PlayerCameraAngle = 45;

    public static string ID;
    public static string PWD;

    static bool m_useWeather = true;
    static bool m_useBloom = true;
    static bool m_useAmbient = true;
    static bool m_useOutline = true;
    static bool m_useFog = true;

    static int m_joyStick = 0;
    static int m_frameLevel = 1;
    static int m_autoAimLevel = 1;

    public static bool UseWeather { get { return m_useWeather; } set { m_useWeather = value; } }
    public static bool UseBloom { get{ return m_useBloom; } set { m_useBloom = value; CameraMng.Instance.UseBloom = value; } }
    public static bool UseAmbient { get { return m_useAmbient; }set { m_useAmbient = value; CameraMng.Instance.UseAmbient = value; } }
    public static bool UseOutline { get { return m_useOutline; } set { m_useOutline = value; } }
    public static bool UseFog  { get { return m_useFog; } set { m_useFog = value; } }
    public static int FPS { get { return m_frameLevel; } set { m_frameLevel = value; Application.targetFrameRate = 15 + value*15; } }
    public static int Joystick { get { return m_joyStick; } set { m_joyStick = value; UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).InputWindow.Enabled(Joystick); } }
    public static int AutoAim { get { return m_autoAimLevel; } set { m_autoAimLevel = value; } }

    public static string SuperArmorIcon = "";
    public static string InvincibilityArmor = "";
    public static void SaveFileStream()
    {
        string Data = ID + "\n";
        Data += PWD + "\n";
        Data += ParsingToBoolean(UseMusic) + "\n";
        Data += MusicVolume + "\n";
        Data += ParsingToBoolean(UseSound) + "\n";
        Data += SoundVolume + "\n";
        Data += ParsingToBoolean(m_useWeather) + "\n";
        Data += ParsingToBoolean(m_useBloom) + "\n";
        Data += ParsingToBoolean(m_useAmbient) + "\n";
        Data += ParsingToBoolean(m_useOutline) + "\n";
        Data += ParsingToBoolean(m_useFog) + "\n";
        Data += ParsingToBoolean(PlayerCameraHoldRot) + "\n";
        Data += m_joyStick + "\n";
        Data += m_frameLevel + "\n";
        Data += m_autoAimLevel + "\n";
        File.WriteAllText(Path + "/Setting.txt", Data, System.Text.Encoding.UTF8);
    }

public static void Exit()
    {
        SceneMng.Instance.Exit();
        NetworkMng.Instance.Exit();
        PlayerMng.Instance.Exit();
        ItemMng.Instance.Exit();
    }
    public static void Init()
    {
#if UNITY_EDITOR
        Path = "Cache/UserSetting";
#else
        Path = Application.persistentDataPath + "/Cache/UserSetting";
#endif

        LoadUserSetting();
    }
    static void LoadUserSetting()
    {
        if (!Directory.Exists(Path))
            Directory.CreateDirectory(Path);

        FileStream fs;

        if (!File.Exists(Path + "/Setting.txt"))
        {
            fs = new FileStream(Path + "/Setting.txt", FileMode.Create);
            fs.Close();

            SaveFileStream();
        }
        else
            LoadFileStream();

        FPS = m_frameLevel;
    }
    public static void Reset()
    {
        DoubleTouchTime = 0.3f;
        MusicVolume = 0.8f;
        UseMusic = true;
        SoundVolume = 0.8f;
        UseSound = true;
    }
    static void LoadFileStream()
    {
        FileStream file = new FileStream(Path + "/Setting.txt", FileMode.Open);

        byte[] bytes = new byte[file.Length];
        try
        {
            bytes = new byte[file.Length];
            file.Read(bytes, 0, (int)file.Length);
        }
        catch (IOException e)
        {
            LogSystem.Log(LogType.Error, e.Message);
            file.Close();
            return;
        }

        string temp = System.Text.Encoding.UTF8.GetString(bytes);
        temp = temp.Trim();
        string[] item = temp.Split('\n');
        ID = item[0];
        PWD = item[1];
        UseMusic = item[2] == "1";
        MusicVolume = float.Parse(item[3]);
        UseSound = item[4] == "1";
        SoundVolume = float.Parse(item[5]);
        m_useWeather = item[6] == "1";
        m_useBloom = item[7] == "1";
        m_useAmbient = item[8] == "1";
        m_useOutline = item[9] == "1";
        m_useFog = item[10] == "1";
        PlayerCameraHoldRot = item[11]== "1";
        m_joyStick = int.Parse(item[12]);
        m_frameLevel = int.Parse(item[13]);
        m_autoAimLevel = int.Parse(item[14]);
        file.Close();
    }
    static string ParsingToBoolean(bool str)
    {
        if (str)
            return "1";
        else
            return "0";
    }
    static void WriteFileToString(FileStream file, string str)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        file.Write(bytes, 0, bytes.Length);
    }
}

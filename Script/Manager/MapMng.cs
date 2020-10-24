using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class SMatchPortal
{
    public Vector3 Coord;
    public List<int> HandleList = new List<int>();
}
public class SMapPortal
{
    public Vector3 Coord;
    public Vector3 DeltaCoord;
    public int Handle;
}
public class SMapMonster
{
    public Vector3 Coord;
    public float Angle;
    public int Handle;
    public float RespawnTime;
}
public class SMapNPC
{
    public Vector3 Coord;
    public int Handle;
}
public enum EMapType
{
    None,
    Public,
    Private,
}
public enum EMapAreaGroup
{
    None,
    TwilightDesert,
    EmpireOfEstela,
}
public class Map
{
    public int Handle;
    public EMapType Type;
    public EMapAreaGroup Group;
    public int MinLevel;
    public int Level;
    public int Number;
    public string MapName ;
    public string Information;
    public string MapImgPath = "Sprite/MapImg/";
    public string MapIconPath = "";
    public string SceneName = "";
    public string BGM = "";
    public Vector2 Size;
    public Vector3 WayPoint;
    public float CoordScaleFactorX = 0;
    public float CoordScaleFactorY = 0;

    public List<SMatchPortal> MatchPortalList = new List<SMatchPortal>();
    public List<SMapPortal> PortalList = new List<SMapPortal>();
    public List<SMapMonster> MonsterList = new List<SMapMonster>();
    public List<SMapNPC> NPCList = new List<SMapNPC>();
}

public class MapMng : TSingleton<MapMng>
{
    public Dictionary<int, Map> MapDic = new Dictionary<int, Map>();
    public Map CurrMap { get; private set; }
    DigitalRuby.RainMaker.RainScript m_rain;
    public void SetCurrMap(int handle)
    {
        CurrMap = MapDic[handle];
    }
    private void LateUpdate()
    {
        if (CurrMap == null)
            return;
        if (CurrMap.Group == EMapAreaGroup.None)
            return;

        if (GameSystem.UseWeather)
        {
            System.DateTime time = System.DateTime.Now;
            if (time.Minute % (float)time.Day >= (float)time.Day / 2)
            {
                m_rain.RainIntensity = Mathf.Clamp((time.Hour % (time.Minute + 12)) * 0.1f, 0, 1);
                m_rain.RainMistThreshold = Mathf.Clamp((time.Hour % (time.Minute + 12)) * 0.1f, 0, 1);
            }
            else
            {
                m_rain.RainIntensity = 0;
                m_rain.RainMistThreshold = 0;
            }
        }
        else
        {
            m_rain.RainIntensity = 0;
            m_rain.RainMistThreshold = 0;
        }

    }
    public override void Init()
    {
        m_rain = Instantiate(Resources.Load<DigitalRuby.RainMaker.RainScript>("Weather/Rain"));
        m_rain.Init();
        m_rain.transform.SetParent(transform);
        m_rain.Camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera;
#if UNITY_EDITOR
        TextAsset Asset = new TextAsset(System.IO.File.ReadAllText("Assets/AssetBundle_Database/DB_Map.json"));
        JSONNode Node = JSON.Parse(Asset.text);
#else
        JSONNode Node = JSON.Parse(AssetMng.Instance["database"].LoadAsset<TextAsset>("DB_Map").text);
#endif
        for (int i = 0; i < Node.Count; ++i)
        {
            Map map = new Map();
            map.Handle = int.Parse(Node[i]["Handle"]);
            map.Type = (EMapType)int.Parse(Node[i]["Area"]);
            map.Group = (EMapAreaGroup)int.Parse(Node[i]["Group"]);
            map.MapName = Node[i]["MapName"];
            map.Information = Node[i]["Information"];
            map.MinLevel = int.Parse(Node[i]["MinLevel"]);
            map.Level = int.Parse(Node[i]["Level"]);
            map.Number = int.Parse(Node[i]["Number"]);
            map.MapImgPath = Node[i]["MapImgPath"];
            map.MapIconPath = Node[i]["MapIconPath"];
            map.SceneName = Node[i]["SceneName"];
            map.BGM = Node[i]["BGM"];
            string[] SizeArr = Node[i]["Size"].Value.Split(',');
            map.Size = new Vector2(float.Parse(SizeArr[0]), float.Parse(SizeArr[1]));
            map.CoordScaleFactorX = 350 / map.Size.x;
            map.CoordScaleFactorY = 350 / map.Size.y;
            string[] WayPoint = Node[i]["WayPoint"].Value.Split(',');
            map.WayPoint = new Vector3(float.Parse(WayPoint[0]), float.Parse(WayPoint[1]), float.Parse(WayPoint[2]));
            if (Node[i]["Portal"] != "")
            {
                string[] PortalArr = Node[i]["Portal"].Value.Split('/');
                for (int j = 0; j < PortalArr.Length; ++j)
                { 
                    string[] PortalArr2 = PortalArr[j].Split(',');
                    SMapPortal portal = new SMapPortal();
                    portal.Handle = int.Parse(PortalArr2[0]);
                    portal.Coord = new Vector3(float.Parse(PortalArr2[1]), float.Parse(PortalArr2[2]), float.Parse(PortalArr2[3]));
                    portal.DeltaCoord = new Vector3(float.Parse(PortalArr2[4]), float.Parse(PortalArr2[5]), float.Parse(PortalArr2[6]));
                    map.PortalList.Add(portal);
                }
            }
            if (Node[i]["Monster"] != "")
            {
                string[] MonsterArr = Node[i]["Monster"].Value.Split('/');
                for (int j = 0; j < MonsterArr.Length; ++j)
                {
                    string[] MonsterArr2 = MonsterArr[j].Split(',');
                    SMapMonster portal = new SMapMonster();
                    portal.Handle = int.Parse(MonsterArr2[0]);
                    portal.RespawnTime = float.Parse(MonsterArr2[1]);
                    portal.Coord = new Vector3(float.Parse(MonsterArr2[2]), float.Parse(MonsterArr2[3]), float.Parse(MonsterArr2[4]));
                    portal.Angle = float.Parse(MonsterArr2[5]);
                    map.MonsterList.Add(portal);
                }
            }
            if (Node[i]["MatchPortal"] != "")
            {
                string[] PortalArr = Node[i]["MatchPortal"].Value.Split('/');
                for (int j = 0; j < PortalArr.Length; ++j)
                {
                    string[] PortalArr2 = PortalArr[j].Split(',');
                    SMatchPortal portal = new SMatchPortal();
                    portal.Coord = new Vector3(float.Parse(PortalArr2[0]), float.Parse(PortalArr2[1]), float.Parse(PortalArr2[2]));
                    for (int k = 3; k < PortalArr2.Length; ++k)
                        portal.HandleList.Add(int.Parse(PortalArr2[k]));
                    map.MatchPortalList.Add(portal);
                }
            }
            //if (Node[i]["NPC"] != "")
            //{
            //    string[] NPCArr = Node[i]["NPC"].Value.Split('/');
            //    for (int j = 0; j < NPCArr.Length; ++j)
            //    {
            //        string[] NPCArr2 = NPCArr[j].Split(',');
            //        SMapNPC portal = new SMapNPC();
            //        portal.Handle = int.Parse(NPCArr2[0]);
            //        portal.Coord = new Vector3(float.Parse(NPCArr2[1]), float.Parse(NPCArr2[2]), float.Parse(NPCArr2[3]));
            //        map.NPCList.Add(portal);
            //    }
            //}
            MapDic.Add(map.Handle, map);
        }

        IsLoad = true;
    }
}

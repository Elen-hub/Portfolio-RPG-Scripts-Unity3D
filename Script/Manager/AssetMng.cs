using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using SimpleJSON;

#if !UNITY_EDITOR
public class AssetMng : TSingleton<AssetMng>
{
    string path;
    string directory;

    struct AssetInfo
    {
        public string Name;
        public int Version;
        public string URI;
        public float Size;
        public AssetInfo(string name, int version, string uri, float size)
        {
            Name = name;
            Version = version;
            URI = uri;
            Size = size;
        }
    }

    Dictionary<string, AssetBundle> m_assetBundleDic = new Dictionary<string, AssetBundle>();
    Dictionary<string, AssetInfo> m_assetInfoDic = new Dictionary<string, AssetInfo>();
    public AssetBundle this[string key] {
        get {
            if (m_assetBundleDic.ContainsKey(key)) return m_assetBundleDic[key];
            else return null;
        }
    }
    public override void Init()
    {
#if UNITY_EDITOR
        path = "Cache";
        directory = "Assets/AssetBundle";
#else
        directory = Application.persistentDataPath + "/Cache/Bundle";
        path = Application.persistentDataPath + "/Cache";
#endif

        StartCoroutine(SetPatchFile());
    }
    void LoadPatchFile(FileStream file, string Path)
    {
        byte[] bytes = new byte[file.Length];
        file.Read(bytes, 0, (int)file.Length);

        string temp = System.Text.Encoding.UTF8.GetString(bytes);
        temp = temp.Trim();
        string[] item = temp.Split('\n');
        for(int i =0; i<item.Length; ++i)
        {
            if (item[i] == "")
                break;
            string[] items = item[i].Split(',');
            string name = items[0];
            int version = int.Parse(items[1]);
            m_assetInfoDic.Add(name, new AssetInfo(name, version, "", 0));
        }
    }
    void SavePatchFile(FileStream file, List<string> patches)
    {
        string textArr = string.Join("\n", patches.ToArray());
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(textArr);
        file.Write(bytes, 0, bytes.Length);
    }
    public Coroutine LoadAsset(LoadingScene loading, string name)
    {
        return StartCoroutine(LoadAssetInDisk(loading, directory, name));
    }
    public void UnLoadAsset(string name)
    {
        if (m_assetBundleDic.ContainsKey(name))
        {
            AssetBundle bundle = m_assetBundleDic[name];
            m_assetBundleDic.Remove(name);
            bundle.Unload(false);
        }
    }
    IEnumerator SetPatchFile()
    {
        LoadingScene loading = UIMng.Instance.Open<LoadingScene>(UIMng.UIName.LoadingScene);
        loading.SetText = "패치정보를 확인중입니다.";

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        FileStream fs;
        if (File.Exists(path + "/Assets.txt"))
        {
            fs = new FileStream(path + "/Assets.txt", FileMode.Open);
            LoadPatchFile(fs, path);
            fs.Close();
        }
        UnityWebRequest request = UnityWebRequest.Get("https://drive.google.com/uc?export=download&id=19RxuHtkl1T6XCKjirGBkDoXxyealIoy5");
        UnityWebRequestAsyncOperation async = request.SendWebRequest();
        while (!async.isDone)
        {
            yield return null;
            loading.Progress = async.progress;
        }
        DownloadHandler handler = request.downloadHandler;
        string asset = handler.text;
        Dictionary<string, AssetInfo> AssetInfoDic = new Dictionary<string, AssetInfo>();

        JSONNode Node = JSON.Parse(asset);
        for(int i =0; i<Node.Count; ++i)
        {
            string name = Node[i]["name"];
            int version = int.Parse(Node[i]["version"]);
            string uri = Node[i]["uri"];
            float size = float.Parse(Node[i]["size"]);
            AssetInfoDic.Add(name, new AssetInfo(name, version, uri, size));
        }
        List<string> patches = new List<string>();

        // AssetInfoDic = Web VersionFile, m_assetInfoDic = Local VersionFile
        foreach (AssetInfo info in AssetInfoDic.Values)
        {
            if (info.Name == "database")
                continue;
            // 누락되거나 버전이 다르면 다운로드
            if (!File.Exists(directory + "/" + info.Name) || !m_assetInfoDic.ContainsKey(info.Name) || m_assetInfoDic[info.Name].Version != info.Version)
                yield return StartCoroutine(SaveAsset(loading, directory, info.Name, info.URI));

            // 로컬버전파일 갱신
            patches.Add(info.Name + "," + info.Version);
            fs = new FileStream(path + "/Assets.txt", FileMode.Create);
            SavePatchFile(fs, patches);
            fs.Close();
        }

        AssetBundle.UnloadAllAssetBundles(true);

        yield return LoadAsset(loading, "character");
        yield return StartCoroutine(LoadAssetInWeb(loading, "database", AssetInfoDic["database"].URI));

        m_assetInfoDic.Clear();
        IsLoad = true;

        CameraMng.Fade_ON();
        UIMng.Instance.DESTROY = UIMng.UIName.Logo;
        loading.Close();
        SceneMng.Instance.SetCurrScene(EScene.TitleScene);
    }
    IEnumerator LoadAssetInWeb(LoadingScene loading, string name, string uri)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, 0);
        UnityWebRequestAsyncOperation async = request.SendWebRequest();
        loading.SetText = "서버로부터 정보를 받는 중입니다. (" + name + ")";
        while (!async.isDone)
        {
            yield return null;
            loading.Progress = async.progress;
        }
        m_assetBundleDic.Add(name, DownloadHandlerAssetBundle.GetContent(request));
    }
    IEnumerator LoadAssetInDisk(LoadingScene loading, string directory, string name)
    {
        loading.SetText = "파일을 불러오는 중입니다. (" + name + ")";
        string path = directory + "/" + name;
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        while (!request.isDone)
        {
            yield return null;
            loading.Progress = request.progress;
        }
        AssetBundle bundle = request.assetBundle;
        m_assetBundleDic.Add(name, bundle);
    }
    IEnumerator SaveAsset(LoadingScene loading, string directory, string name, string uri)
    {
        loading.SetText = "파일을 다운로드 받는 중입니다. (" + name + ")";
        string path = directory + "/" + name;
        UnityWebRequest request = UnityWebRequest.Get(uri);
        UnityWebRequestAsyncOperation async = request.SendWebRequest();
        while(!async.isDone)
        {
            yield return null;
            loading.Progress = async.progress;
        }
        FileStream file = new FileStream(path, FileMode.Create);
        file.Write(request.downloadHandler.data, 0, (int)request.downloadedBytes);
        file.Close();
        request.Dispose();
    }
}
#endif
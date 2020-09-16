using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EScene
{
    None,
    LogoScene,
    TitleScene,
    MainScene,
    CreateScene,
    GameScene,
}

public class SceneMng : TSingleton<SceneMng>
{
    public bool IsSceneLoad;
    EScene m_currScene = 0;
    Dictionary<EScene, BaseScene> m_sceneList = new Dictionary<EScene, BaseScene>();
    EMapAreaGroup CurrLoadArea;

    public void AddScript<T>(EScene key) where T : BaseScene
    {
        if (m_sceneList.ContainsKey(key))
            return;

        GameObject obj = new GameObject(typeof(T).ToString(), typeof(T));
        obj.transform.parent = transform;
        m_sceneList.Add(key, obj.GetComponent<T>());
    }
    public void SetCurrScene(EScene scene)
    {
        if (m_currScene == scene)
            return;

        foreach (KeyValuePair<EScene, BaseScene> k in m_sceneList)
            k.Value.gameObject.SetActive(false);

        if (m_sceneList.ContainsKey(m_currScene))
            m_sceneList[m_currScene].Exit();

        if (!m_sceneList.ContainsKey(scene))
            return;

        m_currScene = scene;
        m_sceneList[scene].gameObject.SetActive(true);
        m_sceneList[scene].Enter();
    }
    public void SetGameScene(string sceneName, System.Action after)
    {
        IsSceneLoad = false;
        m_currScene = EScene.GameScene;
        StartCoroutine(SceneChange(sceneName, after));
    }
    IEnumerator SceneChange(string name, System.Action after)
    {
#if UNITY_EDITOR
        CameraMng.Fade_ON(1);
        yield return new WaitForSeconds(1);
        UIMng.Instance.Open<Game>(UIMng.UIName.Game).SubWindow.MapWindow.SetMap();

        AsyncOperation async = SceneManager.LoadSceneAsync(name);

        while (!async.isDone)
            yield return null;

        if (after != null)
            after?.Invoke();

        IsSceneLoad = true;

        CameraMng.Fade_OFF(1);
        yield return new WaitForSeconds(1);
        UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).MapNameWindow.Enabled();
        yield return null;
#else
        CameraMng.Fade_ON(1);
        yield return new WaitForSeconds(1);
        UIMng.Instance.Open<Game>(UIMng.UIName.Game).SubWindow.MapWindow.SetMap();
        LoadingScene loadingUI = UIMng.Instance.Open<LoadingScene>(UIMng.UIName.LoadingScene);
        EMapAreaGroup prevArea = MapMng.Instance.CurrMap.Group;
        if (CurrLoadArea != prevArea)
        {
            if(CurrLoadArea == 0)
                CurrLoadArea = prevArea;

            yield return AssetMng.Instance.LoadAsset(loadingUI, "scene_" + prevArea);
        }

        loadingUI.SetText = "맵을 불러오는 중입니다.";
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false;
        float elapsedTime = 0;
        while (!async.isDone)
        {
            yield return null;
            if (async.progress < 0.9f)
            {
                loadingUI.Progress = async.progress;
            }
            else
            {
                elapsedTime += Time.unscaledDeltaTime;
                loadingUI.Progress = Mathf.Lerp(0.9f, 1, elapsedTime);

                if (loadingUI.Progress >= 1)
                    async.allowSceneActivation = true;
            }
        }

        if (CurrLoadArea != prevArea)
        {
            AssetMng.Instance.UnLoadAsset("scene_" + CurrLoadArea);
            CurrLoadArea = prevArea;
        }

        if (after != null)
            after?.Invoke();

        IsSceneLoad = true;
        UIMng.Instance.CLOSE = UIMng.UIName.LoadingScene;

        CameraMng.Fade_OFF(1);
        yield return new WaitForSeconds(1);
        UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).MapNameWindow.Enabled();
        yield return null;
#endif
    }
    public override void Init()
    {
        AddScript<LogoScene>(EScene.LogoScene);
        AddScript<TitleScene>(EScene.TitleScene);
        IsLoad = true;
    }
    public void Exit()
    {
        SetCurrScene(EScene.TitleScene);
#if !UNITY_EDITOR
        AssetMng.Instance.UnLoadAsset("scene_" + CurrLoadArea);
#endif
        CurrLoadArea = 0;
    }
}

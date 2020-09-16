using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene {

    public override void Enter()
    {
        LoadSceneAsync(EScene.TitleScene, null, After);
    }

    void After()
    {
        CameraMng.Fade_OFF(0.5f);
        CameraMng.Instance.SetCamera(CameraMng.CameraStyle.UI | CameraMng.CameraStyle.World);
        StartCoroutine(Loading());
    }

    public override void Exit()
    {

    }

    IEnumerator Loading()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        if (!NetworkMng.IsLoad)
        {
            NetworkMng.Instance.Init();
            while (!NetworkMng.IsLoad)
                yield return wait;
        }
        if (!DBMng.IsLoad)
        {
            DBMng.Instance.Init();
            while (!DBMng.IsLoad)
                yield return wait;
        }
        if (!PlayerMng.IsLoad)
        {
            PlayerMng.Instance.Init();
            while (!PlayerMng.IsLoad)
                yield return wait;
        }
        if (!ItemMng.IsLoad)
        {
            ItemMng.Instance.Init();
            while (!ItemMng.IsLoad)
                yield return wait;
        }
        if (!PlayerMng.IsLoad)
        {
            PlayerMng.Instance.Init();
            while (!PlayerMng.IsLoad)
                yield return wait;
        }
        if (!CharacterMng.IsLoad)
        {
            CharacterMng.Instance.Init();
            while (!CharacterMng.IsLoad)
                yield return wait;
        }
        if (!MapMng.IsLoad)
        {
            MapMng.Instance.Init();
            while (!MapMng.IsLoad)
                yield return wait;
        }
        UIMng.Instance.OPEN = UIMng.UIName.Title;
        yield return null;
    }
}

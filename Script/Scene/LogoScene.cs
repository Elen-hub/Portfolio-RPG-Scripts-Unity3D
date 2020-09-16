using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoScene : BaseScene {

    public override void Enter()
    {
        UIMng.Instance.Init();
        UIMng.Instance.OPEN = UIMng.UIName.Logo;

        LoadSceneAsync(EScene.LogoScene, null, After);
    }

    void After()
    {
        CameraMng.Fade_OFF(1f);
        StartCoroutine(LogoSceneUpdate());
    }

    public override void Exit()
    {

    }

    IEnumerator LogoSceneUpdate()
    {
        yield return new WaitForSeconds(1);
        // UIMng.Instance.GetUI<Logo>(UIMng.UIName.Logo).PlayUISound();
        yield return new WaitForSeconds(2);
        CameraMng.Fade_ON(1f);
        yield return new WaitForSeconds(1);
#if UNITY_EDITOR
        SceneMng.Instance.SetCurrScene(EScene.TitleScene);
        UIMng.Instance.DESTROY = UIMng.UIName.Logo;
#else
        AssetMng.Instance.Init();
#endif
        CameraMng.Fade_OFF();
    }
}

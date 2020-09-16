using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : BaseScene {
    public override void Enter()
    {
        //CameraMng.Instance.CameraDic[CameraMng.CameraTypes.Game].enabled = false;
        //CameraMng.Instance.CurrCamera = CameraMng.CameraTypes.Game;
        //CameraMng.Fade_ON(0);
        //System.GC.Collect();

        //Resources.UnloadUnusedAssets();
        //LoadSceneAsync(Scene.MainScene, null, After);
    }

    void After()
    {
        //CameraMng.Instance.CameraDic[CameraMng.CameraTypes.Game].enabled = true;
        //Camera.main.transform.position = new Vector3(4, 6, 10.5f);
        //Camera.main.transform.eulerAngles = new Vector3(15, 90, 0);
        //CameraMng.Instance.SetBloom(0.8f, 0.25f, 0.3f, 4);
        //CameraMng.Fade_OFF(2);
        //UIMng.Instance.CLOSE = UIMng.UIName.Title;
        //UIMng.Instance.OPEN = UIMng.UIName.Main;
    }

    public override void Exit()
    {

    }
}

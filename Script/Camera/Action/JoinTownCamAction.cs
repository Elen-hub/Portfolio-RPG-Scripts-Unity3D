using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinTownCamAction : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(CameraAction());
    }
    IEnumerator CameraAction()
    {
        yield return null;
        UIMng.Instance.CLOSE = UIMng.UIName.Game;
        WorldCamera worldCam = CameraMng.Instance.GetCamera<WorldCamera>(CameraMng.CameraStyle.World);
        worldCam.Enabled = true;
        yield return worldCam.StartAction("JoinAction_" + MapMng.Instance.CurrMap.SceneName);
        worldCam.Enabled = false;
        UIMng.Instance.OPEN = UIMng.UIName.Game;
    }
}

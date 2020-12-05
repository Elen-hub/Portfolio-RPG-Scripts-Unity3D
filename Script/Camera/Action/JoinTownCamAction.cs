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
        UIMng.Instance.CLOSE = UIMng.UIName.Game;
        yield return null;
        WorldCamera worldCam = CameraMng.Instance.GetCamera<WorldCamera>(CameraMng.CameraStyle.World);
        yield return worldCam.StartAction("JoinAction_" + MapMng.Instance.CurrMap.SceneName);
        UIMng.Instance.OPEN = UIMng.UIName.Game;
    }
}

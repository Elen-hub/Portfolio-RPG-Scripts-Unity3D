using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraAction))]
public class WorldCamera : BaseCamera
{
    delegate IEnumerator ReturnMethod();

    CameraAction m_cameraAction;
    
    public override void Init()
    {
        base.Init();
        
        m_cameraAction = GetComponent<CameraAction>();
    }
    public Coroutine StartAction(string methodName)
    {
        return StartCoroutine((System.Delegate.CreateDelegate(typeof(ReturnMethod), m_cameraAction, methodName) as ReturnMethod)());
    }
}

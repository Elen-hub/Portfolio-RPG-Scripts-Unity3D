using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraAction))]
public class WorldCamera : BaseCamera
{
    public delegate IEnumerator ReturnMethod();

    CameraAction m_cameraAction;
    
    public override void Init()
    {
        base.Init();
        
        m_cameraAction = GetComponent<CameraAction>();
    }
    public ReturnMethod FindAction(string methodName)
    {
        try
        {
            System.Delegate del = System.Delegate.CreateDelegate(typeof(ReturnMethod), m_cameraAction, methodName);
            if (del != null) 
                return del as ReturnMethod;
        }
        catch
        {
            return null;
        }
        return null;
    }
}

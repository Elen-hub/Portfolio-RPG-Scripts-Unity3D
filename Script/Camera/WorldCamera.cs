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
            System.Reflection.MethodInfo info = typeof(CameraAction).GetMethod(methodName);
            ReturnMethod del = info.CreateDelegate(typeof(ReturnMethod), m_cameraAction) as ReturnMethod;
            if (del != null)
                return del;
        }
        catch
        {
            return null;
        }
        return null;
    }
}

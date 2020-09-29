using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : BaseCamera
{
    public Camera UICamera;
    Renderer m_renderer;
    Transform m_character;
    Vector3 m_prevObjectPos;
    Vector3 m_startCameraPos;
    Vector3 m_prevCameraPos;
    Vector3 m_targetCameraPos;
    bool m_isLerp;
    bool m_isLook;
    float m_lookPercent;
    float m_cameraLookElapsedTime;

    float m_elapsedTime;
    public override void Init()
    {
        base.Init();
        UICamera = transform.Find("UI Render").GetComponent<Camera>();
        SetCameraPos();
    }
    public void SetCameraPos()
    {
        camera.transform.localPosition = new Vector3(0, GameSystem.PlayerCameraHeight, -GameSystem.PlayerCameraWidth);
        camera.transform.eulerAngles = new Vector3(GameSystem.PlayerCameraAngle, 0, 0);
        UICamera.transform.localPosition = new Vector3(0, GameSystem.PlayerCameraHeight, -GameSystem.PlayerCameraWidth);
        UICamera.transform.eulerAngles = new Vector3(GameSystem.PlayerCameraAngle, 0, 0);
    }
    void Update()
    {
        if (!m_character)
        {
            if(PlayerMng.Instance.MainPlayer.Character)
                m_character = PlayerMng.Instance.MainPlayer.Character.transform;
            return;
        }
        //if(!GameSystem.PlayerCameraHoldRot)
        //    transform.forward = m_character.forward;

        if (m_isLerp)
        {
            m_elapsedTime += Time.deltaTime / GameSystem.PlayerCameraMoveTime;
            transform.position = Vector3.Lerp(m_prevObjectPos, m_character.position, m_elapsedTime);
            if (m_elapsedTime > 1)
                m_isLerp = false;
        }
        else
        {
            if (Vector3.Distance(transform.position, m_character.position) < GameSystem.PlayerCameraMoveDistance)
            {
                transform.position = m_character.position;
            }
            else
            {
                m_prevObjectPos = transform.position;
                m_elapsedTime = 0;
                m_isLerp = true;
            }
        }

        // Camera LookUp Frame
        if (m_isLook)
        {
            m_cameraLookElapsedTime += Time.deltaTime;
            camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, m_targetCameraPos, m_cameraLookElapsedTime*1);
            if(m_cameraLookElapsedTime>=0.75f)
            {
                m_cameraLookElapsedTime = 0;
                m_isLook = false;
            }
        }
        else if(m_prevCameraPos != Vector3.zero)
        {
            m_cameraLookElapsedTime += Time.deltaTime;
            camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, m_prevCameraPos, m_cameraLookElapsedTime*0.33f);
            if (m_cameraLookElapsedTime >= 3)
                m_prevCameraPos = Vector3.zero;
        }

        UICamera.transform.localPosition = camera.transform.localPosition;
    }
    public void CameraAction_Look(float percent)
    {
        m_isLook = true;
        m_cameraLookElapsedTime = 0;

        if (m_prevCameraPos == Vector3.zero)
        {
            m_targetCameraPos = camera.transform.localPosition * percent;
            m_prevCameraPos = camera.transform.localPosition;
        }
        else
            m_targetCameraPos = m_prevCameraPos * percent;
    }
}

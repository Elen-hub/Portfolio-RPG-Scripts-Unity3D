using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : BaseCamera
{
    public Camera UICamera;
    Renderer m_renderer;
    Transform m_character;
    Vector3 m_prevPos;
    bool m_isLerp;
    float m_elapsedTime;
    public override void Init()
    {
        base.Init();
        UICamera = transform.Find("UI Camera").GetComponent<Camera>();
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
        if(!GameSystem.PlayerCameraHoldRot)
            transform.forward = m_character.forward;

        if (m_isLerp)
        {
            m_elapsedTime += Time.deltaTime / GameSystem.PlayerCameraMoveTime;
            transform.position = Vector3.Lerp(m_prevPos, m_character.position, m_elapsedTime);
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
                m_prevPos = transform.position;
                m_elapsedTime = 0;
                m_isLerp = true;
            }
        }
    }
}

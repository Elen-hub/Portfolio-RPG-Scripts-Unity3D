﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TargetUI : BaseFieldUI
{
    Transform m_camera;
    BaseCharacter m_character;
    Vector3 m_eulerAngle;
    public override void Init(FieldUI.Register register)
    {
        m_camera = CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).UICamera.transform;
    }

    public void Enabled(BaseCharacter target)
    {
        m_character = target;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_character = null;
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if (m_character == null || !m_character.gameObject.activeSelf)
        {
            Disabled();
            return;
        }
        m_eulerAngle.x = 90 - m_camera.eulerAngles.x;
        m_eulerAngle.z += Time.deltaTime * 180;
        transform.localEulerAngles = m_eulerAngle;
        transform.position = m_character.transform.position;
    }
}

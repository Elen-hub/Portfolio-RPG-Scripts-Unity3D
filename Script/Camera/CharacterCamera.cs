using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : BaseCamera
{
    Transform m_character;
    Vector3 m_prevPos;
    float m_elapsedTime;
    public void SetCharacter(Transform character)
    {
        m_character = character;
    }
    public override void Init()
    {
        base.Init();
    }
    void LateUpdate()
    {
        if (!m_character)
        {
            if (PlayerMng.Instance.MainPlayer.Character)
                m_character = PlayerMng.Instance.MainPlayer.Character.transform;
            return;
        }
        transform.localPosition = m_character.position + m_character.forward * 2;
        transform.LookAt(m_character);
    }
}

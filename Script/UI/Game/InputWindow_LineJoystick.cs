using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputWindow_LineJoystick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    PointerEventData m_eventData;
    bool m_isDown;
    Vector2 m_axis = Vector2.zero;
    Vector3 m_deltaVector = new Vector3(200, 200,0);
    Camera m_camera;
    Image m_buttonImg;
    Image m_currButtonImg;

    float m_holdAngle;
    BaseCharacter m_character;
    public void Init()
    {
        m_character = PlayerMng.Instance.MainPlayer.Character;
        m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.UI).camera;
        m_buttonImg = transform.Find("Image").Find("ButtonImg").GetComponent<Image>();
        m_buttonImg.enabled = false;
        m_currButtonImg = m_buttonImg.transform.Find("CurrButtonImg").GetComponent<Image>();
        m_currButtonImg.enabled = false;
        Disabled();
    }
    public void Enabled()
    {
        m_character = PlayerMng.Instance.MainPlayer.Character;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_isDown = false;
        // NetworkMng.Instance.NotifyCharacterState_Idle();
        m_buttonImg.rectTransform.localPosition = Vector2.zero;
        m_buttonImg.enabled = false;
        m_currButtonImg.enabled = false;
        SetJoystickButtonAnchor(Vector2.zero);
        m_character.State = BaseCharacter.CharacterState.Idle;
        gameObject.SetActive(false);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        m_isDown = true;
        m_eventData = eventData;
        m_buttonImg.rectTransform.localPosition = new Vector3(m_eventData.position.x, m_eventData.position.y) - m_deltaVector;
        m_buttonImg.enabled = true;
        m_currButtonImg.enabled = true;
        if (!GameSystem.PlayerCameraHoldRot)
            m_holdAngle = m_character.transform.eulerAngles.y;
        else
            m_holdAngle = 0;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        m_isDown = false;
        m_eventData = eventData;
        m_buttonImg.rectTransform.localPosition = Vector2.zero;
        m_buttonImg.enabled = false;
        m_currButtonImg.enabled = false;
        SetJoystickButtonAnchor(Vector2.zero);
    }
    void SetJoystickButtonAnchor(Vector2 pos)
    {
        if (pos == Vector2.zero && m_character.State == BaseCharacter.CharacterState.Move)
            NetworkMng.Instance.NotifyCharacterState_Idle();

        m_currButtonImg.rectTransform.anchoredPosition = pos;
        m_axis = pos;
    }
    private void Update()
    {
        if (m_isDown)
        {
            if (m_character.State == BaseCharacter.CharacterState.Death)
                return;

            if (m_character.IsStun || m_character.IsHit || m_character.IsNuckback)
                return;

            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_buttonImg.rectTransform,
                   m_eventData.position,
                   m_eventData.pressEventCamera,
                   out pos))
            {
                pos.x = (pos.x / m_buttonImg.rectTransform.sizeDelta.x)*2;
                pos.y = (pos.y / m_buttonImg.rectTransform.sizeDelta.y)*2;

                if (pos.magnitude > 1.0f)
                    pos.Normalize();

                SetJoystickButtonAnchor(pos * 100);
                m_axis = pos;

                if (!m_character.AttackSystem.CompleteAttack)
                    return;
                
                if (Mathf.Abs(m_axis.x) > 0.2f || Mathf.Abs(m_axis.y) > 0.2f)
                {
                    if (m_axis.x >= 0)
                    {
                        float Angle = Vector2.Angle(Vector2.up, m_axis) + m_holdAngle;
                        m_character.SetAngle(Angle);
                        m_character.State = BaseCharacter.CharacterState.Move;
                    }
                    else
                    {
                        float Angle = Vector2.Angle(-Vector2.up, m_axis) + 180 + m_holdAngle;
                        m_character.SetAngle(Angle);
                        m_character.State = BaseCharacter.CharacterState.Move;
                    }
                }
            }
        }
    }
}

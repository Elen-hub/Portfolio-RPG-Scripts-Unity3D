using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

// IDragHandler,IPointerUpHandler, IPointerDownHandler 를 사용하기 위해 UnityEngine.EventSystems를 추가
public partial class InputWindow_BasicJoystick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler 
{
    // 이미지 설정에 대한 설명
    // 1. 백그라운드의 이미지를 왼쪽 하단으로 설정합니다.
    // 2. 백그라운드의 앵커를 0.5, 0.5 로 설정합니다.
    // 이렇게 설정한다면, 포지션값은 좌측이 -0.5로 시작해서 우측은 0.5로 끝나게 됩니다.

    PointerEventData m_eventData;
    Camera m_camera;
	private Image m_backgroundImg;
	private Image m_buttonImg;
	private Vector2 m_axis = Vector2.zero;

    BaseCharacter m_character;

    bool m_isDown;
    float m_holdAngle;
    public void Init()
    {
        m_character = PlayerMng.Instance.MainPlayer.Character;
        m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.UI).camera;
        m_backgroundImg = GetComponent<Image>();
        m_buttonImg = transform.Find("Button").GetComponent<Image>();
        m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.UI).camera;
    }
    public void Enabled()
    {
        m_character = PlayerMng.Instance.MainPlayer.Character;
        gameObject.SetActive(true);
    }

    public void Disabled()
    {
        m_isDown = false;
        SetJoystickButtonAnchor(Vector2.zero);
        // m_character.State = BaseCharacter.CharacterState.Idle;
        gameObject.SetActive(false);
    }
	public void OnPointerDown (PointerEventData eventData)
	{
        m_isDown = true;
        m_eventData = eventData;
        if (!GameSystem.PlayerCameraHoldRot)
            m_holdAngle = m_character.transform.eulerAngles.y;
        else
            m_holdAngle = 0;
    }
	public void OnPointerUp (PointerEventData eventData)
	{
        m_isDown = false;
        NetworkMng.Instance.NotifyCharacterState_Idle();
        SetJoystickButtonAnchor (Vector2.zero);
    }
    // 버튼 이미지의 앵커 값을 변경합니다.
	void SetJoystickButtonAnchor( Vector2 pos )
	{
        m_buttonImg.rectTransform.anchoredPosition = pos;
        m_axis = pos;
    }

    void Update()
    {
        if (m_isDown)
        {
            if (m_character.State == BaseCharacter.CharacterState.Death)
                return;

            if (m_character.IsStun || m_character.IsHit || m_character.IsNuckback)
                return;

            Vector2 pos;

            // ScreenPointToLocalPointInRectangle 함수는 좌표를 Transform의 로컬 좌표계로 바꿔주는 함수입니다.
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_backgroundImg.rectTransform,
                   m_eventData.position,
                   m_eventData.pressEventCamera,
                   out pos))
            {
                //( 설정된 영역으로부터의 떨어진 거리 / 140 ) * 2
                pos.x = (pos.x / m_backgroundImg.rectTransform.sizeDelta.x)*2;
                pos.y = (pos.y / m_backgroundImg.rectTransform.sizeDelta.y)*2;

                // 백터의 길이값을 확인 했을때 1이 넘어갈 경우 단위백터로 설정합니다.
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

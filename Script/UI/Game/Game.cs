using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Game : BaseUI
{
    // Game_Joystick m_joyStick;
    public CharacterWindow CharacterWindow;
    public MainWindow MainWindow;
    public SubWindow SubWindow;
    public ChatWindow ChatWindow;
    public DeviceWindow DeviceWindow;
    public InputWindow InputWindow;
    public MapNameWindow MapNameWindow;
    public InfoWindow InfoWindow;
    List<RaycastResult> m_hitList = new List<RaycastResult>();
    GameObject m_exitBTN;

    float m_inputScreenPoint;
    Vector3 m_deltaScroll;
    bool m_useDragCameraRot;

    public override void Init()
    {
        base.Init();

        //m_joyStick = GetComponentInChildren<Game_Joystick>();
        //m_joyStick.Init();
        MainWindow = GetComponentInChildren<MainWindow>();
        MainWindow.Init();
        SubWindow = GetComponentInChildren<SubWindow>();
        SubWindow.Init();
        ChatWindow = GetComponentInChildren<ChatWindow>();
        ChatWindow.Init();
        InputWindow = GetComponentInChildren<InputWindow>();
        InputWindow.Init();
        MapNameWindow = GetComponentInChildren<MapNameWindow>();
        MapNameWindow.Init();
        InfoWindow = GetComponentInChildren<InfoWindow>();
        InfoWindow.Init();
        m_exitBTN = transform.Find("ExitMap").gameObject;
        m_exitBTN.GetComponent<Button>().onClick.AddListener(() => UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).NormalPopup.Enabled(NetworkMng.Instance.RequestExitPrivateMap, "나가기", null, "취소", "마을로 돌아갑니다."));
        CharacterWindow = GetComponentInChildren<CharacterWindow>();
        CharacterWindow.Init();
        DeviceWindow = GetComponentInChildren<DeviceWindow>();
        DeviceWindow.Init();

        m_deltaScroll = Vector3.zero;
    }
    public override void Open()
    {
        m_exitBTN.SetActive(MapMng.Instance.CurrMap.Type == EMapType.Private);
        InputWindow.Enabled(GameSystem.Joystick);
        DeviceWindow.Enabled();

        gameObject.SetActive(true);
    }
    public override void Close()
    {
        InputWindow.Disabled();
        MapNameWindow.Disabled();
        DeviceWindow.Disabled();

        gameObject.SetActive(false);
    }
    void LateUpdate()
    {
#if !UNITY_EDITOR
        if (0 < Input.touchCount)
            for (int t = 0; t < 1; ++t)
            {
                if (Input.GetTouch(t).phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    
                    m_hitList.Clear();
                    PointerEventData data = new PointerEventData(EventSystem.current);
                    data.position = Input.mousePosition;
                    EventSystem.current.RaycastAll(data, m_hitList);

                    if (m_hitList.Count != 0)
                        return;

                    if (Physics.Raycast(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera.ScreenPointToRay(Input.mousePosition), out hit, 15f, 
                        1 << LayerMask.NameToLayer("NPC") | 1 << LayerMask.NameToLayer("Ally") | 1 << LayerMask.NameToLayer("Hero") | 1 << LayerMask.NameToLayer("Enermy"), QueryTriggerInteraction.Collide))
                    {
                        switch (hit.transform.tag)
                        {
                            case "NPC":
                                BaseCharacter hero = PlayerMng.Instance.MainPlayer.Character;
                                hero.MoveSystem.SetMoveToTarget(hit.transform, 1, () => {
                                    UIMng.Instance.CLOSE = UIMng.UIName.Game;
                                    UIMng.Instance.Open<NPCUI>(UIMng.UIName.NPCUI).Enabled(hit.transform.GetComponent<BaseNPC>());
                                });
                                break;
                            case "Enermy":
                                InfoWindow.Enabled(hit.transform.GetComponent<BaseCharacter>(), true);
                                break;
                            default:
                                BaseHero character = hit.transform.GetComponent<BaseHero>();
                                InfoWindow.Enabled(character, character.Name, true);
                                break;
                        }
                        //switch (hit.transform.tag)
                        //{
                        //    case "NPC":
                        //        BaseCharacter hero = PlayerMng.Instance.MainPlayer.Character;
                        //        hero.MoveSystem.SetMoveToTarget(hit.transform, 1, () => {
                        //            UIMng.Instance.CLOSE = UIMng.UIName.Game;
                        //            UIMng.Instance.Open<NPCUI>(UIMng.UIName.NPCUI).Enabled(hit.transform.GetComponent<BaseNPC>());
                        //        });
                        //        break;
                        //    case "Ally":
                        //    case "Hero":
                        //    case "Enermy":
                        //        InfoWindow.Enabled(hit.transform.GetComponent<BaseCharacter>(), true);
                        //        break;
                        //}
                    }
                }
                //else if (Input.GetTouch(t).phase == TouchPhase.Ended)  { }
                //else if (Input.GetTouch(t).phase == TouchPhase.Moved || Input.GetTouch(t).phase == TouchPhase.Stationary) { }
            }
#else
        // 카메라 회전 시작
        if (Input.GetMouseButtonDown(0))
        {
            if (!GameSystem.PlayerCameraHoldRot)
            {
                m_hitList.Clear();
                PointerEventData data = new PointerEventData(EventSystem.current);
                data.position = Input.mousePosition;
                EventSystem.current.RaycastAll(data, m_hitList);

                if (m_hitList.Count != 0)
                    return;

                m_useDragCameraRot = true;
                m_inputScreenPoint = Input.mousePosition.x;
            }
        }
        // 카메라 회전 중 이라면
        if (m_useDragCameraRot)
        {
            if (!GameSystem.PlayerCameraHoldRot)
            {
                m_deltaScroll.y = m_inputScreenPoint - Input.mousePosition.x;
                m_inputScreenPoint = Input.mousePosition.x;
                CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).transform.eulerAngles += m_deltaScroll * Time.deltaTime * 5;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;

            m_hitList.Clear();
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = Input.mousePosition;
            EventSystem.current.RaycastAll(data, m_hitList);
            // 카메라 회전 종료
            m_useDragCameraRot = false;
            m_deltaScroll = Vector3.zero;
            if (m_hitList.Count != 0)
                return;

            if (Physics.Raycast(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera.ScreenPointToRay(Input.mousePosition), out hit, 15f, 1 << LayerMask.NameToLayer("NPC") | 1 << LayerMask.NameToLayer("Ally") | 1 << LayerMask.NameToLayer("Hero") | 1 << LayerMask.NameToLayer("Enermy"), QueryTriggerInteraction.Collide))
            {
                switch(hit.transform.tag)
                {
                    case "NPC":
                        BaseCharacter hero = PlayerMng.Instance.MainPlayer.Character;
                        hero.MoveSystem.SetMoveToTarget(hit.transform, 1, () => {
                            UIMng.Instance.CLOSE = UIMng.UIName.Game;
                            UIMng.Instance.Open<NPCUI>(UIMng.UIName.NPCUI).Enabled(hit.transform.GetComponent<BaseNPC>());
                        });
                        break;
                    case "Enermy":
                        InfoWindow.Enabled(hit.transform.GetComponent<BaseCharacter>(), true);
                        break;
                    default:
                        BaseHero character = hit.transform.GetComponent<BaseHero>();
                        InfoWindow.Enabled(character, character.Name, true);
                        break;
                }
            }
        }
#endif
    }
}

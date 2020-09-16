using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Title_Login : MonoBehaviour
{
    Transform m_camera;
    float m_elapsedTime;
    float m_targetTime;
    Vector3 m_prevPos;
    Vector3 m_targetPos;
    Vector3 m_prevRot;
    Vector3 m_targetRot;

    public InputField ID;
    public InputField PWD;
    public Text ServerState;

    public void Init()
    {
        ID = transform.Find("ID").GetComponent<InputField>();
        PWD = transform.Find("Password").GetComponent<InputField>();

        ServerState = transform.Find("ServerState").GetComponent<Text>();
        transform.Find("Connect_Sunny").GetComponent<Button>().onClick.AddListener(ConnectSunny);
        transform.Find("Create_Sunny").GetComponent<Button>().onClick.AddListener(CreateSunny);
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => Application.Quit());
        m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.World).transform;
        // transform.Find("Connect_Google").GetComponent<Button>().onClick.AddListener(() => UIMng.Instance.GetUI<Loading>(UIMng.UIName.Loading).Open(Loading.LoadingType.GoogleConnect, () => SceneMng.Instance.SetCurrScene(Scene.MainScene)));
    }

    public void Open()
    {
        if(PlayerMng.Instance.MainPlayer.ID != null)
        {
            NetworkMng.Instance.RequestPlayerCharactersInformation();
            Close();
            return;
        }

        m_camera.position = new Vector3(-4.3f, -0.8f, 6.5f);
        m_camera.eulerAngles = new Vector3(-0, -180, 0);
        m_elapsedTime = m_targetTime;
        ID.text = GameSystem.ID;
        PWD.text = GameSystem.PWD;

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ConnectSunny()
    {
        if (NetworkMng.Instance.NetworkState != ENetworkState.ConnectOn)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "서버 혹은 네트워크 상태를 확인해주세요.");
            return;
        }

        // 테스트
        if (ID.text.Length < 3)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "ID는 2자 이상 입력해야합니다.");
            return;
        }

        if (NetworkMng.Instance.NetworkState != ENetworkState.ConnectOn)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "서버에 접속 할 수 없습니다.");
            return;
        }
        NetworkMng.Instance.RequestLoginAccount(ID.text);
        return;

        if (ID.text.Length < 8)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "ID는 8자 이상 입력해야합니다.");
            return;
        }

        string idChecker = Regex.Replace(ID.text, @"[^0-9a-zA-Z가-힣]{1,10}", "", RegexOptions.Singleline);

        if (ID.text.Equals(idChecker) == false)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "아이디는 특수문자가 들어갈 수 없습니다.");
            return;
        }

        if (PWD.text.Length < 8)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "비밀번호는 8자 이상 입력해야합니다.");
            return;
        }

        string pwdChecker = Regex.Replace(PWD.text, @"[^0-9a-zA-Z가-힣]{1,10}!@#$%^&*-_=+`", "", RegexOptions.Singleline);

        if (PWD.text.Equals(pwdChecker) == false)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "비밀번호는 공백, 일부 특수문자를 입력 할 수 없습니다.");
            return;
        }

        // NetworkMng.Instance.LogginAccount_Sunny(ID.text, PWD.text);
    }

    public void CreateSunny()
    {
        if (NetworkMng.Instance.NetworkState != ENetworkState.ConnectOn)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "서버 혹은 네트워크 상태를 확인해주세요.");
            return;
        }

        UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).Login.Close();
        UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).CreateAccount.Open();
    }

    private void Update()
    {
        m_elapsedTime += Time.deltaTime;
        if (m_elapsedTime > m_targetTime)
        {
            m_prevPos = m_camera.position;
            m_prevRot = m_camera.eulerAngles;
            m_targetPos = new Vector3(Random.Range(-5.5f, -3.5f), Random.Range(-1, -3), Random.Range(4.8f, 5.4f));
            m_targetRot = new Vector3(Random.Range(10,20), Random.Range(165, 195), 0);
            m_targetTime = Random.Range(2, 4);
            m_elapsedTime = 0;
        }
        else
        {
            m_camera.position = Vector3.Lerp(m_prevPos, m_targetPos, m_elapsedTime / m_targetTime);
            // m_camera.eulerAngles = Vector3.Lerp(m_prevRot, m_targetRot, m_elapsedTime / m_targetTime);
        }
        ServerState.text = "서버상태 : " + NetworkMng.Instance.NetworkState;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_SelectCharacter : MonoBehaviour
{
    Transform m_camera;
    float m_elapsedTime;
    float m_targetTime;
    Vector3 m_prevPos;
    Vector3 m_targetPos;
    bool m_isFinishLoadAysnc;
    bool m_isFirst = true;

    public Title_SelectCharacterBTN[] Characters = new Title_SelectCharacterBTN[4];
    Title_DeleteCharacter m_deleteCharacter;
    GameObject m_startBTN;
    Text m_id;
    public int CurrSelect = -1;
    public void Init()
    {
        m_deleteCharacter = GetComponentInChildren<Title_DeleteCharacter>();
        m_deleteCharacter.Init();

        Transform BTNGrid = transform.Find("CharacterStatus");
        for(int i =0; i<4; ++i)
            Characters[i] = BTNGrid.GetChild(i).GetComponent<Title_SelectCharacterBTN>().Init(i);
        m_id = transform.Find("AccountStatus").Find("ID").GetComponent<Text>();
        m_startBTN = transform.Find("Start").gameObject;
        transform.Find("Start").GetComponent<Button>().onClick.AddListener(OnClickStart);
        transform.Find("Delete").GetComponent<Button>().onClick.AddListener(OnClickDelete);
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnClickExit);

        m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.World).transform;
    }
    public void SelectHandle(int handle)
    {
        Vector3 CamPos = Characters[handle].Character.position + Characters[handle].Character.forward * 5;
        CamPos.y += 1;
        CurrSelect = handle;
        m_startBTN.SetActive(true);
    }
    public void Open()
    {
        m_isFinishLoadAysnc = false;
        gameObject.SetActive(true);
    }
    public void Open(string data)
    {
        m_deleteCharacter.Close();
        m_isFinishLoadAysnc = m_isFirst;
        // 캐릭터를 불러온 후 플레이어 정보를 각각 저장
        CurrSelect = -1;
        m_startBTN.SetActive(false);
        m_id.text = PlayerMng.Instance.MainPlayer.ID;

        if (data != "")
        {
            string[] datas = data.Split('/');
            for (int i = 0; i < 4; ++i)
            {
                if (datas.Length > i)
                    Characters[i].Enabled(datas[i]);
                else Characters[i].Disabled();
            }
        }
        else
        {
            for (int i = 0; i < 4; ++i)
                Characters[i].Disabled();
        }

        gameObject.SetActive(true);

        if(m_isFirst)
            StartCoroutine(StartEvent());
        else
             m_camera.eulerAngles = new Vector3(38.64f, 70.37f, 0);
    }
    public void Close()
    {
        for (int i = 0; i < Characters.Length; ++i)
            Characters[i].Disabled();

        gameObject.SetActive(false);
    }
    void OnClickStart()
    {
        if (CurrSelect != -1 && PlayerMng.Instance.MainPlayer.Name == null)
        {
            PlayerMng.Instance.MainPlayer.Name = Characters[CurrSelect].Player.Name;
            PlayerMng.Instance.MainPlayer.Handle = Characters[CurrSelect].Player.Handle;
            NetworkMng.Instance.RequestLoginCharacter(Characters[CurrSelect].Player.Name);
        }
    }
    void OnClickDelete()
    {
        if (CurrSelect != -1 && PlayerMng.Instance.MainPlayer.ID != null)
        {
            m_deleteCharacter.Open(Characters[CurrSelect].Player);
        }
    }
    void OnClickExit()
    {

    }
    private void Update()
    {
        if(!m_isFinishLoadAysnc)
        {
            m_elapsedTime += Time.deltaTime;
            if (m_elapsedTime > m_targetTime)
            {
                m_prevPos = m_camera.position;
                m_targetPos = new Vector3(Random.Range(3, 3.2f), 3.53f, Random.Range(-11.1f, -11.3f));
                m_targetTime = Random.Range(0.5f, 2.5f);
                m_elapsedTime = 0;
            }
            else
            {
                m_camera.position = Vector3.Lerp(m_prevPos, m_targetPos, m_elapsedTime / m_targetTime);
            }
        }
    }
    IEnumerator StartEvent()
    {
        m_isFirst = false;
        m_isFinishLoadAysnc = true;
        WaitForSeconds second = new WaitForSeconds(0.02f);

        Transform camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.World).transform;
        Vector3 PrevPos = camera.position;
        Vector3 PrevRot = camera.eulerAngles;
        Vector3 TargetPos = new Vector3(-7.86f, -3.5f, -2.9f);
        Vector3 TargetRot = new Vector3(30.421f, 180, 0);
        float Range = Vector3.Distance(PrevPos, TargetPos);
        float Count = Range * 5; 
        for(int i =1; i< Count + 1; ++i)
        {
            if (!m_isFinishLoadAysnc)
                goto Exit;
            camera.eulerAngles = Vector3.Lerp(PrevRot, TargetRot, i / Count);
            camera.position = Vector3.Lerp(PrevPos, TargetPos, i/ Count);
            yield return second;
        }
        PrevPos = camera.position;
        PrevRot = camera.eulerAngles;
        TargetPos = new Vector3(-7.86f, 0, -12.41f);
        TargetRot = new Vector3(0, 180, 0);
        Count = Vector3.Distance(PrevPos, TargetPos) * Vector3.Distance(PrevPos, TargetPos) / Range * 10;
        for(int i = 1; i<Count +1; ++i)
        {
            if (!m_isFinishLoadAysnc)
                goto Exit;
            camera.eulerAngles = Vector3.Lerp(PrevRot, TargetRot, i*1.5f/Count) ;
            camera.position = Vector3.Lerp(PrevPos, TargetPos, i / Count);
            yield return second;
        }
        PrevPos = camera.position;
        PrevRot = camera.eulerAngles;
        TargetPos = new Vector3(1.86f, 0, -12.41f);
        TargetRot = new Vector3(0, 90, 0);
        Count = Vector3.Distance(PrevPos, TargetPos) * Vector3.Distance(PrevPos, TargetPos) / Range * 10;
        for (int i = 1; i < Count + 1; ++i)
        {
            if (!m_isFinishLoadAysnc)
                goto Exit;
            camera.eulerAngles = Vector3.Lerp(PrevRot, TargetRot, i*2 / Count);
            camera.position = Vector3.Lerp(PrevPos, TargetPos, i / Count);
            yield return second;
        }
        PrevPos = camera.position;
        PrevRot = camera.eulerAngles;
        TargetPos = new Vector3(3, 3.53f, -11.2f);
        TargetRot = new Vector3(38.64f, 70.37f, 0);
        Count = Vector3.Distance(PrevPos, TargetPos) * Vector3.Distance(PrevPos, TargetPos) / Range * 10;
        for (int i = 1; i < Count + 1; ++i)
        {
            if (!m_isFinishLoadAysnc)
                goto Exit;
            camera.eulerAngles = Vector3.Lerp(PrevRot, TargetRot, i  / Count);
            camera.position = Vector3.Lerp(PrevPos, TargetPos, i / Count);
            yield return second;
        }
        m_isFinishLoadAysnc = false;
        Exit:
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : BaseUI
{
    public float m_elapsedTime;
    public Title_Login Login;
    public Title_CreateAccount CreateAccount;
    public Title_SelectCharacter SelectCharacter;

    protected override void InitControl()
    {
        base.InitControl();

        Login = transform.Find("LogIn").GetComponent<Title_Login>();
        Login.Init();
        CreateAccount = transform.Find("CreateAccount").GetComponent<Title_CreateAccount>();
        CreateAccount.Init();
        SelectCharacter = transform.GetComponentInChildren<Title_SelectCharacter>();
        SelectCharacter.Init();
        NetworkMng.Instance.NetworkConnect();
    }

    public override void Open()
    {
        base.Open();

        Login.Open();
        CreateAccount.Close();
        SelectCharacter.Close();

        SoundMng.Instance.PlayMainMusic("ETC_#1_Heroes");
        gameObject.SetActive(true);
        // CameraMng.Instance.SetBloom(0.5f, 0.3f, 0.3f, 4);
        //Camera.main.transform.position = new Vector3(5, 7, 14);
        //Camera.main.transform.eulerAngles = new Vector3(30, 180, 0);
        //StartCoroutine(TitleCameraAction());
    }

    private void Update()
    {
        if (NetworkMng.Instance.NetworkState == ENetworkState.Offline)
        {
            m_elapsedTime += Time.deltaTime;

            if (m_elapsedTime > 5)
            {
                m_elapsedTime = 0;
                NetworkMng.Instance.NetworkConnect();
            }
        }
    }

    public override void Close()
    {
        base.Close();

        SelectCharacter.Close();
        gameObject.SetActive(false);
    }

    //IEnumerator TitleCameraAction()
    //{
    //    WaitForSeconds wait = new WaitForSeconds(0.033f);
    //    Camera cam = Camera.main;

    //    Vector3 m_targetPos;
    //    Vector3 m_prevPos;

    //    Vector3 m_targetAngle;
    //    Vector3 m_prevAngle;

    //    while(cam.isActiveAndEnabled)
    //    {
    //        Transform trs = cam.transform;
    //        m_prevPos = cam.transform.position;
    //        m_targetPos = new Vector3(Random.Range(4, 6), Random.Range(7, 9), Random.Range(5, 14));

    //        m_prevAngle = cam.transform.eulerAngles;
    //        m_targetAngle = new Vector3(Random.Range(10, 20), Random.Range(175, 185));

    //        float Speed = Vector3.Distance(m_prevPos, m_targetPos) * 30;

    //        for (int i =0; i< Speed; ++i)
    //        {
    //            trs.position = Vector3.Lerp(m_prevPos, m_targetPos, i / Speed);
    //            trs.eulerAngles = Vector3.Lerp(m_prevAngle, m_targetAngle, i / Speed);
    //            yield return wait;
    //        }
    //    }

    //    yield return null;
    //}
}

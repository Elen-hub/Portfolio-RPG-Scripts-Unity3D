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
}

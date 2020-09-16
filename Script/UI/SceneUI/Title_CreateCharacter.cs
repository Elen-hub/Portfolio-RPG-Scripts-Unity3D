using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_CreateCharacter : MonoBehaviour
{
    public int SelectHandle;
    Title_CreateCharacterBTN[] m_characterList;

    Transform m_previewCharacter;
    Transform m_previewPos;
    GameObject m_inputField;
    Image m_icon;
    Text m_idText;
    Text m_classText;
    InputField m_nicknameField;

    public void SetHandle(int handle)
    {
        if (m_previewCharacter != null)
            Destroy(m_previewCharacter.gameObject);

        SelectHandle = handle;
        m_previewCharacter = CharacterMng.Instance.InstantiatePreview(handle);
        m_previewCharacter.SetParent(m_previewPos);
        m_previewCharacter.localPosition = Vector3.zero;
    }
    public void Init()
    {
        m_previewPos = GameObject.Find("PreviewPos").transform;
        m_inputField = transform.Find("InputInformation").gameObject;
        m_inputField.transform.Find("Submit").GetComponent<Button>().onClick.AddListener(OnClickSubmit);
        m_inputField.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => m_inputField.gameObject.SetActive(false));
        m_icon = m_inputField.transform.Find("Icon").GetComponent<Image>();
        m_idText = m_inputField.transform.Find("IDText").GetComponent<Text>();
        m_classText = m_inputField.transform.Find("ClassText").GetComponent<Text>();
        m_nicknameField = m_inputField.GetComponentInChildren<InputField>();

        m_characterList = GetComponentsInChildren<Title_CreateCharacterBTN>();
        for(int i =0; i<m_characterList.Length; ++i)
            m_characterList[i].Init(i);
        transform.Find("Select").GetComponent<Button>().onClick.AddListener(OnClickSelect);
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnClickExit);
    }
    public void Open()
    {
        SelectHandle = 0;
        // 구글아이디 m_idText.text = 
        m_nicknameField.text = null;
        m_inputField.SetActive(false);
        gameObject.SetActive(true);
    }
    public void Close()
    {
        if (m_previewCharacter != null)
            Destroy(m_previewCharacter.gameObject);

        CameraMng.Instance.SetCamera(CameraMng.CameraStyle.UI | CameraMng.CameraStyle.World);
        gameObject.SetActive(false);
    }
    void OnClickExit()
    {
        NetworkMng.Instance.RequestPlayerCharactersInformation();
        Close();
    }
    void OnClickSelect()
    {
        if (SelectHandle == 0)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "클래스를 선택해주세요.");
            return;
        }

        m_icon.sprite = Resources.Load<Sprite>(CharacterMng.Instance.GetCharacterStat(SelectHandle).Icon);
        m_nicknameField.text = null;
        m_idText.text = "아이디: " + PlayerMng.Instance.MainPlayer.ID;
        m_classText.text = "클래스: " + ParseLib.GetClassKorConvert((ECharacterClass)SelectHandle);
        m_inputField.SetActive(true);
    }
    void OnClickSubmit()
    {
        if(m_nicknameField.text.Length <3)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "");
            return;
        }
        NetworkMng.Instance.RequestCreateCharacter(PlayerMng.Instance.MainPlayer.ID, m_nicknameField.text, SelectHandle);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_DeleteCharacter : MonoBehaviour
{
    string m_text;
    Image m_icon;
    Text m_classText;
    InputField m_nicknameField;
    Player m_player;

    public void Init()
    {
        m_icon = transform.Find("Icon").GetComponent<Image>();
        m_classText = transform.Find("ClassText").GetComponent<Text>();
        m_nicknameField = GetComponentInChildren<InputField>();

        transform.Find("Submit").GetComponent<Button>().onClick.AddListener(OnClickSubmit);
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnClickExit);
        gameObject.SetActive(false);
    }
    public void Open(Player player)
    {
        m_player = player;
        m_classText.text = "Lv." + player.Level + " " + ParseLib.GetClassKorConvert(CharacterMng.Instance.GetCharacterStat(player.Handle).Class);
        gameObject.SetActive(true);
    }
    public void Close()
    {
        m_player = null;
        m_nicknameField.text = null;
        gameObject.SetActive(false);
    }
    void OnClickExit()
    {
        Close();
    }
    void OnClickSubmit()
    {
        if(m_nicknameField.text != m_player.Name)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "닉네임이 일치하지 않습니다.");
            return;
        }
        NetworkMng.Instance.RequestDeleteCharacter(PlayerMng.Instance.MainPlayer.ID, m_nicknameField.text);
    }
}

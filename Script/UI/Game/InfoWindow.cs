using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    BaseCharacter m_character;
    Text m_nameText;
    EnergyBarBase m_hpBar;
    Text m_hpText;

    TargetUI m_targetUI;

    GameObject m_showStat;
    GameObject m_addParty;
    GameObject m_close;
    bool m_hold;
    public void Init()
    {
        m_hpBar = GetComponentInChildren<EnergyBarBase>();
        m_hpBar.Init();
        m_hpBar.Img.fillAmount = 10;
        m_hpBar.Img.color = new Color(1, 0.25f, 0.25f, 1);
        m_hpText = transform.Find("HPText").GetComponent<Text>();
        m_nameText = transform.Find("NameText").GetComponent<Text>();
        m_showStat = transform.Find("BTNGrid").Find("ShowStatBTN").gameObject;
        m_showStat.GetComponent<Button>().onClick.AddListener(OnClickShowStat);
        m_addParty = transform.Find("BTNGrid").Find("AddPartyBTN").gameObject;
        m_addParty.GetComponent<Button>().onClick.AddListener(OnClickAddParty);
        m_close = transform.Find("BTNGrid").Find("CloseBTN").gameObject;
        m_close.GetComponent<Button>().onClick.AddListener(Disabled);
        m_targetUI = UIMng.Instance.GetUI<FieldUI>(UIMng.UIName.FieldUI).TargetUI;
        Disabled();
    }
    public void Enabled(BaseCharacter character, bool hold = false)
    {
        if (m_hold && !hold)
            return;

        m_targetUI.Enabled(character);
        PlayerMng.Instance.MainPlayer.Character.Target = character;
        m_character = character;
        m_nameText.text = character.StatSystem.BaseStat.Name;

        m_showStat.SetActive(character.tag != "Enermy");
        m_addParty.SetActive(character.tag == "Ally");
        m_hold = hold;
        gameObject.SetActive(true);
    }
    public void Enabled(BaseCharacter character, string name, bool hold = false)
    {
        if (m_hold && !hold)
            return;

        m_targetUI.Enabled(character);
        PlayerMng.Instance.MainPlayer.Character.Target = character;
        m_character = character;
        m_nameText.text = name;

        m_showStat.SetActive(character.tag != "Enermy");
        m_addParty.SetActive(character.tag == "Ally");
        m_hold = hold;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_hold = false;
        m_targetUI.Disabled();
        PlayerMng.Instance.MainPlayer.Character.Target = null;
        m_character = null;
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if(m_character == null || !m_character.gameObject.activeSelf)
        {
            Disabled();
            return;
        }
        float currentHPFill = m_hpBar.Img.fillAmount;
        float targetFill = m_character.StatSystem.CurrHP / m_character.StatSystem.GetHP;
        m_hpBar.Img.fillAmount = currentHPFill + (targetFill - currentHPFill) * Time.deltaTime * 1.5f;
        m_hpText.text = m_character.StatSystem.CurrHP.ToString("F0") + " / " + m_character.StatSystem.GetHP.ToString("F0");
    }
    void OnClickShowStat()
    {
        UIMng.Instance.Open<Status>(UIMng.UIName.Status).Open(m_character);
    }
    void OnClickAddParty()
    {

    }
}

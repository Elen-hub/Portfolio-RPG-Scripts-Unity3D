using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWindow : MonoBehaviour
{
    public List<BuffIcon> MemoryList = new List<BuffIcon>();
    List<BuffIcon> m_iconList = new List<BuffIcon>();
    Transform m_buffGrid;
    BaseCharacter m_character;
    EnergyBarBase m_hpBar;
    EnergyBarBase m_mpBar;
    Text m_hpText;
    Text m_mpText;
    Text m_level;
    Text m_exp;
    public void Init()
    {
        m_hpBar = transform.Find("HP").GetComponent<EnergyBarBase>();
        m_hpBar.Init();
        m_hpText = transform.Find("HPText").GetComponent<Text>();
        m_hpBar.Img.color = new Color(1, 0.15f, 0.15f, 1);
        m_mpBar = transform.Find("MP").GetComponent<EnergyBarBase>();
        m_mpBar.Init();
        m_mpText = transform.Find("MPText").GetComponent<Text>();
        m_mpBar.Img.color = new Color(0.15f, 0.15f, 1, 1);
        m_exp = transform.Find("EXP").GetComponent<Text>();
        m_level = transform.Find("Level").GetComponent<Text>();
        m_buffGrid = transform.Find("BuffGrid");
        m_character = PlayerMng.Instance.MainPlayer.Character;
    }
    // 버프를 등록
    public void EnabledBuff(Buff buff)
    {
        BuffIcon buffIcon;
        if (MemoryList.Count != 0)
        {
            buffIcon = MemoryList[0];
            MemoryList.Remove(buffIcon);
        }
        else
            buffIcon = Instantiate(Resources.Load<BuffIcon>("UI/Instance/BuffIcon"), m_buffGrid).Init();

        buffIcon.transform.SetParent(m_buffGrid);
        buffIcon.Enabled(buff);
    }
    public void Enabled()
    {
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if(!m_character)
        {
            m_character = PlayerMng.Instance.MainPlayer.Character;
            return;
        }

        float currentHPFill = m_hpBar.Img.fillAmount;
        float targetFill = m_character.StatSystem.CurrHP / m_character.StatSystem.GetHP;
        m_hpBar.Img.fillAmount = currentHPFill + (targetFill - currentHPFill) * Time.deltaTime * 1.5f;
        // m_hpText.text = (targetFill * 100).ToString("F0") + "%";
        m_hpText.text = m_character.StatSystem.CurrHP.ToString("F0") + " (" + (targetFill * 100).ToString("F0") + "%)";
        currentHPFill = m_mpBar.Img.fillAmount;
        targetFill = m_character.StatSystem.CurrMP / m_character.StatSystem.GetMP;
        m_mpBar.Img.fillAmount = currentHPFill + (targetFill - currentHPFill) * Time.deltaTime * 1.5f;
        // m_mpText.text = (targetFill * 100).ToString("F0") + "%";
        m_mpText.text = m_character.StatSystem.CurrMP.ToString("F0") + " (" + (targetFill * 100).ToString("F0") + "%)";
        m_level.text = m_character.StatSystem.Level.ToString();
        m_exp.text = PlayerMng.Instance.GetExpPercent().ToString("F2") + "%";
    }
}

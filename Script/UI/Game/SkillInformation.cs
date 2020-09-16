using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInformation : MonoBehaviour
{
    Color m_greyColor = new Color(0.5f, 0.5f, 0.5f);
    Color m_redColor = new Color(1, 0.5f, 0.5f);

    SkillInfo m_skill;
    Image m_icon;
    Text m_name;
    Text m_mp;
    Text m_coolTime;
    Text m_level;
    Text m_point;
    Text m_information;
    Text m_exeption;

    GameObject m_learnBTN;
    GameObject m_quickSlotBTN;
    public SkillInformation Init()
    {
        m_icon = transform.Find("SkillBTN").Find("Icon").GetComponent<Image>();
        m_name = transform.Find("Name").GetComponent<Text>();
        m_level = transform.Find("LevelRect").GetComponentInChildren<Text>();
        m_point = transform.Find("PointRect").GetComponentInChildren<Text>();
        m_coolTime = transform.Find("CoolTimeRect").GetComponentInChildren<Text>();
        m_mp = transform.Find("MPRect").GetComponentInChildren<Text>();
        m_information = transform.Find("Information").GetComponent<Text>();
        m_exeption = transform.Find("Exeption").GetComponent<Text>();
        m_learnBTN = transform.Find("BTNGroup").Find("Learn").gameObject;
        m_learnBTN.GetComponent<Button>().onClick.AddListener(OnClickLearn);
        m_quickSlotBTN = transform.Find("BTNGroup").Find("QuickSlot").gameObject;
        m_quickSlotBTN.GetComponent<Button>().onClick.AddListener(OnClickQuickSlot);
        gameObject.SetActive(false);
        return this;
    }
    public void Open(SkillInfo skill)
    {
        m_skill = skill;
        m_icon.sprite = Resources.Load<Sprite>(skill.Icon);
        m_name.text = skill.Name;
        m_mp.text = skill.MP.ToString();
        m_coolTime.text = skill.CoolTime.ToString();
        m_level.text = "Lv." + skill.Level;
        m_point.text = skill.SkillPoint + "p";
        string[] strs = skill.Information.Split(',');
        m_information.text = string.Join("\n", strs) + "\n\n";
        m_information.text += skill.Explanation;
        if(!PlayerMng.Instance.MainPlayer.Character.AttackSystem.SkillDic.ContainsKey(skill.Handle))
        {
            bool SurcessLearning = true;
            if (PlayerMng.Instance.MainPlayer.SkillPoint < skill.SkillPoint)
            {
                SurcessLearning = false;
                m_exeption.color = Color.red;
                m_exeption.text = "스킬포인트가 부족합니다.";
                m_learnBTN.SetActive(false);
                m_quickSlotBTN.SetActive(false);
            }
            if (PlayerMng.Instance.MainPlayer.Character.StatSystem.Level < skill.Level)
            {
                SurcessLearning = false;
                m_exeption.color = Color.red;
                m_exeption.text = "레벨이 부족합니다.";
                m_learnBTN.SetActive(false);
                m_quickSlotBTN.SetActive(false);
            }
            if (PlayerMng.Instance.MainPlayer.Character.StatSystem.BaseStat.Awakening < skill.CharacterAwakening)
            {
                SurcessLearning = false;
                m_exeption.color = Color.red;
                m_exeption.text = "발현되지 않은 능력입니다.";
                m_learnBTN.SetActive(false);
                m_quickSlotBTN.SetActive(false);
            }

            if (SurcessLearning)
            {
                m_learnBTN.SetActive(true);
                m_quickSlotBTN.SetActive(false);
                m_exeption.text = "습득이 가능합니다.";
                m_exeption.color = m_greyColor;
            }
        }
        else
        {
            m_exeption.text = "습득한 스킬입니다.";
            m_exeption.color = m_greyColor;
            m_learnBTN.SetActive(false);
            m_quickSlotBTN.SetActive(true);
        }
        gameObject.SetActive(true);
    }
    void OnClickLearn()
    {
        NetworkMng.Instance.RequestUseSkillPoint(m_skill);
    }
    void OnClickQuickSlot()
    {
        UIMng.Instance.Open<QuickSlot>(UIMng.UIName.QuickSlot).Enabled(PlayerMng.Instance.MainPlayer.Character.AttackSystem.SkillDic[m_skill.Handle], false, m_skill.Handle);
    }
    public void Open()
    {
        m_icon.sprite = Resources.Load<Sprite>(m_skill.Icon);
        m_name.text = m_skill.Name;
        m_mp.text = m_skill.MP.ToString();
        m_coolTime.text = m_skill.CoolTime.ToString();
        m_level.text = "Lv." + m_skill.Level;
        m_point.text = m_skill.SkillPoint + "p";
        string[] strs = m_skill.Information.Split(',');
        m_information.text = string.Join("\n", strs) + "\n\n";
        m_information.text += m_skill.Explanation;
        if (!PlayerMng.Instance.MainPlayer.Character.AttackSystem.SkillDic.ContainsKey(m_skill.Handle))
        {
            bool SurcessLearning = true;
            if (PlayerMng.Instance.MainPlayer.SkillPoint < m_skill.SkillPoint)
            {
                SurcessLearning = false;
                m_exeption.color = Color.red;
                m_exeption.text = "스킬포인트가 부족합니다.";
                m_learnBTN.SetActive(false);
                m_quickSlotBTN.SetActive(false);
            }
            if (PlayerMng.Instance.MainPlayer.Character.StatSystem.Level < m_skill.Level)
            {
                SurcessLearning = false;
                m_exeption.color = Color.red;
                m_exeption.text = "레벨이 부족합니다.";
                m_learnBTN.SetActive(false);
                m_quickSlotBTN.SetActive(false);
            }
            if (PlayerMng.Instance.MainPlayer.Character.StatSystem.BaseStat.Awakening < m_skill.CharacterAwakening)
            {
                SurcessLearning = false;
                m_exeption.color = Color.red;
                m_exeption.text = "발현되지 않은 능력입니다.";
                m_learnBTN.SetActive(false);
                m_quickSlotBTN.SetActive(false);
            }

            if (SurcessLearning)
            {
                m_learnBTN.SetActive(true);
                m_quickSlotBTN.SetActive(false);
                m_exeption.text = "습득이 가능합니다.";
                m_exeption.color = m_greyColor;
            }
        }
        else
        {
            m_exeption.text = "습득한 스킬입니다.";
            m_exeption.color = m_greyColor;
            m_learnBTN.SetActive(false);
            m_quickSlotBTN.SetActive(true);
        }
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}

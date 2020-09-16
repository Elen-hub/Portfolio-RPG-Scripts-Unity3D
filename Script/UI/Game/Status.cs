using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : BaseUI
{
    Dictionary<EItemType, EquipmentBTN> m_equipDic = new Dictionary<EItemType, EquipmentBTN>();
    BaseCharacter m_character;
    Animator m_animator;

    GameObject m_strBTN;
    GameObject m_dexBTN;
    GameObject m_intBTN;
    GameObject m_wisBTN;
    GameObject m_conBTN;

    Text m_guildText;
    Text m_nameText;
    Text m_jobText;
    Text m_expText;
    Text m_strText;
    Text m_dexText;
    Text m_intText;
    Text m_wisText;
    Text m_conText;
    Text m_hpText;
    Text m_hprText;
    Text m_mpText;
    Text m_mprText;
    Text m_attackDamageText;
    Text m_criticalDamageText;
    Text m_criticalDamageProText;
    Text m_attackSpeedText;
    Text m_defenceText;
    Text m_resistanceText;
    Text m_moveSpeedText;
    Text m_coolTimeText;
    Text m_skillDamageText;
    Text m_pointText;

    protected override void InitUI()
    {
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Close);
        m_animator = GetComponent<Animator>();
        Transform EquipGrid = transform.Find("Equipment");
        m_equipDic.Add(EItemType.Weapon, EquipGrid.Find("Weapon").GetComponent<EquipmentBTN>().Init(EItemType.Weapon));
        m_equipDic.Add(EItemType.Armor, EquipGrid.Find("Armor").GetComponent<EquipmentBTN>().Init(EItemType.Armor));
        m_equipDic.Add(EItemType.Gloves, EquipGrid.Find("Gloves").GetComponent<EquipmentBTN>().Init(EItemType.Gloves));
        m_equipDic.Add(EItemType.Shoes, EquipGrid.Find("Shoes").GetComponent<EquipmentBTN>().Init(EItemType.Shoes));
        m_equipDic.Add(EItemType.Ring, EquipGrid.Find("Ring").GetComponent<EquipmentBTN>().Init(EItemType.Ring));
        m_equipDic.Add(EItemType.Necklace, EquipGrid.Find("Necklace").GetComponent<EquipmentBTN>().Init(EItemType.Necklace));
        Transform NormalStatGrid = transform.Find("NormalStat");
        m_guildText = NormalStatGrid.Find("Guild").GetComponent<Text>();
        m_nameText = NormalStatGrid.Find("Name").GetComponent<Text>();
        m_jobText = NormalStatGrid.Find("Job").GetComponent<Text>();
        m_expText = NormalStatGrid.Find("EXP").GetComponent<Text>();
        m_strText = NormalStatGrid.Find("STR").GetComponent<Text>();
        m_dexText = NormalStatGrid.Find("DEX").GetComponent<Text>();
        m_intText = NormalStatGrid.Find("INT").GetComponent<Text>();
        m_wisText = NormalStatGrid.Find("WIS").GetComponent<Text>();
        m_conText = NormalStatGrid.Find("CON").GetComponent<Text>();
        m_pointText = NormalStatGrid.Find("Point").GetComponent<Text>();
        m_strBTN = NormalStatGrid.Find("STRBTN").gameObject;
        m_strBTN.GetComponent<Button>().onClick.AddListener(() => NetworkMng.Instance.RequestUseStatPoint(EStatType.STR));
        m_dexBTN = NormalStatGrid.Find("DEXBTN").gameObject;
        m_dexBTN.GetComponent<Button>().onClick.AddListener(() => NetworkMng.Instance.RequestUseStatPoint(EStatType.DEX));
        m_intBTN = NormalStatGrid.Find("INTBTN").gameObject;
        m_intBTN.GetComponent<Button>().onClick.AddListener(() => NetworkMng.Instance.RequestUseStatPoint(EStatType.INT));
        m_wisBTN = NormalStatGrid.Find("WISBTN").gameObject;
        m_wisBTN.GetComponent<Button>().onClick.AddListener(() => NetworkMng.Instance.RequestUseStatPoint(EStatType.WIS));
        m_conBTN = NormalStatGrid.Find("CONBTN").gameObject;
        m_conBTN.GetComponent<Button>().onClick.AddListener(() => NetworkMng.Instance.RequestUseStatPoint(EStatType.CON));
        Transform Status = transform.Find("Status");
        m_hpText = Status.Find("HP").GetComponent<Text>();
        m_hprText = Status.Find("RecoveryHP").GetComponent<Text>();
        m_mpText = Status.Find("MP").GetComponent<Text>();
        m_mprText = Status.Find("RecoveryMP").GetComponent<Text>();
        m_attackDamageText = Status.Find("AttackDamage").GetComponent<Text>();
        m_criticalDamageProText = Status.Find("CriticalPro").GetComponent<Text>();
        m_criticalDamageText = Status.Find("CriticalDamage").GetComponent<Text>();
        m_attackSpeedText = Status.Find("AttackSpeed").GetComponent<Text>();
        m_defenceText = Status.Find("Defence").GetComponent<Text>();
        m_resistanceText = Status.Find("Resistance").GetComponent<Text>();
        m_moveSpeedText = Status.Find("MoveSpeed").GetComponent<Text>();
        m_coolTimeText = Status.Find("CoolTime").GetComponent<Text>();
        m_skillDamageText = Status.Find("SkillDamagePro").GetComponent<Text>();
    }
    public void Open(BaseCharacter character)
    {
        if(character != PlayerMng.Instance.MainPlayer.Character)
        {
            m_strBTN.SetActive(false);
            m_dexBTN.SetActive(false);
            m_intBTN.SetActive(false);
            m_wisBTN.SetActive(false);
            m_conBTN.SetActive(false);
            m_pointText.enabled = false;
        }
        foreach(Player player in PlayerMng.Instance.PlayerList.Values)
        {
            if(player.Character == character)
            {
                m_nameText.text = player.Name;
                m_guildText.text = "길드 없음";
                break;
            }
        }
        m_character = character;
        m_jobText.text = ParseLib.GetClassKorConvert(character.StatSystem.BaseStat.Class);
        CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Character).camera.rect = new Rect(0.5f, 0, 1, 1);
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        m_character = null;
        // UIMng.Instance.OPEN = UIMng.UIName.Game;
        gameObject.SetActive(false);
    }
    public void ShowEquipment()
    {
        m_equipDic[EItemType.Weapon].Enabled(m_character);
        m_equipDic[EItemType.Armor].Enabled(m_character);
        m_equipDic[EItemType.Gloves].Enabled(m_character);
        m_equipDic[EItemType.Shoes].Enabled(m_character);
        m_equipDic[EItemType.Ring].Enabled(m_character);
        m_equipDic[EItemType.Necklace].Enabled(m_character);
    }
    public void ShowMainStat()
    {
        bool isPoint = PlayerMng.Instance.MainPlayer.StatPoint > 0;
        if (m_character == PlayerMng.Instance.MainPlayer.Character)
        {
            m_strBTN.SetActive(isPoint);
            m_dexBTN.SetActive(isPoint);
            m_intBTN.SetActive(isPoint);
            m_wisBTN.SetActive(isPoint);
            m_conBTN.SetActive(isPoint);
        }
        m_strText.text = m_character.StatSystem.GetSTR.ToString();
        m_dexText.text = m_character.StatSystem.GetDEX.ToString();
        m_intText.text = m_character.StatSystem.GetINT.ToString();
        m_wisText.text = m_character.StatSystem.GetWIS.ToString();
        m_conText.text = m_character.StatSystem.GetCON.ToString();
    }
    public void ShowOtherStat()
    {
        m_strText.text = m_character.StatSystem.GetSTR.ToString("F0");
        m_dexText.text = m_character.StatSystem.GetDEX.ToString("F0");
        m_intText.text = m_character.StatSystem.GetINT.ToString("F0");
        m_wisText.text = m_character.StatSystem.GetWIS.ToString("F0");
        m_conText.text = m_character.StatSystem.GetCON.ToString("F0");
        m_pointText.text = "포인트: " + PlayerMng.Instance.MainPlayer.StatPoint.ToString();
        m_hpText.text = m_character.StatSystem.GetHP.ToString();
        m_hprText.text = m_character.StatSystem.GetHPRecovery+" / 5초당";
        m_mpText.text = m_character.StatSystem.GetMP.ToString();
        m_mprText.text = m_character.StatSystem.GetMPRecovery + " / 5초당";
        m_expText.text = "경험치: " + m_character.StatSystem.Exp + " (" + PlayerMng.Instance.GetExpPercent().ToString("F2") + "%)";
        m_attackDamageText.text = (m_character.StatSystem.GetAttackDamage * (m_character.StatSystem.GetAttackDamagePro+1)).ToString("F1") + " (" + m_character.StatSystem.GetAttackDamagePro * 100 + "%)";
        m_criticalDamageProText.text = m_character.StatSystem.GetCriticalPro * 100 + "%";
        m_criticalDamageText.text = m_character.StatSystem.GetCriticalDamage * 100 + "%";
        m_defenceText.text = (m_character.StatSystem.GetDefence * (1 + m_character.StatSystem.GetDefencePro)).ToString("F1") + " (" + m_character.StatSystem.GetDefencePro * 100 + "%)";
        m_resistanceText.text = m_character.StatSystem.GetResistance * 100 + "%";
        m_attackSpeedText.text = m_character.StatSystem.GetAttackSpeed.ToString();
        m_moveSpeedText.text = (m_character.StatSystem.GetMoveSpeed * (1 + m_character.StatSystem.GetMoveSpeedPro)).ToString("0") + " (" + m_character.StatSystem.GetMoveSpeedPro * 100 + "%)";
        m_coolTimeText.text = m_character.StatSystem.GetCoolTime * 100 + "%";
        m_skillDamageText.text = m_character.StatSystem.GetSkillDamage * 100 + "%";
    }

    private void LateUpdate()
    {
        if (m_character == null)
            return;

        ShowEquipment();
        ShowMainStat();
        ShowOtherStat();
    }
}

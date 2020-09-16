using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum ESkillPublicOption
{
    Public,
    Private,
}
public class Skill : BaseUI
{
    Dictionary<int, List<SkillBTN>> m_btnDic = new Dictionary<int, List<SkillBTN>>();
    ESkillPublicOption m_currOption;
    Color m_blackColor = new Color(0, 0, 0, 0.5f);
    Color m_greyColor = new Color(0.5882f, 0.5882f, 0.5882f);
    Transform[] m_contentGrid = new Transform[2];
    Text[] m_contentText = new Text[2];
    Image m_publicImg;
    Image m_privateImg;
    Text m_publicText;
    Text m_privateText;
    Text m_pointText;
    public SkillInformation Information;
    public int IComparerItem(SkillInfo start, SkillInfo to)
    {
            if (start.Level > to.Level)
                return 1;
            else
                return -1;
    }
    protected override void InitUI()
    {
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Close);

        Transform Grid = transform.Find("SkillTreeBackGround").Find("SelectTypeButton");
        m_publicImg = Grid.Find("Default").GetComponent<Image>();
        m_privateImg = Grid.Find("Job").GetComponent<Image>();
        m_publicText = m_publicImg.GetComponentInChildren<Text>();
        m_privateText = m_privateImg.GetComponentInChildren<Text>();
        m_publicImg.GetComponent<Button>().onClick.AddListener(OnClickPublic);
        m_privateImg.GetComponent<Button>().onClick.AddListener(OnClickPrivate);
        m_pointText = transform.Find("PointText").GetComponent<Text>();

        for (int i =0; i<2; ++i)
        {
            m_btnDic.Add(i, new List<SkillBTN>());
            m_contentGrid[i] = transform.Find("Content"+i);
            m_contentText[i] = transform.Find("ContentText"+i).GetComponent<Text>();
        }

        Information = GetComponentInChildren<SkillInformation>().Init();
    }

    public override void Open()
    {
        m_pointText.text = PlayerMng.Instance.MainPlayer.SkillPoint.ToString();
        ECharacterClass Class = ECharacterClass.Default;
        int Awakening = PlayerMng.Instance.MainPlayer.Character.StatSystem.BaseStat.Awakening;

        switch (m_currOption)
        {
            case ESkillPublicOption.Public:
                m_contentText[0].text = "일반";
                m_contentText[1].text = "채집";
                break;
            case ESkillPublicOption.Private:
                Class = PlayerMng.Instance.MainPlayer.Character.StatSystem.BaseStat.Class;
                m_contentText[0].text = ParseLib.GetClassKorConvert(Class);
                break;
        }

        for (int i =0; i<2; ++i)
        {
            var Value = from skill in CharacterMng.Instance.SkillInfo.Values
                              where skill.CharacterClass == Class && skill.CharacterAwakening == i && ((skill.Type & ESkillType.NotLearn) == 0)
                        select skill;

            List<SkillInfo> SkillHandle = new List<SkillInfo>();
            SkillHandle.AddRange(Value);
            SkillHandle.Sort((x, y) => IComparerItem(x, y));
            m_contentGrid[i].gameObject.SetActive(SkillHandle.Count != 0);

            int j = 0;
            foreach (SkillInfo info in SkillHandle)
            {
                if(m_btnDic[i].Count<=j)
                    m_btnDic[i].Add(Instantiate(Resources.Load<SkillBTN>("UI/Instance/SkillBTN"), m_contentGrid[i]).Init());

                m_btnDic[i][j].Open(info);
                ++j;
            }
        }

        gameObject.SetActive(true);
    }

    public override void Close()
    {
        foreach(List<SkillBTN> BtnList in m_btnDic.Values)
            for (int i = 0; i < BtnList.Count; ++i)
                BtnList[i].Close();

        Information.Close();
        gameObject.SetActive(false);
    }

    void OnClickPublic()
    {
        Close();
        m_currOption = ESkillPublicOption.Public;
        m_privateImg.color = Color.white; ;
        m_privateText.color = m_greyColor;
        m_publicImg.color = m_blackColor;
        m_publicText.color = Color.white;
        Open();
    }
    void OnClickPrivate()
    {
        Close();
        m_currOption = ESkillPublicOption.Private;
        m_publicImg.color = Color.white;
        m_publicText.color = m_greyColor;
        m_privateImg.color = m_blackColor;
        m_privateText.color = Color.white;
        Open();
    }
}

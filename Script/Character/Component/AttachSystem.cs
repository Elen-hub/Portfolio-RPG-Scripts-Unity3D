using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public enum EAttachPoint
{
    Chest,
    UnderHead,
    Name,
    HP,
    ChatBox,
    Weapon,
    SubWeapon,
    Foot,
}
public class AttachSystem : MonoBehaviour
{
    Dictionary<EAttachPoint, Transform> m_attachDic = new Dictionary<EAttachPoint, Transform>();
    public Transform GetAttachPoint(EAttachPoint point)
    {
        return m_attachDic[point];
    }
    public void Init()
    {
        m_attachDic.Add(EAttachPoint.UnderHead, transform.Find("Attach_UnderHead"));
        Transform HP = transform.Find("Attach_HP");
        if(HP != null)
        {
            m_attachDic.Add(EAttachPoint.HP, HP);
        }
        Transform Name = transform.Find("Attach_Name");
        if (Name != null)
        {
            m_attachDic.Add(EAttachPoint.Name, Name);
        }
        Transform ChatBox = transform.Find("Attach_ChatBox");
        if (ChatBox != null)
        {
            m_attachDic.Add(EAttachPoint.ChatBox, ChatBox);
        }
        Transform Foot = transform.Find("Attach_Foot");
        if (Foot != null)
        {
            m_attachDic.Add(EAttachPoint.Foot, Foot);
        }
        Weapon Weapon = transform.GetComponentInChildren<Weapon>();
        if(Weapon != null)
            m_attachDic.Add(EAttachPoint.Weapon, Weapon.transform);
        SubWeapon SubWeapon = transform.GetComponentInChildren<SubWeapon>();
        if (SubWeapon != null)
            m_attachDic.Add(EAttachPoint.SubWeapon, SubWeapon.transform);
        Transform chest = transform.Find("Attach_Chest");
        if (Foot != null)
        {
            m_attachDic.Add(EAttachPoint.Chest, chest);
        }
    }
}

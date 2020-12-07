using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldUI : BaseUI
{
    public delegate void Register(EUIFieldType type, BaseFieldUI ui);
    public enum EUIFieldType
    {
        DamageText,
        NameText,
        ChatBox,
        TargetUI,
    }
    Dictionary<EUIFieldType, Queue<BaseFieldUI>> m_fieldUIDic = new Dictionary<EUIFieldType, Queue<BaseFieldUI>>();
    public TargetUI TargetUI;
    public void AddRegister(EUIFieldType type, BaseFieldUI ui)
    {
        m_fieldUIDic[type].Enqueue(ui);
    }
    public override void Init()
    {
        base.Init();

        TargetUI = Instantiate(Resources.Load<TargetUI>("UI/FieldUI/TargetUI"), transform);
        TargetUI.Init(null);
        m_canvas.planeDistance = 100;
        m_canvas.worldCamera = CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).UICamera;
        m_fieldUIDic.Add(EUIFieldType.DamageText, new Queue<BaseFieldUI>());
        m_fieldUIDic.Add(EUIFieldType.NameText, new Queue<BaseFieldUI>());
        m_fieldUIDic.Add(EUIFieldType.ChatBox, new Queue<BaseFieldUI>());
    }

    public T FindDisabledUIField<T>(EUIFieldType type) where T : BaseFieldUI
    {
        if (m_fieldUIDic[type].Count > 0)
            return m_fieldUIDic[type].Dequeue() as T;

        T t = Instantiate(Resources.Load<T>("UI/FieldUI/" + type), transform);
        t.Init(AddRegister);
        return t;
    }
    public DamageText SetDamageText(BaseCharacter target, string damage, Color color, bool isCritical = false)
    {
        DamageText text = FindDisabledUIField<DamageText>(EUIFieldType.DamageText);
        text.Enabled(target, damage, color, isCritical);
        return text;
    }
    public NameText SetNameText(BaseCharacter target, string name)
    {
        Color color = Color.white;
        switch(LayerMask.LayerToName(target.gameObject.layer))
        {
            case "Player":
                color = Color.white;
                break;
            case "NPC":
                color = Color.white;
                break;
            case "Ally":
                color = new Color(0.4f, 0.6f, 1);
                break;
            case "Enermy":
                BaseEnermy enermy = target as BaseEnermy;
                switch(enermy.StatSystem.BaseStat.MonsterGrade)
                {
                    case EMonsterGrade.Normal:
                        color = Color.grey;
                        break;
                    case EMonsterGrade.Elite:
                        color = Color.green;
                        break;
                    case EMonsterGrade.MediumBoss:
                    case EMonsterGrade.Boss:
                        color = Color.red;
                        break;
                }
                break;
            case "Hero":
                color = Color.grey;
                break;
        }
        NameText text = FindDisabledUIField<NameText>(EUIFieldType.NameText);
        text.Enabled(target, name, color);
        return text;
    }
    public ChatBox SetChatBox(BaseCharacter target, string text)
    {
        ChatBox ChatBox = FindDisabledUIField<ChatBox>(EUIFieldType.ChatBox);
        ChatBox.Enabled(target, text);
        return ChatBox;
    }
}

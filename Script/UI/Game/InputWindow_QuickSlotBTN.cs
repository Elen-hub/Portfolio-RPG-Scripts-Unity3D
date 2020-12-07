using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputWindow_QuickSlotBTN : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool m_isClick;
    int m_number;
    IQuickSlotable m_contents;
    Item_Base m_item;
    IItemNumber m_itemNumber;
    BaseSkill m_linkSkill;
    BaseSkill m_skill;
    Image m_icon;
    Image m_backGround;
    Text m_coolTimeText;
    bool m_isAttackBTN;
    float m_currValue;
    public void Init(int number)
    {
        if (number == 7)
            m_isAttackBTN = true;

        m_number = number;
        m_icon = transform.Find("Icon").GetComponent<Image>();
        m_backGround = transform.Find("BackGround").GetComponent<Image>();
        m_coolTimeText = transform.Find("CoolTime").GetComponent<Text>();
    }
    public void Enabled()
    {
        m_linkSkill = null;
        m_contents = PlayerMng.Instance.MainPlayer.Quickslot[m_number];
        m_backGround.fillAmount = 0;

        if (m_contents != null)
        {
            m_item = m_contents as Item_Base;
            m_itemNumber = m_contents as IItemNumber;
            m_skill = m_contents as BaseSkill;
            m_icon.sprite = Resources.Load<Sprite>(m_contents.Icon);
        }
        else
        {
            if(m_isAttackBTN) // AttackSprite로 지정
                m_icon.sprite = Resources.Load<Sprite>("Sprite/EquipBTN");
            else
                m_icon.sprite = Resources.Load<Sprite>("Sprite/EquipBTN");

            m_item = null;
            m_skill = null;
            m_coolTimeText.text = null;
        }
    }
    public void Disabled()
    {
        PlayerMng.Instance.MainPlayer.Quickslot[m_number] = null;
        if(m_isAttackBTN) // AttackSprite로 지정
            m_icon.sprite = Resources.Load<Sprite>("Sprite/EquipBTN");
        else
            m_icon.sprite = Resources.Load<Sprite>("Sprite/EquipBTN");

        m_contents = null;
        m_item = null;
        m_skill = null;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Use();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!m_isClick)
            return;

        m_isClick = false;

        if ((m_skill.SkillInfo.Type & ESkillType.Keydown) != 0 || (m_skill.SkillInfo.Type & ESkillType.Channeling) != 0)
            NetworkMng.Instance.NotifyCharacterState_SkillEnd(PlayerMng.Instance.MainPlayer.Character.MoveSystem.GetCurrAxis, m_skill.SkillInfo.Handle);

        if ((m_skill.SkillInfo.Type & ESkillType.Link) != 0)
        {
            if (m_linkSkill != null)
            {
                if (m_skill.LinkSkill == null)
                {
                    Enabled();
                    return;
                }
            }

            BaseSkill skill = m_skill.LinkSkill;
            m_linkSkill = m_skill;
            m_skill = skill;
            m_icon.sprite = Resources.Load<Sprite>(m_skill.Icon);
            m_coolTimeText.text = "LINK";
            m_backGround.fillAmount = 0;
        }
    }
    public void Use()
    {
        BaseCharacter character = PlayerMng.Instance.MainPlayer.Character;

        if (character.State == BaseCharacter.CharacterState.Death)
            return;

        if (m_item != null)
        {
            NetworkMng.Instance.RequestItemUse(m_item);
            if (m_itemNumber != null)
                if (m_itemNumber.Number <= 0)
                    Disabled();
        }

        if (m_skill != null)
        {
            if ((m_skill.SkillInfo.Type & ESkillType.Passive) != 0)
                return;

            if (!m_skill.Using())
                return;

            m_isClick = true;
            NetworkMng.Instance.NotifyCharacterState_Skill(character.MoveSystem.GetCurrAxis, m_skill.SkillInfo.Handle);
        }
        if (m_isAttackBTN && m_contents == null)
        {
            if (character.IsStun || character.IsHit || character.IsNuckback)
                return;

            if ((character.State == BaseCharacter.CharacterState.Battle && !character.AttackSystem.HoldAttack) || character.AttackSystem.CompleteAttack)
            {
                int currAttackCount = character.AttackSystem.AttackCount;
                int maxAttackCount = character.AttackSystem.NormalAttack.Count;

                if (maxAttackCount <= currAttackCount)
                    return;

                switch (GameSystem.AutoAim)
                {
                    case 1:
                        if (character.Target)
                        {
                            if (character.Target.State == BaseCharacter.CharacterState.Death)
                                return;

                            AutoAIM(character, character.AttackSystem.NormalAttack.Range[currAttackCount]);
                            return;
                        }
                        break;
                    case 2:
                        if (character.Target)
                        {
                            if (character.Target.State != BaseCharacter.CharacterState.Death)
                            {
                                AutoAIM(character, character.AttackSystem.NormalAttack.Range[currAttackCount]);
                                return;
                            }
                        }
                        BaseCharacter target = CharacterMng.Instance.GetFilterClosestCharacter(character.transform.position, EAllyType.Hostile);
                        if (target != null)
                        {
                            UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).InfoWindow.Enabled(target);
                            AutoAIM(character, character.AttackSystem.NormalAttack.Range[currAttackCount]);
                            return;
                        }
                        break;
                }
                NetworkMng.Instance.NotifyCharacterState_Attack(character.MoveSystem.GetCurrAxis);
            }
        }
    }
    bool AutoAIM(BaseCharacter caster, float range)
    {
        if (Vector3.Distance(caster.transform.position, caster.Target.transform.position) < range)
        {
            float angle = Vector3.Angle(Vector3.forward, caster.Target.transform.position - caster.transform.position);
            if (caster.Target.transform.position.x - caster.transform.position.x < 0)
                angle *= -1;
            caster.MoveSystem.SetAxis = angle;
            NetworkMng.Instance.NotifyCharacterState_Attack(caster.MoveSystem.GetCurrAxis);
            return true;
        }
        else
        {
            caster.MoveSystem.SetMoveToTarget(caster.Target.transform, range, () =>
            {
                caster.State = BaseCharacter.CharacterState.Idle;
                float angle = Vector3.Angle(Vector3.forward, caster.Target.transform.position - caster.transform.position);
                if (caster.Target.transform.position.x - caster.transform.position.x < 0)
                    angle *= -1;
                caster.MoveSystem.SetAxis = angle;
                NetworkMng.Instance.NotifyCharacterState_Attack(caster.MoveSystem.GetCurrAxis);
            });
            return false;
        }
    }
    public void LateUpdate()
    {
        if (m_skill != null)
        {
            if (m_linkSkill != null)
            {
                if (!m_linkSkill.IsLink)
                    Enabled();
            }
            else
            {
                float coolTime = (m_skill.CoolTime - m_skill.ElapsedTime);
                if (coolTime > 0)
                {
                    if (m_currValue != coolTime)
                    {
                        m_currValue = coolTime;
                        m_coolTimeText.text = m_currValue.ToString("F0");
                    }
                }
                else m_coolTimeText.text = null;
                m_backGround.fillAmount = 1 - (m_skill.ElapsedTime / m_skill.CoolTime);
            }
            return;
        }
        else if (PlayerMng.Instance.MainPlayer.Quickslot[m_number] != m_contents)
        {
            Enabled();
            return;
        }
        if (m_itemNumber != null)
        {
            if (m_currValue != m_itemNumber.Number)
            {
                m_currValue = m_itemNumber.Number;
                m_coolTimeText.text = "x" + m_currValue;
            }
        }
    }
}

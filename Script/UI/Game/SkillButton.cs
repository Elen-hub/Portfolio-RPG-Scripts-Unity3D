using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public int handle;
    BaseCharacter m_character;
    Text m_text;

    private void Awake()
    {
        m_text = GetComponentInChildren<Text>();
        GetComponentInChildren<Button>().onClick.AddListener(Dash);
        m_character = PlayerMng.Instance.MainPlayer.Character;
    }
    void Dash()
    {
        if (!m_character.AttackSystem.HoldAttack && m_character.State != BaseCharacter.CharacterState.Death)
        {
            m_character.AttackSystem.UseSkill(handle);
            m_character.State = BaseCharacter.CharacterState.Battle;
        }
    }
    void Telleport()
    {
        if (!m_character.AttackSystem.HoldAttack && m_character.State != BaseCharacter.CharacterState.Death)
        {
            m_character.AttackSystem.UseSkill(handle);
            m_character.State = BaseCharacter.CharacterState.Battle;
        }
    }
    private void Update()
    {
        if (!m_character)
        {
            m_character = PlayerMng.Instance.MainPlayer.Character;
            return;
        }

        if (!m_character.AttackSystem.SkillDic[handle].PossibleSkill)
            m_text.text = m_character.AttackSystem.SkillDic[handle].CoolTime.ToString("F0");
        else
        {
            if (handle == 0)
                m_text.text = "Dash";
            else
                m_text.text = "Tellerport";
        }

        if (Input.GetKeyDown(KeyCode.S) && handle ==0 && !m_character.AttackSystem.HoldAttack && m_character.State != BaseCharacter.CharacterState.Death)
            Dash();

        if (Input.GetKeyDown(KeyCode.D) && handle == 1 && !m_character.AttackSystem.HoldAttack && m_character.State != BaseCharacter.CharacterState.Death)
            Telleport();
    }
}

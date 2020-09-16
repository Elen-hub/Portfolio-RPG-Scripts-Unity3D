using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMissile_KnifeWind : DefaultMissile
{
    new Hero_Archer m_caster;
    public override void Enabled(BaseCharacter caster, EAttackType type, EAllyType allyType, float damage, float hitTime, Vector3 launcherAxis, Vector3 targetAxis, System.Action<BaseCharacter> action = null)
    {
        base.Enabled(caster, type, allyType, damage, hitTime, launcherAxis, targetAxis, action);

        m_caster = caster.GetComponent<Hero_Archer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        BaseCharacter character = other.transform.GetComponent<BaseCharacter>();

        if (character == null)
            return;
        if (m_caster.KnifeWindTargetList.Contains(character))
            return;
        if (character.State == BaseCharacter.CharacterState.Death)
            return;
        if ((character.AllyType & m_allyType) == 0)
            return;
        if (Vector3.Distance(other.transform.position, transform.position) > 2)
            return;
        if (character.AttackSystem.Invincibility)
            return;

        m_hit = true;
        m_collider.enabled = false;
        m_missile.SetActive(false);
        m_effect.SetActive(true);
        m_hitAction?.Invoke(character);

        if (m_caster.tag == "Player" || other.tag == "Player")
        {
            m_caster.KnifeWindTargetList.Add(character);
            NetworkMng.Instance.NotifyReceiveDamage(m_attackType, m_caster.UniqueID, character.UniqueID, m_damage, m_hitTime);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMissile : BaseMissile
{
    protected System.Action<BaseCharacter> m_hitAction;
    public virtual void Enabled(BaseCharacter caster, EAttackType type, EAllyType allyType, float damage, float hitTime, Vector3 launcherAxis, Vector3 targetAxis, System.Action<BaseCharacter> action = null)
    {
        m_caster = caster;
        m_attackType = type;
        m_allyType = allyType;
        m_damage = damage;
        m_hitTime = hitTime;

        m_casterPos = launcherAxis;
        m_targetPos = targetAxis;
        m_elapsedTime = 0;
        m_effectTime = 0;
        m_hit = false;

        transform.position = m_casterPos;
        Vector3 nextPos = Vector3.Lerp(m_casterPos, m_targetPos, m_elapsedTime);
        nextPos.y *= m_curve.Evaluate(m_elapsedTime) * m_maxHeight;
        transform.LookAt(nextPos);

        m_collider.enabled = true;

        m_hitAction = action;

        m_missile.SetActive(true);
        m_effect.SetActive(false);
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        BaseCharacter character = other.transform.GetComponent<BaseCharacter>();

        if (character == null)
        {
            m_hit = true;
            m_collider.enabled = false;
            m_missile.SetActive(false);
            m_effect.SetActive(true);
            return;
        }

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
            NetworkMng.Instance.NotifyReceiveDamage(m_attackType, m_caster.UniqueID, character.UniqueID, m_damage, m_hitTime);
    }
    public override void FixedUpdate  ()
    {
        if (m_hit)
        {
            m_effectTime += Time.fixedDeltaTime;

            if (m_effectTime > 2)
                Disabled();

            return;
        }

        m_elapsedTime += Time.fixedDeltaTime * m_speed;
        Vector3 nextPos = Vector3.Lerp(m_casterPos, m_targetPos, m_elapsedTime);
        nextPos.y *= m_curve.Evaluate(m_elapsedTime) * m_maxHeight;
        transform.LookAt(nextPos);
        transform.position = nextPos;
        if (m_elapsedTime > 1)
        {
            m_collider.enabled = false;
            m_hit = true;
        }
    }
}

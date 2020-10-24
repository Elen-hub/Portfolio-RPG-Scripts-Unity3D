using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMissileType
{
    Default,
    Penetration,
    Splash,
}
public class BaseMissile : MonoBehaviour
{
    Stack<BaseMissile> m_missileStack;
    protected BaseCharacter m_caster;
    protected EAttackType m_attackType;
    protected float m_damage;
    protected float m_hitTime;
    protected Collider m_collider;

    protected float m_range;
    protected float m_speed;

    protected Vector3 m_casterPos;
    protected Vector3 m_targetPos;
    protected EAllyType m_allyType;

    protected GameObject m_missile;
    protected GameObject m_effect;

    protected float m_elapsedTime;
    protected float m_targetTime;
    protected float m_effectTime = 2;
    protected bool m_hit;

    [SerializeField] protected AnimationCurve m_curve = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] protected float m_maxHeight = 1;
    public virtual BaseMissile Init(ref Stack<BaseMissile> missileStack, float speed)
    {
        m_missileStack = missileStack;
        m_collider = GetComponent<Collider>();
        m_speed = speed;
        m_missile = transform.Find("Missile").gameObject;
        m_effect = transform.Find("Effect").gameObject;
        return this;
    }
    public void Enabled(BaseCharacter caster, Vector3 launcherAxis, Vector3 targetAxis)
    {
        m_caster = caster;
        m_casterPos = launcherAxis;
        m_targetPos = targetAxis;

        m_elapsedTime = 0;
        m_effectTime = 0;
        m_hit = false;

        transform.position = m_casterPos;
        Vector3 nextPos = Vector3.Lerp(m_casterPos, m_targetPos, m_elapsedTime);
        nextPos.y *= m_curve.Evaluate(m_elapsedTime) * m_maxHeight;
        transform.LookAt(nextPos);

        m_missile.SetActive(true);
        m_effect.SetActive(false);
        gameObject.SetActive(true);
    }
    public virtual void Disabled()
    {
        m_missile.SetActive(false);
        m_effect.SetActive(false);
        m_missileStack.Push(this);
        gameObject.SetActive(false);
    }
    public virtual void FixedUpdate()
    {
        if (m_hit)
        {
            m_effectTime += Time.fixedDeltaTime;

            if (m_effectTime > 2)
                Disabled();

            return;
        }

        Vector3 nextPos = Vector3.Lerp(m_casterPos, m_targetPos, m_elapsedTime);
        // Curve의 높이값 연산
        nextPos.y *= m_curve.Evaluate(m_elapsedTime) * m_maxHeight;
        transform.LookAt(nextPos);
        transform.position = nextPos;
        m_elapsedTime += Time.fixedDeltaTime * m_speed;

        if (m_elapsedTime > 1)
            m_hit = true;
    }
}

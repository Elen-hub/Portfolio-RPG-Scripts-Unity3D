using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour
{
    AudioSource[] m_sounds;
    Transform m_effectPivot;
    float m_effectTime;
    float m_lifeTime;
    float m_targetTime;
    float m_elapsedTime;
    public float ResetTargetTime { set { m_elapsedTime = 0; m_targetTime = value; } }

    public virtual BaseEffect Init(float effectTime)
    {
        gameObject.SetActive(false);
        InitSound();

        m_effectTime = effectTime;
        m_lifeTime = 0;

        return this;
    }
    protected virtual void InitSound()
    {
        m_sounds = GetComponentsInChildren<AudioSource>();
    }
    protected virtual void SetSound()
    {
        if (m_sounds != null)
        {
            for (int i = 0; i < m_sounds.Length; ++i)
            {
                m_sounds[i].mute = !GameSystem.UseSound;
                m_sounds[i].volume = GameSystem.SoundVolume;
            }
        }
    }
    public virtual void Enabled(Transform effectPivot)
    {
        SetSound();
        m_effectPivot = effectPivot;
        m_elapsedTime = 0;
        m_targetTime = m_effectTime + m_lifeTime;
        transform.position = effectPivot.position;
        gameObject.SetActive(true);
    }
    public virtual void Enabled(Vector3 effectPos, Vector3 eulerAngle)
    {
        SetSound();
        transform.position = effectPos;
        transform.eulerAngles = eulerAngle;
        m_elapsedTime = 0;
        m_targetTime = m_effectTime + m_lifeTime;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_effectPivot = null;
        gameObject.SetActive(false);
    }
    public void DisabledTime()
    {
        if (m_elapsedTime > m_targetTime - m_lifeTime)
            return;

        m_elapsedTime = m_targetTime - m_lifeTime;
    }
    protected virtual void LateUpdate()
    {
        m_elapsedTime += Time.deltaTime;

        if(m_targetTime < m_elapsedTime)
        {
            Disabled();
            return;
        }
        if (m_effectPivot != null)
            transform.position = m_effectPivot.position;
    }
}

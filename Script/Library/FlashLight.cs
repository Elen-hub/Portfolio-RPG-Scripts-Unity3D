using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public AnimationCurve FlashCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
    bool m_flag;
    Light m_light;
    float m_intensity;
    float m_elasedTime = 0;
    private void Awake()
    {
        m_light = GetComponent<Light>();
        m_intensity = m_light.intensity;
    }
    private void OnEnable()
    {
        m_elasedTime = 0;
        m_light.intensity = 0;
    }
    private void OnDisable()
    {
        m_light.intensity = 0;
    }
    private void LateUpdate()
    {
        m_elasedTime += Time.deltaTime;
        m_light.intensity = m_intensity * FlashCurve.Evaluate(m_elasedTime);
    }
}

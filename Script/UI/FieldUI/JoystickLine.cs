using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickLine : BaseFieldUI
{
    #region HPBar Shader

    [Range(-1.0f, 1.0f)]
    float Multiplier = 1.0f;

    Vector2 Smoke1Speed = new Vector2(0.05f, 0.1f);
    Vector2 Smoke2Speed = new Vector2(0.1f, 0.1f);
    Vector2 Particles1Speed = new Vector2(0.2f, 0.1f);
    Vector2 Particles2Speed = new Vector2(0.15f, 0.05f);
    float SurfaceSpeed = 0.1f;

    public Material m_material;

    Vector2 smoke1Offset;
    Vector2 smoke2Offset;
    Vector2 particles1Offset;
    Vector2 particles2Offset;
    public float surfaceOffset;
    bool reverseSurfaceOffset;
    #endregion

    protected Image m_childImage;

    private void Awake()
    {
        Init();
    }
    public override void Init()
    {
        m_childImage = transform.GetComponent<Image>();
        m_childImage.material = Instantiate(Resources.Load<Material>("Material/JoystickLineMaterial"));
        m_material = m_childImage.material;

        smoke1Offset = Random.Range(0.0f, 1.0f) * Vector2.one;
        smoke2Offset = Random.Range(0.0f, 1.0f) * Vector2.one;
        particles1Offset = Random.Range(0.0f, 1.0f) * Vector2.one;
        particles2Offset = Random.Range(0.0f, 1.0f) * Vector2.one;
        surfaceOffset = Random.Range(0.0f, 1.0f);
    }

    protected virtual void LateUpdate()
    {
        if (surfaceOffset > 1.2f)
            reverseSurfaceOffset = true;
        else if (surfaceOffset < 0.5f)
            reverseSurfaceOffset = false;

        smoke1Offset += Multiplier * Time.deltaTime * Smoke1Speed;
        smoke2Offset += Multiplier * Time.deltaTime * Smoke2Speed;
        particles1Offset += Multiplier * Time.deltaTime * Particles1Speed;
        particles2Offset += Multiplier * Time.deltaTime * Particles2Speed;

        if(reverseSurfaceOffset)
            surfaceOffset -= Multiplier * Time.deltaTime * SurfaceSpeed;
        else
            surfaceOffset += Multiplier * Time.deltaTime * SurfaceSpeed;

        m_material.SetFloat("_Smoke1OffsetX", smoke1Offset.x);
        m_material.SetFloat("_Smoke1OffsetY", smoke1Offset.y);
        m_material.SetFloat("_Smoke2OffsetX", smoke2Offset.x);
        m_material.SetFloat("_Smoke2OffsetY", smoke2Offset.y);
        m_material.SetFloat("_Particles1OffsetX", particles1Offset.x);
        m_material.SetFloat("_Particles1OffsetY", particles1Offset.y);
        m_material.SetFloat("_Particles2OffsetX", particles1Offset.x);
        m_material.SetFloat("_Particles2OffsetY", particles1Offset.y);
        m_material.SetFloat("_SurfaceOffsetX", surfaceOffset);
    }
}

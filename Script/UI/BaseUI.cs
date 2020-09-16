using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    protected Canvas m_canvas;
    protected CanvasScaler m_canvasScaler;
    protected AudioSource m_sound;

    protected virtual void InitControl()
    {
        m_canvas = transform.GetComponent<Canvas>();
        m_canvasScaler = transform.GetComponent<CanvasScaler>();

        if (m_canvas != null)
        {
            m_canvas.worldCamera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.UI).camera;
            m_canvas.planeDistance = 1;
        }

        if (m_canvasScaler != null)
        {
            m_canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            m_canvasScaler.referenceResolution = new Vector2(1920, 1080);
            m_canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            m_canvasScaler.matchWidthOrHeight = 1;
        }
    }

    protected virtual void InitUI()
    {

    }

    protected virtual void InitSound()
    {
        m_sound = GetComponent<AudioSource>();
        if (m_sound != null)
        {
            m_sound.playOnAwake = false;
            m_sound.loop = false;
        }
    }

    protected virtual void PlaySound()
    {
        if (m_sound == null)
            return;

        m_sound.volume = GameSystem.SoundVolume;
        m_sound.Play();
    }

    public virtual void Open()
    {

    }

    public virtual void Close()
    {

    }

    public virtual void Init()
    {
        InitControl();
        InitUI();
        InitSound();
    }
}

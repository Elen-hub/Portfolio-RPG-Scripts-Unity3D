using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputWindow : MonoBehaviour
{
    InputWindow_QuickSlot m_quickSlot;
    InputWindow_LineJoystick m_lineJoyStick;
    InputWindow_BasicJoystick m_basicJoyStick;
    public void Init()
    {
        m_lineJoyStick = GetComponentInChildren<InputWindow_LineJoystick>();
        m_lineJoyStick.Init();
        m_basicJoyStick = GetComponentInChildren<InputWindow_BasicJoystick>();
        m_basicJoyStick.Init();
        m_quickSlot = GetComponentInChildren<InputWindow_QuickSlot>();
        m_quickSlot.Init();
    }
    public void Enabled(int JoystickNumber)
    {
        if (GameSystem.Joystick == 0)
        {
            m_lineJoyStick.Disabled();
            m_basicJoyStick.Enabled();
        }
        else
        {
            m_lineJoyStick.Enabled();
            m_basicJoyStick.Disabled();
        }
        m_quickSlot.Enabled();
    }
    public void ResetQuickSlot(int quickslotNum)
    {
        m_quickSlot.ResetQuickSlot(quickslotNum);
    }
    public void Disabled()
    {
        m_lineJoyStick.Disabled();
        m_basicJoyStick.Disabled();
        m_quickSlot.Disabled();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected BaseCharacter m_targetCharacter;
    public BaseState(BaseCharacter target)
    {
        m_targetCharacter = target;
    }
    public abstract void OnStateEnter();
    public abstract void OnStateStay(float deltaTime);
    public abstract void OnStateExit();
}

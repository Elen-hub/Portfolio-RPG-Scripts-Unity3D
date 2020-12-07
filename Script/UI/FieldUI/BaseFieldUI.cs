
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFieldUI : MonoBehaviour
{
    protected FieldUI.Register dRegister;
    public virtual void Init(FieldUI.Register register)
    {
        dRegister = register;
    }
}

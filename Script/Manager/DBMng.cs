using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class DBMng : TSingleton<DBMng>
{
    public override void Init()
    {
        IsLoad = true;
    }
}

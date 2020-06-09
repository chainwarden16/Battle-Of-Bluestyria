using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Estado
{

    public Estado()
    {

    }

    public virtual IEnumerator StartState()
    {

        yield break;

    }


}

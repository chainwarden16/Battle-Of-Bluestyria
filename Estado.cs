using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Estado
{

    protected CombatePorTurnos combatePorTurnos;

    public Estado(CombatePorTurnos turnos)
    {
        combatePorTurnos = turnos;
    }

    public virtual IEnumerator StartState()
    {

        yield break;

    }

    

    public CombatePorTurnos GetCombatePorTurnos()
    {
        return combatePorTurnos;
    }

}

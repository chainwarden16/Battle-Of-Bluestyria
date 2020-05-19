using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatePorTurnos : MaquinaDeEstados
{
    // Start is called before the first frame update
    public void ComenzarCombate()
    {
        SetEstado(new EstadoInicio(this));
    }


}

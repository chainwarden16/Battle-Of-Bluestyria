using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaquinaDeEstados : MonoBehaviour
{

    private Estado estado;

    public void SetEstado(Estado estadoNuevo)
    {
        
        estado = estadoNuevo;

        IniciacionCombate.SetEstado(estadoNuevo);

    }

    public void ComenzarCombate()
    {
        SetEstado(new EstadoInicio());
    }

    public Estado GetEstado()
    {
        return estado;
    }

}

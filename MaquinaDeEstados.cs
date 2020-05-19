using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaquinaDeEstados : MonoBehaviour
{

    protected Estado estado;

    public void SetEstado(Estado estadoNuevo)
    {
        
        estado = estadoNuevo;

        IniciacionCombate.SetEstado(estadoNuevo);

    }

    public Estado GetEstado()
    {
        return estado;
    }

}

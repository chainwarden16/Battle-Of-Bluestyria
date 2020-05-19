using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoInicio : Estado
{
    
    private GameManager gameMan;

    private MaquinaDeEstados maquinaEstados = IniciacionCombate.GetMaquinaDeEstados();

    public override IEnumerator StartState()
    {
        //Iniciamos el combate. Para eso, vamos al estado de Espera

        gameMan = GameManager.InstanciarGameManager();
        
        gameMan.InicializarCombate();

        maquinaEstados.SetEstado(new EstadoEsperar(combatePorTurnos));

        yield return new WaitForSeconds(0.1f);

    }

    public EstadoInicio(CombatePorTurnos turnos) : base(turnos)
    {

    }

}

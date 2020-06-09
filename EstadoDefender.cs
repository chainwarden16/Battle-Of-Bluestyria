using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoDefender : Estado
{
    private GameObject unidad;

    private MaquinaDeEstados maquina;

    public EstadoDefender(CombatePorTurnos combatePorTurnos): base(combatePorTurnos)
    {


    }
    // Start is called before the first frame update
    public override IEnumerator StartState()
    {
        maquina = IniciacionCombate.GetMaquinaDeEstados();

        unidad = EstadoEsperar.GetUnidadSeleccionada();
        unidad.GetComponent<Unidad>().SetEstaDefendiendo(true);
        GameManager.BorrarCasillas();
        GameManager.TerminarTurnoUnidad(unidad, combatePorTurnos);
        GameManager.CerrarInterfazUnidad();

        maquina.SetEstado(new EstadoEsperar(combatePorTurnos));



        yield return new WaitForSeconds(0.1f);
    }

    
}

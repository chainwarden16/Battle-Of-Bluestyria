using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EstadoAtacar : Estado
{

    private GameObject atacante;

    private GameObject objetivo;

    private GameObject cursor;

    private MaquinaDeEstados maquina;

    private bool resetado = false;

    private GameManager gm = GameManager.InstanciarGameManager();

    private Animator animator;

    private GameObject popUp;

    private Clase clase;

    private bool sePuedePulsarTecla = true;

    private static Animator animacionesAtaques;

    private static GameObject objetoAnimaciones;

    private AnimatorClipInfo[] info;
    public EstadoAtacar(CombatePorTurnos combatePorTurnos): base(combatePorTurnos)
    {

    }

    public override IEnumerator StartState()
    {
        if(resetado == false)
        {
            objetoAnimaciones = GameObject.Find("AnimacionesCombate");
            animacionesAtaques = objetoAnimaciones.GetComponent<Animator>();
            maquina = IniciacionCombate.GetMaquinaDeEstados();
            atacante = EstadoEsperar.GetUnidadSeleccionada();
            cursor = GameObject.Find("Cursor");
            animator = atacante.GetComponent<Animator>();
            popUp = GameObject.Find("NumeroPopUp");
            clase = atacante.GetComponent<Clase>();

        }

        if (sePuedePulsarTecla)
        {
            GameManager.MoverElCursor();
        }
        


        if(Input.GetKeyUp(KeyCode.C) && GameManager.HayEnemigoEnCelda(cursor.transform.position) && sePuedePulsarTecla && GameManager.GetListaCeldasAtacar().Contains(cursor.transform.position))
        {
            sePuedePulsarTecla = false;
            objetivo = GameManager.SeleccionarUnidad(cursor.transform.position);
            GameManager.CambiarDireccionUnidad(atacante);
            atacante.GetComponent<Unidad>().SetEstaAtacando(true, animator);
            
            
            GameManager.BorrarCasillas();

            //Se mira de qué tipo es la unidad atacante (su clase) para reproducir el sonido adecuado

            if (clase.GetTipoClase() == 1)
            {
                GameManager.ReproducirSonido("Audio/Ice2");
                GameManager.ReproducirAnimacion(20, objetivo);


            }else if (clase.GetTipoClase() == 2)
            {
                GameManager.ReproducirSonido("Audio/Blow2");
                GameManager.ReproducirAnimacion(21, objetivo);
            }
            else if (clase.GetTipoClase() == 3)
            {
                GameManager.ReproducirSonido("Audio/Saint3");
                GameManager.ReproducirAnimacion(22, objetivo);
            }
            else if (clase.GetTipoClase() == 4)
            {
                GameManager.ReproducirSonido("Audio/Crossbow");
                GameManager.ReproducirAnimacion(23, objetivo);
            }

            info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
            yield return new WaitForSeconds(info[0].clip.length);

            animacionesAtaques.SetInteger("ID", -1);
            yield return new WaitForSeconds(0.1f);

            GameManager.AtacarUnidad(atacante, objetivo);
            

            yield return new WaitForSecondsRealtime(2.5f);
            
            atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);
            atacante.GetComponent<Unidad>().setTurnoAcabado(true);
            

            GameManager.EliminarPopUp(popUp);
            

            //gm.EsperarSegundos(2);


            Color colo = atacante.GetComponent<SpriteRenderer>().color;

            colo.a = 0.4f;

            atacante.GetComponent<SpriteRenderer>().color = colo;

            GameManager.SoltarUnidad(atacante);

            atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
            

            atacante = null;
            objetivo = null;
            EstadoEsperar.SetUnidadSeleccionada(null);

            GameManager.CerrarInterfazUnidad();

            maquina.SetEstado(new EstadoEsperar(combatePorTurnos));
            yield return new WaitForSeconds(0.01f);
        }else if (Input.GetKeyUp(KeyCode.X)){ //cancelas la accion

            GameManager.MoverUnidad(atacante, EstadoMover.GetPosicionOriginal());
            GameManager.BorrarCasillas();
            GameManager.SoltarUnidad(atacante);
            
            atacante = null;
            objetivo = null;
            EstadoEsperar.SetUnidadSeleccionada(null);
            maquina.SetEstado(new EstadoEsperar(combatePorTurnos));

            yield return new WaitForSeconds(0.01f);

        }

    }

}

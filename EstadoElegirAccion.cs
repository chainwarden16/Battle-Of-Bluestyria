using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoElegirAccion : Estado
{

    #region Propiedades

    public GameObject cursor;

    private static Vector3 posicionOriginalUnidad;

    private GameObject unidadAMover;

    private GameObject cursorMenu;

    private GameObject botonAtacar;

    private GameObject botonDefender;

    private GameObject botonHabilidad;

    private GameObject cursorHabilidad;

    private GameObject botonObjetos;

    private int posicionSelectorMenu;

    private static bool reseteado = false;

    private MaquinaDeEstados maquinaEstados = IniciacionCombate.GetMaquinaDeEstados();

    private GameManager gm = GameManager.InstanciarGameManager();

    private int direccionOriginal;

    private Animator animator;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    #endregion

    #region Constructor

    public EstadoElegirAccion(CombatePorTurnos comba) : base(comba)
    {

    }

    #endregion


    public override IEnumerator StartState()
    {
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        if (reseteado == false)
        {

            cursor = GameObject.Find("Cursor");

            cursorMenu = GameObject.Find("Flecha-Menu-Unidad");

            cursorHabilidad = GameObject.Find("Flecha-Lista-Habilidades");

            botonAtacar = GameObject.Find("Boton-Atacar");

            botonDefender = GameObject.Find("Boton-Habilidad");

            botonHabilidad = GameObject.Find("Boton-Defender");

            botonObjetos = GameObject.Find("Boton-Objetos");

            unidadAMover = EstadoEsperar.GetUnidadSeleccionada();

            posicionOriginalUnidad = new Vector3(EstadoMover.GetPosicionOriginal().x, EstadoMover.GetPosicionOriginal().y, 0f);

            posicionSelectorMenu = 0;

            animator = unidadAMover.GetComponent<Animator>();

            direccionOriginal = unidadAMover.GetComponent<Unidad>().GetDireccionAMirar(animator);

            reseteado = true;

        }

        if (Input.GetKeyUp(KeyCode.DownArrow) && posicionSelectorMenu < 3)
        {
            GameManager.ReproducirSonido("Audio/Cursor1");
            cursorMenu.transform.position = new Vector3(cursorMenu.transform.position.x, cursorMenu.transform.position.y - 1f, 0f);
            posicionSelectorMenu++;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) && posicionSelectorMenu > 0)
        {
            GameManager.ReproducirSonido("Audio/Cursor1");
            cursorMenu.transform.position = new Vector3(cursorMenu.transform.position.x, cursorMenu.transform.position.y + 1f, 0f);
            posicionSelectorMenu--;
        }
        else if (Input.GetKeyUp(KeyCode.X))
        { //cancela el mover la unidad a este punto
            GameManager.ReproducirSonido("Audio/Cancel2");

            GameManager.MoverUnidad(unidadAMover, posicionOriginalUnidad);
            CerrarMenuAcciones();

            GameManager.SoltarUnidad(unidadAMover);

            unidadAMover.GetComponent<Unidad>().SetDireccionAMirar(direccionOriginal, animator);
            unidadAMover.GetComponent<Unidad>().SetEstaCaminando(false, animator);

            unidadAMover = null;
            EstadoEsperar.SetUnidadSeleccionada(null);
            GameManager.BorrarCasillas();
            
            maquinaEstados.SetEstado(new EstadoEsperar(combatePorTurnos));
            reseteado = false;
            yield return new WaitForSeconds(0.1f);

        }
        else if (Input.GetKeyUp(KeyCode.C))
        {//confirmar accion

           

            if (posicionSelectorMenu == 0)
            {//atacar

                GameManager.ReproducirSonido("Audio/Decision2");
                GameManager.PosicionesPosiblesAtacarUnidad(unidadAMover);
                CerrarMenuAcciones();
                reseteado = false;
                maquinaEstados.SetEstado(new EstadoAtacar(combatePorTurnos));
                yield return new WaitForSeconds(0.1f);
                

            }
            else if (posicionSelectorMenu == 1) //acabar turno
            {

                GameManager.ReproducirSonido("Audio/Decision2");

                CerrarMenuAcciones();
                reseteado = false;
                unidadAMover.GetComponent<Unidad>().SetEstaCaminando(false, animator);
                maquinaEstados.SetEstado(new EstadoDefender(combatePorTurnos));
                yield return new WaitForSeconds(0.1f);
                

            }
            else if(posicionSelectorMenu == 2)
            { //Habilidad
                GameManager.ReproducirSonido("Audio/Decision2");
                CerrarMenuAcciones();
                
                reseteado = false;
                
                maquinaEstados.SetEstado(new EstadoHabilidad(combatePorTurnos));
                yield return new WaitForSeconds(0.1f);


            }
            else //Objetos
            {
                GameManager.ReproducirSonido("Audio/Decision2");
                if (TropaEscogida.GetConsumibles().Count != 0) //si tienes objetos
                {
                    CerrarMenuAcciones();

                    reseteado = false;

                    maquinaEstados.SetEstado(new EstadoElegirConsumible(combatePorTurnos));
                    yield return new WaitForSeconds(0.1f);
                }
                else //si no se tienen consumibles
                {
                    CerrarMenuAcciones();
                    unidadAMover.GetComponent<Unidad>().SetEstaCaminando(false, animator);
                    gm.MostrarMensajePerdidaTurno("No te quedan objetos.", unidadAMover);
                    maquinaEstados.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));
                    yield return new WaitForSeconds(0.1f);
                }

            }


        }


    }

    

    private void CerrarMenuAcciones()
    {
        cursorMenu.transform.position = new Vector3(unidadAMover.transform.position.x + 250.0f, unidadAMover.transform.position.y, 0f);
        botonAtacar.transform.position = new Vector3(cursorMenu.transform.position.x + 250.0f, cursorMenu.transform.position.y, 0f);
        botonDefender.transform.position = new Vector3(botonAtacar.transform.position.x, botonAtacar.transform.position.y - 0.45f, 0f);
        botonHabilidad.transform.position = new Vector3(botonDefender.transform.position.x, botonDefender.transform.position.y - 0.45f, 0f);
        botonObjetos.transform.position = new Vector3(botonHabilidad.transform.position.x, botonHabilidad.transform.position.y - 0.45f, 0f);
    }

    public static Vector3 GetPosicionOriginal()
    {
        return posicionOriginalUnidad;
    }

    public static void SetPosicionOriginal(Vector3 trans)
    {
        posicionOriginalUnidad = trans;

    }

    public static void ResetearEstado(bool res)
    {
        reseteado = res;
    }

}

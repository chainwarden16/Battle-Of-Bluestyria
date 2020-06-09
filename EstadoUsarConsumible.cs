using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoUsarConsumible : Estado
{
    public EstadoUsarConsumible()
    {

    }

    private GameObject cursorHabilidad;

    private GameObject cursor;

    private GameObject atacante;

    private static GameObject objetivo;

    private GameObject fondoDescripcion;

    private GameObject[] fondoHabilidades;

    private Consumible consumible;

    private MaquinaDeEstados maquina = IniciacionCombate.GetMaquinaDeEstados();

    private List<Consumible> consumiblesActuales;

    private bool reseteado = false;

    private Animator animator;

    private GameObject popUp;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    private bool sePuedePulsarTecla = true;

    private static Animator animacionesAtaques;

    private static GameObject objetoAnimaciones;

    private AnimatorClipInfo[] info;

    //=======

    private GameObject botonAtacar;

    private GameObject botonDefender;

    private GameObject botonHabilidad;

    private GameObject botonObjetos;

    private GameObject unidadAMover;

    private GameObject cursorMenu;

    private GameObject camara;

    public override IEnumerator StartState()
    {

        if (reseteado == false)
        {
            objetoAnimaciones = GameObject.Find("AnimacionesCombate");
            animacionesAtaques = objetoAnimaciones.GetComponent<Animator>();

            audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();

            consumible = EstadoElegirConsumible.GetConsumibleElegido();
            
            cursorHabilidad = GameObject.Find("Flecha-Lista-Habilidades");

            fondoDescripcion = GameObject.Find("Fondo-Descripcion");

            fondoHabilidades = GameObject.FindGameObjectsWithTag("FondoHabilidad");

            cursor = GameObject.Find("Cursor");

            atacante = EstadoEsperar.GetUnidadSeleccionada();

            animator = atacante.GetComponent<Animator>();

            popUp = GameObject.Find("NumeroPopUp");

            //====

            cursorMenu = GameObject.Find("Flecha-Menu-Unidad");

            botonAtacar = GameObject.Find("Boton-Atacar");

            botonDefender = GameObject.Find("Boton-Habilidad");

            botonHabilidad = GameObject.Find("Boton-Defender");

            botonObjetos = GameObject.Find("Boton-Objetos");

            unidadAMover = EstadoEsperar.GetUnidadSeleccionada();

            camara = GameObject.Find("Main Camera");

            reseteado = true;
        }

        if (sePuedePulsarTecla)
        {
            GameManager.MoverElCursor();
        }
        


        if (Input.GetKeyUp(KeyCode.C) && GameManager.HayAliadoEnCelda(cursor.transform.position) && sePuedePulsarTecla && GameManager.GetListaCeldasHabilidad().Contains(cursor.transform.position))
        {
            if (!audioSource.isPlaying)
            {
                cursorSFX = Resources.Load<AudioClip>("Audio/Heal1");
                audioSource.PlayOneShot(cursorSFX, 1f);
            }
            sePuedePulsarTecla = false;
            GameManager.CambiarDireccionUnidad(atacante);
            atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
            

            objetivo = GameManager.SeleccionarUnidad(cursor.transform.position);
            GameManager.BorrarCasillas();

            switch (consumible.GetID())
            {
                case 0: //Pocion
                    
                    if (!audioSource.isPlaying)
                    {
                        cursorSFX = Resources.Load<AudioClip>("Audio/Heal1");
                        audioSource.PlayOneShot(cursorSFX, 1f);
                    }

                    consumiblesActuales = TropaEscogida.GetConsumibles();
                    consumiblesActuales.Remove(consumiblesActuales.Find(x => x.GetID() == 0));
                    GameManager.CurarVidaUnidad(objetivo, 20);

                    GameManager.ReproducirAnimacion(7, objetivo);
                    info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                    yield return new WaitForSeconds(info[0].clip.length + 1f);
                    //Se invoca el numero que indica cuánto se va a curar

                    animacionesAtaques.SetInteger("ID", -1);
                    yield return new WaitForSeconds(0.1f);

                    break;

                case 1:

                    //Tonico
                    if (!audioSource.isPlaying)
                    {
                        cursorSFX = Resources.Load<AudioClip>("Audio/Heal2");
                        audioSource.PlayOneShot(cursorSFX, 1f);
                    }
                    consumiblesActuales = TropaEscogida.GetConsumibles();
                    consumiblesActuales.Remove(consumiblesActuales.Find(x => x.GetID() == 1));
                    GameManager.EliminarEstadosDañinosUnidad(objetivo);

                    GameManager.ReproducirAnimacion(16, objetivo);

                    info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                    yield return new WaitForSeconds(info[0].clip.length + 1f);
                    //Se invoca el numero que indica cuánto se va a curar

                    animacionesAtaques.SetInteger("ID", -1);
                    yield return new WaitForSeconds(0.1f);

                    break;

                case 2: //Elixir
                    if (!audioSource.isPlaying)
                    {
                        cursorSFX = Resources.Load<AudioClip>("Audio/Heal3");
                        audioSource.PlayOneShot(cursorSFX, 1f);
                    }
                    consumiblesActuales = TropaEscogida.GetConsumibles();
                    consumiblesActuales.Remove(consumiblesActuales.Find(x => x.GetID() == 2));
                    GameManager.CurarVidaUnidad(objetivo, objetivo.GetComponent<Clase>().GetPSMax());
                    GameManager.EliminarEstadosDañinosUnidad(objetivo);

                    GameManager.ReproducirAnimacion(24, objetivo);

                    info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                    yield return new WaitForSeconds(info[0].clip.length + 1f);
                    //Se invoca el numero que indica cuánto se va a curar

                    animacionesAtaques.SetInteger("ID", -1);
                    yield return new WaitForSeconds(0.1f);


                    break;

                default:
                    break;

            }

            yield return new WaitForSecondsRealtime(1.5f);

            GameManager.EliminarPopUp(popUp);
            atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);
            GameManager.BorrarCasillas();
            GameManager.TerminarTurnoUnidad(atacante);
            GameManager.CerrarInterfazUnidad();

        }
        else if (Input.GetKeyUp(KeyCode.X)) //se cancela la accion
        {
            CancelarAccion();

            yield return new WaitForSeconds(0.01f);
        }


        yield return new WaitForSeconds(0.01f);

    }



    private void CerrarMenuHabilidades()
    {
        cursorHabilidad.transform.position = new Vector3(atacante.transform.position.x + 300.0f, atacante.transform.position.y, 0f);
        fondoDescripcion.transform.position = new Vector3(cursorHabilidad.transform.position.x + 300.0f, cursorHabilidad.transform.position.y - 0.9f, 0f);

        foreach (GameObject gob in fondoHabilidades)
        {

            gob.transform.Translate(300.0f, 0f, 0f);

        }

    }

   

    private void CancelarAccion() //se vuelve a EstadoEsperar y se resetea todo
    {
        if (!audioSource.isPlaying)
        {
            cursorSFX = Resources.Load<AudioClip>("Audio/Cancel2");
            audioSource.PlayOneShot(cursorSFX, 1f);
        }
        CerrarMenuHabilidades();
        GameManager.BorrarCasillas();
        //GameManager.SoltarUnidad(atacante);
        //atacante.GetComponent<Unidad>().SetDireccionAMirar(EstadoMover.GetDireccionOriginal(), animator);
        atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
        //atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);
        //atacante.transform.position = EstadoMover.GetPosicionOriginal();
        cursor.transform.position = atacante.transform.position;
        camara.transform.position = atacante.transform.position;
        atacante = null;
        objetivo = null;
        //EstadoEsperar.SetUnidadSeleccionada(null);
        InvocarMenuAcciones();
        maquina.SetEstado(new EstadoElegirAccion());


    }
    public static void SetObjetivo(GameObject gob)
    {
        objetivo = gob;
    }

    private void InvocarMenuAcciones()
    {
        cursorMenu.transform.position = new Vector3(unidadAMover.transform.position.x + 0.8f, unidadAMover.transform.position.y, 0f);
        botonAtacar.transform.position = new Vector3(cursorMenu.transform.position.x + 2.3f, cursorMenu.transform.position.y, 0f);
        botonHabilidad.transform.position = new Vector3(botonAtacar.transform.position.x, botonAtacar.transform.position.y - 1f, 0f);
        botonDefender.transform.position = new Vector3(botonHabilidad.transform.position.x, botonHabilidad.transform.position.y - 1f, 0f);
        botonObjetos.transform.position = new Vector3(botonDefender.transform.position.x, botonDefender.transform.position.y - 1f, 0f);
    }


}

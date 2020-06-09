using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoMover : Estado
{

    public GameObject cursor;

    private static Vector3 posicionOriginalUnidad;

    private GameObject unidadAMover;

    private GameObject cursorMenu;

    private GameObject botonAtacar;

    private GameObject botonDefender;

    private GameObject botonHabilidad;

    private GameObject cursorHabilidad;

    private GameObject botonObjetos;

    private static bool seHaResteadoEstado = false;

    private MaquinaDeEstados maquinaDeEstados = IniciacionCombate.GetMaquinaDeEstados();

    private static int direccionOriginal;

    private Animator animator;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    public EstadoMover(CombatePorTurnos comba): base(comba)
    {

    }


    public override IEnumerator StartState()
    {

        //se espera a que el jugador elija o cancele la accion

        if(seHaResteadoEstado == false)
        {
            audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();

            cursor = GameObject.Find("Cursor");

            cursorMenu = GameObject.Find("Flecha-Menu-Unidad");

            cursorHabilidad = GameObject.Find("Flecha-Lista-Habilidades");

            botonAtacar = GameObject.Find("Boton-Atacar");

            botonDefender = GameObject.Find("Boton-Habilidad");

            botonHabilidad = GameObject.Find("Boton-Defender");

            botonObjetos = GameObject.Find("Boton-Objetos");

            unidadAMover = EstadoEsperar.GetUnidadSeleccionada();


            posicionOriginalUnidad = new Vector3(unidadAMover.transform.position.x, unidadAMover.transform.position.y, 0f);

            animator = unidadAMover.GetComponent<Animator>();

            direccionOriginal = unidadAMover.GetComponent<Unidad>().GetDireccionAMirar(animator);

            seHaResteadoEstado = true;

        }



        GameManager.MoverElCursor();



        if (Input.GetKeyUp(KeyCode.C) && GameManager.GetListaCeldasMovimiento().Contains(cursor.transform.position)) //selecciona una nueva localizacion a la que mover la unidad
        {

            if (!audioSource.isPlaying)
            {
                cursorSFX = Resources.Load<AudioClip>("Audio/Decision2");
                audioSource.PlayOneShot(cursorSFX, 1f);
            }

            GameManager.CambiarDireccionUnidad(unidadAMover);

            GameManager.MoverUnidad(unidadAMover, cursor.transform.position);

            InvocarMenuAcciones();
            GameManager.BorrarCasillas();
            maquinaDeEstados.SetEstado(new EstadoElegirAccion(combatePorTurnos));
            seHaResteadoEstado = false;
            yield return new WaitForSeconds(0.01f);


        }
        else if (Input.GetKeyUp(KeyCode.X))
        {


            if (!audioSource.isPlaying)
            {
                cursorSFX = Resources.Load<AudioClip>("Audio/Cancel2");
                audioSource.PlayOneShot(cursorSFX, 1f);
            }

            CerrarMenuAcciones();
            unidadAMover.GetComponent<Unidad>().SetDireccionAMirar(direccionOriginal, animator);
            unidadAMover.GetComponent<Unidad>().SetEstaCaminando(false, animator);
            GameManager.SoltarUnidad(unidadAMover);
            
            unidadAMover = null;
            EstadoEsperar.SetUnidadSeleccionada(null);
            GameManager.BorrarCasillas();
            maquinaDeEstados.SetEstado(new EstadoEsperar(combatePorTurnos));
            seHaResteadoEstado = false;
            yield return new WaitForSeconds(0.01f);

        }



    }

    private void InvocarMenuAcciones()
    {
        cursorMenu.transform.position = new Vector3(unidadAMover.transform.position.x + 0.8f, unidadAMover.transform.position.y, 0f);
        botonAtacar.transform.position = new Vector3(cursorMenu.transform.position.x + 2.3f, cursorMenu.transform.position.y, 0f);
        botonHabilidad.transform.position = new Vector3(botonAtacar.transform.position.x, botonAtacar.transform.position.y - 1f, 0f);
        botonDefender.transform.position = new Vector3(botonHabilidad.transform.position.x, botonHabilidad.transform.position.y - 1f, 0f);
        botonObjetos.transform.position = new Vector3(botonDefender.transform.position.x, botonDefender.transform.position.y -1f, 0f);
    }

    private void CerrarMenuAcciones()
    {
        cursorMenu.transform.position = new Vector3(unidadAMover.transform.position.x + 250.0f, unidadAMover.transform.position.y, 0f);
        botonAtacar.transform.position = new Vector3(cursorMenu.transform.position.x + 250.0f, cursorMenu.transform.position.y, 0f);
        botonDefender.transform.position = new Vector3(botonAtacar.transform.position.x, botonAtacar.transform.position.y - 1f, 0f);
        botonHabilidad.transform.position = new Vector3(botonDefender.transform.position.x, botonDefender.transform.position.y - 1f, 0f);
        botonObjetos.transform.position = new Vector3(botonHabilidad.transform.position.x, botonHabilidad.transform.position.y - 1f, 0f);
    }

    public static Vector3 GetPosicionOriginal()
    {
        return posicionOriginalUnidad;
    }

    public static void SetPosicionOriginal(Vector3 trans) 
    {
        posicionOriginalUnidad = trans;

    }

    public static void SetSehaReseteadoEstado(bool reseteado)
    {
        seHaResteadoEstado = reseteado;
    }

    public static int GetDireccionOriginal()
    {
        return direccionOriginal;
    }
    
}

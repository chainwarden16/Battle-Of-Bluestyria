using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EstadoElegirConsumible : Estado
{

    private GameObject cursorHabilidad;

    private GameObject cursor;

    private int posicionCursorHabilidad;

    private GameObject atacante;

    private GameObject fondoDescripcion;

    private GameObject[] fondoHabilidades;

    private List<Consumible> consumibles;

    private bool reseteado = false;

    private MaquinaDeEstados maquina = IniciacionCombate.GetMaquinaDeEstados();

    private static Consumible consumibleElegido;

    private static AudioSource audioSource;
    private static AudioClip cursorSFX;

    //====

    private GameObject botonAtacar;

    private GameObject botonDefender;

    private GameObject botonHabilidad;

    private GameObject botonObjetos;

    private GameObject unidadAMover;

    private GameObject cursorMenu;

    private GameObject camara;
    public EstadoElegirConsumible(CombatePorTurnos comba) : base(comba)
    {

    }

    public override IEnumerator StartState()
    {
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        if (reseteado == false)

        {

            posicionCursorHabilidad = 0;

            cursorHabilidad = GameObject.Find("Flecha-Lista-Habilidades");

            fondoDescripcion = GameObject.Find("Fondo-Descripcion");

            fondoHabilidades = GameObject.FindGameObjectsWithTag("FondoHabilidad");

            cursor = GameObject.Find("Cursor");

            atacante = EstadoEsperar.GetUnidadSeleccionada();

            consumibles = TropaEscogida.GetConsumibles();

            //=====

            cursorMenu = GameObject.Find("Flecha-Menu-Unidad");

            botonAtacar = GameObject.Find("Boton-Atacar");

            botonDefender = GameObject.Find("Boton-Habilidad");

            botonHabilidad = GameObject.Find("Boton-Defender");

            botonObjetos = GameObject.Find("Boton-Objetos");

            unidadAMover = EstadoEsperar.GetUnidadSeleccionada();

            camara = GameObject.Find("Main Camera");

            reseteado = true;

            InvocarMenuHabilidades();

        }

        //Se mira qué consumible se quiere escoger

        if (Input.GetKeyUp(KeyCode.DownArrow) && posicionCursorHabilidad < consumibles.Count - 1)
        {
            GameManager.ReproducirSonido("Audio/Cursor1");
            cursorHabilidad.transform.position = new Vector3(cursorHabilidad.transform.position.x, cursorHabilidad.transform.position.y - 0.9f, 0f);
            posicionCursorHabilidad++;
            TextMeshPro descripcionHabilidad = fondoDescripcion.GetComponentInChildren<TextMeshPro>();

            descripcionHabilidad.text = consumibles[posicionCursorHabilidad].GetDescripcion();

        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) && posicionCursorHabilidad > 0)
        {
            GameManager.ReproducirSonido("Audio/Cursor1");
            cursorHabilidad.transform.position = new Vector3(cursorHabilidad.transform.position.x, cursorHabilidad.transform.position.y + 0.9f, 0f);
            posicionCursorHabilidad--;
            TextMeshPro descripcionHabilidad = fondoDescripcion.GetComponentInChildren<TextMeshPro>();

            descripcionHabilidad.text = consumibles[posicionCursorHabilidad].GetDescripcion();
        }

        else if (Input.GetKeyUp(KeyCode.C))
        {
            GameManager.ReproducirSonido("Audio/Decision2");
            consumibleElegido = consumibles[posicionCursorHabilidad];

            CerrarMenuHabilidades();
            GameManager.PosicionesPosiblesUsarConsumible(atacante, consumibleElegido);
            
            maquina.SetEstado(new EstadoUsarConsumible(combatePorTurnos));
            reseteado = false;
            yield return new WaitForSeconds(0.01f);

        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            GameManager.ReproducirSonido("Audio/Cancel2");
            //GameManager.MoverUnidad(atacante, EstadoMover.GetPosicionOriginal());
            GameManager.BorrarCasillas();
            //GameManager.SoltarUnidad(atacante);
            cursor.transform.position = atacante.transform.position;
            camara.transform.position = atacante.transform.position;
            CerrarMenuHabilidades();
            atacante = null;
            //EstadoEsperar.SetUnidadSeleccionada(null);
            InvocarMenuAcciones();
            maquina.SetEstado(new EstadoElegirAccion(combatePorTurnos));
            reseteado = false;
            yield return new WaitForSeconds(0.01f);

        }
    }

    public static Consumible GetConsumibleElegido()
    {
        return consumibleElegido;
    }

    // Update is called once per frame


    private void InvocarMenuHabilidades()
    {
        cursorHabilidad.transform.position = new Vector3(atacante.transform.position.x + 1f, atacante.transform.position.y + 2f, 0f);
        fondoDescripcion.transform.position = new Vector3(cursorHabilidad.transform.position.x - 3f, atacante.transform.position.y, 0f);

        for (int indice = 0; indice < consumibles.Count; indice++)
        {

            fondoHabilidades[indice].transform.position = new Vector3(cursorHabilidad.transform.position.x + 2.2f, cursorHabilidad.transform.position.y - (indice * 0.9f), 0f);
            TextMeshPro textoNombreHabilidad = fondoHabilidades[indice].GetComponentInChildren<TextMeshPro>();

            textoNombreHabilidad.text = consumibles[indice].GetNombre();


        }

        TextMeshPro textoFondoDescripcion = fondoDescripcion.GetComponentInChildren<TextMeshPro>();
        textoFondoDescripcion.text = consumibles[0].GetDescripcion();
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

    private void InvocarMenuAcciones()
    {
        cursorMenu.transform.position = new Vector3(unidadAMover.transform.position.x + 0.8f, unidadAMover.transform.position.y, 0f);
        botonAtacar.transform.position = new Vector3(cursorMenu.transform.position.x + 2.3f, cursorMenu.transform.position.y, 0f);
        botonHabilidad.transform.position = new Vector3(botonAtacar.transform.position.x, botonAtacar.transform.position.y - 1f, 0f);
        botonDefender.transform.position = new Vector3(botonHabilidad.transform.position.x, botonHabilidad.transform.position.y - 1f, 0f);
        botonObjetos.transform.position = new Vector3(botonDefender.transform.position.x, botonDefender.transform.position.y - 1f, 0f);
    }

}

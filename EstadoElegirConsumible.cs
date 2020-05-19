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
            GameManager.MoverUnidad(atacante, EstadoMover.GetPosicionOriginal());
            GameManager.BorrarCasillas();
            GameManager.SoltarUnidad(atacante);
            CerrarMenuHabilidades();
            atacante = null;
            EstadoEsperar.SetUnidadSeleccionada(null);
            maquina.SetEstado(new EstadoEsperar(combatePorTurnos));
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



}

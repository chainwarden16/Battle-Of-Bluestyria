using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EstadoHabilidad : Estado
{

    private GameObject cursorHabilidad;

    private GameObject cursor;

    private int posicionCursorHabilidad;

    private GameObject atacante;

    private GameObject fondoDescripcion;

    private GameObject[] fondoHabilidades;

    private List<Habilidad> habilidades;

    private bool reseteado = false;

    private MaquinaDeEstados maquina = IniciacionCombate.GetMaquinaDeEstados();

    private static Habilidad habilidadElegida;

    //== Para cancelar la acción y volver al menú de acciones

    private GameObject botonAtacar;

    private GameObject botonDefender;

    private GameObject botonHabilidad;

    private GameObject botonObjetos;

    private GameObject unidadAMover;

    private GameObject cursorMenu;

    private GameObject camara;

    public EstadoHabilidad()
    {

    }

    // Start is called before the first frame update
    public override IEnumerator StartState()
    {

        if(reseteado == false)
        
        {

            posicionCursorHabilidad = 0;

            cursorHabilidad = GameObject.Find("Flecha-Lista-Habilidades");

            fondoDescripcion = GameObject.Find("Fondo-Descripcion");

            fondoHabilidades = GameObject.FindGameObjectsWithTag("FondoHabilidad");

            cursor = GameObject.Find("Cursor");

            atacante = EstadoEsperar.GetUnidadSeleccionada();

            habilidades = atacante.GetComponent<Clase>().GetHabilidades();

            //====

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

        //Se mira qué habilidad se quiere escoger

        if (Input.GetKeyUp(KeyCode.DownArrow) && posicionCursorHabilidad < habilidades.Count-1)
        {
            GameManager.ReproducirSonido("Audio/Cursor1");
            cursorHabilidad.transform.position = new Vector3(cursorHabilidad.transform.position.x, cursorHabilidad.transform.position.y - 0.9f, 0f);
            posicionCursorHabilidad++;
            TextMeshPro descripcionHabilidad = fondoDescripcion.GetComponentInChildren<TextMeshPro>();

            descripcionHabilidad.text = habilidades[posicionCursorHabilidad].GetDescripcion()+"\n Coste: "+habilidades[posicionCursorHabilidad].GetCoste() + " PS"+ "\n Daño: " + habilidades[posicionCursorHabilidad].GetDano();

        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) && posicionCursorHabilidad > 0)
        {
            GameManager.ReproducirSonido("Audio/Cursor1");
            cursorHabilidad.transform.position = new Vector3(cursorHabilidad.transform.position.x, cursorHabilidad.transform.position.y + 0.9f, 0f);
            posicionCursorHabilidad--;
            TextMeshPro descripcionHabilidad = fondoDescripcion.GetComponentInChildren<TextMeshPro>();

            descripcionHabilidad.text = habilidades[posicionCursorHabilidad].GetDescripcion() + "\n Coste: " + habilidades[posicionCursorHabilidad].GetCoste()+ " PS" + "\n Daño: " + habilidades[posicionCursorHabilidad].GetDano();
        }

        else if (Input.GetKeyUp(KeyCode.C))
        {
            GameManager.ReproducirSonido("Audio/Decision2");
            habilidadElegida = habilidades[posicionCursorHabilidad];

            CerrarMenuHabilidades();

            if(habilidadElegida.GetTipo()== (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Apoyo"))
            {
                GameManager.PosicionesPosiblesUsarHabilidadApoyo(atacante, habilidadElegida);
            }
            else
            {
                GameManager.PosicionesPosiblesUsarHabilidadDañina(atacante, habilidadElegida);
            }


            maquina.SetEstado(new EstadoElegirObjetivoHabilidad());
            reseteado = false;
            yield return new WaitForSeconds(0.01f);

        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            //GameManager.MoverUnidad(atacante, EstadoMover.GetPosicionOriginal());
            GameManager.BorrarCasillas();
            //GameManager.SoltarUnidad(atacante);
            CerrarMenuHabilidades();
            cursor.transform.position = atacante.transform.position;
            camara.transform.position = atacante.transform.position;
            atacante = null;
            //EstadoEsperar.SetUnidadSeleccionada(null);
            InvocarMenuAcciones();
            maquina.SetEstado(new EstadoElegirAccion());
            reseteado = false;
            yield return new WaitForSeconds(0.01f);

        }
    }

    public static Habilidad GetHabilidadElegida()
    {
        return habilidadElegida;
    }

    // Update is called once per frame
   

    private void InvocarMenuHabilidades()
    {
        cursorHabilidad.transform.position = new Vector3(atacante.transform.position.x + 1f, atacante.transform.position.y+2f, 0f);
        fondoDescripcion.transform.position = new Vector3(cursorHabilidad.transform.position.x - 3f, atacante.transform.position.y, 0f);

        for(int indice = 0; indice < habilidades.Count; indice++)
        {

            fondoHabilidades[indice].transform.position = new Vector3(cursorHabilidad.transform.position.x + 2.2f, cursorHabilidad.transform.position.y - (indice*0.9f), 0f);
            TextMeshPro textoNombreHabilidad = fondoHabilidades[indice].GetComponentInChildren<TextMeshPro>();
            
            textoNombreHabilidad.text = habilidades[indice].GetNombre();


        }

        TextMeshPro textoFondoDescripcion = fondoDescripcion.GetComponentInChildren<TextMeshPro>();
        textoFondoDescripcion.text = habilidades[0].GetDescripcion() + "\n Coste: " + habilidades[posicionCursorHabilidad].GetCoste() + " PS" + "\n Daño: " + habilidades[posicionCursorHabilidad].GetDano();
    }

    private void CerrarMenuHabilidades()
    {
        cursorHabilidad.transform.position = new Vector3(atacante.transform.position.x + 300.0f, atacante.transform.position.y, 0f);
        fondoDescripcion.transform.position = new Vector3(cursorHabilidad.transform.position.x + 300.0f, cursorHabilidad.transform.position.y - 0.9f, 0f);
        
        foreach(GameObject gob in fondoHabilidades)
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


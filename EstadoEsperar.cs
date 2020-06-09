using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class EstadoEsperar : Estado
{
    //En este estado, se espera a que el jugador elija una unidad que aun no haya gastado su turno. Si no queda ninguna y tiene unidades vivas, se pasa el turno al enemigo

    public static GameObject cursor;

    public GameObject camara;

    private static GameObject unidadSeleccionada;

    private List<GameObject> unidadesJugador = new List<GameObject>();

    private List<GameObject> enemigos;

    private GameObject unidad;

    private int contadorUnidadesPorUsar = 0;

    private MaquinaDeEstados maquina = IniciacionCombate.GetMaquinaDeEstados();

    private Tuple<bool, GameObject> sePuedeUsarUnidad;

    private static bool seHaReseteadoEstado = false;

    private Animator animator;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    // Propiedades para transiciones entre turnos y escenas

    private GameObject fadeIn;

    private GameObject fondoTurno;

    private GameObject simbolosSuperiores;

    private GameObject simbolosInferiores;

    private GameManager gm = GameManager.InstanciarGameManager();

    public EstadoEsperar()
    {

    }

    
    public override IEnumerator StartState()
    {
        fadeIn = GameObject.Find("FadeIn");
        camara = GameObject.Find("Main Camera");
        cursor = GameObject.Find("Cursor");
        
        fondoTurno = GameObject.Find("Turno-Enemigo");
        simbolosSuperiores = GameObject.Find("Simbolos-Turno-Arriba-E");
        simbolosInferiores = GameObject.Find("Simbolos-Turno-Abajo-E");

        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();


        if (seHaReseteadoEstado == false)
        {
            enemigos = GameObject.FindGameObjectsWithTag("Enemigo").ToList();
            unidadesJugador = GameObject.FindGameObjectsWithTag("Player").ToList();

                foreach(GameObject gob in unidadesJugador)
                {
                    if (gob.GetComponent<Unidad>().getTurnoAcabado() == false)
                        {
                            contadorUnidadesPorUsar++;
                        }
                }

                    seHaReseteadoEstado = true;

        }

        if(contadorUnidadesPorUsar == 0) //si ya has usado a todas tus unidades
        {

            if (unidadesJugador.Count != 0) //Alguna de tus unidades esta viva, pero las usaste a todas, asi que le toca al enemigo moverse
            {

                seHaReseteadoEstado = false;
                GameManager.BuffsYEstadosCambioDeTurno(enemigos);
                

                maquina.SetEstado(new EstadoTransicionTurnoEnemigo());
                yield break;


            }
            else if(unidadesJugador.Count == 0) //si has perdido a todas tus unidades, pierdes la partida
            {
                if (GameManager.GetSiCorrutinaHaAcabado())
                {
                    gm.MostrarMensajePerdidaTurno("No te quedan unidades vivas...", camara);
                    maquina.SetEstado(new EstadoDerrota());
                    yield break;
                }
                
            }

            seHaReseteadoEstado = false;
            yield break;
        }
        else
        {

            if (enemigos.Count == 0) //si has matado a todos tus enemigos y te quedan unidades vivas
            {
                if (GameManager.GetSiCorrutinaHaAcabado())
                {
                    maquina.SetEstado(new EstadoVictoria());
                }

            }
            else {

                GameManager.MoverElCursor();
                camara.transform.position = cursor.transform.position;
                //camara.transform.position = new Vector3(cursor.transform.position.x, cursor.transform.position.y, 0f);

                if (Input.GetKeyUp(KeyCode.C))

                {


                    sePuedeUsarUnidad = GameManager.ComprobarSiPuedeElegirUnidad(cursor.transform.position);


                    if (sePuedeUsarUnidad.Item1)//si existe la unidad y pertenece al jugador
                    {

                        unidad = sePuedeUsarUnidad.Item2;
                        GameManager.ReproducirSonido("Audio/Decision2");
                        GameManager.PosicionesPosiblesUnidad(unidad); //se buscan las posibles posiciones

                        //se permite al jugador mover su unidad en un nuevo estado
                        GameManager.ResaltarUnidad(unidad);
                        unidadSeleccionada = GameManager.SeleccionarUnidad(unidad.transform.position);
                        animator = unidadSeleccionada.GetComponent<Animator>();
                        unidadSeleccionada.GetComponent<Unidad>().SetEstaCaminando(true, animator);
                        maquina.SetEstado(new EstadoMover());
                        contadorUnidadesPorUsar = 0;
                        seHaReseteadoEstado = false;
                        yield break;

                    }

                }
                else if (Input.GetKeyUp(KeyCode.Z))
                {
                    foreach (GameObject unidad in unidadesJugador)
                    {
                        if (unidad.GetComponent<Unidad>().getTurnoAcabado() == false)
                        {
                            cursor.transform.position = unidad.transform.position;
                            break;
                        }
                    }
                }
                

            }


        }


    }

    
    public static GameObject GetUnidadSeleccionada()
        {

            return unidadSeleccionada;
        }

    public static void SetUnidadSeleccionada(GameObject gob)
    {
        unidadSeleccionada = gob;
    }

    public static void SetSeHaReseatoestado(bool resetear)
    {

        seHaReseteadoEstado = resetear;
    }

    
}

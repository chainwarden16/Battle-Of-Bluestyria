using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IniciacionCombate : MonoBehaviour

{
    private GameObject fad;

    private float p = 1f;
    private float tiempoEsp = 2f;

    private AudioSource musica;

    private static MaquinaDeEstados maquinaEstados;

    private static Estado estado;

    private bool haTerminadoElDegradado = false;

    private GameObject objetivo;

    private GameObject fondoTurno;

    private GameObject simbolosSuperiores;

    private GameObject simbolosInferiores;

    private GameObject fondoTurnoE;

    private GameObject simbolosSuperioresE;

    private GameObject simbolosInferioresE;

    private GameObject camara;

    private float tiempoAct = 0f;

    private float contadorOpacidad = 1f;

    private bool prepararAnimacion = true;

    private GameObject SFX;

    private static AudioClip cursorSFX;

    private List<GameObject> unidadesCPU;

    private static List<GameObject> unidadesJugador;

    private GameObject barraTeclas;

    private bool haParadoTodasLasCorrutinas = false;

    public void Start()
    {
        GameObject.FindGameObjectWithTag("Musica").GetComponent<MusicaGlobal>().PararMusica();
        SFX = GameObject.Find("SFX");
        musica = GameObject.Find("Música").GetComponent<AudioSource>();
        fad = GameObject.Find("FadeIn");

        objetivo = GameObject.Find("Objetivo");

        fondoTurno = GameObject.Find("Turno-Respectivo");

        simbolosSuperiores = GameObject.Find("Simbolos-Turno-Arriba");

        simbolosInferiores = GameObject.Find("Simbolos-Turno-Abajo");

        camara = GameObject.Find("Main Camera");

        maquinaEstados = gameObject.AddComponent<MaquinaDeEstados>();

        barraTeclas = GameObject.Find("Barra-Teclas");

        maquinaEstados.ComenzarCombate();

        StartCoroutine(MostrarBatalla());

    }

    public void Update()
    {
        //Se inicia el combate en estadoInicio
        if (haTerminadoElDegradado)
        {

            if (estado is EstadoInicio)
            {

                StartCoroutine(estado.StartState());

            }

            else if (estado is EstadoEsperar)
            {


                StartCoroutine(estado.StartState());

            }
            else if (estado is EstadoMover)
            {

                StartCoroutine(estado.StartState());

            }
            else if (estado is EstadoElegirAccion)
            {

                StartCoroutine(estado.StartState());

            }
            else if (estado is EstadoElegirConsumible)
            {
                StartCoroutine(estado.StartState());
            }


            else if (estado is EstadoUsarConsumible)
            {
                StartCoroutine(estado.StartState());
            }

            else if (estado is EstadoAtacar)
            {
                StartCoroutine(estado.StartState());

            }
            else if (estado is EstadoDefender)
            {
                StartCoroutine(estado.StartState());

            }
            else if (estado is EstadoHabilidad)
            {
                StartCoroutine(estado.StartState());

            }
            else if (estado is EstadoElegirObjetivoHabilidad)
            {

                StartCoroutine(estado.StartState());

            }
            else if (estado is EstadoPerdidaTurno)
            {
                StartCoroutine(estado.StartState());
            }
            else if (estado is EstadoAccionNoPermitida)
            {
                StartCoroutine(estado.StartState());
            }
            else if (estado is EstadoTurnoEnemigo)
            {
                //Comienza el estado del turno enemigo.

                //Se busca a todos los enemigos vivos y se ordenan. Los clérigos se moverán primero; luego, los lanceros; después, los magos; por último, los arqueros

                unidadesCPU = GameObject.FindGameObjectsWithTag("Enemigo").ToList();

                unidadesCPU.Sort(EstadoTurnoEnemigo.CompararaPrioridadCPU);

                int contadorEnemigos = EstadoTurnoEnemigo.GetContadorUnidades(); //contamos cuántas unidades quedan por mover

                if (contadorEnemigos == unidadesCPU.Count) //si es igual al total de enemigos, ya se han movido todos
                {
                    unidadesJugador = GameObject.FindGameObjectsWithTag("Player").ToList();
                    foreach (GameObject gameO in unidadesCPU)
                    {
                        Color colorJugador = new Color(1, 1, 1, 1);

                        gameO.GetComponent<SpriteRenderer>().color = colorJugador;

                    }

                    GameManager.BuffsYEstadosCambioDeTurno(unidadesJugador); //pasamos el turno al jugador tras comprobar los estados alterados y buffs actuales
                    maquinaEstados.SetEstado(new EstadoTransicionTurnoJugador());
                    EstadoTurnoEnemigo.SetContadorUnidades(0); //se coloca a 0 para que el ciclo pueda comenzar de nuevo en el siguiente turno enemigo
                }
                else
                {
                    EstadoTurnoEnemigo.ActivarIA(unidadesCPU[contadorEnemigos]); //-1 porque si no,
                }

            }
            else if (estado is EstadoVictoria)
            {
                if (haParadoTodasLasCorrutinas == false)
                {
                    StopAllCoroutines();
                    haParadoTodasLasCorrutinas = true;
                }

                StartCoroutine(estado.StartState());
            }
            else if (estado is EstadoDerrota)
            {
                StartCoroutine(estado.StartState());
            }
            else if (estado is EstadoTransicionTurnoEnemigo)
            {
                IniciarTransicionEnemigo();

            }
            else if (estado is EstadoTransicionTurnoJugador)
            {
                IniciarTransicionJugador();
            }

        }

    }

    private void IniciarTransicionJugador()
    {

        if (prepararAnimacion == true)
        {
            cursorSFX = Resources.Load<AudioClip>("Audio/Flash2");
            SFX.GetComponent<AudioSource>().PlayOneShot(cursorSFX, 1);
            fondoTurno = GameObject.Find("Turno-Respectivo");
            simbolosSuperiores = GameObject.Find("Simbolos-Turno-Arriba");
            simbolosInferiores = GameObject.Find("Simbolos-Turno-Abajo");
            camara = GameObject.Find("Main Camera");

            fondoTurno.transform.position = camara.transform.position;
            simbolosSuperiores.transform.position = new Vector3(camara.transform.position.x + 7.33f, camara.transform.position.y + 4.73f, 0f);
            simbolosInferiores.transform.position = new Vector3(camara.transform.position.x - 6.95f, camara.transform.position.y - 4.76f, 0f);
            prepararAnimacion = false;
        }

        if (tiempoAct < 1f)
        {
            Color actual = new Color(1, 1, 1, tiempoAct);
            fondoTurno.GetComponent<SpriteRenderer>().color = actual;
            simbolosInferiores.GetComponent<SpriteRenderer>().color = actual;
            simbolosSuperiores.GetComponent<SpriteRenderer>().color = actual;
            simbolosSuperiores.transform.Translate(-0.05f, 0, 0);
            simbolosInferiores.transform.Translate(0.05f, 0, 0);
            tiempoAct += Time.deltaTime;
        }

        if (tiempoAct >= 1f && tiempoAct < 2f)
        {
            simbolosSuperiores.transform.Translate(-0.05f, 0, 0);
            simbolosInferiores.transform.Translate(0.05f, 0, 0);
            tiempoAct += Time.deltaTime;
        }



        if (tiempoAct >= 2f && tiempoAct < 3f)
        {

            Color actual = new Color(1, 1, 1, contadorOpacidad);

            simbolosSuperiores.transform.Translate(-0.05f, 0, 0);
            simbolosInferiores.transform.Translate(0.05f, 0, 0);
            fondoTurno.GetComponent<SpriteRenderer>().color = actual;
            simbolosInferiores.GetComponent<SpriteRenderer>().color = actual;
            simbolosSuperiores.GetComponent<SpriteRenderer>().color = actual;
            tiempoAct += Time.deltaTime;
            contadorOpacidad -= Time.deltaTime;
        }


        if (tiempoAct >= 3f)
        {
            fondoTurno.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            simbolosInferiores.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            simbolosSuperiores.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            maquinaEstados.SetEstado(new EstadoEsperar());
            tiempoAct = 0f;
            contadorOpacidad = 1f;
            prepararAnimacion = true;
            barraTeclas.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }



    }

    private void IniciarTransicionEnemigo()
    {
        if (prepararAnimacion == true)
        {
            cursorSFX = Resources.Load<AudioClip>("Audio/Stare");
            SFX.GetComponent<AudioSource>().PlayOneShot(cursorSFX, 1);
            fondoTurnoE = GameObject.Find("Turno-Enemigo");
            simbolosSuperioresE = GameObject.Find("Simbolos-Turno-Arriba-E");
            simbolosInferioresE = GameObject.Find("Simbolos-Turno-Abajo-E");
            camara = GameObject.Find("Main Camera");

            fondoTurnoE.transform.position = camara.transform.position;
            simbolosSuperioresE.transform.position = new Vector3(camara.transform.position.x + 7.33f, camara.transform.position.y + 4.73f, 0f);
            simbolosInferioresE.transform.position = new Vector3(camara.transform.position.x - 6.95f, camara.transform.position.y - 4.76f, 0f);
            prepararAnimacion = false;
            barraTeclas.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        }

        if (tiempoAct < 1f)
        {
            Color actual = new Color(1, 1, 1, tiempoAct);
            fondoTurnoE.GetComponent<SpriteRenderer>().color = actual;
            simbolosInferioresE.GetComponent<SpriteRenderer>().color = actual;
            simbolosSuperioresE.GetComponent<SpriteRenderer>().color = actual;
            simbolosSuperioresE.transform.Translate(-0.05f, 0, 0);
            simbolosInferioresE.transform.Translate(0.05f, 0, 0);
            tiempoAct += Time.deltaTime;
        }

        if (tiempoAct >= 1f && tiempoAct < 2f)
        {
            simbolosSuperioresE.transform.Translate(-0.05f, 0, 0);
            simbolosInferioresE.transform.Translate(0.05f, 0, 0);
            tiempoAct += Time.deltaTime;
        }

        if (tiempoAct >= 2f && tiempoAct < 3f)
        {

            Color actual = new Color(1, 1, 1, contadorOpacidad);

            simbolosSuperioresE.transform.Translate(-0.05f, 0, 0);
            simbolosInferioresE.transform.Translate(0.05f, 0, 0);
            fondoTurnoE.GetComponent<SpriteRenderer>().color = actual;
            simbolosInferioresE.GetComponent<SpriteRenderer>().color = actual;
            simbolosSuperioresE.GetComponent<SpriteRenderer>().color = actual;
            tiempoAct += Time.deltaTime;
            contadorOpacidad -= Time.deltaTime;
        }


        if (tiempoAct >= 3f)
        {
            fondoTurnoE.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            simbolosInferioresE.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            simbolosSuperioresE.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            maquinaEstados.SetEstado(new EstadoTurnoEnemigo());
            tiempoAct = 0f;
            contadorOpacidad = 1f;
            prepararAnimacion = true;
        }


    }

    public static MaquinaDeEstados GetMaquinaDeEstados()
    {

        return maquinaEstados;
    }

    public static void SetEstado(Estado estadoNuevo)
    {
        estado = estadoNuevo;

    }

    private IEnumerator MostrarBatalla()
    {

        Color colorFadeOut = new Color(0, 0, 0, 0);
        while (p > 0f)

        {

            Color actual = fad.GetComponent<SpriteRenderer>().color;
            actual.a = p;
            fad.GetComponent<SpriteRenderer>().color = actual;

            p -= Time.deltaTime / tiempoEsp;
            yield return new WaitForSeconds(0.01f);

        }
        colorFadeOut.a = 0;
        fad.GetComponent<SpriteRenderer>().color = colorFadeOut;
        p = 1f;

        yield return new WaitForSeconds(1f);

        musica.Play();

        while (p > 0f)

        {

            Color actual = objetivo.GetComponent<SpriteRenderer>().color;
            actual.a = p;
            objetivo.GetComponent<SpriteRenderer>().color = actual;

            p -= Time.deltaTime / tiempoEsp;
            yield return new WaitForSeconds(0.01f);

        }

        p = 0f;

        objetivo.GetComponent<SpriteRenderer>().color = colorFadeOut;
        objetivo.transform.Translate(350, 350, 0);



        while (p < 1f)

        {
            Color actual = new Color(1, 1, 1, p);
            fondoTurno.GetComponent<SpriteRenderer>().color = actual;
            simbolosInferiores.GetComponent<SpriteRenderer>().color = actual;
            simbolosSuperiores.GetComponent<SpriteRenderer>().color = actual;
            simbolosSuperiores.transform.Translate(-0.05f, 0, 0);
            simbolosInferiores.transform.Translate(0.05f, 0, 0);

            p += Time.deltaTime / 0.5f;
            yield return new WaitForSeconds(0.01f);

        }

        p = 1f;

        while (p > 0f)
        {

            simbolosSuperiores.transform.Translate(-0.05f, 0, 0);
            simbolosInferiores.transform.Translate(0.05f, 0, 0);
            p -= Time.deltaTime / tiempoEsp;
            yield return new WaitForSeconds(0.01f);

        }

        p = 1f;

        while (p > 0f)

        {
            Color actual = new Color(1, 1, 1, p);
            actual.a = p;
            simbolosSuperiores.transform.Translate(-0.05f, 0, 0);
            simbolosInferiores.transform.Translate(0.05f, 0, 0);
            fondoTurno.GetComponent<SpriteRenderer>().color = actual;
            simbolosInferiores.GetComponent<SpriteRenderer>().color = actual;
            simbolosSuperiores.GetComponent<SpriteRenderer>().color = actual;

            p -= Time.deltaTime / 0.5f;
            yield return new WaitForSeconds(0.01f);

        }

        p = 1f;
        fondoTurno.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        simbolosInferiores.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        simbolosSuperiores.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        barraTeclas.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        haTerminadoElDegradado = true;

    }

}
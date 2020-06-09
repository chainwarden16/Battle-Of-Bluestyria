using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{

    #region Propiedades


    //Clase Singleton que controla todo lo que ocurre en el juego. Aqui se instanciaran los heroes concretos, se cntrolara el estado del juego, el momento en el que alguien gane y el flujo del juego

    private static GameManager _instance;

    private GameObject HeroeMago;
    private GameObject HeroeArquero;
    private GameObject HeroeClerigo;
    private GameObject HeroeLancero;

    private GameObject EnemigoMago;
    private GameObject EnemigoLancero;
    private GameObject EnemigoClerigo;
    private GameObject EnemigoArquero;


    private static GameObject casilla;

    private static GameObject cursor;

    private static GameObject camara;

    private static Animator animator;

    private static MaquinaDeEstados maquinaDeEstados = IniciacionCombate.GetMaquinaDeEstados();

    private List<int> tropaJugador;

    private List<int> hordaEnemigo;

    private List<int> armasHordaEnemiga;

    private Consumible creadorConsumibles = new Consumible();

    private static List<GameObject> unidadesAMantenerControladas = new List<GameObject>();

    private static List<Vector3> listaCeldasMovimiento = new List<Vector3>();

    private static List<Vector3> listaCeldasAtacar = new List<Vector3>();

    private static List<Vector3> listaCeldasHabilidad = new List<Vector3>();

    private static int jugadorActualmenteActivo; // 0 = jugador; 1 = CPU

    private const int tamanoIndividuo = 8;

    private const float aumentoFitnessGenCorrecto = 100.0f;

    private const float mejorValorFitnessPosible = tamanoIndividuo * aumentoFitnessGenCorrecto;

    private int numeroActualGeneraciones = 0;

    private int numeroMaximoGeneraciones = 500;

    private static System.Random rng = new System.Random();

    private AlgoritmoCreacionHordas horda;

    private AlgoritmoCreacionHordas armasHorda;

    private static GameObject mensajePerdidaTurno;

    private static GameObject popUp;

    private static AudioSource audioSource;
    private static AudioClip cursorSFX;


    private static Animator animacionesAtaques;

    private static GameObject objetoAnimaciones;

    private static bool haTerminadoLaCorrutina;

    private GameObject fondoTurno;

    private GameObject simbolosSuperiores;

    private GameObject simbolosInferiores;

    // =====

    private static GameObject panelEstadisticas;

    private static GameObject panelNombre;

    private static GameObject panelArma;

    private static GameObject iconoArma;

    private static GameObject iconoEscudado;

    private static GameObject iconoEstado;

    private static GameObject iconoBuff;

    private static TextMeshPro ps;

    private static TextMeshPro fuer;

    private static TextMeshPro mag;

    private static TextMeshPro def;

    private static TextMeshPro defM;

    private static TextMeshPro agi;

    private static TextMeshPro fuerza;

    private static TextMeshPro agilidad;

    private static TextMeshPro magia;

    private static TextMeshPro psActualesMenu;

    private static TextMeshPro psMaximos;

    private static TextMeshPro defensa;

    private static TextMeshPro defensaM;

    private static TextMeshPro nombre;

    private static TextMeshPro arma;




    #endregion


    #region GameManager Singleton

    public static GameManager InstanciarGameManager()
    {

        if (_instance == null)
        {

            GameObject gm = new GameObject("Game Manager");
            gm.AddComponent<GameManager>();

        }

        return _instance;

    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    #region Preparar Batalla

    public void InicializarCombate()
    {
        popUp = GameObject.Find("NumeroPopUp");
        objetoAnimaciones = GameObject.Find("AnimacionesCombate");
        animacionesAtaques = objetoAnimaciones.GetComponent<Animator>();
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        camara = GameObject.Find("Main Camera");
        objetoAnimaciones = GameObject.Find("AnimacionesCombate");
        tropaJugador = TropaEscogida.GetTropa();

        //Aquí irían las excepciones, pero estas son costosas y detienen la ejecuión del juego, así que las trataré de otra forma

        if (tropaJugador.Count == 0) //si no hubiera heroes en la lista, se rellena con valores aleatorios controlados
        {
            int conta = 0;
            while(conta < 4){
                tropaJugador.Add(rng.Next(1, 5));
                conta++;
            }
        }

        else if(tropaJugador.Count > 4) //si hay más de cuatro, se recorta hasta que sólo queden cuatro héroes
        {
            tropaJugador.GetRange(0, 4);
        }

        for (int numero = 0; numero< tropaJugador.Count; numero++) //si el héroe tiene un valor distinto del esperado, se vuelve a introducir dentro de los valores controlados
        {
            if(tropaJugador[numero] < 0 || tropaJugador[numero] > 4)
            {
                tropaJugador[numero] = rng.Next(1, 5);
            }
        }

        

        List<int> armJug = TropaEscogida.GetArmasJugador();

        for (int num=0; num < armJug.Count; num++) //se comprueba que las armas sean o las ofensivas o las defensivas
        {
            if (armJug[num] < 0 && armJug[num] > 1)
            {
                armJug[num] = rng.Next(0, 2);
            }
        }

        horda = new AlgoritmoCreacionHordas(rng, tropaJugador, null);

        while (horda.GetFitnessMejorSolucion() < mejorValorFitnessPosible && numeroActualGeneraciones <= numeroMaximoGeneraciones)
        {
            horda.crearGeneracionSiguiente();
            numeroActualGeneraciones++;

        }


        TropaEscogida.setHordaEnemiga(horda.GetGenesMejorSolucion());


        HeroeArquero = GameObject.Find("HeroeArquero");
        HeroeLancero = GameObject.Find("HeroeLancero");
        HeroeMago = GameObject.Find("HeroeMago");
        HeroeClerigo = GameObject.Find("HeroeClerigo");

        EnemigoClerigo = GameObject.Find("EnemigoClerigo");
        EnemigoArquero = GameObject.Find("EnemigoArquero");
        EnemigoMago = GameObject.Find("EnemigoMago");
        EnemigoLancero = GameObject.Find("EnemigoLancero");

        cursor = GameObject.Find("Cursor");
        casilla = GameObject.Find("Casilla");

        //Ahora se va a crear la lista de armas para dar a los enemigos. Para ello, debe existir una lista de armas que tenga el jugador, que se selecciona en la escena anterior (después de elegir a los personajes que se quiera usar)

        hordaEnemigo = TropaEscogida.GetHordaEnemiga();

        numeroActualGeneraciones = 0;

        armasHorda = new AlgoritmoCreacionHordas(rng, tropaJugador, hordaEnemigo);

        while (armasHorda.GetFitnessMejorSolucion() < mejorValorFitnessPosible && numeroActualGeneraciones <= numeroMaximoGeneraciones)
        {
            armasHorda.crearGeneracionSiguiente();
            numeroActualGeneraciones++;

        }

        TropaEscogida.SetArmasEnemigas(armasHorda.GetGenesMejorSolucion());

        jugadorActualmenteActivo = 0; //el jugador empieza la partida

        unidadesAMantenerControladas = new List<GameObject>();

        ColocarUnidadesAliadas();
        ColocarUnidadesEnemigas();

        CrearConsumibles();

    }


    public void CrearConsumibles()
    {

        Consumible pocion = creadorConsumibles.CrearPocion();
        Consumible tonico = creadorConsumibles.CrearTonico();
        Consumible elixir = creadorConsumibles.CrearElixir();

        List<Consumible> consumibles = new List<Consumible>() { pocion, tonico, elixir };

        TropaEscogida.SetConsumibles(consumibles);

    }


    private void ColocarUnidadesAliadas()
    {

        double posSpawnX = -3.5;
        double posSpawnY = -6.5;

        Arma a;

        for (int indiceJugador = 0; indiceJugador < tropaJugador.Count; indiceJugador++)
        {
            if (tropaJugador[indiceJugador] == 1)//mago
            {
                GameObject mago = InstanciarUnidad("Prefabs/HeroeMago");
                MoverUnidad(mago, new Vector2((float)posSpawnX, (float)posSpawnY));
                unidadesAMantenerControladas.Add(mago);

                AsignacionPropiedadesClases.AsignarAtributosUnidadAliada(mago, 1);


                if (TropaEscogida.GetArmasJugador()[indiceJugador] == 0)
                {
                    a = Arma.CrearTomo();
                }
                else
                {
                    a = Arma.CrearCodex();
                }

                mago.GetComponent<Clase>().SetArma(a);

            }
            else if (tropaJugador[indiceJugador] == 2) //lancero
            {
                GameObject lancero = InstanciarUnidad("Prefabs/HeroeLancero");
                MoverUnidad(lancero, new Vector2((float)posSpawnX, (float)posSpawnY));
                unidadesAMantenerControladas.Add(lancero);
                AsignacionPropiedadesClases.AsignarAtributosUnidadAliada(lancero, 2);


                if (TropaEscogida.GetArmasJugador()[indiceJugador] == 0)
                {
                    a = Arma.CrearLanza();
                }
                else
                {
                    a = Arma.CrearPica();
                }

                lancero.GetComponent<Clase>().SetArma(a);


            }
            else if (tropaJugador[indiceJugador] == 3) //clerigo
            {
                GameObject clerigo = InstanciarUnidad("Prefabs/HeroeClerigo");
                MoverUnidad(clerigo, new Vector2((float)posSpawnX, (float)posSpawnY));
                unidadesAMantenerControladas.Add(clerigo);
                AsignacionPropiedadesClases.AsignarAtributosUnidadAliada(clerigo, 3);

                if (TropaEscogida.GetArmasJugador()[indiceJugador] == 0)
                {
                    a = Arma.CrearBaculo();
                }
                else
                {
                    a = Arma.CrearAngelus();
                }

                clerigo.GetComponent<Clase>().SetArma(a);
            }
            else if (tropaJugador[indiceJugador] == 4) //arquero
            {
                GameObject arquero = InstanciarUnidad("Prefabs/HeroeArquero");
                MoverUnidad(arquero, new Vector2((float)posSpawnX, (float)posSpawnY));
                unidadesAMantenerControladas.Add(arquero);
                AsignacionPropiedadesClases.AsignarAtributosUnidadAliada(arquero, 4);

                if (TropaEscogida.GetArmasJugador()[indiceJugador] == 0)
                {
                    a = Arma.CrearArco();
                }
                else
                {
                    a = Arma.CrearPlatiun();
                }

                arquero.GetComponent<Clase>().SetArma(a);
            }

            //Se mueve el punto de spawn de unidad. Se rellenan de arriba abajo y de izquierda a derecha

            if (posSpawnX == -3.5)
            {
                posSpawnX += 5.0;
            }
            else
            {
                posSpawnX = -3.5;
                posSpawnY += 4.0;
            }

        }

    }

    private void ColocarUnidadesEnemigas()
    {

        Arma a;

        float posSpawnX = -3.5f;
        float posSpawnY = 12.5f;

        for (int indiceJugador = 0; indiceJugador < TropaEscogida.GetHordaEnemiga().Count; indiceJugador++)
        {
            if (hordaEnemigo[indiceJugador] == 1)//mago
            {
                GameObject mago = InstanciarUnidad("Prefabs/EnemigoMago");
                MoverUnidad(mago, new Vector3(posSpawnX, posSpawnY));
                unidadesAMantenerControladas.Add(mago);
                AsignacionPropiedadesClases.AsignarAtributosUnidadEnemiga(mago, 1);
                if (TropaEscogida.GetArmasEnemigas()[indiceJugador] == 0)
                {
                    a = Arma.CrearTomo();
                }
                else
                {
                    a = Arma.CrearCodex();
                }

                mago.GetComponent<Clase>().SetArma(a);

            }
            else if (hordaEnemigo[indiceJugador] == 2) //lancero
            {
                GameObject lancero = InstanciarUnidad("Prefabs/EnemigoLancero");
                MoverUnidad(lancero, new Vector3(posSpawnX, posSpawnY));
                unidadesAMantenerControladas.Add(lancero);
                AsignacionPropiedadesClases.AsignarAtributosUnidadEnemiga(lancero, 2);

                if (TropaEscogida.GetArmasEnemigas()[indiceJugador] == 0)
                {
                    a = Arma.CrearLanza();
                }
                else
                {
                    a = Arma.CrearPica();
                }

                lancero.GetComponent<Clase>().SetArma(a);
            }
            else if (hordaEnemigo[indiceJugador] == 3) //clerigo
            {
                GameObject clerigo = InstanciarUnidad("Prefabs/EnemigoClerigo");
                MoverUnidad(clerigo, new Vector3(posSpawnX, posSpawnY));
                unidadesAMantenerControladas.Add(clerigo);
                AsignacionPropiedadesClases.AsignarAtributosUnidadEnemiga(clerigo, 3);
                if (TropaEscogida.GetArmasEnemigas()[indiceJugador] == 0)
                {
                    a = Arma.CrearBaculo();
                }
                else
                {
                    a = Arma.CrearAngelus();
                }

                clerigo.GetComponent<Clase>().SetArma(a);
            }
            else if (hordaEnemigo[indiceJugador] == 4) //arquero
            {
                GameObject arquero = InstanciarUnidad("Prefabs/EnemigoArquero");
                MoverUnidad(arquero, new Vector3(posSpawnX, posSpawnY));
                unidadesAMantenerControladas.Add(arquero);
                AsignacionPropiedadesClases.AsignarAtributosUnidadEnemiga(arquero, 4);
                if (TropaEscogida.GetArmasEnemigas()[indiceJugador] == 0)
                {
                    a = Arma.CrearArco();
                }
                else
                {
                    a = Arma.CrearPlatiun();
                }

                arquero.GetComponent<Clase>().SetArma(a);
            }

            //Se mueve el punto de spawn de unidad. Se rellenan de arriba abajo y de izquierda a derecha

            if (posSpawnX == -3.5f || posSpawnX == -1.5f || posSpawnX == 1.5)
            {
                posSpawnX += 2.0f;
            }

            else
            {
                posSpawnX = -3.5f;
                posSpawnY -= 2.0f;
            }

        }

    }

    #endregion

    #region Maniplar unidades


    public static void CambiarDireccionUnidad(GameObject unidadAMover)
    {
        animator = unidadAMover.GetComponent<Animator>();

        int diffX = (int)(cursor.transform.position.x - unidadAMover.transform.position.x);

        int diffY = (int)(cursor.transform.position.y - unidadAMover.transform.position.y);

        if (Mathf.Abs(diffY) >= Mathf.Abs(diffX)) //si la distancia del cursor en la vertical es mayor o igual a la horizontal...
        {

            if (diffY >= 0) //está mirando hacia arriba
            {
                unidadAMover.GetComponent<Unidad>().SetDireccionAMirar(2, animator);
            }
            else //está mirando hacia abajo
            {
                unidadAMover.GetComponent<Unidad>().SetDireccionAMirar(0, animator);
            }


        }
        else
        {
            if (diffX >= 0) //está mirando a la derecha
            {
                unidadAMover.GetComponent<Unidad>().SetDireccionAMirar(3, animator);
            }
            else //está mirando a la izquierda
            {
                unidadAMover.GetComponent<Unidad>().SetDireccionAMirar(1, animator);
            }
        }

    }

    public static void CambiarDireccionUnidadEnemiga(GameObject unidadEnemiga, GameObject unidadJugador)
    {
        animator = unidadEnemiga.GetComponent<Animator>();

        int diffX = (int)(unidadJugador.transform.position.x - unidadEnemiga.transform.position.x);

        int diffY = (int)(unidadJugador.transform.position.y - unidadEnemiga.transform.position.y);

        if (Mathf.Abs(diffY) >= Mathf.Abs(diffX)) //si la distancia del cursor en la vertical es mayor o igual a la horizontal...
        {

            if (diffY >= 0) //está mirando hacia arriba
            {
                unidadEnemiga.GetComponent<Unidad>().SetDireccionAMirar(2, animator);
            }
            else //está mirando hacia abajo
            {
                unidadEnemiga.GetComponent<Unidad>().SetDireccionAMirar(0, animator);
            }


        }
        else
        {
            if (diffX >= 0) //está mirando a la derecha
            {
                unidadEnemiga.GetComponent<Unidad>().SetDireccionAMirar(3, animator);
            }
            else //está mirando a la izquierda
            {
                unidadEnemiga.GetComponent<Unidad>().SetDireccionAMirar(1, animator);
            }
        }
    }

    public static GameObject SeleccionarUnidad(Vector3 coordCursor)
    {

        GameObject unidad = null;

        foreach (GameObject uni in unidadesAMantenerControladas)
        {
            if (uni.transform.position == coordCursor)
            {
                unidad = uni;

                break;
            }
        }

        return unidad;

    }

    public static void MoverUnidad(GameObject unidad, Vector3 dest)
    {

        //Se usará una interpolación lineal para hacer que la colocación de la unidad sea más suave en lugar de "teletransportarse" al punto
        unidad.transform.position = dest;

    }


    public static void ResaltarUnidad(GameObject unidad)
    {
        animator = unidad.GetComponent<Animator>();
        unidad.GetComponent<Unidad>().SetEstaCaminando(true, animator);
    }

    public static void SoltarUnidad(GameObject unidad)
    {
        animator = unidad.GetComponent<Animator>();
        unidad.GetComponent<Unidad>().SetEstaCaminando(false, animator);
    }

    public static void MatarUnidad(GameObject unidad)
    {

        float temporizadorMuerte = 2f;
        float deltaMuerte = 1f;
        //Cuando los puntos de vida de alguna unidad llegar a 0, este debe desaparecer del juego, pues no es posible revivir unidades muertas en este juego
        unidadesAMantenerControladas.Remove(unidad);
        unidad.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
        Color unColor = unidad.GetComponent<SpriteRenderer>().color;
        Color unColorOpacity = unidad.GetComponent<SpriteRenderer>().color;

        ReproducirSonido("Audio/Collapse1");

        unColorOpacity.a = 0f;

        while (deltaMuerte > 0f)
        {
            unidad.GetComponent<SpriteRenderer>().color = Color.Lerp(unColor, unColorOpacity, deltaMuerte);
            deltaMuerte -= Time.deltaTime / temporizadorMuerte;
        }



        unidad.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0);


        Destroy(unidad);

    }

    public static GameObject InstanciarUnidad(string h)

    {

        //GameObject unidad = Instantiate(h, new Vector3(0f, 0f, 0f), Quaternion.identity);
        GameObject unidad = Instantiate(Resources.Load(h)) as GameObject;

        return unidad;

    }



    public static Tuple<bool, GameObject> ComprobarSiPuedeElegirUnidad(Vector3 casilla)
    {
        bool condicion = false;

        GameObject unidad = null;

        foreach (GameObject go in unidadesAMantenerControladas)
        {
            if (go.transform.position == casilla)
            {
                unidad = go;
                break;
            }
        }

        if (unidad != null && unidad.tag == "Player" && unidad.GetComponent<Unidad>().getTurnoAcabado() == false)
        {
            condicion = true;
        }

        return new Tuple<bool, GameObject>(condicion, unidad);
    }

    #endregion

    #region Limitar Movimiento Unidades

    public static bool EsCasillaOcupada(Vector3 lugar, GameObject unidad)
    {
        //Si devuelve true -> La casilla esta ocupada por alguna unidad, ya sea aliada o enemiga. Una casilla no puede estar ocupada por dos unidades diferentes
        bool condicion = false;

        foreach (GameObject g in unidadesAMantenerControladas)
        {
            if (g.transform.position == lugar && g != unidad)
            {
                condicion = true; //esta casilla se encuentra ocupada
                break;
            }
        }

        return condicion;


    }

    public static bool HayObstaculoEnCasilla(Vector3 casilla)
    {

        bool hayObstaculo = false;

        if ((casilla.x == -8.5 && casilla.y == -6.5) || (casilla.x == -6.5 && casilla.y == -3.5) || (casilla.x == 3.5 && casilla.y == -1.5) || (casilla.x == 11.5 && casilla.y == 0.5) || (casilla.x == 2.5 && casilla.y == 9.5)) //arboles y pozo
        {
            hayObstaculo = true;
        }
        else if (casilla.x >= -10.5 && casilla.x <= -6.5 && casilla.y >= 6.5 && casilla.y <= 7.5) //primera parte de la casa verde
        {
            hayObstaculo = true;
        }
        else if (casilla.x <= -4.5 && casilla.y >= 8.5) // segunda parte de la casa verde
        {
            hayObstaculo = true;
        }
        else if (casilla.x >= 6.5 && casilla.y >= 8.5) // casa naranja
        {
            hayObstaculo = true;
        }
        else if (casilla.x >= -12.5 && casilla.x <= -10.5 && casilla.y >= -5.5 && casilla.y <= -2.5) // casa azul
        {
            hayObstaculo = true;
        }
        else if (casilla.x >= 5.5 && casilla.y >= -5.5 && casilla.y <= -2.5) // casa negra
        {
            hayObstaculo = true;
        }
        else if (casilla.x >= 9.5 && casilla.y == -6.5) // casa negra 2
        {
            hayObstaculo = true;
        }

        else if ((casilla.x == 0.5 || casilla.x == -2.5) && (casilla.y == 3.5 || casilla.y == 0.5)) // Esquinas del puente
        {
            hayObstaculo = true;
        }

        return hayObstaculo;

    }

    public static bool HayAguaRioEnCasilla(Vector3 casilla)
    {
        bool hayAgua = false;

        if ((casilla.y == 1.5 || casilla.y == 2.5) && (casilla.x >= 0.5 || casilla.x <= -2.5)) // rio
        {
            hayAgua = true;
        }

        return hayAgua;
    }

    public static bool HayEnemigoEnCelda(Vector3 lugar)
    {
        //Si devuelve true -> La casilla esta ocupada por alguna unidad enemiga.
        bool condicion = false;

        foreach (GameObject g in unidadesAMantenerControladas)
        {
            if (g.transform.position == lugar && g.tag == "Enemigo")
            {
                condicion = true; //esta casilla se encuentra ocupada
                break;
            }
        }

        return condicion;

    }

    public static bool HayAliadoEnCelda(Vector3 lugar)
    {
        //Si devuelve true -> La casilla esta ocupada por alguna unidad aliada.
        bool condicion = false;

        foreach (GameObject g in unidadesAMantenerControladas)
        {
            if (g.transform.position == lugar && g.tag == "Player")
            {
                condicion = true; //esta casilla se encuentra ocupada por un aliado
                break;
            }
        }

        return condicion;

    }

    public static bool EsCasillaFueraDeMapa(Vector3 lugar)
    {

        bool condicion = false;

        if (lugar.x < -12.5f || lugar.x > 11.5 || lugar.y < -6.5 || lugar.y > 13.5)
        {
            condicion = true;
        }

        return condicion;

    }

    public static bool EsCasillaAlOtroLadoDeOrilla(GameObject personaje, Vector3 lugar)
    {
        //Se procura que el personaje no pueda cruzar al otro lado del río si su agilidad le permite ir más allá del ancho del río. Se mira si se encuentra en la orilla opuesta y en la zona donde no
        //hay puente. Si devuelve falso, la unidad se podá colocar ahí

        bool fueraDeLimite = false;

        if (personaje.transform.position.y < 1.5 && lugar.y > 2.5 && (personaje.transform.position.x >= 0.5 || personaje.transform.position.x <= -2.5))
        {
            fueraDeLimite = true;

        }
        else if (personaje.transform.position.y < 2.5 && lugar.y > 1.5 && (personaje.transform.position.x >= 0.5 || personaje.transform.position.x <= -2.5))
        {
            fueraDeLimite = true;
        }

        return fueraDeLimite;
    }

    public static List<Vector3> PosicionesPosiblesUnidad(GameObject unidad)
    {

        List<Vector3> posiciones = new List<Vector3>();

        int agilidad = unidad.GetComponent<Clase>().GetAgilidad();

        int x = Mathf.FloorToInt(agilidad / 2);

        int y = Mathf.FloorToInt(agilidad / 2);

        int distan = Mathf.FloorToInt(agilidad / 2);

        for (int i = 0; i <= agilidad; i++)
        {
            for (int j = 0; j <= agilidad; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !EsCasillaOcupada(posibleMovimiento, unidad) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento) && !EsCasillaAlOtroLadoDeOrilla(unidad, posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {

                    GameObject celda = Instantiate(casilla, posibleMovimiento, Quaternion.identity);
                    celda.tag = "Casilla";
                    celda.GetComponent<SpriteRenderer>().color = new Color(0f, 0.3f, 1f);
                    listaCeldasMovimiento.Add(posibleMovimiento);
                    posiciones.Add(posibleMovimiento);

                }


            }
        }


        return posiciones;

    }

    public static List<Vector3> PosicionesPosiblesAtacarUnidad(GameObject unidad)
    {

        List<Vector3> posiciones = new List<Vector3>();

        int rangoArma = unidad.GetComponent<Clase>().GetArma().GetRango();

        int x = Mathf.FloorToInt(rangoArma / 2);

        int y = Mathf.FloorToInt(rangoArma / 2);

        int distan = Mathf.FloorToInt(rangoArma / 2);

        for (int i = 0; i <= rangoArma; i++)
        {
            for (int j = 0; j <= rangoArma; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !HayAliadoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {

                    GameObject celda = Instantiate(casilla, posibleMovimiento, Quaternion.identity);
                    celda.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                    listaCeldasAtacar.Add(posibleMovimiento);
                    celda.tag = "Casilla";

                }


            }
        }


        return posiciones;

    }

    public static List<Vector3> PosicionesPosiblesUsarHabilidadDañina(GameObject unidad, Habilidad hab)
    {

        List<Vector3> posiciones = new List<Vector3>();

        int rangoHab = hab.GetRango();

        int x = Mathf.FloorToInt(rangoHab / 2);

        int y = Mathf.FloorToInt(rangoHab / 2);

        int distan = Mathf.FloorToInt(rangoHab / 2);

        for (int i = 0; i <= rangoHab; i++)
        {
            for (int j = 0; j <= rangoHab; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !HayAliadoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {
                    posiciones.Add(posibleMovimiento);
                    GameObject celda = Instantiate(casilla, posibleMovimiento, Quaternion.identity);
                    celda.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                    listaCeldasHabilidad.Add(posibleMovimiento);
                    celda.tag = "Casilla";

                }


            }
        }


        return posiciones;

    }

    public static List<Vector3> PosicionesPosiblesUsarHabilidadApoyo(GameObject unidad, Habilidad hab)
    {

        List<Vector3> posiciones = new List<Vector3>();

        int rangoHab = hab.GetRango();

        int x = Mathf.FloorToInt(rangoHab / 2);

        int y = Mathf.FloorToInt(rangoHab / 2);

        int distan = Mathf.FloorToInt(rangoHab / 2);

        for (int i = 0; i <= rangoHab; i++)
        {
            for (int j = 0; j <= rangoHab; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !HayEnemigoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {
                    posiciones.Add(posibleMovimiento);
                    GameObject celda = Instantiate(casilla, posibleMovimiento, Quaternion.identity);
                    celda.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                    listaCeldasHabilidad.Add(posibleMovimiento);
                    celda.tag = "Casilla";

                }


            }
        }


        return posiciones;

    }

    public static List<Vector3> PosicionesPosiblesUsarConsumible(GameObject unidad, Consumible con)
    {

        List<Vector3> posiciones = new List<Vector3>();

        int rangoConsumible = con.GetRango();

        int x = Mathf.FloorToInt(rangoConsumible / 2);

        int y = Mathf.FloorToInt(rangoConsumible / 2);

        int distan = Mathf.FloorToInt(rangoConsumible / 2);

        for (int i = 0; i <= rangoConsumible; i++)
        {
            for (int j = 0; j <= rangoConsumible; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !HayEnemigoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {
                    posiciones.Add(posibleMovimiento);
                    GameObject celda = Instantiate(casilla, posibleMovimiento, Quaternion.identity);
                    celda.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                    listaCeldasHabilidad.Add(posibleMovimiento);
                    celda.tag = "Casilla";

                }


            }
        }


        return posiciones;

    }


    public static void SetListaCeldas()
    {
        listaCeldasMovimiento.Clear();
    }

    public static void SetListaCeldasAtacar()
    {
        listaCeldasAtacar.Clear();
    }

    public static void SetListaCeldasHabilidad()
    {
        listaCeldasHabilidad.Clear();
    }

    #endregion

    #region Operaciones de la IA

    public static List<Vector3> DeterminarPosiblesEnemigosAtacarCPU(GameObject unidad)
    {
        Clase clase = unidad.GetComponent<Clase>();

        Arma arma = clase.GetArma();

        List<Vector3> posicionesAtacar = new List<Vector3>();

        int rangoArma = arma.GetRango();

        int rangoTotal = (clase.GetAgilidad() + rangoArma);

        int x, y, distan;


        if (clase.GetTipoClase() != 2 && clase.GetTipoClase() != 3)
        {

            x = Mathf.FloorToInt(rangoTotal / 2) - 2;

            y = Mathf.FloorToInt(rangoTotal / 2) - 2;

            distan = Mathf.FloorToInt(rangoTotal / 2) - 2;

        }
        else
        {
            x = Mathf.FloorToInt(rangoTotal / 2) - 1;

            y = Mathf.FloorToInt(rangoTotal / 2) - 1;

            distan = Mathf.FloorToInt(rangoTotal / 2) - 1;
        }



        for (int i = 0; i <= rangoTotal; i++)
        {
            for (int j = 0; j <= rangoTotal; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !HayEnemigoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {

                    posicionesAtacar.Add(posibleMovimiento);

                }


            }
        }


        return posicionesAtacar;


    }

    public static List<Vector3> DeterminarPosiblesEnemigosHabilidadDañinaCPU(GameObject unidad, Habilidad hab)
    {
        Clase clase = unidad.GetComponent<Clase>();

        List<Vector3> posiciones = new List<Vector3>();

        int rangoHab = hab.GetRango();

        int rangoTotal = (rangoHab + clase.GetAgilidad());

        int x, y, distan;

        if (clase.GetTipoClase() == 4)
        {

            x = Mathf.FloorToInt(rangoTotal / 2) - 5;

            y = Mathf.FloorToInt(rangoTotal / 2) - 5;

            distan = Mathf.FloorToInt(rangoTotal / 2) - 5;

        }else if (clase.GetTipoClase() == 2 || clase.GetTipoClase() == 1)
        {
            x = Mathf.FloorToInt(rangoTotal / 2) - 3;

            y = Mathf.FloorToInt(rangoTotal / 2) - 3;

            distan = Mathf.FloorToInt(rangoTotal / 2) - 3;
        }
      
        else
        {
            x = Mathf.FloorToInt(rangoTotal / 2) - 2;

            y = Mathf.FloorToInt(rangoTotal / 2) - 2;

            distan = Mathf.FloorToInt(rangoTotal / 2) - 2;
        }

        for (int i = 0; i <= rangoTotal; i++)
        {
            for (int j = 0; j <= rangoTotal; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !HayEnemigoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {
                    posiciones.Add(posibleMovimiento);

                }


            }
        }


        return posiciones;


    }

    public static List<Vector3> DeterminarPosiblesEnemigosHabilidadApoyoCPU(GameObject unidad, Habilidad hab)
    {
        Clase clase = unidad.GetComponent<Clase>();

        List<Vector3> posiciones = new List<Vector3>();

        int rangoHab = hab.GetRango();

        int rangoTotal = (rangoHab + clase.GetAgilidad());

        int x, y, distan;


        x = Mathf.FloorToInt(rangoTotal / 2) - 2;

        y = Mathf.FloorToInt(rangoTotal / 2) - 2;

        distan = Mathf.FloorToInt(rangoTotal / 2) - 2;




        for (int i = 0; i <= rangoTotal; i++)
        {
            for (int j = 0; j <= rangoTotal; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !HayAliadoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {
                    posiciones.Add(posibleMovimiento);

                }


            }
        }


        return posiciones;


    }


    public static List<Vector3> PosicionesPosiblesAtacarUnidadCPU(GameObject unidad)
    {

        List<Vector3> posiciones = new List<Vector3>();

        Unidad unid = unidad.GetComponent<Unidad>();

        Clase clase = unidad.GetComponent<Clase>();

        Arma arma = unidad.GetComponent<Clase>().GetArma();

        int rangoArma = arma.GetRango();

        int x = Mathf.FloorToInt(rangoArma / 2);

        int y = Mathf.FloorToInt(rangoArma / 2);

        int distan = Mathf.FloorToInt(rangoArma / 2);

        for (int i = 0; i <= rangoArma; i++)
        {
            for (int j = 0; j <= rangoArma; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !HayEnemigoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {

                    posiciones.Add(posibleMovimiento);

                }


            }
        }


        return posiciones;

    }

    public static List<Vector3> PosicionesPosiblesUsarHabilidadDañinaCPU(GameObject unidad, Habilidad hab)
    {

        List<Vector3> posiciones = new List<Vector3>();

        int rangoHab = hab.GetRango();

        int x = Mathf.FloorToInt(rangoHab / 2) - 1;

        int y = Mathf.FloorToInt(rangoHab / 2) - 1;

        int distan = Mathf.FloorToInt(rangoHab / 2) - 1;

        for (int i = 0; i <= rangoHab; i++)
        {
            for (int j = 0; j <= rangoHab; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && !HayEnemigoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {
                    posiciones.Add(posibleMovimiento);


                }


            }
        }


        return posiciones;

    }

    public static List<Vector3> PosicionesPosiblesUsarHabilidadApoyoCPU(GameObject unidad, Habilidad hab)
    {

        List<Vector3> posiciones = new List<Vector3>();

        int rangoHab = hab.GetRango();

        int x = Mathf.FloorToInt(rangoHab / 2);

        int y = Mathf.FloorToInt(rangoHab / 2);

        int distan = Mathf.FloorToInt(rangoHab / 2);

        for (int i = 0; i <= rangoHab; i++)
        {
            for (int j = 0; j <= rangoHab; j++)
            {
                int dist = Mathf.Abs(x - i) + Mathf.Abs(y - j);

                Vector3 posibleMovimiento = new Vector3(unidad.transform.position.x + (x - i), unidad.transform.position.y + (y - j), 0f);

                if ((dist == 0 || dist <= distan) && !EsCasillaFueraDeMapa(posibleMovimiento) && HayEnemigoEnCelda(posibleMovimiento) && !HayObstaculoEnCasilla(posibleMovimiento) && !HayAguaRioEnCasilla(posibleMovimiento)) //si se encuentra dentro del mapa y no ocupada, puede ser introducida en la lista
                {
                    posiciones.Add(posibleMovimiento);


                }


            }
        }


        return posiciones;

    }



    #endregion

    #region Funciones para el combate

    public static void AtacarUnidad(GameObject atacante, GameObject atacado)
    {


        int dano = CalcularDañoAtaque(atacante, atacado);

        CausarDañoUnidad(atacado, dano);

    }

    public static int CalcularDañoAtaque(GameObject atacante, GameObject atacado)
    {

        int fuerza = atacante.GetComponent<Clase>().GetAtaque();
        int danioArma = atacante.GetComponent<Clase>().GetArma().GetAtaque();
        int defensa = atacado.GetComponent<Clase>().GetDefensa();
        int defensaArma = atacante.GetComponent<Clase>().GetArma().GetDefensas();

        bool tieneBuffDefensa = false;
        List<int> estados = atacante.GetComponent<Unidad>().GetBuffs();
        List<int> estadosAtacado = atacado.GetComponent<Unidad>().GetBuffs();
        bool tieneBuffAtaque = false;
        bool tieneNerfAtaque = false;
        bool tieneNerfDefensa = false;

        if (estados.Contains(1)) //1 es el buff de ataque y magia mas fuerte
        {
            tieneBuffAtaque = true;
        }

        else if (estados.Contains(3))
        {// 3 es el nerf de ataque

            tieneNerfAtaque = true;

        }


        if (estadosAtacado.Contains(0))
        { // 0 es el buff de def

            tieneBuffDefensa = true;

        }
        else if (estadosAtacado.Contains(2)) //2 es el nerf de defensas

        {
            tieneNerfDefensa = true;
        }


        int buffDefensa = (int)(defensa * 1.5) * System.Convert.ToInt32(tieneBuffDefensa);



        int buffAtaque = (int)(fuerza * 1.5) * System.Convert.ToInt32(tieneBuffAtaque);

        int bajaDefensa = (int)(defensa * 0.5) * System.Convert.ToInt32(tieneNerfDefensa);

        int bajoAtaque = (int)(fuerza * 0.5) * System.Convert.ToInt32(tieneNerfAtaque);



        int daño = fuerza + buffAtaque + danioArma + bajoAtaque - defensa - defensaArma - buffDefensa - bajaDefensa;

        return daño;

    }

    public static int CalcularDañoHabilidadMagica(GameObject atacante, GameObject atacado, Habilidad hab)
    {
        int magia = atacante.GetComponent<Clase>().GetMagia();

        int danioHabilidad = atacante.GetComponent<Clase>().GetHabilidades().Find(h => h == hab).GetDano();
        int defensaM = atacado.GetComponent<Clase>().GetDefensaM();

        bool tieneBuffDefensa = false;
        int defensaArma = atacante.GetComponent<Clase>().GetArma().GetDefensas();
        List<int> estados = atacante.GetComponent<Unidad>().GetBuffs();
        List<int> estadosAtacado = atacado.GetComponent<Unidad>().GetBuffs();
        bool tieneBuffAtaque = false;
        bool tieneNerfAtaque = false;
        bool tieneNerfDefensa = false;
        int dano = 0;

        if (estados.Contains(1)) //1 es el buff de ataque y magia mas fuerte
        {
            tieneBuffAtaque = true;
        }

        else if (estados.Contains(3))
        {// 3 es el nerf de ataque

            tieneNerfAtaque = true;

        }


        if (estadosAtacado.Contains(0))
        { // 0 es el buff de def

            tieneBuffDefensa = true;

        }
        else if (estadosAtacado.Contains(2)) //2 es el nerf de defensas

        {
            tieneNerfDefensa = true;
        }


        int buffDefensa = (int)(defensaM * 1.5) * System.Convert.ToInt32(tieneBuffDefensa);



        int buffAtaque = (int)(magia * 1.5) * System.Convert.ToInt32(tieneBuffAtaque);

        int bajaDefensa = (int)(defensaM * 0.5) * System.Convert.ToInt32(tieneNerfDefensa);

        int bajoAtaque = (int)(magia * 0.5) * System.Convert.ToInt32(tieneNerfAtaque);

        dano = magia + buffAtaque + danioHabilidad + bajoAtaque - defensaM - defensaArma - buffDefensa - bajaDefensa;

        return dano;
    }


    public static void AtacarUnidadHabilidadMagica(GameObject atacante, GameObject atacado, Habilidad hab)
    {

        int dano = CalcularDañoHabilidadMagica(atacante, atacado, hab);

        CausarDañoUnidad(atacado, dano);

    }

    public static int CalcularDañoHabilidadFisica(GameObject atacante, GameObject atacado, Habilidad hab)
    {
        int fuerza = atacante.GetComponent<Clase>().GetAtaque();

        int danioHabilidad = atacante.GetComponent<Clase>().GetHabilidades().Find(h => h == hab).GetDano();
        int defensa = atacado.GetComponent<Clase>().GetDefensa();
        int defensaArma = atacante.GetComponent<Clase>().GetArma().GetDefensas();
        List<int> estadosAtacado = atacado.GetComponent<Unidad>().GetBuffs();
        bool tieneBuffDefensa = false;
        List<int> estados = atacante.GetComponent<Unidad>().GetBuffs();
        bool tieneBuffAtaque = false;
        bool tieneNerfAtaque = false;
        bool tieneNerfDefensa = false;

        if (estados.Contains(1)) //1 es el buff de ataque y magia mas fuerte
        {
            tieneBuffAtaque = true;
        }

        else if (estados.Contains(3))
        {// 3 es el nerf de ataque

            tieneNerfAtaque = true;

        }


        if (estadosAtacado.Contains(0))
        { // 0 es el buff de def

            tieneBuffDefensa = true;

        }
        else if (estadosAtacado.Contains(2)) //2 es el nerf de defensas

        {
            tieneNerfDefensa = true;
        }


        int buffDefensa = (int)(defensa * 1.5) * System.Convert.ToInt32(tieneBuffDefensa);



        int buffAtaque = (int)(fuerza * 1.5) * System.Convert.ToInt32(tieneBuffAtaque);

        int bajaDefensa = (int)(defensa * 0.5) * System.Convert.ToInt32(tieneNerfDefensa);

        int bajoAtaque = (int)(fuerza * 0.5) * System.Convert.ToInt32(tieneNerfAtaque);

        int dano = fuerza + buffAtaque + danioHabilidad + bajoAtaque - defensa - defensaArma - buffDefensa - bajaDefensa;

        return dano;
    }

    public static void AtacarUnidadHabilidadFisica(GameObject atacante, GameObject atacado, Habilidad hab)
    {

        int dano = CalcularDañoHabilidadFisica(atacante, atacado, hab);

        CausarDañoUnidad(atacado, dano);
    }

    public static void CurarVidaUnidad(GameObject unidad, int cantidad)
    {


        int curacion = cantidad;

        int vidaActual = unidad.GetComponent<Clase>().GetPSAct();

        int vidaMaxima = unidad.GetComponent<Clase>().GetPSMax();

        CambiarColorPopUp(popUp.GetComponent<TextMeshPro>(), 1);
        popUp.transform.position = new Vector3(unidad.transform.position.x, unidad.transform.position.y + 1.0f, 0f);
        popUp.GetComponent<TextMeshPro>().text = curacion.ToString();
        _instance.InvocarPopUp(popUp, 0.1f);

        if ((curacion + vidaActual) > vidaMaxima)
        {
            unidad.GetComponent<Clase>().SetPSAct(vidaMaxima);
        }
        else
        {
            unidad.GetComponent<Clase>().SetPSAct(vidaActual + curacion);
        }

    }

    public static void EliminarEstadosDañinosUnidad(GameObject unidad)
    {

        List<int> estados = unidad.GetComponent<Unidad>().GetEstados();

        if (estados.Count != 0)
        {
            estados.Clear();
            unidad.GetComponent<Unidad>().SetEstados(estados);

        }

    }

    public static void CausarDañoUnidad(GameObject unidad, int daño)
    {





        List<int> estados;
        int vida = unidad.GetComponent<Clase>().GetPSAct();

        int dañoReal = Mathf.Max(1, daño);


        Unidad objetivo = unidad.GetComponent<Unidad>();

        if (objetivo.GetEstaSiendoEscudado().Item2 != null && !ReferenceEquals(objetivo.GetEstaSiendoEscudado().Item2, null))
        {

            GameObject objetivoRedireccionado = objetivo.GetEstaSiendoEscudado().Item2;

            int vidaOtroObjetivo = objetivoRedireccionado.GetComponent<Clase>().GetPSAct();
            int dañoMitigado = Mathf.Max(1, dañoReal / 2);



            estados = objetivoRedireccionado.GetComponent<Unidad>().GetEstados();

            CambiarColorPopUp(popUp.GetComponent<TextMeshPro>(), 0);
            popUp.transform.position = new Vector3(unidad.transform.position.x, unidad.transform.position.y + 1.0f, 0f);
            popUp.GetComponent<TextMeshPro>().text = dañoMitigado.ToString();
            _instance.InvocarPopUp(popUp, 0.1f);

            if (estados.Contains(3))
            {
                estados.Clear();
                objetivoRedireccionado.GetComponent<Unidad>().SetEstados(estados);

            }


            if (vidaOtroObjetivo >= dañoMitigado)
            {
                objetivoRedireccionado.GetComponent<Clase>().SetPSAct(vidaOtroObjetivo - dañoMitigado);

            }
            else
            {
                objetivoRedireccionado.GetComponent<Clase>().SetPSAct(0);
            }

            vida = objetivoRedireccionado.GetComponent<Clase>().GetPSAct();


            if (vida <= 0)
            {
                MatarUnidad(objetivoRedireccionado);
                objetivo.SetEstaSiendoEscudado(new Tuple<bool, GameObject>(false, null));

            }




        }
        else
        {
            CambiarColorPopUp(popUp.GetComponent<TextMeshPro>(), 0);
            popUp.transform.position = new Vector3(unidad.transform.position.x, unidad.transform.position.y + 1.0f, 0f);
            popUp.GetComponent<TextMeshPro>().text = dañoReal.ToString();
            _instance.InvocarPopUp(popUp, 0.1f);

            estados = unidad.GetComponent<Unidad>().GetEstados();

            if (estados.Contains(3))
            {
                estados.Clear();
                unidad.GetComponent<Unidad>().SetEstados(estados);

            }


            if (vida >= dañoReal)
            {
                unidad.GetComponent<Clase>().SetPSAct(vida - dañoReal);
            }
            else
            {
                unidad.GetComponent<Clase>().SetPSAct(0);
            }


            vida = unidad.GetComponent<Clase>().GetPSAct();


            if (vida <= 0)
            {
                MatarUnidad(unidad);

            }

        }



    }

    public static bool ComprobarInflingirCambioEstadoDañino(GameObject unidad)
    {

        bool esPosibleAccion = false;

        List<int> estados = unidad.GetComponent<Unidad>().GetEstados();

        if (estados.Count == 0) //envenenado, quemado, paralizado, dormido
        {
            esPosibleAccion = true;
        }

        return esPosibleAccion;

    }

    public static void InflingirEstadoDañino(GameObject unidad, int estado, int turnos)
    {
        List<int> estados = unidad.GetComponent<Unidad>().GetEstados();
        estados.Add(estado);
        unidad.GetComponent<Unidad>().SetEstados(estados);
        unidad.GetComponent<Unidad>().SetContadorDuracionEstado(turnos);
    }

    public static bool ComprobarInflingirBuff(GameObject unidad)
    {

        bool esPosibleAccion = false;

        List<int> estados = unidad.GetComponent<Unidad>().GetBuffs();

        if (estados.Count == 0) //buff defensa, buff ataque, nerf defensa, nerf ataque
        {
            esPosibleAccion = true;
        }

        return esPosibleAccion;

    }

    public static void InflingirBuff(GameObject unidad, int estado, int turnos)
    {
        List<int> estados = unidad.GetComponent<Unidad>().GetBuffs();
        estados.Add(estado);
        unidad.GetComponent<Unidad>().SetBuffs(estados);
        unidad.GetComponent<Unidad>().SetContadorDuracionBuff(turnos);
    }

    public static void BuffsYEstadosCambioDeTurno(List<GameObject> unidades)
    {
        foreach (GameObject jugador in unidades)
        {
            Animator ani = jugador.GetComponent<Animator>();

            Unidad estadisticasJugador = jugador.GetComponent<Unidad>();

            Clase claseJugador = jugador.GetComponent<Clase>();

            List<int> buffs = estadisticasJugador.GetBuffs();

            List<int> estados = estadisticasJugador.GetEstados();

            Color colorJugador = new Color(1, 1, 1, 1);

            colorJugador.a = 1f;

            //comprobacion de escudado de unidades

            if (estadisticasJugador.GetEstaSiendoEscudado().Item1)
            {
                estadisticasJugador.SetEstaSiendoEscudado(new Tuple<bool, GameObject>(false, null));
            }

            //comprobacion de buffs/debuffs

            if (estadisticasJugador.GetContadorDuracionBuff() > 0)
            {
                int duracion = estadisticasJugador.GetContadorDuracionBuff();
                duracion--;
                estadisticasJugador.SetContadorDuracionBuff(duracion);
            }
            else
            {
                buffs.Clear();
                estadisticasJugador.SetBuffs(buffs);
            }

            //comprobacion de estados

            if (estadisticasJugador.GetContadorDuracionEstado() > 0)
            {
                int duracion = estadisticasJugador.GetContadorDuracionEstado();
                int contadorDañoVeneno = estadisticasJugador.GetContadorVeneno();
                duracion--;
                estadisticasJugador.SetContadorDuracionEstado(duracion);

                if (estadisticasJugador.GetEstados().Contains(0)) //envenenamiento
                {

                    int vida = claseJugador.GetPSAct();
                    vida -= 3 + contadorDañoVeneno * 3;
                    contadorDañoVeneno++;
                    estadisticasJugador.SetContadorVeneno(contadorDañoVeneno);
                    if (vida <= 0)
                    {
                        claseJugador.SetPSAct(0);
                        MatarUnidad(jugador);
                    }
                    else
                    {

                        claseJugador.SetPSAct(vida);
                        estadisticasJugador.setTurnoAcabado(false);
                        estadisticasJugador.SetEstaDefendiendo(false);
                        estadisticasJugador.SetEstaCaminando(false, ani);
                        estadisticasJugador.GetComponent<SpriteRenderer>().color = colorJugador;

                    }



                }
                else if (estadisticasJugador.GetEstados().Contains(1))
                {  //quemadura

                    int vida = claseJugador.GetPSAct();
                    vida -= 3;
                    if (vida <= 0)
                    {
                        claseJugador.SetPSAct(0);
                        MatarUnidad(jugador);
                    }
                    else
                    {
                        claseJugador.SetPSAct(vida);
                        estadisticasJugador.setTurnoAcabado(false);
                        estadisticasJugador.SetEstaDefendiendo(false);
                        estadisticasJugador.SetEstaCaminando(false, ani);
                        estadisticasJugador.GetComponent<SpriteRenderer>().color = colorJugador;
                    }

                }
                else if (estadisticasJugador.GetEstados().Contains(2) || estadisticasJugador.GetEstados().Contains(3)) //paralisis & sueño
                {


                    //la unidad no se puede mover, asi que se toma como si su turno estuviera acabado
                    estadisticasJugador.setTurnoAcabado(true);
                    estadisticasJugador.SetEstaDefendiendo(false);
                    estadisticasJugador.SetEstaCaminando(false, ani);


                }

            }
            else //El contador del estado alterado ha llegado a 0, así que se debe eliminar de la lista
            {
                List<int> listaEstadosvacia = new List<int>() { };
                estadisticasJugador.SetContadorVeneno(0);
                estadisticasJugador.SetContadorDuracionEstado(0);
                estadisticasJugador.SetEstados(listaEstadosvacia);
                estadisticasJugador.setTurnoAcabado(false);
                estadisticasJugador.SetEstaDefendiendo(false);
                estadisticasJugador.SetEstaCaminando(false, ani);
                estadisticasJugador.GetComponent<SpriteRenderer>().color = colorJugador;
                if (EstadoTurnoEnemigo.GetJugadoresConestadosAlerados().Contains(jugador))
                {
                    List<GameObject> jugadores = EstadoTurnoEnemigo.GetJugadoresConestadosAlerados();

                    jugadores.Remove(jugador);

                    EstadoTurnoEnemigo.SetJugadoresConEstadosAlterados(jugadores);

                }
            }

            if (estadisticasJugador.GetEstados().Count == 0)
            {

                estadisticasJugador.setTurnoAcabado(false);
                estadisticasJugador.SetEstaDefendiendo(false);
                estadisticasJugador.SetEstaCaminando(false, ani);
                estadisticasJugador.GetComponent<SpriteRenderer>().color = colorJugador;
            }

        }

        CambioJugadorActivo();

    }


    #endregion


    #region Otras

    public static void MoverElCursor()
    {
        camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);

        if (Input.GetKeyUp("down") && cursor.transform.position.y > -6.5f)
        {
            cursor.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y - 1.0f);
        }

        if (Input.GetKeyUp("up") && cursor.transform.position.y < 13.5f)
        {
            cursor.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y + 1.0f);
        }

        if (Input.GetKeyUp("left") && cursor.transform.position.x > -12.5f)
        {
            cursor.transform.position = new Vector2(cursor.transform.position.x - 1.0f, cursor.transform.position.y);
        }

        if (Input.GetKeyUp("right") && cursor.transform.position.x < 11.5f)
        {
            cursor.transform.position = new Vector2(cursor.transform.position.x + 1.0f, cursor.transform.position.y);
        }

    }

    public static void CambioJugadorActivo()
    {
        if (jugadorActualmenteActivo == 0)
        {
            jugadorActualmenteActivo = 1;
        }
        else
        {
            jugadorActualmenteActivo = 0;
        }
    }

    public static int GetJugadorActivo()
    {
        return jugadorActualmenteActivo;
    }

    public static void BorrarCasillas()
    {

        GameObject[] posicionesABorrar = GameObject.FindGameObjectsWithTag("Casilla");
        foreach (GameObject gob in posicionesABorrar)
        {
            if (gob != null)
                GameObject.Destroy(gob);
        }
        SetListaCeldasAtacar();
        SetListaCeldas();
        SetListaCeldasHabilidad();
    }

    public static List<GameObject> GetUnidadesATenerControladas()
    {
        return unidadesAMantenerControladas;
    }

    public static List<Vector3> GetListaCeldasMovimiento()
    {

        return listaCeldasMovimiento;
    }

    public static List<Vector3> GetListaCeldasAtacar()
    {

        return listaCeldasAtacar;
    }

    public static List<Vector3> GetListaCeldasHabilidad()
    {

        return listaCeldasHabilidad;
    }

    public void MostrarMensajePerdidaTurno(string texto, GameObject unidad)
    {

        mensajePerdidaTurno = GameObject.Find("Mensaje-Excepcion");

        mensajePerdidaTurno.transform.position = new Vector3(unidad.transform.position.x, unidad.transform.position.y - 3.0f, 0f);

        TextMeshPro text = mensajePerdidaTurno.GetComponentInChildren<TextMeshPro>();

        text.text = texto;

    }

    public static void EliminarMensajePerdidaTurno()
    {
        mensajePerdidaTurno.transform.Translate(300f, 0f, 0f);
    }

    public static void CerrarInterfazUnidad()
    {

        Color colTrans = new Color(1f, 1f, 1f, 0f);

        panelEstadisticas = GameObject.Find("Estadisticas-Unidad");
        defensaM = GameObject.Find("Defensa-Magica-Cantidad").GetComponent<TextMeshPro>();
        defensa = GameObject.Find("Defensa-Cantidad").GetComponent<TextMeshPro>();
        fuerza = GameObject.Find("Fuerza-Cantidad").GetComponent<TextMeshPro>();
        magia = GameObject.Find("Magia-Cantidad").GetComponent<TextMeshPro>();
        psActualesMenu = GameObject.Find("PS-Cantidad-Actual").GetComponent<TextMeshPro>();
        psMaximos = GameObject.Find("PS-Cantidad-Total").GetComponent<TextMeshPro>();
        agilidad = GameObject.Find("Agilidad-Cantidad").GetComponent<TextMeshPro>();

        defM = GameObject.Find("Defensa-Magica").GetComponent<TextMeshPro>();
        def = GameObject.Find("Defensa").GetComponent<TextMeshPro>();
        fuer = GameObject.Find("Fuerza").GetComponent<TextMeshPro>();
        mag = GameObject.Find("Magia").GetComponent<TextMeshPro>();
        ps = GameObject.Find("PS").GetComponent<TextMeshPro>();
        agi = GameObject.Find("Agilidad").GetComponent<TextMeshPro>();

        panelNombre = GameObject.Find("Nombre-Unidad");
        panelArma = GameObject.Find("Arma-Unidad");
        nombre = panelNombre.GetComponentInChildren<TextMeshPro>();
        arma = panelArma.GetComponentInChildren<TextMeshPro>();
        iconoArma = GameObject.Find("Icono-Arma");
        iconoBuff = GameObject.Find("Buff");
        iconoEstado = GameObject.Find("Estado");
        iconoEscudado = GameObject.Find("Escudado");

        panelArma.GetComponent<SpriteRenderer>().color = colTrans;
        panelEstadisticas.GetComponent<SpriteRenderer>().color = colTrans;
        panelNombre.GetComponent<SpriteRenderer>().color = colTrans;

        defensa.color = colTrans;
        defensaM.color = colTrans;
        magia.color = colTrans;
        fuerza.color = colTrans;
        agilidad.color = colTrans;

        def.color = colTrans;
        defM.color = colTrans;
        mag.color = colTrans;
        fuer.color = colTrans;
        agi.color = colTrans;
        ps.color = colTrans;

        nombre.color = colTrans;
        arma.color = colTrans;
        psMaximos.color = colTrans;
        psActualesMenu.color = colTrans;
        iconoEstado.GetComponent<SpriteRenderer>().color = colTrans;
        iconoArma.GetComponent<SpriteRenderer>().color = colTrans;
        iconoBuff.GetComponent<SpriteRenderer>().color = colTrans;
        iconoEscudado.GetComponent<SpriteRenderer>().color = colTrans;

    }

    public static void TerminarTurnoUnidad(GameObject unidad)
    {


        unidad.GetComponent<Unidad>().setTurnoAcabado(true);

        Color colo = new Color(0.3f, 0.3f, 0.3f, 1);

        colo.a = 0.9f;

        unidad.GetComponent<SpriteRenderer>().color = colo;

        CerrarInterfazUnidad();

        maquinaDeEstados.SetEstado(new EstadoEsperar());

    }


    public void InvocarPopUp(GameObject popUp, float tiempo)
    {

        StartCoroutine(CorrutinaAparecerPopUp(popUp, tiempo));

    }

    public static bool GetSiCorrutinaHaAcabado()
    {
        return haTerminadoLaCorrutina;
    }

    private IEnumerator CorrutinaAparecerPopUp(GameObject popUp, float time)
    {
        haTerminadoLaCorrutina = false;

        float lerp = 0;

        TextMeshPro colorTexto = popUp.GetComponent<TextMeshPro>();

        Color32 color = colorTexto.color;

        Color32 colorEsquinaInferior = colorTexto.colorGradient.bottomRight;

        color.a = 255;

        colorEsquinaInferior.a = 255;

        colorTexto.color = color;

        colorTexto.colorGradient = new VertexGradient(colorTexto.colorGradient.topLeft, colorTexto.colorGradient.topRight, colorTexto.colorGradient.bottomLeft, colorEsquinaInferior);

        Vector3 origen = new Vector3(popUp.transform.position.x, popUp.transform.position.y, 0f);

        Vector3 destino = new Vector3(popUp.transform.position.x, popUp.transform.position.y + 0.5f, 0f);

        while (lerp < 1f)
        {

            popUp.transform.position = Vector3.Lerp(origen, destino, lerp);

            lerp += Time.deltaTime / time;
            yield return new WaitForSeconds(0.01f);

        }

        lerp = 0f;

        while (lerp < 1f)
        {

            popUp.transform.position = Vector3.Lerp(destino, origen, lerp);

            lerp += Time.deltaTime / time;
            yield return new WaitForSeconds(0.01f);

        }

        popUp.transform.position = origen;

        yield return null;
        haTerminadoLaCorrutina = true;
    }

    public static void EliminarPopUp(GameObject popUp)
    {
        Color32 color2 = popUp.GetComponent<TextMeshPro>().color;

        color2.a = 0;

        popUp.GetComponent<TextMeshPro>().color = color2;
    }


    public static void CambiarColorPopUp(TextMeshPro texto, int color)
    {
        Color32 colorArribaIzquierda;
        Color32 colorArribaDerecha;
        Color32 colorAbajoIzquierda;
        Color32 colorAbajoDerecha;

        if (color == 1) //cambia a verde
        {

            colorArribaIzquierda = new Color32(14, 255, 0, 255);
            colorArribaDerecha = new Color32(21, 231, 0, 255);
            colorAbajoIzquierda = new Color32(24, 197, 0, 255);
            colorAbajoDerecha = new Color32(12, 29, 0, 255);

        }
        else //cambia a rojo
        {

            colorArribaIzquierda = new Color32(255, 14, 0, 255);
            colorArribaDerecha = new Color32(231, 21, 0, 255);
            colorAbajoIzquierda = new Color32(197, 24, 0, 255);
            colorAbajoDerecha = new Color32(29, 12, 0, 255);

        }

        texto.colorGradient = new VertexGradient(colorArribaIzquierda, colorArribaDerecha, colorAbajoIzquierda, colorAbajoDerecha);


    }


    public void Esperar(float tiempo)
    {
        StartCoroutine(CorrutinaSegundos(tiempo));
    }

    private IEnumerator CorrutinaSegundos(float tiempo)
    {
        yield return new WaitForSecondsRealtime(tiempo);
    }

    public static void ReproducirSonido(string rutaArchivo)
    {

        cursorSFX = Resources.Load<AudioClip>(rutaArchivo);
        audioSource.PlayOneShot(cursorSFX, 1f);

    }

    public static void ReproducirAnimacion(int ID, GameObject objetivo)
    {

        objetoAnimaciones.transform.position = objetivo.transform.position;
        animacionesAtaques.SetInteger("ID", ID);

    }

    #endregion

    #region Funciones para inicializar valores en los test unitarios

    public static void SetCursor(GameObject curso)
    {
        cursor = curso;
    }

    public static void SetUnidadesAMantenerControladas(List<GameObject> lista)
    {
        unidadesAMantenerControladas = lista;
    }

    public static void SetAudioSource(AudioSource aud)
    {
        audioSource = aud;
    }

    public static void SetCelda(GameObject celda)
    {
        casilla = celda;
    }

    public static void SetAnimator(Animator ani)
    {
        animacionesAtaques = ani;
    }

    public static void SetMaquinaEstados(MaquinaDeEstados maq)
    {
        maquinaDeEstados = maq;
    }

    public static void SetObjetoAnimaciones(GameObject objec)
    {
        objetoAnimaciones = objec;
    }

    public static void SetAudioClipSFX(AudioClip audioC)
    {
        cursorSFX = audioC;
    }

    public static AudioClip GetAudioClipSFX()
    {
        return cursorSFX;
    }

    public static void SetPopUp(GameObject texto)
    {
        popUp = texto;
    }

    public static void SetGameManager(GameManager gm)
    {
        _instance = gm;
    }

    public void SetTropaJugador(List<int> tropa)
    {
        tropaJugador = tropa;
    }

    #endregion

}
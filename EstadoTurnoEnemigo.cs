using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class EstadoTurnoEnemigo : Estado
{

    private static GameObject unidadAuxiliar; //para calcular distancias y no se dedique a crear nuevos objetos cada vez que se le llame

    private static List<int> elegirEstadoAlAzar;

    private static List<GameObject> unidadesJugador;

    private static List<GameObject> unidadesCPU;

    private static GameObject objetivo;

    private static System.Random random = new System.Random();

    private static HashSet<GameObject> posiblesObjetivos = new HashSet<GameObject>();

    private static Dictionary<int, List<Vector3>> rangosPosicionesAccionesCPU = new Dictionary<int, List<Vector3>>();

    private static MaquinaDeEstados maquina = IniciacionCombate.GetMaquinaDeEstados();

    private static GameObject popUp;

    private static GameObject cursor;

    private static GameObject camara;

    private static GameManager gm = GameManager.InstanciarGameManager();

    private static Animator animacionesAtaques;

    private static GameObject objetoAnimaciones;

    private static AnimatorClipInfo[] info;

    private static Animator animator;

    private static bool sePuedePulsarTecla = true;

    private static bool primeraParteAnimacion = false;

    private static bool segundaParteAnimacion = false;

    private static int contadorUnidades;

    private static int estadoElegido;

    private static float tiempoPrimerClip;

    private static bool yaSeHaElegidoAccion = false;

    private static int prioridadAliadoAElegir;

    private static int prioridadAliadoElegido = default(int);

    private static int prioridadEnemigoAElegir;

    private static int prioridadEnemigoElegido = default(int);

    //== Control del tiempo pasado en la animacion

    private static float delta = 0f;


    //== Control de unidades del jugador que ya tienen un estado alterado

    private static List<GameObject> jugadoresConEstadosAlterados = new List<GameObject>() { };



    public EstadoTurnoEnemigo()
    {

    }

    public static void ActivarIA(GameObject gob)
    {
        //IA


        popUp = GameObject.Find("NumeroPopUp");
        cursor = GameObject.Find("Cursor");
        camara = GameObject.Find("Main Camera");
        if (animacionesAtaques == null)
        {

            objetoAnimaciones = GameObject.Find("AnimacionesCombate");
            animacionesAtaques = objetoAnimaciones.GetComponent<Animator>();
            animator = gob.GetComponent<Animator>();
        }


        unidadesCPU = GameObject.FindGameObjectsWithTag("Enemigo").ToList();
        unidadesJugador = GameObject.FindGameObjectsWithTag("Player").ToList();
        //Ordeno las unidades para que cada una ejecute sus acciones en un orden que pueda facilitarle un poco la victoria

        /*
         * Los clérigos se centrarán en curar y buffar sus compañeros, los lanceros protegerán a aquellas unidades que estén más bajas de vida, priorizando aquellas que estén peor. Los magos usarán sus habilidades cuando puedan y los arqueros procurarán usarlas sólo si es necesario, procurando atacar
         * con ataque físicos desde lo más lejos posible aprovechando su rango.
         * 
         * Se recorre cada unidad en la lista de unidades enemigas restantes 
         */



        cursor.transform.position = gob.transform.position;
        camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);


        Clase claseUnidad = gob.GetComponent<Clase>();

        Unidad unidadGob = gob.GetComponent<Unidad>();

        Vector3 posicionGob = gob.transform.position;


        if (unidadGob.getTurnoAcabado() == false)
        {

            if (claseUnidad.GetTipoClase() == 3) //clérigo
            {
                //Se guardan las posibles posiciones desde las que puede atacar

                if (sePuedePulsarTecla)
                {
                    rangosPosicionesAccionesCPU.Clear();
                    posiblesObjetivos.Clear();
                    sePuedePulsarTecla = false;
                    rangosPosicionesAccionesCPU.Add(-1, GameManager.DeterminarPosiblesEnemigosAtacarCPU(gob));


                    foreach (Habilidad hab in claseUnidad.GetHabilidades()) //se comprueban las posiciones de los aliados según la habilidad que conozca la unidad
                    {
                        if (hab.GetTipo() == TipoHabilidad.Apoyo)
                        {

                            rangosPosicionesAccionesCPU.Add(hab.GetID(), GameManager.DeterminarPosiblesEnemigosHabilidadApoyoCPU(gob, hab));
                        }
                        else
                        {
                            rangosPosicionesAccionesCPU.Add(hab.GetID(), GameManager.DeterminarPosiblesEnemigosHabilidadDañinaCPU(gob, hab));
                        }

                    }

                    foreach (GameObject unidadAliada in unidadesCPU) //se recogen las unidades aliadas dentro del rango de habilidades de apoyo, pero se guardan en un HashSet. Es posible que una unidad tenga pocos PS y un estado alterado
                    {

                        if (rangosPosicionesAccionesCPU[7].Contains(unidadAliada.transform.position) && claseUnidad.GetPSAct() <= claseUnidad.GetPSMax() * 0.7)
                        {

                            posiblesObjetivos.Add(unidadAliada);

                        }
                        if ((rangosPosicionesAccionesCPU[8].Contains(unidadAliada.transform.position) || rangosPosicionesAccionesCPU[9].Contains(unidadAliada.transform.position)) && unidadAliada.GetComponent<Unidad>().GetBuffs().Count == 0)
                        {

                            posiblesObjetivos.Add(unidadAliada);
                        }

                        if (rangosPosicionesAccionesCPU[16].Contains(unidadAliada.transform.position) && unidadAliada.GetComponent<Unidad>().GetEstados().Count != 0)
                        {

                            posiblesObjetivos.Add(unidadAliada);
                        }

                    }

                    foreach (GameObject unidad in posiblesObjetivos.ToArray()) //ahora, se decide qué unidad aliada requiere ayuda con más urgencia. 
                                                                               //Se priorizan los PS bajos, luego los cambios de estado y por último el aplicar un buff según la clase de la unidad.
                    {


                        Unidad uni = unidad.GetComponent<Unidad>();

                        Clase claseUnidadObjetivo = unidad.GetComponent<Clase>();

                        if (claseUnidadObjetivo.GetPSAct() <= claseUnidadObjetivo.GetPSMax() * 0.7)
                        {
                            if (objetivo != null)
                            {
                                if (objetivo.GetComponent<Clase>().GetPSAct() > claseUnidadObjetivo.GetPSAct())
                                {
                                    objetivo = unidad;
                                    prioridadAliadoAElegir = 10;
                                    prioridadAliadoElegido = 10;
                                }
                            }
                            else
                            {
                                objetivo = unidad;
                                prioridadAliadoAElegir = 10;
                                prioridadAliadoElegido = 10;
                            }
                        }
                        else if (uni.GetEstados().Count != 0)
                        {
                            prioridadAliadoAElegir = 7;

                            if (prioridadAliadoElegido <= prioridadAliadoAElegir)
                            {
                                objetivo = unidad;
                                prioridadAliadoElegido = prioridadAliadoAElegir;
                            }
                        }
                        else if (uni.GetBuffs().Count == 0)
                        {
                            prioridadAliadoAElegir = 3;

                            if (prioridadAliadoElegido <= prioridadAliadoAElegir)
                            {
                                objetivo = unidad;
                                prioridadAliadoElegido = prioridadAliadoAElegir;
                            }
                        }

                    }

                    //Se ha buscado las distintas unidades aliadas; estamos fuera del foreach. Primero, miramos si hay algun objetivo (enemigo)

                    if (posiblesObjetivos.Count() == 0)
                    {
                        posiblesObjetivos.Clear(); //se limpia el hashset porque ya no nos sirve tener en cuenta a los aliados
                        foreach (GameObject enemigo in unidadesJugador)
                        {
                            if ((rangosPosicionesAccionesCPU[10].Contains(enemigo.transform.position) || rangosPosicionesAccionesCPU[11].Contains(enemigo.transform.position)) && enemigo.GetComponent<Unidad>().GetBuffs().Count == 0)
                            {//si hay enemigos dentro de este rango que no tengan ya un debuff, se incluyen en la lsita
                                posiblesObjetivos.Add(enemigo);

                            }
                            if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position))
                            {
                                posiblesObjetivos.Add(enemigo);

                            }
                        }


                        foreach (GameObject enemigo in posiblesObjetivos)
                        {

                            Clase claEnemigo = enemigo.GetComponent<Clase>();

                            Unidad uniEnemigo = enemigo.GetComponent<Unidad>();

                            if ((claEnemigo.GetPSAct() < claEnemigo.GetPSMax() * 0.4 || claEnemigo.GetTipoClase() == 2 || claEnemigo.GetTipoClase() == 3) && uniEnemigo.GetBuffs().Count == 0) //si tiene poca vida, es un lancero o un clerigo se le debilita para que sea mas facil acabar con esta unidad
                            {
                                if (objetivo != null)
                                {
                                    if (objetivo.GetComponent<Clase>().GetPSAct() > claEnemigo.GetPSAct())
                                    {
                                        objetivo = enemigo;
                                        prioridadEnemigoAElegir = 10;
                                        prioridadEnemigoElegido = 10;
                                    }
                                }
                                else
                                {
                                    objetivo = enemigo;
                                    prioridadEnemigoAElegir = 10;
                                    prioridadEnemigoElegido = 10;
                                }


                            }
                            else if ((claEnemigo.GetTipoClase() == 1 || claEnemigo.GetTipoClase() == 4) && uniEnemigo.GetBuffs().Count == 0)
                            {

                                prioridadEnemigoAElegir = 7;
                                if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                {
                                    prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                    objetivo = enemigo;
                                }


                            }
                            
                            else
                            {
                                prioridadEnemigoAElegir = 3;
                                if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                {
                                    prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                    objetivo = enemigo;
                                }


                            }

                        }
                    }


                }


                if (prioridadEnemigoElegido != 0) //si hay enemigos a los que molestar, vemos qué hacer
                {


                    if (prioridadEnemigoElegido == 10)
                    {

                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 10).First();

                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 0);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, gob);

                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Debilitar") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            GameManager.InflingirBuff(objetivo, 2, 2);

                            RestaurarValoresTrasAccion(gob);



                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Down1");
                            GameManager.ReproducirAnimacion(10, objetivo);
                            //animacionesAtaques.SetInteger("ID", 8);
                            segundaParteAnimacion = true;
                        }

                    }
                    else if (prioridadEnemigoElegido == 7)
                    {

                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 11).First();
                            
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 0);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, gob);
                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Cansar") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            GameManager.InflingirBuff(objetivo, 3, 2);

                            RestaurarValoresTrasAccion(gob);



                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Down2");
                            GameManager.ReproducirAnimacion(11, objetivo);
                            //animacionesAtaques.SetInteger("ID", 8);
                            segundaParteAnimacion = true;
                        }



                    }
                    else if (prioridadEnemigoElegido == 3)
                    {


                        if (primeraParteAnimacion == false)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, null, 0);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            gobAnimator.SetBool("estaAtacando", true);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            GameManager.ReproducirSonido("Audio/Saint3");
                            GameManager.ReproducirAnimacion(22, objetivo);
                            GameManager.AtacarUnidad(gob, objetivo);
                            primeraParteAnimacion = true;
                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Ataque-Clerigo") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            segundaParteAnimacion = true;
                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);
                        }



                    }
                    else //si no los hay, se defiende
                    {

                        gob.GetComponent<Unidad>().SetEstaDefendiendo(true);
                        cursor.transform.position = gob.transform.position;
                        camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                        RestaurarValoresTrasAccion(gob);
                    }



                }

                //Si se entra en este else, es que hay aliados que requieren ayuda

                else if(prioridadAliadoElegido !=0)//si hay una unidad aliada a la que ayudar, se aplica el tratamiento adecuado
                {

                    if (prioridadAliadoElegido == 10) //curar a alguien
                    {

                        //=======

                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 7).First();

                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, gob);
                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            GameManager.CurarVidaUnidad(objetivo, random.Next(10, 20));
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Curar") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);

                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Recovery");
                            GameManager.ReproducirAnimacion(7, objetivo);
                            //animacionesAtaques.SetInteger("ID", 7);
                            segundaParteAnimacion = true;
                        }


                    }
                    else if (prioridadAliadoElegido == 7) //Purificar
                    {

                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 16).First();
                           
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, gob);
                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Purificar") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            GameManager.EliminarEstadosDañinosUnidad(objetivo);

                            RestaurarValoresTrasAccion(gob);



                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Heal2");
                            GameManager.ReproducirAnimacion(16, objetivo);
                            //animacionesAtaques.SetInteger("ID", 16);
                            segundaParteAnimacion = true;
                        }
                    }
                    else if (prioridadAliadoElegido == 3)
                    {

                        if (objetivo.GetComponent<Clase>().GetTipoClase() == 1 || objetivo.GetComponent<Clase>().GetTipoClase() == 4) //si la unidad necesita un buff y es un mago o un arquero, se duplican sus estadísticas ofensivas
                        {

                            if (primeraParteAnimacion == false)
                            {
                                Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 9).First();

                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                                primeraParteAnimacion = true; //para que se ejecute solo una vez
                                GameManager.ReproducirSonido("Audio/Magic1");
                                GameManager.ReproducirAnimacion(18, gob);
                            }
                            else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                            {
                                Animator gobAnimator = gob.GetComponent<Animator>();

                                animacionesAtaques.SetInteger("ID", -1);
                                gobAnimator.SetBool("estaAtacando", true);
                                delta = 1;

                            }

                            else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Reforzar") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                            {


                                GameManager.InflingirBuff(objetivo, 1, 2);

                                RestaurarValoresTrasAccion(gob);



                            } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                            else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                            {
                                GameManager.ReproducirSonido("Audio/Up4");
                                GameManager.ReproducirAnimacion(9, objetivo);
                                segundaParteAnimacion = true;
                            }


                        }
                        else //CLERIGO BUSCAME
                        {

                            if (primeraParteAnimacion == false)
                            {
                                Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 8).First();

                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                                primeraParteAnimacion = true; //para que se ejecute solo una vez
                                GameManager.ReproducirSonido("Audio/Magic1");
                                GameManager.ReproducirAnimacion(18, gob);
                            }
                            else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                            {
                                Animator gobAnimator = gob.GetComponent<Animator>();
                                animacionesAtaques.SetInteger("ID", -1);
                                gobAnimator.SetBool("estaAtacando", true);
                                delta = 1;

                            }

                            else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Fortalecer") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                            {


                                GameManager.InflingirBuff(objetivo, 0, 2);

                                RestaurarValoresTrasAccion(gob);



                            } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                            else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                            {
                                GameManager.ReproducirSonido("Audio/Up4");
                                GameManager.ReproducirAnimacion(8, objetivo);
                                //animacionesAtaques.SetInteger("ID", 8);
                                segundaParteAnimacion = true;
                            }


                        }
                    }
                    else
                    {

                        gob.GetComponent<Unidad>().SetEstaDefendiendo(true);
                        cursor.transform.position = gob.transform.position;
                        camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                        gob.GetComponent<Unidad>().setTurnoAcabado(true);

                        RestaurarValoresTrasAccion(gob);
                    }

                }
                else
                {
                    gob.GetComponent<Unidad>().SetEstaDefendiendo(true);
                    cursor.transform.position = gob.transform.position;
                    camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                    gob.GetComponent<Unidad>().setTurnoAcabado(true);

                    RestaurarValoresTrasAccion(gob);
                }



            }

            else if (claseUnidad.GetTipoClase() == 2) //lancero
            {
                //La mayor prioridad de un lancero es proteger a sus aliados, sobre todo si estos están heridos. Hay que tener en cuenta que una unidad podria recibir daño indirecto por quemaduras/veneno.
                //Si ese fuera el caso, se debería proteger a otra.
                //Tambien hace uso minimo de sus habilidades, solo en el caso en que se sepa que hay posibilidades de matar al oponente, para evitar un malgasto de PS.
                /*
                 * Luego, ataca fisicamente.
                 * 
                 * Finalmente, si no hay enemigos cerca, se defiende.
                 * 
                 */

                if (sePuedePulsarTecla)
                {
                    sePuedePulsarTecla = false;
                    rangosPosicionesAccionesCPU.Clear();
                    posiblesObjetivos.Clear();
                    rangosPosicionesAccionesCPU.Add(-1, GameManager.DeterminarPosiblesEnemigosAtacarCPU(gob));

                    foreach (Habilidad hab in claseUnidad.GetHabilidades()) //se comprueban las posiciones de los aliados según la habilidad que conozca la unidad
                    {
                        if (hab.GetID() == 17)
                        {
                            rangosPosicionesAccionesCPU.Add(hab.GetID(), GameManager.DeterminarPosiblesEnemigosHabilidadApoyoCPU(gob, hab));
                        }
                        else
                        {
                            rangosPosicionesAccionesCPU.Add(hab.GetID(), GameManager.DeterminarPosiblesEnemigosHabilidadDañinaCPU(gob, hab));
                        }

                    }


                    //se miran las unidades aliadas cercanas al lancero en cuestión que no sean un lancero

                    foreach (GameObject unidadAliada in unidadesCPU)
                    {
                        if (unidadAliada != gob)
                        {
                            if (rangosPosicionesAccionesCPU[17].Contains(unidadAliada.transform.position) && !unidadAliada.GetComponent<Unidad>().GetEstaSiendoEscudado().Item1 && 
                                unidadAliada.GetComponent<Clase>().GetTipoClase() != 2 && claseUnidad.GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 17).First().GetCoste())
                            { //si la unidad está dentro del rango de la habilidad, no tiene a alguien que la escude y no es un lancero

                                //ahora se verifica si hay enemigos cerca

                                foreach (GameObject gm in unidadesJugador)
                                {

                                    if (Mathf.Abs(gm.transform.position.x - unidadAliada.transform.position.x) <= 4 && Mathf.Abs(gm.transform.position.y - unidadAliada.transform.position.y) <= 4)
                                    {
                                        posiblesObjetivos.Add(unidadAliada);

                                    }

                                }



                            }
                        }
                    }


                    foreach (GameObject unidadAliada in posiblesObjetivos)
                    {

                        Clase claseUnidadAli = unidadAliada.GetComponent<Clase>();

                        Unidad uniUniAli = unidadAliada.GetComponent<Unidad>();

                        if (objetivo != null)
                        {
                            if (objetivo.GetComponent<Clase>().GetPSAct() > claseUnidadAli.GetPSAct())
                            {
                                objetivo = unidadAliada;
                                prioridadAliadoElegido = 10;
                            }
                        }
                        else
                        {
                            objetivo = unidadAliada;
                            prioridadAliadoElegido = 10;
                            estadoElegido = 15;
                            yaSeHaElegidoAccion = true;
                        }

                    }

                    //Ahora, se comprobará si posibles objetivos está vacío. Si lo está, es que no hay nadie que escudar, por lo que se buscarán enemigos a los que atacar

                    if (posiblesObjetivos.Count ==0)
                    {
                        posiblesObjetivos.Clear();
                        foreach (GameObject enemigo in unidadesJugador)
                        {
                            if (rangosPosicionesAccionesCPU[3].Contains(enemigo.transform.position) || rangosPosicionesAccionesCPU[4].Contains(enemigo.transform.position))
                            {
                                posiblesObjetivos.Add(enemigo);

                            }
                            if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position))
                            {
                                posiblesObjetivos.Add(enemigo);

                            }

                        }




                        foreach (GameObject enemigo in posiblesObjetivos)
                        {
                            if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position) && GameManager.CalcularDañoAtaque(gob, enemigo) >= enemigo.GetComponent<Clase>().GetPSAct())
                            {
                                if (primeraParteAnimacion == false)
                                {
                                    Animator gobAnimator = gob.GetComponent<Animator>();
                                    gobAnimator.SetBool("estaCaminando", true);
                                    gob.transform.position = CasillaAMayorDistancia(gob, enemigo, null, 0);
                                    cursor.transform.position = gob.transform.position;
                                    camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                    GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                    GameManager.ReproducirSonido("Audio/Blow3");
                                    GameManager.ReproducirAnimacion(20, enemigo);
                                    primeraParteAnimacion = true;
                                    yaSeHaElegidoAccion = true;
                                    GameManager.AtacarUnidad(gob, enemigo);
                                    estadoElegido = 12;
                                    objetivo = enemigo;
                                }

                                break;
                            }

                            else if (rangosPosicionesAccionesCPU[3].Contains(enemigo.transform.position) && GameManager.CalcularDañoHabilidadFisica(gob, enemigo, claseUnidad.GetHabilidades().Where(n => n.GetID() == 3).First()) >= enemigo.GetComponent<Clase>().GetPSAct() && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 3).First().GetCoste())
                            {


                                if (primeraParteAnimacion == false)
                                {

                                    Habilidad hab = claseUnidad.GetHabilidades().Where(n => n.GetID() == 3).First();
                                    gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - hab.GetCoste());
                                    Animator gobAnimator = gob.GetComponent<Animator>();
                                    gobAnimator.SetBool("estaCaminando", true);
                                    gob.transform.position = CasillaAMayorDistancia(gob, enemigo, hab, 0);
                                    cursor.transform.position = gob.transform.position;
                                    camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                    GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                    primeraParteAnimacion = true; //para que se ejecute solo una vez
                                    yaSeHaElegidoAccion = true;
                                    estadoElegido = 13;
                                    GameManager.ReproducirSonido("Audio/Skill1");
                                    GameManager.ReproducirAnimacion(19, gob);
                                    GameManager.AtacarUnidadHabilidadFisica(gob, enemigo, hab);

                                    objetivo = enemigo;
                                }


                                break;
                            }
                            else if (rangosPosicionesAccionesCPU[4].Contains(enemigo.transform.position) && GameManager.CalcularDañoHabilidadFisica(gob, enemigo, claseUnidad.GetHabilidades().Where(n => n.GetID() == 4).First()) >= enemigo.GetComponent<Clase>().GetPSAct() && claseUnidad.GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 4).First().GetCoste())
                            {
                                if (primeraParteAnimacion == false)
                                {

                                    Habilidad hab = claseUnidad.GetHabilidades().Where(n => n.GetID() == 4).First();
                                    gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - hab.GetCoste());
                                    Animator gobAnimator = gob.GetComponent<Animator>();
                                    gobAnimator.SetBool("estaCaminando", true);
                                    gob.transform.position = CasillaAMayorDistancia(gob, enemigo, hab, 0);
                                    cursor.transform.position = gob.transform.position;
                                    camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                    GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                    primeraParteAnimacion = true; //para que se ejecute solo una vez
                                    yaSeHaElegidoAccion = true;
                                    estadoElegido = 14;
                                    GameManager.ReproducirSonido("Audio/Skill1");
                                    GameManager.ReproducirAnimacion(19, gob);
                                    GameManager.AtacarUnidadHabilidadFisica(gob, enemigo, hab);
                                    objetivo = enemigo;
                                }


                                break;
                            }


                            else
                            { //si no se puede asegurar la ejecucion de una unidad, se ataca usando una habilidad o atacando con normalidad

                                if (rangosPosicionesAccionesCPU[3].Contains(enemigo.transform.position) && claseUnidad.GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 3).First().GetCoste())
                                {

                                    prioridadEnemigoAElegir = 10;
                                    if (objetivo != null)
                                    {
                                        if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                        {

                                            if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                            {
                                                prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                                objetivo = enemigo;

                                            }
                                            else if (objetivo == null)
                                            {
                                                objetivo = enemigo;
                                            }


                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }

                                    }

                                    else
                                    {
                                        prioridadEnemigoElegido = 10;
                                        objetivo = enemigo;
                                    }

                                }
                                else if (rangosPosicionesAccionesCPU[4].Contains(enemigo.transform.position) && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 4).First().GetCoste())
                                {

                                    prioridadEnemigoAElegir = 9;
                                    if (objetivo != null)
                                    {
                                        if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                        {

                                            if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                            {
                                                prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                                objetivo = enemigo;

                                            }
                                            else if (objetivo == null)
                                            {
                                                objetivo = enemigo;
                                            }


                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }


                                    }

                                    else
                                    {
                                        prioridadEnemigoElegido = 9;
                                        objetivo = enemigo;
                                    }

                                }

                                else if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position))
                                {

                                    prioridadEnemigoAElegir = 8;
                                    if (objetivo != null)
                                    {
                                        if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                        {

                                            if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                            {
                                                prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                                objetivo = enemigo;

                                            }
                                            else if (objetivo == null)
                                            {
                                                objetivo = enemigo;
                                            }


                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }


                                    }

                                    else
                                    {
                                        prioridadEnemigoElegido = 8;
                                        objetivo = enemigo;
                                    }

                                }

                                else //si no se puede atacar a ningun enemigo de ninguna forma, se defiende
                                {
                                    prioridadEnemigoAElegir = 7;

                                    if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                    {
                                        prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                    }
                                }
                            }

                        }

                    }


                }


                //Ahora se mira que accion contra el enemigo es la mas correcta, suponiendo que haya enemigos

                if (yaSeHaElegidoAccion == false)
                {

                    if (prioridadEnemigoElegido == 10)
                    {
                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 3).First();
                            gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - habi.GetCoste());
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Skill1");
                            GameManager.ReproducirAnimacion(19, gob);

                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Fisica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Estocada") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);

                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Damage1");
                            GameManager.ReproducirAnimacion(3, objetivo);
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 3).First();
                            GameManager.AtacarUnidadHabilidadFisica(gob, objetivo, habi);
                            //animacionesAtaques.SetInteger("ID", 7);
                            segundaParteAnimacion = true;
                        }
                    }
                    else if (prioridadEnemigoElegido == 9)
                    {

                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 4).First();
                            gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - habi.GetCoste());
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Skill1");
                            GameManager.ReproducirAnimacion(19, gob);

                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Fisica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Placaje") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);

                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;

                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Blow1");
                            GameManager.ReproducirAnimacion(4, objetivo);
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 4).First();
                            GameManager.AtacarUnidadHabilidadFisica(gob, objetivo, habi);
                            //animacionesAtaques.SetInteger("ID", 7);
                            segundaParteAnimacion = true;
                        }


                    }

                    else if (prioridadEnemigoElegido == 8)
                    {
                        if (primeraParteAnimacion == false)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, null, 0);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            primeraParteAnimacion = true;
                            gobAnimator.SetBool("estaAtacando", true);
                            GameManager.ReproducirSonido("Audio/Blow1");
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            GameManager.ReproducirAnimacion(21, objetivo);
                            GameManager.AtacarUnidad(gob, objetivo);
                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Ataque-Lancero") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            segundaParteAnimacion = true;
                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;


                        }

                    }
                    else
                    {
                        gob.GetComponent<Unidad>().SetEstaDefendiendo(true);
                        cursor.transform.position = gob.transform.position;
                        camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                        gob.GetComponent<Unidad>().setTurnoAcabado(true);

                        RestaurarValoresTrasAccion(gob);


                    }

                }
                else //animaciones de ataques y demas
                {


                    if (estadoElegido == 13)
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Fisica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Estocada") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        }

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {

                            GameManager.ReproducirSonido("Audio/Damage1");
                            GameManager.ReproducirAnimacion(3, objetivo);
                            segundaParteAnimacion = true;

                        }
                    }
                    else if (estadoElegido == 14)
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Fisica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            delta = 1;
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Placaje") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        }

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1 && delta == 1)
                        {


                            GameManager.ReproducirSonido("Audio/Blow2");
                            GameManager.ReproducirAnimacion(4, objetivo);
                            segundaParteAnimacion = true;


                        }
                    }
                    else if (estadoElegido == 15)
                    {
                        if (primeraParteAnimacion == false)
                        {
                            Habilidad hab = claseUnidad.GetHabilidades().Where(n => n.GetID() == 17).First();
                            gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - hab.GetCoste());
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, hab, -1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            objetivo.GetComponent<Unidad>().SetEstaSiendoEscudado(new Tuple<bool, GameObject>(true, gob));
                            gobAnimator.SetBool("estaAtacando", true);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, gob);
                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Escudar") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);



                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Saint2");
                            GameManager.ReproducirAnimacion(17, objetivo);
                            //animacionesAtaques.SetInteger("ID", 9);
                            segundaParteAnimacion = true;
                        }
                    }


                    else //está atacando sin usar magia
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Ataque-Lancero") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            segundaParteAnimacion = true;
                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;


                        }
                    }
                }



            }
            else if (claseUnidad.GetTipoClase() == 1) //mago
            {

                /*
                 * El mago se centrará en causar un estado alterado. Si no fuera posible, atacaria usando una habilidad. Y sie sto no fuera posible, atacaria de forma fisica.
                 * Si todo esto fallara, se defenderia.
                 * 
                 * El mago busca qué enemigo tiene menos Puntos de Salud dentro de su rango y lo prioriza
                 */
                if (sePuedePulsarTecla)
                {
                    sePuedePulsarTecla = false;
                    rangosPosicionesAccionesCPU.Clear();
                    posiblesObjetivos.Clear();

                    rangosPosicionesAccionesCPU.Add(-1, GameManager.DeterminarPosiblesEnemigosAtacarCPU(gob));

                    foreach (Habilidad hab in claseUnidad.GetHabilidades()) //se comprueban las posiciones de los enemigos según la habilidad que conozca la unidad
                    {
                        rangosPosicionesAccionesCPU.Add(hab.GetID(), GameManager.DeterminarPosiblesEnemigosHabilidadDañinaCPU(gob, hab));
                    }


                    foreach (GameObject enemigo in unidadesJugador)
                    {
                        if (rangosPosicionesAccionesCPU[0].Contains(enemigo.transform.position) || rangosPosicionesAccionesCPU[1].Contains(enemigo.transform.position) ||
                            rangosPosicionesAccionesCPU[2].Contains(enemigo.transform.position))
                        {
                            posiblesObjetivos.Add(enemigo);

                        }
                        if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position))
                        {
                            posiblesObjetivos.Add(enemigo);

                        }
                        if ((rangosPosicionesAccionesCPU[12].Contains(enemigo.transform.position) || rangosPosicionesAccionesCPU[13].Contains(enemigo.transform.position)
                           || rangosPosicionesAccionesCPU[14].Contains(enemigo.transform.position) || rangosPosicionesAccionesCPU[15].Contains(enemigo.transform.position))
                           && jugadoresConEstadosAlterados.Contains(enemigo)) //se comprueba que esté dentro del rango y que no tenga ya un estado alterado
                        {
                            posiblesObjetivos.Add(enemigo);
                        }
                    }

                    foreach (GameObject enemigo in posiblesObjetivos)
                    {
                        if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position) && GameManager.CalcularDañoAtaque(gob, enemigo) >= enemigo.GetComponent<Clase>().GetPSAct())
                        {
                            if (primeraParteAnimacion == false)
                            {
                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, enemigo, null, 0);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                gobAnimator.SetBool("estaAtacando", true);
                                GameManager.ReproducirSonido("Audio/Ice2");
                                GameManager.ReproducirAnimacion(20, enemigo);
                                primeraParteAnimacion = true;
                                yaSeHaElegidoAccion = true;
                                GameManager.AtacarUnidad(gob, enemigo);
                            }

                            break;
                        }

                        else if (rangosPosicionesAccionesCPU[12].Contains(enemigo.transform.position) && !jugadoresConEstadosAlterados.Contains(enemigo) && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 12).First().GetCoste())
                        {


                            if (primeraParteAnimacion == false)
                            {
                                elegirEstadoAlAzar = new List<int>() { 12, 12, 12, 13, 13, 13, 14, 14, 15, 15 };

                                estadoElegido = elegirEstadoAlAzar[random.Next(0, 10)];
                                Habilidad hab = claseUnidad.GetHabilidades().Where(n => n.GetID() == estadoElegido).First();
                                gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - hab.GetCoste());
                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, enemigo, hab, 0);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                primeraParteAnimacion = true; //para que se ejecute solo una vez
                                yaSeHaElegidoAccion = true;
                                jugadoresConEstadosAlterados.Add(enemigo);
                                GameManager.ReproducirSonido("Audio/Magic1");
                                GameManager.ReproducirAnimacion(18, gob);
                                InflingirEstadoEnemigo(gob, enemigo, claseUnidad, estadoElegido);
                                objetivo = enemigo;
                            }


                            break;
                        }
                        else if (rangosPosicionesAccionesCPU[14].Contains(enemigo.transform.position) && !jugadoresConEstadosAlterados.Contains(enemigo) && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 14).First().GetCoste())
                        {
                            if (primeraParteAnimacion == false)
                            {
                                elegirEstadoAlAzar = new List<int>() { 13, 13, 13, 13, 14, 14, 14, 15, 15, 15 };

                                estadoElegido = elegirEstadoAlAzar[random.Next(0, 10)];
                                Habilidad hab = claseUnidad.GetHabilidades().Where(n => n.GetID() == estadoElegido).First();
                                gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - hab.GetCoste());
                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, enemigo, hab, 0);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                primeraParteAnimacion = true; //para que se ejecute solo una vez
                                yaSeHaElegidoAccion = true;
                                jugadoresConEstadosAlterados.Add(enemigo);
                                GameManager.ReproducirSonido("Audio/Magic1");
                                GameManager.ReproducirAnimacion(18, gob);
                                InflingirEstadoEnemigo(gob, enemigo, claseUnidad, estadoElegido);
                                objetivo = enemigo;
                            }


                            break;
                        }
                        else if (rangosPosicionesAccionesCPU[13].Contains(enemigo.transform.position) && !jugadoresConEstadosAlterados.Contains(enemigo) && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 13).First().GetCoste())
                        {
                            if (primeraParteAnimacion == false)
                            {
                                elegirEstadoAlAzar = new List<int>() { 13, 13, 13, 13, 13, 15, 15, 15, 15, 15 };

                                estadoElegido = elegirEstadoAlAzar[random.Next(0, 10)];
                                Habilidad hab = claseUnidad.GetHabilidades().Where(n => n.GetID() == estadoElegido).First();
                                gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - hab.GetCoste());
                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, enemigo, hab, 0);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                primeraParteAnimacion = true; //para que se ejecute solo una vez
                                yaSeHaElegidoAccion = true;
                                jugadoresConEstadosAlterados.Add(enemigo);
                                GameManager.ReproducirSonido("Audio/Magic1");
                                GameManager.ReproducirAnimacion(18, gob);
                                InflingirEstadoEnemigo(gob, enemigo, claseUnidad, estadoElegido);
                                objetivo = enemigo;
                            }

                            break;
                        }
                        else if (rangosPosicionesAccionesCPU[15].Contains(enemigo.transform.position) && !jugadoresConEstadosAlterados.Contains(enemigo) && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 15).First().GetCoste())
                        {

                            if (primeraParteAnimacion == false)
                            {

                                Habilidad hab = claseUnidad.GetHabilidades().Where(n => n.GetID() == 15).First();
                                gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - hab.GetCoste());
                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, enemigo, hab, 0);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                primeraParteAnimacion = true; //para que se ejecute solo una vez
                                yaSeHaElegidoAccion = true;
                                jugadoresConEstadosAlterados.Add(enemigo);
                                GameManager.ReproducirSonido("Audio/Magic1");
                                GameManager.ReproducirAnimacion(18, gob);
                                InflingirEstadoEnemigo(gob, enemigo, claseUnidad, estadoElegido);
                                objetivo = enemigo;

                            }

                            break;
                        }


                        else
                        { //si no se puede aplicar un cambio de estado, se ataca usando una habilidad

                            if (rangosPosicionesAccionesCPU[0].Contains(enemigo.transform.position) && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 0).First().GetCoste())
                            {

                                prioridadEnemigoAElegir = 10;
                                if (objetivo != null)
                                {
                                    if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                    {

                                        if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                        {
                                            prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                            objetivo = enemigo;

                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }


                                    }
                                    else if (objetivo == null)
                                    {
                                        objetivo = enemigo;
                                    }

                                }

                                else
                                {
                                    prioridadEnemigoElegido = 10;
                                    objetivo = enemigo;
                                }

                            }
                            else if (rangosPosicionesAccionesCPU[2].Contains(enemigo.transform.position) && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 2).First().GetCoste())
                            {

                                prioridadEnemigoAElegir = 9;
                                if (objetivo != null)
                                {
                                    if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                    {

                                        if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                        {
                                            prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                            objetivo = enemigo;

                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }


                                    }
                                    else if (objetivo == null)
                                    {
                                        objetivo = enemigo;
                                    }


                                }

                                else
                                {
                                    prioridadEnemigoElegido = 9;
                                    objetivo = enemigo;
                                }

                            }
                            else if (rangosPosicionesAccionesCPU[1].Contains(enemigo.transform.position) && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 1).First().GetCoste())
                            {
                                prioridadEnemigoAElegir = 8;
                                if (objetivo != null)
                                {
                                    if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                    {

                                        if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                        {
                                            prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                            objetivo = enemigo;

                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }


                                    }
                                    else if (objetivo == null)
                                    {
                                        objetivo = enemigo;
                                    }


                                }

                                else
                                {
                                    prioridadEnemigoElegido = 8;
                                    objetivo = enemigo;
                                }

                            }
                            else if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position))
                            {

                                prioridadEnemigoAElegir = 7;
                                if (objetivo != null)
                                {
                                    if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                    {

                                        if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                        {
                                            prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                            objetivo = enemigo;

                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }


                                    }
                                    else if (objetivo == null)
                                    {
                                        objetivo = enemigo;
                                    }


                                }

                                else
                                {
                                    prioridadEnemigoElegido = 7;
                                    objetivo = enemigo;
                                }

                            }

                            else //si no se puede atacar a ningun enemigo de ninguna forma, se defiende
                            {
                                prioridadEnemigoAElegir = 6;

                                if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                {
                                    prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                }
                            }
                        }

                    }


                }


                //Ahora se mira que accion contra el enemigo es la mas correcta, suponiendo que haya enemigos

                if (yaSeHaElegidoAccion == false)
                {

                    if (prioridadEnemigoElegido == 10)
                    {
                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 0).First();
                            gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - habi.GetCoste());
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, gob);

                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Piro") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);

                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Fire1");
                            GameManager.ReproducirAnimacion(0, objetivo);
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 0).First();
                            GameManager.AtacarUnidadHabilidadMagica(gob, objetivo, habi);
                            //animacionesAtaques.SetInteger("ID", 7);
                            segundaParteAnimacion = true;
                        }
                    }
                    else if (prioridadEnemigoElegido == 9)
                    {

                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 2).First();
                            gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - habi.GetCoste());
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, gob);

                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Electro") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);

                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;

                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Thunder1");
                            GameManager.ReproducirAnimacion(2, objetivo);
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 2).First();
                            GameManager.AtacarUnidadHabilidadMagica(gob, objetivo, habi);
                            //animacionesAtaques.SetInteger("ID", 7);
                            segundaParteAnimacion = true;
                        }


                    }
                    else if (prioridadEnemigoElegido == 8)
                    {
                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 1).First();
                            gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - habi.GetCoste());
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, gob);

                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Aqua") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Water1");
                            GameManager.ReproducirAnimacion(1, objetivo);
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 1).First();
                            GameManager.AtacarUnidadHabilidadMagica(gob, objetivo, habi);
                            //animacionesAtaques.SetInteger("ID", 7);
                            segundaParteAnimacion = true;
                        }

                    }
                    else if (prioridadEnemigoElegido == 7)
                    {
                        if (primeraParteAnimacion == false)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, null, 0);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            primeraParteAnimacion = true;
                            gobAnimator.SetBool("estaAtacando", true);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            GameManager.ReproducirSonido("Audio/Ice2");
                            GameManager.ReproducirAnimacion(20, objetivo);
                            GameManager.AtacarUnidad(gob, objetivo);
                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Ataque-Mago") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            segundaParteAnimacion = true;
                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;


                        }

                    }
                    else
                    {
                        gob.GetComponent<Unidad>().SetEstaDefendiendo(true);
                        cursor.transform.position = gob.transform.position;
                        camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                        gob.GetComponent<Unidad>().setTurnoAcabado(true);

                        RestaurarValoresTrasAccion(gob);


                    }

                }
                else //animaciones de envenamiento, ataque y demas
                {


                    if (estadoElegido == 12)
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Envenenar") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        }

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {

                            GameManager.ReproducirSonido("Audio/Pollen");
                            GameManager.ReproducirAnimacion(12, objetivo);
                            segundaParteAnimacion = true;

                        }
                    }
                    else if (estadoElegido == 13)
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Quemar") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        }

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1 && delta == 1)
                        {


                            GameManager.ReproducirSonido("Audio/Fire2");
                            GameManager.ReproducirAnimacion(13, objetivo);
                            segundaParteAnimacion = true;


                        }
                    }
                    else if (estadoElegido == 14)
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Dormir") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        }

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1 && delta == 1)
                        {


                            GameManager.ReproducirSonido("Audio/Sleep");
                            GameManager.ReproducirAnimacion(14, objetivo);
                            segundaParteAnimacion = true;


                        }
                    }
                    else if (estadoElegido == 15)
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Magica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Paralizar") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);



                        }

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1 && delta == 1)
                        {


                            GameManager.ReproducirSonido("Audio/Leakage");
                            GameManager.ReproducirAnimacion(15, objetivo);
                            segundaParteAnimacion = true;


                        }
                    }
                    else //está atacando sin usar magia
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Ataque-Mago") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            segundaParteAnimacion = true;
                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;


                        }
                    }
                }

            }
            else if (claseUnidad.GetTipoClase() == 4) //arquero
            {
                /*
                 * Lo mismo que el mago, pero los roles se invierten: el arquero pone enfasis en atacar fisicamente sin habilidades y unicamente usa estas ultimas si asegura que maten al oponente
                 * Si ningun escenario se puediera dar, se defenderia
                 * 
                 * El arquero busca qué enemigo tiene menos Puntos de Salud dentro de su rango y lo prioriza
                 */


                if (sePuedePulsarTecla)
                {
                    sePuedePulsarTecla = false;
                    rangosPosicionesAccionesCPU.Clear();
                    posiblesObjetivos.Clear();
                    rangosPosicionesAccionesCPU.Add(-1, GameManager.DeterminarPosiblesEnemigosAtacarCPU(gob));

                    foreach (Habilidad hab in claseUnidad.GetHabilidades()) //se comprueban las posiciones de los aliados según la habilidad que conozca la unidad
                    {

                        rangosPosicionesAccionesCPU.Add(hab.GetID(), GameManager.DeterminarPosiblesEnemigosHabilidadDañinaCPU(gob, hab));

                    }


                    //Ahora, se buscarán enemigos a los que atacar


                    foreach (GameObject enemigo in unidadesJugador)
                    {
                        if (rangosPosicionesAccionesCPU[5].Contains(enemigo.transform.position) || rangosPosicionesAccionesCPU[6].Contains(enemigo.transform.position))
                        {
                            posiblesObjetivos.Add(enemigo);

                        }
                        if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position))
                        {
                            posiblesObjetivos.Add(enemigo);

                        }

                    }

                    foreach (GameObject enemigo in posiblesObjetivos)
                    {
                        if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position) && GameManager.CalcularDañoAtaque(gob, enemigo) >= enemigo.GetComponent<Clase>().GetPSAct())
                        {
                            if (primeraParteAnimacion == false)
                            {
                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, enemigo, null, 0);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                gobAnimator.SetBool("estaAtacando", true);
                                GameManager.ReproducirSonido("Audio/Crossbow");
                                GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                GameManager.ReproducirAnimacion(23, enemigo);
                                primeraParteAnimacion = true;
                                yaSeHaElegidoAccion = true;
                                GameManager.AtacarUnidad(gob, enemigo);
                                estadoElegido = 14;
                                objetivo = enemigo;
                            }

                            break;
                        }

                        else if (rangosPosicionesAccionesCPU[5].Contains(enemigo.transform.position) && GameManager.CalcularDañoHabilidadFisica(gob, enemigo, claseUnidad.GetHabilidades().Where(n => n.GetID() == 5).First()) >= enemigo.GetComponent<Clase>().GetPSAct() && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 5).First().GetCoste())
                        {


                            if (primeraParteAnimacion == false)
                            {

                                Habilidad hab = claseUnidad.GetHabilidades().Where(n => n.GetID() == 5).First();
                                gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - hab.GetCoste());
                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, enemigo, hab, 0);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                primeraParteAnimacion = true; //para que se ejecute solo una vez
                                yaSeHaElegidoAccion = true;
                                estadoElegido = 12;
                                GameManager.ReproducirSonido("Audio/Skill1");
                                GameManager.ReproducirAnimacion(19, gob);

                                objetivo = enemigo;
                            }


                            break;
                        }
                        else if (rangosPosicionesAccionesCPU[6].Contains(enemigo.transform.position) && GameManager.CalcularDañoHabilidadFisica(gob, enemigo, claseUnidad.GetHabilidades().Where(n => n.GetID() == 6).First()) >= enemigo.GetComponent<Clase>().GetPSAct() && claseUnidad.GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 6).First().GetCoste())
                        {
                            if (primeraParteAnimacion == false)
                            {

                                Habilidad hab = claseUnidad.GetHabilidades().Where(n => n.GetID() == 6).First();
                                gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - hab.GetCoste());
                                Animator gobAnimator = gob.GetComponent<Animator>();
                                gobAnimator.SetBool("estaCaminando", true);
                                gob.transform.position = CasillaAMayorDistancia(gob, enemigo, hab, 0);
                                cursor.transform.position = gob.transform.position;
                                camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                                GameManager.CambiarDireccionUnidadEnemiga(gob, enemigo);
                                primeraParteAnimacion = true; //para que se ejecute solo una vez
                                yaSeHaElegidoAccion = true;
                                estadoElegido = 13;
                                GameManager.ReproducirSonido("Audio/Skill1");
                                GameManager.ReproducirAnimacion(19, gob);

                                objetivo = enemigo;
                            }


                            break;
                        }


                        else
                        { //si no se puede asegurar la ejecucion de una unidad, se ataca usando una habilidad o atacando con normalidad

                            if (rangosPosicionesAccionesCPU[5].Contains(enemigo.transform.position) && claseUnidad.GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 5).First().GetCoste())
                            {

                                prioridadEnemigoAElegir = 8;
                                if (objetivo != null)
                                {
                                    if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                    {

                                        if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                        {
                                            prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                            objetivo = enemigo;

                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }


                                    }
                                    else if (objetivo == null)
                                    {
                                        objetivo = enemigo;
                                    }

                                }

                                else
                                {
                                    prioridadEnemigoElegido = 8;
                                    objetivo = enemigo;
                                }

                            }
                            else if (rangosPosicionesAccionesCPU[6].Contains(enemigo.transform.position) && gob.GetComponent<Clase>().GetPSAct() > claseUnidad.GetHabilidades().Where(n => n.GetID() == 6).First().GetCoste())
                            {

                                prioridadEnemigoAElegir = 9;
                                if (objetivo != null)
                                {
                                    if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                    {

                                        if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                        {
                                            prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                            objetivo = enemigo;

                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }


                                    }
                                    else if (objetivo == null)
                                    {
                                        objetivo = enemigo;
                                    }


                                }

                                else
                                {
                                    prioridadEnemigoElegido = 9;
                                    objetivo = enemigo;
                                }

                            }

                            else if (rangosPosicionesAccionesCPU[-1].Contains(enemigo.transform.position))
                            {

                                prioridadEnemigoAElegir = 10;
                                if (objetivo != null)
                                {
                                    if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                    {

                                        if (objetivo.GetComponent<Clase>().GetPSAct() > enemigo.GetComponent<Clase>().GetPSAct())
                                        {
                                            prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                            objetivo = enemigo;

                                        }
                                        else if (objetivo == null)
                                        {
                                            objetivo = enemigo;
                                        }


                                    }
                                    else if (objetivo == null)
                                    {
                                        objetivo = enemigo;
                                    }


                                }

                                else
                                {
                                    prioridadEnemigoElegido = 10;
                                    objetivo = enemigo;
                                }

                            }

                            else //si no se puede atacar a ningun enemigo de ninguna forma, se defiende
                            {
                                prioridadEnemigoAElegir = 7;

                                if (prioridadEnemigoAElegir >= prioridadEnemigoElegido)
                                {
                                    prioridadEnemigoElegido = prioridadEnemigoAElegir;
                                }
                            }
                        }

                    }


                }


                //Ahora se mira que accion contra el enemigo es la mas correcta, suponiendo que haya enemigos

                if (yaSeHaElegidoAccion == false)
                {

                    if (prioridadEnemigoElegido == 8)
                    {
                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 5).First();
                            gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - habi.GetCoste());
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Skill1");
                            GameManager.ReproducirAnimacion(19, gob);

                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Fisica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Diana") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);

                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Crossbow");
                            GameManager.ReproducirAnimacion(5, objetivo);
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 5).First();
                            GameManager.AtacarUnidadHabilidadFisica(gob, objetivo, habi);
                            //animacionesAtaques.SetInteger("ID", 7);
                            segundaParteAnimacion = true;
                        }
                    }
                    else if (prioridadEnemigoElegido == 9)
                    {

                        if (primeraParteAnimacion == false)
                        {
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 6).First();
                            gob.GetComponent<Clase>().SetPSAct(claseUnidad.GetPSAct() - habi.GetCoste());
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, habi, 1);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            primeraParteAnimacion = true; //para que se ejecute solo una vez
                            GameManager.ReproducirSonido("Audio/Skill1");
                            GameManager.ReproducirAnimacion(19, gob);

                        }
                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Fisica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Doble-Disparo") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);

                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;

                        } //&& animacionesAtaques.GetCurrentAnimatorClipInfo(0). >= 1.0f)

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {
                            GameManager.ReproducirSonido("Audio/Crossbow");
                            GameManager.ReproducirAnimacion(6, objetivo);
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 6).First();
                            GameManager.AtacarUnidadHabilidadFisica(gob, objetivo, habi);
                            //animacionesAtaques.SetInteger("ID", 7);
                            segundaParteAnimacion = true;
                        }


                    }

                    else if (prioridadEnemigoElegido == 10)
                    {
                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);

                        if (primeraParteAnimacion == false)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            gobAnimator.SetBool("estaCaminando", true);
                            gob.transform.position = CasillaAMayorDistancia(gob, objetivo, null, 0);
                            cursor.transform.position = gob.transform.position;
                            camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                            primeraParteAnimacion = true;
                            gobAnimator.SetBool("estaAtacando", true);
                            GameManager.ReproducirSonido("Audio/Crossbow");
                            GameManager.ReproducirAnimacion(23, objetivo);
                            GameManager.CambiarDireccionUnidadEnemiga(gob, objetivo);
                            GameManager.AtacarUnidad(gob, objetivo);
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Ataque-Arquero") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            segundaParteAnimacion = true;
                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;


                        }

                    }
                    else
                    {
                        gob.GetComponent<Unidad>().SetEstaDefendiendo(true);
                        cursor.transform.position = gob.transform.position;
                        camara.transform.position = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
                        gob.GetComponent<Unidad>().setTurnoAcabado(true);

                        RestaurarValoresTrasAccion(gob);


                    }

                }
                else //animaciones de envenamiento, ataque y demas
                {


                    if (estadoElegido == 12)
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Fisica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;

                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Diana") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);
                            GameManager.EliminarPopUp(popUp);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        }

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {

                            GameManager.ReproducirSonido("Audio/Crossbow");
                            GameManager.ReproducirAnimacion(5, objetivo);
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 5).First();
                            GameManager.AtacarUnidadHabilidadFisica(gob, objetivo, habi);
                            segundaParteAnimacion = true;

                        }
                    }
                    else if (estadoElegido == 13)
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Preparar-Habilidad-Fisica") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            Animator gobAnimator = gob.GetComponent<Animator>();
                            animacionesAtaques.SetInteger("ID", -1);
                            gobAnimator.SetBool("estaAtacando", true);
                            delta = 1;
                        }

                        else if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Doble-Disparo") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {


                            RestaurarValoresTrasAccion(gob);
                            GameManager.EliminarPopUp(popUp);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;



                        }

                        else if (primeraParteAnimacion && segundaParteAnimacion == false && delta == 1)
                        {


                            GameManager.ReproducirSonido("Audio/Crossbow");
                            GameManager.ReproducirAnimacion(6, objetivo);
                            Habilidad habi = claseUnidad.GetHabilidades().Where(n => n.GetID() == 6).First();
                            GameManager.AtacarUnidadHabilidadFisica(gob, objetivo, habi);
                            segundaParteAnimacion = true;


                        }
                    }


                    else //está atacando sin usar magia
                    {
                        if (animacionesAtaques.GetCurrentAnimatorStateInfo(0).IsName("Ataque-Arquero") && animacionesAtaques.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {

                            animacionesAtaques.SetInteger("ID", -1);
                            segundaParteAnimacion = true;
                            RestaurarValoresTrasAccion(gob);

                            GameManager.EliminarPopUp(popUp);
                            prioridadAliadoAElegir = 0;

                            prioridadAliadoElegido = 0;

                            prioridadEnemigoAElegir = 0;

                            prioridadEnemigoElegido = 0;


                        }
                    }
                }


            }



        }
        else //si está paralizado/dormido, no se podrá mover, así que se contará como si ya hubiera consumido su turno
        {
            contadorUnidades++;
        }

    }


    public static int CompararaPrioridadCPU(GameObject unidad1, GameObject unidad2)
    {

        // En este caso, los clérigos irán primero. Luego, los lanceros, después los magos y por último, los arqueros


        int resultado = 0;

        Clase claseU1 = unidad1.GetComponent<Clase>();

        Clase claseU2 = unidad2.GetComponent<Clase>();

        if (claseU1.GetPrioridadOrdenCPU() > claseU2.GetPrioridadOrdenCPU())
        {

            resultado = -1;

        }
        else if (claseU1 == null && claseU2 == null)
        {
            resultado = 0;
        }

        else if (claseU1.GetPrioridadOrdenCPU() < claseU2.GetPrioridadOrdenCPU())
        {
            resultado = 1;
        }
        else if (claseU1 == null)
        {
            resultado = 1;
        }
        else if (claseU2 == null)
        {
            resultado = -1;
        }

        return resultado;
    }


    public static int GetContadorUnidades()
    {
        return contadorUnidades;
    }

    public static void SetContadorUnidades(int cont)
    {
        contadorUnidades = cont;
    }


    private static Vector3 CasillaAMayorDistancia(GameObject atacante, GameObject objetivo, Habilidad hab, int tipo)
    {

        Vector3 posicionResultado = cursor.transform.position;

        if (atacante != null)
        {
            unidadAuxiliar = atacante;


            posicionResultado = new Vector3(atacante.transform.position.x, atacante.transform.position.y, 0f);

            Vector3 posicionObjetivo = objetivo.transform.position;

            List<Vector3> movimientosCPU;

            List<Vector3> ataqueCPU;

            float distanciaEntreCasillas;

            float distanciaEntreCasillasMayor = 0.0f;

            Clase claseUnidadEnemiga = atacante.GetComponent<Clase>();

            //se guardan las posiciones a las que puede moverse

            movimientosCPU = GameManager.PosicionesPosiblesUnidad(atacante);

            foreach (Vector3 pos in movimientosCPU) //por cada casilla donde se puede deplazar la unidad...
            {
                unidadAuxiliar.transform.position = pos; //se coloca la unidad auxiliar
                GameManager.BorrarCasillas();

                if (hab == null)
                {
                    ataqueCPU = GameManager.PosicionesPosiblesAtacarUnidadCPU(unidadAuxiliar); //y se calcula su rango de ataque
                }
                else
                {
                    if (tipo == 0) //Habilidad dañina
                    {
                        ataqueCPU = GameManager.PosicionesPosiblesUsarHabilidadDañinaCPU(unidadAuxiliar, hab);
                    }
                    else //Habilidad de apoyo
                    {
                        ataqueCPU = GameManager.PosicionesPosiblesUsarHabilidadApoyoCPU(unidadAuxiliar, hab);
                    }
                }


                foreach (Vector3 posAtaque in ataqueCPU) //por cada casilla dentro del rango de ataque...
                {
                    if (posicionObjetivo == posAtaque) //se mira si el enemigo está dentro de esta. Si lo está...
                    {
                        distanciaEntreCasillas = (posAtaque - pos).sqrMagnitude; //se calcula la distancia

                        if(hab == null)
                        {
                            if (distanciaEntreCasillas*distanciaEntreCasillas > distanciaEntreCasillasMayor) //si la distancia actual es mayor que la que habia guardada
                            {
                                distanciaEntreCasillasMayor = distanciaEntreCasillas*distanciaEntreCasillas; //se actualiza el valor y se guarda la posicion a la que se puede mover (no la de ataque)
                                posicionResultado = new Vector3(pos.x, pos.y, pos.z);
                            }
                        }
                        else
                        {
                            if (distanciaEntreCasillas * distanciaEntreCasillas > distanciaEntreCasillasMayor) //si la distancia actual es mayor que la que habia guardada
                        {
                            distanciaEntreCasillasMayor = distanciaEntreCasillas*distanciaEntreCasillas; //se actualiza el valor y se guarda la posicion a la que se puede mover (no la de ataque)
                            posicionResultado = new Vector3(pos.x, pos.y, pos.z);
                        }
                        }

                        
                    }
                }

            }
        }

        GameManager.BorrarCasillas();

        return posicionResultado; //se devuelve la casilla que está más lejos dentro del rango de ataque
    }

    private static void InflingirEstadoEnemigo(GameObject gob, GameObject enemigo, Clase claseUnidad, int IDHabilidad)
    {
        int psActuales = claseUnidad.GetPSAct();

        psActuales -= claseUnidad.GetHabilidades().Where(n => n.GetID() == IDHabilidad).First().GetCoste();

        if (IDHabilidad == 12)
        {
            GameManager.InflingirEstadoDañino(enemigo, 0, 3);
        }
        else if (IDHabilidad == 13)
        {
            GameManager.InflingirEstadoDañino(enemigo, 1, 3);
        }
        else if (IDHabilidad == 15)
        {
            GameManager.InflingirEstadoDañino(enemigo, 2, 3);
        }

        else
        {
            GameManager.InflingirEstadoDañino(enemigo, 3, 9999);
        }


    }

    private static void RestaurarValoresTrasAccion(GameObject gob)
    {
        animacionesAtaques.SetInteger("ID", -1);

        gob.GetComponent<Animator>().SetBool("estaCaminando", false);
        gob.GetComponent<Animator>().SetBool("estaAtacando", false);

        prioridadAliadoAElegir = 0;

        prioridadAliadoElegido = 0;

        prioridadEnemigoAElegir = 0;

        prioridadEnemigoElegido = 0;

        Color colo = new Color(0.3f, 0.3f, 0.3f, 1);

        colo.a = 0.9f;

        gob.GetComponent<SpriteRenderer>().color = colo;

        GameManager.BorrarCasillas();

        GameManager.CerrarInterfazUnidad();

        primeraParteAnimacion = false;

        segundaParteAnimacion = false;

        sePuedePulsarTecla = true;

        yaSeHaElegidoAccion = false;

        contadorUnidades++;

        estadoElegido = 0;

        objetivo = null;

        delta = 0f;
    }

    public static List<GameObject> GetJugadoresConestadosAlerados()
    {
        return jugadoresConEstadosAlterados;
    }

    public static void SetJugadoresConEstadosAlterados(List<GameObject> jugadores)
    {
        jugadoresConEstadosAlterados = jugadores;
    }

}

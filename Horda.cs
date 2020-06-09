using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horda
{

    #region Declaracion de propiedades
    //Estos valores siempre se mantendrán igual en el resto del código, así que se colocarán como constantes. Además, si se quisiera cambiar en el futuro, sólo habría que editar un par de líneas y no todo el código
    private const int tamanoIndividuo = 8;
    private const float porcentajePosibilidadesMutacion = 4f; //4%
    private const float aumentoFitnessGenCorrecto = 100f;

    private List<int> hordaEnemiga;
    private List<int> armasJugador;
    private List<int> genesIndividuo;
    private float resultadoFitness;

    private System.Random numeroAleatorio;

    #endregion

    #region Constructor

    public Horda(System.Random nAletorio, List<int> tropaEnemiga, List<int> armasJugador)
    {

        hordaEnemiga = tropaEnemiga;
        genesIndividuo = new List<int>() { }; //aunque podría haber menos de 8 individuos en la solución final, la lista puede tener, como máximo, 8 enemigos. Los espacios en blanco se marcarán con un 0 e la posición.
        numeroAleatorio = nAletorio;
        resultadoFitness = 0.0f;
        this.armasJugador = armasJugador;


    }

    #endregion


    public Horda MutacionDeGen(Horda individuo)
    {
        List<int> listaMutada = new List<int>(individuo.getGenesIndividuo());
        //Se echará a suertes si existirá una mutación en el individuo. Si se produce mutación, se cambiara un gen aleatorio siguiendo el método de Bit Flip en el caso de las armas

        int limite = Mathf.FloorToInt(numeroAleatorio.Next(0, 100)); //tendra un porcentaje de posibilidad de mutacion entre 100. El porcentaje se define más arriba, en la declaración de propiedades

        if (limite <= porcentajePosibilidadesMutacion)
        {

            int indice = Mathf.FloorToInt(numeroAleatorio.Next(0, tamanoIndividuo));

            if (hordaEnemiga == null) //si se va a generar el grupo de enemigos, el valor deberá cambiar entre 0 y 4
            {
                listaMutada[indice] = numeroAleatorio.Next(0, 5);
                individuo.setGenesIndividuo(listaMutada);
            }
            else //si se trata de la generación de armas, se cambia el valor del bit
            {
                if(listaMutada[indice] == 0)
                {
                    listaMutada[indice] = 1;
                }
                else
                {
                    listaMutada[indice] = 0;
                }

                individuo.setGenesIndividuo(listaMutada);
            }

        }

        return individuo;

    }


    public Horda CruceIndividuos(Horda padre, Horda madre)
    {
        Horda nuevoIndividuo;

        int puntoCorte;

        //Al cruzar dos individuos, debe salir uno nuevo. Para ello, se usará la técnica del único punto de corte.

        if (hordaEnemiga == null)
        {
            nuevoIndividuo = new Horda(numeroAleatorio, null, null);
        }
        else
        {
            nuevoIndividuo = new Horda(numeroAleatorio, hordaEnemiga, armasJugador);
        }

        /*Una vez se sabe qué se va a generar (enemigos o armas), se procede a elegir un punto de corte al azar. Todos los genes que estén situados antes de o en la misma posición elegida para el corte
            pertenecerán al primer padre, y el resto serán del segundo padre

        */

        puntoCorte = numeroAleatorio.Next(0, tamanoIndividuo);

        for (int contador = 0; contador < tamanoIndividuo; contador++)
        {
            if (contador <= puntoCorte)
            {
                nuevoIndividuo.genesIndividuo.Add(padre.genesIndividuo[contador]);
            }
            else
            {
                nuevoIndividuo.genesIndividuo.Add(madre.genesIndividuo[contador]);
            }
        }

        return nuevoIndividuo;

    }

    public float DeterminarValorFitnessArmasHorda(List<int> armasJugador)
    {

        float fitness = 0.0f;

        int contadorArmas0 = 0; //numero de armas que tienen mas ataque que defensas

        int contadorArmas1 = 0; //numero de armas que tienen mas defensas que ataque

        List<int> copiaArmas = new List<int>(genesIndividuo);

        for (int j = 0; j < armasJugador.Count; j++)
        {

            if (armasJugador[j] == 0)
            {
                contadorArmas0++;
            }
            else if (armasJugador[j] == 1)
            {
                contadorArmas1++;
            }

        }


        for (int i = 0; i < copiaArmas.Count; i++)
        {

            switch (copiaArmas[i])
            {

                case 0:

                    if (contadorArmas1 > contadorArmas0)
                    {
                        fitness += aumentoFitnessGenCorrecto; //Si han escogido más armas ofensivas que defensivas, se premia que el individuo tenga más armas defensivas
                    }

                    break;

                case 1:

                    if (contadorArmas1 <= contadorArmas0)
                    {
                        fitness += aumentoFitnessGenCorrecto; //No obstante, si hay más armas defensivas que ofensivas en el arsenal del jugador, se premia que el individuo tenga más armas ofensivas
                    }

                    break;

                default:
                    break;

            }


        }

        return fitness;

    }


    public float DeterminarValorFitnessHorda(List<int> tropaJugador, Horda posibleHorda)
    {
        //Para determinar los enemigos a generar, se observa si contiene unidades capaces de contrarrestar a todas unidades del jugador. Se sabe qué unidades contrarrestan a otras, así que se revisará si se cumple usando una lógica siminar al del juego Piedra, papel, Tijeras

        float puntuacionParaFitness = 0.0f;

        //Se hace una copia de posibleHorda, pues se va a manipular esta lista y no se desea afectar la original

        List<int> copiaHorda = new List<int>(posibleHorda.genesIndividuo);

        int numeroAparicionesEnemigo = 0;
        int unidadesQuitadas = 0;

        for (int ind = 0; ind < tropaJugador.Count; ind++)
        {
            //Se recorre la lista de la tropa del jugador indice a indice, y se revisa el contenido de la posible horda


            int miembroJugador = tropaJugador[ind];


            /* Ahora se procederá a observar cuántos enemigos aparecen en la lista para contrarrestar la tropa del jugador. Por cada indice de la lista del jugador, debe haber dos que le contrarresten.
             * Así, se asegura que el número de unidades a derrotar sea mayor que el número de unidades que tenga el jugador.
             * 
             * Sin embargo, es posible que el jugador tenga una tropa compuesta por dos, tres o cuatro miembros de la misma clase.
             */

            for (int indHorda = posibleHorda.getGenesIndividuo().Count - 1 - unidadesQuitadas; indHorda >= 0; indHorda--)
            {


                if (numeroAparicionesEnemigo < 2)
                {
                    switch (miembroJugador)
                    {

                        case 1: //mago - la horda debe poseer al menos 2 lanceros
                            if (copiaHorda[indHorda] == 2)
                            {
                                numeroAparicionesEnemigo++;
                                puntuacionParaFitness = puntuacionParaFitness + aumentoFitnessGenCorrecto;
                                unidadesQuitadas++;
                                copiaHorda.RemoveAt(indHorda);
                            }


                            break;

                        case 2: //lancero - la horda debe poseer al menos 2 clerigos

                            if (copiaHorda[indHorda] == 3)
                            {
                                numeroAparicionesEnemigo++;
                                puntuacionParaFitness = puntuacionParaFitness + aumentoFitnessGenCorrecto;
                                unidadesQuitadas++;
                                copiaHorda.RemoveAt(indHorda);
                            }

                            break;
                        case 3: //clerigo - la horda debe poseer al menos 2 arqueros

                            if (copiaHorda[indHorda] == 4)
                            {
                                numeroAparicionesEnemigo++;
                                puntuacionParaFitness = puntuacionParaFitness + aumentoFitnessGenCorrecto;
                                unidadesQuitadas++;
                                copiaHorda.RemoveAt(indHorda);
                            }

                            break;
                        case 4: //arquero - la horda debe poseer al menos 2 magos
                            if (copiaHorda[indHorda] == 1)
                            {
                                numeroAparicionesEnemigo++;
                                puntuacionParaFitness = puntuacionParaFitness + aumentoFitnessGenCorrecto;
                                unidadesQuitadas++;
                                copiaHorda.RemoveAt(indHorda);
                            }

                            break;
                        default: //0 o cualquier otra cosa; significa que hay un hueco en la tropa del jugador, y por tanto debe haber dos huecos vacíos en la horda

                            if (copiaHorda[indHorda] == 0)
                            {
                                numeroAparicionesEnemigo++;
                                puntuacionParaFitness = puntuacionParaFitness + aumentoFitnessGenCorrecto;
                                unidadesQuitadas++;
                                copiaHorda.RemoveAt(indHorda);
                            }

                            break;

                    }

                }
                else
                {

                    break; //salimos del blucle for que mira en la lista y pasamos al siguiente índice de la lista del jugador
                }

            }
            numeroAparicionesEnemigo = 0;

            /*volvemos a poner a 0 esta variable para poder revisar el siguiente índice. Si no se hiciera, en el caso de contar con, por ejemplo, una tropa con 3 clérigos, podría correrse el riesgo de contar la misma unidad enemiga
            * más veces de las que realmente existen
            */
        }


        return puntuacionParaFitness;

    }

    public void crearGenesIndividuo()
    {
        int contadorCeros = 0;
        //Se toma un individuo y se rellena la lista de enteros con numeros dentro del rango esperado
        int gen;

        for (int inte = 0; inte < tamanoIndividuo; inte++)
        {

            if (hordaEnemiga == null)
            {
                gen = numeroAleatorio.Next(0, 5);
            }
            else
            {
                gen = numeroAleatorio.Next(0, 2);
            }


            genesIndividuo.Add(gen);
            if (gen == 0)
            {
                contadorCeros++;
            }

        }

        if (contadorCeros > 6)
        {

            //En este caso, se tendría que todos los índices son igual a 0, y eso no es posible. Se alteran los genes para que haya al menos dos enemigos en la horda

            //esto sólo pasa en el caso en el que se esté creando las unidades. La generación de armas sí que puede devolver todo 0 (el arma básica, más ataque que defensa)

            if (hordaEnemiga == null)
            {
                genesIndividuo[0] = numeroAleatorio.Next(1, 5); //entre 1 inclusivo y 5 exclusivo, para evitar la posibilidad de que salga 0 de nuevo
                genesIndividuo[1] = numeroAleatorio.Next(1, 5);
            }

        }
    }

    #region Getters y Setters
    public List<int> getGenesIndividuo()
    {

        return genesIndividuo;
    }

    public void setGenesIndividuo(List<int> genes)
    {
        genesIndividuo = genes;

    }

    public float getResultadoFitness()
    {

        return resultadoFitness;
    }

    public void setResultadoFitness(float nuevoFitness)
    {

        resultadoFitness = nuevoFitness;
    }

    public int GteTamanoIndividuo()
    {
        return tamanoIndividuo;
    }

    public float GetAumentoFitnessGenCorrecto()
    {
        return aumentoFitnessGenCorrecto;
    }

    #endregion

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreacionHordas
{
    //Estos valores siempre se mantendr�n igual en el resto del c�digo, as� que se colocar�n como constantes. Adem�s, si se quisiera cambiar en el futuro, s�lo habr�a que editar un par de l�neas y no todo el c�digo
    public const int tamanoIndividuo = 8;
    private const float porcentajePosibilidadesMutacion = 0.05f; //5%
    public const float aumentoFitnessGenCorrecto = 100f;


    public List<int> genesIndividuo;
    public float resultadoFitness;


    private System.Random numeroAleatorio = new System.Random();
    private System.Random genACambiar = new System.Random();
    private System.Random valorGenMutado = new System.Random();

    public CreacionHordas(System.Random nAletorio)
    {

        genesIndividuo = new List<int>() { }; //aunque podr�a haber menos de 8 individuos en la soluci�n final, la lista puede tener, como m�ximo, 8 enemigos. Los espacios en blanco se marcar�n con un 0 e la posici�n.
        numeroAleatorio = nAletorio;
        this.resultadoFitness = 0.0f;
    }

    public CreacionHordas MutacionDeGen(CreacionHordas individuo)
    {

        //Se echar� a suertes si existir� una mutaci�n en el individuo. Si se produce mutaci�n, se cambiara un gen aleatorio

        if (numeroAleatorio.NextDouble() < porcentajePosibilidadesMutacion)
        {
            int indice = genACambiar.Next(0, tamanoIndividuo);
            individuo.genesIndividuo[indice] = valorGenMutado.Next(0, 5);

        }

        return individuo;

    }


    public CreacionHordas CruceIndividuos(CreacionHordas padre, CreacionHordas madre)
    {

        //Al cruzar dos individuos, debe salir uno nuevo. Para ello, se tomar�n dos listas de int y se crear� una nueva tomando valores de los �ndices de ambos de forma aleatoria.

        CreacionHordas nuevoIndividuo = new CreacionHordas(numeroAleatorio);



        for (int j = 0; j < tamanoIndividuo; j++)
        {

            if (numeroAleatorio.NextDouble() < 0.5)
            {
                nuevoIndividuo.genesIndividuo.Add(padre.genesIndividuo[j]);

            }
            else
            {
                nuevoIndividuo.genesIndividuo.Add(madre.genesIndividuo[j]);
            }

        }

        return nuevoIndividuo;
    }



    public float DeterminarValorFitness(List<int> tropaJugador, CreacionHordas posibleHorda)
    {
        //Para determinar los enemigos a generar, se observa si contiene unidades capaces de contrarrestar a todas unidades del jugador. Se sabe qu� unidades contrarrestan a otras, as� que se revisar� si se cumple usando una l�gica siminar al del juego Piedra, papel, Tijeras

        float puntuacionParaFitness = 0.0f;

        //Se hace una copia de posibleHorda, pues se va a manipular esta lista y no se desea afectar la original

        List<int> copiaHorda = new List<int>(posibleHorda.genesIndividuo);

        int numeroAparicionesEnemigo = 0;
        int unidadesQuitadas = 0;

        for (int ind = 0; ind < tropaJugador.Count; ind++)
        {
            //Se recorre la lista de la tropa del jugador indice a indice, y se revisa el contenido de la posible horda

            
            int miembroJugador = tropaJugador[ind];
            

            /* Ahora se proceder� a observar cu�ntos enemigos aparecen en la lista para contrarrestar la tropa del jugador. Por cada indice de la lista del jugador, debe haber dos que le contrarresten.
             * As�, se asegura que el n�mero de unidades a derrotar sea mayor que el n�mero de unidades que tenga el jugador.
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
                        default: //0 o cualquier otra cosa; significa que hay un hueco en la tropa del jugador, y por tanto debe haber dos huecos vac�os en la horda

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

                    break; //salimos del blucle for que mira en la lista y pasamos al siguiente �ndice de la lista del jugador
                }

            }
            numeroAparicionesEnemigo = 0;

            /*volvemos a poner a 0 esta variable para poder revisar el siguiente �ndice. Si no se hiciera, en el caso de contar con, por ejemplo, una tropa con 3 cl�rigos, podr�a correrse el riesgo de contar la misma unidad enemiga
            * m�s veces de las que realmente existen
            */
        }

        

        return puntuacionParaFitness;

    }

    public void crearGenesIndividuo()
    {
        int contadorCeros = 0;
        //Se toma un individuo y se rellena la lista de enteros con numeros dentro del rango esperado

        for (int inte = 0; inte < tamanoIndividuo; inte++)
        {
           
            int gen = numeroAleatorio.Next(0, 5);
            
            genesIndividuo.Add(gen);
            if(gen == 0)
            {
                contadorCeros++;
            }


        }

        if (contadorCeros > 6)
        {

            //En este caso, se tendr�a que todos los �ndices son igual a 0, y eso no es posible. Se alteran los genes para que haya al menos dos enemigos en la horda

            genesIndividuo[0] = valorGenMutado.Next(1, 5); //entre 1 inclusivo y 5 exclusivo, para evitar la posibilidad de que salga 0 de nuevo
            genesIndividuo[1] = valorGenMutado.Next(1, 5);

        }
    }

    public List<int> getGenesIndividuo()
    {

        return this.genesIndividuo;
    }

    public void setGenesIndividuo(List<int> genes)
    {
        this.genesIndividuo = genes;

    }

    public float getResultadoFitness()
    {

        return this.resultadoFitness;
    }

    public void setResultadoFitness(float nuevoFitness)
    {

        this.resultadoFitness = nuevoFitness;
    }
}
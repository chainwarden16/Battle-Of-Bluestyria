using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AlgoritmoCreacionHordas
{

    private const int tamanoPoblacion = 1000;

    private List<Horda> poblacionIndividuos;

    private System.Random numAleatorio;

    private List<int> tropaJugador;

    private List<int> hordaEnemiga;


    private float fitnessMejorSolucion = 0.0f;

    private float peorFitnessEncontrado = 1000.0f;

    private List<int> genesMejorSolucion = new List<int>() { };

    public AlgoritmoCreacionHordas(System.Random numAleatorio, List<int> tropaJugador, List<int> tropaEnemiga)
    {
        poblacionIndividuos = new List<Horda>();

        this.numAleatorio = numAleatorio;

        this.tropaJugador = tropaJugador;

        hordaEnemiga = tropaEnemiga;

        int i = 0;

        while (i < tamanoPoblacion)
        {

            Horda hordaNueva = new Horda(numAleatorio, tropaJugador, tropaEnemiga);
            hordaNueva.crearGenesIndividuo();
            poblacionIndividuos.Add(hordaNueva);
           
            i++;

        }

    }

    public void crearGeneracionSiguiente()
    {
        Horda hijo;

        if (hordaEnemiga == null)
        {
            hijo = new Horda(numAleatorio, null, null);
        }
        else
        {
            hijo = new Horda(numAleatorio, hordaEnemiga, TropaEscogida.GetArmasJugador());
        }

        

       

        float fitnessIndividuoActual;

        if (poblacionIndividuos.Count > 0)
        {
            List<Horda> generacion = new List<Horda>();



            for (int numeroIndividuo = 0; numeroIndividuo < poblacionIndividuos.Count; numeroIndividuo++)
            {
                //Se buscan padres con un fitness aceptable
                Horda padre = BuscarPadresConFitnessAceptable();

                Horda madre = BuscarPadresConFitnessAceptable();

                //se cruzan los individuos

                hijo = hijo.CruceIndividuos(padre, madre);

                //se produce una posible mutacion de un gen del hijo

                Horda hijoMutado = hijo.MutacionDeGen(hijo);

                //se añade el hijo con una posible mutacion a la generacion que se está creando

                generacion.Add(hijoMutado);

                //se determina el valor fitness del hijo

                if(hordaEnemiga == null)
                {
                    
                    fitnessIndividuoActual = hijoMutado.DeterminarValorFitnessHorda(tropaJugador, hijoMutado);

                }
                else
                {
                    fitnessIndividuoActual = hijoMutado.DeterminarValorFitnessArmasHorda(TropaEscogida.GetArmasJugador(), hordaEnemiga);
                }

                
                

                if (fitnessIndividuoActual > fitnessMejorSolucion) //si resula que este individuo es una mejor solución que la que se guarda actualmente, se reemplazan tanto el valor del mejor fitness como los gens guardados
                {

                    fitnessMejorSolucion = fitnessIndividuoActual;
                    genesMejorSolucion.Clear();
                    genesMejorSolucion.AddRange(hijoMutado.getGenesIndividuo());
                }

                //asimismo, hacemos una búsqueda del peor fitness de la generacion

                if(fitnessIndividuoActual < peorFitnessEncontrado)
                {
                    peorFitnessEncontrado = fitnessIndividuoActual;
                }

            }
            
            //eliminación de los sujetos menos aptos

            foreach(Horda hor in generacion.ToArray())
            {
                if(hor.getResultadoFitness() == peorFitnessEncontrado) //si es un individuo muy malo, se elimina y se reemplaza con un individuo nuevo que, con suerte, cuente con un mejor fitness. 
                    //Además, la introduccion de nuevos individuos evita una pronta convergencia
                {
                    generacion.Remove(hor);

                    Horda hordaNueva;

                    if(hordaEnemiga == null)
                    {
                        hordaNueva = new Horda(numAleatorio, null, null);
                    }
                    else
                    {
                        hordaNueva = new Horda(numAleatorio, hordaEnemiga, TropaEscogida.GetArmasJugador());
                    }

                    
                    hordaNueva.crearGenesIndividuo();
                    generacion.Add(hordaNueva);
                }
            }

            poblacionIndividuos = generacion; //se sutituye la antigua poblacion con la nueva

        }
    }
    

    private Horda BuscarPadresConFitnessAceptable()
    {

        //En esta función, se busca un padre (el que sea) que tenga un valor fitness por encima de un valor, determinado por el mejor fitness encontrado hasta ese momento. Para añadir aletoriedad, se divide entre una cantidad aleatoria

        Horda progenitor = null;

        float minimoFitness = fitnessMejorSolucion / numAleatorio.Next(2,5);

        foreach (Horda individuo in poblacionIndividuos)
        {

            if (individuo.getResultadoFitness() >= minimoFitness)
            {
                progenitor = individuo;
                break;
            }   
            
        }

        //necesitamos que haya un cruce sí o sí, así que si no se encuentra nada bueno, nos conformamos con uno cualquiera (como en la vida real)

        if (progenitor == null)
             {
                 int padreAleatorio = numAleatorio.Next(0, poblacionIndividuos.Count);

                 progenitor = poblacionIndividuos[padreAleatorio];
             }
        return progenitor;
    }


    #region Getters y setters

    public List<int> GetGenesMejorSolucion()
    {
        return genesMejorSolucion;
    }

    public float GetFitnessMejorSolucion()
    {
        return fitnessMejorSolucion;
    }

    #endregion

}
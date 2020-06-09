using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AlgoritmoCreacionHordas
{

    private const int tamanoPoblacion = 1000;

    private const int tamanoTorneo = 4; // Porcentaje de individuos a enfrentar en la seleccion por torneo. Se dividirá el total de la poblacion actual por este número (ej: con 4, se escogerá un 25% de la población)

    private List<Horda> poblacionIndividuos;

    private System.Random numAleatorio;

    private List<int> tropaJugador;

    private List<int> hordaEnemiga;

    private float fitnessTotalPoblacion = 0f;

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
            //Se puebla la primera generación
            Horda hordaNueva = new Horda(numAleatorio, tropaJugador, tropaEnemiga);
            hordaNueva.crearGenesIndividuo();
            poblacionIndividuos.Add(hordaNueva);


            i++;

        }

    }

    public void crearGeneracionSiguiente()
    {
        Horda hijo;

        float fitnessIndividuoActual;

        //Se comprueba si se va a generar las clases de los enemigos o sus armas

        if (hordaEnemiga == null)
        {
            hijo = new Horda(numAleatorio, null, null);
        }
        else
        {
            hijo = new Horda(numAleatorio, hordaEnemiga, TropaEscogida.GetArmasJugador());
        }


        if (poblacionIndividuos.Count > 0)
        {
            List<Horda> generacion = new List<Horda>();

            for (int numeroIndividuo = 0; numeroIndividuo < poblacionIndividuos.Count; numeroIndividuo++) //se calcula el fitness total de la poblacion actual para la seleccion por torneo
            {

                if (hordaEnemiga == null)
                {

                    fitnessTotalPoblacion += poblacionIndividuos[numeroIndividuo].DeterminarValorFitnessHorda(tropaJugador, poblacionIndividuos[numeroIndividuo]);

                }
                else
                {
                    fitnessTotalPoblacion += poblacionIndividuos[numeroIndividuo].DeterminarValorFitnessArmasHorda(TropaEscogida.GetArmasJugador(), hordaEnemiga);
                }


            }

            //Se procede a crear la generación nueva
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

                if (hordaEnemiga == null)
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

                //asimismo, se hace una búsqueda del peor fitness de la generacion

                if (fitnessIndividuoActual < peorFitnessEncontrado)
                {
                    peorFitnessEncontrado = fitnessIndividuoActual;
                }

            }

            //Siguiendo el modelo elitista de los algoritmos genéticos, se procede a la eliminación de los sujetos menos aptos

            foreach (Horda hor in generacion.ToArray())
            {
                if (hor.getResultadoFitness() == peorFitnessEncontrado) //si es un individuo muy malo, se elimina y se reemplaza con un individuo nuevo que, con suerte, cuente con un mejor fitness. 
                                                                        //Además, la introduccion de nuevos individuos evita una pronta convergencia
                {
                    generacion.Remove(hor);

                    Horda hordaNueva;

                    if (hordaEnemiga == null)
                    {
                        hordaNueva = new Horda(numAleatorio, null, null);
                    }
                    else
                    {
                        hordaNueva = new Horda(numAleatorio, hordaEnemiga, TropaEscogida.GetArmasJugador());
                    }


                    hordaNueva.crearGenesIndividuo(); //se crean sus genes y se añaden a la poblacion
                    generacion.Add(hordaNueva);

                    if (hordaEnemiga == null) //se comprueba si alguno de estos individuos nuevos tiene los genes que constituyen una solución óptima
                    {

                        fitnessIndividuoActual = hordaNueva.DeterminarValorFitnessHorda(tropaJugador, hordaNueva);

                    }
                    else
                    {
                        fitnessIndividuoActual = hordaNueva.DeterminarValorFitnessArmasHorda(TropaEscogida.GetArmasJugador(), hordaEnemiga);
                    }

                    if (fitnessIndividuoActual > fitnessMejorSolucion) //si resula que este individuo es una mejor solución que la que se guarda actualmente, se reemplazan tanto el valor del mejor fitness como los gens guardados
                    {

                        fitnessMejorSolucion = fitnessIndividuoActual;
                        genesMejorSolucion.Clear();
                        genesMejorSolucion.AddRange(hordaNueva.getGenesIndividuo());
                    }

                }
            }

            poblacionIndividuos = generacion; //se sutituye la antigua poblacion con la nueva
            peorFitnessEncontrado = 1000f; // se vuelve a colocar el peor fitness a un numero que no llegaría ninguna solución, para adaptarlo al peor fitness de la siguiente generación
            fitnessTotalPoblacion = 0f; //se vuelve a 0, para 
        }
    }


    private Horda BuscarPadresConFitnessAceptable()
    {

        //En esta función, se busca un padre que tenga un valor fitness por encima de un valor (en este caso, la media de la población total) usando la selección por torneo. Para encontrarlo, se someterá un conjunto de la población escogida al azar
        //a un torneo, y aquellos que tengan un fitness mayor que el de la media se considerará un ganador y no será eliminado de la lista de posibles padres

        Horda progenitor;

        int indexGanador;

        float minimoFitness;

        int cantidadParticipantes = poblacionIndividuos.Count / tamanoTorneo; //cada torneo contendrá un cuarto de la poblacion actual de individuos que se enfrentará entre sí

        int contadorParticipantes = 0;

        List<Horda> participantesTorneo = new List<Horda>() { };

        List<Horda> ganadoresTorneo = new List<Horda>() { };

        if (poblacionIndividuos.Count == 0)
        {
            minimoFitness = fitnessTotalPoblacion / numAleatorio.Next(2, 5);
        }
        else
        {
            minimoFitness = fitnessTotalPoblacion / poblacionIndividuos.Count;
        }

        while (contadorParticipantes < cantidadParticipantes)
        {
            int participante = numAleatorio.Next(0, poblacionIndividuos.Count);

            participantesTorneo.Add(poblacionIndividuos[participante]);

            contadorParticipantes++;
        }

        //Con todos los participantes metidos en el grupo de posibles padres, se procede a eliminar aquellos que no pasen el corte

        ganadoresTorneo.AddRange(participantesTorneo);

        //Dado que se van a eliminar miembros de una lista que vamos a ir recorriendo, el IEnumerator va a dar fallo por estar manipulando una lista sobre la que se está iterando. 
        //Por eso, se copian los valores a otra y se modifica la lista con la copia

        foreach (Horda posiblePadre in participantesTorneo)
        {
            if (posiblePadre.getResultadoFitness() < minimoFitness)
            {
                ganadoresTorneo.Remove(posiblePadre);
            }
        }

        //Ahora que se tiene a los mejores de la generación, se procede a elegir uno al azar, y ese será el padre elegido para cruzarse

        if (ganadoresTorneo.Count >= 2)
        {
            indexGanador = numAleatorio.Next(0, ganadoresTorneo.Count);
            progenitor = ganadoresTorneo[indexGanador];
        }
        else
        {
            int padreAleatorio = numAleatorio.Next(0, poblacionIndividuos.Count); //si no hay ninguno que sea lo bastante bueno, se escoge un miembro aleatorio del grupo como padre

            progenitor = poblacionIndividuos[padreAleatorio];
        }



        /*foreach (Horda individuo in poblacionIndividuos)
        {

            if (individuo.getResultadoFitness() >= minimoFitness)
            {
                progenitor = individuo;
                break;
            }   
            
        }*/

        //necesitamos que haya un cruce sí o sí, así que si no se encuentra nada bueno, nos conformamos con uno cualquiera (como en la vida real)

        /*if (progenitor == null)
        {
            int padreAleatorio = numAleatorio.Next(0, poblacionIndividuos.Count);

            progenitor = poblacionIndividuos[padreAleatorio];
        }*/
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
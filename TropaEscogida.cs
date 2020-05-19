using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TropaEscogida {

    //Este atributo guarda la información de la tropa escogida por el jugador en el menú de selección. Es posible acceder a ella desde otros scripts y modificarlo según las unidades que el jugador quiera usar

    //Su valor por defecto será el que aparece nada más cargar la escena, pero podrá ser editado en cualquier momento antes de entrar en pelea
    private static List<int> tropa = new List<int>() { 1, 2, 3, 4 };

    private static List<int> hordaEnemiga = new List<int>();

    private static List<int> armasEnemigo = new List<int>();

    private static List<int> armasJugador = new List<int>() { 0, 0, 0, 0 };

    private static List<Consumible> consumibles = new List<Consumible>();

    private static List<int> tropaViva = new List<int>() {};

    private static int tipoAdvertencia = 0;

    public static List<int> GetTropaViva()
    {
        return tropaViva;
    }

    public static void SetTropaViva(List<int> tropaV)
    {
        tropaViva = tropaV;
    }

    public static int GetTipoAdvertencia()
    {
        return tipoAdvertencia;
    }

    public static void SetTipoAdvertencia(int tipo)
    {
        tipoAdvertencia = tipo;
    }

    public static List<Consumible> GetConsumibles()
    {
        return consumibles;
    }

    public static void SetConsumibles(List<Consumible> objetos)
    {
        consumibles = objetos;
    }

    public static List<int> GetTropa()
    {
        return tropa;
    }

    public static void SetTropa(List<int> value)
    {
        tropa = value;
    }

    public static List<int> GetHordaEnemiga()
    {

        return hordaEnemiga;
    }

    public static void setHordaEnemiga(List<int> nuevaHorda)
    {

        hordaEnemiga = nuevaHorda;

    }

    public static List<int> GetArmasEnemigas()
    {

        return armasEnemigo;
    }

    public static void SetArmasEnemigas(List<int> armas)
    {

        armasEnemigo = armas;

    }

    public static List<int> GetArmasJugador()
    {

        return armasJugador;
    }

    public static void SetArmasJugador(List<int> armas)
    {

        armasJugador = armas;

    }
}

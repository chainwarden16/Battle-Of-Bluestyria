using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma
{
    
    private string nombre;

    private int ataque;

    private int defensas;

    private int clase;

    private int rango;


    public Arma( string n, int a, int d, int c, int r)
    {

        nombre = n;

        ataque = a;

        defensas = d;

        clase = c;

        rango = r;

    }

    public string GetNombre()
    {

        return this.nombre;
    }

    public int GetAtaque()
    {

        return this.ataque;
    }

    public int GetClase()
    {

        return this.clase;
    }

    public int GetRango()
    {
        return rango;
    }

    public void SetRango(int ran)
    {
        rango = ran;
    }

    public int GetDefensas()
    {
        return defensas;
    }

    public void SetDefensas(int def)
    {
        defensas = def;
    }

    public static Arma CrearTomo()
    {
        Arma tomo = new Arma("Tomo", 3, 0, 1, 4);



        return tomo;

    }

    public static Arma CrearCodex()
    {
        Arma tomo = new Arma("Códex", 0, 1, 1, 4);

        return tomo;

    }

    public static Arma CrearLanza()
    {
        Arma lanza = new Arma("Lanza", 3, 0, 2, 2);

        return lanza;

    }

    public static Arma CrearPica()
    {
        Arma lanza = new Arma("Pica", 0, 1, 2, 2);

        return lanza;

    }

    public static Arma CrearArco()
    {
        Arma arco = new Arma("Arco", 4, 0, 4, 6);

        return arco;
    }

    public static Arma CrearPlatiun()
    {
        Arma arco = new Arma("Platiun", 0, 2, 4, 6);

        return arco;
    }

    public static Arma CrearBaculo()
    {

        Arma baculo = new Arma("Báculo", 2, 0, 3, 2);

        return baculo;
    }

    public static Arma CrearAngelus()
    {

        Arma baculo = new Arma("Ángelus", 0, 1, 3, 2);

        return baculo;
    }

}

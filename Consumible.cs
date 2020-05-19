using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumible
{

    private string nombre;

    private string descripcion;

    private int id;

    private int rango;

    public Consumible()
    {

    }

    public Consumible(string nombre, string descripcion, int id, int rango)
    {
        this.nombre = nombre;
        this.descripcion = descripcion;
        this.id = id;
        this.rango = rango;
    }

    public Consumible CrearPocion()
    {

        Consumible pocion = new Consumible("Poción", "Restaura 20 PS a una unidad.", 0, 2);

        return pocion;
    }

    public Consumible CrearTonico()
    {

        Consumible pocion = new Consumible("Tónico", "Elimina los estados alterados de una unidad.", 1, 2);

        return pocion;
    }

    public Consumible CrearElixir()
    {

        Consumible pocion = new Consumible("Elixir", "Restaura todos los PS y cura los estados alterados de una unidad.", 2, 2);

        return pocion;
    }

    public string GetNombre()
    {
        return nombre;
    }

    public string GetDescripcion()
    {
        return descripcion;
    }

    public int GetID()
    {
        return id;
    }

    public int GetRango()
    {
        return rango;
    }

}

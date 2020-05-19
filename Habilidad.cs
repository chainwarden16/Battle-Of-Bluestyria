using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum TipoHabilidad
    {
        Físico,
        Mágico,
        Apoyo
    };

public class Habilidad
{

    private string nombre;

    private int dano;

    private string descripcion;

    private int coste;

    private int clase;

    private TipoHabilidad tipo;

    private int rango;

    private int id;

    public Habilidad(int id, string n, int d, string des, int co, int cl, TipoHabilidad ti, int ran)
    {
        this.id = id;

        nombre = n;

        dano = d;

        descripcion = des;

        coste = co;

        clase = cl;

        tipo = ti;

        rango = ran;
    }

    public int GetID()
    {
        return id;
    }

    public void SetID(int idNuevo)
    {
        id = idNuevo;
    }

    public int GetRango()
    {
        return rango;
    }

    public string GetNombre()
    {

        return this.nombre;
    }

    public int GetDano()
    {

        return this.dano;
    }

    public int GetCoste()
    {

        return this.coste;
    }

    public int GetClase()
    {

        return this.clase;
    }

    public TipoHabilidad GetTipo()
    {

        return this.tipo;
    }

    public string GetDescripcion()
    {
        return descripcion;
    }

    public static List<Habilidad> CrearHabilidades(int num)
    {
        List<Habilidad> habili = new List<Habilidad>();

        switch (num)
        {
            case 1: //habilidades del mago
                Habilidad h1 = new Habilidad(0, "Piro",6, "Invoca una bola de fuego que daña a un enemigo.", 7, 1, (TipoHabilidad) Enum.Parse(typeof(TipoHabilidad), "Mágico"), 6);
                Habilidad h2 = new Habilidad(1, "Aqua", 8, "Usa el poder del agua para atacar a un oponente.", 8, 1, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Mágico"), 6);
                Habilidad h3 = new Habilidad(2, "Electro", 5, "Electrocuta a una unidad enemiga.", 6, 1, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Mágico"), 6);
                Habilidad h13 = new Habilidad(12, "Envenenar", 0, "Produce daño indirecto que crece con cada turno. Dura 4 turnos.", 7, 1, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Mágico"), 4);
                Habilidad h14 = new Habilidad(13,"Quemar", 0, "Produce daño indirecto de 3 PS durante 4 turnos.", 5, 1, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Mágico"), 4);
                Habilidad h15 = new Habilidad(14,"Dormir", 0, "Evita que un enemigo se mueva hasta que lo golpeen.", 8, 1, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Mágico"), 4);
                Habilidad h16 = new Habilidad(15, "Paralizar", 0, "El objetivo enemigo no se moverá durante 4 turnos.", 5, 1, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Mágico"), 4);

                
                //TODO: Meter una diferencia entre dormido y paralizado

                habili.Add(h1);
                habili.Add(h2);
                habili.Add(h3);
                habili.Add(h13);
                habili.Add(h14);
                habili.Add(h15);
                habili.Add(h16);

                break;

            case 2: //habilidades del lancero

                Habilidad h4 = new Habilidad(3, "Estocada", 8, "Ataca a un oponente usando la lanza.", 5, 2, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Físico"), 3);
                Habilidad h5 = new Habilidad(4, "Placaje", 6, "Golpea a un oponente con una embestida.", 4, 2, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Físico"), 3);
                Habilidad h18 = new Habilidad(17, "Escudar", 0, "Protege a una unidad de todo daño directo durante un turno.", 3, 2, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Apoyo"), 3);

                habili.Add(h4);
                habili.Add(h5);
                habili.Add(h18);

                break;

            case 3: //habilidades del clerigo

                Habilidad h8 = new Habilidad(7, "Curar", 0, "Restaura algo de PS a una unidad aliada.", 0, 3, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Apoyo"), 4);
                Habilidad h9 = new Habilidad(8, "Fortificar", 0, "Aumenta la Defensa y Def. Mágica de un aliado durante 3 turnos.", 0, 3, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Apoyo"), 4);
                Habilidad h10 = new Habilidad(9, "Reforzar", 0, "Aumenta el Ataque y Magia de un aliado durante 3 turnos.", 0, 3, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Apoyo"), 4);
                Habilidad h11 = new Habilidad(10, "Debilitar", 0, "Reduce la Defensa y Def. Mágica de un enemigo durante 3 turnos.", 0, 3, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Mágico"), 4);
                Habilidad h12 = new Habilidad(11, "Cansar", 0, "Reduce el Ataque y Magia de un enemigo durante 3 turnos", 0, 3, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Mágico"), 4);
                Habilidad h17 = new Habilidad(16, "Purificar", 0, "Elimina todos los cambios de estado de una unidad.", 0, 1, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Apoyo"), 4);


                habili.Add(h8);
                habili.Add(h9);
                habili.Add(h10);
                habili.Add(h11);
                habili.Add(h12);
                habili.Add(h17);

                break;

            case 4: //habilidades del arquero

                Habilidad h6 = new Habilidad(5, "¡Diana!",9,"Un disparo certero que daña a un oponente.",7, 4, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Físico"), 4);
                Habilidad h7 = new Habilidad(6, "Doble disparo", 7, "Dos flechas golpean a un mismo enemigo.", 5, 4, (TipoHabilidad)Enum.Parse(typeof(TipoHabilidad), "Físico"), 4);

                habili.Add(h6);
                habili.Add(h7);

                break;

            default:

                break;
        }

        return habili;

    }

}

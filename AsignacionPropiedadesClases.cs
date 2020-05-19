using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsignacionPropiedadesClases : MonoBehaviour
{

    private static List<string> posiblesNombres = new List<string>() { "Laur", "Frisken", "Noreh", "Irisir", "Hagaew", "Najore", "Lokceri", "Philak", "Rosemer", "Darekn", "Ostreit", "Ghoulef", "Hufderr", "Raral", "Wunaak", "Frestolag", "Feiranm", "Mosretieur" };

    private static System.Random rng = new System.Random();

    public static void AsignarAtributosUnidadAliada(GameObject go, int tipoUnidad)
    {
        

        List<Habilidad> h;

        if (tipoUnidad == 1) //mago
        {

            
            h = Habilidad.CrearHabilidades(1);

            go.GetComponent<Unidad>().SetEquipo(0);
            go.GetComponent<Clase>().SetPrioridadOrdenCPU(2);
            go.GetComponent<Unidad>().SetEstaDefendiendo(false);
            string n = setNombreUnidad();
            go.GetComponent<Unidad>().SetNombre(n);
            go.GetComponent<Unidad>().setTurnoAcabado(false);
            go.GetComponent<Clase>().SetPSAct(50);
            go.GetComponent<Clase>().SetPSMax(50);
            go.GetComponent<Clase>().SetAgilidad(9); //9
            go.GetComponent<Clase>().SetDefensaM(4);
            go.GetComponent<Clase>().SetDefensa(3);
            go.GetComponent<Clase>().SetMagia(12);
            go.GetComponent<Clase>().SetTipoClase(1);
            go.GetComponent<Clase>().SetHabilidades(h);
            go.GetComponent<Clase>().SetAtaque(2);

        }
        else if (tipoUnidad == 2) //lancero
        {
            h = Habilidad.CrearHabilidades(2);
            go.GetComponent<Clase>().SetPrioridadOrdenCPU(3);
            go.GetComponent<Unidad>().SetEquipo(0);
            go.GetComponent<Unidad>().SetEstaDefendiendo(false);
            string n = setNombreUnidad();
            go.GetComponent<Unidad>().SetNombre(n);
            go.GetComponent<Unidad>().setTurnoAcabado(false);
            go.GetComponent<Clase>().SetPSAct(80);
            go.GetComponent<Clase>().SetPSMax(80);
            go.GetComponent<Clase>().SetAgilidad(7); //7
            go.GetComponent<Clase>().SetDefensaM(10);
            go.GetComponent<Clase>().SetDefensa(10);
            go.GetComponent<Clase>().SetMagia(1);
            go.GetComponent<Clase>().SetTipoClase(2);
            go.GetComponent<Clase>().SetHabilidades(h);
            go.GetComponent<Clase>().SetAtaque(4);
        }
        else if (tipoUnidad == 3) //clerigo
        {
            h = Habilidad.CrearHabilidades(3);
            go.GetComponent<Clase>().SetPrioridadOrdenCPU(4);
            go.GetComponent<Unidad>().SetEquipo(0);
            go.GetComponent<Unidad>().SetEstaDefendiendo(false);
            string n = setNombreUnidad();
            go.GetComponent<Unidad>().SetNombre(n);
            go.GetComponent<Unidad>().setTurnoAcabado(false);
            go.GetComponent<Clase>().SetPSAct(35);
            go.GetComponent<Clase>().SetPSMax(35);
            go.GetComponent<Clase>().SetAgilidad(7); //7
            go.GetComponent<Clase>().SetDefensaM(2);
            go.GetComponent<Clase>().SetDefensa(1);
            go.GetComponent<Clase>().SetMagia(3);
            go.GetComponent<Clase>().SetTipoClase(3);
            go.GetComponent<Clase>().SetHabilidades(h);
            go.GetComponent<Clase>().SetAtaque(2);
        }
        else if (tipoUnidad == 4)
        { //arquero
            h = Habilidad.CrearHabilidades(4);
            go.GetComponent<Clase>().SetPrioridadOrdenCPU(1);
            go.GetComponent<Unidad>().SetEquipo(0);
            go.GetComponent<Unidad>().SetEstaDefendiendo(false);
            string n = setNombreUnidad();
            go.GetComponent<Unidad>().SetNombre(n);
            go.GetComponent<Unidad>().setTurnoAcabado(false);
            go.GetComponent<Clase>().SetPSAct(40);
            go.GetComponent<Clase>().SetPSMax(40);
            go.GetComponent<Clase>().SetAgilidad(9); //9
            go.GetComponent<Clase>().SetDefensaM(1);
            go.GetComponent<Clase>().SetDefensa(2);
            go.GetComponent<Clase>().SetMagia(1);
            go.GetComponent<Clase>().SetTipoClase(4);
            go.GetComponent<Clase>().SetHabilidades(h);
            go.GetComponent<Clase>().SetAtaque(10);
        }

    }


    public static string setNombreUnidad()
    {
        string nombre;
        int nombreAleatorio = Mathf.FloorToInt(rng.Next(0, posiblesNombres.Count-1));
        nombre = posiblesNombres[nombreAleatorio];
        posiblesNombres.RemoveAt(nombreAleatorio);
        return nombre;

    }

    public static void AsignarAtributosUnidadEnemiga(GameObject go, int tipoUnidad)
    {

        List<Habilidad> h;

        if (tipoUnidad == 1) //mago
        {

            
            h = Habilidad.CrearHabilidades(1);
            go.GetComponent<Clase>().SetPrioridadOrdenCPU(2);
            go.GetComponent<Unidad>().SetEquipo(1);
            go.GetComponent<Unidad>().SetEstaDefendiendo(false);
            string n = setNombreUnidad();
            go.GetComponent<Unidad>().SetNombre(n);
            go.GetComponent<Unidad>().setTurnoAcabado(false);
            go.GetComponent<Clase>().SetPSAct(45);
            go.GetComponent<Clase>().SetPSMax(45);
            go.GetComponent<Clase>().SetAgilidad(9);
            go.GetComponent<Clase>().SetDefensaM(4);
            go.GetComponent<Clase>().SetDefensa(3);
            go.GetComponent<Clase>().SetMagia(11);
            go.GetComponent<Clase>().SetTipoClase(1);
            go.GetComponent<Clase>().SetHabilidades(h);
            go.GetComponent<Clase>().SetAtaque(2);

        }
        else if (tipoUnidad == 2) //lancero
        {
            h = Habilidad.CrearHabilidades(2);
            go.GetComponent<Clase>().SetPrioridadOrdenCPU(3);
            go.GetComponent<Unidad>().SetEquipo(1);
            go.GetComponent<Unidad>().SetEstaDefendiendo(false);
            string n = setNombreUnidad();
            go.GetComponent<Unidad>().SetNombre(n);
            go.GetComponent<Unidad>().setTurnoAcabado(false);
            go.GetComponent<Clase>().SetPSAct(70);
            go.GetComponent<Clase>().SetPSMax(70);
            go.GetComponent<Clase>().SetAgilidad(7);
            go.GetComponent<Clase>().SetDefensaM(9);
            go.GetComponent<Clase>().SetDefensa(9);
            go.GetComponent<Clase>().SetMagia(1);
            go.GetComponent<Clase>().SetTipoClase(2);
            go.GetComponent<Clase>().SetHabilidades(h);
            go.GetComponent<Clase>().SetAtaque(5);
        }
        else if (tipoUnidad == 3) //clerigo
        {

            h = Habilidad.CrearHabilidades(3);
            go.GetComponent<Clase>().SetPrioridadOrdenCPU(4);
            go.GetComponent<Unidad>().SetEquipo(1);
            go.GetComponent<Unidad>().SetEstaDefendiendo(false);
            string n = setNombreUnidad();
            go.GetComponent<Unidad>().SetNombre(n);
            go.GetComponent<Unidad>().setTurnoAcabado(false);
            go.GetComponent<Clase>().SetPSAct(35);
            go.GetComponent<Clase>().SetPSMax(35);
            go.GetComponent<Clase>().SetAgilidad(7);

            go.GetComponent<Clase>().SetDefensaM(2);
            go.GetComponent<Clase>().SetDefensa(2);
            go.GetComponent<Clase>().SetMagia(3);
            go.GetComponent<Clase>().SetTipoClase(3);
            go.GetComponent<Clase>().SetHabilidades(h);
            go.GetComponent<Clase>().SetAtaque(2);
        }
        else if (tipoUnidad == 4)
        { //arquero

            h = Habilidad.CrearHabilidades(4);
            go.GetComponent<Clase>().SetPrioridadOrdenCPU(1);
            go.GetComponent<Unidad>().SetEquipo(1);
            go.GetComponent<Unidad>().SetEstaDefendiendo(false);
            string n = setNombreUnidad();
            go.GetComponent<Unidad>().SetNombre(n);
            go.GetComponent<Unidad>().setTurnoAcabado(false);
            go.GetComponent<Clase>().SetPSAct(40);
            go.GetComponent<Clase>().SetPSMax(40);
            go.GetComponent<Clase>().SetAgilidad(9);

            go.GetComponent<Clase>().SetDefensaM(1);
            go.GetComponent<Clase>().SetDefensa(2);
            go.GetComponent<Clase>().SetMagia(1);
            go.GetComponent<Clase>().SetTipoClase(4);
            go.GetComponent<Clase>().SetHabilidades(h);
            go.GetComponent<Clase>().SetAtaque(9);
        }

    }

}

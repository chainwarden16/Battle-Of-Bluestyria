using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unidad : MonoBehaviour
{
    private int equipo;
    private string nombre;
    private bool turnoAcabado;
    private bool estaDefendiendo;
    
     //0 = abajo, 1 = izquierda, 2 = arriba, 3 = derecha. Empieza mirando hacia arriba por defecto si es un heroe y hacia abajo si es un enemigo
    private List<int> estados = new List<int>(); // 0 - envenenado, 1 - quemado, 2 - paralizado, 3 - dormido
    private List<int> buffs = new List<int>(); // 0 - Fortificar (DEF), 1 - Reforzar (FUE/MAG), 2 - Debilitar (DEF), 3- Cansar (ATA/MAG)
    private List<Vector3> posiblesPosicionesActuales;
    private int contadorDuracionBuff;
    private int contadorDuracionEstado;
    private int contadorDañoVeneno = 0;
    private Tuple<bool, GameObject> estaSiendoEscudado = new Tuple<bool, GameObject>(false, null);

    //

    #region Getters y Setters para el combate

    public int GetContadorVeneno()
    {
        return contadorDañoVeneno;
    }

    public void SetContadorVeneno(int veneno)
    {
        contadorDañoVeneno = veneno;
    }


    public Tuple<bool, GameObject> GetEstaSiendoEscudado()
    {
        return estaSiendoEscudado;
    }

    public void SetEstaSiendoEscudado(Tuple<bool, GameObject> escudado)
    {
        estaSiendoEscudado = escudado;
    }

    public int GetContadorDuracionBuff()
    {
        return contadorDuracionBuff;
    }

    public void SetContadorDuracionBuff(int duracion)
    {
        contadorDuracionBuff = duracion;
    }

    public int GetContadorDuracionEstado()
    {
        return contadorDuracionEstado;
    }

    public void SetContadorDuracionEstado(int duracion)
    {
        contadorDuracionEstado = duracion;
    }


    public List<Vector3> GetPosiblesPosicionesActuales()
    {
        return posiblesPosicionesActuales;
    }

    public void SetPosiblesPosicionesActuales(List<Vector3> mov)
    {
        posiblesPosicionesActuales = mov;
    }

    public bool getTurnoAcabado()
    {
        return turnoAcabado;
    }

    public void setTurnoAcabado(bool turnoAcabado)
    {

        this.turnoAcabado = turnoAcabado;
    }

    public bool GetEstaDefendiendo()
    {
        return estaDefendiendo;
    }

    public void SetEstaDefendiendo(bool def)
    {
        estaDefendiendo = def;
    }

    public List<int> GetEstados()
    {
        return estados;
    }

    public void SetEstados(List<int> estados)
    {
        this.estados = estados;
    }

    public List<int> GetBuffs()
    {
        return buffs;
    }

    public void SetBuffs(List<int> b)
    {
        buffs = b;
    }


    #endregion

    #region Getters y Setters de animaciones

    

    public void SetEstaCaminando(bool camina, Animator animator)
    {
        animator.SetBool("estaCaminando", camina);
    }


    public void SetEstaAtacando(bool camina, Animator animator)
    {
        animator.SetBool("estaAtacando", camina);
    }

    

    public void SetHaGanado(bool camina, Animator animator)
    {
        animator.SetBool("haGanado", camina);
    }

    public void SetDireccionAMirar(int dire, Animator animator)
    {
        animator.SetInteger("direccionAMirar", dire);
    }

    public int GetDireccionAMirar(Animator animator)
    {
        return animator.GetInteger("direccionAMirar");
    }

    public bool GetHaGanado(Animator animator)
    {
        return animator.GetBool("haGanado");
    }

    public bool GetEstaCaminando(Animator animator)
    {
        return animator.GetBool("estaCaminando");
    }

    public bool GetEstaAtacando(Animator animator)
    {
        return animator.GetBool("estaAtacando");
    }

    #endregion

    #region Getters y Setters para manipular la unidad

    public int getEquipo()
    {

        return equipo;
    }

    public string getNombre()
    {

        return nombre;

    }

    public void SetEquipo(int e)
    {
        equipo = e;
    }

 

    public void SetNombre(string n)
    {
        nombre = n;
    }

    #endregion

    
}
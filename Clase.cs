using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clase : MonoBehaviour

{
    protected int tipoClase;

    protected int psMax;

    protected int psAct;

    protected int defensa;

    protected int defensaM;

    protected int ataque;

    protected int magia;

    protected int agilidad;

    protected List<Habilidad> habilidades = new List<Habilidad>();

    protected Arma arma;

    private int prioridadOrdenCPU; //cuanto más alto, antes se moverá la unidad en el turno de la CPU

    public int GetPrioridadOrdenCPU()
    {
        return prioridadOrdenCPU;
    }

    public void SetPrioridadOrdenCPU(int prioridad)
    {
        prioridadOrdenCPU = prioridad;
    }

    public int GetTipoClase()
    {

        return this.tipoClase;
    }

    public void SetTipoClase(int cl)
    {
        tipoClase = cl;
    }

    public int GetPSMax()
    {

        return this.psMax;
    }

    public void SetPSMax(int ps)
    {
        psMax = ps;
    }

    public int GetPSAct()
    {

        return this.psAct;
    }

    public void SetPSAct(int vida)
    {

        psAct = vida;
    }

    public int GetDefensa()
    {

        return this.defensa;
    }

    public void SetDefensa(int de)
    {
        defensa = de;
    }

    public int GetDefensaM()
    {

        return this.defensaM;
    }

    public void SetDefensaM(int dm)
    {
        defensaM = dm;
    }

    public int GetAtaque()
    {

        return this.ataque;
    }

    public void SetAtaque(int at)
    {
        ataque = at;
    }

    public int GetMagia()
    {

        return this.magia;
    }

    public void SetMagia(int m)
    {
        magia = m;
    }

    public int GetAgilidad()
    {

        return this.agilidad;
    }

    public void SetAgilidad(int agi)
    {
        agilidad = agi;
    }

    public List<Habilidad> GetHabilidades()
    {

        return this.habilidades;
    }

    public void SetHabilidades(List<Habilidad> habi)
    {
        habilidades = habi;
    }

    public Arma GetArma()
    {

        return this.arma;
    }

    public void SetArma(Arma ar)
    {
        arma = ar;
    }
    
}

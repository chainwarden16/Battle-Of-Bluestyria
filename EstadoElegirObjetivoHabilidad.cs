using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


public class EstadoElegirObjetivoHabilidad : Estado
{
    private System.Random random = new System.Random();
    private GameObject cursorHabilidad;

    private int psActuales;

    private GameObject cursor;

    private GameObject atacante;

    private static GameObject objetivo;

    private GameObject fondoDescripcion;

    private GameObject[] fondoHabilidades;

    private Habilidad hab;

    private MaquinaDeEstados maquina = IniciacionCombate.GetMaquinaDeEstados();

    private bool reseteado = false;

    private GameManager gm = GameManager.InstanciarGameManager();

    private Animator animator;

    private GameObject popUp;

    private bool sePuedePulsarTecla = true;

    private static AudioSource audioSource;
    private static AudioClip cursorSFX;

    private static Animator animacionesAtaques;

    private static GameObject objetoAnimaciones;

    private AnimatorClipInfo[] info;

    //========

    private GameObject unidadAMover;

    private GameObject cursorMenu;

    private GameObject botonAtacar;

    private GameObject botonDefender;

    private GameObject botonHabilidad;

    private GameObject botonObjetos;

    private GameObject camara;
    public EstadoElegirObjetivoHabilidad(CombatePorTurnos comba) : base(comba)
    {

    }

    public override IEnumerator StartState()
    {
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        objetoAnimaciones = GameObject.Find("AnimacionesCombate");
        animacionesAtaques = objetoAnimaciones.GetComponent<Animator>();
        if (reseteado == false)
        {


            hab = EstadoHabilidad.GetHabilidadElegida();
            cursorHabilidad = GameObject.Find("Flecha-Lista-Habilidades");

            fondoDescripcion = GameObject.Find("Fondo-Descripcion");

            fondoHabilidades = GameObject.FindGameObjectsWithTag("FondoHabilidad");

            cursor = GameObject.Find("Cursor");

            atacante = EstadoEsperar.GetUnidadSeleccionada();

            animator = atacante.GetComponent<Animator>();

            popUp = GameObject.Find("NumeroPopUp");

            //====

            cursorMenu = GameObject.Find("Flecha-Menu-Unidad");

            botonAtacar = GameObject.Find("Boton-Atacar");

            botonDefender = GameObject.Find("Boton-Habilidad");

            botonHabilidad = GameObject.Find("Boton-Defender");

            botonObjetos = GameObject.Find("Boton-Objetos");

            unidadAMover = EstadoEsperar.GetUnidadSeleccionada();

            camara = GameObject.Find("Main Camera");

            reseteado = true;
        }
        if (sePuedePulsarTecla)
        {
            GameManager.MoverElCursor();
        }
        

        if (Input.GetKeyUp(KeyCode.C) && GameManager.HayEnemigoEnCelda(cursor.transform.position) && sePuedePulsarTecla && GameManager.GetListaCeldasHabilidad().Contains(cursor.transform.position))
        {
            sePuedePulsarTecla = false;
            objetivo = GameManager.SeleccionarUnidad(cursor.transform.position);
            GameManager.CambiarDireccionUnidad(atacante);
            atacante.GetComponent<Unidad>().SetEstaAtacando(true, animator);
            GameManager.BorrarCasillas();

            switch (hab.GetID())
            {

                case 0: //Piro

                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());

                        GameManager.ReproducirSonido("Audio/Magic1");

                        GameManager.ReproducirAnimacion(18, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Fire1");

                        GameManager.ReproducirAnimacion(0, objetivo);

                        GameManager.AtacarUnidadHabilidadMagica(atacante, objetivo, hab);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.EliminarPopUp(popUp);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }


                    break;

                case 1: //Aqua


                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());

                        GameManager.ReproducirSonido("Audio/Magic1");

                        GameManager.ReproducirAnimacion(18, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Water1");

                        GameManager.ReproducirAnimacion(1, objetivo);

                        GameManager.AtacarUnidadHabilidadMagica(atacante, objetivo, hab);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                       

                        GameManager.EliminarPopUp(popUp);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }

                    break;

                case 2: //Electro

                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());

                        GameManager.ReproducirSonido("Audio/Magic1");

                        GameManager.ReproducirAnimacion(18, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Thunder1");

                        GameManager.ReproducirAnimacion(2, objetivo);

                        GameManager.AtacarUnidadHabilidadMagica(atacante, objetivo, hab);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.EliminarPopUp(popUp);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }

                    break;

                case 3: // Estocada
                   
                    
                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());

                        GameManager.ReproducirSonido("Audio/Skill1");

                        GameManager.ReproducirAnimacion(19, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Damage1");

                        GameManager.ReproducirAnimacion(3, objetivo);

                        GameManager.AtacarUnidadHabilidadFisica(atacante, objetivo, hab);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);


                        GameManager.EliminarPopUp(popUp);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }

                    break;

                case 4: //Placaje

                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());

                        GameManager.ReproducirSonido("Audio/Skill1");

                        GameManager.ReproducirAnimacion(19, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Blow3");

                        GameManager.ReproducirAnimacion(4, objetivo);

                        GameManager.AtacarUnidadHabilidadFisica(atacante, objetivo, hab);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);


                        GameManager.EliminarPopUp(popUp);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }

                    break;

                case 5: //Diana

                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());

                        GameManager.ReproducirSonido("Audio/Skill1");

                        GameManager.ReproducirAnimacion(19, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Crossbow");

                        GameManager.ReproducirAnimacion(5, objetivo);

                        GameManager.AtacarUnidadHabilidadFisica(atacante, objetivo, hab);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);


                        GameManager.EliminarPopUp(popUp);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }


                    break;

                case 6: //Doble disparo

                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());

                        GameManager.ReproducirSonido("Audio/Skill1");

                        GameManager.ReproducirAnimacion(19, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Crossbow");

                        GameManager.ReproducirAnimacion(6, objetivo);

                        GameManager.AtacarUnidadHabilidadFisica(atacante, objetivo, hab);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);


                        GameManager.EliminarPopUp(popUp);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }


                    break;


                case 11: //Cansar - 3
                    
                    if (GameManager.ComprobarInflingirBuff(objetivo))
                    {
                        GameManager.ReproducirSonido("Audio/Magic1");
                        GameManager.ReproducirAnimacion(18, atacante);

                        AnimatorClipInfo[] info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);

                        //Se vuelve al estado inicial

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        //Se hace la animacion especifica
                        GameManager.ReproducirSonido("Audio/Down2");
                        GameManager.ReproducirAnimacion(11, objetivo);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length+1f);

                        //Se vuelve al estado inicial

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.InflingirBuff(objetivo, 3, 2);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);
                    }

                    //TODO: mensaje de aviso de perdida de turno para cuando no se pueda realizar esta accion

                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);
                        maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));
                    }


                    break;

                case 12: //envenenar
                    
                    
                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        
                        if (GameManager.ComprobarInflingirCambioEstadoDañino(objetivo))
                        {

                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, atacante);

                            AnimatorClipInfo[] info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length + 1f);

                            //Se vuelve al estado inicial

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);

                            //Se hace la animacion especifica
                            GameManager.ReproducirSonido("Audio/Pollen");
                            GameManager.ReproducirAnimacion(12, objetivo);

                            info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length+1f);

                            //Se vuelve al estado inicial

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);

                            GameManager.InflingirEstadoDañino(objetivo, 0, 3);

                            atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());
                            GameManager.BorrarCasillas();
                            GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                            //MaquinaDeEstados.SetEstado(new EstadoEsperar(combatePorTurnos));
                        }

                        //TODO: mensaje de aviso de perdida de turno para cuando no se pueda realizar esta accion

                        else
                        {

                            gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);
                            maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));

                        }

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }

                    break;

                case 13: //quemar
                   
                    
                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        objetivo = GameManager.SeleccionarUnidad(cursor.transform.position);
                        if (GameManager.ComprobarInflingirCambioEstadoDañino(objetivo))
                        {

                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, atacante);

                            AnimatorClipInfo[] info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length + 1f);

                            //Se vuelve al estado inicial

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);

                            //Se hace la animacion especifica
                            GameManager.ReproducirSonido("Audio/Fire2");
                            GameManager.ReproducirAnimacion(13, objetivo);

                            info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length+1f);

                            //Se vuelve al estado inicial

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);

                            GameManager.InflingirEstadoDañino(objetivo, 1, 3);

                            atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());
                            GameManager.BorrarCasillas();
                            GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                            
                        }

                        else
                        {

                            gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);
                            maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));

                        }

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }
                    break;

                case 14: //dormir
                    
                    
                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        objetivo = GameManager.SeleccionarUnidad(cursor.transform.position);
                        if (GameManager.ComprobarInflingirCambioEstadoDañino(objetivo))
                        {

                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, atacante);

                            AnimatorClipInfo[] info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length + 1f);

                            //Se vuelve al estado inicial

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);

                            //Se hace la animacion especifica
                            GameManager.ReproducirSonido("Audio/Sleep");
                            GameManager.ReproducirAnimacion(14, objetivo);

                            info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length+1f);

                            //Se vuelve al estado inicial

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);

                            GameManager.InflingirEstadoDañino(objetivo, 3, 9999);

                            atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());
                            GameManager.BorrarCasillas();
                            GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                            //MaquinaDeEstados.SetEstado(new EstadoEsperar(combatePorTurnos));
                        }

                        //TODO: mensaje de aviso de perdida de turno para cuando no se pueda realizar esta accion

                        else
                        {

                            gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);
                            maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));

                        }

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }
                    break;

                case 15: //paralizar
                    
                    
                    if (atacante.GetComponent<Clase>().GetPSAct() > hab.GetCoste())
                    {

                        objetivo = GameManager.SeleccionarUnidad(cursor.transform.position);
                        if (GameManager.ComprobarInflingirCambioEstadoDañino(objetivo))
                        {

                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, atacante);

                            AnimatorClipInfo[] info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length + 1f);

                            //Se vuelve al estado inicial

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);

                            //Se hace la animacion especifica
                            GameManager.ReproducirSonido("Audio/Leakage");
                            GameManager.ReproducirAnimacion(15, objetivo);

                            info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length+1f);

                            //Se vuelve al estado inicial

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);

                            GameManager.InflingirEstadoDañino(objetivo, 2, 1);

                            atacante.GetComponent<Clase>().SetPSAct(atacante.GetComponent<Clase>().GetPSAct() - hab.GetCoste());
                            GameManager.BorrarCasillas();
                            GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                            
                        }

                        //TODO: mensaje de aviso de perdida de turno para cuando no se pueda realizar esta accion

                        else
                        {

                            gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);
                            maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));

                        }

                    }
                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }
                    break;

                case 10: //Debilitar - 2
                    
                    if (GameManager.ComprobarInflingirBuff(objetivo))
                    {
                        GameManager.ReproducirSonido("Audio/Magic1");
                        GameManager.ReproducirAnimacion(18, atacante);

                        AnimatorClipInfo[] info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);

                        //Se vuelve al estado inicial

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        //Se hace la animacion especifica
                        GameManager.ReproducirSonido("Audio/Down1");
                        GameManager.ReproducirAnimacion(10, objetivo);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length+1f);

                        //Se vuelve al estado inicial

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);


                        GameManager.InflingirBuff(objetivo, 2, 2);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);
                    }


                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);
                        maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));
                    }


                    break;
                
                default:

                    break;

            }
            atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
            atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);
        }

        else if (Input.GetKeyUp(KeyCode.C) && GameManager.HayAliadoEnCelda(cursor.transform.position) && sePuedePulsarTecla && GameManager.GetListaCeldasHabilidad().Contains(cursor.transform.position))
        {
            sePuedePulsarTecla = false;
            GameManager.CambiarDireccionUnidad(atacante);
            
            atacante.GetComponent<Unidad>().SetEstaAtacando(true, animator);
            
            objetivo = GameManager.SeleccionarUnidad(cursor.transform.position);
            Clase clase = atacante.GetComponent<Clase>();
            GameManager.BorrarCasillas();
            switch (hab.GetID())
            {
                case 7: //Curar
                    

                    GameManager.ReproducirSonido("Audio/Magic1");
                    GameManager.ReproducirAnimacion(18, atacante);

                    info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                    yield return new WaitForSeconds(info[0].clip.length+1f);
                    //Se invoca el numero que indica cuánto se va a curar

                    animacionesAtaques.SetInteger("ID", -1);
                    yield return new WaitForSeconds(0.1f);

                    GameManager.ReproducirSonido("Audio/Recovery");
                    GameManager.ReproducirAnimacion(7, objetivo);

                    GameManager.CurarVidaUnidad(objetivo, random.Next(10, 20));
                    info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                    yield return new WaitForSeconds(info[0].clip.length + 1f);
                    //yield return new WaitForSeconds(2f);
                    animacionesAtaques.SetInteger("ID", -1);
                    GameManager.EliminarPopUp(popUp);
                    GameManager.BorrarCasillas();
                    GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                    break;

                case 8:

                    //Fortificar - 0
                    
                    
                    if (GameManager.ComprobarInflingirBuff(objetivo))
                    {
                        GameManager.ReproducirSonido("Audio/Magic1");
                        GameManager.ReproducirAnimacion(18, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Up4");
                        GameManager.ReproducirAnimacion(8, objetivo);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length+1);

                        GameManager.InflingirBuff(objetivo, 0, 2);

                        animacionesAtaques.SetInteger("ID", -1);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);
                    }

                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);
                        maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));
                    }


                    break;

                case 9: //Reforzar - 1
                    
                    

                    if (GameManager.ComprobarInflingirBuff(objetivo))
                    {
                        GameManager.ReproducirSonido("Audio/Magic1");
                        GameManager.ReproducirAnimacion(18, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Up1");
                        GameManager.ReproducirAnimacion(9, objetivo);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length+1);

                        GameManager.InflingirBuff(objetivo, 1, 2);

                        animacionesAtaques.SetInteger("ID", -1);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);
                    }

                    

                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);
                        maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));
                    }

                    break;

                case 16: //Purificar
                   

                    if (objetivo.GetComponent<Unidad>().GetEstados().Count != 0)
                    {
                        GameManager.ReproducirSonido("Audio/Magic1");
                        GameManager.ReproducirAnimacion(18, atacante);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length + 1f);
                        //Se invoca el numero que indica cuánto se va a curar

                        animacionesAtaques.SetInteger("ID", -1);
                        yield return new WaitForSeconds(0.1f);

                        GameManager.ReproducirSonido("Audio/Heal2");
                        GameManager.ReproducirAnimacion(16, objetivo);

                        info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                        yield return new WaitForSeconds(info[0].clip.length+1f);

                        GameManager.EliminarEstadosDañinosUnidad(objetivo);

                        animacionesAtaques.SetInteger("ID", -1);
                        GameManager.BorrarCasillas();
                        GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);
                    }

                    //TODO: mensaje de aviso de perdida de turno para cuando no se pueda realizar esta accion

                    else
                    {

                        gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);


                        maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));

                    }

                    break;

                case 17: //Escudar

                    Unidad objetivoAEscudar = objetivo.GetComponent<Unidad>();

                    if(clase.GetPSAct() > clase.GetHabilidades()[2].GetCoste())
                    {

                        if (objetivoAEscudar.GetEstaSiendoEscudado().Item2 == null)
                        {

                            GameManager.ReproducirSonido("Audio/Magic1");
                            GameManager.ReproducirAnimacion(18, atacante);

                            info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length + 1f);
                            //Se invoca el numero que indica cuánto se va a curar

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);


                            GameManager.ReproducirSonido("Audio/Saint2");
                            GameManager.ReproducirAnimacion(17, objetivo);
                            info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                            yield return new WaitForSeconds(info[0].clip.length + 1f);
                            //Se invoca el numero que indica cuánto se va a curar

                            animacionesAtaques.SetInteger("ID", -1);
                            yield return new WaitForSeconds(0.1f);
                            psActuales = clase.GetPSAct() - clase.GetHabilidades()[2].GetCoste();
                            clase.SetPSAct(psActuales);
                            objetivo.GetComponent<Unidad>().SetEstaSiendoEscudado(new Tuple<bool, GameObject>(true, atacante));
                            GameManager.BorrarCasillas();
                            GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);

                        }
                        else
                        {
                            gm.MostrarMensajePerdidaTurno("No tuvo ningún efecto...", objetivo);
                            maquina.SetEstado(new EstadoPerdidaTurno(combatePorTurnos));

                        }

                    }
                    else
                    {
                        gm.MostrarMensajePerdidaTurno("No tienes suficientes PS para realizar esta acción.", objetivo);

                        maquina.SetEstado(new EstadoAccionNoPermitida(combatePorTurnos));

                    }


                    break;

                default:
                    break;

            }
            cursor.transform.position = new Vector3(objetivo.transform.position.x, objetivo.transform.position.y, 0f);
            atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
            atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);

        }
        else if (Input.GetKeyUp(KeyCode.X)) //se cancela la accion
        {
            //GameManager.MoverUnidad(atacante, EstadoMover.GetPosicionOriginal());
            GameManager.BorrarCasillas();
            //GameManager.SoltarUnidad(atacante);
            InvocarMenuAcciones();
            //atacante.GetComponent<Unidad>().SetDireccionAMirar(EstadoMover.GetDireccionOriginal(), animator);
            atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
            //atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);
            cursor.transform.position = atacante.transform.position;
            camara.transform.position = atacante.transform.position;
            atacante = null;
            objetivo = null;
            //EstadoEsperar.SetUnidadSeleccionada(null);
            maquina.SetEstado(new EstadoElegirAccion(combatePorTurnos));

            yield return new WaitForSeconds(0.01f);
        }

        
        yield return new WaitForSeconds(0.1f);

    }
    
    public static void SetObjetivo(GameObject gob)
    {
        objetivo = gob;
    }

    private void InvocarMenuAcciones()
    {
        cursorMenu.transform.position = new Vector3(unidadAMover.transform.position.x + 0.8f, unidadAMover.transform.position.y, 0f);
        botonAtacar.transform.position = new Vector3(cursorMenu.transform.position.x + 2.3f, cursorMenu.transform.position.y, 0f);
        botonHabilidad.transform.position = new Vector3(botonAtacar.transform.position.x, botonAtacar.transform.position.y - 1f, 0f);
        botonDefender.transform.position = new Vector3(botonHabilidad.transform.position.x, botonHabilidad.transform.position.y - 1f, 0f);
        botonObjetos.transform.position = new Vector3(botonDefender.transform.position.x, botonDefender.transform.position.y - 1f, 0f);
    }

}

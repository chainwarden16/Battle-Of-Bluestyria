using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoUsarConsumible : Estado
{
    public EstadoUsarConsumible(CombatePorTurnos comba) : base(comba)
    {

    }
    private GameObject cursorHabilidad;

    private GameObject cursor;

    private GameObject atacante;

    private static GameObject objetivo;

    private GameObject fondoDescripcion;

    private GameObject[] fondoHabilidades;

    private Consumible consumible;

    private MaquinaDeEstados maquina = IniciacionCombate.GetMaquinaDeEstados();

    private List<Consumible> consumiblesActuales;

    private bool reseteado = false;

    private Animator animator;

    private GameObject popUp;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    private bool sePuedePulsarTecla = true;

    private static Animator animacionesAtaques;

    private static GameObject objetoAnimaciones;

    private AnimatorClipInfo[] info;

    public override IEnumerator StartState()
    {

        if (reseteado == false)
        {
            objetoAnimaciones = GameObject.Find("AnimacionesCombate");
            animacionesAtaques = objetoAnimaciones.GetComponent<Animator>();

            audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();

            consumible = EstadoElegirConsumible.GetConsumibleElegido();
            
            cursorHabilidad = GameObject.Find("Flecha-Lista-Habilidades");

            fondoDescripcion = GameObject.Find("Fondo-Descripcion");

            fondoHabilidades = GameObject.FindGameObjectsWithTag("FondoHabilidad");

            cursor = GameObject.Find("Cursor");

            atacante = EstadoEsperar.GetUnidadSeleccionada();

            animator = atacante.GetComponent<Animator>();

            popUp = GameObject.Find("NumeroPopUp");

            reseteado = true;
        }

        if (sePuedePulsarTecla)
        {
            GameManager.MoverElCursor();
        }
        


        if (Input.GetKeyUp(KeyCode.C) && GameManager.HayAliadoEnCelda(cursor.transform.position) && sePuedePulsarTecla && GameManager.GetListaCeldasHabilidad().Contains(cursor.transform.position))
        {
            if (!audioSource.isPlaying)
            {
                cursorSFX = Resources.Load<AudioClip>("Audio/Heal1");
                audioSource.PlayOneShot(cursorSFX, 1f);
            }
            sePuedePulsarTecla = false;
            GameManager.CambiarDireccionUnidad(atacante);
            atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
            

            objetivo = GameManager.SeleccionarUnidad(cursor.transform.position);
            GameManager.BorrarCasillas();

            switch (consumible.GetID())
            {
                case 0: //Pocion
                    
                    if (!audioSource.isPlaying)
                    {
                        cursorSFX = Resources.Load<AudioClip>("Audio/Heal1");
                        audioSource.PlayOneShot(cursorSFX, 1f);
                    }

                    consumiblesActuales = TropaEscogida.GetConsumibles();
                    consumiblesActuales.Remove(consumiblesActuales.Find(x => x.GetID() == 0));
                    GameManager.CurarVidaUnidad(objetivo, 20);

                    GameManager.ReproducirAnimacion(7, objetivo);
                    info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                    yield return new WaitForSeconds(info[0].clip.length + 1f);
                    //Se invoca el numero que indica cuánto se va a curar

                    animacionesAtaques.SetInteger("ID", -1);
                    yield return new WaitForSeconds(0.1f);

                    break;

                case 1:

                    //Tonico
                    if (!audioSource.isPlaying)
                    {
                        cursorSFX = Resources.Load<AudioClip>("Audio/Heal2");
                        audioSource.PlayOneShot(cursorSFX, 1f);
                    }
                    consumiblesActuales = TropaEscogida.GetConsumibles();
                    consumiblesActuales.Remove(consumiblesActuales.Find(x => x.GetID() == 1));
                    GameManager.EliminarEstadosDañinosUnidad(objetivo);

                    GameManager.ReproducirAnimacion(16, objetivo);

                    info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                    yield return new WaitForSeconds(info[0].clip.length + 1f);
                    //Se invoca el numero que indica cuánto se va a curar

                    animacionesAtaques.SetInteger("ID", -1);
                    yield return new WaitForSeconds(0.1f);

                    break;

                case 2: //Elixir
                    if (!audioSource.isPlaying)
                    {
                        cursorSFX = Resources.Load<AudioClip>("Audio/Heal3");
                        audioSource.PlayOneShot(cursorSFX, 1f);
                    }
                    consumiblesActuales = TropaEscogida.GetConsumibles();
                    consumiblesActuales.Remove(consumiblesActuales.Find(x => x.GetID() == 2));
                    GameManager.CurarVidaUnidad(objetivo, objetivo.GetComponent<Clase>().GetPSMax());
                    GameManager.EliminarEstadosDañinosUnidad(objetivo);

                    GameManager.ReproducirAnimacion(24, objetivo);

                    info = animacionesAtaques.GetCurrentAnimatorClipInfo(0);
                    yield return new WaitForSeconds(info[0].clip.length + 1f);
                    //Se invoca el numero que indica cuánto se va a curar

                    animacionesAtaques.SetInteger("ID", -1);
                    yield return new WaitForSeconds(0.1f);


                    break;

                default:
                    break;

            }

            yield return new WaitForSecondsRealtime(1.5f);

            GameManager.EliminarPopUp(popUp);
            atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);
            GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);
            GameManager.CerrarInterfazUnidad();

        }
        else if (Input.GetKeyUp(KeyCode.X)) //se cancela la accion
        {
            CancelarAccion();

            yield return new WaitForSeconds(0.01f);
        }


        yield return new WaitForSeconds(0.01f);

    }



    private void CerrarMenuHabilidades()
    {
        cursorHabilidad.transform.position = new Vector3(atacante.transform.position.x + 300.0f, atacante.transform.position.y, 0f);
        fondoDescripcion.transform.position = new Vector3(cursorHabilidad.transform.position.x + 300.0f, cursorHabilidad.transform.position.y - 0.9f, 0f);

        foreach (GameObject gob in fondoHabilidades)
        {

            gob.transform.Translate(300.0f, 0f, 0f);

        }

    }

   

    private void CancelarAccion() //se vuelve a EstadoEsperar y se resetea todo
    {
        if (!audioSource.isPlaying)
        {
            cursorSFX = Resources.Load<AudioClip>("Audio/Cancel2");
            audioSource.PlayOneShot(cursorSFX, 1f);
        }
        CerrarMenuHabilidades();
        GameManager.BorrarCasillas();
        GameManager.SoltarUnidad(atacante);
        atacante.GetComponent<Unidad>().SetDireccionAMirar(EstadoMover.GetDireccionOriginal(), animator);
        atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
        atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);
        atacante.transform.position = EstadoMover.GetPosicionOriginal();
        atacante = null;
        objetivo = null;
        EstadoEsperar.SetUnidadSeleccionada(null);
        maquina.SetEstado(new EstadoEsperar(combatePorTurnos));


    }
    public static void SetObjetivo(GameObject gob)
    {
        objetivo = gob;
    }


    

}

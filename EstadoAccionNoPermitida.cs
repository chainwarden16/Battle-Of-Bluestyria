using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoAccionNoPermitida : Estado
{
    private GameObject atacante = EstadoEsperar.GetUnidadSeleccionada();


    private MaquinaDeEstados maquina = IniciacionCombate.GetMaquinaDeEstados();

    private Animator animator;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    public EstadoAccionNoPermitida(CombatePorTurnos comba) : base(comba)
    {

    }

    // Start is called before the first frame update
    public override IEnumerator StartState()
    {
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        if (Input.GetKeyUp(KeyCode.C))
        {

            if (!audioSource.isPlaying)
            {
                cursorSFX = Resources.Load<AudioClip>("Audio/Decision2");
                audioSource.PlayOneShot(cursorSFX, 1f);
            }
            GameManager.EliminarMensajePerdidaTurno();
            animator = atacante.GetComponent<Animator>();
            
            GameManager.BorrarCasillas();
            GameManager.SoltarUnidad(atacante);
            atacante.transform.position = EstadoMover.GetPosicionOriginal();
            atacante.GetComponent<Unidad>().SetDireccionAMirar(EstadoMover.GetDireccionOriginal(), animator);
            atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
            atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);
            EstadoEsperar.SetUnidadSeleccionada(null);
            EstadoElegirObjetivoHabilidad.SetObjetivo(null);
            atacante = null;
            maquina.SetEstado(new EstadoEsperar(combatePorTurnos));
            yield return null;
        }

    }
    
}

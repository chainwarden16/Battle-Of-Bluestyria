using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoPerdidaTurno : Estado
{
    private GameObject atacante = EstadoEsperar.GetUnidadSeleccionada();

    private Animator animator;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    public EstadoPerdidaTurno(CombatePorTurnos comba) : base(comba)
    {

    }

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
            animator = atacante.GetComponent<Animator>();
            GameManager.EliminarMensajePerdidaTurno();
            
            atacante.GetComponent<Unidad>().SetDireccionAMirar(EstadoMover.GetDireccionOriginal(), animator);
            atacante.GetComponent<Unidad>().SetEstaAtacando(false, animator);
            atacante.GetComponent<Unidad>().SetEstaCaminando(false, animator);
            GameManager.BorrarCasillas();
            GameManager.TerminarTurnoUnidad(atacante, combatePorTurnos);
            yield return null;
        }


    }
}

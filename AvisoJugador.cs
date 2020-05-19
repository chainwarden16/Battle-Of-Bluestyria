using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AvisoJugador : MonoBehaviour
{
    private TextMeshProUGUI advertencia;
    private GameObject cursor;
    private int posicionCursor = 0;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        advertencia = GameObject.Find("Mensaje").GetComponentInChildren<TextMeshProUGUI>();
        cursor = GameObject.Find("Cursor");
        if (TropaEscogida.GetTipoAdvertencia() == 1)
        {
            advertencia.text = "No es recomendable usar sólo un tipo de unidad en combate. ¿Quieres continuar de todos modos?";
        }
        else
        {
            advertencia.text = "No es recomendable usar sólo una unidad en combate. ¿Quieres continuar de todos modos?";
        }
    }

    // Update is called once per frame
    void Update()
    {
        cursor.transform.Translate(Mathf.Cos(Time.time * 2) / 200, 0f, 0f);

        MoverCursor();

    }

    private void MoverCursor()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            
            if (posicionCursor == 0)
            {
                if (!audioSource.isPlaying)
                {
                    cursorSFX = Resources.Load<AudioClip>("Audio/Decision2");
                    audioSource.PlayOneShot(cursorSFX, 1f);
                }
                SceneManager.LoadScene(2);

            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    cursorSFX = Resources.Load<AudioClip>("Audio/Cancel2");
                    audioSource.PlayOneShot(cursorSFX, 1f);
                }
                SceneManager.LoadScene(5);
            }

        }else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if(posicionCursor == 0)
            {
                posicionCursor++;
                cursor.transform.Translate(3.6f,0,0);
            }
        }else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (posicionCursor == 1)
            {
                posicionCursor--;
                cursor.transform.Translate(-3.6f, 0, 0);
            }
        }

        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuResumen: MonoBehaviour
{
    private List<int> tropaJugador = TropaEscogida.GetTropa();
    private List<int> armasJugador = TropaEscogida.GetArmasJugador();

    private int posicionCursor;
    private GameObject marcoSeleccion;
    private GameObject flechaArriba;
    private GameObject flechaAbajo;
    private GameObject cursor;

    private GameObject heroe;
    private GameObject arma;
    private GameObject iconoArma;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    private GameObject degra;

    private float degAmo = 0f;
    private float latencia = 2f;

    // Start is called before the first frame update
    public void Start()
    {
        degra = GameObject.Find("FadeIn");
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        posicionCursor = 0;
        marcoSeleccion = GameObject.Find("Marco-Seleccion");
        flechaAbajo = GameObject.Find("Flecha-Abajo");
        flechaArriba = GameObject.Find("Flecha-Arriba");
        cursor = GameObject.Find("Flecha-Derecha");


        for(int indice = 0; indice < tropaJugador.Count; indice++)
        {
            heroe = GameObject.Find("Heroe" + (indice+1));
            arma = GameObject.Find("Arma" + (indice+1));
            iconoArma = GameObject.Find("IconoArma"+(indice+1));

            if (tropaJugador[indice] == 1)
            {
                
                heroe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Mago-Seleccion");
                iconoArma.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Tomo");

                if (armasJugador[indice] == 0)
                {
                    
                    arma.GetComponentInChildren<TextMeshProUGUI>().text = "Tomo";
                }
                else
                {
                    
                    arma.GetComponentInChildren<TextMeshProUGUI>().text = "Códex";
                }

            }else if (tropaJugador[indice] == 2)
            {
                heroe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Lancero-Seleccion");
                iconoArma.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Lanza");
                if (armasJugador[indice] == 0)
                {
                    arma.GetComponentInChildren<TextMeshProUGUI>().text = "Lanza";
                }
                else
                {
                    arma.GetComponentInChildren<TextMeshProUGUI>().text = "Pica";
                }
            }
            else if (tropaJugador[indice] == 3)
            {
                heroe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Clerigo-Seleccion");
                iconoArma.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Baculo");
                if (armasJugador[indice] == 0)
                {
                    arma.GetComponentInChildren<TextMeshProUGUI>().text = "Báculo";
                }
                else
                {
                    arma.GetComponentInChildren<TextMeshProUGUI>().text = "Ángelus";
                }
            }
            else if (tropaJugador[indice] == 4)
            {
                heroe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arquero-Seleccion");
                iconoArma.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Arco");
                if (armasJugador[indice] == 0)
                {
                    arma.GetComponentInChildren<TextMeshProUGUI>().text = "Arco";
                }
                else
                {
                    arma.GetComponentInChildren<TextMeshProUGUI>().text = "Platiun";
                }
            }
            else
            {
                heroe.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                arma.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                arma.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
                iconoArma.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

            }
        }

    }

    // Update is called once per frame
    public void Update()
    {
        
        MoverCursor();

    }

    private void MoverCursor()
    {
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if(posicionCursor == 0)
            {
                if (!audioSource.isPlaying)
                {
                    cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1");
                    audioSource.PlayOneShot(cursorSFX, 1f);
                }
                marcoSeleccion.transform.Translate(0, -1.8f, 0);
                flechaArriba.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                flechaArriba.transform.Translate(0, -1.8f, 0);
                flechaAbajo.transform.Translate(0, -1.8f, 0);
                cursor.transform.Translate(0, -1.8f, 0);
                posicionCursor++;

            }
            else if(posicionCursor == 1)
            {
                if (!audioSource.isPlaying)
                {
                    cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1");
                    audioSource.PlayOneShot(cursorSFX, 1f);
                }
                marcoSeleccion.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                flechaArriba.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                flechaAbajo.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                cursor.transform.position = new Vector3(-2.02f, -3.76f, 0);

                posicionCursor++;

            }
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {

            if(posicionCursor == 2)
            {
                if (!audioSource.isPlaying)
                {
                    cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1");
                    audioSource.PlayOneShot(cursorSFX, 1f);
                }
                cursor.transform.position = new Vector3(-6.4f, -1.8f, 0);
                marcoSeleccion.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                flechaArriba.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                flechaAbajo.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                posicionCursor--;

            }else if(posicionCursor == 1)
            {
                if (!audioSource.isPlaying)
                {
                    cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1");
                    audioSource.PlayOneShot(cursorSFX, 1f);
                }
                marcoSeleccion.transform.Translate(0, 1.8f, 0);
                flechaArriba.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                flechaArriba.transform.Translate(0, 1.8f, 0);
                flechaAbajo.transform.Translate(0, 1.8f, 0);
                cursor.transform.Translate(0, 1.8f, 0);
                posicionCursor--;
            }

        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
           
            SeleccionarOpcion();
           
        }
    }

    private void SeleccionarOpcion()
    {
        if(posicionCursor == 0)
        {
            if (!audioSource.isPlaying)
            {
                cursorSFX = Resources.Load<AudioClip>("Audio/Decision2");
                audioSource.PlayOneShot(cursorSFX, 1f);
            }
            //Se carga la escena con las armas

            SceneManager.LoadScene(6);


        }
        else if(posicionCursor == 1)
        {
            if (!audioSource.isPlaying)
            {
                cursorSFX = Resources.Load<AudioClip>("Audio/Decision2");
                audioSource.PlayOneShot(cursorSFX, 1f);
            }
            //Se carga la escena con los personajes. Una vez se escogen, la lista de armas se actualiza automáticamente
            SceneManager.LoadScene(1);

        }
        else if(posicionCursor == 2)
        {
            if (!audioSource.isPlaying)
            {
                cursorSFX = Resources.Load<AudioClip>("Audio/Decision2");
                audioSource.PlayOneShot(cursorSFX, 1f);
            }
            int magos = 0;
            int lanceros = 0;
            int clerigos = 0;
            int arqueros = 0;
            int nulos = 0;

            foreach(int unidad in tropaJugador)
            {
                if (unidad == 1)
                {
                    magos++;
                }else if (unidad == 2)
                {
                    lanceros++;
                }else if(unidad == 3){
                    clerigos++;
                }else if(unidad == 4)
                {
                    lanceros++;
                }
                else
                {
                    nulos++;
                }
            }
            //Colocamos un aviso en caso de que el jugador vaya sólo con un tipo de unidad o con una única unidad. Que luego no diga que es difícil ganar después de darle una advertencia
            if (nulos >=3)
            {
                TropaEscogida.SetTipoAdvertencia(0);
                SceneManager.LoadScene(7);

            }
            else if(magos == tropaJugador.Count || lanceros == tropaJugador.Count || clerigos == tropaJugador.Count || arqueros == tropaJugador.Count)
            {
                TropaEscogida.SetTipoAdvertencia(1);
                SceneManager.LoadScene(7);
            }
            else
            {
                StartCoroutine(CrearFundidoNegro(degra));
                

            }

        }
    }


    private IEnumerator CrearFundidoNegro(GameObject cosaAOpacar)
    {
        Color colorFadeOut = new Color(0, 0, 0, 1);
        while (degAmo < 1f)
        {

            Color actual = cosaAOpacar.GetComponent<SpriteRenderer>().color;
            actual.a = degAmo;
            cosaAOpacar.GetComponent<SpriteRenderer>().color = actual;
            
            degAmo += Time.deltaTime / latencia;
            yield return new WaitForSeconds(0.01f);

        }
        
        colorFadeOut.a = 1;
        cosaAOpacar.GetComponent<SpriteRenderer>().color = colorFadeOut;
        SceneManager.LoadScene(2);
    }

}

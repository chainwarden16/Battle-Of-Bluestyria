using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Titulo : MonoBehaviour
{
    private int cursor = 0;
    private GameObject indicadorCursor;
    private AudioSource audioSource;
    private AudioClip cursorSFX;
    private GameObject fade;
    private float time = 3f;
    private float lapso = 0f;
    private float pausa = 0f;
    private int contador = -1;
    private TextMeshPro texto;
    private bool sePuedePulsarTecla = true;
    private GameObject titulo;
    private GameObject botonComenzar;
    private GameObject botonSalir;

    public void Start(){
        botonComenzar = GameObject.Find("Start");
        botonSalir = GameObject.Find("Exit");
        titulo = GameObject.Find("Battle-Of-Bluestyria-Titulo");
        texto = GameObject.Find("Texto").GetComponent<TextMeshPro>();
        fade = GameObject.Find("FadeIn");
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        Screen.SetResolution(800, 600, false);
        indicadorCursor = GameObject.Find("Cursor");
        indicadorCursor.transform.Translate(Mathf.Cos(Time.time * 2) / 200, 0f, 0f);
        StartCoroutine(Aparecer(fade, 3f));
    }

    public void Update()
    {
        /*while(fadeIn.GetComponent<SpriteRenderer>().color.a !=0)
        {
            Aparecer();
        }*/
        indicadorCursor.transform.Translate(Mathf.Cos(Time.time*2)/200, 0f, 0f);
        if(contador == -1)
        {
            MoverCursor();
        }
        else
        {
            ComenzarPartida();
        }
        
    }

    private void ComenzarPartida(){

        //Si el jugador decide empezar una partida, se carga la pantalla que lo llevará a la selección de su tropa
        
        if (Input.GetKeyUp(KeyCode.C))
        {
            
            if (contador == 0)
            {
                ReproducirSonido("Audio/Decision2");
                texto.text = "Bluestyria, país pacífico de bosques frondosos, y Redstaren, una tierra árida regida por un gobernante de ideales expansionistas.";
                contador++;
                
            }else if (contador == 1)
            {
                ReproducirSonido("Audio/Decision2");
                texto.text = "Redstaren ha intentado conquistar los terrenos de Bluestyria durante muchos años, mas este siempre ha sido capaz de mantenerlos a raya.";
                contador++;
            }
            else if (contador == 2)
            {
                ReproducirSonido("Audio/Decision2");
                texto.text = "No obstante, en esta ocasión el ataque se ha llevado a cabo desde varios lugares de la región al mismo tiempo, lo que ha obligado a ambos bandos a dividir las tropas en grupos pequeños.";
                contador++;
            }
            else if (contador == 3)
            {
                ReproducirSonido("Audio/Decision2");
                texto.text = "La batalla tendrá lugar por la mañana. Es hora de decidir quiénes defenderán la entrada cercana al pueblo de Tinvill.";
                contador++;
            }
            else if (contador == 4)
            {
                ReproducirSonido("Audio/Decision2");
                texto.text = "Escoge sabiamente. El destino de miles de personas está en tus manos.";
                contador++;
            }
            else
            {
                ReproducirSonido("Audio/Decision2");
                SceneManager.LoadScene(1);
            }
        }



        

    }

    private void CerrarJuego(){

        //Hacemos una transición con fundido negro, para que no quede demasiado abrupto

        //El programa se cierra
        Application.Quit();
    }

    private void MoverCursor()
    {

        if(Input.GetKeyUp(KeyCode.DownArrow)&& cursor == 0)
        {

            indicadorCursor.transform.position = new Vector3(-2.5f, -2.9f, 0);
            cursor++;
            ReproducirSonido("Audio/Cursor1");

        }
        else if(Input.GetKeyUp(KeyCode.UpArrow) && cursor == 1)
        {

            indicadorCursor.transform.position = new Vector3(-2.2f, -1.35f, 0);
            cursor--;
            ReproducirSonido("Audio/Cursor1");

        }

        else if (Input.GetKeyUp(KeyCode.C) && sePuedePulsarTecla)
        {
            sePuedePulsarTecla = false;
            ReproducirSonido("Audio/Decision2");

            if (cursor == 0)
            {

                StartCoroutine(DesvanecerTitulo(3f));
                
                fade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
                texto.text = "En el continente de Warsof, existen dos reinos vecinos:";
                contador = 0;
                
            }
            else
            {
                StartCoroutine(Desaparecer(fade));
                
                CerrarJuego();
            }
        }
    }
    private IEnumerator DesvanecerTitulo(float tiempo)
    {
        Color colorFadeOut = new Color(0, 0, 0, 0);


        while (lapso < 1f)
        {

            titulo.GetComponent<SpriteRenderer>().color = Color.Lerp(titulo.GetComponent<SpriteRenderer>().color, colorFadeOut, lapso);
            botonComenzar.GetComponent<SpriteRenderer>().color = Color.Lerp(botonComenzar.GetComponent<SpriteRenderer>().color, colorFadeOut, lapso);
            botonSalir.GetComponent<SpriteRenderer>().color = Color.Lerp(botonSalir.GetComponent<SpriteRenderer>().color, colorFadeOut, lapso);
            indicadorCursor.GetComponent<SpriteRenderer>().color = Color.Lerp(indicadorCursor.GetComponent<SpriteRenderer>().color, colorFadeOut, lapso);
            lapso += Time.deltaTime / tiempo;
            yield return new WaitForSeconds(0.01f);

        }
        colorFadeOut.a = 0;
        titulo.GetComponent<SpriteRenderer>().color = colorFadeOut;
        botonComenzar.GetComponent<SpriteRenderer>().color = colorFadeOut;
        botonSalir.GetComponent<SpriteRenderer>().color = colorFadeOut;
        indicadorCursor.GetComponent<SpriteRenderer>().color = colorFadeOut;
        lapso = 0f;
        yield return null;
    }

    private IEnumerator Aparecer(GameObject fadeIn, float tiempo)
    {
        Color colorFadeOut = new Color(0, 0, 0, 0);


        while (pausa < 1f)
        {

            fadeIn.GetComponent<SpriteRenderer>().color = Color.Lerp(fadeIn.GetComponent<SpriteRenderer>().color, colorFadeOut, pausa);

            pausa += Time.deltaTime / tiempo;
            yield return new WaitForSeconds(0.01f);

        }
        colorFadeOut.a = 0;
        fadeIn.GetComponent<SpriteRenderer>().color = colorFadeOut;
        pausa = 0f;
        yield return null;
    }

    private IEnumerator Desaparecer(GameObject fadeIn)
    {
        float pau = 0f;
        Color colorFadeOut = new Color(0, 0, 0, 1);
        
        while (pau < 1f)
        {

            fadeIn.GetComponent<SpriteRenderer>().color = Color.Lerp(fadeIn.GetComponent<SpriteRenderer>().color, colorFadeOut, pau);

            pau += Time.deltaTime / time;
            yield return new WaitForSeconds(0.01f);

        }

        colorFadeOut.a = 1;
        fadeIn.GetComponent<SpriteRenderer>().color = colorFadeOut;
        pau = 0f;
        yield return null;
    }

    public void ReproducirSonido(string rutaArchivo)
    {
        if (!audioSource.isPlaying)
        {
            cursorSFX = Resources.Load<AudioClip>(rutaArchivo);
            audioSource.PlayOneShot(cursorSFX, 1f);
        }
    }
}

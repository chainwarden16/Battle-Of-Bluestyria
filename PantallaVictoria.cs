using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PantallaVictoria : MonoBehaviour
{
    private GameObject degradadoFin;
    private GameObject tabernaExterior;
    private float degAm = 1f;
    private float latencia = 2f;
    private TextMeshPro texto;
    private List<GameObject> heroes;
    public List<Sprite> spritesPersonajes = new List<Sprite>();

    private List<int> tropaViva = TropaEscogida.GetTropaViva();

    private int contador = 0;

    private bool sePuedePulsarTecla = false;

    private AudioSource audioS;

    // Start is called before the first frame update
    public void Start()
    {
        heroes = GameObject.FindGameObjectsWithTag("Player").ToList();
        //spritesPersonajes = new List<Sprite>() { Resources.Load<Sprite>("Sprites/Personajes/Gráficos/Atlas-Personajes-Juego/Atlas-Personajes-Juego_0"), Resources.Load<Sprite>("Sprites/Personajes/Gráficos/Atlas-Personajes-Juego/Atlas-Personajes-Juego_0"), Resources.Load<Sprite>("Sprites/Personajes/Gráficos/Atlas-Personajes-Juego/Atlas-Personajes-Juego_65"), Resources.Load<Sprite>("Sprites/Personajes/Gráficos/Atlas-Personajes-Juego/Atlas-Personajes-Juego_22") };
        audioS = GameObject.Find("SFX").GetComponent<AudioSource>();
        tabernaExterior = GameObject.Find("ExteriorTaberna");
        degradadoFin = GameObject.Find("FadeIn");
        texto = GameObject.Find("Texto").GetComponent<TextMeshPro>();

        

        for (int i = 0; i < tropaViva.Count; i++)
        {

            if (tropaViva[i] == 1)
            {

                heroes[i].GetComponent<SpriteRenderer>().sprite = spritesPersonajes[0];
            }
            else if (tropaViva[i] == 2)
            {
                heroes[i].GetComponent<SpriteRenderer>().sprite = spritesPersonajes[1];
            }
            else if (tropaViva[i] == 3)
            {
                heroes[i].GetComponent<SpriteRenderer>().sprite = spritesPersonajes[2];
            }
            else if (tropaViva[i] == 4)
            {
                heroes[i].GetComponent<SpriteRenderer>().sprite = spritesPersonajes[3];
            }
        }

        StartCoroutine(QuitarFundidoNegroMitad(degradadoFin));

    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.C) && sePuedePulsarTecla)
        {
            sePuedePulsarTecla = false;
            if (contador == 0)
            {
                texto.text = "¡Hay que celebrar la victoria! Esta noche, la taberna invita a todos a la primera ronda.";
                contador++;
                sePuedePulsarTecla = true;

            }
            else if (contador == 1)
            {
                texto.text = "";
                degradadoFin.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                
                StartCoroutine(QuitarFundidoNegro(tabernaExterior));
                contador++; 
                
            }
            else if (contador == 2)
            {   
                
                StartCoroutine(PonerFundidoNegro(degradadoFin));
                Application.Quit();
                
            }
        }
    }

    private IEnumerator QuitarFundidoNegro(GameObject cosaADegradar)
    {
        Color colorFadeOut = new Color(1, 1, 1, 0);
        while (degAm > 0f)
        {
            Color actual = new Color(0, 0, 0, 1);
            actual.a = degAm;
            cosaADegradar.GetComponent<SpriteRenderer>().color = actual;

            degAm -= Time.deltaTime / latencia;
            yield return new WaitForSeconds(0.01f);

        }
        colorFadeOut.a = 0;
        cosaADegradar.GetComponent<SpriteRenderer>().color = colorFadeOut;
        degAm = 0f;
        sePuedePulsarTecla = true;
    }

    private IEnumerator QuitarFundidoNegroMitad(GameObject cosaADegradar)
    {
        Color colorFadeOut = new Color(1, 1, 1, 0.5f);
        while (degAm > 0.5f)
        {
            Color actual = new Color(0, 0, 0, 1);
            actual.a = degAm;
            cosaADegradar.GetComponent<SpriteRenderer>().color = actual;

            degAm -= Time.deltaTime / latencia;
            yield return new WaitForSeconds(0.01f);

        }
        colorFadeOut.a = 0;
        cosaADegradar.GetComponent<SpriteRenderer>().color = colorFadeOut;
        degAm = 1f;
        
        sePuedePulsarTecla = true;

    }

    private IEnumerator PonerFundidoNegro(GameObject cosaAOpacar)
    {
        Color colorFadeOut = new Color(0, 0, 0, 1);
        while (degAm < 1f)
        {

            Color actual = cosaAOpacar.GetComponent<SpriteRenderer>().color;
            actual.a = degAm;
            cosaAOpacar.GetComponent<SpriteRenderer>().color = actual;
            audioS.volume -= Time.deltaTime / latencia;
            degAm += Time.deltaTime / latencia;
            yield return new WaitForSeconds(0.01f);

        }
        audioS.volume = 0;

        
        audioS.Stop();
        colorFadeOut.a = 1;
        cosaAOpacar.GetComponent<SpriteRenderer>().color = colorFadeOut;
    }

    

}
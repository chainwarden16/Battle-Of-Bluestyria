using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class EstadoVictoria : Estado
{
    private SpriteRenderer tituloVictoria;

    private GameObject objetoVictoria;

    private GameObject camara;

    private float pasit = 0f;
    private float tiempoEspera = 3f;

    private List<GameObject> unidadesVivas;

    private List<int> integerUnidadesVivas = new List<int>();

    private Animator animator;

    private GameObject fadeIn;

    private static bool fanfarriaVictoria = false;


    public EstadoVictoria(CombatePorTurnos com) : base(com)
    {

    }

    public override IEnumerator StartState()
    {
        if (fanfarriaVictoria == false)
        {
            GameObject.FindGameObjectWithTag("Musica").GetComponent<MusicaGlobal>().PararMusica();
            GameObject.Find("Música").GetComponent<AudioSource>().Stop();
            fadeIn = GameObject.Find("FadeIn");
            camara = GameObject.Find("Main Camera");
            objetoVictoria = GameObject.Find("Victoria-Batalla");
            unidadesVivas = GameObject.FindGameObjectsWithTag("Player").ToList();
            tituloVictoria = objetoVictoria.GetComponent<SpriteRenderer>();
            foreach (GameObject heroe in unidadesVivas)
            {
                animator = heroe.GetComponent<Animator>();
                animator.SetBool("haGanado", true);
                integerUnidadesVivas.Add(heroe.GetComponent<Clase>().GetTipoClase());
                heroe.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }

            TropaEscogida.SetTropaViva(integerUnidadesVivas);

            tituloVictoria.sprite = Resources.Load<Sprite>("Sprites/Menu/Victoria-Batalla");
            tituloVictoria.transform.position = new Vector3(camara.transform.position.x, camara.transform.position.y + 4, 0f);
            Color col = new Color(0, 0, 0, 0.5f);
            fadeIn.GetComponent<SpriteRenderer>().color = col;

            GameManager.ReproducirSonido("Audio/Victory1");
            fanfarriaVictoria = true;
        }


        yield return new WaitForSeconds(5f);

        objetoVictoria.layer = 29;

        Color colorFadeOut = new Color(0, 0, 0, 1);
        while (pasit < 1f)

        {

            fadeIn.GetComponent<SpriteRenderer>().color = Color.Lerp(fadeIn.GetComponent<SpriteRenderer>().color, colorFadeOut, pasit);

            pasit += Time.deltaTime / tiempoEspera;
            yield return new WaitForSeconds(0.01f);

        }
        colorFadeOut.a = 1;
        fadeIn.GetComponent<SpriteRenderer>().color = colorFadeOut;


        //Se carga la escena final
        SceneManager.LoadScene(3);
    }
}

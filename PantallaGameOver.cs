using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PantallaGameOver : MonoBehaviour
{
    private GameObject fadeIn;
    float deg = 0f;
    float time = 3f;
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(AparecerFin());
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.C))

            StartCoroutine(DesaparecerFin());
    }

    private IEnumerator AparecerFin()
    {
        Color colorFadeOut = new Color(0, 0, 0, 0);
        fadeIn = GameObject.Find("FadeIn");
        while (deg < 1f)
        {

            fadeIn.GetComponent<SpriteRenderer>().color = Color.Lerp(fadeIn.GetComponent<SpriteRenderer>().color, colorFadeOut, deg);

            deg += Time.deltaTime / time;
            yield return new WaitForSeconds(0.01f);

        }
        colorFadeOut.a = 0;
        fadeIn.GetComponent<SpriteRenderer>().color = colorFadeOut;
        deg = 0f;
    }
    
    private IEnumerator DesaparecerFin()
    {
        Color colorFadeOut = new Color(0, 0, 0, 1);
        fadeIn = GameObject.Find("FadeIn");
        while (deg < 1f)
        {

            fadeIn.GetComponent<SpriteRenderer>().color = Color.Lerp(fadeIn.GetComponent<SpriteRenderer>().color, colorFadeOut, deg);

            deg += Time.deltaTime / time;
            yield return new WaitForSeconds(0.01f);

        }
        
        colorFadeOut.a = 1;
        fadeIn.GetComponent<SpriteRenderer>().color = colorFadeOut;
        Application.Quit(); //vuelta al titulo
    }
}

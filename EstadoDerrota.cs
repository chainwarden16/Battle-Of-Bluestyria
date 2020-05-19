using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EstadoDerrota : Estado
{

    private GameObject fadeIn;
    float lele = 0f;
    float tiemp = 1f;

    public EstadoDerrota(CombatePorTurnos com) : base(com)
    {

    }

    public override IEnumerator StartState()
    {
        fadeIn = GameObject.Find("FadeIn");

        
        Color colorFadeOut = new Color(1, 1, 1, 1);
        while (lele< 1f)
        {

        fadeIn.GetComponent<SpriteRenderer>().color = Color.Lerp(fadeIn.GetComponent<SpriteRenderer>().color, colorFadeOut, lele);

        lele+= Time.deltaTime / tiemp;
        yield return new WaitForSeconds(0.01f);

        }
        colorFadeOut.a = 1;
        fadeIn.GetComponent<SpriteRenderer>().color = colorFadeOut;
        yield return new WaitForSeconds(2f);

            
        SceneManager.LoadScene(4);
        }
    
    
    }

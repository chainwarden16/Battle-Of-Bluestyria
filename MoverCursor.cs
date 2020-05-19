using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoverCursor : MonoBehaviour
{

    private GameObject camara;

    private static bool cambiarEstado;

    //private List<GameObject> unidadesJugador = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

        camara = GameObject.Find("Main Camera");
        cambiarEstado = false;
        //menuAcciones = GameObject.Find("MenuAccionesUnidad").GetComponent<Dropdown>();
        /*foreach (GameObject gob in GameManager.GetUnidadesATenerControladas())
        {

            if (gob.tag == "Player")
            {
                unidadesJugador.Add(gob);
            }

        }*/
    }

    
    public static bool LlamarColliderCursor(Collision collision)
    {

        OnCollisionStay(collision);
        return cambiarEstado;
    }
    
    
    static void OnCollisionStay(Collision collision)
    {

        if(Input.GetKeyUp("c")){

            //Se revisa si la unidad ya se ha movido
            GameObject unidad = collision.gameObject;
            if(unidad.GetComponent<Unidad>().getTurnoAcabado() == false)
            { 
                
                cambiarEstado = true;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

        //Y = -6.5 y 13.5
        //X = -12.5 y 11.

        camara.transform.position = new Vector2(this.transform.position.x, this.transform.position.y);

    }

   

    
}

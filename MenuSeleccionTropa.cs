using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuSeleccionTropa : MonoBehaviour
{

    //Primero se crea una referencia a cada elemento que vaya a cambiar dentro del menu de seleccion de tropa. Además, se debe tener en cuenta que el jugador podría querer ver información acerca de las unidades
    private SpriteRenderer cuadro1, cuadro2, cuadro3, cuadro4, unidad1, unidad2, unidad3, unidad4, flechaIzquierda, flechaDerecha, flechaArriba, flechaAbajo, MarcoSeleccion;

    private Sprite spriteFlechaDerecha, spriteFlechaIzquierda, spriteCuadro1, spriteCuadro2, spriteCuadro3, spriteCuadro4, spriteUnidad1, spriteUnidad2, spriteUnidad3, spriteUnidad4;

    private Button botonDerecha, botonIzquierda, botonArriba, botonAbajo;

    //Indica la posición inicial del marco de selección. Al cargar la escena, este se encontrará en el primer personaje

    private int indiceColumna = 1;

    private AudioSource audioSource;
    private AudioClip cursorSFX;

    private GameObject[] explicacionClases;

    private GameObject[] flechasMoverExplicacion;

    private bool estaMostrandoseInformacion = false;

    private List<int> tropaTemporal = new List<int> { 1, 2, 3, 4 }; // mago = 1, lancero = 2, clérigo = 3, arquero = 4
    // Start is called before the first frame update
    void Start()
    {
        flechasMoverExplicacion = GameObject.FindGameObjectsWithTag("Cursor");
        GameObject.FindGameObjectWithTag("Musica").GetComponent<MusicaGlobal>().ReproducirMusica();
        explicacionClases = GameObject.FindGameObjectsWithTag("Menu");
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();

        //Se va a llamar a estos objetos en más de una ocasión, por lo que se inicializarán al principio de cargar la escena.

        cuadro1 = GameObject.Find("PanelMago").GetComponent<SpriteRenderer>();
        cuadro2 = GameObject.Find("PanelLancero").GetComponent<SpriteRenderer>();
        cuadro3 = GameObject.Find("PanelClerigo").GetComponent<SpriteRenderer>();
        cuadro4 = GameObject.Find("PanelArquero").GetComponent<SpriteRenderer>();

        unidad1 = GameObject.Find("MagoSeleccion").GetComponent<SpriteRenderer>();
        unidad2 = GameObject.Find("LanceroSeleccion").GetComponent<SpriteRenderer>();
        unidad3 = GameObject.Find("ClerigoSeleccion").GetComponent<SpriteRenderer>();
        unidad4 = GameObject.Find("ArqueroSeleccion").GetComponent<SpriteRenderer>();

        //Las flechas deben moverse junto al marco de seleccion, asi que las buscamos tambien

        flechaAbajo = GameObject.Find("FlechaAbajo").GetComponent<SpriteRenderer>();
        flechaArriba = GameObject.Find("FlechaArriba").GetComponent<SpriteRenderer>();
        flechaDerecha = GameObject.Find("FlechaDerecha").GetComponent<SpriteRenderer>();
        flechaIzquierda = GameObject.Find("FlechaIzquierda").GetComponent<SpriteRenderer>();
        MarcoSeleccion = GameObject.Find("MarcoSeleccion").GetComponent<SpriteRenderer>();

        //Ahora se cargan los sprites que se usarán mientras el jugador cambia de unidades y el panel que aparece sobre ellos mostrando información, 
        //y también para las flechas que tienen que aparecer o desaparecer según la posición del marco

        spriteFlechaDerecha = Resources.Load<Sprite>("Sprites/Menu/Flecha-Derecha");

        spriteFlechaIzquierda = Resources.Load<Sprite>("Sprites/Menu/Flecha-Izquierda");

        spriteCuadro1 = Resources.Load<Sprite>("Sprites/Menu/Panel-Mago");

        spriteCuadro2 = Resources.Load<Sprite>("Sprites/Menu/Panel-Lancero");

        spriteCuadro3 = Resources.Load<Sprite>("Sprites/Menu/Panel-Clerigo");

        spriteCuadro4 = Resources.Load<Sprite>("Sprites/Menu/Panel-Arquero");

        spriteUnidad1 = Resources.Load<Sprite>("Sprites/Menu/Mago-Seleccion");

        spriteUnidad2 = Resources.Load<Sprite>("Sprites/Menu/Lancero-Seleccion");

        spriteUnidad3 = Resources.Load<Sprite>("Sprites/Menu/Clerigo-Seleccion");

        spriteUnidad4 = Resources.Load<Sprite>("Sprites/Menu/Arquero-Seleccion");

        //Y a continuación, se busca el botón de la IU para habilitarlos/desahilitarlos cuando sea conveniente

        botonDerecha = GameObject.Find("FlechaDerecha").GetComponent<Button>();
        botonIzquierda = GameObject.Find("FlechaIzquierda").GetComponent<Button>();
        botonArriba = GameObject.Find("FlechaArriba").GetComponent<Button>();
        botonAbajo = GameObject.Find("FlechaAbajo").GetComponent<Button>();


    }

    // Update is called once per frame
    public void Update()
    {

        if (estaMostrandoseInformacion == false)
        {

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {

                CambiarUnidadAbajo();

            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {

                CambiarUnidadArriba();

            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                MoverElementosDerecha();

            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {

                MoverElementosIzquierda();

            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                VolverAlMenu();
            }

        }


        if (Input.GetKeyUp(KeyCode.Z))
        {
            MostrarInformacionClases();
        }

    }

    private void MostrarInformacionClases()
    {

        if (estaMostrandoseInformacion == false)
        {
            foreach (GameObject gob in explicacionClases)
            {
                gob.transform.position = new Vector3(0, 0, 0);
                estaMostrandoseInformacion = true;
            }

            foreach(GameObject flecha in flechasMoverExplicacion)
            {
                flecha.transform.Translate(4000, 0, 0);
            }
            
        }
        else
        {
            foreach (GameObject gob in explicacionClases)
            {
                gob.transform.Translate(200, 0, 0);
                estaMostrandoseInformacion = false;
            }
            foreach (GameObject flecha in flechasMoverExplicacion)
            {
                flecha.transform.Translate(-4000, 0, 0);
            }
        }


    }

    public void MoverElementosDerecha()
    {
        Vector2 botonArribaPosicion = botonArriba.transform.position;
        Vector2 botonAbajoPosicion = botonAbajo.transform.position;
        Vector2 botonDerechaPosicion = botonDerecha.transform.position;
        Vector2 botonIzquierdaPosicion = botonIzquierda.transform.position;
        //Si el jugador hace click en la flecha derecha, el marco de seleccion y las flechas deben desplazarse, a menos que se encuentren en el último personaje

        if (indiceColumna == 1)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna++;
            MarcoSeleccion.transform.position = new Vector2(-1.77f, 0.25f);
            botonDerechaPosicion.x += 172.0f;
            botonArribaPosicion.x += 173.0f;
            botonAbajoPosicion.x += 173.0f;

            botonAbajo.transform.position = botonAbajoPosicion;
            botonDerecha.transform.position = botonDerechaPosicion;
            botonArriba.transform.position = botonArribaPosicion;
            botonIzquierda.interactable = true;
            Color nuevoColor = new Color(1, 1, 1, 1);
            botonIzquierda.GetComponent<Image>().color = nuevoColor;

        }

        else if (indiceColumna == 2)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna++;
            MarcoSeleccion.transform.position = new Vector2(1.3f, 0.25f);
            botonDerechaPosicion.x += 186.0f;
            botonArribaPosicion.x += 184.0f;
            botonAbajoPosicion.x += 184.0f;
            botonIzquierdaPosicion.x += 184.0f;

            botonAbajo.transform.position = botonAbajoPosicion;
            botonDerecha.transform.position = botonDerechaPosicion;
            botonArriba.transform.position = botonArribaPosicion;
            botonIzquierda.transform.position = botonIzquierdaPosicion;
        }

        else if (indiceColumna == 3)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna++;
            MarcoSeleccion.transform.position = new Vector2(4.37f, 0.25f);


            botonArribaPosicion.x += 184.0f;
            botonAbajoPosicion.x += 184.0f;
            botonIzquierdaPosicion.x += 184.0f;

            botonAbajo.transform.position = botonAbajoPosicion;
            botonArriba.transform.position = botonArribaPosicion;
            botonIzquierda.transform.position = botonIzquierdaPosicion;

            botonDerecha.interactable = false;
            Color nuevoColor = new Color(1, 1, 1, 0);
            botonDerecha.GetComponent<Image>().color = nuevoColor;
        }
    }

    public void MoverElementosIzquierda()
    {

        Vector2 botonArribaPosicion = botonArriba.transform.position;
        Vector2 botonAbajoPosicion = botonAbajo.transform.position;
        Vector2 botonDerechaPosicion = botonDerecha.transform.position;
        Vector2 botonIzquierdaPosicion = botonIzquierda.transform.position;
        //Si el jugador hace click en la flecha izquierda, el marco de seleccion y las flechas deben desplazarse, a menos que se encuentren en el primer personaje

        if (indiceColumna == 2)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna--;
            MarcoSeleccion.transform.position = new Vector2(-4.65f, 0.25f);
            botonDerechaPosicion.x -= 172.0f;
            botonArribaPosicion.x -= 173.0f;
            botonAbajoPosicion.x -= 173.0f;

            botonAbajo.transform.position = botonAbajoPosicion;
            botonDerecha.transform.position = botonDerechaPosicion;
            botonArriba.transform.position = botonArribaPosicion;
            botonIzquierda.interactable = false;
            Color nuevoColor = new Color(1, 1, 1, 0);
            botonIzquierda.GetComponent<Image>().color = nuevoColor;


        }

        else if (indiceColumna == 3)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna--;
            MarcoSeleccion.transform.position = new Vector2(-1.77f, 0.25f);
            botonDerechaPosicion.x -= 186.0f;
            botonArribaPosicion.x -= 184.0f;
            botonAbajoPosicion.x -= 184.0f;
            botonIzquierdaPosicion.x -= 184.0f;

            botonAbajo.transform.position = botonAbajoPosicion;
            botonDerecha.transform.position = botonDerechaPosicion;
            botonArriba.transform.position = botonArribaPosicion;
            botonIzquierda.transform.position = botonIzquierdaPosicion;
        }

        else if (indiceColumna == 4)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna--;
            MarcoSeleccion.transform.position = new Vector2(1.3f, 0.25f);


            botonArribaPosicion.x -= 184.0f;
            botonAbajoPosicion.x -= 184.0f;
            botonIzquierdaPosicion.x -= 184.0f;
            botonDerechaPosicion.x += 186.0f;

            botonAbajo.transform.position = botonAbajoPosicion;
            botonArriba.transform.position = botonArribaPosicion;
            botonIzquierda.transform.position = botonIzquierdaPosicion;

            botonDerecha.interactable = true;
            Color nuevoColor = new Color(1, 1, 1, 1);
            botonDerecha.GetComponent<Image>().color = nuevoColor;
        }

    }

    public void CambiarUnidadAbajo()
    {

        int unidadACambiar = tropaTemporal[indiceColumna - 1];


        if (indiceColumna == 1)
        {//el primer index es especial, porque debe tener un personaje sí o sí. El resto pueden estar vacíos, así que deben tratarse aparte.
         //se toma el int que haya en la primera posicion de la lista
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1: //los datos y el personaje deben pasar a ser los del lancero, así como el index de la lista
                    unidad1.sprite = spriteUnidad2;
                    cuadro1.sprite = spriteCuadro2;
                    tropaTemporal[0] = 2;
                    break;
                case 2://los datos y el personaje deben pasar a ser los del clérigo, así como el index de la lista
                    unidad1.sprite = spriteUnidad3;
                    cuadro1.sprite = spriteCuadro3;
                    tropaTemporal[0] = 3;
                    break;
                case 3:
                    //los datos y el personaje deben pasar a ser los del arquero, así como el index de la lista
                    unidad1.sprite = spriteUnidad4;
                    cuadro1.sprite = spriteCuadro4;
                    tropaTemporal[0] = 4;
                    break;
                case 4:
                    //los datos y el personaje deben pasar a ser los del mago, así como el index de la lista
                    unidad1.sprite = spriteUnidad1;
                    cuadro1.sprite = spriteCuadro1;
                    tropaTemporal[0] = 1;
                    break;
            }
        }

        else if (indiceColumna == 2)
        { //el código es casi igual; sólo se contempla que no haya ningún personaje asignado (index con valor = 0)
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1:
                    tropaTemporal[indiceColumna - 1] = 2;
                    unidad2.sprite = spriteUnidad2;
                    cuadro2.sprite = spriteCuadro2;
                    break;
                case 2:
                    unidad2.sprite = spriteUnidad3;
                    cuadro2.sprite = spriteCuadro3;
                    tropaTemporal[indiceColumna - 1] = 3;
                    break;
                case 3:

                    unidad2.sprite = spriteUnidad4;
                    cuadro2.sprite = spriteCuadro4;
                    tropaTemporal[indiceColumna - 1] = 4;
                    break;
                case 4:

                    unidad2.sprite = null;
                    cuadro2.sprite = null;
                    tropaTemporal[indiceColumna - 1] = 0;
                    break;
                default:


                    unidad2.sprite = spriteUnidad1;
                    cuadro2.sprite = spriteCuadro1;
                    tropaTemporal[indiceColumna - 1] = 1;
                    break;
            }
        }

        else if (indiceColumna == 3)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1:
                    tropaTemporal[indiceColumna - 1] = 2;
                    unidad3.sprite = spriteUnidad2;
                    cuadro3.sprite = spriteCuadro2;
                    break;
                case 2:
                    unidad3.sprite = spriteUnidad3;
                    cuadro3.sprite = spriteCuadro3;
                    tropaTemporal[indiceColumna - 1] = 3;
                    break;
                case 3:

                    unidad3.sprite = spriteUnidad4;
                    cuadro3.sprite = spriteCuadro4;
                    tropaTemporal[indiceColumna - 1] = 4;
                    break;
                case 4:

                    unidad3.sprite = null;
                    cuadro3.sprite = null;
                    tropaTemporal[indiceColumna - 1] = 0;
                    break;
                default:


                    unidad3.sprite = spriteUnidad1;
                    cuadro3.sprite = spriteCuadro1;
                    tropaTemporal[indiceColumna - 1] = 1;
                    break;
            }

        }

        else
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1:
                    tropaTemporal[indiceColumna - 1] = 2;
                    unidad4.sprite = spriteUnidad2;
                    cuadro4.sprite = spriteCuadro2;
                    break;
                case 2:
                    unidad4.sprite = spriteUnidad3;
                    cuadro4.sprite = spriteCuadro3;
                    tropaTemporal[indiceColumna - 1] = 3;
                    break;
                case 3:

                    unidad4.sprite = spriteUnidad4;
                    cuadro4.sprite = spriteCuadro4;
                    tropaTemporal[indiceColumna - 1] = 4;
                    break;
                case 4:

                    unidad4.sprite = null;
                    cuadro4.sprite = null;
                    tropaTemporal[indiceColumna - 1] = 0;
                    break;
                default:


                    unidad4.sprite = spriteUnidad1;
                    cuadro4.sprite = spriteCuadro1;
                    tropaTemporal[indiceColumna - 1] = 1;
                    break;
            }

        }

    }

    public void CambiarUnidadArriba()
    {

        int unidadACambiar = tropaTemporal[indiceColumna - 1];

        if (indiceColumna == 1)
        {//el primer index es especial, porque debe tener un personaje sí o sí. El resto pueden estar vacíos, así que deben tratarse aparte.
         //se toma el int que haya en la primera posicion de la lista
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1: //los datos y el personaje deben pasar a ser los del arquero, así como el index de la lista
                    unidad1.sprite = spriteUnidad4;
                    cuadro1.sprite = spriteCuadro4;
                    tropaTemporal[0] = 4;
                    break;
                case 2://los datos y el personaje deben pasar a ser los del mago, así como el index de la lista
                    unidad1.sprite = spriteUnidad1;
                    cuadro1.sprite = spriteCuadro1;
                    tropaTemporal[0] = 1;
                    break;
                case 3:
                    //los datos y el personaje deben pasar a ser los del lancero, así como el index de la lista
                    unidad1.sprite = spriteUnidad2;
                    cuadro1.sprite = spriteCuadro2;
                    tropaTemporal[0] = 2;
                    break;
                case 4:
                    //los datos y el personaje deben pasar a ser los del clérigo, así como el index de la lista
                    unidad1.sprite = spriteUnidad3;
                    cuadro1.sprite = spriteCuadro3;
                    tropaTemporal[0] = 3;
                    break;
            }
        }

        else if (indiceColumna == 2)
        { //el código es casi igual; sólo se contempla que no haya ningún personaje asignado (index con valor = 0)
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1:
                    tropaTemporal[indiceColumna - 1] = 0;
                    unidad2.sprite = null;
                    cuadro2.sprite = null;
                    break;
                case 2://los datos y el personaje deben pasar a ser los del mago, así como el index de la lista
                    unidad2.sprite = spriteUnidad1;
                    cuadro2.sprite = spriteCuadro1;
                    tropaTemporal[indiceColumna - 1] = 1;
                    break;
                case 3:
                    //los datos y el personaje deben pasar a ser los del lancero, así como el index de la lista
                    unidad2.sprite = spriteUnidad2;
                    cuadro2.sprite = spriteCuadro2;
                    tropaTemporal[indiceColumna - 1] = 2;
                    break;
                case 4:
                    //los datos y el personaje deben pasar a ser los del clérigo, así como el index de la lista
                    unidad2.sprite = spriteUnidad3;
                    cuadro2.sprite = spriteCuadro3;
                    tropaTemporal[indiceColumna - 1] = 3;
                    break;
                default:

                    //los datos y el personaje deben pasar a ser los del arquero, así como el index de la lista
                    unidad2.sprite = spriteUnidad4;
                    cuadro2.sprite = spriteCuadro4;
                    tropaTemporal[indiceColumna - 1] = 4;
                    break;
            }
        }

        else if (indiceColumna == 3)
        { //el código es casi igual; sólo se contempla que no haya ningún personaje asignado (index con valor = 0)
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1:
                    tropaTemporal[indiceColumna - 1] = 0;
                    unidad3.sprite = null;
                    cuadro3.sprite = null;
                    break;
                case 2://los datos y el personaje deben pasar a ser los del mago, así como el index de la lista
                    unidad3.sprite = spriteUnidad1;
                    cuadro3.sprite = spriteCuadro1;
                    tropaTemporal[indiceColumna - 1] = 1;
                    break;
                case 3:
                    //los datos y el personaje deben pasar a ser los del lancero, así como el index de la lista
                    unidad3.sprite = spriteUnidad2;
                    cuadro3.sprite = spriteCuadro2;
                    tropaTemporal[indiceColumna - 1] = 2;
                    break;
                case 4:
                    //los datos y el personaje deben pasar a ser los del clérigo, así como el index de la lista
                    unidad3.sprite = spriteUnidad3;
                    cuadro3.sprite = spriteCuadro3;
                    tropaTemporal[indiceColumna - 1] = 3;
                    break;
                default:

                    //los datos y el personaje deben pasar a ser los del arquero, así como el index de la lista
                    unidad3.sprite = spriteUnidad4;
                    cuadro3.sprite = spriteCuadro4;
                    tropaTemporal[indiceColumna - 1] = 4;
                    break;
            }

        }

        else
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1:  //se está decidiendo que no se quiere tener un personaje en este slot
                    tropaTemporal[indiceColumna - 1] = 0;
                    unidad4.sprite = null;
                    cuadro4.sprite = null;
                    break;
                case 2://los datos y el personaje deben pasar a ser los del mago, así como el index de la lista
                    unidad4.sprite = spriteUnidad1;
                    cuadro4.sprite = spriteCuadro1;
                    tropaTemporal[indiceColumna - 1] = 1;
                    break;
                case 3:
                    //los datos y el personaje deben pasar a ser los del lancero, así como el index de la lista
                    unidad4.sprite = spriteUnidad2;
                    cuadro4.sprite = spriteCuadro2;
                    tropaTemporal[indiceColumna - 1] = 2;
                    break;
                case 4:
                    //los datos y el personaje deben pasar a ser los del clérigo, así como el index de la lista
                    unidad4.sprite = spriteUnidad3;
                    cuadro4.sprite = spriteCuadro3;
                    tropaTemporal[indiceColumna - 1] = 3;
                    break;
                default: //los datos y el personaje deben pasar a ser los del arquero, así como el index de la lista
                    unidad4.sprite = spriteUnidad4;
                    cuadro4.sprite = spriteCuadro4;
                    tropaTemporal[indiceColumna - 1] = 4;
                    break;
            }

        }

    }

    public void VolverAlMenu()
    {

        TropaEscogida.SetTropa(tropaTemporal);
        TropaEscogida.SetArmasJugador(new List<int>() { 0, 0, 0, 0 });
        cursorSFX = Resources.Load<AudioClip>("Audio/Decision2");
        audioSource.PlayOneShot(cursorSFX, 1f);
        SceneManager.LoadScene(5);


    }

}

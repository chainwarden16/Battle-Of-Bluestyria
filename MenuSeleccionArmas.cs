using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuSeleccionArmas : MonoBehaviour
{
    private SpriteRenderer heroe, cuadro1, cuadro2, cuadro3, cuadro4, flechaIzquierda, flechaDerecha, flechaArriba, flechaAbajo, MarcoSeleccion, arma1, arma2, arma3, arma4, cuadro, arma, estadistica1, estadistica2, estadistica3, estadistica4, estadistica;

    private TextMeshProUGUI unidad1, unidad2, unidad3, unidad4, unidad;

    private TextMeshPro extra1, extra2, extra3, extra4, extra;

    private List<int> armasJugador = new List<int>() { };

    private AudioSource audioSource;
    private AudioClip cursorSFX;


    //Indica la posición inicial del marco de selección. Al cargar la escena, este se encontrará en el primer personaje

    private int indiceColumna = 1;

    private List<int> tropaTemporal = TropaEscogida.GetTropa();

    // Start is called before the first frame update
    void Start()
    {
        armasJugador.AddRange(TropaEscogida.GetArmasJugador());
        audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        cuadro1 = GameObject.Find("IconoArma1").GetComponent<SpriteRenderer>();
        cuadro2 = GameObject.Find("IconoArma2").GetComponent<SpriteRenderer>();
        cuadro3 = GameObject.Find("IconoArma3").GetComponent<SpriteRenderer>();
        cuadro4 = GameObject.Find("IconoArma4").GetComponent<SpriteRenderer>();

        arma1 = GameObject.Find("Arma1").GetComponent<SpriteRenderer>();
        arma2 = GameObject.Find("Arma2").GetComponent<SpriteRenderer>();
        arma3 = GameObject.Find("Arma3").GetComponent<SpriteRenderer>();
        arma4 = GameObject.Find("Arma4").GetComponent<SpriteRenderer>();

        unidad1 = arma1.GetComponentInChildren<TextMeshProUGUI>();
        unidad2 = arma2.GetComponentInChildren<TextMeshProUGUI>();
        unidad3 = arma3.GetComponentInChildren<TextMeshProUGUI>();
        unidad4 = arma4.GetComponentInChildren<TextMeshProUGUI>();

        flechaAbajo = GameObject.Find("Flecha-Abajo").GetComponent<SpriteRenderer>();
        flechaArriba = GameObject.Find("Flecha-Arriba").GetComponent<SpriteRenderer>();
        flechaDerecha = GameObject.Find("Flecha-Derecha").GetComponent<SpriteRenderer>();
        flechaIzquierda = GameObject.Find("Flecha-Izquierda").GetComponent<SpriteRenderer>();
        MarcoSeleccion = GameObject.Find("Marco-Seleccion").GetComponent<SpriteRenderer>();

        estadistica1 = GameObject.Find("EstadisticaExtra1").GetComponent<SpriteRenderer>();
        estadistica2 = GameObject.Find("EstadisticaExtra2").GetComponent<SpriteRenderer>();
        estadistica3 = GameObject.Find("EstadisticaExtra3").GetComponent<SpriteRenderer>();
        estadistica4 = GameObject.Find("EstadisticaExtra4").GetComponent<SpriteRenderer>();

        extra1 = estadistica1.GetComponentInChildren<TextMeshPro>();
        extra2 = estadistica2.GetComponentInChildren<TextMeshPro>();
        extra3 = estadistica3.GetComponentInChildren<TextMeshPro>();
        extra4 = estadistica4.GetComponentInChildren<TextMeshPro>();

        for (int numero = 0; numero< tropaTemporal.Count;numero++)
        {
            heroe = GameObject.Find("Heroe" + (numero + 1)).GetComponent<SpriteRenderer>();
            cuadro = GameObject.Find("IconoArma" + (numero + 1)).GetComponent<SpriteRenderer>();
            cuadro.color = new Color(1, 1, 1, 1);
            arma = GameObject.Find("Arma"+(numero+1)).GetComponent<SpriteRenderer>();
            estadistica = GameObject.Find("EstadisticaExtra" + (numero + 1)).GetComponent<SpriteRenderer>();
            unidad = arma.GetComponentInChildren<TextMeshProUGUI>();
            extra = estadistica.GetComponentInChildren<TextMeshPro>();

            if (tropaTemporal[numero] == 0)
            {
                //No hay unidad, asi que no hay arma que crear

                Color co = new Color(1, 1, 1, 0);

                cuadro.color = co;

                unidad.color = co;

                arma.color = co;

                estadistica.color = co;

                extra.color = co;

                heroe.color = co;

            }
            else if (tropaTemporal[numero] == 1)
            {

                cuadro.sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Tomo");
                heroe.sprite = Resources.Load<Sprite>("Sprites/Menu/Mago-Seleccion");
                if (armasJugador[numero] == 0)
                {
                    
                    unidad.text = "Tomo";
                    extra.text = "+ Ataque";
                }
                else
                {
                    
                    unidad.text = "Códex";
                    extra.text = "+ Defensa";
                }

            }
            else if (tropaTemporal[numero] == 2)
            {

                cuadro.sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Lanza");
                heroe.sprite = Resources.Load<Sprite>("Sprites/Menu/Lancero-Seleccion");
                if (armasJugador[numero] == 0)
                {
                    
                    unidad.text = "Lanza";
                    extra.text = "+ Ataque";
                }
                else
                {
                   
                    unidad.text = "Pica";
                    extra.text = "+ Defensa";
                }

            }
            else if (tropaTemporal[numero] == 3)
            {
                heroe.sprite = Resources.Load<Sprite>("Sprites/Menu/Clerigo-Seleccion");
                cuadro.sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Baculo");
                if (armasJugador[numero] == 0)
                {
                    
                    unidad.text = "Báculo";
                    extra.text = "+ Ataque";
                }
                else
                {
                    
                    unidad.text = "Ángelus";
                    extra.text = "+ Defensa";
                }

            }
            else if (tropaTemporal[numero] == 4)
            {
                heroe.sprite = Resources.Load<Sprite>("Sprites/Menu/Arquero-Seleccion");
                cuadro.sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Arco");
                if (armasJugador[numero] == 0)
                {
                    
                    unidad.text = "Arco";
                    extra.text = "+ Ataque";
                }
                else
                {
                    
                    unidad.text = "Platiun";
                    extra.text = "+ Defensa";
                }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {

            CambiarUnidadAbajo();

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

    public void MoverElementosDerecha()
    {
        
        //Si el jugador hace click en la flecha derecha, el marco de seleccion y las flechas deben desplazarse, a menos que se encuentren en el último personaje

        if (indiceColumna == 1)
        {

            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }

            indiceColumna++;
            MarcoSeleccion.transform.position = new Vector3(-1.75f, -0.016f, 0);

            flechaAbajo.transform.position = new Vector3(-1.69f, -0.67f, 0);
            flechaDerecha.transform.position = new Vector3(-0.23f, 0, 0);
            flechaArriba.transform.position = new Vector3(-1.69f, 0.67f, 0);
            flechaIzquierda.transform.position = new Vector3(-3.29f, 0.01f, 0);

            Color nuevoColor = new Color(1, 1, 1, 1);
            flechaIzquierda.GetComponent<SpriteRenderer>().color = nuevoColor;

        }

        else if (indiceColumna == 2)
        {

            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna++;
            MarcoSeleccion.transform.position = new Vector3(1.28f, -0.016f, 0);

            flechaAbajo.transform.position = new Vector3(1.34f, -0.67f, 0);
            flechaDerecha.transform.position = new Vector3(2.8f, 0, 0);
            flechaArriba.transform.position = new Vector3(1.34f, 0.67f, 0);
            flechaIzquierda.transform.position = new Vector3(-0.2f, 0.01f, 0);
        }

        else if (indiceColumna == 3)
        {

            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna++;
            MarcoSeleccion.transform.position = new Vector3(4.35f, -0.016f, 0);

            flechaAbajo.transform.position = new Vector3(4.41f, -0.67f, 0);
            flechaDerecha.transform.position = new Vector3(5.87f, 0, 0);
            flechaArriba.transform.position = new Vector3(4.41f, 0.67f, 0);
            flechaIzquierda.transform.position = new Vector3(2.81f, 0.01f, 0);
            Color nuevoColor = new Color(1, 1, 1, 0);
            flechaDerecha.GetComponent<SpriteRenderer>().color = nuevoColor;
        }
    }

    public void MoverElementosIzquierda()
    {

       
        //Si el jugador hace click en la flecha izquierda, el marco de seleccion y las flechas deben desplazarse, a menos que se encuentren en el primer personaje

        if (indiceColumna == 2)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna--;
            MarcoSeleccion.transform.position = new Vector3(-4.8f,-0.016f,0);

            flechaAbajo.transform.position = new Vector3(-4.74f,-0.67f,0);
            flechaDerecha.transform.position = new Vector3(-3.28f, 0,0);
            flechaArriba.transform.position = new Vector3(-4.74f,0.67f,0);
            flechaIzquierda.transform.position = new Vector3(-6.34f,0.01f,0);

            Color nuevoColor = new Color(1, 1, 1, 0);
            flechaIzquierda.GetComponent<SpriteRenderer>().color = nuevoColor;


        }

        else if (indiceColumna == 3)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna--;
            MarcoSeleccion.transform.position = new Vector3(-1.75f, -0.016f, 0);

            flechaAbajo.transform.position = new Vector3(-1.69f, -0.67f, 0);
            flechaDerecha.transform.position = new Vector3(-0.23f, 0, 0);
            flechaArriba.transform.position = new Vector3(-1.69f, 0.67f, 0);
            flechaIzquierda.transform.position = new Vector3(-3.29f, 0.01f, 0);

        }

        else if (indiceColumna == 4)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            indiceColumna--;
            MarcoSeleccion.transform.position = new Vector3(1.28f, -0.016f, 0);

            flechaAbajo.transform.position = new Vector3(1.34f, -0.67f, 0);
            flechaDerecha.transform.position = new Vector3(2.8f, 0, 0);
            flechaArriba.transform.position = new Vector3(1.34f, 0.67f, 0);
            flechaIzquierda.transform.position = new Vector3(-0.24f, 0.01f, 0);
            Color nuevoColor = new Color(1, 1, 1, 1);
            flechaDerecha.GetComponent<SpriteRenderer>().color = nuevoColor;
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
                    cuadro1.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Tomo");
                    if (armasJugador[0] == 0)
                    {
                        armasJugador[0] = 1;
                        unidad1.text = "Códex";
                        extra1.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[0] = 0;
                        unidad1.text = "Tomo";
                        extra1.text = "+ Ataque";
                    }
                    
                    
                    break;
                case 2://los datos y el personaje deben pasar a ser los del clérigo, así como el index de la lista
                    cuadro1.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Lanza");
                    if (armasJugador[0] == 0)
                    {
                        armasJugador[0] = 1;
                        unidad1.text = "Pica";
                        extra1.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[0] = 0;
                        unidad1.text = "Lanza";
                        extra1.text = "+ Ataque";
                    }
                    break;
                case 3:
                    //los datos y el personaje deben pasar a ser los del arquero, así como el index de la lista
                    cuadro1.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Baculo");
                    if (armasJugador[0] == 0)
                    {
                        armasJugador[0] = 1;
                        unidad1.text = "Ángelus";
                        extra1.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[0] = 0;
                        unidad1.text = "Báculo";
                        extra1.text = "+ Ataque";
                    }
                    break;
                case 4:
                    //los datos y el personaje deben pasar a ser los del mago, así como el index de la lista
                    cuadro1.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Arco");
                    if (armasJugador[0] == 0)
                    {
                        armasJugador[0] = 1;
                        unidad1.text = "Platiun";
                        extra1.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[0] = 0;
                        unidad1.text = "Arco";
                        extra1.text = "+ Ataque";
                    }
                    break;
            }
        }

        else if(indiceColumna==2)
        { //el código es casi igual; sólo se contempla que no haya ningún personaje asignado (index con valor = 0)
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1: //los datos y el personaje deben pasar a ser los del lancero, así como el index de la lista
                    cuadro2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Tomo");
                    if (armasJugador[1] == 0)
                    {
                        armasJugador[1] = 1;
                        unidad2.text = "Códex";
                        extra2.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[1] = 0;
                        unidad2.text = "Tomo";

                        extra2.text = "+ Ataque";
                    }


                    break;
                case 2://los datos y el personaje deben pasar a ser los del clérigo, así como el index de la lista
                    cuadro2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Lanza");
                    if (armasJugador[1] == 0)
                    {
                        armasJugador[1] = 1;
                        unidad2.text = "Pica";
                        extra2.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[1] = 0;
                        unidad2.text = "Lanza";
                        extra2.text = "+ Ataque";
                    }
                    break;
                case 3:
                    //los datos y el personaje deben pasar a ser los del arquero, así como el index de la lista
                    cuadro2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Baculo");
                    if (armasJugador[1] == 0)
                    {
                        armasJugador[1] = 1;
                        unidad2.text = "Ángelus";
                        extra2.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[1] = 0;
                        unidad2.text = "Báculo";
                        extra2.text = "+ Ataque";
                    }
                    break;
                case 4:
                    //los datos y el personaje deben pasar a ser los del mago, así como el index de la lista
                    cuadro2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Arco");
                    if (armasJugador[1] == 0)
                    {
                        armasJugador[1] = 1;
                        unidad2.text = "Platiun";
                        extra2.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[1] = 0;
                        unidad2.text = "Arco";
                        extra2.text = "+ Ataque";
                    }
                    break;
                default:

                    //No hay unidad, asi que no hay arma que crear
                    unidad2.color = new Color(1, 1, 1, 0);
                    cuadro2.color = new Color(1, 1, 1, 0);
                    arma2.color = new Color(1, 1, 1, 0);

                    break;
            }
        }

        else if (indiceColumna == 3)
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1: //los datos y el personaje deben pasar a ser los del lancero, así como el index de la lista
                    cuadro3.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Tomo");
                    if (armasJugador[2] == 0)
                    {
                        armasJugador[2] = 1;
                        unidad3.text = "Códex";
                        extra3.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[2] = 0;
                        unidad3.text = "Tomo";
                        extra3.text = "+ Ataque";
                    }


                    break;
                case 2://los datos y el personaje deben pasar a ser los del clérigo, así como el index de la lista
                    cuadro3.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Lanza");
                    if (armasJugador[2] == 0)
                    {
                        armasJugador[2] = 1;
                        unidad3.text = "Pica";
                        extra3.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[2] = 0;
                        unidad3.text = "Lanza";
                        extra3.text = "+ Ataque";
                    }
                    break;
                case 3:
                    //los datos y el personaje deben pasar a ser los del arquero, así como el index de la lista
                    cuadro3.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Baculo");
                    if (armasJugador[2] == 0)
                    {
                        armasJugador[2] = 1;
                        unidad3.text = "Ángelus"; 
                        extra3.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[2] = 0;
                        unidad3.text = "Báculo";
                        extra3.text = "+ Ataque";
                    }
                    break;
                case 4:
                    //los datos y el personaje deben pasar a ser los del mago, así como el index de la lista
                    cuadro3.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Arco");
                    if (armasJugador[2] == 0)
                    {
                        armasJugador[2] = 1;
                        unidad3.text = "Platiun";
                        extra3.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[2] = 0;
                        unidad3.text = "Arco";
                        extra3.text = "+ Ataque";
                    }
                    break;
                default:

                    //No hay unidad, asi que no hay arma que crear
                    unidad3.color = new Color(1, 1, 1, 0);
                    cuadro3.color = new Color(1, 1, 1, 0);
                    arma3.color = new Color(1, 1, 1, 0);

                    break;
            }

        }

        else
        {
            if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Cursor1"); audioSource.PlayOneShot(cursorSFX, 1f); }
            switch (unidadACambiar)
            {
                case 1: //los datos y el personaje deben pasar a ser los del lancero, así como el index de la lista
                    cuadro4.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Tomo");
                    if (armasJugador[3] == 0)
                    {
                        armasJugador[3] = 1;
                        unidad4.text = "Códex";
                        extra4.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[3] = 0;
                        unidad4.text = "Tomo";
                        extra4.text = "+ Ataque";
                    }


                    break;
                case 2://los datos y el personaje deben pasar a ser los del clérigo, así como el index de la lista
                    cuadro4.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Lanza");
                    if (armasJugador[3] == 0)
                    {
                        armasJugador[3] = 1;
                        unidad4.text = "Pica";
                        extra4.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[3] = 0;
                        unidad4.text = "Lanza";
                        extra4.text = "+ Ataque";
                    }
                    break;
                case 3:
                    //los datos y el personaje deben pasar a ser los del arquero, así como el index de la lista
                    cuadro4.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Baculo");
                    if (armasJugador[3] == 0)
                    {
                        armasJugador[3] = 1;
                        unidad4.text = "Ángelus";
                        extra4.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[3] = 0;
                        unidad4.text = "Báculo";
                        extra4.text = "+ Ataque";
                    }
                    break;
                case 4:
                    //los datos y el personaje deben pasar a ser los del mago, así como el index de la lista
                    cuadro4.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Arco");
                    if (armasJugador[3] == 0)
                    {
                        armasJugador[3] = 1;
                        unidad4.text = "Platiun";
                        extra4.text = "+ Defensa";
                    }
                    else
                    {
                        armasJugador[3] = 0;
                        unidad4.text = "Arco";
                        extra4.text = "+ Ataque";
                    }
                    break;
                default:

                    //No hay unidad, asi que no hay arma que crear
                    unidad4.color = new Color(1, 1, 1, 0);
                    cuadro4.color = new Color(1, 1, 1, 0);
                    arma4.color = new Color(1, 1, 1, 0);

                    break;
            }
        }

    }


    public void VolverAlMenu()
    {
        if (!audioSource.isPlaying) { cursorSFX = Resources.Load<AudioClip>("Audio/Decision2"); audioSource.PlayOneShot(cursorSFX, 1f); }
        TropaEscogida.SetArmasJugador(armasJugador);
        SceneManager.LoadScene(5);


    }
}

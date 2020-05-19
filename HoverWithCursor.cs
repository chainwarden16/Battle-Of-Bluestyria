using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HoverWithCursor : MonoBehaviour

{

    private GameObject panelEstadisticas;

    private GameObject iconoEscudado;

    private Camera camara;

    private GameObject cursor;

    private GameObject panelNombre;

    private GameObject panelArma;

    private GameObject iconoArma;

    private GameObject iconoEstado;

    private GameObject iconoBuff;

    private TextMeshPro ps;

    private TextMeshPro fuer;

    private TextMeshPro mag;

    private TextMeshPro def;

    private TextMeshPro defM;

    private TextMeshPro agi;

    private TextMeshPro fuerza;

    private TextMeshPro agilidad;

    private TextMeshPro magia;

    private TextMeshPro psActuales;

    private TextMeshPro psMaximos;

    private TextMeshPro defensa;

    private TextMeshPro defensaM;

    private TextMeshPro nombre;

    private TextMeshPro arma;

    private Clase clase;

    private Unidad unidad;

    void OnEnable()
    {

        cursor = GameObject.Find("Cursor");
        
    }


    public void OnTriggerEnter2D(Collider2D col)
    {

        clase = gameObject.GetComponent<Clase>();

        unidad = gameObject.GetComponent<Unidad>();

        camara = Camera.main;

        iconoArma = GameObject.Find("Icono-Arma");
        iconoBuff = GameObject.Find("Buff");
        iconoEstado = GameObject.Find("Estado");
        iconoEscudado = GameObject.Find("Escudado");

        panelEstadisticas = GameObject.Find("Estadisticas-Unidad");
        defensaM = GameObject.Find("Defensa-Magica-Cantidad").GetComponent<TextMeshPro>();
        defensa = GameObject.Find("Defensa-Cantidad").GetComponent<TextMeshPro>();
        fuerza = GameObject.Find("Fuerza-Cantidad").GetComponent<TextMeshPro>();
        magia = GameObject.Find("Magia-Cantidad").GetComponent<TextMeshPro>();
        psActuales = GameObject.Find("PS-Cantidad-Actual").GetComponent<TextMeshPro>();
        psMaximos = GameObject.Find("PS-Cantidad-Total").GetComponent<TextMeshPro>();
        agilidad = GameObject.Find("Agilidad-Cantidad").GetComponent<TextMeshPro>();

        defM = GameObject.Find("Defensa-Magica").GetComponent<TextMeshPro>();
        def = GameObject.Find("Defensa").GetComponent<TextMeshPro>();
        fuer = GameObject.Find("Fuerza").GetComponent<TextMeshPro>();
        mag = GameObject.Find("Magia").GetComponent<TextMeshPro>();
        ps = GameObject.Find("PS").GetComponent<TextMeshPro>();
        agi = GameObject.Find("Agilidad").GetComponent<TextMeshPro>();

        panelNombre = GameObject.Find("Nombre-Unidad");
        panelArma = GameObject.Find("Arma-Unidad");
        nombre = panelNombre.GetComponentInChildren<TextMeshPro>();
        arma = panelArma.GetComponentInChildren<TextMeshPro>();

        Color colOpaco = new Color(1f, 1f, 1f, 1f);


        if (col.gameObject.tag == "Cursor")
        {
            defensaM.text = ((clase.GetDefensaM() + clase.GetArma().GetDefensas() +clase.GetDefensaM() * Convert.ToInt32(unidad.GetBuffs().Contains(0)))).ToString();
            defensa.text = ((clase.GetDefensa()+clase.GetArma().GetDefensas()+clase.GetDefensa() * Convert.ToInt32(unidad.GetBuffs().Contains(0)))).ToString();
            fuerza.text = ((clase.GetArma().GetAtaque()+clase.GetAtaque()+clase.GetAtaque()* Convert.ToInt32(unidad.GetBuffs().Contains(1)))).ToString();
            magia.text = ((clase.GetArma().GetAtaque() + clase.GetMagia() + clase.GetMagia() * Convert.ToInt32(unidad.GetBuffs().Contains(1)))).ToString();
            


            if (unidad.GetBuffs().Contains(2))
            {
                defensa.text = (Int32.Parse(defensa.text) / 2).ToString();
                defensaM.text = (Int32.Parse(defensaM.text) / 2).ToString();
            }else if (unidad.GetBuffs().Contains(3))
            {

                fuerza.text = (Int32.Parse(fuerza.text) / 2).ToString();
                magia.text = (Int32.Parse(magia.text) / 2).ToString();

            }

            
            agilidad.text = clase.GetAgilidad().ToString();
            psActuales.text = clase.GetPSAct().ToString();
            psMaximos.text = " / "+clase.GetPSMax().ToString();
            nombre.text = unidad.getNombre();
            arma.text = clase.GetArma().GetNombre();

            if (clase.GetTipoClase() == 1)
            {
                iconoArma.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Tomo");
            }else if (clase.GetTipoClase() == 2)
            {
                iconoArma.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Lanza");
            }else if (clase.GetTipoClase() == 3)
            {
                iconoArma.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Baculo");
            }
            else
            {
                iconoArma.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Arma-Arco");
            }

            //se miran los estados

            if (unidad.GetEstados().Contains(0)) //envenenado
            {
                iconoEstado.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Estado-Envenenado");
            }
            else if (unidad.GetEstados().Contains(1)) // quemado
            {
                iconoEstado.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Estado-Quemado");

            }
            else if (unidad.GetEstados().Contains(2)) //paralizado
            {
                iconoEstado.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Estado-Paralizado");

            }
            else if (unidad.GetEstados().Contains(3)) //dormido
            {
                iconoEstado.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Estado-Dormido");
            }
            else //si no tiene estados
            {
                iconoEstado.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Estado-Neutral");
            }

            if (unidad.GetEstaSiendoEscudado().Item1)
            {
                iconoEscudado.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Icono-Escudado");
                iconoEscudado.GetComponent<SpriteRenderer>().color = colOpaco;
            }

            //se miran los buffs/debuffs
            Color coloR = new Color32(255, 0, 0, 255);
            Color coloB = new Color32(0, 190, 255, 255);
            Color colorN = new Color32(0, 0, 0, 255);

            if (unidad.GetBuffs().Contains(0)) //Fortificar
            {
                iconoBuff.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Buff-Mejora");

                
                defensa.color = Color.red;
                defensaM.color = Color.red;

                def.color = Color.red;
                defM.color = Color.red;

                fuer.color = colorN;
                mag.color = colorN;

                fuerza.color = colorN;
                magia.color = colorN;


            }
            else if (unidad.GetBuffs().Contains(1)) // Reforzar
            {
                iconoBuff.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Buff-Mejora");
                
                fuerza.color = Color.red;
                magia.color = Color.red;


                fuer.color = Color.red;
                mag.color = Color.red;

                def.color = colorN;
                defM.color = colorN;

                defensa.color = colorN;
                defensaM.color = colorN;

                

            }
            else if (unidad.GetBuffs().Contains(2)) //debilitar
            {
                iconoBuff.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Buff-Empeora");
                defensa.color = Color.blue;
                defensaM.color = Color.blue;

                def.color = Color.blue;
                defM.color = Color.blue;

                fuerza.color = colorN;
                magia.color = colorN;

                fuer.color = colorN;
                mag.color = colorN;
            }
            else if (unidad.GetBuffs().Contains(3)) //cansar
            {
                iconoBuff.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Buff-Empeora");
                fuerza.color = Color.blue;
                magia.color = Color.blue;


                fuer.color = Color.blue;
                mag.color = Color.blue;

                def.color = colorN;
                defM.color = colorN;

                defensa.color = colorN;
                defensaM.color = colorN;
            }
            else if(unidad.GetBuffs().Count==0) //si no tiene estados
            {
                iconoBuff.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menu/Estado-Neutral");
                fuerza.color = colorN;
                magia.color = colorN;
                defensa.color = colorN;
                defensaM.color = colorN;

                fuer.color = colorN;
                mag.color = colorN;
                def.color = colorN;
                defM.color = colorN;
            }


            panelArma.GetComponent<SpriteRenderer>().color = colOpaco;
            panelEstadisticas.GetComponent<SpriteRenderer>().color = colOpaco;
            panelNombre.GetComponent<SpriteRenderer>().color = colOpaco;

            nombre.color = colorN;
            arma.color = colorN;

            psMaximos.color = colorN;
            psActuales.color = colorN;
            
            agilidad.color = colorN;

            ps.color = colorN;
            
            agi.color = colorN;


            iconoArma.GetComponent<SpriteRenderer>().color = colOpaco;
            iconoBuff.GetComponent<SpriteRenderer>().color = colOpaco;
            iconoEstado.GetComponent<SpriteRenderer>().color = colOpaco;
            
            /*
            panelNombre.transform.position = camara.ViewportToWorldPoint(new Vector3(0.12f, 0.7f, 0f));
            panelEstadisticas.transform.position = new Vector3(panelNombre.transform.position.x, panelNombre.transform.position.y - 1.8f, 0f);
            panelArma.transform.position = new Vector3(panelEstadisticas.transform.position.x,panelEstadisticas.transform.position.y-1.8f,0f);
            */
        }
    }

   public void OnTriggerExit2D(Collider2D col)
    {
        Color colTrans = new Color(1f, 1f, 1f, 0f);

        panelEstadisticas = GameObject.Find("Estadisticas-Unidad");
        defensaM = GameObject.Find("Defensa-Magica-Cantidad").GetComponent<TextMeshPro>();
        defensa = GameObject.Find("Defensa-Cantidad").GetComponent<TextMeshPro>();
        fuerza = GameObject.Find("Fuerza-Cantidad").GetComponent<TextMeshPro>();
        magia = GameObject.Find("Magia-Cantidad").GetComponent<TextMeshPro>();
        psActuales = GameObject.Find("PS-Cantidad-Actual").GetComponent<TextMeshPro>();
        psMaximos = GameObject.Find("PS-Cantidad-Total").GetComponent<TextMeshPro>();
        agilidad = GameObject.Find("Agilidad-Cantidad").GetComponent<TextMeshPro>();

        defM = GameObject.Find("Defensa-Magica").GetComponent<TextMeshPro>();
        def = GameObject.Find("Defensa").GetComponent<TextMeshPro>();
        fuer = GameObject.Find("Fuerza").GetComponent<TextMeshPro>();
        mag = GameObject.Find("Magia").GetComponent<TextMeshPro>();
        ps = GameObject.Find("PS").GetComponent<TextMeshPro>();
        agi = GameObject.Find("Agilidad").GetComponent<TextMeshPro>();

        panelNombre = GameObject.Find("Nombre-Unidad");
        panelArma = GameObject.Find("Arma-Unidad");
        nombre = panelNombre.GetComponentInChildren<TextMeshPro>();
        arma = panelArma.GetComponentInChildren<TextMeshPro>();
        iconoArma = GameObject.Find("Icono-Arma");
        iconoBuff = GameObject.Find("Buff");
        iconoEstado = GameObject.Find("Estado");


        if (col.gameObject.tag == "Cursor")
        {
            

            panelArma.GetComponent<SpriteRenderer>().color = colTrans;
            panelEstadisticas.GetComponent<SpriteRenderer>().color = colTrans;
            panelNombre.GetComponent<SpriteRenderer>().color = colTrans;

            defensa.color = colTrans;
            defensaM.color = colTrans;
            magia.color = colTrans;
            fuerza.color = colTrans;
            agilidad.color = colTrans;

            def.color = colTrans;
            defM.color = colTrans;
            mag.color = colTrans;
            fuer.color = colTrans;
            agi.color = colTrans;
            ps.color = colTrans;

            nombre.color = colTrans;
            arma.color = colTrans;
            psMaximos.color = colTrans;
            psActuales.color = colTrans;
            iconoEstado.GetComponent<SpriteRenderer>().color = colTrans;
            iconoArma.GetComponent<SpriteRenderer>().color = colTrans;
            iconoBuff.GetComponent<SpriteRenderer>().color = colTrans;
            iconoEscudado.GetComponent<SpriteRenderer>().color = colTrans;
        }
    }


}

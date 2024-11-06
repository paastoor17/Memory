using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    public GameObject figura;
    public Material image;
    public static bool startVar = false;

    private GameObject gm;
    private int id;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController"); // Encuentra el GameManager
        Renderer renderer = figura.GetComponent<Renderer>();
        renderer.material = image;
    }

    void OnMouseDown()
    {
        // Comprueba si el juego ha comenzado
        if (!startVar) return;

        // Permite la acción si se puede hacer clic
        if (gm.GetComponent<GameManager>().canClickTrigger())
        {
            gm.GetComponent<GameManager>().setClickTrigger(true);
            AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("FigureDown"))
            {
                if (gm.GetComponent<GameManager>().hiHaPuesto())
                {
                    Animator animator = GetComponent<Animator>();
                    animator.SetTrigger("FigureShowTrigger");
                    gm.GetComponent<GameManager>().cardTouched(gameObject);
                }
            }
        }
    }

    // Método para ocultar la carta
    public void esconder()
    {
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("FigureUp"))
        {
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("FigureHideTrigger");
        }
    }

    // Método para mostrar la carta
    public void mostrar()
    {
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("FigureDown"))
        {
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("FigureShowTrigger");
        }
    }

    public void setId(double newId)
    {
        id = (int)newId;
    }

    public double getId()
    {
        return id;
    }

    public Renderer getFiguraRenderer()
    {
        return figura.GetComponent<Renderer>();
    }

    public void setStartVar(bool val)
    {
        startVar = val;
    }
}

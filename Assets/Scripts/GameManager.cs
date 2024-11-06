using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] cards; // Lista de referencias directas a las cartas
    public GameObject[] cardSons; // Lista de referencias a las cartas como render, es decir, los hijos de los objetos
    private double id_double = 0; // ID para asignar parejas de cartas
    private GameObject[] cardsSelected = new GameObject[2]; // Lista de las dos cartas seleccionadas
    private int cartesAdivinades;
    private bool clickTrigger = false; // Permiso para hacer clic en las cartas
    private float clickCooldown = 0; // Un retraso para activar el permiso de hacer clic
    private bool startGameTrigger = false; // Variable para permitir el inicio del juego
    public Button startButton;
    public TextMeshProUGUI timeText;
    private double timeNum; // Tiempo total
    private int intentsNum;
    public TextMeshProUGUI titolText;
    public TextMeshProUGUI intentsText;
    public AudioClip[] audioClips; // Lista de audios
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // Al iniciar declaramos el listener del botón de start.
        if (startButton != null)
        {
            startButton.onClick.AddListener(AccionBoton);
        }
        titolText.text = "Emborracharse Memory";
        intentsNum = 0;
        intentsText.text = "Intentos: " + intentsNum;
        timeText.text = "Timer: " + 0;
        cardsSelected[0] = null;
        cardsSelected[1] = null;
        cartesAdivinades = 0;
        cards = GameObject.FindGameObjectsWithTag("CardTag");
        cardSons = GameObject.FindGameObjectsWithTag("CardTagSon");
    }

    // Update is called once per frame
    void Update()
    {
        // Tiempo de la partida
        if (startGameTrigger)
        {
            timeNum += Time.deltaTime;
            timeText.text = "Timer: " + (int)timeNum;
        }
        // Retraso para poder pulsar alguna tecla
        if (clickTrigger)
        {
            clickCooldown += Time.deltaTime;
            if (clickCooldown >= 1)
            {
                clickCooldown = 0;
                clickTrigger = false;
            }
        }
        // Comprueba si hay dos cartas seleccionadas
        if (cardsSelected[0] != null && cardsSelected[1] != null)
        {
            // Si es así, comprueba que las cartas estén en el estado Up, mostrando la figura
            AnimatorStateInfo stateInfo0 = cardsSelected[0].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo stateInfo1 = cardsSelected[1].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            // Una vez estén Up, comprobamos las ids...
            if (stateInfo0.IsName("FigureUp") && stateInfo1.IsName("FigureUp"))
            {
                checkIds();
            }
        }
        // Si se han adivinado 8 parejas, entonces se acaba el juego.
        if (cartesAdivinades == 8)
        {
            audioSource.PlayOneShot(audioClips[4]);
            titolText.text = "Buen trabajo borracho!";
            startGameTrigger = false;
            cartesAdivinades += 1;
            Invoke("FinishScene", 5);
        }
    }

    // Función que llamamos desde las cartas para avisar al GameManager que alguna ha sido apretada
    public void cardTouched(GameObject cardtouched)
    {
        if (cardsSelected[0] == null && cardsSelected[1] == null)
        {
            cardsSelected[0] = cardtouched.gameObject;
            audioSource.PlayOneShot(audioClips[1]);
        }
        else
        {
            cardsSelected[1] = cardtouched.gameObject;
            audioSource.PlayOneShot(audioClips[1]);
        }
    }

    // Comprueba las ids de las cartas levantadas para saber si son iguales o no.
    public void checkIds()
    {
        if (cardsSelected[0] != null && cardsSelected[1] != null)
        {
            if (cardsSelected[0].GetComponent<CardScript>().getId() != cardsSelected[1].GetComponent<CardScript>().getId())
            {
                cardsSelected[0].GetComponent<CardScript>().esconder();
                cardsSelected[1].GetComponent<CardScript>().esconder();
                intentsNum += 1;
                intentsText.text = "Intentos: " + intentsNum;
                audioSource.PlayOneShot(audioClips[2]);
                borrarSeleccionados(12);
            }
            else
            {
                cartesAdivinades++;
                audioSource.PlayOneShot(audioClips[3]);
                borrarSeleccionados(12);
            }
        }
    }

    // Sirve para saber si hay más de 2 cartas seleccionadas.
    public bool hiHaPuesto()
    {
        return cardsSelected[0] == null || cardsSelected[1] == null;
    }

    // Reset de las cartas seleccionadas.
    public void borrarSeleccionados(int num)
    {
        if (num == 0)
        {
            cardsSelected[0] = null;
        }
        if (num == 1)
        {
            cardsSelected[1] = null;
        }
        if (num == 12)
        {
            cardsSelected[0] = null;
            cardsSelected[1] = null;
        }
    }

    // Mezcla una lista de manera aleatoria
    void Mezclar(GameObject[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int randomIndex = Random.Range(i, n);
            GameObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    // Llamamos a esta función cuando apretamos el botón de start para hacer el set de todas las variables.
    void AccionBoton()
    {
        audioSource.PlayOneShot(audioClips[0]);
        cardSons[0].GetComponent<CardScript>().setStartVar(true);
        startButton.gameObject.SetActive(false);
        startGameTrigger = true;
        titolText.text = "";
        intentsNum = 0;
        intentsText.text = "Intentos: " + intentsNum;
        // Asignamos ids y figuras a las cartas.
        foreach (GameObject card in cardSons)
        {
            card.GetComponent<CardScript>().setId(id_double);
            id_double += 0.5;

            // Carga el material según la ID
            int id_int = (int)card.GetComponent<CardScript>().getId();
            string materialPath = "Materials/" + id_int.ToString();

            // Asegúrate de que el material existe
            Material materialCargado = Resources.Load<Material>(materialPath);

            if (materialCargado != null)
            {
                card.GetComponent<CardScript>().getFiguraRenderer().material = materialCargado;
            }
            else
            {
                Debug.LogError("No se pudo cargar el material para la ID: " + id_int);
            }
        }
        // Mezclamos las cartas.
        Mezclar(cards);

        // Posicionamos las cartas en su lugar.
        int i = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Vector3 posicion = new Vector3((float)((x * 2) - 2), (float)-1.3, (y * 2) - 2);
                cards[i].transform.position = posicion;
                i++;
            }
        }
    }

    // Gracias a un Invoke hacemos reset a la escena para volver a jugar.
    void FinishScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    /* Setters y getters */
    public bool canClickTrigger()
    {
        return !clickTrigger;
    }

    public void setClickTrigger(bool clickTriger)
    {
        clickTrigger = clickTriger;
    }
}

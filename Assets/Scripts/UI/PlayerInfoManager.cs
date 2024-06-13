using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Gender 
{
    Chico,
    Chica
}
public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager Instance {get; private set;}

    [SerializeField] Button Chico;
    [SerializeField] Button Chica;
    [SerializeField] Button ConfirmButton;
    [SerializeField] Image Cuadrado;
    [SerializeField] TextMeshProUGUI Edad;
    [SerializeField] TextMeshProUGUI Nombre;

    private CanvasGroup canvas;

    public string Name { get; private set; }
    public int Age;
    public Gender gender;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Chica.onClick.AddListener(delegate { MoveSquare(Chica, Gender.Chica); });
        Chico.onClick.AddListener(delegate { MoveSquare(Chico, Gender.Chico); });
        ConfirmButton.onClick.AddListener(Confirm);
        canvas = GetComponent<CanvasGroup>();
    }

    public void MoveSquare(Button b, Gender g) 
    {
        Cuadrado.transform.position = b.transform.position;
        gender = g;
    }

    public void Confirm() 
    {
        if (int.TryParse(Edad.text, out int age))
        {
            Age = age;
        }
        else return;
        if (Nombre.text.Trim() != "") Name = Nombre.text;
        else return;
        SceneLoader.Instance.LoadScene(SceneIndex.LEVEL_SCENE);

        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }
}

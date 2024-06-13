using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

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
    [SerializeField] Button PlusAge;
    [SerializeField] Button MinusAge;
    [SerializeField] TextMeshProUGUI Nombre;

    private CanvasGroup canvas;

    public string Name { get; private set; }
    public int Age = 12;
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
        Age = 12;
        Edad.text = Age.ToString();
        PlusAge.onClick.AddListener(delegate { Age = Math.Min(Age + 1, 99); Edad.text = Age.ToString(); Edad.ForceMeshUpdate(); });
        MinusAge.onClick.AddListener(delegate { Age = Math.Max(Age - 1, 0); Edad.text = Age.ToString(); Edad.ForceMeshUpdate(); });
    }

    public void MoveSquare(Button b, Gender g) 
    {
        Cuadrado.transform.position = b.transform.position;
        gender = g;
    }

    public void Confirm() 
    {
        if (Nombre.text.Trim() != "") Name = Nombre.text;
        else return;

        SceneLoader.Instance.LoadScene(SceneIndex.LEVEL_SCENE);

        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

    }
}

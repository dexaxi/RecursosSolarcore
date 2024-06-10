using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class get_unity : MonoBehaviour
{
    IEnumerator SendPostRequest()
    {
		//Construye JSON para la petición REST
        string data = @"{
          ""username"":""FGMVAB"", ""password"":""2024TFGjuegogestionecoPC"",
          ""table"":""test"",
          ""filter"":{""name"": ""name1"" }
        }";

        //Construye UnityWebRequest para enviar solicitud 
        UnityWebRequest request = UnityWebRequest.Post("https://tfvj.etsii.urjc.es/get", data);

		// Configurar la solicitud (headers, etc.) si es necesario
		request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

		// Enviar la solicitud y esperar la respuesta
		yield return request.SendWebRequest();

        // Verificar si hay errores
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Error: " + request.error);
        }
        else
        {
            // La solicitud fue exitosa, puedes acceder a la respuesta
            Debug.Log("Respuesta: " + request.downloadHandler.text);
        }
    }

    void Awake()
    {
        StartCoroutine(SendPostRequest());
    }
}

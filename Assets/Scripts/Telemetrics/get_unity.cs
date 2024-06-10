using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class LoginRequest 
{
    public string username;
    public string password;
}

public class get_unity : MonoBehaviour
{
    IEnumerator SendPostRequest()
    {
        //Construye JSON para la peticiĆ³n REST
        var data = new LoginRequest
        {
            username = "TFGMVAB",
            password = "2024TFGjuegogestionecoPC"
        };

        WWWForm form = new WWWForm();
        form.AddField("username", "TFGMVAB");
        form.AddField("password", "2024TFGjuegogestionecoPC");
        
        //Construye UnityWebRequest para enviar solicitud 
        UnityWebRequest request = UnityWebRequest.Post("https://tfvj.etsii.urjc.es/rest/login", form);
        request.certificateHandler = new BypassCertificate();
        // Configurar la solicitud (headers, etc.) si es necesario

        // Enviar la solicitud y esperar la respuesta
        yield return request.SendWebRequest();

        // Verificar si hay errores
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
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
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}


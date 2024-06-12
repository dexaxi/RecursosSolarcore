using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DBConnection : MonoBehaviour
{
    private string token;
    private const string
        _hello = "https://tfvj.etsii.urjc.es/rest/hello",
        _login = "https://tfvj.etsii.urjc.es/rest/login",
        _get = "https://tfvj.etsii.urjc.es/rest/get",
        _insert = "https://tfvj.etsii.urjc.es/rest/insert",
        _update = "https://tfvj.etsii.urjc.es/rest/update";

    private void Awake()
    {
        SayHello();
        DontDestroyOnLoad(this);
    }

    //---------- TOKEN ----------
    public void GetToken()
    {
        string data = @"{""username"":""TFGMVAB"", ""password"":""2024TFGjuegogestionecoPC""}";
        StartCoroutine(DBAccess(data, _login, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error al obtener TOKEN");
            }
            else
            {
                TokenResponse response = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
                token = response.token;
                PlayerPrefs.SetString("token", response.token);
            }
        }));
    }
    public void SayHello()
    {
        string t = PlayerPrefs.GetString("token");
        string data = $@"{{""username"":""TFGMVAB"", ""token"":""{t}""}}";
        StartCoroutine(DBAccess(data, _hello, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error al saludar");
                GetToken();
            }
            else
            {
                HelloResponse response = JsonUtility.FromJson<HelloResponse>(request.downloadHandler.text);
                Debug.Log("Token aceptado");
                token = t;
                if (response.result != "Ok")        //  Token caducado.
                {
                    Debug.Log("Token caducado");
                    GetToken();
                    SayHello();     //  Solo para comprobar que el token se ha generado correctamente.
                }
            }
        }));
    }

    /*//---------- GETTERS ----------
    public void GetAllUsers(Action<int> callback)
    {
        string data = $@"{{
            ""username"":""TFGMVAB"",""token"":""{token}"",
            ""table"":""users"",
            ""filter"":{{}}
        }}";

        StartCoroutine(DBAccess(data, _get, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + request.error);
                callback?.Invoke(-1);
            }
            else
            {
                // La solicitud fue exitosa, puedes acceder a la respuesta
                UsersResponse response = JsonUtility.FromJson<UsersResponse>(request.downloadHandler.text);
                int newId = 1;
                if (response.data.Count != 0)
                {
                    List<User> list = response.data.OrderBy(x => x.id).ToList();
                    User user = list[list.Count - 1];
                    newId += user.id;
                }
                callback?.Invoke(newId);
            }
        }));
    }
    public void GetUser(int id, Action<User> callback)
    {
        string data = $@"{{
            ""username"":""TFGMVAB"", ""token"":""{token}"",
            ""table"":""users"",
            ""filter"":{{""id"":""{id}""}}
        }}";

        StartCoroutine(DBAccess(data, _get, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                callback?.Invoke(null);
            }
            else
            {
                UsersResponse response = JsonUtility.FromJson<UsersResponse>(request.downloadHandler.text);
                if (response.data.Count == 0)
                {
                    callback?.Invoke(null);     //Se ha enviado bien la petición pero no existe el usuario
                }
                else
                {
                    callback?.Invoke(response.data[0]);
                }
            }
        }));
    }

    public void GetScore(int id, string level, Action<int> callback)
    {
        string data = $@"{{
            ""username"":""TFGMVAB"", ""token"":""{token}"",
            ""table"":""scores"",
            ""filter"":{{""userId"":""{id}"",""levelName"":""{level}""}}
        }}";

        StartCoroutine(DBAccess(data, _get, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                callback?.Invoke(-1);
            }
            else
            {
                ScoreResponse response = JsonUtility.FromJson<ScoreResponse>(request.downloadHandler.text);
                if (response.data.Count == 0)
                {
                    callback?.Invoke(response.data[0].userScore);     //Se ha enviado bien la petición pero no existe el usuario
                }
                else
                {
                    callback?.Invoke(0);
                }
            }
        }));
    }
    public void GetGame(int id, string start, Action<Game> callback)
    {
        string data = $@"{{
            ""username"":""TFGMVAB"",""token"":""{token}"",
            ""table"":""games"",
            ""filter"":{{""userId"":""{id}"",""start"":""{start}""}}
        }}";

        StartCoroutine(DBAccess(data, _get, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + request.error);
                callback?.Invoke(null);
            }
            else
            {
                // La solicitud fue exitosa, puedes acceder a la respuesta
                GameResponse response = JsonUtility.FromJson<GameResponse>(request.downloadHandler.text);
                if (response.data.Count != 0)
                {
                    callback?.Invoke(response.data[0]);
                }
                else
                {
                    callback.Invoke(null);
                }
            }
        }));
    }

    //---------- INSERTS ----------
    public void InsertUser(string username, int age, string gender, Action<int> callback)   //Registra un nuevo usuario en la bbdd
    {
        GetAllUsers((newId) =>
        {
            if (newId != -1)
            {
                string data = $@"{{
                        ""username"":""TFGMVAB"", ""token"":""{token}"",
                        ""table"":""users"",
                        ""data"":{{""id"":""{newId}"",""name"": ""{username}"", ""age"": ""{age}"", ""gender"": ""{gender}""}}
                }}";
                StartCoroutine(DBAccess(data, _insert, (request) =>
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(request.downloadHandler.data);
                        callback?.Invoke(-1);
                    }
                    else
                    {
                        // La solicitud fue exitosa, puedes acceder a la respuesta
                        Debug.Log(request.downloadHandler.text);
                        callback?.Invoke(newId);
                    }
                }));
            }
            else
            {
                callback?.Invoke(-1);
            }
        });
    }

    public void InsertNewGame(string dateStart)     //Registra el inicio de una nueva partida en la bbdd
    {
        int id = PlayerPrefs.GetInt("id");
        string data = $@"{{
            ""username"":""TFGMVAB"",""token"":""{token}"",
            ""table"":""games"",
            ""data"":{{""userId"":""{id}"",""start"":""{dateStart}""}}
        }}";
        StartCoroutine(DBAccess(data, _insert, (request) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error al insertar partida en BD");
            }
        }));
    }
    public void InsertLevelDetails(string level, string time, int score, bool win)  //Inserta en la bbdd los datos del nivel jugado
    {
        int id = PlayerPrefs.GetInt("id");
        GetGame(id, PlayerPrefs.GetString("start"), (game) =>
        {
            if (game != null)
            {
                int winInt = win ? 1 : 0;
                string data = $@"{{
                    ""username"":""TFGMVAB"", ""token"":""{token}"",
                    ""table"":""levelsDetails"",
                    ""data"":{{""userId"":""{id}"",""gameId"":""{game.id}"",""levelName"":""{level}"",""timeSpent"":""{time}"",""score"":""{score}"",""win"":""{winInt}""}}
                }}";

                StartCoroutine(DBAccess(data, _insert, (request) =>
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(request.downloadHandler.text);
                    }
                    else
                    {
                        // La solicitud fue exitosa, puedes acceder a la respuesta
                        Debug.Log(request.downloadHandler.text);
                    }
                }));
            }
        });
    }
    */

    //---------- ACCESO BASE DE DATOS ----------
    IEnumerator DBAccess(string data, string typeOfRequest, Action<UnityWebRequest> callback)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);

        UnityWebRequest request = new UnityWebRequest(typeOfRequest, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        callback?.Invoke(request);
    }

    //---------- OTROS ----------
    public string GetTimeSpent(string start, string end)
    {
        Debug.Log("Start: " + start + ", End: " + end);
        string format = "HH:mm:ss.fff";

        DateTime time1 = DateTime.ParseExact(start, format, null);
        DateTime time2 = DateTime.ParseExact(end, format, null);

        TimeSpan difference = time2 - time1;

        return $"{difference.Hours}:{difference.Minutes}:{difference.Seconds}:{difference.Milliseconds}";
        //return difference;
    }
}

//---------- RESPONSE CLASSES ----------
[Serializable]
public class LevelResponse
{
    public string result;
    //public List<LevelData> data;
}

[Serializable]
public class TokenResponse
{
    public string result;
    public string token;
    public string until;
}

[Serializable]
public class HelloResponse
{
    public string result;
    public string message;
    public int lifespan;
}

[Serializable]
public class UsersResponse
{
    public string result;
    public List<User> data;
}

[Serializable]
public class User
{
    public int id;
    public string name;
    public int age;
    public string gender;
}

[Serializable]
public class ScoreResponse
{
    public string result;
    public List<Score> data;
}

[Serializable]
public class Score
{
    public int id, userId, maxScore, userScore;
    public string levelName;
}

[Serializable]
public class GameResponse
{
    public string result;
    public List<Game> data;
}

[Serializable]
public class Game
{
    public int id, userId;
    public DateTime start, end;
}
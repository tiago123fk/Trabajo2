using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Http : MonoBehaviour
{
    [SerializeField] private RawImage[] imagenes;
    private string url1 = "https://my-json-server.typicode.com/tiago123fk/Trabajo2/users";
    string url2 = "https://rickandmortyapi.com/api/character";
    [SerializeField] private TextMeshProUGUI name1;
    [SerializeField] private TextMeshProUGUI[] name2;
    [SerializeField] private TextMeshProUGUI[] name3;
    private int arreglo = 0;
    private int cambioId = 0;

    private void Start()
    {
        SendRequest();
    }

    public void SendRequest()
    {
        if(cambioId == 5)
        {
            cambioId = 0;
        }

        cambioId++;

        StartCoroutine("GetRequest", cambioId);
    }

    IEnumerator GetRequest(int numero)
    {
        UnityWebRequest request = UnityWebRequest.Get(url1 + "/" + numero);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                Mijson decks = JsonUtility.FromJson<Mijson>(request.downloadHandler.text);

                name1.text = decks.username;

                for (int i = 0; i < decks.deck.Length; i++)
                {
                    yield return StartCoroutine("GetRequest2", decks.deck[i]);
                }
            }
            else
            {
                Debug.Log($"Status: {request.responseCode} \n Error: {request.error}");
            }
        }
    }

    IEnumerator GetRequest2(int id)
    {
        if (arreglo == 5)
        {
            arreglo = 0;
        }

        UnityWebRequest request = UnityWebRequest.Get(url2 + "/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);

                name2[arreglo].text = character.name;
                name3[arreglo].text = character.species;

                yield return StartCoroutine("GetTexture", character.image);
            }
            else
            {
                Debug.Log($"Status: {request.responseCode} \n Error: {request.error}");
            }
        }

        arreglo += 1;
    }

    IEnumerator GetTexture(string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            imagenes[arreglo].texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }

    class Character
    {
        public int id;
        public string name;
        public string species;
        public string image;
    }

    class Mijson
    {
        public int id;
        public string username;
        public int[] deck;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class ActividadDos : MonoBehaviour
{
    private const string BASE_URI = "https://my-json-server.typicode.com/Alafresh/TwitterFollowCard/db";
    private const string RICK_MORTY_URI = "https://rickandmortyapi.com/api/character/";
    
    [SerializeField]
    private Card[] cards;
    [SerializeField]
    private TextMeshProUGUI userName;

    private Users _users;
    private Characters _characters;
    private int _index = 0;
    private string[][] _decks;
    
    private void Start() {
        StartCoroutine(GetUsers());
    }

    IEnumerator GetUsers() {
        using (UnityWebRequest request = UnityWebRequest.Get(BASE_URI)) {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success) {
                Debug.LogError(request.error.ToString());
            } else {
                string json = request.downloadHandler.text;
                Debug.Log(json);
                _users = JsonUtility.FromJson<Users>(json);
                _decks = new string[_users.users.Count][];
                for (int i = 0; i < _users.users.Count; i++) {
                    _decks[i] = new string[_users.users[i].deck.Count];
                    //Debug.Log("Usuario " + _users.users[i].username);
                    for (int j = 0; j < _users.users[i].deck.Count; j++) {
                        _decks[i][j] = _users.users[i].deck[j].ToString();
                        //Debug.Log(_decks[i][j]);
                    }
                }
                SetCards(_index);
            }
        }
    }

    IEnumerator GetCharacters(string cardsNumber) {
        using (UnityWebRequest request = UnityWebRequest.Get(RICK_MORTY_URI + cardsNumber)) {
            yield return request.SendWebRequest();

            switch(request.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError($"Error: {request.error.ToString()}");
                    break;
                case UnityWebRequest.Result.Success:
                    string json = request.downloadHandler.text;
                    _characters = JsonUtility.FromJson<Characters>(json);

                    break;
            }
        }
    }
    IEnumerator GetImage(RawImage rawImage, string imageUrl) {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl)) {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) {
                Debug.LogError(request.error.ToString());
            } else {
                Texture2D texture2D = DownloadHandlerTexture.GetContent(request);
                rawImage.texture = texture2D;
            }
        }
    }
    public void SetCards(int userIndex) {
        userName.text = _users.users[userIndex].username;

        for (int i = 0; i < _users.users[userIndex].deck.Count - 1; i++) {
            int cardNumber  = _users.users[userIndex].deck[i];
            cards[i].species.text = _characters.results[cardNumber].species;
            cards[i].characterName.text = _characters.results[cardNumber].name;
            
        }
    }
}

[Serializable]
public class User {
    public int id;
    public string username;
    public bool state;
    public List<int> deck;
}

[Serializable]
public class Users {
    public List<User> users;
}

[Serializable]
public class Info {
    public int count;
    public int pages;
    public string next;
    public string prev;
}

[Serializable]
public class Character {
    public int id;
    public string name;
    public string species;
    public string image;
}
[Serializable]
public class Characters {
    public Info info;
    public List<Character> results;
}
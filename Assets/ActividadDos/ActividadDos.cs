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
    private List<Card> cards = new List<Card>();
    [SerializeField]
    private TextMeshProUGUI userName;

    private Users _users;
    private CharactersList _charactersList;
    private int _index = 0;
    private string[] _decks;
    
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
                _decks = new string[_users.users.Count];
                for (int i = 0; i < _users.users.Count; i++) {
                    //Debug.Log("Usuario " + _users.users[i].username);
                    for (int j = 0; j < _users.users[i].deck.Count; j++) {
                        _decks[i] += _users.users[i].deck[j].ToString() + ",";
                        //Debug.Log(_decks[i]);
                    }
                }
                StartCoroutine(GetCharacters(_decks[_index]));
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
                    string wrappedJson = "{ \"characters\": " + json + " }";
                    _charactersList = JsonUtility.FromJson<CharactersList>(wrappedJson);
                    SetCards(_index);
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
        
        for (int i = 0; i < _users.users[userIndex].deck.Count; i++) {
            cards[i].species.text = _charactersList.characters[i].species;
            cards[i].characterName.text = _charactersList.characters[i].name;
            StartCoroutine(GetImage(cards[i].image, _charactersList.characters[i].image));
            cards[i].gameObject.SetActive(true);
        }
    }

    public void NextUser() {
        _index = (_index + 1) % _users.users.Count;
        for (int i = 0; i < cards.Count; i++) {
            cards[i].gameObject.SetActive(false);
        }
        StartCoroutine(GetCharacters(_decks[_index]));
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
public class Character {
    public int id;
    public string name;
    public string species;
    public string image;
}
[Serializable]
public class CharactersList {
    public List<Character> characters;
}
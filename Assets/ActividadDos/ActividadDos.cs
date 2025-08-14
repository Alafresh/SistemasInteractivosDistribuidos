using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class ActividadDos : MonoBehaviour
{
    private const string BASE_URI = "https://my-json-server.typicode.com/Alafresh/TwitterFollowCard";
    private const string RICK_MORTY_URI = "https://rickandmortyapi.com/api/character/";
    [SerializeField]
    private RawImage rawImage;
    private void Start() {
        StartCoroutine(GetCharacter(1));
    }

    IEnumerator GetCharacter(int id) {
        using (UnityWebRequest request = UnityWebRequest.Get(RICK_MORTY_URI + id)) {
            yield return request.SendWebRequest();

            switch(request.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError($"Error: {request.error.ToString()}");
                    break;
                case UnityWebRequest.Result.Success:
                    string json = request.downloadHandler.text;
                    Character character = JsonUtility.FromJson<Character>(json);
                    StartCoroutine(GetImage(character.image));
                    break;
            }
        }
    }
    IEnumerator GetImage(string imageUrl) {
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
}

[System.Serializable]
public class Character {
    public int id;
    public string name;
    public string species;
    public string image;
}
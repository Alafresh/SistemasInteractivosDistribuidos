using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Httptest : MonoBehaviour {
    [SerializeField] string url;
    void Start() {
        StartCoroutine(GetText());
    }

    IEnumerator GetText() {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log(request.error);
        } else {
            if(request.responseCode == 200) {
                string json = request.downloadHandler.text;
                Character character = JsonUtility.FromJson<Character>(json);
                Debug.Log($"mi nombre es {character.name}" +
                    $" mi id es {character.id} y mi especie es {character.species}");
            }
        }
    }
}

public class Character {
    public int id;
    public string name;
    public string species;
}

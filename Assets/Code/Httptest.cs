using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Httptest : MonoBehaviour
{
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
            // Show results as text
            Debug.Log(request.downloadHandler.text);
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI species;
    public RawImage image;

    private void Awake() {
        image = GetComponent<RawImage>();
        characterName = GetComponent<TextMeshProUGUI>();
        species = GetComponent<TextMeshProUGUI>();
    }
}

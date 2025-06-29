using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConbinationLockNumber : MonoBehaviour
{
    public int number = 0;
    public TextMeshProUGUI text;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnNumberButtonClick);
    }

    private void OnNumberButtonClick()
    {
        number = (number + 1 + 10) % 10;
        text.SetText(number.ToString());
    }
}

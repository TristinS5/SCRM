using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraSensSlider : MonoBehaviour
{
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_Text textSlider;
    [SerializeField] cameraController camController;
    int sensValue;

    void Start()
    {
        sensSlider.onValueChanged.AddListener((v) =>
        {
            textSlider.text = sensSlider.value.ToString("0");
            camController.SetSens((int)sensSlider.value);
        });
    }
}

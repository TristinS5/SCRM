using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingSliders : MonoBehaviour
{
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_Text senstextSlider;

    [SerializeField] Slider fovSlider;
    [SerializeField] TMP_Text fovtextSlider;

    [SerializeField] cameraController camController;
    int sensValue;

    void Start()
    {
        sensSlider.onValueChanged.AddListener((v) =>
        {
            senstextSlider.text = sensSlider.value.ToString("0");
            camController.SetSens((int)sensSlider.value);
        });

        fovSlider.onValueChanged.AddListener((v) =>
        {
            fovtextSlider.text = fovSlider.value.ToString("0");
            camController.SetFov((int)fovSlider.value);
        });
    }
}

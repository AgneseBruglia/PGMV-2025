using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events; // Aggiungi questa direttiva

public class PlantUIController : MonoBehaviour
{
    private GameObject canvasUI;

    public void CreatePlantUI()
    {
        // Crea un Canvas
        canvasUI = new GameObject("Canvas");
        Canvas canvas = canvasUI.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasUI.AddComponent<CanvasScaler>();
        canvasUI.AddComponent<GraphicRaycaster>();

        // Crea un pannello all'interno del canvas
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasUI.transform);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(300, 200);
        panel.AddComponent<Image>().color = Color.white;

        // Aggiungi slider e pulsanti
        CreateSlider(panel.transform, "Scale", 1f, 0.1f, 10f, 1f, OnScaleValueChanged);
        CreateSlider(panel.transform, "Delta", 20f, 0f, 90f, 20f, OnDeltaValueChanged);

        // Aggiungi pulsante di chiusura
        CreateCloseButton(panel.transform);
    }

    // Metodo che gestisce il cambiamento del valore di scale
    void OnScaleValueChanged(float value)
    {
        Debug.Log("Scale: " + value);
    }

    // Metodo che gestisce il cambiamento del valore di delta
    void OnDeltaValueChanged(float value)
    {
        Debug.Log("Delta: " + value);
    }

    void CreateSlider(Transform parent, string labelText, float defaultValue, float minValue, float maxValue, float currentValue, UnityAction<float> onValueChanged)
    {
        // Crea il label
        GameObject label = new GameObject(labelText);
        label.transform.SetParent(parent);
        TextMeshProUGUI labelTextComponent = label.AddComponent<TextMeshProUGUI>();
        labelTextComponent.text = labelText;
        labelTextComponent.fontSize = 14;

        // Crea lo slider
        GameObject sliderGO = new GameObject("Slider");
        sliderGO.transform.SetParent(parent);
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = defaultValue;

        // Aggiungi l'evento listener (ora con UnityAction direttamente)
        slider.onValueChanged.AddListener(onValueChanged);

        // Imposta le dimensioni e la posizione dello slider
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(200, 30);
        sliderRect.localPosition = new Vector3(0, -30, 0);
    }

    void CreateCloseButton(Transform parent)
    {
        // Crea il pulsante di chiusura
        GameObject buttonGO = new GameObject("CloseButton");
        buttonGO.transform.SetParent(parent);
        Button button = buttonGO.AddComponent<Button>();

        // Crea il testo del pulsante
        GameObject buttonText = new GameObject("Text");
        buttonText.transform.SetParent(buttonGO.transform);
        TextMeshProUGUI buttonTextComp = buttonText.AddComponent<TextMeshProUGUI>();
        buttonTextComp.text = "Close";
        buttonTextComp.fontSize = 16;

        // Imposta le dimensioni e la posizione del pulsante
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(100, 50);
        buttonRect.localPosition = new Vector3(0, -80, 0);

        // Aggiungi l'evento per chiudere il canvas
        button.onClick.AddListener(CloseCanvas);
    }

    void CloseCanvas()
    {
        // Distruggi il canvas quando il pulsante viene premuto
        Destroy(canvasUI);
    }
}

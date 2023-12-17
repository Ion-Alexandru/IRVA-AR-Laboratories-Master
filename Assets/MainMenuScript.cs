using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public Button MeasureButton;
    public Button AnchorButton;
    public Button FruitButton;
    public Button AugmentedButton;

    private void Start()
    {
        MeasureButton.onClick.AddListener(() => OpenScene("Measure Distances"));
        AnchorButton.onClick.AddListener(() => OpenScene("Cloud Anchors"));
        FruitButton.onClick.AddListener(() => OpenScene("Fruit Ninja"));
        FruitButton.onClick.AddListener(() => OpenScene("Augmented Images"));
    }

    void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

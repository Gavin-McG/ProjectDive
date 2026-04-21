using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string playSceneName = "GameScene"; // Set in Inspector

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // Query buttons by name
        Button playButton = root.Q<Button>("Play");
        Button quitButton = root.Q<Button>("Quit");

        if (playButton != null)
            playButton.clicked += OnPlayClicked;
        else
            Debug.LogWarning("Play button not found!");

        if (quitButton != null)
            quitButton.clicked += OnQuitClicked;
        else
            Debug.LogWarning("Quit button not found!");
    }

    private void OnDisable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        Button playButton = root.Q<Button>("Play");
        Button quitButton = root.Q<Button>("Quit");

        if (playButton != null)
            playButton.clicked -= OnPlayClicked;

        if (quitButton != null)
            quitButton.clicked -= OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene(playSceneName);
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit clicked");

        // Works in build
        Application.Quit();

        // Stops play mode in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
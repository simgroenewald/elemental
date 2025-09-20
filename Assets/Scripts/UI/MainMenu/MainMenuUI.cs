using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject instructionsButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject gameName;

    private bool isInstructionSceneLoaded = false;
    private void Start()
    {
        // Play Music
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);
        SceneManager.LoadScene("CharacterSelector", LoadSceneMode.Additive);
        backButton.SetActive(false);

    }

    public void PlayGame()
    {
        SceneManager.LoadScene("ElementalGame");
    }

    public void LoadCharacterSelector()
    {
        backButton.SetActive(false);

        if (isInstructionSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("Instructions");
            isInstructionSceneLoaded = false;
        }

        gameName.SetActive(true);
        playButton.SetActive(true);
        quitButton.SetActive(true);
        instructionsButton.SetActive(true);

        // Load character selector scene additively
        SceneManager.LoadScene("CharacterSelector", LoadSceneMode.Additive);
    }

    public void LoadInstructions()
    {
        gameName.SetActive(false);
        playButton.SetActive(false);
        quitButton.SetActive(false);
        instructionsButton.SetActive(false);
        isInstructionSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelector");

        backButton.SetActive(true);

        // Load instructions scene additively
        SceneManager.LoadScene("Instructions", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

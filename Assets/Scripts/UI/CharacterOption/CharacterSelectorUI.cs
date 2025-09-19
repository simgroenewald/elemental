using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class CharacterSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform characterSelector;
    private List<PlayerDetailsSO> characterOptions;
    private GameObject characterOptionPrefab;
    private PlayerSO currentPlayer;
    private List<GameObject> playerCharacterGameObjectList = new List<GameObject>();
    private Coroutine coroutine;
    private int selectedPlayerIndex = 0;
    private float offset = 4f;

    private void Awake()
    {
        // Load resources
        characterOptionPrefab = GameResources.Instance.characterOptionPrefab;
        characterOptions = GameResources.Instance.characterOptions;
        currentPlayer = GameResources.Instance.player;
    }

    private void Start()
    {
        // Instatiate player characters
        for (int i = 0; i < characterOptions.Count; i++)
        {
            GameObject playerSelectionObject = Instantiate(characterOptionPrefab, characterSelector);
            playerCharacterGameObjectList.Add(playerSelectionObject);
            playerSelectionObject.transform.localPosition = new Vector3((offset * i), 0f, 0f);
            PopulatePlayerDetails(playerSelectionObject.GetComponent<CharacterOptionUI>(), characterOptions[i]);
        }

        // Initialise the current player
        currentPlayer.playerDetails = characterOptions[selectedPlayerIndex];

    }

    private void PopulatePlayerDetails(CharacterOptionUI characterSelection, PlayerDetailsSO playerDetails)
    {
        characterSelection.animator.runtimeAnimatorController = playerDetails.runtimeAnimatorController;
    }

    public void NextCharacter()
    {
        if (selectedPlayerIndex >= characterOptions.Count - 1)
            return;
        selectedPlayerIndex++;

        currentPlayer.playerDetails = characterOptions[selectedPlayerIndex];

        MoveToSelectedCharacter(selectedPlayerIndex);
    }

    public void PreviousCharacter()
    {
        if (selectedPlayerIndex == 0)
            return;

        selectedPlayerIndex--;

        currentPlayer.playerDetails = characterOptions[selectedPlayerIndex];

        MoveToSelectedCharacter(selectedPlayerIndex);
    }


    private void MoveToSelectedCharacter(int index)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(MoveToSelectedCharacterRoutine(index));
    }

    private IEnumerator MoveToSelectedCharacterRoutine(int index)
    {
        float currentLocalXPosition = characterSelector.localPosition.x;
        float targetLocalXPosition = index * offset * characterSelector.localScale.x * -1f;

        while (Mathf.Abs(currentLocalXPosition - targetLocalXPosition) > 0.01f)
        {
            currentLocalXPosition = Mathf.Lerp(currentLocalXPosition, targetLocalXPosition, Time.deltaTime * 10f);

            characterSelector.localPosition = new Vector3(currentLocalXPosition, characterSelector.localPosition.y, 0f);
            yield return null;
        }

        characterSelector.localPosition = new Vector3(targetLocalXPosition, characterSelector.localPosition.y, 0f);
    }

}
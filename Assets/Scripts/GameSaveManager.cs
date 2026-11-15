using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSaveManager : MonoBehaviour
{
    public const int GameSaveCount = 3;

    //name of the gameplay scene
    [SerializeField] private string gameSceneName;

    //one of each per game save
    [SerializeField] private Button[] gameSaveButtons;
    [SerializeField] private TMP_Text[] gameSaveLabels;
    [SerializeField] private Button[] deleteButtons;

    //confirmation prompt shown before a game save is deleted
    [SerializeField] private GameObject deleteConfirmPanel;
    [SerializeField] private TMP_Text deleteConfirmLabel;

    //game save file the game scene should load from, null = start a new game.
    //static so that game scene reads it to load game
    public static string GameSaveToLoad { get; private set; }

    //game save waiting on the delete prompt, 0 = none
    private int gameSaveToDelete;

    //full path of a game save file, game saves are numbered 1 to GameSaveCount
    public static string GetGameSavePath(int gameSave)
    {
        return Path.Combine(Application.persistentDataPath, "GameSave" + gameSave + ".json");
    }

    public static bool GameSaveExists(int gameSave)
    {
        return File.Exists(GetGameSavePath(gameSave));
    }

    //called by MenuManager so a new game doesn't load an old game save
    public static void ClearGameSaveToLoad()
    {
        GameSaveToLoad = null;
    }

    void OnEnable()
    {
        gameSaveToDelete = 0;
        if (deleteConfirmPanel != null)
        {
            deleteConfirmPanel.SetActive(false);
        }

        Refresh();
    }

    //looks for the game save files and updates each label and button
    public void Refresh()
    {
        for (int gameSave = 1; gameSave <= GameSaveCount; gameSave++)
        {
            int index = gameSave - 1;
            bool exists = GameSaveExists(gameSave);

            if (index < gameSaveLabels.Length && gameSaveLabels[index] != null)
            {
                if (exists)
                {
                    string savedAt = File.GetLastWriteTime(GetGameSavePath(gameSave)).ToString("d MMM yyyy HH:mm");
                    gameSaveLabels[index].text = "Game Save " + gameSave + " - " + savedAt;
                }
                else
                {
                    gameSaveLabels[index].text = "Game Save " + gameSave + " - Empty";
                }
            }

            //empty game saves can't be loaded or deleted
            if (index < gameSaveButtons.Length && gameSaveButtons[index] != null)
            {
                gameSaveButtons[index].interactable = exists;
            }

            if (index < deleteButtons.Length && deleteButtons[index] != null)
            {
                deleteButtons[index].interactable = exists;
            }
        }
    }

    //called by the game save buttons, gameSave is 1, 2 or 3
    public void LoadGameSave(int gameSave)
    {
        if (gameSave < 1 || gameSave > GameSaveCount || !GameSaveExists(gameSave))
        {
            return;
        }

        GameSaveToLoad = GetGameSavePath(gameSave);
        SceneFlow.ResetShipPosition();
        SceneManager.LoadScene(gameSceneName);
    }

    //called by the delete buttons, shows the prompt instead of deleting straight away
    public void AskDeleteGameSave(int gameSave)
    {
        if (gameSave < 1 || gameSave > GameSaveCount || !GameSaveExists(gameSave))
        {
            return;
        }

        gameSaveToDelete = gameSave;
        deleteConfirmLabel.text = "Delete Game Save " + gameSave + "?\nThis can't be undone.";
        deleteConfirmPanel.SetActive(true);
    }

    //called by the prompt's delete button
    public void ConfirmDeleteGameSave()
    {
        if (gameSaveToDelete != 0)
        {
            File.Delete(GetGameSavePath(gameSaveToDelete));
        }

        gameSaveToDelete = 0;
        deleteConfirmPanel.SetActive(false);
        Refresh();
    }
}

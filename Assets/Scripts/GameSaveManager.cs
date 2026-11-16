using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//everything written to a game save file. missions are saved by title
[Serializable]
public class GameSaveData
{
    public int quota;
    public int quotaScience;
    public int totalScience;
    public int materials;
    public bool gameWon;
    public string activeMission;
    //false = the quota's missions haven't been rolled yet (JsonUtility can't save a null list)
    public bool hasOffered;
    public List<string> offered = new List<string>();
    public List<string> completed = new List<string>();
    //false = start at the scene's spawn point
    public bool hasPosition;
    public Vector2 position;
}

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

    //game save file the current game saves to, null = not saving
    private static string currentPath;

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

    //called by MenuManager. a new game saves to the first empty game save, or over the oldest one if all are used
    public static void StartNewGameSave()
    {
        currentPath = GetGameSavePath(1);
        for (int gameSave = 1; gameSave <= GameSaveCount; gameSave++)
        {
            string path = GetGameSavePath(gameSave);
            if (!File.Exists(path))
            {
                currentPath = path;
                return;
            }

            if (File.GetLastWriteTime(path) < File.GetLastWriteTime(currentPath))
            {
                currentPath = path;
            }
        }
    }

    //writes the current game to its game save file. does nothing on the start menu, where there's no game running
    public static void Save()
    {
        if (currentPath == null || SceneManager.GetActiveScene().name == SceneFlow.StartSceneName)
        {
            return;
        }

        GameSaveData save = new GameSaveData();
        MissionManager.WriteSave(save);
        Vector2? position = SceneFlow.ShipPosition;
        save.hasPosition = position != null;
        save.position = position ?? Vector2.zero;
        File.WriteAllText(currentPath, JsonUtility.ToJson(save));
    }

    //saves when the game is closed
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Application.quitting += Save;
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

        GameSaveData save = JsonUtility.FromJson<GameSaveData>(File.ReadAllText(GetGameSavePath(gameSave)));
        if (save == null)
        {
            return;
        }

        currentPath = GetGameSavePath(gameSave);
        MissionManager.ReadSave(save);
        SceneFlow.ShipPosition = save.hasPosition ? save.position : (Vector2?)null;
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

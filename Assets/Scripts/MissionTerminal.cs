using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//the mission terminal screen opened by the Command Desk. shows the missions to pick from, or the active mission, or that this quota's mission is done.
public class MissionTerminal : MonoBehaviour
{
    [SerializeField] private TextAsset missionsJson;

    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Text headerLabel;

    //used for the active mission / mission complete / no missions messages
    [SerializeField] private TMP_Text messageLabel;

    //one per mission choice (up to 3 which is set in the inspector), unused ones are hidden
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TMP_Text[] optionLabels;

    public bool IsOpen => panel != null && panel.activeSelf;

    void Awake()
    {
        MissionManager.Load(missionsJson);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            if (optionButtons[i] != null)
            {
                optionButtons[i].onClick.AddListener(() => SelectOption(index));
            }
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    void Update()
    {
        if (IsOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    public void Open()
    {
        if (panel == null || PauseMenu.IsPaused)
        {
            return;
        }

        panel.SetActive(true);
        GameState.minigameOpen = true;
        Refresh();
    }

    public void Close()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        GameState.minigameOpen = false;
    }

    void OnDestroy()
    {
        //leaving the scene with the terminal open shouldn't leave the player frozen
        if (IsOpen)
        {
            GameState.minigameOpen = false;
        }
    }

    private void Refresh()
    {
        headerLabel.text = "MISSION TERMINAL - QUOTA " + MissionManager.CurrentQuota + "/" + MissionManager.FinalQuota;

        if (MissionManager.HasActiveMission)
        {
            ShowMessage("<b>ACTIVE MISSION</b>\n\n" + Describe(MissionManager.ActiveMission));
            return;
        }

        if (MissionManager.MissionCompletedThisQuota)
        {
            ShowMessage("Mission complete.\n\nNo new missions until the next quota.");
            return;
        }

        IReadOnlyList<Mission> offered = MissionManager.GetOfferedMissions();
        if (offered.Count == 0)
        {
            ShowMessage("No missions available.");
            return;
        }

        messageLabel.text = "Select a mission:";
        for (int i = 0; i < optionButtons.Length; i++)
        {
            bool used = i < offered.Count;
            optionButtons[i].gameObject.SetActive(used);
            if (used)
            {
                optionLabels[i].text = Describe(offered[i]);
            }
        }
    }

    private void ShowMessage(string text)
    {
        messageLabel.text = text;
        foreach (Button button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void SelectOption(int index)
    {
        IReadOnlyList<Mission> offered = MissionManager.GetOfferedMissions();
        if (index < offered.Count && MissionManager.SelectMission(offered[index]))
        {
            Refresh();
        }
    }

    private static string Describe(Mission mission)
    {
        StringBuilder text = new StringBuilder();
        text.Append("<b>[").Append(mission.difficulty.ToString().ToUpper()).Append("] ").Append(mission.title).Append("</b>\n");
        text.Append("<size=80%>").Append(mission.description);

        if (mission.objectives != null && mission.objectives.Length > 0)
        {
            text.Append("\nObjectives:");
            foreach (MissionObjective objective in mission.objectives)
            {
                text.Append("\n - ").Append(objective.location).Append(": ").Append(objective.measure);
            }
        }

        text.Append("</size>");
        return text.ToString();
    }
}

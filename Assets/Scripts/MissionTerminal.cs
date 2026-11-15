using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//the mission terminal screen opened by the Command Desk. shows the quota's science progress and the missions to pick from, or the active mission, plus the next quota button once the quota is filled.
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

    //mission the player clicked and is being asked to confirm, null = none
    private Mission pendingMission;

    //which option button is currently the next quota button, -1 = none
    private int advanceSlot = -1;

    //how much lighter the next quota button is than a mission button
    [SerializeField] private float advanceButtonLighten = 0.15f;

    //each option button's colour from the scene, so it can be put back after being the next quota button
    private Color[] optionColors;

    //small note shown to the left of the Close button
    [SerializeField] private TMP_Text footerLabel;

    public bool IsOpen => panel != null && panel.activeSelf;

    void Awake()
    {
        MissionManager.Load(missionsJson);

        optionColors = new Color[optionButtons.Length];

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            if (optionButtons[i] != null)
            {
                optionButtons[i].onClick.AddListener(() => SelectOption(index));
                if (optionButtons[i].targetGraphic != null)
                {
                    optionColors[i] = optionButtons[i].targetGraphic.color;
                }
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

        pendingMission = null;
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

        advanceSlot = -1;
        SetFooterText("");

        if (MissionManager.GameWon)
        {
            ShowMessage("All quotas complete.\n\nTotal science points: " + MissionManager.TotalScience);
            return;
        }

        string status = "Science points: " + MissionManager.QuotaScience + "/" + MissionManager.QuotaTarget + "   Materials: " + MissionManager.Materials + "\n\n";

        if (MissionManager.HasActiveMission)
        {
            ShowMessage(status + "<b>ACTIVE MISSION</b>\n\n" + Describe(MissionManager.ActiveMission));
            return;
        }

        if (pendingMission != null)
        {
            ShowConfirm(status);
            return;
        }

        IReadOnlyList<Mission> offered = MissionManager.GetOfferedMissions();
        bool canAdvance = MissionManager.CanAdvanceQuota;
        if (offered.Count == 0 && !canAdvance)
        {
            ShowMessage(status + "No missions available.");
            return;
        }

        if (!canAdvance)
        {
            messageLabel.text = status + "Select a mission:";
        }
        else if (offered.Count > 0)
        {
            messageLabel.text = status + "Quota filled. Move on, or finish the remaining missions for extra materials:";
        }
        else
        {
            messageLabel.text = status + "Quota filled. All missions complete.";
        }

        //missions first, then the next quota button in the first slot that's free
        //(a quota takes at least two missions to fill, so there's always a free slot by then)
        if (canAdvance && offered.Count < optionButtons.Length)
        {
            advanceSlot = offered.Count;
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            SetAdvanceColor(i, i == advanceSlot);
            if (i < offered.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionLabels[i].text = Describe(offered[i]);
            }
            else if (i == advanceSlot)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionLabels[i].text = MissionManager.CurrentQuota >= MissionManager.FinalQuota
                    ? "<b>COMPLETE FINAL QUOTA</b>"
                    : "<b>START QUOTA " + (MissionManager.CurrentQuota + 1) + "</b>\n<size=80%>Unfinished missions are dropped. Science points reset, materials are kept.</size>";
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    //makes an option button slightly lighter while it's the next quota button, normal colour otherwise
    private void SetAdvanceColor(int index, bool isAdvance)
    {
        Graphic graphic = optionButtons[index].targetGraphic;
        if (graphic == null)
        {
            return;
        }

        Color normal = optionColors[index];
        Color lighter = Color.Lerp(normal, Color.white, advanceButtonLighten);
        lighter.a = normal.a;
        graphic.color = isAdvance ? lighter : normal;
    }

    //asks the player if they're sure about the mission they clicked, using the first two option buttons as yes / no
    private void ShowConfirm(string status)
    {
        messageLabel.text = status + "<b>ACCEPT THIS MISSION?</b>\n\n" + Describe(pendingMission);
        SetFooterText("You can't pick another mission until this one is finished.");

        for (int i = 0; i < optionButtons.Length; i++)
        {
            SetAdvanceColor(i, false);
            optionButtons[i].gameObject.SetActive(i < 2);
        }

        optionLabels[0].text = "<b>YES, ACCEPT MISSION</b>";
        optionLabels[1].text = "<b>NO, GO BACK</b>";
    }

    private void SetFooterText(string text)
    {
        if (footerLabel != null)
        {
            footerLabel.text = text;
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
        //answering the "are you sure" question: first button = yes, second = no
        if (pendingMission != null)
        {
            Mission mission = pendingMission;
            pendingMission = null;
            if (index == 0)
            {
                MissionManager.SelectMission(mission);
            }
            Refresh();
            return;
        }

        if (index == advanceSlot)
        {
            if (MissionManager.StartNextQuota())
            {
                Refresh();
            }
            return;
        }

        IReadOnlyList<Mission> offered = MissionManager.GetOfferedMissions();
        if (index < offered.Count)
        {
            pendingMission = offered[index];
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

        text.Append("\nReward: +").Append(mission.science).Append(" science points, +").Append(mission.material).Append(" materials");
        text.Append("</size>");
        return text.ToString();
    }
}

using System.Collections.Generic;
using UnityEngine;

//keeps track of the current quota, which missions the terminal is offering, the mission the player picked, and whether it's done.
//static so it survives going to the cubesat designer (chaging scenes) and back
public static class MissionManager
{
    public const int FinalQuota = 5;

    public static int CurrentQuota { get; private set; } = 1;

    //mission the player picked, null = none
    public static Mission ActiveMission { get; private set; }

    //true once this quota's mission is done, so the terminal doesn't give another one
    public static bool MissionCompletedThisQuota { get; private set; }

    public static bool HasActiveMission => ActiveMission != null;

    //true when the terminal should show missions to pick from
    public static bool CanPickMission => !HasActiveMission && !MissionCompletedThisQuota;

    private static MissionList missions;

    //choices for the current quota, kept so reopening the terminal shows the same ones
    private static List<Mission> offeredMissions;

    public static bool IsLoaded => missions != null;

    //called when a new game starts
    public static void NewGame()
    {
        CurrentQuota = 1;
        ActiveMission = null;
        MissionCompletedThisQuota = false;
        offeredMissions = null;
    }

    //reads missions.json
    public static void Load(TextAsset missionsJson)
    {
        if (IsLoaded || missionsJson == null)
        {
            return;
        }

        missions = JsonUtility.FromJson<MissionList>(missionsJson.text);
        if (missions == null)
        {
            Debug.LogError("MissionManager: missions.json could not be read.");
            return;
        }

        SetDifficulty(missions.easy, MissionDifficulty.Easy);
        SetDifficulty(missions.medium, MissionDifficulty.Medium);
        SetDifficulty(missions.hard, MissionDifficulty.Hard);
    }

    //missions to choose from this quota: 1 easy on quota 1, easy + medium on quota 2, easy + medium + hard from quota 3 on
    public static IReadOnlyList<Mission> GetOfferedMissions()
    {
        if (!CanPickMission || !IsLoaded)
        {
            return new List<Mission>();
        }

        if (offeredMissions == null)
        {
            offeredMissions = new List<Mission>();
            AddRandomMission(missions.easy);
            if (CurrentQuota >= 2)
            {
                AddRandomMission(missions.medium);
            }
            if (CurrentQuota >= 3)
            {
                AddRandomMission(missions.hard);
            }
        }

        return offeredMissions;
    }

    //called by the terminal when the player picks a mission
    public static bool SelectMission(Mission mission)
    {
        if (!CanPickMission || mission == null || offeredMissions == null || !offeredMissions.Contains(mission))
        {
            return false;
        }

        ActiveMission = mission;
        offeredMissions = null;
        return true;
    }

    //call when the active mission succeeds
    public static void CompleteActiveMission()
    {
        if (!HasActiveMission)
        {
            return;
        }

        ActiveMission = null;
        MissionCompletedThisQuota = true;
    }

    //call when the quota system moves on to the next quota
    public static void StartNextQuota()
    {
        if (CurrentQuota < FinalQuota)
        {
            CurrentQuota++;
        }

        ActiveMission = null;
        MissionCompletedThisQuota = false;
        offeredMissions = null;
    }

    private static void SetDifficulty(Mission[] list, MissionDifficulty difficulty)
    {
        if (list == null)
        {
            return;
        }

        foreach (Mission mission in list)
        {
            if (mission != null)
            {
                mission.difficulty = difficulty;
            }
        }
    }

    //picks one mission from the list that's allowed on the current quota
    private static void AddRandomMission(Mission[] list)
    {
        if (list == null)
        {
            return;
        }

        List<Mission> allowed = new List<Mission>();
        foreach (Mission mission in list)
        {
            if (mission != null && mission.MinQuota <= CurrentQuota)
            {
                allowed.Add(mission);
            }
        }

        if (allowed.Count > 0)
        {
            offeredMissions.Add(allowed[Random.Range(0, allowed.Count)]);
        }
    }
}

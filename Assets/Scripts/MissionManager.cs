using System.Collections.Generic;
using UnityEngine;

//keeps track of the current quota, its science target, which missions the terminal is offering, the mission the player picked, and the science/materials earned.
//static so it survives going to the cubesat designer (chaging scenes) and back
public static class MissionManager
{
    //one quota: the science needed to fill it and the difficulties of the three missions it offers
    private class QuotaLevel
    {
        public readonly int target;
        public readonly MissionDifficulty[] offers;

        public QuotaLevel(int target, params MissionDifficulty[] offers)
        {
            this.target = target;
            this.offers = offers;
        }
    }

    //the best two missions of each quota always reach its target, so the player can always skip one
    private static readonly QuotaLevel[] Quotas =
    {
        new QuotaLevel(20, MissionDifficulty.Easy, MissionDifficulty.Easy, MissionDifficulty.Easy),
        new QuotaLevel(35, MissionDifficulty.Easy, MissionDifficulty.Medium, MissionDifficulty.Medium),
        new QuotaLevel(60, MissionDifficulty.Easy, MissionDifficulty.Medium, MissionDifficulty.Hard),
        new QuotaLevel(75, MissionDifficulty.Medium, MissionDifficulty.Medium, MissionDifficulty.Hard),
        new QuotaLevel(100, MissionDifficulty.Medium, MissionDifficulty.Hard, MissionDifficulty.Hard),
    };

    public static int FinalQuota => Quotas.Length;

    public static int CurrentQuota { get; private set; } = 1;

    //science points needed to fill the current quota
    public static int QuotaTarget => Quotas[CurrentQuota - 1].target;

    //science points earned this quota, goes back to 0 on the next one
    public static int QuotaScience { get; private set; }

    //all science points earned this game. this is basically game score
    public static int TotalScience { get; private set; }

    //mission rewards to spend in the cubesat designer, carries over between quotas
    public static int Materials { get; private set; }

    public static bool QuotaFilled => QuotaScience >= QuotaTarget;

    //true once the final quota has been filled and turned in
    public static bool GameWon { get; private set; }

    //mission the player picked, null = none
    public static Mission ActiveMission { get; private set; }

    public static bool HasActiveMission => ActiveMission != null;

    //true when the terminal should show missions to pick from
    public static bool CanPickMission => !HasActiveMission && !GameWon;

    //true when the terminal should show the next quota button
    public static bool CanAdvanceQuota => QuotaFilled && !HasActiveMission && !GameWon;

    private static MissionList missions;

    //missions of the current quota that haven't been picked yet, kept so reopening the terminal shows the same ones
    private static List<Mission> offeredMissions;

    //missions already done this game, so they aren't offered again
    private static readonly HashSet<Mission> completedMissions = new HashSet<Mission>();

    public static bool IsLoaded => missions != null;

    //called when a new game starts
    public static void NewGame()
    {
        CurrentQuota = 1;
        QuotaScience = 0;
        TotalScience = 0;
        Materials = 0;
        GameWon = false;
        ActiveMission = null;
        offeredMissions = null;
        completedMissions.Clear();
        pendingSave = null;
    }

    //game save whose missions are waiting for missions.json to be read, null = none
    private static GameSaveData pendingSave;

    //fills a game save with the current game
    public static void WriteSave(GameSaveData save)
    {
        save.quota = CurrentQuota;
        save.quotaScience = QuotaScience;
        save.totalScience = TotalScience;
        save.materials = Materials;
        save.gameWon = GameWon;
        save.activeMission = ActiveMission?.title;
        save.hasOffered = offeredMissions != null;
        foreach (Mission mission in offeredMissions ?? new List<Mission>())
        {
            save.offered.Add(mission.title);
        }
        foreach (Mission mission in completedMissions)
        {
            save.completed.Add(mission.title);
        }
    }

    //replaces the current game with a game save
    public static void ReadSave(GameSaveData save)
    {
        NewGame();
        CurrentQuota = Mathf.Clamp(save.quota, 1, FinalQuota);
        QuotaScience = save.quotaScience;
        TotalScience = save.totalScience;
        Materials = save.materials;
        GameWon = save.gameWon;
        pendingSave = save;
        ResolveSavedMissions();
    }

    //turns the saved mission titles back into missions, once missions.json has been read
    private static void ResolveSavedMissions()
    {
        if (pendingSave == null || !IsLoaded)
        {
            return;
        }

        ActiveMission = FindMission(pendingSave.activeMission);
        if (pendingSave.hasOffered)
        {
            offeredMissions = new List<Mission>();
            foreach (string title in pendingSave.offered)
            {
                Mission mission = FindMission(title);
                if (mission != null)
                {
                    offeredMissions.Add(mission);
                }
            }
        }
        foreach (string title in pendingSave.completed)
        {
            Mission mission = FindMission(title);
            if (mission != null)
            {
                completedMissions.Add(mission);
            }
        }
        pendingSave = null;
    }

    private static Mission FindMission(string title)
    {
        foreach (Mission[] pool in new[] { missions.easy, missions.medium, missions.hard })
        {
            foreach (Mission mission in pool ?? new Mission[0])
            {
                if (mission != null && mission.title == title)
                {
                    return mission;
                }
            }
        }
        return null;
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
        ResolveSavedMissions();
    }

    //missions left to choose from this quota (three to start with, picked ones are removed)
    public static IReadOnlyList<Mission> GetOfferedMissions()
    {
        if (!CanPickMission || !IsLoaded)
        {
            return new List<Mission>();
        }

        if (offeredMissions == null)
        {
            offeredMissions = new List<Mission>();
            foreach (MissionDifficulty difficulty in Quotas[CurrentQuota - 1].offers)
            {
                AddRandomMission(PoolFor(difficulty));
            }
            CheckQuotaCanBeFilled();
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
        offeredMissions.Remove(mission);
        return true;
    }

    //call when the active mission succeeds, gives its science and materials
    public static void CompleteActiveMission()
    {
        if (!HasActiveMission)
        {
            return;
        }

        QuotaScience += ActiveMission.science;
        TotalScience += ActiveMission.science;
        Materials += ActiveMission.material;
        completedMissions.Add(ActiveMission);
        ActiveMission = null;
    }

    //call when the active mission fails, it goes back on the terminal so it can be tried again
    public static void FailActiveMission()
    {
        if (!HasActiveMission)
        {
            return;
        }

        offeredMissions?.Add(ActiveMission);
        ActiveMission = null;
    }

    //called by the cubesat designer when the player pays for something
    public static bool SpendMaterials(int amount)
    {
        if (amount < 0 || amount > Materials)
        {
            return false;
        }

        Materials -= amount;
        return true;
    }

    //called by the terminal's next quota button. unfinished missions are dropped, leftover science doesn't carry over
    public static bool StartNextQuota()
    {
        if (!CanAdvanceQuota)
        {
            return false;
        }

        offeredMissions = null;

        if (CurrentQuota >= FinalQuota)
        {
            GameWon = true;
            return true;
        }

        CurrentQuota++;
        QuotaScience = 0;
        return true;
    }

    private static Mission[] PoolFor(MissionDifficulty difficulty)
    {
        switch (difficulty)
        {
            case MissionDifficulty.Medium: return missions.medium;
            case MissionDifficulty.Hard: return missions.hard;
            default: return missions.easy;
        }
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

    //picks one mission from the list that's allowed on the current quota and isn't already offered.
    //missions done earlier this game are skipped unless there's nothing else left
    private static void AddRandomMission(Mission[] list)
    {
        if (list == null)
        {
            return;
        }

        List<Mission> fresh = new List<Mission>();
        List<Mission> repeats = new List<Mission>();
        foreach (Mission mission in list)
        {
            if (mission == null || mission.MinQuota > CurrentQuota || offeredMissions.Contains(mission))
            {
                continue;
            }

            if (completedMissions.Contains(mission))
            {
                repeats.Add(mission);
            }
            else
            {
                fresh.Add(mission);
            }
        }

        List<Mission> allowed = fresh.Count > 0 ? fresh : repeats;
        if (allowed.Count > 0)
        {
            offeredMissions.Add(allowed[Random.Range(0, allowed.Count)]);
        }
    }

    //warns if the science values in missions.json break the "best two missions fill the quota" rule
    private static void CheckQuotaCanBeFilled()
    {
        int best = 0;
        int secondBest = 0;
        foreach (Mission mission in offeredMissions)
        {
            if (mission.science > best)
            {
                secondBest = best;
                best = mission.science;
            }
            else if (mission.science > secondBest)
            {
                secondBest = mission.science;
            }
        }

        if (best + secondBest < QuotaTarget)
        {
            Debug.LogWarning("MissionManager: the best two missions offered on quota " + CurrentQuota + " only give " + (best + secondBest) + " science, doesnt meet quota: " + QuotaTarget + ".");
        }
    }
}

using System;

//classes that match missions.json, so JsonUtility can read it straight in

public enum MissionDifficulty
{
    Easy,
    Medium,
    Hard
}

[Serializable]
public class MissionObjective
{
    public string location;
    public float depth;
    public string measure;
    public float toughness;
    public int minquota;
}

[Serializable]
public class Mission
{
    public string title;
    public string description;
    public MissionObjective[] objectives;
    public int material;
    public int science;

    //not in the JSON, filled in when the missions are loaded
    [NonSerialized] public MissionDifficulty difficulty;

    //lowest quota this mission can be offered on (the highest minquota of its objectives)
    public int MinQuota
    {
        get
        {
            int min = 1;
            if (objectives != null)
            {
                foreach (MissionObjective objective in objectives)
                {
                    if (objective != null && objective.minquota > min)
                    {
                        min = objective.minquota;
                    }
                }
            }
            return min;
        }
    }
}

//the whole missions.json file
[Serializable]
public class MissionList
{
    public Mission[] easy;
    public Mission[] medium;
    public Mission[] hard;
}

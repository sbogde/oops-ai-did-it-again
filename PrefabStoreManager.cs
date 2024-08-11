//using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public static class PrefabStoreManager
{
    public static List<string> prefabNames = new List<string>();

    public static List<string> agentNames = new List<string>();
    public static List<int> agentTimes = new List<int>();
    public static List<float> agentDistances = new List<float>();

    // Store waypoint scores and total scores
    public static List<int> agentWaypointScores = new List<int>();
    public static List<int> agentTotalScores = new List<int>();


    //public static void AddPrefabName(string name)
    //{
    //    if (!prefabNames.Contains(name))
    //    {
    //        prefabNames.Add(name);
    //    }
    //}

    //public static void AddAgentTimeAndDistance(string agentName, int time, float distance)
    //{
    //    agentNames.Add(agentName);
    //    agentTimes.Add(time);
    //    agentDistances.Add(distance);
    //}


    public static void AddAgentScores(string agentName, int score_total)
    {
        agentNames.Add(agentName);
        agentTotalScores.Add(score_total);
    }

}


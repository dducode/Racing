using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class GenerateTrackWindow : EditorWindow
{
    static GameObject directRoad;
    static GameObject turnRoad;
    static GameObject mirrorTurnRoad;
    static GameObject finish;
    string beginPointName = "begin";
    string endPointName = "end";
    [Range(100, 1000)]
    int trackLength = 100;
    bool isStraight;
    List<GameObject> roads;
    static List<GameObject> tracks = new List<GameObject>();
    int turnRoadsCount = 0;
    int turnRoadsMirrorCount = 0;
    GameObject newTrack;
    bool generateCheck;

    [MenuItem("Window/Generate Track")]
    public static void Init()
    {
        var window = EditorWindow.GetWindow<GenerateTrackWindow>("Generate Track");
        DontDestroyOnLoad(window);
    }

    void OnGUI()
    {
        using (var verticalArea = 
            new EditorGUILayout.VerticalScope())
        {
            directRoad = EditorGUILayout.ObjectField(
                "Direct road", directRoad, typeof(GameObject), false) as GameObject;
            turnRoad = EditorGUILayout.ObjectField(
                "Turn road", turnRoad, typeof(GameObject), false) as GameObject;
            mirrorTurnRoad = EditorGUILayout.ObjectField(
                "Mirror turn road", mirrorTurnRoad, typeof(GameObject), false) as GameObject;
            finish = EditorGUILayout.ObjectField(
                "Finish", finish, typeof(GameObject), false) as GameObject;
            beginPointName = EditorGUILayout.TextField("Begin point name", beginPointName);
            endPointName = EditorGUILayout.TextField("End point name", endPointName);
            isStraight = EditorGUILayout.Toggle("Is straight", isStraight);
            var isGenerate = GUILayout.Button("Generate");
            if (isGenerate)
                CreateTrack();
            var isRemove = GUILayout.Button("Remove all tracks");
            if (isRemove)
                RemoveTrack();
            if ((directRoad is null || turnRoad is null || mirrorTurnRoad is null || finish is null) && generateCheck)
                EditorGUILayout.HelpBox("Some objects for generation were not selected", MessageType.Warning);
        }
    }

    public void RemoveTrack()
    {
        if (tracks.Count is 0)
        {
            Debug.LogWarning("There are no objects to delete");
            return;
        }
        roads.Clear();
        turnRoadsCount = 0;
        turnRoadsMirrorCount = 0;
        int j = tracks.Count;
        for (int i = 0; i < j; i++)
        {
            DestroyImmediate(tracks[tracks.Count - 1]);
            tracks.Remove(tracks[tracks.Count - 1]);
        }
    }

    public void CreateTrack()
    {
        if (directRoad is null || turnRoad is null || mirrorTurnRoad is null || finish is null)
        {
            generateCheck = true;
            return;
        }
        roads = new List<GameObject>();
        newTrack = new GameObject("Track");
        if (tracks.Count > 0)
        {
            Vector3 position = tracks[tracks.Count - 1].transform.position;
            position.y += 100;
            newTrack.transform.position = position;
        }
        tracks.Add(newTrack);
        GameObject firstRoad = Instantiate(directRoad);
        firstRoad.name = "directroad";
        firstRoad.transform.position = newTrack.transform.position;
        firstRoad.transform.SetParent(newTrack.transform);
        roads.Add(firstRoad);
        for(int i = 0; i < trackLength; i++)
        {
            GameObject road;
            bool rnd = UnityEngine.Random.Range(0, 10) is 0;
            if(rnd && roads[roads.Count - 1].transform.name is "directroad" && !isStraight)
            {
                bool mirror = UnityEngine.Random.Range(0, 2) is 0;
                road = mirror ? 
                    CreateTurnRoad(mirrorTurnRoad, "turnroad_mirror") :
                    CreateTurnRoad(turnRoad, "turnroad");
                road = ReplaceTurnRoad(road);
            }
            else
                road = CreateDirectRoad(directRoad, "directroad");

            road.transform.SetParent(newTrack.transform);
            roads.Add(road);
        }
        GameObject finishRoad;
        finishRoad = CreateDirectRoad(finish, "finish");
        finishRoad.transform.SetParent(newTrack.transform);
        roads.Add(finishRoad);
    }

    GameObject CreateDirectRoad(GameObject roadType, string roadName)
    {
        GameObject road = Instantiate(roadType);
        GameObject road2 = roads[roads.Count - 1];
        road.name = roadName;
        
        if(road2.transform.name is "directroad")
            road.transform.rotation = road2.transform.rotation;
        else
        {
            int degrees = road2.transform.name is "turnroad" ? 90 : -90;
            Quaternion rotation = Quaternion.Euler(
                0, roads[roads.Count - 2].transform.eulerAngles.y + degrees, 0
            );
            road.transform.rotation = rotation;
        }

        int i = FindPointIndex(road, beginPointName);
        int j = FindPointIndex(road2, endPointName);

        road.transform.position = 
            road2.transform.GetChild(j).position - 
            road.transform.GetChild(i).position;
        
        return road;
    }

    GameObject CreateTurnRoad(GameObject roadType, string roadName)
    {
        int degrees = roadName is "turnroad" ? 0 : 90;
        GameObject turnRoad = Instantiate(roadType);
        GameObject road2 = roads[roads.Count - 1];

        turnRoad.name = roadName;
        if (roadName is "turnroad")
            turnRoadsCount++;
        else
            turnRoadsMirrorCount++;

        Quaternion rotation = Quaternion.Euler(
            0, road2.transform.eulerAngles.y + degrees, 0
        );
        turnRoad.transform.rotation = rotation;

        int i = FindPointIndex(turnRoad, beginPointName);
        int j = FindPointIndex(road2, endPointName);

        turnRoad.transform.position = 
            road2.transform.GetChild(j).position - 
            turnRoad.transform.GetChild(i).position;

        return turnRoad;
    }

    GameObject ReplaceTurnRoad(GameObject roadToReplace)
    {
        GameObject replacedRoad = roadToReplace;
        if (Mathf.Abs(turnRoadsCount - turnRoadsMirrorCount) > 1)
        {
            roadToReplace.transform.position = Vector3.zero;
            if (roadToReplace.name is "turnroad")
            {
                turnRoadsCount--;
                replacedRoad = CreateTurnRoad(mirrorTurnRoad, "turnroad_mirror");
            }
            else if (roadToReplace.name is "turnroad_mirror")
            {
                turnRoadsMirrorCount--;
                replacedRoad = CreateTurnRoad(turnRoad, "turnroad");
            }
            DestroyImmediate(roadToReplace);
        }

        return replacedRoad;
    }

    int FindPointIndex(GameObject road, string pointName)
    {
        int i = 0;
        for ( ; i < road.transform.childCount; i++)
            if(road.transform.GetChild(i).name == pointName)
                break;
        
        return i;
    }
}

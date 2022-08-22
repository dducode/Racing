using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrackGenerate : MonoBehaviour
{
    public List<GameObject> directRoads;
    public GameObject turnRoad;
    public GameObject mirrorTurnRoad;
    public GameObject finish;
    public string beginPointName = "begin";
    public string endPointName = "end";
    [Range(100, 300)]
    public int trackLength;
    public bool isStraight;
    List<GameObject> roads = new List<GameObject>();
    int turnRoadsCount = 0;
    int turnRoadsMirrorCount = 0;

    void Awake() => StartCoroutine(CreateRoads());

    void OnEnable() => BroadcastMessages.AddListener(Messages.RELOAD_TRACK, ReloadTrack);
    void OnDisable() => BroadcastMessages.RemoveListener(Messages.RELOAD_TRACK, ReloadTrack);

    public void ReloadTrack()
    {
        int j = roads.Count;
        // удаляем все чанки
        for(int i = 0; i < j; i++)
        {
            Destroy(roads[roads.Count - 1].gameObject);
            roads.Remove(roads[roads.Count - 1]);
        }
        turnRoadsCount = 0;
        turnRoadsMirrorCount = 0;
        StartCoroutine(CreateRoads()); // создаём все чанки заново
    }

    public IEnumerator CreateRoads()
    {
        // создаём первый чанк
        GameObject firstRoad = Instantiate(directRoads[UnityEngine.Random.Range(0, directRoads.Count)]);
        firstRoad.name = "directroad";
        firstRoad.transform.position = transform.position;
        firstRoad.transform.SetParent(transform);
        roads.Add(firstRoad);
        float progress = 1f * 100f / (trackLength + 2f);
        GameManager.uiManager.LoadScene((int)progress, "Generate track: ");
        yield return null;
        // в цикле создаём оставшиеся чанки, кроме финишного
        for (int i = 0; i < trackLength; i++)
        {
            GameObject road;
            bool rnd = UnityEngine.Random.Range(0, 10) == 0;
            if(rnd && roads[roads.Count - 1].transform.name == "directroad" && !isStraight)
            {
                bool mirror = UnityEngine.Random.Range(0, 2) == 0;
                road = mirror ? 
                    CreateTurnRoad(mirrorTurnRoad, "turnroad_mirror") :
                    CreateTurnRoad(turnRoad, "turnroad");
                road = ReplaceTurnRoad(road);
            }
            else
                road = CreateDirectRoad(directRoads[UnityEngine.Random.Range(0, directRoads.Count)], "directroad");

            road.transform.SetParent(transform);
            roads.Add(road);
            progress += 100f / (trackLength + 2f);
            GameManager.uiManager.LoadScene((int)progress, "Generate track: ");
            yield return null;
        }
        // создаём последний, финишный чанк
        GameObject finishRoad;
        finishRoad = CreateDirectRoad(finish, "finish");
        finishRoad.transform.SetParent(transform);
        roads.Add(finishRoad);
        progress += 100f / (trackLength + 2f);
        GameManager.uiManager.LoadScene((int)progress, "Generate track: ");
        yield return null;
        GameManager.uiManager.CloseLoadWindow(2);
    }

    GameObject CreateDirectRoad(GameObject roadType, string roadName)
    {
        GameObject road = Instantiate(roadType);
        GameObject road2 = roads[roads.Count - 1];
        road.name = roadName;
        
        if(road2.transform.name == "directroad")
            road.transform.rotation = road2.transform.rotation;
        else
        {
            int degrees = road2.transform.name == "turnroad" ? 90 : -90;
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
        int degrees = roadName == "turnroad" ? 0 : 90;
        GameObject turnRoad = Instantiate(roadType);
        GameObject road2 = roads[roads.Count - 1];

        turnRoad.name = roadName;
        if (roadName == "turnroad")
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

    GameObject ReplaceTurnRoad(GameObject replacedRoad)
    {
        if (Mathf.Abs(turnRoadsCount - turnRoadsMirrorCount) > 1)
        {
            replacedRoad.transform.position = Vector3.zero;
            Destroy(replacedRoad);
            if (replacedRoad.name == "turnroad")
            {
                turnRoadsCount--;
                replacedRoad = CreateTurnRoad(mirrorTurnRoad, "turnroad_mirror");
            }
            else if (replacedRoad.name == "turnroad_mirror")
            {
                turnRoadsMirrorCount--;
                replacedRoad = CreateTurnRoad(turnRoad, "turnroad");
            }
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

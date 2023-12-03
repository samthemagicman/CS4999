using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class MissedDrag
{
    public float rotationToTargetDifference;

    public float positionToTargetDifference;

    public float scaleToTargetDifference;
    
    public MissedDrag(float rotationToTargetDifference, float positionToTargetDifference, float scaleToTargetDifference)
    {
        this.rotationToTargetDifference = rotationToTargetDifference;
        this.positionToTargetDifference = positionToTargetDifference;
        this.scaleToTargetDifference = scaleToTargetDifference;
    }
}

[Serializable]
public class TaskConfiguration
{
    public bool useTransform;
    public bool useRotation;
    public bool useScale;
    
    public float rotationTolerance;
    public float transformTolerance;
    public float scaleTolerance;
    
    public TaskConfiguration(bool useTransform, bool useRotation, bool useScale, float rotationTolerance, float transformTolerance, float scaleTolerance)
    {
        this.useTransform = useTransform;
        this.useRotation = useRotation;
        this.useScale = useScale;
        this.rotationTolerance = rotationTolerance;
        this.transformTolerance = transformTolerance;
        this.scaleTolerance = scaleTolerance;
    }
}

[Serializable]
public class TaskCompletedData
{
    public float timeFromFirstGrab;
    public int taskNumber;
    public bool failed = false;
    public List<MissedDrag> missed = new List<MissedDrag>();
    public TaskAccuracyConfiguration accuracyConfiguration;
    public TaskAreaConfiguration areaConfiguration;
    
    public TaskCompletedData()
    {
        this.timeFromFirstGrab = 0;
        this.taskNumber = 0;
    }
    
    public TaskCompletedData(float timeFromFirstGrab, int taskNumber)
    {
        this.timeFromFirstGrab = timeFromFirstGrab;
        this.taskNumber = taskNumber;
    }
    
    public void AddMissedDrag(MissedDrag missedDrag)
    {
        missed.Add(missedDrag);
    }
}

[Serializable]
public class ListTaskCompletedData
{
    public List<TaskCompletedData> data = new List<TaskCompletedData>();
    
    public void Add(TaskCompletedData taskCompletedData)
    {
        data.Add(taskCompletedData);
    }
}

public class PlayStats : MonoBehaviour
{
    public bool saveToFile = false;
    public string filePrefix = "playstats";
    
    public PlayAreaSequence playAreaSequence;
    private MoveToTargetTask currentTask;

    private float objectGrabbedTime = 0;
    private bool counting = false;

    private static string Timestamp;
    
    private readonly ListTaskCompletedData listTaskCompletedData = new ListTaskCompletedData();
    private TaskCompletedData currentTaskCompletedData;

    private void Start()
    {
        if (Timestamp == null)
        {
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
        }
        playAreaSequence.taskStarted.AddListener(OnTaskStarted);
        playAreaSequence.taskFinished.AddListener(OnTaskFinished);
    }
    
    private void OnTaskStarted(MoveToTargetTask task)
    {
        currentTask = task;
        /*var taskName = task.name;
        var taskNumber = playAreaSequence.NumberOfTasksCompleted;
        var taskCount = playAreaSequence.playAreaConfigurations.Count;
        var accuracy = playAreaSequence.playAreaAccuracyConfig[taskNumber].accuracyName;
        var distance = playAreaSequence.playAreaConfigurations[taskNumber].distanceName;
        var taskInfo = $"{taskName} {taskNumber} / {taskCount} {accuracy} {distance}";*/
        // task.ObjectGrabbed.AddListener(ObjectGrabbed);
        currentTaskCompletedData = new TaskCompletedData
        {
            accuracyConfiguration = playAreaSequence.currentAccuracyConfig,
            areaConfiguration = playAreaSequence.currentAreaConfig
        };
        task.objectGrabbed.AddListener(ObjectGrabbed);
        task.dragFailed.AddListener(DragFailed);
    }
    
    private void DragFailed()
    {
        var missedDrag = new MissedDrag(
            Quaternion.Angle(currentTask.transform.rotation, currentTask.targetRotation),
            Vector3.Distance(currentTask.transform.position, currentTask.targetTransform),
            Vector3.Distance(currentTask.transform.localScale, currentTask.targetScale)
        );
        currentTaskCompletedData.AddMissedDrag(missedDrag);
    }

    private void ObjectGrabbed()
    {
        // For some reason, RemoveListener makes the object use gravity, so we do this instead
        if (!counting)
        {
            objectGrabbedTime = Time.time;
        }
    }
    
    private void OnTaskFinished(bool success)
    {
        counting = false;
        
        // Save data to file
        float finishedTime = Time.time - objectGrabbedTime;
        currentTaskCompletedData.timeFromFirstGrab = finishedTime;
        currentTaskCompletedData.taskNumber = playAreaSequence.NumberOfTasksCompleted;
        currentTaskCompletedData.failed = !success;
        listTaskCompletedData.Add(currentTaskCompletedData);
        
        if (saveToFile)
        {
            SaveJsonData(listTaskCompletedData, $"{filePrefix}-{Timestamp}.json");
        }
        Debug.Log($"Task ${playAreaSequence.NumberOfTasksCompleted} finished in {finishedTime} seconds");
    }

    private static void SaveJsonData<T>(T data, string filePath)
    {
        var jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, jsonData);
        Debug.Log($"JSON data saved to: {filePath}");
    }

    private T LoadJsonData<T>(string filePath)
    {
        if (File.Exists(filePath))
        {
            var jsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(jsonData);
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
            return default(T);
        }
    }
}

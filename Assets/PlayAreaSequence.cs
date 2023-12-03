using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Serialization;

public enum DistanceType
{
    Close,
    Medium,
    Far
}
public enum AccuracyType
{
    Low,
    Medium,
    High
}
[Serializable]
public class TaskAreaConfiguration
{
    public String name;
    public Vector3 offset;
    
    public TaskAreaConfiguration(String name, Vector3 offset)
    {
        this.name = name;
        this.offset = offset;
    }
}
[Serializable]
public class TaskAccuracyConfiguration
{
    public String name;
    public TaskConfiguration config;
    
    public TaskAccuracyConfiguration(String name, TaskConfiguration config)
    {
        this.name = name;
        this.config = config;
    }
}

public class PlayAreaSequence : MonoBehaviour
{
    public string nextScene;
    
    private TaskAccuracyConfiguration lowAccuracyConfig = new TaskAccuracyConfiguration("Low", new TaskConfiguration(true, false, false, 8, 0.02f, 0.02f));
    private TaskAccuracyConfiguration highAccuracyConfig = new TaskAccuracyConfiguration("High", new TaskConfiguration(true, false, false, 4, 0.008f, 0.008f));
    
    private TaskAreaConfiguration closeAreaConfig = new TaskAreaConfiguration("Close", new Vector3(0, -0.5f, 0.5f));

    private TaskAreaConfiguration farAreaConfig = new TaskAreaConfiguration("Far", new Vector3(0, -0.5f, 2.5f));
    
    public TaskAreaConfiguration currentAreaConfig;
    public TaskAccuracyConfiguration currentAccuracyConfig;

    [SerializeField]
    private List<PlayAreaAccurancyConfig> playAreaAccuracyConfig = new List<PlayAreaAccurancyConfig>();
    
    public UnityEvent<MoveToTargetTask> taskStarted;
    public UnityEvent<bool> taskFinished;
    
    public GameObject playArea;
    public ObjectGeneratorArea objectGeneratorArea;
    private ObjectGenerator objectGenerator;

    private int numberOfTasksCompleted = 0;

    private MoveToTargetTask currentTask;

    public int NumberOfTasksCompleted => numberOfTasksCompleted;

    private void Start()
    {
        currentAccuracyConfig = lowAccuracyConfig;
        currentAreaConfig = closeAreaConfig;
        
        objectGenerator = objectGeneratorArea.generator;
        objectGenerator.autogenerate = false;
        objectGenerator.newObjectGenerated.AddListener(OnNewTask);
        
        Invoke(nameof(HandleNextTask), 1);
    }

    private void OnNewTask(MoveToTargetTask task)
    {
        currentTask = task;
        task.onCompleted.AddListener(NextTask);
        task.onFailed.AddListener(TaskFail);
        taskStarted.Invoke(task);
    }
    
    public void SkipCurrentTask()
    {
        currentTask.Fail();
    }

    private void TaskFail()
    {
        numberOfTasksCompleted++;
        taskFinished.Invoke(false);
        HandleNextTask();
    }

    private void NextTask()
    {
        numberOfTasksCompleted++;
        taskFinished.Invoke(true);
        HandleNextTask();
    }

    private void HandleNextTask()
    {
        switch (numberOfTasksCompleted)
        {
            case <= 10:
                // Close, low accuracy, use transform
                currentAccuracyConfig = lowAccuracyConfig;
                break;
            case <= 20:
                // Far, low accuracy, use transform
                currentAreaConfig = farAreaConfig;
                break;
            case <= 30:
                // Close, low accuracy, use transform, use rotation
                currentAreaConfig = closeAreaConfig;
                currentAccuracyConfig = lowAccuracyConfig;
                currentAccuracyConfig.config.useRotation = true;
                break;
            case <= 40:
                // Close, low accuracy, use transform, use rotation, use scale
                currentAreaConfig = closeAreaConfig;
                currentAccuracyConfig = lowAccuracyConfig;
                currentAccuracyConfig.config.useRotation = true;
                currentAccuracyConfig.config.useScale = true;
                break;
            case <= 50:
                // Far, high accuracy, use transform, use rotation
                currentAreaConfig = farAreaConfig;
                currentAccuracyConfig = highAccuracyConfig;
                currentAccuracyConfig.config.useRotation = true;
                break;
            case <= 60:
                // Far, high accuracy, use transform, use rotation, use scale
                currentAreaConfig = farAreaConfig;
                currentAccuracyConfig = highAccuracyConfig;
                currentAccuracyConfig.config.useRotation = true;
                currentAccuracyConfig.config.useScale = true;
                break;
            case > 70:
            {
                if (nextScene != null)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
                }

                break;
            }
        }

        objectGenerator.taskConfiguration = currentAccuracyConfig.config;
        objectGeneratorArea.SetPosition(currentAreaConfig.offset);
        
        Invoke(nameof(Generate), objectGeneratorArea.OrbitalSpeed + 0.15f);
    }

    private void Generate()
    {
        objectGenerator.Generate();
    }
}

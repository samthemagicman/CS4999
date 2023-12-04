using TMPro;
using UnityEngine;

public class ExperimentInfoText : MonoBehaviour
{
    public PlayAreaSequence playAreaSequence;
    public TextMeshProUGUI taskCount;
    private void Update()
    {
        taskCount.SetText($"Task: {playAreaSequence.NumberOfTasksCompleted} / 70" + "\n" +
                          $"Distance: {playAreaSequence.currentAreaConfig.name}" + "\n" +
                          $"Accuracy: {playAreaSequence.currentAccuracyConfig.name}" + "\n" +
                          $"Use Scale: {playAreaSequence.currentAccuracyConfig.config.useScale}" + "\n" +
                          $"Use Rotation: {playAreaSequence.currentAccuracyConfig.config.useRotation}");
    }
}

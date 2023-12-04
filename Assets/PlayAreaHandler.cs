using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using System;
using TMPro;
using Unity.Burst.Intrinsics;


[Serializable]
public class PlayAreaDistanceConfig
{
    public String distanceName;
    public Vector3 offset;
}

[Serializable]
public class PlayAreaAccurancyConfig
{
    public String accuracyName;
    public float    transformTargetTolerance;
    public float rotationTargetTolerance;
    public float scaleTargetTolerance;
}

public class PlayAreaHandler : MonoBehaviour
{

    [SerializeField]
    private List<PlayAreaDistanceConfig> playAreaConfigurations = new List<PlayAreaDistanceConfig>();

    [SerializeField]
    private List<PlayAreaAccurancyConfig> playAreaAccuracyConfig = new List<PlayAreaAccurancyConfig>();
    
    public GameObject playArea;
    public ObjectGenerator objectGenerator;

    public PressableButton templateButton;
    public GameObject parentForDistanceButtons;

    public PressableButton useRotationButton;
    public PressableButton useScaleButotn;
    public PressableButton resetButton;
    public Slider accuracySlider;

    private Orbital _orbital;
    private Vector3 currentPlayAreaOffset;
    private Orbital orbital
    {
        get { if (_orbital == null)
            {
                _orbital = playArea.GetComponent<Orbital>();
            }
            return _orbital;
        }
    }

    private void Awake()
    {
        // Initialize the dictionary from the serialized list
        foreach (var config in playAreaConfigurations)
        {
            GameObject btn = Instantiate(templateButton.gameObject);
            btn.transform.SetParent(parentForDistanceButtons.transform);
            btn.SetActive(true);
            btn.GetComponentInChildren<TextMeshProUGUI>().SetText(config.distanceName);
            btn.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            btn.GetComponent<PressableButton>().OnClicked.AddListener(() =>
            {
                SetPlayArea(config.offset);
            });
        }

        accuracySlider.MaxValue = playAreaAccuracyConfig.Count - 1;
        accuracySlider.SliderStepDivisions = playAreaAccuracyConfig.Count - 1;
        accuracySlider.OnValueUpdated.AddListener((arg) =>
        {
            UpdateAccuranceSlider((int)arg.NewValue);
        });
    }

    void UpdateAccuranceSlider(int index)
    {
        PlayAreaAccurancyConfig config = playAreaAccuracyConfig[index];
        objectGenerator.taskConfiguration.scaleTolerance = config.scaleTargetTolerance;
        objectGenerator.taskConfiguration.transformTolerance = config.transformTargetTolerance;
        objectGenerator.taskConfiguration.rotationTolerance = config.rotationTargetTolerance;

        ResetGenerator();
    }

    void ResetGenerator()
    {
        objectGenerator.Reset();
    }

    void Start()
    {
        accuracySlider.Value = 0;
        UpdateAccuranceSlider((int)accuracySlider.Value);
        templateButton.gameObject.SetActive(false);
        currentPlayAreaOffset = playAreaConfigurations[0].offset;
        ResetPlayArea();
        resetButton.OnClicked.AddListener(() =>
        {
            ResetPlayArea();
        });
        useRotationButton.OnClicked.AddListener(() =>
        {
            objectGenerator.taskConfiguration.useRotation = useRotationButton.IsToggled;
            ResetPlayArea();
        });
        useScaleButotn.OnClicked.AddListener(() =>
        {
            objectGenerator.taskConfiguration.useScale = useScaleButotn.IsToggled;
            ResetPlayArea();
        });
    }

    public void SetPlayArea(Vector3 offset)
    {
        currentPlayAreaOffset = offset;
        ResetPlayArea();
    }

    public void ResetPlayArea()
    {
        orbital.WorldOffset = currentPlayAreaOffset;
        orbital.enabled = true;
        Invoke(nameof(ResetGenerator), 0.7f); // required or the old move task position is kept
    }
}

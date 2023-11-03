using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using System;

public enum PlayAreaType
{
    Short,
    Medium,
    Long
}

[Serializable]
public class PlayAreaConfiguration
{
    public PlayAreaType playAreaType;
    public Vector3 playAreaPosition;
    public PressableButton button;
}

public class PlayAreaHandler : MonoBehaviour
{

    [SerializeField]
    private List<PlayAreaConfiguration> playAreaConfigurations = new List<PlayAreaConfiguration>();
    public ObjectGenerator objectGenerator;
    private Dictionary<PlayAreaType, Vector3> playAreaDictionary;
    public GameObject playArea;
    private Orbital _orbital;
    public PlayAreaType currentPlayAreaType;
    public PressableButton reset;
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
        playAreaDictionary = new Dictionary<PlayAreaType, Vector3>();
        foreach (var config in playAreaConfigurations)
        {
            playAreaDictionary[config.playAreaType] = config.playAreaPosition;
            if (config.button != null)
            {
                config.button.OnClicked.AddListener(() =>
                {
                    SetPlayArea(config.playAreaType);
                });
            }
        }
        reset.OnClicked.AddListener(() =>
        {
            ResetPlayArea();
        });
    }

    void ResetGenerator()
    {
        objectGenerator.Reset();
    }

    void Start()
    {
        ResetPlayArea();
    }

    public void SetPlayArea(PlayAreaType playAreaType)
    {
        currentPlayAreaType = playAreaType;
        ResetPlayArea();
    }

    public void ResetPlayArea()
    {
        orbital.WorldOffset = playAreaDictionary[currentPlayAreaType];
        orbital.enabled = true;
        Invoke(nameof(ResetGenerator), 0.7f); // required or the old move task position is kept
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneMainMenu : MonoBehaviour
{
    public List<ButtonScenePair> buttonScenePairs = new();

    void Start()
    {
        foreach (var pair in buttonScenePairs)
        {
            pair.button.OnClicked.AddListener(() =>
            {
                SceneManager.LoadScene(pair.scene);
            });
        }
    }
}

[System.Serializable]
public class ButtonScenePair
{
    public PressableButton button;
    public String scene;
}
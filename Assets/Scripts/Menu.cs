using System;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public event Action HomeClicked;
    public event Action GameClicked;
    public event Action LearningClicked;
    public event Action SettingsClicked;

    public void OnHomeClicked() => HomeClicked?.Invoke();
    public void OnGameClicked() => GameClicked?.Invoke();
    public void OnLearningClicked() => LearningClicked?.Invoke();
    public void OnSettingsClicked() => SettingsClicked?.Invoke();
}
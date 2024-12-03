using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreen : MonoBehaviour
{
    [SerializeField] private Menu _menu;
    [SerializeField] private LearningScreen _learningScreen;
    [SerializeField] private QuizScreen _quizScreen;
    [SerializeField] private Settings _settings;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action LearningClikced;
    public event Action SettingsClicked;
    public event Action QuizClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _menu.LearningClicked += OnLearningClicked;
        _menu.GameClicked += OnGameClicked;
        _menu.SettingsClicked += OnSettingsClicked;
        _learningScreen.HomeClicked += _screenVisabilityHandler.EnableScreen;
        _quizScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
        _settings.HomeClicked += _screenVisabilityHandler.EnableScreen;
    }

    private void OnDisable()
    {
        _menu.LearningClicked -= OnLearningClicked;
        _menu.GameClicked -= OnGameClicked;
        _menu.SettingsClicked -= OnSettingsClicked;
        _learningScreen.HomeClicked -= _screenVisabilityHandler.EnableScreen;
        _quizScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
        _settings.HomeClicked -= _screenVisabilityHandler.EnableScreen;
    }

    public void OnGameClicked()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnQuizClicked()
    {
        QuizClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnLearningClicked()
    {
        LearningClikced?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnSettingsClicked()
    {
        SettingsClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }
}

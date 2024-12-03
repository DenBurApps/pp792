using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private GameObject _privacyCanvas;
    [SerializeField] private GameObject _termsCanvas;
    [SerializeField] private GameObject _contactCanvas;
    [SerializeField] private GameObject _versionCanvas;
    [SerializeField] private TMP_Text _versionText;
    [SerializeField] private Menu _menu;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private LearningScreen _learningScreen;
    private string _version = "Application version:\n";

    public event Action HomeClicked;
    public event Action LearningClicked;
    
    private void Awake()
    {
        _settingsCanvas.SetActive(false);
        _privacyCanvas.SetActive(false);
        _termsCanvas.SetActive(false);
        _contactCanvas.SetActive(false);
        _versionCanvas.SetActive(false);
        SetVersion();
    }

    private void OnEnable()
    {
        _learningScreen.SettingsClicked += ShowSettings;
        _mainScreen.SettingsClicked += ShowSettings;
        
        _menu.LearningClicked += OnLearningClicked;
        _menu.GameClicked += OnGameClicked;
        _menu.HomeClicked += OnHomeClicked;
    }

    private void OnDisable()
    {
        _learningScreen.SettingsClicked -= ShowSettings;
        _mainScreen.SettingsClicked -= ShowSettings;
        
        _menu.LearningClicked -= OnLearningClicked;
        _menu.GameClicked -= OnGameClicked;
        _menu.HomeClicked -= OnHomeClicked;
    }

    private void SetVersion()
    {
        _versionText.text = _version + Application.version;
    }

    public void ShowSettings()
    {
        _settingsCanvas.SetActive(true);
    }

    public void RateUs()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#endif
    }

    private void OnGameClicked()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnHomeClicked()
    {
        HomeClicked?.Invoke();
        _settingsCanvas.SetActive(false);
    }

    private void OnLearningClicked()
    {
        LearningClicked?.Invoke();
        _settingsCanvas.SetActive(false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class LearningScreen : MonoBehaviour
{
    [SerializeField] private List<ArticleData> _actualData;
    [SerializeField] private List<ArticlePlane> _actualPlanes;
    [SerializeField] private OpenArticleScreen _openArticleScreen;
    [SerializeField] private List<ArticleData> _articleDatas;
    [SerializeField] private List<ArticlePlane> _articlePlanes;
    [SerializeField] private List<LearningTag> _learningTags;
    [SerializeField] private Menu _menu;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private Settings _settings;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private LearningTag _currentTag;
    
    public event Action<ArticleData> PlaneOpened;
    public event Action HomeClicked;
    public event Action SettingsClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }
    
    private void OnEnable()
    {
        foreach (var plane in _actualPlanes)
        {
            plane.Opened += OnPlaneOpened;
        }
        
        foreach (var tag in _learningTags)
        {
            tag.CategorySelected += LearningTagSelected;
        }

        foreach (var plane in _articlePlanes)
        {
            plane.Opened += OnPlaneOpened;
        }

        _openArticleScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
        _menu.HomeClicked += OnHomeClicked;
        _menu.GameClicked += OnGameClicked;
        _menu.SettingsClicked += OnSettingsClicked;
        _mainScreen.LearningClikced += _screenVisabilityHandler.EnableScreen;
        _settings.LearningClicked += _screenVisabilityHandler.EnableScreen;
    }
    
    private void OnDisable()
    {
        foreach (var plane in _actualPlanes)
        {
            plane.Opened -= OnPlaneOpened;
        }
        
        foreach (var tag in _learningTags)
        {
            tag.CategorySelected -= LearningTagSelected;
        }

        foreach (var plane in _articlePlanes)
        {
            plane.Opened -= OnPlaneOpened;
        }
        
        _openArticleScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
        _menu.HomeClicked -= OnHomeClicked;
        _menu.GameClicked -= OnGameClicked;
        _menu.SettingsClicked -= OnSettingsClicked;
        _mainScreen.LearningClikced -= _screenVisabilityHandler.EnableScreen;
        _settings.LearningClicked -= _screenVisabilityHandler.EnableScreen;
    }

    private void Start()
    {
        for (int i = 0; i < _actualData.Count; i++)
        {
            _actualPlanes[i].Enable(_actualData[i]);
        }
        
        var chosenTag = _learningTags.FirstOrDefault();
        LearningTagSelected(chosenTag);
        
        _screenVisabilityHandler.DisableScreen();
    }

    private void LearningTagSelected(LearningTag tag)
    {
        DisableAllPlanes();

        foreach (var article in _articleDatas)
        {
            if (article.Category == tag.LearningCategory)
            {
                var availableArticle = _articlePlanes.FirstOrDefault(a => !a.IsActive);
                availableArticle?.Enable(article);
            }
        }

        SetCurrentTag(tag);
    }

    private void SetCurrentTag(LearningTag tag)
    {
        if (_currentTag != null)
        {
            _currentTag.SetSelected(false);
        }

        _currentTag = tag;
        _currentTag.SetSelected(true);
    }

    private void DisableAllPlanes()
    {
        foreach (var plane in _articlePlanes)
        {
            plane.Disable();
        }
    }

    private void OnPlaneOpened(ArticleData data)
    {
        PlaneOpened?.Invoke(data);
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnHomeClicked()
    {
        HomeClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnGameClicked()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnSettingsClicked()
    {
        SettingsClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }
}

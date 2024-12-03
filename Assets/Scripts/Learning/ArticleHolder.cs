using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class ArticleHolder : MonoBehaviour
{
    [SerializeField] private List<LearningTag> _learningTags;
    [SerializeField] private List<ArticleData> _articleDatas;
    [SerializeField] private List<ArticlePlane> _articlePlanes;
    [SerializeField] private OpenArticleScreen _openArticleScreen;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    private LearningTag _currentTag;
    
    public event Action<ArticleData> PlaneOpened;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        foreach (var tag in _learningTags)
        {
            tag.CategorySelected += LearningTagSelected;
        }

        foreach (var plane in _articlePlanes)
        {
            plane.Opened += OnPlaneOpened;
        }

        _openArticleScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
    }

    private void OnDisable()
    {
        foreach (var tag in _learningTags)
        {
            tag.CategorySelected -= LearningTagSelected;
        }

        foreach (var plane in _articlePlanes)
        {
            plane.Opened -= OnPlaneOpened;
        }
        
        _openArticleScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
    }

    private void Start()
    {
        var chosenTag = _learningTags.FirstOrDefault();
        LearningTagSelected(chosenTag);
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
}
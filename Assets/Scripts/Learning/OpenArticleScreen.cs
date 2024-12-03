using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OpenArticleScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _subtitle;
    [SerializeField] private TMP_Text _content;
    [SerializeField] private ArticleHolder _articleHolder;
    [SerializeField] private LearningScreen _learningScreen;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action BackClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnEnable()
    {
        if (_articleHolder != null)
            _articleHolder.PlaneOpened += Enable;

        if (_learningScreen != null)
            _learningScreen.PlaneOpened += Enable;
    }

    private void OnDisable()
    {
        if (_articleHolder != null)
            _articleHolder.PlaneOpened -= Enable;
        
        if (_learningScreen != null)
            _learningScreen.PlaneOpened -= Enable;
    }

    public void Enable(ArticleData data)
    {
        _screenVisabilityHandler.EnableScreen();

        _title.text = data.Title;
        _image.sprite = data.Sprite;
        _subtitle.text = data.Subtitle;
        _content.text = data.Content;
    }

    public void OnBackClicked()
    {
        BackClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }
}
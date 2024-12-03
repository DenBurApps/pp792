using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArticlePlane : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _subtitle;
    [SerializeField] private TMP_Text _duration;
    [SerializeField] private Button _readButton;

    public event Action<ArticleData> Opened;

    public ArticleData Data { get; private set; }

    public bool IsActive { get; private set; }

    private void OnEnable()
    {
        _readButton.onClick.AddListener(OnOpened);
    }

    private void OnDisable()
    {
        _readButton.onClick.RemoveListener(OnOpened);
    }

    public void Enable(ArticleData data)
    {
        gameObject.SetActive(true);
        IsActive = true;

        Data = data;

        _image.sprite = Data.Sprite;
        _title.text = Data.Title;

        if (_subtitle != null)
            _subtitle.text = Data.Subtitle;

        if (_duration != null)
            _duration.text = Data.Duration;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
    }

    private void OnOpened() => Opened?.Invoke(Data);
}

[Serializable]
public class ArticleData
{
    public string Content;
    public string Title;
    public string Subtitle;
    public Sprite Sprite;
    public string Duration;
    public LearningCategory Category;
}
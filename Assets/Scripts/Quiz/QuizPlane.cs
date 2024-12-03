using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class QuizData
{
    public QuizType Type;
    public bool IsComplete;
}

[RequireComponent(typeof(Button))]
public class QuizPlane : MonoBehaviour
{
    [SerializeField] private Sprite _complete;
    [SerializeField] private Sprite _notPassed;

    [SerializeField] private TMP_Text _questionNumber;
    [SerializeField] private Image _mainImage;
    [SerializeField] private Image _image;
    [SerializeField] private QuizData _quizData;
    [SerializeField] private string _content;

    private Button _button;

    public event Action<QuizPlane> Opened;
    public event Action<QuizData> CompletionChanged;
    [field: SerializeField] public string QuestionNumber { get; private set; }

    public Sprite MainSprite => _mainImage.sprite;
    public string Content => _content;
    public QuizData Data => _quizData;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _image.sprite = _quizData.IsComplete ? _complete : _notPassed;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }

    public void UpdateCompletion(bool isComplete)
    {
        _quizData.IsComplete = isComplete;
        _image.sprite = _quizData.IsComplete ? _complete : _notPassed;

        CompletionChanged?.Invoke(_quizData);
    }

    private void OnButtonClicked()
    {
        Opened?.Invoke(this);
    }
}

public enum QuizType
{
    Pair,
    TwoPairs,
    Triple,
    Street,
    Flush,
    FullHouse,
    FourAKind,
    StreetFlush,
    RoyalFlush
}
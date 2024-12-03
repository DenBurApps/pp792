using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OpenQuizScreen : MonoBehaviour
{
    [SerializeField] private List<AnswerButton> _answerButtons;
    [SerializeField] private QuizScreen _quizScreen;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private TMP_Text _questionContent;
    [SerializeField] private Image _image;

    private AnswerButton _currentButton;
    private QuizPlane _currentPlane;
    private int _currentIndex;
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action BackClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        UnselectAllButtons();
        Disable();
    }

    private void OnEnable()
    {
        foreach (var button in _answerButtons)
        {
            button.Clicked += SelectButton;
        }

        _quizScreen.QuizOpened += Enable;
    }

    private void OnDisable()
    {
        foreach (var button in _answerButtons)
        {
            button.Clicked -= SelectButton;
        }

        _quizScreen.QuizOpened -= Enable;
    }

    private void Enable(QuizPlane plane)
    {
        if (plane == null)
            return;

        _screenVisabilityHandler.EnableScreen();
        UnselectAllButtons();
        _currentIndex = _quizScreen.Planes.IndexOf(plane);
        if (_currentIndex >= 0)
        {
            _currentPlane = plane;

            if (_currentPlane.Data.IsComplete)
            {
                AnswerButton button = _answerButtons.Find(b => b.Type == _currentPlane.Data.Type);
                SelectButton(button);
            }

            UpdateUi(_currentPlane);
        }
    }

    private void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnBackClicked()
    {
        BackClicked?.Invoke();
        Disable();
    }

    public void OnNextClicked()
    {
        if (_quizScreen.Planes == null || _quizScreen.Planes.Count == 0)
            return;

        ValidateAnswer();

        _currentIndex = (_currentIndex + 1) % _quizScreen.Planes.Count;

        _currentPlane = _quizScreen.Planes[_currentIndex];
        UpdateUi(_currentPlane);
        UnselectAllButtons();

        if (_currentPlane.Data.IsComplete)
        {
            AnswerButton button = _answerButtons.Find(b => b.Type == _currentPlane.Data.Type);
            SelectButton(button);
        }
    }

    private void UpdateUi(QuizPlane plane)
    {
        if (plane == null)
            return;

        _questionText.text = plane.QuestionNumber;
        _questionContent.text = plane.Content;
        _image.sprite = plane.MainSprite;
    }

    private void UnselectAllButtons()
    {
        foreach (var button in _answerButtons)
        {
            button.SetSelectedStatus(false);
        }
    }

    private void SelectButton(AnswerButton button)
    {
        if (_currentButton != null)
        {
            _currentButton.SetSelectedStatus(false);
        }

        _currentButton = button;
        _currentButton.SetSelectedStatus(true);
    }

    private void ValidateAnswer()
    {
        if (_currentButton == null || _currentPlane == null)
            return;

        bool isCorrect = _currentButton.Type == _currentPlane.Data.Type;
        _currentPlane.UpdateCompletion(isCorrect);
    }
}
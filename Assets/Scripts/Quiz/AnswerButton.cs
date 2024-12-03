using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    [SerializeField] private Color _notSelectedColor;
    [SerializeField] private Sprite _notSelectedButtonSprite;
    [SerializeField] private Sprite _notSelectedSprite;

    [SerializeField] private Color _selectedColor;
    [SerializeField] private Sprite _selectedButtonSprite;
    [SerializeField] private Sprite _selectedSprite;

    [SerializeField] private QuizType _type;

    [SerializeField] private Button _button;
    [SerializeField] private Image _image;

    public event Action<AnswerButton> Clicked;
    public QuizType Type => _type;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClicked);
    }

    public void SetSelectedStatus(bool status)
    {
        if (status)
        {
            _button.image.sprite = _selectedSprite;
            _button.image.color = _selectedColor;
            _image.sprite = _selectedButtonSprite;
        }
        else
        {
            _button.image.sprite = _notSelectedSprite;
            _button.image.color = _notSelectedColor;
            _image.sprite = _notSelectedButtonSprite;
        }
    }

    private void OnClicked()
    {
        Clicked?.Invoke(this);
    }
}

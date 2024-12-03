using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LearningTag : MonoBehaviour
{
    [SerializeField] private Color _selectedTagColor;
    [SerializeField] private Color _selectedTextColor;
    [SerializeField] private LearningCategory _learningCategory;
    [SerializeField] private TMP_Text _text;

    private Color _defaultTagColor;
    private Color _defaultTextColor;
    private Button _button;
    private bool _isSelected;

    public event Action<LearningTag> CategorySelected;
    public LearningCategory LearningCategory => _learningCategory;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _defaultTagColor = _button.image.color;
        _defaultTextColor = _text.color;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }
    
    public void SetSelected(bool status)
    {
        if (status)
        {
            _button.image.color = _selectedTagColor;
            _text.color = _selectedTextColor;
        }
        else
        {
            _button.image.color = _defaultTagColor;
            _text.color = _defaultTextColor;
        }
    }

    private void OnButtonClicked()
    {
        _isSelected = !_isSelected;
        CategorySelected?.Invoke(this);
    }
}

public enum LearningCategory
{
    Rules,
    Analysis,
    Strategy,
    Dealing,
}
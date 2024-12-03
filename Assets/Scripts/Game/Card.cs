using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour
{
    [SerializeField] private int _value = 0;
    [SerializeField] private Sprite _backSprite;
    [SerializeField] private Deck _deck;

    private SpriteRenderer _image;

    private void Awake()
    {
        _image = GetComponent<SpriteRenderer>();
    }

    public int GetValueOfCard()
    {
        return _value;
    }

    public void SetValue(int newValue)
    {
        _value = newValue;
    }
    
    public void SetSprite(Sprite newSprite)
    {
        _image.sprite = newSprite;
    }

    public void ResetCard()
    {
        _backSprite = _deck.GetCardBack();
        _image.sprite = _backSprite;
        _value = 0;
    }
}

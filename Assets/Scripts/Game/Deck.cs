using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Sprite[] _cardSprites;

    private readonly int[] _cardValues = new int[53];
    private int _currentIndex;

    private void Start()
    {
        InitializeCardValues();
        Shuffle();
    }

    private void InitializeCardValues()
    {
        int num = 0;

        for (int i = 0; i < _cardSprites.Length; i++)
        {
            num = i;
            num %= 13;

            if (num > 10 || num == 0)
            {
                num = 10;
            }

            _cardValues[i] = num++;
        }
    }

    public void Shuffle()
    {
        for (int i = _cardSprites.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // Generate a random index from 0 to i

            // Swap sprites
            Swap(ref _cardSprites[i], ref _cardSprites[j]);
            // Swap values
            Swap(ref _cardValues[i], ref _cardValues[j]);
        }

        _currentIndex = 0; 
    }

    public int DealCard(Card card)
    {
        if (_currentIndex >= _cardSprites.Length)
        {
            Debug.LogWarning("No more cards to deal.");
            return -1;
        }

        card.SetSprite(_cardSprites[_currentIndex]);
        card.SetValue(_cardValues[_currentIndex]);
        _currentIndex++;

        return card.GetValueOfCard();
    }

    public Sprite GetCardBack()
    {
        return _cardSprites[0];
    }

    private void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }
}
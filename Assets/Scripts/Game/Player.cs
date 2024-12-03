using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Deck _deck;
    [SerializeField] private GameObject[] _handSlots;

    private List<Card> _aceList = new();
    private int _handValue = 0;
    private int _currentBalance = 0;
    private int _betAmount = 0;
    private int _cardIndex = 0;
    
    public int CurrentBalance => _currentBalance;
    public int HandValue => _handValue;
    public int BetAmount => _betAmount;
    public int CardIndex => _cardIndex;
    
    public event Action<int> BalanceAmountChanged;
    
    public void StartHand()
    {
        DrawCard();
        DrawCard();
    }
    
    public int DrawCard()
    {
        if (_cardIndex >= _handSlots.Length)
        {
            Debug.LogWarning("No more slots available for cards.");
            return _handValue;
        }

        Card card = _handSlots[_cardIndex].GetComponent<Card>();
        int cardValue = _deck.DealCard(card);

        _handSlots[_cardIndex].GetComponent<SpriteRenderer>().enabled = true;
        _handValue += cardValue;

        if (cardValue == 1)
        {
            _aceList.Add(card);
        }

        AdjustAces();
        _cardIndex++;
        return _handValue;
    }
    
    private void AdjustAces()
    {
        foreach (var ace in _aceList)
        {
            if (_handValue + 10 <= 21 && ace.GetValueOfCard() == 1)
            {
                ace.SetValue(11);
                _handValue += 10;
            }
            else if (_handValue > 21 && ace.GetValueOfCard() == 11)
            {
                ace.SetValue(1);
                _handValue -= 10;
            }
        }
    }
    
    public void SetBetAmount(int amount)
    {
        if (amount >= 0)
        {
            _betAmount = amount;
        }
        else
        {
            Debug.LogWarning("Bet amount cannot be negative.");
        }
    }
    
    public void SetBalance(int amount)
    {
        if (amount >= 0)
        {
            _currentBalance = amount;
            BalanceAmountChanged?.Invoke(_currentBalance);
        }
        else
        {
            Debug.LogWarning("Balance amount cannot be negative.");
        }
    }

    public void ResetHand()
    {
        foreach (var slot in _handSlots)
        {
            var card = slot.GetComponent<Card>();
            card.ResetCard();
            slot.GetComponent<SpriteRenderer>().enabled = false;
        }

        _cardIndex = 0;
        _handValue = 0;
        _aceList.Clear();
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;

public class GameController : MonoBehaviour
{
    [Header("UI Elements")] [SerializeField]
    private GameObject _betArea;

    [SerializeField] private TMP_Text _currentBetText;
    [SerializeField] private TMP_Text _playerHandText;
    [SerializeField] private TMP_Text _dealerHandText;
    [SerializeField] private TMP_Text _winLoseText;

    [Header("Buttons")] [SerializeField] private Button _standButton;
    [SerializeField] private Button _doubleButton;
    [SerializeField] private Button _hitButton;
    [SerializeField] private Button _exitButton;

    [Header("Game Components")] [SerializeField]
    private StartWindow _startWindow;

    [SerializeField] private Player _player;
    [SerializeField] private Player _dealer;
    [SerializeField] private Deck _deck;
    [SerializeField] private GameObject _hideCard;
    [SerializeField] private Card _dealerSecondCard;

    private int _standClicks = 0;

    public event Action GameEnded;

    private void Start()
    {
        ToggleGameElements(false);
        InitializeUI();
    }

    private void OnEnable()
    {
        _startWindow.DealButtonClicked += StartGame;
        _standButton.onClick.AddListener(OnStandClicked);
        _doubleButton.onClick.AddListener(OnDoubleClicked);
        _hitButton.onClick.AddListener(OnHitClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnDisable()
    {
        _startWindow.DealButtonClicked -= StartGame;
        _standButton.onClick.RemoveListener(OnStandClicked);
        _doubleButton.onClick.RemoveListener(OnDoubleClicked);
        _hitButton.onClick.RemoveListener(OnHitClicked);
    }

    private void StartGame()
    {
        ToggleGameElements(true);
        ResetGame();

        _deck.Shuffle();

        _player.SetBetAmount(_startWindow.CurrentBet);
        _player.SetBalance(_startWindow.PlayerBalance);
        _player.StartHand();
        _dealer.StartHand();

        UpdateHandUI();
        UpdateBetAndBalance();
    }

    private void ResetGame()
    {
        _player.ResetHand();
        _dealer.ResetHand();

        _standClicks = 0;
        _winLoseText.text = "";
        _hideCard.SetActive(true);

        EnableButtons(true);
    }

    private void OnExitClicked()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnHitClicked()
    {
        if (_player.CardIndex >= 10) return;

        _player.DrawCard();
        UpdateHandUI();

        if (_player.HandValue > 21)
            EndRound();
    }

    private void OnDoubleClicked()
    {
        if (_player.CurrentBalance < _player.BetAmount * 2)
        {
            _doubleButton.interactable = false;
            return;
        }

        _player.SetBalance(_player.CurrentBalance - _player.BetAmount);
        _player.SetBetAmount(_player.BetAmount * 2);

        UpdateBetAndBalance();
        OnHitClicked();
    }

    private void OnStandClicked()
    {
        _standClicks++;

        if (_standClicks > 1)
        {
            EndRound();
        }
        else
        {
            DealerPlay();
            _standButton.GetComponentInChildren<TMP_Text>().text = "Call";
        }
    }

    private void DealerPlay()
    {
        while (_dealer.HandValue < 16 && _dealer.CardIndex < 10)
        {
            _dealer.DrawCard();
            UpdateDealerHandUI();

            if (_dealer.HandValue > 21)
                EndRound();
        }
    }

    private void EndRound()
    {
        bool playerBust = _player.HandValue > 21;
        bool dealerBust = _dealer.HandValue > 21;

        if (playerBust && dealerBust)
        {
            DisplayResult("All Bust! Bets returned");
            _player.SetBalance(_player.CurrentBalance + _player.BetAmount);
        }
        else if (playerBust || (_dealer.HandValue > _player.HandValue && !dealerBust))
        {
            DisplayResult("Dealer wins!");
        }
        else if (dealerBust || _player.HandValue > _dealer.HandValue)
        {
            DisplayResult("You win!");
            _player.SetBalance(_player.CurrentBalance + _player.BetAmount * 2);
        }
        else
        {
            DisplayResult("Push: Bets returned");
            _player.SetBalance(_player.CurrentBalance + _player.BetAmount);
        }

        StartCoroutine(FinalizeRound());
    }

    private IEnumerator FinalizeRound()
    {
        EnableButtons(false);
        _hideCard.SetActive(false);
        _dealerHandText.text = _dealer.HandValue.ToString();
        _standClicks = 0;

        yield return new WaitForSeconds(2f);
        GameEnded?.Invoke();
        ToggleGameElements(false);
    }

    private void DisplayResult(string resultText)
    {
        _winLoseText.text = resultText;
    }

    private void UpdateHandUI()
    {
        _playerHandText.text = _player.HandValue.ToString();
        _dealerHandText.text = (_dealer.HandValue - _dealerSecondCard.GetValueOfCard()).ToString();
    }

    private void UpdateDealerHandUI()
    {
        _dealerHandText.text = (_dealer.HandValue - _dealerSecondCard.GetValueOfCard()).ToString();
    }

    private void UpdateBetAndBalance()
    {
        _currentBetText.text = "Bet: " + _player.BetAmount.ToString("N0") + "$";
    }

    private void EnableButtons(bool state)
    {
        _hitButton.interactable = state;
        _doubleButton.interactable = state;
        _standButton.interactable = state;
    }

    private void ToggleGameElements(bool status)
    {
        _betArea.SetActive(status);
        _standButton.gameObject.SetActive(status);
        _doubleButton.gameObject.SetActive(status);
        _hitButton.gameObject.SetActive(status);
        _player.gameObject.SetActive(status);
        _dealer.gameObject.SetActive(status);
        _winLoseText.gameObject.SetActive(status);
    }

    private void InitializeUI()
    {
        _winLoseText.text = "";
        _currentBetText.text = "0";
    }
}
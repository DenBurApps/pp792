using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartWindow : MonoBehaviour
{
    private const int InitBalance = 3000;
    private const int MinBet = 10;

    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _balanceText;
    [SerializeField] private TMP_Text _currentBetText;
    [SerializeField] private TMP_Text _warningMessage;

    [SerializeField] private Button _5Chip;
    [SerializeField] private Button _10Chip;
    [SerializeField] private Button _25Chip;
    [SerializeField] private Button _50Chip;
    [SerializeField] private Button _100Chip;

    [SerializeField] private Button _5ChipToLower;
    [SerializeField] private Button _10ChipToLower;
    [SerializeField] private Button _25ChipToLower;
    [SerializeField] private Button _50ChipToLower;
    [SerializeField] private Button _100ChipToLower;

    [SerializeField] private Button _dealButton;
    [SerializeField] private Button _maxBetButton;

    [SerializeField] private Player _player;
    [SerializeField] private GameController _gameController;

    private int _playerBalance;
    private int _currentBet;
    private bool _isMaxBetSet;

    private readonly Dictionary<Button, int> _chipLowerCounters = new();

    public event Action DealButtonClicked;

    public int PlayerBalance => _playerBalance;

    public int CurrentBet => _currentBet;

    private void Start()
    {
        _playerBalance = InitBalance;
        _maxBetButton.onClick.AddListener(SetMaxBet);
        InitChipLowerButtons();
        StartNewRound();
    }

    private void StartNewRound()
    {
        ToggleAllElements(true);
        _balanceText.text = "$" + _playerBalance.ToString("N0");
        _warningMessage.gameObject.SetActive(false);
        _dealButton.gameObject.SetActive(false);
        
        _isMaxBetSet = false;
        
        ProcessGameStart();
    }

    private void InitChipLowerButtons()
    {
        _chipLowerCounters[_5ChipToLower] = 0;
        _chipLowerCounters[_10ChipToLower] = 0;
        _chipLowerCounters[_25ChipToLower] = 0;
        _chipLowerCounters[_50ChipToLower] = 0;
        _chipLowerCounters[_100ChipToLower] = 0;

        foreach (var button in _chipLowerCounters.Keys)
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(() => ProcessChipToLowerButtonClicked(button));
        }
    }

    private void OnEnable()
    {
        _exitButton.onClick.AddListener(ProcessExitButtonClicked);

        _5Chip.onClick.AddListener(() => ProcessChipButtonClicked(5, _5Chip, _5ChipToLower));
        _10Chip.onClick.AddListener(() => ProcessChipButtonClicked(10, _10Chip, _10ChipToLower));
        _25Chip.onClick.AddListener(() => ProcessChipButtonClicked(25, _25Chip, _25ChipToLower));
        _50Chip.onClick.AddListener(() => ProcessChipButtonClicked(50, _50Chip, _50ChipToLower));
        _100Chip.onClick.AddListener(() => ProcessChipButtonClicked(100, _100Chip, _100ChipToLower));
        
        _dealButton.onClick.AddListener(ProcessDealButtonPressed);

        _player.BalanceAmountChanged += SetNewBalance;
        _gameController.GameEnded += StartNewRound;
    }

    private void OnDisable()
    {
        _exitButton.onClick.RemoveListener(ProcessExitButtonClicked);

        _5Chip.onClick.RemoveListener(() => ProcessChipButtonClicked(5, _5Chip, _5ChipToLower));
        _10Chip.onClick.RemoveListener(() => ProcessChipButtonClicked(10, _10Chip, _10ChipToLower));
        _25Chip.onClick.RemoveListener(() => ProcessChipButtonClicked(25, _25Chip, _25ChipToLower));
        _50Chip.onClick.RemoveListener(() => ProcessChipButtonClicked(50, _50Chip, _50ChipToLower));
        _100Chip.onClick.RemoveListener(() => ProcessChipButtonClicked(100, _100Chip, _100ChipToLower));
        _dealButton.onClick.RemoveListener(ProcessDealButtonPressed);

        foreach (var button in _chipLowerCounters.Keys)
        {
            button.onClick.RemoveListener(() => ProcessChipToLowerButtonClicked(button));
        }

        _player.BalanceAmountChanged -= SetNewBalance;
        _gameController.GameEnded -= StartNewRound;
    }

    public void ResetCurrentBet()
    {
        _currentBet = 0;
    }

    private void SetMaxBet()
    {
        if (_playerBalance >= MinBet)
        {
            _currentBet = _playerBalance; // Set current bet to the player's balance.
            _isMaxBetSet = true; // Mark max bet as set.
            UpdateCurrentBetText();

            // Get a copy of the keys to avoid modifying the collection during enumeration.
            List<Button> chipButtons = new List<Button>(_chipLowerCounters.Keys);

            foreach (var chip in chipButtons)
            {
                int chipValue = GetChipValue(chip);
                _chipLowerCounters[chip] = _currentBet / chipValue; // Update the count.
                if (_chipLowerCounters[chip] > 0)
                    chip.gameObject.SetActive(true); // Show the chip button if it has a valid count.
            }

            _dealButton.gameObject.SetActive(true);
        }
        else
        {
            StartCoroutine(ShowWarningMessage($"You need at least ${MinBet} to set max bet!"));
        }
    }

    private void ProcessChipButtonClicked(int chipValue, Button chipButton, Button chipToLowerButton)
    {
        if (_isMaxBetSet)
        {
            StartCoroutine(ShowWarningMessage("You can't increase your bet after setting max bet!"));
            return;
        }

        if (_playerBalance >= chipValue)
        {
            _currentBet += chipValue;
            _chipLowerCounters[chipToLowerButton]++;
            chipToLowerButton.gameObject.SetActive(true);
            
            UpdateCurrentBetText();
        }

        _dealButton.gameObject.SetActive(true);
        
    }

    private void ProcessChipToLowerButtonClicked(Button chipToLowerButton)
    {
        int chipValue = GetChipValue(chipToLowerButton);

        if (_chipLowerCounters[chipToLowerButton] > 0 && _currentBet >= chipValue)
        {
            _currentBet -= chipValue;
            _chipLowerCounters[chipToLowerButton]--;

            if (_chipLowerCounters[chipToLowerButton] == 0)
                chipToLowerButton.gameObject.SetActive(false);

            UpdateCurrentBetText();

            if (_currentBet < _playerBalance)
            {
                _isMaxBetSet = false;
            }
        }

        if (_currentBet <= 0)
        {
            foreach (var button in _chipLowerCounters.Keys)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    private int GetChipValue(Button chipButton)
    {
        if (chipButton == _5ChipToLower) return 5;
        if (chipButton == _10ChipToLower) return 10;
        if (chipButton == _25ChipToLower) return 25;
        if (chipButton == _50ChipToLower) return 50;
        if (chipButton == _100ChipToLower) return 100;

        return 0;
    }

    private void ProcessDealButtonPressed()
    {
        if (_currentBet < MinBet)
        {
            StartCoroutine(ShowWarningMessage($"The minimum bet is ${MinBet}!"));
            return;
        }

        if (_playerBalance > 0)
        {
            _playerBalance -= _currentBet;
        }
        
        ToggleAllElements(false);
        DealButtonClicked?.Invoke();
    }

    private void ToggleAllElements(bool status)
    {
        _dealButton.gameObject.SetActive(status);
        _maxBetButton.gameObject.SetActive(status);
        
        List<Button> chipButtons = new List<Button>(_chipLowerCounters.Keys);

        foreach (var button in chipButtons)
        {
            _chipLowerCounters[button] = 0;
            button.gameObject.SetActive(status);
        }
        
        _5Chip.gameObject.SetActive(status);
        _10Chip.gameObject.SetActive(status);
        _25Chip.gameObject.SetActive(status);
        _50Chip.gameObject.SetActive(status);
        _100Chip.gameObject.SetActive(status);
        _currentBetText.gameObject.SetActive(status);
    }

    private IEnumerator ShowWarningMessage(string message)
    {
        _warningMessage.text = message;
        _warningMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _warningMessage.gameObject.SetActive(false);
    }

    private void UpdateCurrentBetText()
    {
        _currentBetText.text = "Bet: " + _currentBet.ToString("N0") + "$";
    }

    private void ReturnToDefaultState()
    {
        List<Button> chipButtons = new List<Button>(_chipLowerCounters.Keys);

        foreach (var button in chipButtons)
        {
            _chipLowerCounters[button] = 0;
            button.gameObject.SetActive(false);
        }

        _currentBet = 0;
        _currentBetText.text = "Bet: 0$";

        _dealButton.gameObject.SetActive(false);
    }

    private void SetNewBalance(int amount)
    {
        _playerBalance = amount;
        _balanceText.text = "$" + _playerBalance.ToString("N0");
    }

    private void ProcessExitButtonClicked()
    {
        ReturnToDefaultState();
        SceneManager.LoadScene("MainScene");
    }

    private void ProcessGameStart()
    {
        ReturnToDefaultState();
    }
}
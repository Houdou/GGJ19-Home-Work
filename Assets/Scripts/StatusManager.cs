using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class Config {
    public static readonly int WorkDayInWeek = 3;
    public static readonly int RestDayInWeek = 1;
    
    public static readonly int DaysInWeek = RestDayInWeek + WorkDayInWeek;

    public static readonly int MorningHoursInDay = 4;
    public static readonly int AfternoonHoursInDay = 4;
    public static readonly int NightHoursInDay = 4;
    public static readonly int HoursInDay = 12;
    
    public static readonly float HoursInRealSecond = 3.0f;
}

public class GameStatus {

    private GameTime _currentTime;

    public GameTime CurrentTime {
        get { return _currentTime; }
        set {
            var diff = value - _currentTime;
            if (diff != GameTime.zero) {
                OnGameTimeChange?.Invoke(value, diff);
            }

            _currentTime = value;
        }
    }

    private int _energy;

    public int Energy {
        get { return _energy; }
        set {
            var diff = value - _energy;
            if (diff != 0) {
                OnEnergyChange?.Invoke(value, diff);
            }

            _energy = value;
        }
    }

    private int _money;

    public int Money {
        get { return _money; }
        set {
            var diff = value - _money;
            if (diff != 0) {
                OnMoneyChange?.Invoke(value, diff);
            }

            _money = value;
        }
    }

    public event Action<int, int> OnEnergyChange;
    public event Action<int, int> OnMoneyChange;
    public event Action<GameTime, GameTime> OnGameTimeChange;

    public int TotalHour => CurrentTime.TotalHourInGame;

    public GameStatus(GameTime gameTime = default(GameTime), int money = 100, int energy = 100) {
        _currentTime = gameTime;
        _money = Money = money;
        _energy = Energy = energy;
    }

    public void Merge(StatusChangeData statusChange) {
        Money += statusChange.Money;
        Energy += statusChange.Energy;
    }

    public void Replace(GameStatus status, bool triggerEvents = false) {
        if (triggerEvents) {
            CurrentTime = status.CurrentTime;
            Energy = status.Energy;
            Money = status.Money;
        } else {
            _currentTime = status.CurrentTime;
            _energy = status.Energy;
            _money = status.Money;
        }

    }
}

public class StatusManager : MonoBehaviour {
    #region Singleton

    private static StatusManager instance;

    private static object _lock = new object();

    public static StatusManager Instance {
        get {
            if (applicationIsQuitting) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(StatusManager) +
                                 "' already destroyed on application quit." +
                                 " Won't create again - returning null.");
                return null;
            }

            lock (_lock) {
                if (instance == null) {
                    instance = (StatusManager) FindObjectOfType(typeof(StatusManager));

                    if (FindObjectsOfType(typeof(StatusManager)).Length > 1) {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       " - there should never be more than 1 singleton!" +
                                       " Reopening the scene might fix it.");
                        return instance;
                    }

                    if (instance == null) {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<StatusManager>();
                        singleton.name = "(singleton) " + typeof(StatusManager);

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(StatusManager) +
                                  " is needed in the scene, so '" + singleton +
                                  "' was created with DontDestroyOnLoad.");
                    }
                    else {
                        Debug.Log("[Singleton] Using instance already created: " +
                                  instance.gameObject.name);
                    }
                }

                return instance;
            }
        }
    }

    private static bool applicationIsQuitting;

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy() {
        applicationIsQuitting = true;
    }

    #endregion

    private GameStatus _status;
    
    private List<BaseEvent> _listEvents;
    private void Awake() {
         _listEvents = new List<BaseEvent>();
         _energyText = EnergyUI.GetComponentInChildren<TextMeshProUGUI>();
         _moneyText = MoneyUI.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update() {
    }


    public void Init() {
        _status = new GameStatus();
        _status.OnMoneyChange += (value, diff) => {
            Debug.Log(value);
            if (value >= 2000) {
                Debug.Log("Call draw rich house");
            }
        };
    }

    public void LoadStatus(GameStatus status) {
        _status.Replace(status);
    }
    
    public void ProgressHours(GameTime hour) {
        
        foreach (var ev in _listEvents) {
            ev.ProgressInTime(hour);
        }
    }

    public void AddCard(BaseEvent ev) {
        _listEvents.Add(ev);
    }

    public void AddStatusTrigger(StatusTriggerData trigger) {
        throw new NotImplementedException();
    }

    #region Render
    public GameObject StatusContainer;
    public GameObject EnergyUI;
    private TextMeshProUGUI _energyText;
    public GameObject MoneyUI;
    private TextMeshProUGUI _moneyText;

    public void UpdateUI() {
        _energyText.text = $"Energy {_status.Energy}";
        _moneyText.text = $"Money {_status.Money}";
    }
    #endregion

    public void ApplyStatusChange(StatusChangeData statusChange) {
        _status.Merge(statusChange);
        UpdateUI();
    }
}
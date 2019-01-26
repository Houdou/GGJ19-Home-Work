using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    public event Action<int, int> OnEnergyChange;
    public event Action<int, int> OnMoneyChange;
    public event Action<GameTime, GameTime> OnGameTimeChange;
    
    private List<BaseEvent> _listEvents;
    private void Awake() {
         _listEvents = new List<BaseEvent>();
         _energyText = EnergyUI.GetComponentInChildren<TextMeshProUGUI>();
         _moneyText = MoneyUI.GetComponentInChildren<TextMeshProUGUI>();
         _timeText = TimeUI.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update() {
    }


    public void Init() {
        _status = new GameStatus();
        
        // Proxy the status event
        _status.OnMoneyChange += (value, diff) => {
            OnMoneyChange?.Invoke(value, diff);
        };
        _status.OnEnergyChange += (value, diff) => {
            OnEnergyChange?.Invoke(value, diff);
        };
        _status.OnGameTimeChange += (value, diff) => {
            OnGameTimeChange?.Invoke(value, diff);
        };
        
    }

    public void LoadStatus(GameStatus status) {
        _status.Replace(status);
    }
    
    public void ProgressTime(GameTime time, bool capToday = false) {
        foreach (var ev in _listEvents) {
            ev.ProgressInTime(time);
        }

        _status.CurrentTime += time;
    }

    public void AddCard(BaseEvent ev) {
        _listEvents.Add(ev);
    }

    #region StatusTrigger
    public void AddStatusTrigger(IntStatusTriggerData trigger) {
        switch (trigger.Field) {
            case StatusFields.Money: {
                var handler = new Action<int, int>((value, diff) => {
                    if (!trigger.Test(value)) return;
                    
                    foreach (var ev in trigger.TriggerEvents) {
                        EventManager.Instance.ProcessEvent(ev);
                    }
                });
                _status.OnMoneyChange += (value, diff) => {
                    handler(value, diff);
                    _status.OnMoneyChange -= handler;
                };
            }
                break;
            case StatusFields.Energy: {
                var handler = new Action<int, int>((value, diff) => {
                    if (!trigger.Test(value)) return;
                    
                    foreach (var ev in trigger.TriggerEvents) {
                        EventManager.Instance.ProcessEvent(ev);
                    }
                });
                _status.OnMoneyChange += (value, diff) => {
                    handler(value, diff);
                    _status.OnMoneyChange -= handler;
                };
            }
                break;
            default:
                Debug.LogWarning($"Wrong data type Int to Field {nameof(trigger.Field)}");
                break;
        }
    }

    public void AddGameTimeStatusTrigger(GameTimeStatusTriggerData trigger) {
        switch (trigger.Field) {
            case StatusFields.GameTime:
                var handler = new Action<GameTime, GameTime>((value, diff) => {
                    if (!trigger.Test(value)) return;
                    
                    foreach (var ev in trigger.TriggerEvents) {
                        EventManager.Instance.ProcessEvent(ev);
                    }
                });
                _status.OnGameTimeChange += (value, diff) => {
                    handler(value, diff);
                    _status.OnGameTimeChange -= handler;
                };
                break;
            default:
                Debug.LogWarning($"Wrong data type GameTime to Field {nameof(trigger.Field)}");
                break;
        }
    }
    #endregion

    #region Render
    public GameObject StatusContainer;
    public GameObject EnergyUI;
    private TextMeshProUGUI _energyText;
    public GameObject MoneyUI;
    private TextMeshProUGUI _moneyText;
    public GameObject TimeUI;
    private TextMeshProUGUI _timeText;

    public void UpdateUI() {
        _energyText.text = $"Energy {_status.Energy}";
        _moneyText.text = $"Money {_status.Money}";
        _timeText.text = _status.CurrentTime.ToString();
    }
    #endregion

    public void ApplyStatusChange(StatusChangeData statusChange) {
        _status.Merge(statusChange);
        if (statusChange.Time != GameTime.zero) {
            ProgressTime(statusChange.Time, true);
        }
        
        UpdateUI();
    }
}
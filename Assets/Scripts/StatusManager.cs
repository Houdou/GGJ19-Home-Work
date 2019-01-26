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

    public GameTime CurrentTime => _status.CurrentTime;
    public bool Busy;

    public event Action<LocationType, LocationType> OnLocationChange;
    public event Action<GameTime, GameTime> OnGameTimeChange;
    public event Action<int, int> OnMoneyChange;
    public event Action<int, int> OnEnergyChange;
    public event Action<int, int> OnPersonalHappinessChange;
    public event Action<int, int> OnFamilyHappinessChange;
    public event Action<int, int> OnCareerChange;
    public event Action<int, int> OnProjectProgressChange;

    private void Awake() {
        _energyText = EnergyUI.GetComponentInChildren<TextMeshProUGUI>();
        _moneyText = MoneyUI.GetComponentInChildren<TextMeshProUGUI>();
        _timeText = TimeUI.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update() { }


    public void Init() {
        _status = new GameStatus();

        // Proxy the status event
        _status.OnGameTimeChange += (value, diff) => { OnGameTimeChange?.Invoke(value, diff); };
        _status.OnLocationChange += (value, prev) => { OnLocationChange?.Invoke(value, prev); };
        _status.OnCareerChange += (value, diff) => { OnCareerChange?.Invoke(value, diff); };
        _status.OnMoneyChange += (value, diff) => { OnMoneyChange?.Invoke(value, diff); };
        _status.OnEnergyChange += (value, diff) => { OnEnergyChange?.Invoke(value, diff); };
        _status.OnPersonalHappinessChange += (value, diff) => { OnPersonalHappinessChange?.Invoke(value, diff); };
        _status.OnFamilyHappinessChange += (value, diff) => { OnFamilyHappinessChange?.Invoke(value, diff); };
        _status.OnProjectProgressChange += (value, diff) => { OnProjectProgressChange?.Invoke(value, diff); };

    }

    public void LoadStatus(GameStatus status) {
        _status.Replace(status);
    }

    public void ProgressTime(GameTime time) {
        _status.CurrentTime += time;
    }

    #region StatusTrigger

    public void AddIntStatusTrigger(IntStatusTriggerData trigger) {
        Debug.Log($"Add {trigger}");
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
                    if (trigger.Repeat) return;
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
                _status.OnEnergyChange += (value, diff) => {
                    handler(value, diff);
                    if (trigger.Repeat) return;
                    _status.OnEnergyChange -= handler;
                };
            }
                break;
            case StatusFields.PersonalHappiness: {
                var handler = new Action<int, int>((value, diff) => {
                    if (!trigger.Test(value)) return;

                    foreach (var ev in trigger.TriggerEvents) {
                        EventManager.Instance.ProcessEvent(ev);
                    }
                });
                _status.OnPersonalHappinessChange += (value, diff) => {
                    handler(value, diff);
                    if (trigger.Repeat) return;
                    _status.OnPersonalHappinessChange -= handler;
                };
            }
                break;
            case StatusFields.FamilyHappiness: {
                var handler = new Action<int, int>((value, diff) => {
                    if (!trigger.Test(value)) return;

                    foreach (var ev in trigger.TriggerEvents) {
                        EventManager.Instance.ProcessEvent(ev);
                    }
                });
                _status.OnFamilyHappinessChange += (value, diff) => {
                    handler(value, diff);
                    if (trigger.Repeat) return;
                    _status.OnFamilyHappinessChange -= handler;
                };
            }
                break;
            case StatusFields.Career: {
                var handler = new Action<int, int>((value, diff) => {
                    if (!trigger.Test(value)) return;

                    foreach (var ev in trigger.TriggerEvents) {
                        EventManager.Instance.ProcessEvent(ev);
                    }
                });
                _status.OnCareerChange += (value, diff) => {
                    handler(value, diff);
                    if (trigger.Repeat) return;
                    _status.OnCareerChange -= handler;
                };
            }
                break;
            case StatusFields.ProjectProgress: {
                var handler = new Action<int, int>((value, diff) => {
                    if (!trigger.Test(value)) return;

                    foreach (var ev in trigger.TriggerEvents) {
                        EventManager.Instance.ProcessEvent(ev);
                    }
                });
                _status.OnProjectProgressChange += (value, diff) => {
                    handler(value, diff);
                    if (trigger.Repeat) return;
                    _status.OnProjectProgressChange -= handler;
                };
            }
                break;
        }
    }

    public void AddGameTimeStatusTrigger(GameTimeStatusTriggerData trigger) {
        if (trigger.Field != StatusFields.GameTime) return;

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
    }

    public void AddLocationStatusTrigger(LocationStatusTriggerData trigger) {
        if (trigger.Field != StatusFields.Location) return;

        var handler = new Action<LocationType, LocationType>((value, diff) => {
            if (!trigger.Test(value)) return;

            foreach (var ev in trigger.TriggerEvents) {
                EventManager.Instance.ProcessEvent(ev);
            }
        });
        _status.OnLocationChange += (value, diff) => {
            handler(value, diff);
            _status.OnLocationChange -= handler;
        };
    }

    public void ApplyStatusChange(StatusChangeData changes) {
        Debug.Log($"Applied {changes}");
        _status.Merge(changes);

        // TODO: Check date limit
        if (changes.OverrideTime) {
            _status.CurrentTime = changes.Time;
        } else if (changes.Time != GameTime.zero) {
            EventManager.Instance.ProgressTime(changes.Time);
        }


        UpdateUI();
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

}
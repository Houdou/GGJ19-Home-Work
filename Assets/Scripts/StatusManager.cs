using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public ViewController View;

    public GameTime CurrentTime => _status.CurrentTime;
    public GameTime RemainingTimeInDay => _status.RemainingHourToday;
    public LocationType Location => _status.Location;
    public bool Busy;

    public int HomeLevel {
        get {
            if (_status.Money < 2000) {
                return 1;
            }

            if (_status.Money < 5000) {
                return 2;
            }

            return 3;
        }
    }

    public int OfficeLevel {
        get {
            if (_status.Career < 20) {
                return 1;
            }

            if (_status.Career < 60) {
                return 2;
            }

            return 3;
        }
    }

    // TODO: Not this one
    public bool Sick => _status.Energy < 20;

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
        _personalText = PersonalHappinessUI.GetComponentInChildren<TextMeshProUGUI>();
        _familyText = FamilyHappinessUI.GetComponentInChildren<TextMeshProUGUI>();
        _careerText = CareerUI.GetComponentInChildren<TextMeshProUGUI>();
        _dayText = DayUI.GetComponentInChildren<TextMeshProUGUI>();
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

        View.Init();
        View.UpdateUI();
        UpdateUI();
    }

    public void LoadStatus(GameStatus status) {
        _status.Replace(status);
        UpdateUI();
    }

    public void ProgressTime(GameTime time) {
        _status.CurrentTime += time;
        UpdateUI();
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

                    trigger.Inactive = true;
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
                    
                    trigger.Inactive = true;
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
                    
                    trigger.Inactive = true;
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
                    
                    trigger.Inactive = true;
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
                    
                    trigger.Inactive = true;
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
                    
                    trigger.Inactive = true;
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
            if (trigger.Inactive || !trigger.Test(value)) return;

            foreach (var ev in trigger.TriggerEvents) {
                EventManager.Instance.ProcessEvent(ev);
            }
            
            if (trigger.Repeat) {
                var newTrigger = ScriptableObject.CreateInstance<GameTimeStatusTriggerData>();
                newTrigger.Name = trigger.Name;
                newTrigger.name = trigger.name;
                newTrigger.IsInnate = trigger.IsInnate;
                newTrigger.Repeat = trigger.Repeat;
                newTrigger.Field = trigger.Field;
                newTrigger.ConditionOperator = trigger.ConditionOperator;
                newTrigger.DeltaTime = trigger.DeltaTime;
                newTrigger.TriggerEvents = trigger.TriggerEvents;

                newTrigger.TargetGameTime = trigger.TargetGameTime + trigger.DeltaTime;

                AddGameTimeStatusTrigger(newTrigger);
                GameMaster.Instance.DictGameTimeStatusTriggerData[trigger.name] = newTrigger;
            }
            trigger.Inactive = true;

        });
        _status.OnGameTimeChange += handler;
    }

    public void AddLocationStatusTrigger(LocationStatusTriggerData trigger) {
        if (trigger.Field != StatusFields.Location) return;

        var handler = new Action<LocationType, LocationType>((value, diff) => {
            if (!trigger.Test(value)) return;

            foreach (var ev in trigger.TriggerEvents) {
                EventManager.Instance.ProcessEvent(ev);
            }

            trigger.Inactive = true;
        });
        _status.OnLocationChange += handler;
    }

    public void ApplyStatusChange(StatusChangeData changes) {
        Debug.Log($"Applied {changes}");
        _status.Merge(changes);
        
        // TODO: Check date limit
        if (changes.OverrideTime) {
            _status.CurrentTime = changes.Time;
        } else if (changes.Time != GameTime.zero) {
            EventManager.Instance.ProgressTime(changes.Time);
            _status.CurrentTime += changes.Time;
        }

        UpdateUI();
    }
    
    public void Tick() {
        _status.Tick();
    }

    #endregion

    #region Render

    public GameObject StatusContainer;
    public GameObject EnergyUI;
    private TextMeshProUGUI _energyText;
    public GameObject MoneyUI;
    private TextMeshProUGUI _moneyText;
    public GameObject PersonalHappinessUI;
    private TextMeshProUGUI _personalText;
    public GameObject FamilyHappinessUI;
    private TextMeshProUGUI _familyText;
    public GameObject CareerUI;
    private TextMeshProUGUI _careerText;
    public GameObject DayUI;
    private TextMeshProUGUI _dayText;
    public TimeBarController TimeBar;

    public void UpdateUI() {
        _energyText.text = $"{_status.Energy}%";
        _moneyText.text = $"{_status.Money}HKD";
        _personalText.text = $"{_status.PersonalHappiness}%";
        _familyText.text = $"{_status.FamilyHappiness}%";
        _careerText.text = $"{_status.Career}%";
        _dayText.text = $"Day: {_status.CurrentTime.Day / 2 + 1}";
        TimeBar.TargetAmount =
            _status.CurrentTime.TotalHourInGame % (2f * Config.HoursInDay) / (2f * Config.HoursInDay);
        
        View.UpdateUI();
    }

    #endregion

}
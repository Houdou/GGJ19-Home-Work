using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    #region Singleton

    private static GameMaster instance;

    private static object _lock = new object();

    public static GameMaster Instance {
        get {
            if (applicationIsQuitting) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(GameMaster) +
                                 "' already destroyed on application quit." +
                                 " Won't create again - returning null.");
                return null;
            }

            lock (_lock) {
                if (instance == null) {
                    instance = (GameMaster) FindObjectOfType(typeof(GameMaster));

                    if (FindObjectsOfType(typeof(GameMaster)).Length > 1) {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       " - there should never be more than 1 singleton!" +
                                       " Reopening the scene might fix it.");
                        return instance;
                    }

                    if (instance == null) {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<GameMaster>();
                        singleton.name = "(singleton) " + typeof(GameMaster);

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(GameMaster) +
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

    private EventManager _eventManager;
    private StatusManager _statusManager;
    
    public bool IsGameOver;

    private void Awake() {
        _eventManager = GetComponent<EventManager>();
        _statusManager = GetComponent<StatusManager>();
    }

    void Start() {
        LoadData();
        SetupStatusManager();
        IsGameOver = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            //_eventManager.CreateTodo(DictTodoCardData["BlackFriday"]);
            _eventManager.CreateCard(DictActionCardData["Project"]);
        }

        if (Input.GetKeyDown(KeyCode.Y)) {
            _eventManager.CreateTodo(DictTodoCardData["HomeIssue"]);
            //_eventManager.CreateCard(DictActionCardData["Task"]);
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            _eventManager.ProgressTime(GameTime.oneHour);
        }
    }

    #region Init

    public Dictionary<string, ActionCardData> DictActionCardData;
    public Dictionary<string, TodoCardData> DictTodoCardData;
    public Dictionary<string, GenerateCardsData> DictGenerateCardsData;
    public Dictionary<string, GenerateDelayedCardsData> DictGenerateDelayedCardsData;
    public Dictionary<string, IntStatusTriggerData> DictIntStatusTriggerData;
    public Dictionary<string, LocationStatusTriggerData> DictLocationStatusTriggerData;
    public Dictionary<string, GameTimeStatusTriggerData> DictGameTimeStatusTriggerData;
    
    public Dictionary<string, Sprite> DictSpriteResources;

    private void LoadData() {
        var actionCardData = Resources.LoadAll("Data/ActionCards", typeof(ActionCardData))
            .Cast<ActionCardData>().ToArray();

        DictActionCardData = new Dictionary<string, ActionCardData>();
        foreach (var card in actionCardData) {
            DictActionCardData.Add(card.Name, card);
        }

        var todoCardData = Resources.LoadAll("Data/TodoCards", typeof(TodoCardData))
            .Cast<TodoCardData>().ToArray();

        DictTodoCardData = new Dictionary<string, TodoCardData>();
        foreach (var card in todoCardData) {
            DictTodoCardData.Add(card.Name, card);
        }

        var generateCardsData = Resources.LoadAll("Data/GenerateDelayedCards", typeof(GenerateCardsData))
            .Cast<GenerateCardsData>().ToArray();

        DictGenerateCardsData = new Dictionary<string, GenerateCardsData>();
        foreach (var card in generateCardsData) {
            DictGenerateCardsData.Add(card.Name, card);
        }
        
        var generateDelayedCardsData = Resources.LoadAll("Data/GenerateDelayedCards", typeof(GenerateDelayedCardsData))
            .Cast<GenerateDelayedCardsData>().ToArray();

        DictGenerateDelayedCardsData = new Dictionary<string, GenerateDelayedCardsData>();
        foreach (var card in generateDelayedCardsData) {
            DictGenerateDelayedCardsData.Add(card.Name, card);
        }

        var intStatusTriggerData = Resources.LoadAll("Data/StatusTriggers", typeof(IntStatusTriggerData))
            .Cast<IntStatusTriggerData>().ToArray();

        DictIntStatusTriggerData = new Dictionary<string, IntStatusTriggerData>();
        foreach (var trigger in intStatusTriggerData) {
            DictIntStatusTriggerData.Add(trigger.Name, trigger);
        }
        
        var locationStatusTriggerData = Resources.LoadAll("Data/StatusTriggers", typeof(LocationStatusTriggerData))
            .Cast<LocationStatusTriggerData>().ToArray();

        DictLocationStatusTriggerData = new Dictionary<string, LocationStatusTriggerData>();
        foreach (var trigger in locationStatusTriggerData) {
            DictLocationStatusTriggerData.Add(trigger.Name, trigger);
        }

        var gameTimeStatusTriggerData = Resources.LoadAll("Data/StatusTriggers", typeof(GameTimeStatusTriggerData))
            .Cast<GameTimeStatusTriggerData>().ToArray();

        DictGameTimeStatusTriggerData = new Dictionary<string, GameTimeStatusTriggerData>();
        foreach (var trigger in gameTimeStatusTriggerData) {
            DictGameTimeStatusTriggerData.Add(trigger.Name, trigger);
        }

        var spritesResources = Resources.LoadAll("Sprites", typeof(Sprite)).Cast<Sprite>().ToArray();

        DictSpriteResources = new Dictionary<string, Sprite>();
        foreach (var sprite in spritesResources) {
            DictSpriteResources.Add(sprite.name, sprite);
        }
    }

    public Sprite GetSpriteResources(string id) {
        if (!DictSpriteResources.ContainsKey(id)) {
            Debug.LogError($"{id} sprite is not presented");
            return null;
        }

        return DictSpriteResources[id];
    }

    private void SetupStatusManager() {
        _statusManager.Init();

        _statusManager.OnGameTimeChange += (value, diff) => {
            var realPauseTime = diff.TotalHourInGame * Config.HoursInRealSecond;
            Invoke(nameof(ResetBusyStatus), realPauseTime);
        };

        foreach (var trigger in DictIntStatusTriggerData.Values) {
            if (trigger.IsInnate) {
                switch (trigger.Field) {
                    case StatusFields.Money:
                    case StatusFields.Energy:
                    case StatusFields.PersonalHappiness:
                    case StatusFields.FamilyHappiness:
                    case StatusFields.Career:
                    case StatusFields.ProjectProgress:
                        _statusManager.AddIntStatusTrigger(trigger);
                        break;
                }
            }
        }

        foreach (var trigger in DictGameTimeStatusTriggerData.Values) {
            if (trigger.IsInnate && trigger.Field == StatusFields.GameTime) {                
                _statusManager.AddGameTimeStatusTrigger(trigger);
            }
        }

        foreach (var trigger in DictLocationStatusTriggerData.Values) {
            if (trigger.IsInnate && trigger.Field == StatusFields.Location) {
                _statusManager.AddLocationStatusTrigger(trigger);
            }
        }
    }

    #endregion

    private void ResetBusyStatus() {
        _statusManager.Busy = false;
    }

    public void EndGame(GameEnding ending) {
        IsGameOver = true;
        Debug.Log($"Game End : {ending.GetDescription()}");
    }
}
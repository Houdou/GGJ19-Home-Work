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
    private void Awake() {
        _eventManager = GetComponent<EventManager>();
        _statusManager = GetComponent<StatusManager>();
    }

    public Dictionary<string, ActionCardData> DictActionCardData;
    public Dictionary<string, IntStatusTriggerData> DictIntStatusTriggerData;
    public Dictionary<string, FloatStatusTriggerData> DictFloatStatusTriggerData;
    public Dictionary<string, GameTimeStatusTriggerData> DictGameTimeStatusTriggerData;

    void Start() {
        var actionCardData = Resources.LoadAll("Data/ActionCards", typeof(ActionCardData))
            .Cast<ActionCardData>().ToArray();

        DictActionCardData = new Dictionary<string, ActionCardData>();
        foreach (var card in actionCardData) {
            DictActionCardData.Add(card.Name, card);
        }

        var intStatusTriggerData = Resources.LoadAll("Data/StatusTriggers", typeof(IntStatusTriggerData))
            .Cast<IntStatusTriggerData>().ToArray();

        DictIntStatusTriggerData = new Dictionary<string, IntStatusTriggerData>();
        foreach (var trigger in intStatusTriggerData) {
            DictIntStatusTriggerData.Add(trigger.Name, trigger);
        }

        var floatStatusTriggerData = Resources.LoadAll("Data/StatusTriggers", typeof(FloatStatusTriggerData))
            .Cast<FloatStatusTriggerData>().ToArray();

        DictFloatStatusTriggerData = new Dictionary<string, FloatStatusTriggerData>();
        foreach (var trigger in floatStatusTriggerData) {
            DictFloatStatusTriggerData.Add(trigger.Name, trigger);
        }

        var gameTimeStatusTriggerData = Resources.LoadAll("Data/StatusTriggers", typeof(GameTimeStatusTriggerData))
            .Cast<GameTimeStatusTriggerData>().ToArray();

        DictGameTimeStatusTriggerData = new Dictionary<string, GameTimeStatusTriggerData>();
        foreach (var trigger in gameTimeStatusTriggerData) {
            DictGameTimeStatusTriggerData.Add(trigger.Name, trigger);
        }

        _statusManager.Init();
        
        _statusManager.AddGameTimeStatusTrigger(DictGameTimeStatusTriggerData["GameOverByTimeTrigger"]);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            _eventManager.CreateCard(DictActionCardData["HouseCleaning"]);
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            _eventManager.CreateCard(DictActionCardData["Work"]);
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            _statusManager.ProgressTime(GameTime.oneHour);
        }
    }

    public void EndGame(GameEnding ending) {
        Debug.Log($"Game End : {ending.GetDescription()}");
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour {
    #region Singleton

    private static EventManager instance;

    private static object _lock = new object();

    public static EventManager Instance {
        get {
            if (applicationIsQuitting) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(EventManager) +
                                 "' already destroyed on application quit." +
                                 " Won't create again - returning null.");
                return null;
            }

            lock (_lock) {
                if (instance == null) {
                    instance = (EventManager) FindObjectOfType(typeof(EventManager));

                    if (FindObjectsOfType(typeof(EventManager)).Length > 1) {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       " - there should never be more than 1 singleton!" +
                                       " Reopening the scene might fix it.");
                        return instance;
                    }

                    if (instance == null) {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<EventManager>();
                        singleton.name = "(singleton) " + typeof(EventManager);

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(EventManager) +
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

    public GameObject CardPrefab;
    public GameObject TodoPrefab;
    public GameObject AlertPrefab;
    public GameObject LeftPanel;
    public GameObject RightPanel;

    private Dictionary<string, Transform> _dicRefPos;
    private Dictionary<LocationType, string> _dicLocationPanel;


    private static List<Todo> _listTodo = new List<Todo>();
    private static List<CardController> _listActionCards = new List<CardController>();
    private static List<CardController> _listTodoCards = new List<CardController>();
    public event Action<CardController> OnCardCreated;
    public event Action<CardController> OnCardDestroyed;

    private void Awake() {
        _dicRefPos = new Dictionary<string, Transform> {
            {"leftCardRefPos", LeftPanel.transform.Find("CardRefPos")},
            {"leftCardCenterPos", LeftPanel.transform.Find("CardCenterPos")},
            {"leftCardContainer", LeftPanel.transform.Find("CardContainer")},
            {"rightCardRefPos", RightPanel.transform.Find("CardRefPos")},
            {"rightCardCenterPos", RightPanel.transform.Find("CardCenterPos")},
            {"rightCardContainer", RightPanel.transform.Find("CardContainer")},
        };

        _dicLocationPanel = new Dictionary<LocationType, string> {
            {LocationType.Home, "left"},
            {LocationType.Office, "right"},
            {LocationType.Other, "right"},
        };
    }

    private void Update() { }

    [NonSerialized]
    public bool Operatable = true;

    public void ProcessEvent(EventData ev) {
        Debug.Log($"Processing {ev}");
        // TODO: Special handling for some events


        foreach (var statusChange in ev.StatusChanges) {
            StatusManager.Instance.ApplyStatusChange(statusChange);
        }

        foreach (var generateData in ev.CardsGenerations) {
            GenerateCard(generateData);
        }

        foreach (var generateData in ev.DelayedCardsGenerations) {
            var timerTrigger = ScriptableObject.CreateInstance<GameTimeStatusTriggerData>();
            timerTrigger.Field = StatusFields.GameTime;
            timerTrigger.TargetGameTime = StatusManager.Instance.CurrentTime + generateData.Delay;
            timerTrigger.ConditionOperator = ConditionOperator.GreaterOrEqual;
            timerTrigger.Repeat = generateData.Repeat;
            timerTrigger.DeltaTime = generateData.DeltaTime;
            timerTrigger.Name = $"{nameof(GenerateDelayedCardsData)}From{generateData.Name}";

            var generateEvent = ScriptableObject.CreateInstance<EventData>();
            generateEvent.Name = $"{timerTrigger}Event";
            generateEvent.CardsGenerations = new[] {generateData.GenerateData};

            timerTrigger.TriggerEvents = new[] {generateEvent};

            GameMaster.Instance.DictGameTimeStatusTriggerData.Add(timerTrigger.Name, timerTrigger);
            StatusManager.Instance.AddGameTimeStatusTrigger(timerTrigger);
        }

        switch (ev.Ending) {
            // TODO : End game
            case GameEnding.GameOver:
            case GameEnding.HappyEnd:
                GameMaster.Instance.EndGame(ev.Ending);
                break;
        }
    }


    public void GenerateCard(GenerateCardsData data) {
        foreach (var actionCard in data.ActionCardsToGenerate) {
            CreateCard(actionCard, data.IsEmergency);
        }

        foreach (var todoCardData in data.TodoCardsToGenerate) {
            CreateTodo(todoCardData);
        }
    }

    public void CreateCard(ActionCardData data, bool isEmergency = false) {
        if (data.Location == LocationType.Null) {
            Debug.LogWarning($"Wrong {nameof(LocationType)} passed to {nameof(CreateCard)}");
            return;
        }

        var cardController = DrawCard(data, isEmergency);

        cardController.OnClick += () => {
            if (!Operatable) return;

            if (data.Cost) {
                StatusManager.Instance.ApplyStatusChange(data.Cost);
            }

            // TODO: Remove cards
            OnCardDestroyed?.Invoke(cardController);
        };
    }

    public void CreateTodo(TodoCardData data) {
        if (data.Location == LocationType.Null) {
            Debug.LogWarning($"Wrong {nameof(LocationType)} passed to {nameof(CreateCard)}");
            return;
        }

        var todo = new Todo(data.IsExpirable, data.IsInternal, data.ExpiryTime + StatusManager.Instance.CurrentTime);
        _listTodo.Add(todo);

        var cardController = DrawTodo(data);
        cardController.OnClick += () => {
            if (!Operatable) return;

            if (data.Cost) {
                StatusManager.Instance.ApplyStatusChange(data.Cost);
            }

            foreach (var ev in data.FulFillEvent) {
                ProcessEvent(ev);
            }

            if (_listTodo.Contains(todo)) {
                _listTodo.Remove(todo);
            }
        };
    }


    #region Render

    private CardController DrawCard(ActionCardData data, bool isEmergency) {
        var panel = _dicLocationPanel[data.Location];
        var thoughtPos = _dicRefPos[$"{panel}CardCenterPos"].position;
        var parentTransform = _dicRefPos[$"{panel}CardContainer"];

        var cardIndex = _listActionCards.Count;

        thoughtPos += new Vector3(5 + UnityEngine.Random.Range(0, 15), 100 * cardIndex, 0);

        var newCard = Instantiate(CardPrefab, thoughtPos, Quaternion.identity, parentTransform);
        var cardController = newCard.GetComponent<CardController>();
        cardController.PinPos = thoughtPos;

        return cardController;
    }

    private CardController DrawTodo(TodoCardData data) {
        var panel = _dicLocationPanel[data.Location];
        var thoughtPos = _dicRefPos[$"{panel}CardCenterPos"].position;
        var parentTransform = _dicRefPos[$"{panel}CardContainer"];

        var cardIndex = _listTodoCards.Count;

        thoughtPos += new Vector3(5 + UnityEngine.Random.Range(0, 15), 100 * cardIndex, 0);

        var newCard = Instantiate(TodoPrefab, thoughtPos, Quaternion.identity, parentTransform);
        var cardController = newCard.GetComponent<CardController>();
        cardController.PinPos = thoughtPos;

        return cardController;
    }

    public void CleanAllCard() {
        foreach (var card in _listActionCards) {
            OnCardDestroyed?.Invoke(card);
            Destroy(card.gameObject);
        }

        foreach (var todo in _listTodoCards) {
            OnCardDestroyed?.Invoke(todo);
            Destroy(todo.gameObject);
        }
    }

    #endregion

    public void ProgressTime(GameTime time) {
        foreach (var ev in _listTodo) {
            ev.ProgressInTime(time);
        }

        StatusManager.Instance.ProgressTime(time);
    }
}
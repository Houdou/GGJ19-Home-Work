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
            {"leftTodoRefPos", LeftPanel.transform.Find("TodoRefPos")},
            {"leftCardContainer", LeftPanel.transform.Find("CardContainer")},
            {"rightCardRefPos", RightPanel.transform.Find("CardRefPos")},
            {"rightTodoRefPos", RightPanel.transform.Find("TodoRefPos")},
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
        if (ev.Name == "RestEvent" || ev.Name == "NapEvent") {
            var time = StatusManager.Instance.RemainingTimeInDay;
            
            foreach (var changes in ev.StatusChanges) {
                for (int i = 0; i < time.TotalHourInGame; i++) {
                    StatusManager.Instance.ApplyStatusChange(changes);
                }
            }
            
            ProgressTime(time);
            return;
        }

        foreach (var deleteData in ev.CardsDeletions) {
            DeleteCards(deleteData);
            _listActionCards.Clear();
        }
        
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
        Debug.Log($"Create cards at {data.name}");
        
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
            if (!Operatable || GameMaster.Instance.IsGameOver) return;

            if (data.Cost) {
                StatusManager.Instance.ApplyStatusChange(data.Cost);
            }

            foreach (var ev in data.TriggerEvents) {
                ProcessEvent(ev);
            }
        };
    }

    public void CreateTodo(TodoCardData data) {
        if (data.Location == LocationType.Null) {
            Debug.LogWarning($"Wrong {nameof(LocationType)} passed to {nameof(CreateCard)}");
            return;
        }

        var todo = new Todo(data.IsExpirable, data.IsInternal, data.ExpiryTime + StatusManager.Instance.CurrentTime);
        _listTodo.Add(todo);

        todo.OnExpire += () => {
            foreach (var ev in data.FailedEvent) {
                ProcessEvent(ev);
            }
            
            _listTodo.Remove(todo);
        };

        var cardController = DrawTodo(data);
        cardController.OnClick += () => {
            if (!Operatable || GameMaster.Instance.IsGameOver) return;

            if (data.Cost) {
                StatusManager.Instance.ApplyStatusChange(data.Cost);
            }

            foreach (var ev in data.FulFillEvent) {
                ProcessEvent(ev);
            }

            _listTodo.Remove(todo);
        };
    }


    #region Render

    private CardController DrawCard(ActionCardData data, bool isEmergency) {
        var panel = _dicLocationPanel[data.Location];
        var pos = _dicRefPos[$"{panel}CardRefPos"].position;
        var parentTransform = _dicRefPos[$"{panel}CardContainer"];
        var cardIndex = _listActionCards.Count;

        pos += new Vector3(0f, -Config.CardVerticalStep * cardIndex, 0f);

        var newCard = Instantiate(CardPrefab, pos, Quaternion.identity, parentTransform);
        var cardController = newCard.GetComponent<CardController>();
        cardController.DetailOffset = new Vector2(0f, 100f);
        cardController.name = data.name;
        cardController.PinPos = pos;
        cardController.GetComponentInChildren<TextMeshProUGUI>().text = data.Name;
        _listActionCards.Add(cardController);

        return cardController;
    }

    private CardController DrawTodo(TodoCardData data) {
        var panel = _dicLocationPanel[data.Location];
        var pos = _dicRefPos[$"{panel}TodoRefPos"].position;
        var parentTransform = _dicRefPos[$"{panel}CardContainer"];
        var cardIndex = _listTodoCards.Count;

        pos += new Vector3(0f, -Config.CardVerticalStep * cardIndex, 0f);

        var newCard = Instantiate(TodoPrefab, pos, Quaternion.identity, parentTransform);
        var cardController = newCard.GetComponent<CardController>();
        cardController.name = data.name;
        cardController.PinPos = pos;
        cardController.GetComponentInChildren<TextMeshProUGUI>().text = data.Name;
        _listTodoCards.Add(cardController);

        return cardController;
    }

    public void ClearAllCards() {
        ClearActionCards();
        ClearTodoCards();
    }


    public void DeleteCards(GenerateCardsData data) {
        foreach (var card in data.ActionCardsToGenerate) {
            DeleteCard(card);
        }
//        foreach (var card in data.ActionCardsToGenerate) {
//            DeleteCard(card);
//        }
    }
    public void DeleteCard(ActionCardData data) {
        CardController toRemove = null;
        foreach (var card in _listActionCards) {
            if (card.name == data.name) {
                toRemove = card;
            }
        }
        
        Debug.Log($"To remove : {toRemove}");

        if (!toRemove) return;
        OnCardDestroyed?.Invoke(toRemove);
        _listActionCards.Remove(toRemove);
        Destroy(toRemove.gameObject);
    }

    public void ClearActionCards() {
        foreach (var card in _listActionCards) {
            OnCardDestroyed?.Invoke(card);
            Destroy(card.gameObject);
        }
        _listActionCards.Clear();
    }

    public void ClearTodoCards() {
        foreach (var todo in _listTodoCards) {
            OnCardDestroyed?.Invoke(todo);
            Destroy(todo.gameObject);
        }
        _listTodoCards.Clear();
    }

    #endregion

    public void ProgressTime(GameTime time) {
        foreach (var ev in _listTodo) {
            ev.ProgressInTime(time);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject ThoughtPrefab;
    public GameObject LeftPanel;
    public GameObject RightPanel;

    private Dictionary<string, Transform> _dicRefPos;
    private Dictionary<LocationType, string> _dicLocationPanel;

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

    private void Update() {
        
    }

    [NonSerialized]
    public bool Operatable = true;

    public void ProcessEvent(EventData ev) {
        foreach (var statusChange in ev.StatusChanges) {
            StatusManager.Instance.ApplyStatusChange(statusChange);
        }

        switch (ev.Ending) {
            // TODO : End game
            case GameEnding.GameOver:
            case GameEnding.HappyEnd:
                GameMaster.Instance.EndGame(ev.Ending);
                break;
        }
    }

    public void GenerateNewGameTimeTrigger(CardType type, string cardName, GameTime time) {
        switch (type) {
            case CardType.Action: {
                if (!GameMaster.Instance.DictActionCardData.ContainsKey(cardName)) {
                Debug.LogWarning($"{name} {type.GetDescription()}Card does not exist");
                    return;
                }
                var trigger = ScriptableObject.CreateInstance<GenerateActionCardsData>();
                trigger.Name = $"Generate{cardName}Trigger";
                trigger.ActionCardsToGenerate = new[] {cardName};
            }
                break;
            case CardType.Todo: {
                if (!GameMaster.Instance.DictTodoCardData.ContainsKey(cardName)) {
                Debug.LogWarning($"{name} {type.GetDescription()}Card does not exist");
                    return;
                }
                var trigger = ScriptableObject.CreateInstance<GenerateTodoCardsData>();
                trigger.Name = $"Generate{cardName}Trigger";
                trigger.TodoCardsToGenerate = new[] {cardName};
            }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    

    #region Render
    public void CreateCard(ActionCardData card) {
        if (card.Location == LocationType.Null) {
            Debug.LogWarning($"Wrong {nameof(LocationType)} passed to {nameof(CreateCard)}");
            return;
        }

        var panel = _dicLocationPanel[card.Location];
        // TODO: Use group manager to update pos
        var thoughtPos = _dicRefPos[$"{panel}CardCenterPos"].position;
        var parentTransform = _dicRefPos[$"{panel}CardContainer"]; 

        var newCard = Instantiate(ThoughtPrefab, thoughtPos, Quaternion.identity, parentTransform);
        var cardController = newCard.GetComponent<CardController>();

        cardController.RefPos = _dicRefPos[$"{panel}CardRefPos"];
        cardController.CenterPos = _dicRefPos[$"{panel}CardCenterPos"];
        cardController.PinPos = thoughtPos;
        
        cardController.OnClick += () => {
            if (!Operatable) return;
            
            if (card.Cost) {
                StatusManager.Instance.ApplyStatusChange(card.Cost);
            }

            // TODO: Remove cards
            OnCardDestroyed?.Invoke(cardController);
        };

        OnCardCreated?.Invoke(cardController);
    }
    #endregion
}
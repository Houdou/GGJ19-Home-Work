using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
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

    public GameObject ThoughtPrefab;
    public GameObject LeftPanel;
    public GameObject RightPanel;

    private Dictionary<string, Transform> _dicRefPos;
    private Dictionary<LocationType, string> _dicLocationPanel;

    private void Awake() {
        _dicRefPos = new Dictionary<string, Transform> {
            {"leftThoughtsRefPos", LeftPanel.transform.Find("ThoughtsRefPos")},
            {"leftThoughtsCenterPos", LeftPanel.transform.Find("ThoughtsCenterPos")},
            {"leftThoughtsContainer", LeftPanel.transform.Find("ThoughtsContainer")},
            {"rightThoughtsRefPos", RightPanel.transform.Find("ThoughtsRefPos")},
            {"rightThoughtsCenterPos", RightPanel.transform.Find("ThoughtsCenterPos")},
            {"rightThoughtsContainer", RightPanel.transform.Find("ThoughtsContainer")},
        };
        
        _dicLocationPanel = new Dictionary<LocationType, string> {
            {LocationType.Home, "left"},
            {LocationType.Office, "right"},
            {LocationType.Other, "right"},
        };
    }

    private void Update() {
        
    }

    public void CreateAction(LocationType location) {
        if (location == LocationType.Null) {
            Debug.LogWarning($"Wrong {nameof(LocationType)} passed to {nameof(CreateAction)}");
            return;
        }

        var panel = _dicLocationPanel[location];
        var thoughtPos = _dicRefPos[$"{panel}ThoughtsCenterPos"].position;
        var parentTransform = _dicRefPos[$"{panel}ThoughtsContainer"]; 

        var newCard = Instantiate(ThoughtPrefab, thoughtPos, Quaternion.identity, parentTransform);
        var cardController = newCard.GetComponent<CardController>();

        cardController.RefPos = _dicRefPos[$"{panel}ThoughtsRefPos"];
        cardController.CenterPos = _dicRefPos[$"{panel}ThoughtsCenterPos"];
        cardController.OnClick += () => { Debug.Log($"Clicked!!! {cardController.RefPos.position}"); };
    }
}
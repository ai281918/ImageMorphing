using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class FeatureManager : MonoBehaviour
{
    static FeatureManager _instance;
    public static FeatureManager instance{
        get{
            if (_instance == null){
                _instance = FindObjectOfType(typeof(FeatureManager)) as FeatureManager;
                if (_instance == null){
                    GameObject go = new GameObject("FeatureManager");
                    _instance = go.AddComponent<FeatureManager>();
                }
            }
            return _instance;
        }
    }

    UnityEvent onFeatureNumberChangedEvents = new UnityEvent(), onCurrentIdChangedEvents = new UnityEvent();
    public Text currentIdText;
    int _featureCount = 10;
    public int featureCount{
        get{
            return _featureCount;
        }
        set{
            _featureCount = value;
            onFeatureNumberChangedEvents.Invoke();
        }
    }
    int _currentFeatureId = 0;
    public int currentFeatureId{
        get{
            return _currentFeatureId;
        }
        set{
            _currentFeatureId = value;
            currentIdText.text = value.ToString();
            onCurrentIdChangedEvents.Invoke();
        }
    }
    int imageCount = 2;
    public Color activeColor, notActiveColor;
    public GameObject featureLinePrefab;
    public FeatureGroup[] featureGroups;

    private void Awake() {
        for(int i=0;i<imageCount;++i){
            onFeatureNumberChangedEvents.AddListener(featureGroups[i].OnFeatureNumberChanged);
            onCurrentIdChangedEvents.AddListener(featureGroups[i].OnCurrentIdChanged);
        }
    }

    private void Start() {
        currentFeatureId = currentFeatureId;
    }
    
    public void OnFeatureNumberChanged(string s){
        int t;
        if(Int32.TryParse(s, out t)){
            featureCount = t;
        }
        currentFeatureId = Mathf.Clamp(currentFeatureId, 0, featureCount-1);
    }

    public void NextFeature(int step){
        currentFeatureId = Mathf.Clamp(currentFeatureId+step, 0, featureCount-1);
    }
}

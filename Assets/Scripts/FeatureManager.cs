using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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

    public int featureCount = 10;
    public int currentFeatureId = 0;
    int imageCount = 2;
    public Color activeColor, notActiveColor;
    public GameObject featureLinePrefab;
    FeatureGroup[] featureGroups;
    
    public void OnFeatureNumberChanged(string s){
        int t;
        if(Int32.TryParse(s, out t)){
            featureCount = t;
        }
        currentFeatureId = Mathf.Clamp(currentFeatureId, 0, featureCount-1);
        for(int i=0;i<imageCount;++i){
            featureGroups[i].OnFeatureNumberChanged();
        }
    }

    public void NextFeature(int step){
        currentFeatureId = Mathf.Clamp(currentFeatureId+step, 0, featureCount-1);
        for(int i=0;i<imageCount;++i){
            featureGroups[i].OnCurrentIdChanged();
        }
    }
}

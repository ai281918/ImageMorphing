using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureGroup : MonoBehaviour
{
    FeatureManager featureManager;
    int currentFeatureId = 0;
    public List<FeatureLine> featureList = new List<FeatureLine>();

    private void Awake() {
        featureManager = FeatureManager.instance;

        for(int i=0;i<featureManager.featureCount;++i){
            featureList.Add(InitializeFeatureLine());
        }
    }
    
    public void OnFeatureNumberChanged(){
        while(featureList.Count < featureManager.featureCount){
            featureList.Add(InitializeFeatureLine());
        }
        for(int i=featureManager.featureCount;i<featureList.Count;++i){
            featureList[i].Reset();
        }
    }

    public void OnCurrentIdChanged(){
        featureList[currentFeatureId].SetColor(featureManager.notActiveColor);
        featureList[currentFeatureId].Cancel();
        currentFeatureId = featureManager.currentFeatureId;
        featureList[currentFeatureId].SetColor(featureManager.activeColor);
    }

    FeatureLine InitializeFeatureLine(){
        GameObject go = Instantiate(featureManager.featureLinePrefab);
        go.transform.SetParent(transform);
        go.GetComponent<RectTransform>().anchoredPosition = -GetComponent<RectTransform>().rect.size/2f;
        return go.GetComponent<FeatureLine>();
    }

    public void AddFeaturePoint(){
        Vector2 p = (Vector2)Input.mousePosition - new Vector2(Screen.width/2, Screen.height/2) - GetComponent<RectTransform>().anchoredPosition;
        featureList[currentFeatureId].AddFeaturePoint(p);
    }
}

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFeatureNumberChanged(){
        while(featureList.Count < featureManager.featureCount){
            featureList.Add(InitializeFeatureLine());
        }
        OnCurrentIdChanged();
        for(int i=currentFeatureId+1;i<featureList.Count;++i){
            featureList[i].Reset();
        }
    }

    public void OnCurrentIdChanged(){
        featureList[currentFeatureId].SetColor(featureManager.notActiveColor);
        if(featureList[currentFeatureId].pointCount == 1){
            featureList[currentFeatureId].Reset();
        }
        currentFeatureId = featureManager.currentFeatureId;
        featureList[currentFeatureId].SetColor(featureManager.activeColor);
    }

    FeatureLine InitializeFeatureLine(){
        GameObject go = Instantiate(featureManager.featureLinePrefab);
        go.transform.SetParent(transform);
        return go.GetComponent<FeatureLine>();
    }

    public void AddFeaturePoint(){
        
        switch(featureList[currentFeatureId].pointCount){
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }
}

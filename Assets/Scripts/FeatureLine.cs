using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureLine : MonoBehaviour
{
    public Vector2 p1, p2;
    public int pointCount = 0;
    public Image featureArrow, featurePoint;

    private void Awake() {
        Reset();
    }

    public void Reset(){
        pointCount = 0;
        featureArrow.gameObject.SetActive(false);
        featurePoint.gameObject.SetActive(false);
    }

    public void Cancel(){
        if(pointCount == 1){
            Reset();
        }
    }

    public void AddFeaturePoint(Vector2 p){
        pointCount %= 2;
        switch(pointCount){
            case 0:
                SetPointOne(p);
                break;
            case 1:
                SetPointTwo(p);
                break;
        }
    }

    void SetPointOne(Vector2 p){
        Reset();
        pointCount = 1;
        p1 = p;
        featurePoint.rectTransform.anchoredPosition = p;
        featurePoint.gameObject.SetActive(true);
        featureArrow.gameObject.SetActive(false);
    }

    void SetPointTwo(Vector2 p){
        pointCount = 2;
        p2 = p;
        featureArrow.rectTransform.anchoredPosition = p1;
        featureArrow.rectTransform.sizeDelta = new Vector2(Vector2.Distance(p1, p2), 16f);
        if(p1 != p2){
            featureArrow.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(p2.y-p1.y, p2.x-p1.x)/Mathf.PI*180f);
        }
        featurePoint.gameObject.SetActive(false);
        featureArrow.gameObject.SetActive(true);
    }

    public void SetColor(Color color){
        featurePoint.color = color;
        featureArrow.color = color;
    }
}

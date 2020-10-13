using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public struct PointPair{
    public Vector2 p, q, qp, p_qp;
    public float qp_magnitude, qp_sqrMagnitude;
    public PointPair(Vector2 p1, Vector2 p2){
        this.p = p1;
        this.q = p2;
        qp = p2 - p1;
        p_qp = new Vector2(qp.y, -qp.x);
        qp_magnitude = qp.magnitude;
        qp_sqrMagnitude = qp.sqrMagnitude;
    }
}

public class ImageMorphing : MonoBehaviour
{
    public Text alphaText;
    public Image imageA, imageB, imageATmp, imageBTmp, imageResult;
    Texture2D textureA, textureB, textureATmp, textureBTmp, textureResult;
    float p = 0f, a = 1f, b = 2f;

    float _alpha = 0f;
    public float alpha{
        get{
            return _alpha;
        }
        set{
            _alpha = value;
            alphaText.text = value.ToString("0.00");
            Process();
        }
    }
    List<PointPair> targerFeatureList = new List<PointPair>();
    List<PointPair> featureList_1 = new List<PointPair>();
    List<PointPair> featureList_2 = new List<PointPair>();
    FeatureManager featureManager;

    private void Awake() {
        featureManager  =FeatureManager.instance;
        textureA = imageA.sprite.texture;
        textureB = imageB.sprite.texture;
        textureATmp = new Texture2D(textureA.width, textureA.height);
        textureBTmp = new Texture2D(textureA.width, textureA.height);
        textureResult = new Texture2D(textureA.width, textureA.height);
        imageATmp.sprite = Sprite.Create (textureATmp, new Rect (0, 0, textureATmp.width, textureATmp.height), Vector2.zero);
        imageBTmp.sprite = Sprite.Create (textureBTmp, new Rect (0, 0, textureBTmp.width, textureBTmp.height), Vector2.zero);
        imageResult.sprite = Sprite.Create (textureResult, new Rect (0, 0, textureResult.width, textureResult.height), Vector2.zero);
    }

    public void Process(){
        targerFeatureList.Clear();
        featureList_1.Clear();
        featureList_2.Clear();
        List<FeatureLine> featureListTmp_1 = featureManager.featureGroups[0].featureList;
        List<FeatureLine> featureListTmp_2 = featureManager.featureGroups[1].featureList;

        // Extract active feature line
        for(int i=0;i<featureManager.featureCount;++i){
            if(featureListTmp_1[i].pointCount == 2 && featureListTmp_2[i].pointCount == 2){
                targerFeatureList.Add(new PointPair(Vector2.Lerp(featureListTmp_1[i].p1, featureListTmp_2[i].p1, alpha), 
                                                    Vector2.Lerp(featureListTmp_1[i].p2, featureListTmp_2[i].p2, alpha)));
                featureList_1.Add(new PointPair(featureListTmp_1[i].p1, featureListTmp_1[i].p2));
                featureList_2.Add(new PointPair(featureListTmp_2[i].p1, featureListTmp_2[i].p2));
            }
        }

        Parallel.For(0, textureA.width, i => {
            for(int j=0;j<textureA.height;++j){
                Vector2 xp_1 = Vector2.zero, xp_2 = Vector2.zero;
                float ws = 0;
                float w;
                Vector2 x = new Vector2(i, j);
                for(int k=0;k<targerFeatureList.Count;++k){
                    float u = Vector2.Dot(x - targerFeatureList[k].p, targerFeatureList[k].qp)/targerFeatureList[k].qp_sqrMagnitude;
                    float v = Vector2.Dot(x - targerFeatureList[k].p, targerFeatureList[k].p_qp) / targerFeatureList[k].qp_magnitude;
                    // p = 0, a = 1, b = 2
                    if(u > 1){
                        // w = Mathf.Pow(Mathf.Pow(Mathf.Sqrt(targerFeatureList[k].qp_magnitude), p) / (a + Vector2.Distance(x, targerFeatureList[k].q)), b);
                        // 由於p=0 length^p可以直接簡化為1
                        w = Mathf.Pow(1 / (a + Vector2.Distance(x, targerFeatureList[k].q)), b);
                    }
                    else if(u < 0){
                        // w = Mathf.Pow(Mathf.Pow(Mathf.Sqrt(targerFeatureList[k].qp_magnitude), p) / (a + Vector2.Distance(x, targerFeatureList[k].p)), b);
                        w = Mathf.Pow(1 / (a + Vector2.Distance(x, targerFeatureList[k].p)), b);
                    }
                    else{
                        // w = Mathf.Pow(Mathf.Pow(Mathf.Sqrt(targerFeatureList[k].qp_magnitude), p) / (a + Mathf.Abs(v)), b);
                        w = Mathf.Pow(1 / (a + Mathf.Abs(v)), b);
                    }
                    xp_1 += (featureList_1[k].p + u * (featureList_1[k].qp) + v * featureList_1[k].p_qp / featureList_1[k].qp_magnitude)*w;
                    xp_2 += (featureList_2[k].p + u * (featureList_2[k].qp) + v * featureList_2[k].p_qp / featureList_2[k].qp_magnitude)*w;
                    ws += w;
                }
                xp_1 /= ws;
                xp_2 /= ws;
                textureATmp.SetPixel((int)i, j, ColorInterpolation(xp_1, textureA));
                textureBTmp.SetPixel((int)i, j, ColorInterpolation(xp_2, textureB));
                textureResult.SetPixel((int)i, j, Color.Lerp(textureATmp.GetPixel((int)i, j), textureBTmp.GetPixel((int)i, j), alpha));
            }
        });

        textureATmp.Apply();
        textureBTmp.Apply();
        textureResult.Apply();
    }

    Color ColorInterpolation(Vector2 p, Texture2D texture){
        Color color = Color.Lerp(Color.Lerp(texture.GetPixel(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y)), texture.GetPixel(Mathf.CeilToInt(p.x), Mathf.FloorToInt(p.y)), p.x),
                                    Color.Lerp(texture.GetPixel(Mathf.FloorToInt(p.x), Mathf.CeilToInt(p.y)), texture.GetPixel(Mathf.CeilToInt(p.x), Mathf.CeilToInt(p.y)), p.x),
                                    p.y);

        return color;
    }
}

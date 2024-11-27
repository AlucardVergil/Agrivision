using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TillagePanel : MonoBehaviour
{
    public TMP_Text tillageInfo;

    GameObject apisManager;

    TillageData tillageData;


    // Start is called before the first frame update
    void Start()
    {
        apisManager = GameObject.FindGameObjectWithTag("APIsManager");
                

        apisManager.GetComponent<TillageAPI>().GetTillageData("157212", (jsonResponseTillage) =>
        {
            tillageData = JsonUtility.FromJson<TillageData>(jsonResponseTillage);

            if (tillageData != null)
            {
                for (int i = 0; i < tillageData.tillage_chart_data.Length; i++)
                {
                    tillageInfo.text += tillageData.tillage_chart_data[i].date + "\n";
                    tillageInfo.text += tillageData.tillage_chart_data[i].value.ToString() + " " + tillageData.unit + "\n";
                }
            }
        });
    }

}

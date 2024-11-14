using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    public GameObject CharacterPrefab;
    public GameController Controller;
    public GameObject MaskPath;
    public Transform MaskParentTrans;
    // Start is called before the first frame update
    void Start()
    {
        TextAsset initData = Resources.Load<TextAsset>("data");
        if (initData != null)
        {
            JObject jsonObject = JObject.Parse(initData.text);
            JArray initItemsArray = (JArray)jsonObject["Activity"]["Questions"][0]["Body"]["options"];
            JArray anwserArray = (JArray)jsonObject["Activity"]["Questions"][0]["Body"]["answers"];
            JArray tagsArray = (JArray)jsonObject["Activity"]["Questions"][0]["Body"]["tags"];
            List<int> anwser = new List<int>();
            Tags tag = new Tags();
            foreach (JObject initItem in tagsArray)
            {
                tag = initItem.ToObject<Tags>();
            }
            Controller.GameTags = tag;
            foreach (string i in anwserArray[0])
            {
                anwser.Add(int.Parse(i));
            }
            Controller.Anwser = anwser;
            Controller.CharacterParentTransform = transform;
            Controller.InitGame();
            foreach (JObject initItem in initItemsArray)
            {
                CharacterCellData data = initItem.ToObject<CharacterCellData>();
                if (CharacterPrefab != null)
                {
                    CharacterCell cell = Instantiate(CharacterPrefab,transform).GetComponent<CharacterCell>();
                    cell.CellData = data;
                    cell.InitItem();
                    cell.CellClicked = Controller.OnCellClicked;
                    if (MaskParentTrans != null && MaskPath != null)
                    {
                        RectTransform transform = Instantiate(MaskPath, MaskParentTrans).GetComponent<RectTransform>();                        
                        cell.MaskTransform = transform;
                    }
                }              
            }         
        }
        else
        {
            Debug.LogError("Failed to load initData file.");
        }
    }
}

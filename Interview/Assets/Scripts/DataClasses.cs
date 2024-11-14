using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterCellData
{
    public string type;
    public string id;
    public string text;
    public string prompt;
    public string hideText;
    public CharacterImageData image;
    public string audio;
    public string pdf;
    public string video;
    public string audioLocal;
    public string academic;
    public string showMode;
    public string subtitles;
    public int rowIndex;
    public int colIndex;
    public string lockedPosition;
    public string expected;
    public string speaker;
    public string table;
    public int startRow;
    public int endRow;
    public int startCol;
    public int endCol;
    public string cells;
}

[System.Serializable]
public class CharacterImageData
{
    public string id;
    public string url;
    public int size;
    public string sha1;
    public string mimeType;
    public int width;
    public int height;
    public string language;
    public string title;
    public int duration;
    public string thumbnails;
}

[System.Serializable]
public class Tags
{
    public string compassTags;
    public string subSkillSet;
    public string vocabulary;
    public int rows;
    public int cols;
    [JsonProperty("primary-skill-set")]
    public string primary;
}
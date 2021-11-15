using UnityEngine;

[System.Serializable]
public class GitFile
{
    //an instance of this class contains the minimum data of the file
    //you want to download or upload to github

    //commit message
    public string message;
    
    //the sha of the file
    public string sha;

    //the content of the file
    public string content;

    public static GitFile CreateFromJSON(string json)
    {
        return JsonUtility.FromJson<GitFile>(json);
    }

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

    public string GetDecodedContent()
    {
        return SaveSystem.Base64Decode(content);
    }
}

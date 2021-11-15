using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class WebManager : MonoBehaviour
{
    //class used to perform GET and PUT request

    //the informations of the file you retrieve from github (json format)
    private string fileData;

    //the decoded content of the file you retrieve form github 
    private string userData;
    
    //object representation of fileData
    private GitFile gf;

    //the file in the repository you want to retrieve data from, or you want to update
    private string fileName;

    //the link with the call to the api used to access repository content
    //with the path of the directory inside the repository where the file is
    private string apiPath = "https://api.github.com/repos/{owner}/{repo}/contents/{path}";

    //the PAT used to access your github data
    private string pat = "{PAT}";

    public void SendData()
    {
        StartCoroutine(PutRequest());
    }

    public void DownloadData()
    {
        StartCoroutine(GetRequest());
    }

    IEnumerator GetRequest()
    {
        UnityWebRequest uwb = UnityWebRequest.Get(apiPath + fileName);
        uwb.SetRequestHeader("Authorization", "token " + pat);
        yield return uwb.SendWebRequest();

        if (uwb.isNetworkError)
        {
            Debug.Log("Network error: " + uwb.error);
        }
        else if (uwb.isHttpError)
        {
            if (uwb.responseCode == 404)
            {
                Debug.Log("Http error: " + uwb.error + " file not found");
            }
            else
            {
                Debug.Log("Http error: " + uwb.error);
            }
        }
        else
        {
            while (!uwb.downloadHandler.isDone)
            {
                yield return null;
            }
            fileData = uwb.downloadHandler.text;
            uwb.downloadHandler.Dispose();
            gf = GitFile.CreateFromJSON(fileData);
            userData = gf.GetDecodedContent();
        }    
    }

    IEnumerator PutRequest()
    {
        gf.message = DateTime.Now.ToString();
        gf.content = SaveSystem.Base64Encode(userData);
       
        UnityWebRequest uwb = UnityWebRequest.Put(apiPath + fileName, gf.SaveToString());
        uwb.SetRequestHeader("Authorization", "token " + pat);
        uwb.SetRequestHeader("Accept", "application/vnd.github.v3+json");
        yield return uwb.SendWebRequest();

        if (uwb.isNetworkError)
        {
            Debug.Log("Network error: " + uwb.error);
        }
        else if (uwb.isHttpError)
        {
            if (uwb.responseCode == 404)
            {
                Debug.Log("Http error: " + uwb.error + " file not found");
            }
            else
            {
                Debug.Log("Http error: " + uwb.error);
            }
        }
    }
}

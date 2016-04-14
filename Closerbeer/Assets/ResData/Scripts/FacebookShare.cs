using UnityEngine;
using System.Collections;

public class FacebookShare : MonoBehaviour {

    void Awake()
    {
        enabled = false;
        FB.Init(SetInit, OnHideUnity);
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void LogInCallBack(FBResult result)
    {
        if(FB.IsLoggedIn)
        {
            Debug.Log(FB.UserId);
            Share();
        }
        else
        {
            Debug.LogWarning(result.Error);
        }
        
    }

    private void SetInit()
    {
        enabled = true;

        // "enabled" is a magic global; this lets us wait for FB before we start rendering
        Debug.Log("Facebook SDK Is Init");
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
    }

    // Unity will call OnApplicationPause(false) when an app is resumed
    // from the background
    void OnApplicationPause(bool pauseStatus)
    {
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!pauseStatus)
        {
            //app resume
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // start the game back up - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void Share()
    {
        if (FB.IsLoggedIn)
        {
            string linkCaption = string.Format("I just smashed {0:0.00} in CloserBeer's Level {1}. Friends ! Can you beat it?", DataManager.Ins.CurScore, DataManager.Ins.CurLevel); 
            string link = "https://www.facebook.com/CloserBeerGame";
            //string pictureLink = "";
            //string name = "name";
            //string description = "description";
            string linkName = "CloserBeer Score";
            string pictureURL = "http://mlkw.cdn.download.movefun.com/toto_image_sea/temp/closerbeershare.jpg";

            FB.Feed(
                linkCaption:linkCaption,
                linkName:linkName,
                link:link,
                picture:pictureURL
                );
        }
        else
        {
            FB.Login("publish_actions", LogInCallBack);
        }
    }

}

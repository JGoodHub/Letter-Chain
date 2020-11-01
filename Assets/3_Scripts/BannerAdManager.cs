using GoodHub.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAdManager : Singleton<BannerAdManager>
{
    [Space]

    public string gameId;
    public string placementId;
    public bool testMode;

    [Space]

    public bool showOnStartup;
    public float startupDelay;

    private IEnumerator Start()
    {
        Advertisement.Initialize(gameId, testMode);

        yield return new WaitForSeconds(startupDelay);

        while (Advertisement.IsReady(placementId) == false)
            yield return null;

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);

        SetBannerState(showOnStartup, startupDelay);
    }


    public void SetBannerState(bool state)
    {
        StopAllCoroutines();
        StartCoroutine(SetBannerStateCouroutine(state, 0f));
    }

    public void SetBannerState(bool state, float delay)
    {
        StopAllCoroutines();
        StartCoroutine(SetBannerStateCouroutine(state, delay));
    }

    IEnumerator SetBannerStateCouroutine(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (state == true)
            Advertisement.Banner.Show(placementId);
        else
            Advertisement.Banner.Hide();
    }

}

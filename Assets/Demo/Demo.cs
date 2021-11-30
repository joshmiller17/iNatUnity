using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JoshAaronMiller.INaturalist;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public GameObject INatImage;
    public GameObject INatManagerObj;
    public GameObject AttributionObj;
    public GameObject ObservationCountObj;
    public GameObject LoadingTextObj;
    public GameObject VoteButtonObjs;
    public GameObject VoteButtonOneObj;
    public GameObject VoteButtonTwoObj;
    
    INatManager INatManager;
    Text Attribution;
    Text ObservationCount;
    Text VoteButtonOne;
    Text VoteButtonTwo;

    List<Observation> observations = new List<Observation>();
    int carouselIndex = 0;

    void Start()
    {
        INatManager = INatManagerObj.GetComponent<INatManager>();
        Attribution = AttributionObj.GetComponent<Text>();
        ObservationCount = ObservationCountObj.GetComponent<Text>();
        VoteButtonOne = VoteButtonOneObj.GetComponent<Text>();
        VoteButtonTwo = VoteButtonTwoObj.GetComponent<Text>();
        Debug.Log(VoteButtonOne);

        INatManager.GetApiToken();
        //ShowDemoSearch();
    }

    void ShowDemoSearch()
    {
        ObservationSearch os = new ObservationSearch();
        //os.SetIconicTaxa(new List<ObservationSearch.IconicTaxon>() { ObservationSearch.IconicTaxon.Mammalia });
        os.SetOrder(ObservationSearch.OrderBy.Votes, ObservationSearch.SortOrder.Desc);
        os.SetPagination(200, Random.Range(1,5));
        os.SetBooleanParameter(ObservationSearch.BooleanParameter.HasPhotos, true);
        os.SetBooleanParameter(ObservationSearch.BooleanParameter.IsPopular, true);
        INatManager.SearchObservations(os, PopulateCarousel);
    }

    public void MoveCarouselLeft()
    {
        if (observations.Count == 0) return;
        carouselIndex -= 1;
        if (carouselIndex < 0)
        {
            carouselIndex = observations.Count - 1;
        }
        RefreshActiveObservation();
    }

    public void MoveCarouselRight()
    {
        if (observations.Count == 0) return;
        carouselIndex += 1;
        if (carouselIndex >= observations.Count)
        {
            carouselIndex = 0;
        }
        RefreshActiveObservation();
    }

    public void VoteOptionOne()
    {
        // TODO submit vote
        RemoveObservation();
    }

    public void VoteOptionTwo()
    {
        // TODO submit vote
        RemoveObservation();
    }

    public void RemoveObservation()
    {
        observations.RemoveAt(carouselIndex);
        if (carouselIndex > 0)
        {
            carouselIndex -= 1;
        }
        RefreshActiveObservation();
    }

    void RefreshActiveObservation()
    {
        LoadingTextObj.SetActive(true);
        if (observations.Count == 0)
        {
            ObservationCount.text = "0 / 0";
            VoteButtonObjs.SetActive(false);
        }
        else
        {
            ObservationCount.text = (carouselIndex + 1).ToString() + " / " + observations.Count;
            VoteButtonObjs.SetActive(true);
        }
        string photoUrl = observations[carouselIndex].GetPhotoUrls(Observation.ImageSize.Large)[0];
        StartCoroutine(Utilities.LoadImageFromPath(photoUrl, INatImage, RemoveLoading));
        Attribution.text = observations[carouselIndex].photos[0].attribution;
        if (VoteButtonObjs.activeInHierarchy)
        {
            PopulateVoteOptions();
        }
    }

    void PopulateVoteOptions()
    {
        Dictionary<string, int> idents = observations[carouselIndex].CountIdentifications();
        string bestNameOne = "";
        int bestCountOne = 0;
        string bestNameTwo = "";
        int bestCountTwo = 0;
        foreach (KeyValuePair<string, int> ident in idents)
        {
            if (ident.Value > bestCountOne && bestCountOne < bestCountTwo)
            {
                bestNameOne = ident.Key;
                bestCountOne = ident.Value;
            }
            else if (ident.Value > bestCountTwo)
            {
                bestNameTwo = ident.Key;
                bestCountTwo = ident.Value;
            }
        }
        VoteButtonOne.text = bestNameOne;
        VoteButtonTwo.text = bestNameTwo;
    }

    public void RemoveLoading()
    {
        LoadingTextObj.SetActive(false);
    }

    void PopulateCarousel(List<Observation> results)
    {
        observations.Clear();
        Debug.Log("Search yielded " + results.Count + " results");
        carouselIndex = 0;

        foreach (Observation r in results)
        {
            // for the purpose of this demo, only save results that have some disagreement and some popularity
            if (r.num_identification_disagreements < 1 || r.identifications_count < 5) continue;
            observations.Add(r);
        }
        Debug.Log("Kept " + observations.Count + " observations");
        RefreshActiveObservation();
    }
}

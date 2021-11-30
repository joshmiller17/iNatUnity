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
    
    INatManager INatManager;
    Text Attribution;
    Text ObservationCount;

    List<Observation> observations = new List<Observation>();
    int carouselIndex = 0;

    void Start()
    {
        INatManager = INatManagerObj.GetComponent<INatManager>();
        Attribution = AttributionObj.GetComponent<Text>();
        ObservationCount = ObservationCountObj.GetComponent<Text>();

        ShowDemoSearch();
    }

    void ShowDemoSearch()
    {
        ObservationSearch os = new ObservationSearch();
        os.SetOrder(ObservationSearch.OrderBy.SpeciesGuess, ObservationSearch.SortOrder.Asc);
        os.SetIconicTaxa(new List<ObservationSearch.IconicTaxon>() { ObservationSearch.IconicTaxon.Mammalia });
        os.SetOrder(ObservationSearch.OrderBy.Votes, ObservationSearch.SortOrder.Desc);
        os.SetPagination(200, 1);
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

    void RefreshActiveObservation()
    {
        LoadingTextObj.SetActive(true);
        if (observations.Count == 0)
        {
            ObservationCount.text = "0 / 0";
        }
        else
        {
            ObservationCount.text = (carouselIndex + 1).ToString() + " / " + observations.Count;
        }
        string photoUrl = observations[carouselIndex].GetPhotoUrls(Observation.ImageSize.Large)[0];
        StartCoroutine(Utilities.LoadImageFromPath(photoUrl, INatImage, RemoveLoading));
        Attribution.text = observations[carouselIndex].photos[0].attribution;
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
            Dictionary<string, int> idents = r.CountIdentifications();
            observations.Add(r);
        }
        Debug.Log("Kept " + observations.Count + " observations");
        RefreshActiveObservation();
    }
}

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
    
    INatManager INatManager;
    Text Attribution;

    // Start is called before the first frame update
    void Start()
    {
        INatManager = INatManagerObj.GetComponent<INatManager>();
        Attribution = AttributionObj.GetComponent<Text>();

        //TEST load an image
        Debug.Log("Running test");
        ObservationSearch os = new ObservationSearch();
        os.SetOrder(ObservationSearch.OrderBy.SpeciesGuess, ObservationSearch.SortOrder.Asc);
        os.SetQualityGrade(ObservationSearch.QualityGrade.Research);
        os.SetIconicTaxa(new List<ObservationSearch.IconicTaxon>() { ObservationSearch.IconicTaxon.Mammalia });
        os.SetObservedOnDateLimits("2021-1-1", "2021-11-1");
        os.SetBooleanParameter(ObservationSearch.BooleanParameter.HasPhotos, true);
        os.SetBooleanParameter(ObservationSearch.BooleanParameter.IsPopular, true);
        INatManager.SearchObservations(os, TestCallback);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestCallback(List<Observation> results)
    {
        string photoUrl = results[0].GetPhotoUrls(Observation.ImageSize.Large)[0];
        Debug.Log(photoUrl);
        StartCoroutine(Utilities.LoadImageFromPath(photoUrl, INatImage));
        Attribution.text = results[0].photos[0].attribution;
    }
}

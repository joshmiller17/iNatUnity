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
        //os.SetQualityGrade(ObservationSearch.QualityGrade.Research);
        os.SetIconicTaxa(new List<ObservationSearch.IconicTaxon>() { ObservationSearch.IconicTaxon.Mammalia });
        os.SetOrder(ObservationSearch.OrderBy.Votes, ObservationSearch.SortOrder.Desc);
        os.SetPagination(200, 1);
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
        Debug.Log(results.Count + " results");

        string photoUrl = results[0].GetPhotoUrls(Observation.ImageSize.Large)[0];
        StartCoroutine(Utilities.LoadImageFromPath(photoUrl, INatImage));
        Attribution.text = results[0].photos[0].attribution;

        foreach (Observation r in results)
        {
            if (r.num_identification_disagreements < 1 || r.identifications_count < 5) continue;
            Debug.Log(r.taxon.preferred_common_name + "/" + r.GetAgreementRate().ToString());
            Debug.Log(r.photos[0].url);
            Dictionary<string, int> idents = r.CountIdentifications();
            if (idents.Keys.Count > 3) continue;
            foreach (KeyValuePair<string, int> ident in idents)
            {
                Debug.Log("--- " + ident.Key + ": " + ident.Value);
            }
        }
    }
}

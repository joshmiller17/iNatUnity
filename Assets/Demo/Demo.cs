using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JoshAaronMiller.INaturalist;
using UnityEngine.UI;
using System.Net;
using System.Linq;

public class Demo : MonoBehaviour
{
    public GameObject INatManagerObj;

    [Space(10)]
    [Header("Observation Carousel")]
    public GameObject INatImage;
    public GameObject AttributionObj;
    public GameObject ObservationCountObj;
    public GameObject LoadingTextObj;

    [Space(10)]
    [Header("Vote Buttons")]
    public GameObject VoteButtonObjs;
    public GameObject VoteButtonOneObj;
    public GameObject VoteButtonTwoObj;

    [Space(10)]
    [Header("Login Panel")]
    public GameObject LoginButtonObj;
    public GameObject PostLoginObj;
    public GameObject ErrorMessageObj;
    public GameObject ApiTokenInputObj;
    public GameObject LoggedInAsObj;
    public GameObject InfoDetailsObj;


    INatManager INatManager;
    Text Attribution;
    Text ObservationCount;
    Text VoteButtonOne;
    Text VoteButtonTwo;
    Text ErrorMessage;
    Text LoggedInAs;
    InputField ApiTokenInput;
    Taxon VoteTaxonOne;
    Taxon VoteTaxonTwo;

    static readonly string BadApiTokenSyntax = "Invalid syntax. Paste just the token string without quotes or the \"api_token\" label before it.";
    static readonly string InvalidApiToken = "Invalid API token.";


    List<Observation> observations = new List<Observation>();
    int carouselIndex = 0;

    void Start()
    {
        INatManager = INatManagerObj.GetComponent<INatManager>();
        Attribution = AttributionObj.GetComponent<Text>();
        ObservationCount = ObservationCountObj.GetComponent<Text>();
        VoteButtonOne = VoteButtonOneObj.GetComponent<Text>();
        VoteButtonTwo = VoteButtonTwoObj.GetComponent<Text>();
        ErrorMessage = ErrorMessageObj.GetComponent<Text>();
        LoggedInAs = LoggedInAsObj.GetComponent<Text>();
        ApiTokenInput = ApiTokenInputObj.GetComponent<InputField>();

        //ShowDemoSearch();
    }

    public void ClickLoginButton()
    {
        LoginButtonObj.SetActive(false);
        PostLoginObj.SetActive(true);
        InfoDetailsObj.SetActive(true);
    }

    public void CheckApiToken()
    {
        string apiToken = ApiTokenInput.text;
        if (new List<string>() { "\"", "{", "}", ":", "api_token" }.Any(apiToken.Contains))
        {
            ErrorMessage.text = BadApiTokenSyntax;
            ErrorMessageObj.SetActive(true);
        }
        else
        {
            ErrorMessageObj.SetActive(false);
            INatManager.SetApiToken(apiToken);
            INatManager.GetUserMe(SetUser, SetUserError);
        }
    }

    public void SetUser(User me)
    {
        ErrorMessageObj.SetActive(false);
        LoggedInAs.text = "Logged in as: " + me.login;
        LoggedInAsObj.SetActive(true);
        PostLoginObj.SetActive(false);

    }

    public void SetUserError(Error e)
    {
        if (e.status == (int)HttpStatusCode.Unauthorized)
        {
            ErrorMessage.text = InvalidApiToken;
            ErrorMessageObj.SetActive(true);
        }
    }

    void ShowDemoSearch()
    {
        ObservationSearch os = new ObservationSearch();
        //os.SetIconicTaxa(new List<ObservationSearch.IconicTaxon>() { ObservationSearch.IconicTaxon.Mammalia });
        os.SetOrder(ObservationSearch.OrderBy.Votes, ObservationSearch.SortOrder.Desc);
        os.SetPagination(200, Random.Range(1,5));
        os.SetBooleanParameter(ObservationSearch.BooleanParameter.HasPhotos, true);
        os.SetBooleanParameter(ObservationSearch.BooleanParameter.IsPopular, true);
        INatManager.SearchObservations(os, PopulateCarousel, HandleError);
    }

    public void HandleError(Error e)
    {
        Debug.LogError(e.error);
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
        SubmitVote(VoteTaxonOne);
        RemoveObservation();
    }

    public void VoteOptionTwo()
    {
        SubmitVote(VoteTaxonTwo);
        RemoveObservation();
    }

    void SubmitVote(Taxon taxon)
    {
        Identification ident = new Identification();

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
        Dictionary<Taxon, int> idents = observations[carouselIndex].CountIdentifications();
        int bestCountOne = 0;
        int bestCountTwo = 0;
        foreach (KeyValuePair<Taxon, int> ident in idents)
        {
            if (ident.Value > bestCountOne && bestCountOne < bestCountTwo)
            {
                VoteButtonOne.text = ident.Key.preferred_common_name;
                VoteTaxonOne = ident.Key;
                bestCountOne = ident.Value;
            }
            else if (ident.Value > bestCountTwo)
            {
                VoteButtonTwo.text = ident.Key.preferred_common_name;
                VoteTaxonTwo = ident.Key;
                bestCountTwo = ident.Value;
            }
        }
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

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
    public GameObject apiTokenInputObj;
    public GameObject LoggedInAsObj;
    public GameObject InfoDetailsObj;


    INatManager iNatManager;
    Text attribution;
    Text observationCount;
    Text voteButtonOne;
    Text voteButtonTwo;
    Text errorMessage;
    Text loggedInAs;
    InputField apiTokenInput;
    Taxon voteTaxonOne;
    Taxon voteTaxonTwo;

    User user;

    static readonly string BadApiTokenSyntax = "Invalid syntax. Paste just the token string without quotes or the \"api_token\" label before it.";
    static readonly string InvalidApiToken = "Invalid API token.";


    List<Observation> observations = new List<Observation>();
    int carouselIndex = 0;
    bool loggedIn = false;

    void Start()
    {
        iNatManager = INatManagerObj.GetComponent<INatManager>();
        attribution = AttributionObj.GetComponent<Text>();
        observationCount = ObservationCountObj.GetComponent<Text>();
        voteButtonOne = VoteButtonOneObj.GetComponent<Text>();
        voteButtonTwo = VoteButtonTwoObj.GetComponent<Text>();
        errorMessage = ErrorMessageObj.GetComponent<Text>();
        loggedInAs = LoggedInAsObj.GetComponent<Text>();
        apiTokenInput = apiTokenInputObj.GetComponent<InputField>();

        ShowDemoSearch();
    }

    public void TestButton()
    {
        IdentificationSubmission ident = new IdentificationSubmission();
        ident.observation_id = 36167334;
        ident.taxon_id = 244400;
        int identId = 224240608;
        iNatManager.UpdateIdentification(identId, ident, TestPrint, HandleError);
    }

    public void TestPrint(Identification i)
    {
        Debug.Log("success!");
    }

    public void ClickLoginButton()
    {
        LoginButtonObj.SetActive(false);
        PostLoginObj.SetActive(true);
        InfoDetailsObj.SetActive(true);
    }

    public void CheckApiToken()
    {
        string apiToken = apiTokenInput.text;
        if (new List<string>() { "\"", "{", "}", ":", "api_token" }.Any(apiToken.Contains))
        {
            errorMessage.text = BadApiTokenSyntax;
            ErrorMessageObj.SetActive(true);
        }
        else
        {
            ErrorMessageObj.SetActive(false);
            iNatManager.SetApiToken(apiToken);
            iNatManager.GetUserMe(SetUser, SetUserError);
        }
    }

    public void SetUser(User me)
    {
        user = me;
        ErrorMessageObj.SetActive(false);
        loggedInAs.text = "Logged in as: " + user.login;
        LoggedInAsObj.SetActive(true);
        PostLoginObj.SetActive(false);
        loggedIn = true;
        TryShowVotingButtons();
    }

    public void SetUserError(Error e)
    {
        if (e.status == (int)HttpStatusCode.Unauthorized)
        {
            errorMessage.text = InvalidApiToken;
            ErrorMessageObj.SetActive(true);
        }
    }

    void ShowDemoSearch()
    {
        ObservationSearch os = new ObservationSearch();
        os.SetIconicTaxa(new List<ObservationSearch.IconicTaxon>() { (ObservationSearch.IconicTaxon)Random.Range(1,13) }); //limit to a random iconic taxon
        os.SetOrder(OrderBy.Votes, SortOrder.Desc);
        os.SetPagination(30, Random.Range(1,5));
        os.SetBooleanParameter(ObservationSearch.BooleanParameter.HasPhotos, true);
        os.SetBooleanParameter(ObservationSearch.BooleanParameter.IsPopular, true);
        iNatManager.SearchObservations(os, PopulateCarousel, HandleError);
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

    public void TryShowVotingButtons()
    {
        if (loggedIn && observations.Count > 0)
        {
            VoteButtonObjs.SetActive(true);
            PopulateVoteOptions();
        }
        else
        {
            VoteButtonObjs.SetActive(false);
        }
    }

    public void VoteOptionOne()
    {
        SubmitVote(voteTaxonOne);
        RemoveObservation();
    }

    public void VoteOptionTwo()
    {
        SubmitVote(voteTaxonTwo);
        RemoveObservation();
    }

    void SubmitVote(Taxon taxon)
    {
        IdentificationSubmission identSub = new IdentificationSubmission();
        identSub.observation_id = observations[carouselIndex].id;
        identSub.taxon_id = taxon.id;
        iNatManager.CreateIdentification(identSub, CreateIdentificationCallback, HandleError);
    }

    public void CreateIdentificationCallback(Identification ident)
    {
        Debug.Log("Successfully submitted identification " + ident.id);
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
            observationCount.text = "0 / 0";
        }
        else
        {
            observationCount.text = (carouselIndex + 1).ToString() + " / " + observations.Count;
        }
        TryShowVotingButtons();
        if (observations.Count > 0)
        {
            string photoUrl = observations[carouselIndex].GetPhotoUrls(Observation.ImageSize.Large)[0];
            StartCoroutine(Utilities.LoadImageFromPath(photoUrl, INatImage, RemoveLoading));
            attribution.text = observations[carouselIndex].photos[0].attribution;
            if (VoteButtonObjs.activeInHierarchy)
            {
                PopulateVoteOptions();
            }
        }
        else
        {
            RemoveLoading();
        }
    }

    void PopulateVoteOptions()
    {
        Dictionary<Taxon, int> idents = observations[carouselIndex].CountIdentifications();
        int bestCountOne = 0;
        int bestCountTwo = 0;
        voteButtonOne.text = "";
        voteButtonTwo.text = "";
        foreach (KeyValuePair<Taxon, int> ident in idents)
        {
            if (ident.Value > bestCountOne && bestCountOne < bestCountTwo 
                && ident.Key.preferred_common_name != "" && ident.Key.preferred_common_name != null
                && ident.Key.preferred_common_name != voteButtonTwo.text)
            {
                voteButtonOne.text = ident.Key.preferred_common_name;
                voteTaxonOne = ident.Key;
                bestCountOne = ident.Value;
            }
            else if (ident.Value > bestCountTwo
                && ident.Key.preferred_common_name != "" && ident.Key.preferred_common_name != null)
            {
                voteButtonTwo.text = ident.Key.preferred_common_name;
                voteTaxonTwo = ident.Key;
                bestCountTwo = ident.Value;
            }
        }
        if (bestCountOne < 1 || voteButtonOne.text == "")
        {
            VoteButtonOneObj.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            VoteButtonOneObj.transform.parent.gameObject.SetActive(true);
        }
        if (bestCountTwo < 1 || voteButtonTwo.text == "")
        {
            VoteButtonTwoObj.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            VoteButtonTwoObj.transform.parent.gameObject.SetActive(true);
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
        observations = results;
        RefreshActiveObservation();
    }
}

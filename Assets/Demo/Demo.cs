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

    [Space(10)]
    [Header("Criteria Panel")]
    public GameObject SearchButton;
    public GameObject CityDropdownObj;
    public GameObject TaxonDropdownObj;
    public GameObject YearDropdownObj;
    public GameObject QualityGradeDropdownObj;


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
    Dropdown cityDropdown;
    Dropdown taxonDropdown;
    Dropdown yearDropdown;
    Dropdown qualityGradeDropdown;

    User user;

    static readonly string BadApiTokenSyntax = "Invalid syntax. Paste just the token string without quotes or the \"api_token\" label before it.";
    static readonly string InvalidApiToken = "Invalid API token.";
    static readonly string DefaultCity = "Filter by location...";
    static readonly string DefaultTaxon = "Filter by taxon...";
    static readonly string DefaultYear = "Filter by year...";
    static readonly string DefaultQuality = "Filter by quality...";

    static readonly double SearchRadius = 100; // when filtering by cities, search within 100 km

    static Dictionary<string, (double, double)> citiesToGeoLocations = new Dictionary<string, (double, double)>()
    {
        // neg lat South
        // neg lng West
        {"Boston", (42.3601, -71.0589) },
        {"Dubai", (25.2048, 55.2708) },
        {"Johannesburg", (-26.2041, 28.0473) },
        {"Lima", (-12.0464, -77.0428) },
        {"London", (51.5072, -0.1276) },
        {"Los Angeles", (34.0522, -118.2437) },
        {"New York", (40.7128, -74.006) },
        {"Paris", (48.8566, 2.3522) },
        {"Sao Paulo", (-23.558, -46.6396) },
        {"Sydney", (-33.8688, 151.2093) },
        {"Tokyo", (35.6762, 139.6503) }
    };


    // A sampling of the iconic taxa you can search with
    static Dictionary<string, ObservationSearch.IconicTaxon> iconicTaxa = new Dictionary<string, ObservationSearch.IconicTaxon>()
    {
        { "Amphibians", ObservationSearch.IconicTaxon.Amphibia },
        { "Birds", ObservationSearch.IconicTaxon.Aves },
        { "Insects", ObservationSearch.IconicTaxon.Insecta },
        { "Mammals", ObservationSearch.IconicTaxon.Mammalia },
        { "Mushrooms", ObservationSearch.IconicTaxon.Fungi },
        { "Mollusks", ObservationSearch.IconicTaxon.Mollusca },
        { "Plants", ObservationSearch.IconicTaxon.Plantae },
        { "Reptiles", ObservationSearch.IconicTaxon.Reptilia },
        { "Spiders", ObservationSearch.IconicTaxon.Arachnida }
    };

    // How verified the observation is
    static Dictionary<string, QualityGrade> qualityGrades = new Dictionary<string, QualityGrade>()
    {
        { "Casual", QualityGrade.Casual},
        { "Needs ID", QualityGrade.NeedsId},
        { "Research Grade", QualityGrade.Research}
    };

    List<Observation> observations = new List<Observation>();
    int carouselIndex = 0;
    int totalResults = 0;
    bool loggedIn = false;

    bool threatened = false;
    bool captive = false;
    bool publicDomain = false;
    bool speciesSpecific = false;

    ObservationSearch userSearch;

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
        cityDropdown = CityDropdownObj.GetComponent<Dropdown>();
        taxonDropdown = TaxonDropdownObj.GetComponent<Dropdown>();
        yearDropdown = YearDropdownObj.GetComponent<Dropdown>();
        qualityGradeDropdown = QualityGradeDropdownObj.GetComponent<Dropdown>();

        PopulateDropdowns();
        ShowDemoSearch();
    }

    // ------ CRITERIA PANEL ------
    void PopulateDropdowns()
    {
        Debug.Log("Initializing dropdown menus");

        Dropdown.OptionData option;

        // City menu
        List<Dropdown.OptionData> cities = new List<Dropdown.OptionData>();
        option = new Dropdown.OptionData();
        option.text = DefaultCity;
        cities.Add(option);
        foreach (string city in citiesToGeoLocations.Keys)
        {
            option = new Dropdown.OptionData();
            option.text = city;
            cities.Add(option);
        }
        cityDropdown.ClearOptions();
        cityDropdown.AddOptions(cities);

        // Taxa menu
        List<Dropdown.OptionData> taxa = new List<Dropdown.OptionData>();
        option = new Dropdown.OptionData();
        option.text = DefaultTaxon;
        taxa.Add(option);
        foreach (string taxon in iconicTaxa.Keys)
        {
            option = new Dropdown.OptionData();
            option.text = taxon;
            taxa.Add(option);
        }
        taxonDropdown.ClearOptions();
        taxonDropdown.AddOptions(taxa);


        // Year menu
        List<Dropdown.OptionData> years = new List<Dropdown.OptionData>();
        option = new Dropdown.OptionData();
        option.text = DefaultYear;
        years.Add(option);
        for (int year = 1990; year < 2022; year++)
        {
            option = new Dropdown.OptionData();
            option.text = year.ToString();
            years.Add(option);
        }
        yearDropdown.ClearOptions();
        yearDropdown.AddOptions(years);


        // Quality grade menu
        List<Dropdown.OptionData> qdList = new List<Dropdown.OptionData>();
        option = new Dropdown.OptionData();
        option.text = DefaultQuality;
        qdList.Add(option);
        foreach (string grade in qualityGrades.Keys)
        {
            option = new Dropdown.OptionData();
            option.text = grade;
            qdList.Add(option);
        }
        qualityGradeDropdown.ClearOptions();
        qualityGradeDropdown.AddOptions(qdList);
    }

    public void CitySelectDropdownCallback()
    {
        string choice = CityDropdownObj.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        if (choice != DefaultCity)
        {
            userSearch.SetBoundingCircle(citiesToGeoLocations[choice].Item1, citiesToGeoLocations[choice].Item2, SearchRadius);
        }
    }

    public void TaxonSelectDropdownCallback()
    {
        string choice = TaxonDropdownObj.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        if (choice != DefaultTaxon)
        {
            userSearch.SetIconicTaxa(new List<ObservationSearch.IconicTaxon>() { iconicTaxa[choice] });
        }
    }

    public void YearSelectDropdownCallback()
    {
        string choice = YearDropdownObj.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        if (choice != DefaultYear)
        {
            userSearch.IncludeYears(new List<int>() { System.Int32.Parse(choice) });
        }
    }

    public void QualityGradeSelectDropdownCallback()
    {
        string choice = QualityGradeDropdownObj.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        if (choice != DefaultQuality)
        {
            userSearch.SetQualityGrade(qualityGrades[choice]);
        }
    }

    public void ToggleThreatened()
    {
        threatened = !threatened;
        userSearch.SetBooleanParameter(ObservationSearch.BooleanParameter.IsThreatened, threatened);
    }

    public void ToggleCaptive()
    {
        captive = !captive;
        userSearch.SetBooleanParameter(ObservationSearch.BooleanParameter.IsCaptive, captive);
    }

    public void TogglePublicDomain()
    {
        publicDomain = !publicDomain;
        if (publicDomain)
        {
            userSearch.SetLicense(ObservationSearch.License.Cc0);
        }
        else
        {
            userSearch.SetLicense(ObservationSearch.License.None);
        }
    }

    public void ToggleSpeciesSpecific()
    {
        speciesSpecific = !speciesSpecific;
        if (speciesSpecific)
        {
            userSearch.SetTaxonRankLimits(highest: TaxonRank.Species);
        }
        else
        {
            userSearch.SetTaxonRankLimits(TaxonRank.None, TaxonRank.None);
        }
    }

    void DelaySearchButton()
    {
        SearchButton.GetComponent<Button>().interactable = false;
        SearchButton.GetComponent<Image>().color = Color.gray;
        SearchButton.transform.GetChild(0).GetComponent<Text>().text = "Please wait";
        Invoke("EnableSearchButton", 3);
    }

    void EnableSearchButton()
    {
        SearchButton.GetComponent<Button>().interactable = true;
        SearchButton.GetComponent<Image>().color = Color.yellow;
        SearchButton.transform.GetChild(0).GetComponent<Text>().text = "Search!";
    }

    public void DoSearch()
    {
        Debug.Log("Sending observation search request");
        iNatManager.SearchObservations(userSearch, PopulateCarousel, HandleError);
        DelaySearchButton();
    }

    public void HandleError(Error e)
    {
        Debug.LogError(e.error);
    }

    void ShowDemoSearch()
    {
        userSearch = new ObservationSearch();
        userSearch.SetIconicTaxa(new List<ObservationSearch.IconicTaxon>() { (ObservationSearch.IconicTaxon)Random.Range(1, 13) }); //limit to a random iconic taxon
        userSearch.SetOrder(OrderBy.Votes, SortOrder.Desc);
        userSearch.SetPagination(30, Random.Range(1, 5));
        userSearch.SetBooleanParameter(ObservationSearch.BooleanParameter.HasPhotos, true);
        userSearch.SetBooleanParameter(ObservationSearch.BooleanParameter.IsPopular, true);
        DoSearch();
    }

    // ------ LOGIN PANEL ------

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

   // ------ OBSERVATION CAROUSEL ------

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
        INatImage.GetComponent<RawImage>().texture = null;
        attribution.text = "";
        if (observations.Count == 0)
        {
            observationCount.text = "No search results";
        }
        else
        {
            observationCount.text = string.Format("({0} total results)\n", totalResults) + (carouselIndex + 1).ToString() + " / " + observations.Count;
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

    void PopulateCarousel(Results<Observation> results)
    {
        observations.Clear();
        totalResults = results.total_results;
        Debug.Log("Search yielded " + totalResults + " results");
        carouselIndex = 0;
        observations = results.results;
        RefreshActiveObservation();
    }
}

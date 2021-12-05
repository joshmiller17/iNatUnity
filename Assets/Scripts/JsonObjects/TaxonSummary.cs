using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class TaxonSummary : JsonObject<TaxonSummary>
    {
        //conservation_status not yet implemented -- usually null
        //listed_taxon not yet implemented -- usually null
        public string wikipedia_summary; //contains HTML tags
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    public enum TaxonRank
    {
        None, Kingdom, Phylum, Subphylum, Superclass, Class, Subclass, Superorder,
        Order, Suborder, Infraorder, Superfamily, Epifamily, Family, Subfamily, Supertribe,
        Tribe, Subtribe, Genus, GenusHybrid, Species, Hybrid, Subspecies, Variety, Form
    };

    public enum QualityGrade
    {
        None, Casual, NeedsId, Research
    }

    public enum OrderBy
    {
        CreatedAt, ObservedOn, SpeciesGuess, Votes, Id
    };

    public enum SortOrder
    {
        Desc, Asc
    };

    public enum TaxonRankLevel
    {
        None = -1,
        Subspecies = 5,
        Species = 10,
        Genus = 20,
        Family = 30,
        Order = 40,
        Class = 50,
        Phylum = 60,
        Kingdom = 70
    };

    public static class Enums
    {

        public static Dictionary<QualityGrade, string> QualityToString = new Dictionary<QualityGrade, string>()
    {
        { QualityGrade.None, "" },
        { QualityGrade.Casual, "casual" },
        { QualityGrade.NeedsId, "needs_id" },
        { QualityGrade.Research, "research" }
    };

        public static Dictionary<OrderBy, string> OrderByToString = new Dictionary<OrderBy, string>() {
            { OrderBy.CreatedAt, "created_at"},
            { OrderBy.ObservedOn, "observed_on"},
            { OrderBy.SpeciesGuess, "species_guess"},
            { OrderBy.Votes, "votes"},
            { OrderBy.Id, "id"},
        };


    }
}

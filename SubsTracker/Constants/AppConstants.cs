using System.Collections.Generic;

namespace SubsTracker.Constants
{
    public static class AppConstants
    {
        public static class Cycles
        {
            public const string Monthly = "Monthly";
            public const string Yearly = "Yearly";
            public const string Weekly = "Weekly";
        }

        public static class Categories
        {
            // Consolidated list from your various files
            public const string Streaming = "Streaming";
            public const string Gaming = "Gaming";
            public const string Software = "Software";
            public const string Gym = "Gym";
            public const string Music = "Music";
            public const string Entertainment = "Entertainment";
            public const string Utilities = "Utilities";
            public const string Work = "Work";
            public const string Personal = "Personal";
            public const string Other = "Other";

            // Single source of truth for the list used in dropdowns/pickers
            public static readonly List<string> All = new()
            {
                Streaming,
                Gaming,
                Software,
                Gym,
                Music,
                Entertainment,
                Utilities,
                Work,
                Personal,
                Other
            };
        }
    }
}
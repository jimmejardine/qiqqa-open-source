using Utilities.GUI.Wizard;

namespace Qiqqa.Wizards
{
    internal class SearchResultWizard
    {
        public static Route GetRoute()
        {
            return new Route(
                "Qiqqa Full Text Search",
                new Step[]
                {
                    new Step
                    {
                        Instructions = "Welcome to Qiqqa's powerful full-text search!  Let's walk through three quick steps to turn you into a search power user.\n\nPress Next to get going.",
                        PostCondition_GreyInstructions = false
                    },

                    new Step
                    {
                        Instructions = "Type your search into the search box and press ENTER to see the results.  E.g. type 'the' and press ENTER.",
                        PointOfInterests = new string[] { "LibrarySearchQuickTextBox", "SearchQuickHelpButton" },
                        PostCondition = (point_of_interest_locator) => { return WizardTools.AreAnyPOIsVisible(point_of_interest_locator, "LibrarySearchScoreButton"); }
                    },

                    new Step
                    {
                        Instructions = "Each document is given a score as to how closely it matches your search.  You can click on this button to see individual search matches.",
                        PointOfInterests = new string[] { "LibrarySearchScoreButton" },
                        PostCondition = (point_of_interest_locator) => { return WizardTools.AreAnyPOIsVisible(point_of_interest_locator, "LibrarySearchDetails"); }
                    },

                    new Step
                    {
                        Instructions = "As well as doing Google-like searches through your documents, there are some pretty powerful additional search options available.  You can review them at any time by pressing on the Search Help button.",
                        PostCondition_GreyInstructions = false,
                        PostCondition_PointOfInterests = new string[] { "LibrarySearchQuickHelpButton" }
                    },
                },
                "That's it!  Get searching!"
            );
        }
    }
}

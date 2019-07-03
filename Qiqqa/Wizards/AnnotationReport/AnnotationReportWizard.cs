using Qiqqa.Documents.PDF.PDFControls.Page.Annotation;
using Utilities.GUI;
using Utilities.GUI.Wizard;

namespace Qiqqa.Wizards.AnnotationReport
{
    class AnnotationReportWizard
    {
        public static Route GetRoute()
        {
            return new Route(
                "Your first Qiqqa Annotation Report",
                new Step[]
                {
                    new Step
                    { 
                        Instructions = "Welcome to Qiqqa!  Let's quickly walk you through what we think is one of the most compelling reasons for you to love Qiqqa: the Annotation Report.\n\nPress Next to get going.",
                        PostCondition_GreyInstructions= false
                    },

                    new Step
                    { 
                        Instructions = "This is the Start Screen.  It shows all the libraries to which you belong, along with recommended reading for each library.  We are going to annotate a PDF in your Guest Library.  So click the Title or Open Button of the 'Guest Library' to open it.",
                        PointOfInterests = new string[] { "GuestLibraryOpenButton", "GuestLibraryTitle" },
                        PostCondition = (point_of_interest_locator) => { return WizardTools.AreAnyPOIsVisible(point_of_interest_locator, "GuestLibraryQiqqaManualTitle", "GuestLibraryQiqqaManualOpenButton"); }
                    },

                    new Step
                    { 
                        Instructions = "This is the Library Screen, from which you can search, sort and filter the PDFs in this library.  Along the toolbar at the top there are also functions that apply to the library as a whole.  We are going to add an Annotation to the PDF called 'The Qiqqa Manual'.  Please click its Title or Open Button to open the PDF.",
                        PointOfInterests = new string[] { "GuestLibraryQiqqaManualTitle", "GuestLibraryQiqqaManualOpenButton" },
                        PostCondition = (point_of_interest_locator) => { return WizardTools.AreAnyPOIsVisible(point_of_interest_locator, "PDFReadingAnnotationButton"); }
                    },

                    new Step
                    { 
                        Instructions = "This is the PDF Reading Screen.  The toolbar at the top gives you access to various functions that you can perform on your PDF.  We are going to add an annotation, so please select the 'Annotation' tool.",
                        PointOfInterests = new string[] { "PDFReadingAnnotationButton" },
                        PostCondition = (point_of_interest_locator) =>  
                        {
                            AugmentedToggleButton button = point_of_interest_locator.GetFirstPOI("PDFReadingAnnotationButton") as AugmentedToggleButton;
                            if (null == button) return false;
                            return button.IsChecked ?? false;
                        }
                    },

                    new Step
                    { 
                        Instructions = "You will notice that when you move the mouse cursor over the PDF it has become a crosshair, indicating that you are about to add an Annotation.  Click on the PDF and drag a new Annotation box around the word 'Qiqqa'.  Then scroll down and draw another around the Qiqqa logo.",
                        PostCondition = (point_of_interest_locator) => 
                        {
                            PDFAnnotationLayer layer = point_of_interest_locator.GetFirstPOI("PDFReadingAnnotationLayer") as PDFAnnotationLayer;
                            return (null != layer) && (layer.Children.Count >= 2);
                        }
                    },

                    new Step
                    { 
                        Instructions = "Having made your annotations, we want to generate an Annotation Report, so let's go back to the Library Screen, either by closing the PDF Reading Screen, or by clicking on the Library Screen tab at the top of the screen.",
                        PostCondition = (point_of_interest_locator) => { return WizardTools.AreAnyPOIsVisible(point_of_interest_locator, "GuestLibraryQiqqaManualTitle", "GuestLibraryQiqqaManualOpenButton"); }
                    },

                    new Step
                    { 
                        Instructions = "To run the Annotation Report, press the 'Annotation Report' button on the toolbar.",
                        PointOfInterests = new string[] { "LibraryAnnotationReportButton" },
                        PostCondition = (point_of_interest_locator) => { return WizardTools.AreAnyPOIsVisible(point_of_interest_locator, "LibraryAnnotationReportOptionsWindow"); }
                    },

                    new Step
                    { 
                        Instructions = "This is the Annotations Report Options Screen.  It allows to to select a subset of the Annotations you have made - perhaps only those you made last week, or only those you tagged with a specific tag.  Right now, we want all annotations, so just press the Generate button to create your Annotation Report.",
                        PointOfInterests = new string[] { "LibraryAnnotationReportOptionsWindowGenerateButton" },
                        PostCondition = (point_of_interest_locator) => { return WizardTools.AreAnyPOIsVisible(point_of_interest_locator, "LibraryAnnotationReportViewer"); }
                    },

                    new Step
                    { 
                        Instructions = "The Annotation Report pulls every important fact and detail that you have ever annotated, highlighted or drawn on your PDFs.\n\nIt's remarkable that you will never have to search through piles of those PDFs ever again, looking for that missing piece of information.  Even better, you can click on an image or the [Open] link to jump straight to the annotation's location in the PDF or press [Cite] to cite the PDF this annotation directly in Microsoft Word.\n\nNo wonder people love it!",
                        PointOfInterests = new string[] { "LibraryAnnotationReportOptionsWindowGenerateButton" },
                        PostCondition_GreyInstructions = false
                    },

                },
                "Congratulations!  You have made your first Annotations and created your first Annotation Report.  Never again will you have to open or read a PDF more than once to forever recall its most vital information!\n\nWe hope you enjoy exploring the rest of the features of Qiqqa as your research work becomes more and more efficient."
            );
        }
    }
}

namespace Qiqqa.UtilisationTracking
{
    public class Features
    {
        public static readonly Feature App_Open = new Feature { Name = "App_Open", Description = "Open Qiqqa" };
        public static readonly Feature App_Close = new Feature { Name = "App_Close", Description = "Close Qiqqa" };
        public static readonly Feature App_TermsAndConditions = new Feature { Name = "App_TermsAndConditions", Description = "Accepted terms and conditions" };
        public static readonly Feature App_ThemeColour = new Feature { Name = "App_ThemeColour", Description = "" };

        public static readonly Feature Beta_40 = new Feature { Name = "Beta_40", Description = "Beta v40 - Using .NET4" };

        public static readonly Feature Framework_OpenGenericControl = new Feature { Name = "Framework_OpenGenericControl", Description = "" };

        public static readonly Feature Document_Open = new Feature { Name = "Document_Open", Description = "Open a PDF" };
        public static readonly Feature Document_Save = new Feature { Name = "Document_Save", Description = "Open a PDF" };        
        public static readonly Feature Document_Print = new Feature { Name = "Document_Print", Description = "Print a PDF" };
        public static readonly Feature Document_JumpToSection = new Feature { Name = "Document_JumpToSection", Description = "" };
        public static readonly Feature Document_AddTag = new Feature { Name = "Document_AddTag", Description = "" };
        public static readonly Feature Document_RemoveTag = new Feature { Name = "Document_RemoveTag", Description = "" };
        public static readonly Feature Document_AddAnnotationTag = new Feature { Name = "Document_AddAnnotationTag", Description = "" };
        public static readonly Feature Document_RemoveAnnotationTag = new Feature { Name = "Document_RemoveAnnotationTag", Description = "" };
        public static readonly Feature Document_AddAnnotation = new Feature { Name = "Document_AddAnnotation", Description = "" };
        public static readonly Feature Document_AddHighlight = new Feature { Name = "Document_AddHighlight", Description = "" };
        public static readonly Feature Document_ChangeHighlightColour = new Feature { Name = "Document_ChangeHighlightColour", Description = "" };
        public static readonly Feature Document_HighlightSelectErase = new Feature { Name = "Document_HighlightSelectErase", Description = "" };
        public static readonly Feature Document_Camera = new Feature { Name = "Document_Camera", Description = "" };

        public static readonly Feature Document_SpeedRead = new Feature { Name = "Document_SpeedRead", Description = "" };
        public static readonly Feature Document_ReadOutLoud = new Feature { Name = "Document_ReadOutLoud", Description = "" };
        public static readonly Feature Document_ExportToText = new Feature { Name = "Document_ExportToText", Description = "" };
        public static readonly Feature Document_Search = new Feature { Name = "Document_Search", Description = "" };
        public static readonly Feature Document_SearchAgain = new Feature { Name = "Document_SearchAgain", Description = "" };
        public static readonly Feature Document_CopyText = new Feature { Name = "Document_CopyText", Description = "" };
        public static readonly Feature Document_TextSelectSpeedRead = new Feature { Name = "Document_TextSelectSpeedRead", Description = "" };
        public static readonly Feature Document_WebsiteDictionary = new Feature { Name = "Document_WebsiteDictionary", Description = "" };
        public static readonly Feature Document_TextToTag = new Feature { Name = "Document_TextToTag", Description = "" };
        public static readonly Feature Document_TextToMetadata = new Feature { Name = "Document_TextToMetadata", Description = "" };
        public static readonly Feature Document_SearchLibrary = new Feature { Name = "Document_SearchLibrary", Description = "" };
        public static readonly Feature Document_SearchInternet = new Feature { Name = "Document_SearchInternet", Description = "" };
        public static readonly Feature Document_TagCloud = new Feature { Name = "Document_TagCloud", Description = "" };
        public static readonly Feature Document_MetadataCommentEditor = new Feature { Name = "Document_MetadataCommentEditor", Description = "" };
        public static readonly Feature Document_InkChanged = new Feature { Name = "Document_InkChanged", Description = "" };
        public static readonly Feature Document_ChangeInkEditingMode = new Feature { Name = "Document_ChangeInkEditingMode", Description = "" };
        public static readonly Feature Document_ChangeInkParameters = new Feature { Name = "Document_ChangeInkParameters", Description = "" };
        public static readonly Feature Document_ExploreDocumentInBrainstorm = new Feature { Name = "Document_ExploreDocumentInBrainstorm", Description = "" };
        public static readonly Feature Document_FindAllCitations = new Feature { Name = "Document_FindAllCitations", Description = "" };
        public static readonly Feature Document_SetAbstract = new Feature { Name = "Document_SetAbstract", Description = "" };
        public static readonly Feature Document_ClearAbstract = new Feature { Name = "Document_ClearAbstract", Description = "" };

        public static readonly Feature Brainstorm_Open = new Feature { Name = "Brainstorm_Open", Description = "" };
        public static readonly Feature Brainstorm_Sample = new Feature { Name = "Brainstorm_Sample", Description = "" };
        public static readonly Feature Brainstorm_Search = new Feature { Name = "Brainstorm_Search", Description = "" };
        public static readonly Feature Brainstorm_Save = new Feature { Name = "Brainstorm_Save", Description = "" };
        public static readonly Feature Brainstorm_Print = new Feature { Name = "Brainstorm_Print", Description = "" };

        public static readonly Feature Library_KeywordFilter = new Feature { Name = "Library_KeywordFilter", Description = "" };
        public static readonly Feature Library_SearchInsideOpen = new Feature { Name = "Library_SearchInsideOpen", Description = "" };
        public static readonly Feature Library_SearchInsideJumpToLocation = new Feature { Name = "Library_SearchInsideJumpToLocation", Description = "" };
        public static readonly Feature Library_TagFilter = new Feature { Name = "Library_TagFilter", Description = "" };
        public static readonly Feature Library_TagExplorer = new Feature { Name = "Library_TagExplorer", Description = "" };
        public static readonly Feature Library_AITagExplorer = new Feature { Name = "Library_AITagExplorer", Description = "" };
        public static readonly Feature Library_AuthorExplorer = new Feature { Name = "Library_AuthorExplorer", Description = "" };
        public static readonly Feature Library_PublicationExplorer = new Feature { Name = "Library_PublicationExplorer", Description = "" };
        public static readonly Feature Library_ReadingStageExplorer = new Feature { Name = "Library_ReadingStageExplorer", Description = "" };
        public static readonly Feature Library_YearExplorer = new Feature { Name = "Library_YearExplorer", Description = "" };
        public static readonly Feature Library_RatingExplorer = new Feature { Name = "Library_RatingExplorer", Description = "" };
        public static readonly Feature Library_ThemeExplorer = new Feature { Name = "Library_ThemeExplorer", Description = "" };
        public static readonly Feature Library_TypeExplorer = new Feature { Name = "Library_TypeExplorer", Description = "" };
        public static readonly Feature Library_GenericExplorer_Filter = new Feature { Name = "Library_GenericExplorer_Filter", Description = "" };
        public static readonly Feature Library_GenericExplorer_ChartItem = new Feature { Name = "Library_GenericExplorer_ChartItem", Description = "" };

        public static readonly Feature Library_JSONAnnotationReport = new Feature { Name = "Library_JSONAnnotationReport", Description = "" };
        public static readonly Feature Library_LinkedDocsAnnotationReport = new Feature { Name = "Library_LinkedDocsAnnotationReport", Description = "" };
        public static readonly Feature Library_AnnotationReport = new Feature { Name = "Library_AnnotationReport", Description = "" };
        public static readonly Feature Library_ExploreInBrainstorm = new Feature { Name = "Library_ExploreInBrainstorm", Description = "" };
        public static readonly Feature Library_ExploreInPivot = new Feature { Name = "Library_ExploreInPivot", Description = "" };        
        public static readonly Feature Library_Search = new Feature { Name = "Library_Search", Description = "" };
        public static readonly Feature Library_BibTexExport = new Feature { Name = "Library_BibTexExport", Description = "" };
        public static readonly Feature Library_Word2007Export = new Feature { Name = "Library_Word2007Export", Description = "" };
        public static readonly Feature Library_Export = new Feature { Name = "Library_Export", Description = "" };
        public static readonly Feature Library_ImportFromBibTeXGeneric = new Feature { Name = "Library_ImportFromBibTeXGeneric", Description = "" };
        public static readonly Feature Library_ImportFromMendeley = new Feature { Name = "Library_ImportFromMendeley", Description = "" };
        public static readonly Feature Library_ImportFromZotero = new Feature { Name = "Library_ImportFromZotero", Description = "" };
        public static readonly Feature Library_ImportFromEndNote = new Feature { Name = "Library_ImportFromEndNote", Description = "" };
        public static readonly Feature Library_ImportFromThirdParty = new Feature { Name = "Library_ImportFromThirdParty", Description = "" };
        public static readonly Feature Library_ImportAutoFromEndNote = new Feature { Name = "Library_ImportAutoFromEndNote", Description = "" };
        public static readonly Feature Library_ImportAutoFromMendeley = new Feature { Name = "Library_ImportAutoFromMendeley", Description = "" };

        public static readonly Feature Library_PreviewPDF = new Feature { Name = "Library_PreviewPDF", Description = "" };
        public static readonly Feature Library_GenerateReferences = new Feature { Name = "Library_GenerateReferences", Description = "" };
        public static readonly Feature Library_FindDuplicates = new Feature { Name = "Library_FindDuplicates", Description = "" };

        public static readonly Feature Library_UseDirectoriesAsTags = new Feature { Name = "Library_UseDirectoriesAsTags", Description = "" };
        public static readonly Feature Library_UseFilenameAsTitle = new Feature { Name = "Library_UseFilenameAsTitle", Description = "" };
        public static readonly Feature Library_OpenOutsideQiqqa = new Feature { Name = "Library_OpenOutsideQiqqa", Description = "" };
        public static readonly Feature Library_OpenInWindowsExplorer = new Feature { Name = "Library_OpenInWindowsExplorer", Description = "" };
        public static readonly Feature Library_CopyDocumentToAnotherLibrary = new Feature { Name = "Library_CopyDocumentToAnotherLibrary", Description = "" };
        public static readonly Feature Library_MoveDocumentToAnotherLibrary = new Feature { Name = "Library_MoveDocumentToAnotherLibrary", Description = "" };
        public static readonly Feature Library_ExploreDocumentInBrainstorm = new Feature { Name = "Library_ExploreDocumentInBrainstorm", Description = "" };
        public static readonly Feature Library_ExploreDocumentInExpedition = new Feature { Name = "Library_ExploreDocumentInExpedition", Description = "" };
        public static readonly Feature Library_ExploreDocumentInPivot = new Feature { Name = "Library_ExploreDocumentInPivot", Description = "" };
        public static readonly Feature Library_AddMultipleTags = new Feature { Name = "Library_AddMultipleTags", Description = "" };
        public static readonly Feature Library_RemoveAllTags = new Feature { Name = "Library_RemoveAllTags", Description = "" };
        public static readonly Feature Library_RemoveAllBibTeX = new Feature { Name = "Library_RemoveAllBibTeX", Description = "" };
        public static readonly Feature Library_ForceOCR = new Feature { Name = "Library_ForceOCR", Description = "" };
        public static readonly Feature Library_ClearOCR = new Feature { Name = "Library_ClearOCR", Description = "" };
        public static readonly Feature Library_ReIndex = new Feature { Name = "Library_ReIndex", Description = "" };
        public static readonly Feature Library_CopyBibTeXKey = new Feature { Name = "Library_CopyBibTeXKey", Description = "" };
        public static readonly Feature Library_CopyQiqqaURI = new Feature { Name = "Library_CopyQiqqaURI", Description = "" };
        public static readonly Feature Library_ImportLegacyAnnotations = new Feature { Name = "Library_ImportLegacyAnnotations", Description = "" };
        public static readonly Feature Library_ForgetLegacyAnnotations = new Feature { Name = "Library_ForgetLegacyAnnotations", Description = "" };

        public static readonly Feature Library_AttachToVanilla_Local = new Feature { Name = "Library_AttachToVanilla_Local", Description = "" };
        public static readonly Feature Library_AttachToVanilla_Web = new Feature { Name = "Library_AttachToVanilla_Web", Description = "" };

        public static readonly Feature Library_ImportFromOmnipatents = new Feature { Name = "Library_ImportFromOmnipatents", Description = "" };

        public static readonly Feature Document_ExtractKeywordsAsTags = new Feature { Name = "Document_ExtractKeywordsAsTags", Description = "" };

        public static readonly Feature Library_ImportError_EndnoteImportUnknownField = new Feature { Name = "Library_ImportError_EndnoteImportUnknownField", Description = "" };

        public static readonly Feature Web_Browse = new Feature { Name = "Web_Browse", Description = "" };
        public static readonly Feature Web_Search = new Feature { Name = "Web_Search", Description = "" };
        public static readonly Feature Web_AddToLibrary = new Feature { Name = "Web_AddToLibrary", Description = "" };
        public static readonly Feature Web_ExportToPDF = new Feature { Name = "Web_ExportToPDF", Description = "" };

        public static readonly Feature Sync_Stats = new Feature { Name = "Sync_Stats", Description = "" };
        public static readonly Feature Sync_SyncMetadata = new Feature { Name = "Sync_SyncMetadata", Description = "" };
        public static readonly Feature Sync_SyncMetadataAndPDFs = new Feature { Name = "Sync_SyncMetadataAndPDFs", Description = "" };
        public static readonly Feature Sync_SyncDetails = new Feature { Name = "Sync_SyncDetails", Description = "" };        
        public static readonly Feature Sync_SyncUploadSinglePDF = new Feature { Name = "Sync_SyncUploadSinglePDF", Description = "" };
        public static readonly Feature Sync_SyncDownloadSinglePDF = new Feature { Name = "Sync_SyncDownloadSinglePDF", Description = "" };

        public static readonly Feature AnnotationReport_ToWord = new Feature { Name = "AnnotationReport_ToWord", Description = "" };
        public static readonly Feature AnnotationReport_Print = new Feature { Name = "AnnotationReport_Print", Description = "" };
        public static readonly Feature AnnotationReport_Open = new Feature { Name = "AnnotationReport_Open", Description = "" };
        public static readonly Feature AnnotationReport_Cite = new Feature { Name = "AnnotationReport_Cite", Description = "" };

        public static readonly Feature SimilarAuthor_OpenDoc = new Feature { Name = "SimilarAuthor_OpenDoc", Description = "" };
        public static readonly Feature DuplicateDetection_OpenDoc = new Feature { Name = "DuplicateDetection_OpenDoc", Description = "" };
        public static readonly Feature Citations_OpenDoc = new Feature { Name = "Citations_OpenDoc", Description = "" };
        public static readonly Feature Citations_Regenerate = new Feature { Name = "Citations_Regenerate", Description = "" };
        public static readonly Feature DocumentSearch_GoToSearchResultLocation = new Feature { Name = "DocumentSearch_GoToSearchResultLocation", Description = "" };

        public static readonly Feature LinkedDocument_InfoBar_OpenDoc = new Feature { Name = "LinkedDocument_InfoBar_OpenDoc", Description = "" };
        public static readonly Feature LinkedDocument_Library_OpenDoc = new Feature { Name = "LinkedDocument_Library_OpenDoc", Description = "" };
        public static readonly Feature LinkedDocument_Create = new Feature { Name = "LinkedDocument_Create", Description = "" };

        public static readonly Feature MetadataSniffer_UseMetadata = new Feature { Name = "MetadataSniffer_UseMetadata", Description = "" };
        public static readonly Feature MetadataSniffer_ValidBibTeX = new Feature { Name = "MetadataSniffer_ValidBibTeX", Description = "" };
        public static readonly Feature MetadataSniffer_ValidPubMed = new Feature { Name = "MetadataSniffer_ValidPubMed", Description = "" };
        public static readonly Feature BibTeX_BibTeXSearchMatch = new Feature { Name = "BibTeX_BibTeXSearchMatch", Description = "" };

        public static readonly Feature Adverts_WhatsThis = new Feature { Name = "Adverts_WhatsThis", Description = "" };
        public static readonly Feature Adverts_OpenAdvert = new Feature { Name = "Adverts_OpenAdvert", Description = "" };
        public static readonly Feature Adverts_OpenAdvertQiqqaChampion = new Feature { Name = "Adverts_OpenAdvertQiqqaChampion", Description = "" };
        public static readonly Feature Adverts_OpenAdvertQiqqaDatacopia = new Feature { Name = "Adverts_OpenAdvertQiqqaDatacopia", Description = "" };
        public static readonly Feature Adverts_OpenAdvertQiqqaOmnipatents = new Feature { Name = "Adverts_OpenAdvertQiqqaOmnipatents", Description = "" };
        public static readonly Feature Adverts_Close = new Feature { Name = "Adverts_Close", Description = "" };

        public static readonly Feature Vote_AutoTag = new Feature { Name = "Vote_AutoTag", Description = "" };
        public static readonly Feature Vote_Expedition = new Feature { Name = "Vote_Expedition", Description = "" };

        public static readonly Feature Tool_DocumentConvertWidget = new Feature { Name = "Tool_DocumentConvertWidget", Description = "" };
        public static readonly Feature Chat_Submit = new Feature { Name = "Chat_Submit", Description = "" };

        public static readonly Feature Exception = new Feature { Name = "Exception", Description = "" };
        public static readonly Feature Exception_GeckoInit = new Feature { Name = "Exception_GeckoInit", Description = "" };
        public static readonly Feature Exception_GeckoPreferences = new Feature { Name = "Exception_GeckoPreferences", Description = "" };
        public static readonly Feature Exception_GeckoProxy = new Feature { Name = "Exception_GeckoProxy", Description = "" };
        public static readonly Feature Exception_GeckoPreload = new Feature { Name = "Exception_GeckoPreload", Description = "" };
        public static readonly Feature Exception_NullExceptionInReIndexDocument = new Feature { Name = "Exception_NullExceptionInReIndexDocument", Description = "" };

        public static readonly Feature Legacy_Highlights_ProtoBuf = new Feature { Name = "Legacy_Highlights_ProtoBuf", Description = "", RecordOnlyOncePerSession = true };
        public static readonly Feature Legacy_Annotations_Binary = new Feature { Name = "Legacy_Annotations_Binary", Description = "", RecordOnlyOncePerSession = true };
        public static readonly Feature Legacy_Metadata_Binary = new Feature { Name = "Legacy_Metadata_Binary", Description = "", RecordOnlyOncePerSession = true };
        public static readonly Feature Legacy_DocumentTagsList = new Feature { Name = "Legacy_DocumentTagsList", Description = "", RecordOnlyOncePerSession = true };

        public static readonly Feature Diagnostics_DropBox = new Feature { Name = "Diagnostics_DropBox", Description = "", RecordOnlyOncePerSession = true };
        
        public static readonly Feature Premium_Force = new Feature { Name = "Premium_Force", Description = "", RecordOnlyOncePerSession = true };
        public static readonly Feature Premium_Inquiry = new Feature { Name = "Premium_Inquiry", Description = "" };

        public static readonly Feature Share_InvokeLibrary = new Feature { Name = "Share_InvokeLibrary", Description = "" };
        public static readonly Feature Share_InvokeDocument = new Feature { Name = "Share_InvokeDocument", Description = "" };
        public static readonly Feature Share_OpenURL = new Feature { Name = "Share_OpenURL", Description = "" };
        public static readonly Feature Share_CopyURL = new Feature { Name = "Share_CopyURL", Description = "" };

        public static readonly Feature StartPage_SuggestedReading = new Feature { Name = "StartPage_SuggestedReading", Description = "" };
        public static readonly Feature StartPage_RecentlyAdded = new Feature { Name = "StartPage_RecentlyAdded", Description = "" };
        public static readonly Feature StartPage_RecentlyRead = new Feature { Name = "StartPage_RecentlyRead", Description = "" };
        public static readonly Feature StartPage_QiqqaWeb = new Feature { Name = "StartPage_QiqqaWeb", Description = "" };
        public static readonly Feature StartPage_Android = new Feature { Name = "StartPage_Android", Description = "" };
        public static readonly Feature StartPage_Datacopia = new Feature { Name = "StartPage_Datacopia", Description = "" };
        public static readonly Feature StartPage_Champion = new Feature { Name = "StartPage_Champion", Description = "" };
        public static readonly Feature StartPage_Premium = new Feature { Name = "StartPage_Premium", Description = "" };
        public static readonly Feature StartPage_CreateWebLibrary = new Feature { Name = "StartPage_CreateWebLibrary", Description = "" };
        public static readonly Feature StartPage_CreateIntranetLibrary = new Feature { Name = "StartPage_CreateIntranetLibrary", Description = "" };
        public static readonly Feature StartPage_JoinBundleLibrary = new Feature { Name = "StartPage_JoinBundleLibrary", Description = "" };

        public static readonly Feature Brainstorm_ExploreLibrary_Author_Documents = new Feature { Name = "Brainstorm_ExploreLibrary_Author_Documents", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_AutoTag_Documents = new Feature { Name = "Brainstorm_ExploreLibrary_AutoTag_Documents", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Tag_Documents = new Feature { Name = "Brainstorm_ExploreLibrary_Tag_Documents", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Theme_Documents = new Feature { Name = "Brainstorm_ExploreLibrary_Theme_Documents", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Theme_DocumentsInfluential = new Feature { Name = "Brainstorm_ExploreLibrary_Theme_DocumentsInfluential", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Document_CitationsOutbound = new Feature { Name = "Brainstorm_ExploreLibrary_Document_CitationsOutbound", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Document_CitationsInbound = new Feature { Name = "Brainstorm_ExploreLibrary_Document_CitationsInbound", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Document_AutoTags = new Feature { Name = "Brainstorm_ExploreLibrary_Document_AutoTags", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Document_Tags = new Feature { Name = "Brainstorm_ExploreLibrary_Document_Tags", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Document_Authors = new Feature { Name = "Brainstorm_ExploreLibrary_Document_Authors", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Document_Annotations = new Feature { Name = "Brainstorm_ExploreLibrary_Document_Annotations", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Document_Themes = new Feature { Name = "Brainstorm_ExploreLibrary_Document_Themes", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Document_Similars = new Feature { Name = "Brainstorm_ExploreLibrary_Document_Similars", Description = "" };
        public static readonly Feature Brainstorm_ExploreLibrary_Document_Relevants = new Feature { Name = "Brainstorm_ExploreLibrary_Document_Relevants", Description = "" };

        public static readonly Feature SocialMedia_AnnotationReport = new Feature { Name = "SocialMedia_AnnotationReport", Description = "" };
        public static readonly Feature SocialMedia_AnnotationEditor = new Feature { Name = "SocialMedia_AnnotationEditor", Description = "" };
        public static readonly Feature SocialMedia_AnnotationBrainstorm = new Feature { Name = "SocialMedia_AnnotationBrainstorm", Description = "" };
        public static readonly Feature SocialMedia_Snapshot = new Feature { Name = "SocialMedia_Snapshot", Description = "" };
        public static readonly Feature SocialMedia_Brainstorm = new Feature { Name = "SocialMedia_Brainstorm", Description = "" };

        public static readonly Feature Marketing_AlternativeTo = new Feature { Name = "Marketing_AlternativeTo", Description = "" };

        public static readonly Feature InCite_ChooseLibrary = new Feature { Name = "InCite_ChooseLibrary", Description = "" };
        public static readonly Feature InCite_ChooseOwnCSL = new Feature { Name = "InCite_ChooseOwnCSL", Description = "" };
        public static readonly Feature InCite_ChooseStandardCSL = new Feature { Name = "InCite_ChooseStandardCSL", Description = "" };
        public static readonly Feature InCite_BrowseZoteroCSL = new Feature { Name = "InCite_BrowseZoteroCSL", Description = "" };
        public static readonly Feature InCite_ToggleInCite = new Feature { Name = "InCite_ToggleInCite", Description = "" };
        public static readonly Feature InCite_ClickRecommended = new Feature { Name = "InCite_ClickRecommended", Description = "" };
        public static readonly Feature InCite_ClickUsedReference = new Feature { Name = "InCite_ClickUsedReference", Description = "" };
        public static readonly Feature InCite_ClickMissingReference = new Feature { Name = "InCite_ClickMissingReference", Description = "" };
        public static readonly Feature InCite_AddNewCitation = new Feature { Name = "InCite_AddNewCitation", Description = "" };
        public static readonly Feature InCite_AddNewCitation_FromDocument = new Feature { Name = "InCite_AddNewCitation_FromDocument", Description = "" };
        public static readonly Feature InCite_AddNewCitationSnippet_FromDocument = new Feature { Name = "InCite_AddNewCitationSnippet_FromDocument", Description = "" };
        public static readonly Feature InCite_AddNewCitation_FromPopup = new Feature { Name = "InCite_AddNewCitation_FromPopup", Description = "" };
        public static readonly Feature InCite_AddNewCitationSnippet_FromPopup = new Feature { Name = "InCite_AddNewCitationSnippet_FromPopup", Description = "" };
        public static readonly Feature InCite_AddNewCitation_FromAnnotationReport = new Feature { Name = "InCite_AddNewCitation_FromAnnotationReport", Description = "" };
        public static readonly Feature InCite_Popup_ChooseLibrary = new Feature { Name = "InCite_Popup_ChooseLibrary", Description = "" };

        public static readonly Feature InCite_EditCitationCluster = new Feature { Name = "InCite_EditCitationCluster", Description = "" };
        public static readonly Feature InCite_AddNewBibliography = new Feature { Name = "InCite_AddNewBibliography", Description = "" };
        public static readonly Feature InCite_AddNewCSLStats = new Feature { Name = "InCite_AddNewCSLStats", Description = "" };
        public static readonly Feature InCite_Refresh = new Feature { Name = "InCite_Refresh", Description = "" };
        public static readonly Feature InCite_AddNewCitationSnippet = new Feature { Name = "InCite_AddNewCitationSnippet", Description = "" };
        public static readonly Feature InCite_CSLEditorOpen = new Feature { Name = "InCite_CSLEditorOpen", Description = "" };
        public static readonly Feature InCite_CSLEditorWebOpen = new Feature { Name = "InCite_CSLEditorWebOpen", Description = "" };
        public static readonly Feature InCite_CSLEditorRefresh = new Feature { Name = "InCite_CSLEditorRefresh", Description = "" };
        public static readonly Feature InCite_OpenFindUsedReferences = new Feature { Name = "InCite_OpenFindUsedReferences", Description = "" };
        public static readonly Feature InCite_OpenPopup = new Feature { Name = "InCite_OpenPopup", Description = "" };
        public static readonly Feature InCite_OpenPopupFromToolbar = new Feature { Name = "InCite_OpenPopupFromToolbar", Description = "" };

        public static readonly Feature Expedition_Open_StartPage = new Feature { Name = "Expedition_Open_StartPage", Description = "" };
        public static readonly Feature Expedition_Open_Library = new Feature { Name = "Expedition_Open_Library", Description = "" };
        public static readonly Feature Expedition_Open_Document = new Feature { Name = "Expedition_Open_Document", Description = "" };
        public static readonly Feature Expedition_TopicDocument = new Feature { Name = "Expedition_TopicDocument", Description = "" };
        public static readonly Feature Expedition_ChooseLibrary = new Feature { Name = "Expedition_ChooseLibrary", Description = "" };

        public static readonly Feature Webcast = new Feature { Name = "Webcast", Description = "" };
    }
}

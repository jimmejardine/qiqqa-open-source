using System.Collections.Generic;

namespace Qiqqa.DocumentLibrary.Import.Auto.Endnote
{
    internal class EndnoteConstants
    {
        public static Dictionary<int, string> TYPES = new Dictionary<int, string>
        {
            { -1, "journal_article"          },
            { 1 , "book"                     },
            { 2 , "thesis"                   },
            { 3 , "conference_proceedings"   },
            { 4 , "personal_communication"   },
            { 5 , "newspaper_article"        },
            { 6 , "computer_program"         },
            { 7 , "book_section"             },
            { 8 , "magazine_article"         },
            { 9 , "edited_book"              },
            { 10, "report"                   },
            { 11, "map"                      },
            { 12, "audiovisual_material"     },
            { 13, "artwork"                  },
            { 15, "patent"                   },
            { 16, "web_page"                 },
            { 17, "bill"                     },
            { 19, "hearing"                  },
            { 20, "manuscript"               },
            { 21, "film_or_broadcast"        },
            { 22, "statute"                  },
            { 25, "figure"                   },
            { 26, "chart_or_table"           },
            { 27, "equation"                 },
            { 28, "electoric_article"        },
            { 29, "electronic_book"          },
            { 31, "aggregated_database"      },
            { 32, "government_document"      },
            { 33, "conference_paper"         },
            { 34, "online_multimedia"        },
            { 35, "classical_work"           },
            { 36, "legal_rule_or_regulation" },
            { 37, "unpublished_work"         },
        };

        public static readonly string[] FIELD_NAMES = new string[]
        {
            "id",
            "reference_type",
            "text_styles",
            "author",
            "year",
            "title",
            "pages",
            "secondary_title",
            "volume",
            "number",
            "number_of_volumes",
            "secondary_author",
            "place_published",
            "publisher",
            "subsidiary_author",
            "edition",
            "keywords",
            "type_of_work",
            "date",
            "abstract",
            "label",
            "url",
            "tertiary_title",
            "tertiary_author",
            "notes",
            "isbn",
            "custom_1",
            "custom_2",
            "custom_3",
            "custom_4",
            "alternate_title",
            "accession_number",
            "call_number",
            "short_title",
            "custom_5",
            "custom_6",
            "section",
            "original_publication",
            "reprint_edition",
            "reviewed_item",
            "author_address",
            "image",
            "caption",
            "custom_7",
            "electronic_resource_number",
            "link_to_pdf",
            "translated_author",
            "translated_title",
            "name_of_database",
            "database_provider",
            "research_notes",
            "language",
            "access_date",
            "last_modified_date",
        };
    }
}



/*

author         --> author artist inventor investigators
year   --> year year_of_conference
title  --> title
pages  --> pages description code_pages number_of_pages
secondary_title        --> journal periodical publication_title series_title code title_of_weblog book_title academic_department collection_title published_source periodical_title secondary_title conference_name
volume         --> volume code_volume access_year degree volume_storage_container patent_version_number
number         --> issue publication_number text_number size number bill_number date series_volume document_number folio_number application_number
number_of_volumes      --> number_of_volumes extent_of_work manuscript_number us_patent_classification document_number version session study_number
secondary_author       --> editor series_editor institution issuing_organisation producer
place_published        --> place_published country place_publised conference_location
publisher      --> publisher university institution library_archive assignee distributor
subsidiary_author      --> translator performers sponsor funding_agency
edition        --> epub_date date_published edition session description_of_material international_patent_classification version
keywords       --> keywords
type_of_work   --> type_of_article type_of_work type type_of_medium thesis_type patent_type
date   --> date date_accessed last_update_date date_of_collection
abstract       --> abstract
label  --> label
url    --> url
tertiary_title         --> volume_title legislative_body institution series_title deparment international_author website_title
tertiary_author        --> illustrator editor series_editor advisor international_title
notes  --> notes
isbn   --> issn issn_isbn isbn patent_number
custom_1       --> legal_note cast author_affiliation section year_cited place_published time_period
custom_2       --> pmcid credits issue_date date_cited year_published unit_of_observation
custom_3       --> size_length title_prefix designated_states pmcid proceedings_title data_type
custom_4       --> reviewer attorney_agent dataset
alternate_title        --> alternative_journal alternate_title abbreviated_publication abbreviation alternate_journal
accession_number       --> accession_number accessions_number
call_number    --> call_number
short_title    --> short_title
custom_5       --> format packaging_method references issue_title last_update_date
custom_6       --> nihmsid legal_status
section        --> start_page screens code_section message_number pages chapter international_patent_number epub_date original_release_date
original_publication   --> original_publication contents history priority_numbers source version_history
reprint_edition        --> reprint_edition reprint edition reporint_edition
reviewed_item  --> reviewed_item reviewer_item geographic_coverage
author_address         --> author_address inventor_address
image  -->
caption        --> caption
custom_7       --> article_number pmcid
electronic_resource_number     --> doi
link_to_pdf    -->
translated_author      --> translated_author
translated_title       --> translated_title
name_of_database       --> name_of_database name_od_database
database_provider      --> database_provider
research_notes         --> research_notes
language       --> language lanuage
access_date    --> access_date acess_date
last_modified_date     -->



*/

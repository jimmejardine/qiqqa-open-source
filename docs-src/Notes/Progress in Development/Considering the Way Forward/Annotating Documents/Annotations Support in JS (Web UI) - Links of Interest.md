# Annotations Support in JS (Web UI) :: Links of Interest

![image](https://camo.githubusercontent.com/7300f58327dbdb9beb960399aaed8977cb1f3435a556d7d6193e7785a67f2027/68747470733a2f2f6d656469612e67697068792e636f6d2f6d656469612f6c3253705a6b513058543158744b7573302f67697068792e676966)


Here's a quick dump of the links to look at later. For a dark evening...

## Note To Self

This is a semi-random amalgam of several subjects, extracted straight from the web browser tabs for further triage and investigation later on.

"Annotation" is used for various subjects:

1. "PDF human reader annotating"

   human editing annotations (**remarks**, **highlights**, etc.) over HTML pages, *images* or PDFs. 
 
   Looks like very few folks concerned themselves with generic solutions. Annotator comes closest, but currently doesn't seem to have UNDO (Ctrl+Z) functionality.
   Then there a couple of folks who did / do annotations over images.
  
   Most activity seems to be around 2014, so apparently I'm 7 years *behind the curve* 😅
  
   **NOTE** the very interesting one \[Edit: search "CVAT"] which **does not use rectangles for segmenting an image, but instead does the (IMO) only correct thing: pixel coloring = "pixel labeling"**: rectangles are what I consider the easy-for-the-computer UI and cause a horrible mess in busy layouts / images. pixel labeling is much akin to the coloring preprocessing done by comic artists (*colorists*) and other professionals in the painting/coloring industry (e.g. when coloring old B&W images)
   Anyway, PDF standard has rectangles and polygons for this and generally, that's good enough there as few page layouts go beyond the "bunch of non-overlapping rectangles" approach. 🤷
   
   ![image segmentation done right](https://github.com/openvinotoolkit/cvat/blob/develop/site/content/en/images/cvat.jpg)
  
2. "PDF text extraction / OCR"

   human (Mechanical Turk) and or "assisted" marking of image areas and tag those.

   While this is used for autodrive tests, facial/human recognition, and comparable goals, *I* want to use this tek for helping the OCR engine recognize cover pages, multi-column layouts, etc. and improve the layout a.k.a. "**segmentation**" process that is an important part of **text extraction** -- others often use the term **segmentation** in the context of **character segmentation** or **line segmentation**, which is to help the OCR engine identify the lines (reams) of text and the individual characters in there which need to be OCRed. 
   *This* is the usual context you will encounter when you look into `tesseract` et al (OCR engines). 
   
   Personally, I'm much more bothered with the extremely *mediocre* quality of the text extraction at a higher abstraction level: the recognition of content text paragraphs and general text (+ images !!!) flow across the page and *document*. 
   What is a page header, footer and how are those columns of content organized? 
   (Abstracts spanning multiple columns as they often are full width, while the text which follows the abstract is often dual or even triple column. Then there's the subject of **footnotes** and the **colophon** which often appears on the first page in lower-left corner **as part of the first column of regular text, but in cursive or different font)**: affiliation of the authors and/or sponsoring of the paper and research is often mentioned there.
   
   Properly recognizing such areas will greatly improve the general flow of the extracted text. 
  
   It is expected to also (marginally) help the OCR bottom line as proper area segmentation of a page will help *line segmentation* efforts in the OCR engine as you can now pre-process the page to ensure the OCR engine will only observe and process a specific zone this time around (selective masking + multiple runs per page to decode the parts without the risk of confusing the OCR engine).
  
3. A.I.: "(Photo) Image segmentation" (say facebook face recog) & "Word Segmentation" (NLP assist) 

   included a lot in this list of links: the various GAN, GNN, etc. approaches to image segmentation.
   
   There's several links to Chinese Word Segmentation in there, which I want to take a look at: Asian languages generally don't have the obvious word separations ("spaces") featured in all latin/euro languages, which makes Asian languages a bitch to index of efficient "googling": that's a job for SOLR but given my eagle-flight research there I know I can expect quite a bit of trouble for Asian work. Not my main priority right now, but Chinese publications are certainly featured on my list - given China's internal policies regarding nationalist chauvinism, I expect the number of Chinese-only publications that would be relevant to me to *increase* in the next decade. (Their flavor of American "patriotism" - the latter is easier for me as English is my second language. If I'd been alive a century ago, I'ld had to learn French instead to be able to communicate with the *in crowd*. Alas.)
   
   Here I have shoveled "word segmentation" and "image segmentation" / "image/object annotation" onto a single large heap, which isn't precisely correct, but good enough for now and I plonk this rough list of links below for *triage* later on.

   **NOTE / ASIDE**: what I hadn't realized before (sometimes I'm blind while looking at something 😓) is libtorch === pytorch: the C++ lib is included and fully supported by the same team there.
   
-----

# List of Links (for Triage Later On)

**NOTE**: will contain duplicates, derivatives, etc. Clean up later. Dump(ed) now.


Home - Annotator - Annotating the Web  
https://annotatorjs.org/

recogito/annotorious: A JavaScript library for image annotation  
https://github.com/recogito/annotorious

danielcebrian/richText-annotator: Plugin to use rich text in Annotator   
https://github.com/danielcebrian/richText-annotator

Daniel Cebrián Robles - Tools   
https://www.danielcebrian.com/tools

CtrHellenicStudies/OpenVideoAnnotation: Open Video Annotation Project   
https://github.com/CtrHellenicStudies/OpenVideoAnnotation

Plugins - Annotator - Annotating the Web   
https://annotatorjs.org/plugins/index.html

emory-lits-labs/annotator-marginalia: Annotator.js plugin for creating and displaying annotations in the margin of a page   
https://github.com/emory-lits-labs/annotator-marginalia

emory-lits-labs/annotator-meltdown: Annotator.js plugin for Markdown support for editing and viewing annotations   
https://github.com/emory-lits-labs/annotator-meltdown

leizongmin/js-xss: Sanitize untrusted HTML (to prevent XSS) with a configuration specified by a Whitelist   
https://github.com/leizongmin/js-xss

iphands/Meltdown: Meltdown (Markdown Extra Live Toolbox): A JQuery plugin that adds Markdown Extra live previews, and a toolbar for common markdown actions.   
https://github.com/iphands/Meltdown

Cross-Browser, Event-based, Element Resize Detection - Back Alley Coder   
http://www.backalleycoder.com/2013/03/18/cross-browser-event-based-element-resize-detection/

The Oft-Overlooked Overflow and Underflow Events - Back Alley Coder   
http://www.backalleycoder.com/2013/03/14/oft-overlooked-overflow-and-underflow-events/

Annotator demo   
http://emory-lits-labs.github.io/annotator-meltdown/demo/

openannotation/annotator: Annotation tools for the web. Select text, images, or (nearly) anything else, and add your notes.   
https://github.com/openannotation/annotator

emory-lits-labs/annotator-meltdown-zotero: Add zotero citations when editing annotations with annotator-meltdown   
https://github.com/emory-lits-labs/annotator-meltdown-zotero

Factlink/annotator-paragraph-icons: Port of the paragraph icons from the Factlink annotation library to Annotator.js   
https://github.com/Factlink/annotator-paragraph-icons

Factlink/js-library: Factlink's Javascript annotation library   
https://github.com/Factlink/js-library

Annotator with paragraph buttons demonstration   
http://factlink.github.io/annotator-paragraph-icons/

pytorch/pytorch: Tensors and Dynamic neural networks in Python with strong GPU acceleration   
https://github.com/pytorch/pytorch

CSAILVision/LabelMeAnnotationTool: Source code for the LabelMe annotation tool.   
https://github.com/CSAILVision/LabelMeAnnotationTool

LabelMe. The Open annotation tool   
http://labelme.csail.mit.edu/Release3.0/

chartjs/chartjs-plugin-annotation: Annotation plugin for Chart.js   
https://github.com/chartjs/chartjs-plugin-annotation

openvinotoolkit/cvat: Powerful and efficient Computer Vision Annotation Tool (CVAT)   
https://github.com/openvinotoolkit/cvat

jwaliszko/ExpressiveAnnotations: Annotation-based conditional validation library.   
https://github.com/jwaliszko/ExpressiveAnnotations

visipedia/annotation_tools: Visipedia Annotation Tools   
https://github.com/visipedia/annotation_tools

Machine-Learning-Tokyo/papers-with-annotations: Research papers with annotations, illustrations and explanations   
https://github.com/Machine-Learning-Tokyo/papers-with-annotations

alyssaxuu/screenity: The most powerful screen recorder & annotation tool for Chrome 🎥   
https://github.com/alyssaxuu/screenity

susielu/d3-annotation: Use d3-annotation with built-in annotation types, or extend it to make custom annotations. It is made for d3-v4 in SVG.   
https://github.com/susielu/d3-annotation

Secretmapper/react-image-annotation: An infinitely customizable image annotation library built on React   
https://github.com/Secretmapper/react-image-annotation

agentcooper/react-pdf-highlighter: Set of React components for PDF annotation   
https://github.com/agentcooper/react-pdf-highlighter

luigivieira/Facial-Landmarks-Annotation-Tool: A visual editor for manually annotating facial landmarks in images of human faces.   
https://github.com/luigivieira/Facial-Landmarks-Annotation-Tool

susielu/react-annotation: Use react-annotation with built-in annotation types, or extend it to make custom annotations. It is made for annotations in SVG.   
https://github.com/susielu/react-annotation

Seven Features You'll Want In Your Next Charting Tool   
https://www.vis4.net/blog/2015/03/seven-features-youll-wantin-your-next-charting-tool/

NVIDIA/ai-assisted-annotation-client: Client side integration example source code and libraries for AI-Assisted Annotation SDK   
https://github.com/NVIDIA/ai-assisted-annotation-client

rough-stuff/rough-notation: Create and animate hand-drawn annotations on a web page   
https://github.com/rough-stuff/rough-notation

Rough.js   
https://roughjs.com/

rough-stuff/rough: Create graphics with a hand-drawn, sketchy, appearance   
https://github.com/rough-stuff/rough

RoughNotation   
https://roughnotation.com/

w3c/web-annotation: Web Annotation Working Group repository, see README for links to specs   
https://github.com/w3c/web-annotation

midas-research/audino: Open source audio annotation tool for humans™   
https://github.com/midas-research/audino

cvondrick/vatic: Efficiently Scaling Up Video Annotation with Crowdsourced Marketplaces. IJCV 2012   
https://github.com/cvondrick/vatic

instructure/pdf-annotate.js: Annotation layer for pdf.js (no longer maintained)   
https://github.com/instructure/pdf-annotate.js

PDFJSAnnotate   
http://instructure.github.io/pdf-annotate.js/

RavishaHesh/PDFJsAnnotations: Wrapper for PDF JS to add annotations   
https://github.com/RavishaHesh/PDFJsAnnotations

taivop/awesome-data-annotation: A list of tools for annotating data, managing annotations, etc.   
https://github.com/taivop/awesome-data-annotation

synyi/poplar: A web-based annotation tool for natural language processing (NLP)   
https://github.com/synyi/poplar

hyperstudio/Annotation-Studio: An online annotation platform for teaching and learning in the humanities.   
https://github.com/hyperstudio/Annotation-Studio

CtrHellenicStudies/OpenVideoAnnotation: Open Video Annotation Project   
https://github.com/CtrHellenicStudies/OpenVideoAnnotation

Emigre/openseadragon-annotations: Draw annotations over deep zoom images   
https://github.com/Emigre/openseadragon-annotations

recogito/annotorious: A JavaScript library for image annotation   
https://github.com/recogito/annotorious

kyamagu/js-segment-annotator: Javascript image annotation tool based on image segmentation.   
https://github.com/kyamagu/js-segment-annotator

alexklaeser/imgAnnotation: A bounding box annotation tool for images (e.g., for object recognition) with alignment features.   
https://github.com/alexklaeser/imgAnnotation

openannotation/annotator: Annotation tools for the web. Select text, images, or (nearly) anything else, and add your notes.   
https://github.com/openannotation/annotator

contently/videojs-annotation-comments: A plugin for video.js to add support for timeline moment/range comments and annotations   
https://github.com/contently/videojs-annotation-comments

coin-dataset/annotation-tool   
https://github.com/coin-dataset/annotation-tool

flipbit/jquery-image-annotate: An Image Annotation plugin for jQuery   
https://github.com/flipbit/jquery-image-annotate

thiagobustamante/typescript-rest: This is a lightweight annotation-based expressjs extension for typescript.   
https://github.com/thiagobustamante/typescript-rest

reustmd/DataAnnotationsValidatorRecursive: Use DataAnnotations to validate your entire object graph   
https://github.com/reustmd/DataAnnotationsValidatorRecursive

NaturalIntelligence/imglab: To speedup and simplify image labeling/ annotation process with multiple supported formats.   
https://github.com/NaturalIntelligence/imglab

glenrobson/SimpleAnnotationServer: A simple IIIF and Mirador compatible Annotation Server   
https://github.com/glenrobson/SimpleAnnotationServer

New File   
https://github.com/jimmejardine/qiqqa-open-source/new/master/docs-src/Progress%20in%20Development/Considering%20the%20Way%20Forward

lannonbr/vscode-js-annotations: Javascript / Typescript Parameter Annotations for Visual Studio Code   
https://github.com/lannonbr/vscode-js-annotations

slashhuang/redux-annotation: 对redux源码进行中文标注，并简化代码逻辑   
https://github.com/slashhuang/redux-annotation

mitre-attack/attack-navigator: Web app that provides basic navigation and annotation of ATT&CK matrices   
https://github.com/mitre-attack/attack-navigator

blacklabel/annotations: Annotations plugin for Highstock by Black Label   
https://github.com/blacklabel/annotations

codemix/ts-sql: A SQL database implemented purely in TypeScript type annotations.   
https://github.com/codemix/ts-sql

wayou/vscode-todo-highlight: a vscode extension to highlighting todos, fixmes, and any annotations...   
https://github.com/wayou/vscode-todo-highlight

antingshen/BeaverDam: Video annotation tool for deep learning training labels   
https://github.com/antingshen/BeaverDam

paperai/pdfanno: Linguistic Annotation and Visualization Tool for PDF Documents   
https://github.com/paperai/pdfanno

src-d/code-annotation: 🐈 Code Annotation Tool   
https://github.com/src-d/code-annotation

pprp/landmark_annotation: 关键点标注软件   
https://github.com/pprp/landmark_annotation

weichx/cerialize: Easy serialization through ES7/Typescript annotations   
https://github.com/weichx/cerialize

NCIP/annotation-and-image-markup: Annotation and Image Markup (AIM) is the first project to propose and create a standard means of adding information and knowledge to an image in a clinical environment, so that image content can be easily and automatically searched.   
https://github.com/NCIP/annotation-and-image-markup

FrontendMasters/fm-snippets: Code snippets for course annotations.   
https://github.com/FrontendMasters/fm-snippets

bennylin77/react-annotation-tool: A React based image & video annotation tool   
https://github.com/bennylin77/react-annotation-tool

eBay/modanet: ModaNet: A large-scale street fashion dataset with polygon annotations   
https://github.com/eBay/modanet

spyysalo/annodoc: Annodoc annotation documentation support system   
https://github.com/spyysalo/annodoc

weitechen/anafora: Anafora is a web-based raw text annotation tool   
https://github.com/weitechen/anafora

vojirt/grabcut_annotation_tool: Semi-automatic segmentation of an object in a video sequence or a single image using Grabcut alg.   
https://github.com/vojirt/grabcut_annotation_tool

gpertea/gffcompare: classify, merge, tracking and annotation of GFF files by comparing to a reference annotation GFF   
https://github.com/gpertea/gffcompare

chrisjbryant/errant: ERRor ANnotation Toolkit: Automatically extract and classify grammatical errors in parallel original and corrected sentences.   
https://github.com/chrisjbryant/errant

scalabel/scalabel: Human-machine collaboration platform for visual data annotation   
https://github.com/scalabel/scalabel

git-hulk/libtask-annotation: libtask annotation   
https://github.com/git-hulk/libtask-annotation

alpheios-project/arethusa: Arethusa: Annotation Environment   
https://github.com/alpheios-project/arethusa

burtonator/polar-bookshelf: Polar is a personal knowledge repository for PDF and web content supporting incremental reading and document annotation.   
https://github.com/burtonator/polar-bookshelf

0xabu/pdfannots: Extracts and formats text annotations from a PDF file   
https://github.com/0xabu/pdfannots

kyamagu/bbox-annotator: A bounding box annotation widget written in CoffeeScript.   
https://github.com/kyamagu/bbox-annotator

congajs/conga-annotations: Annotation parser library for node.js   
https://github.com/congajs/conga-annotations

menpo/landmarker.io: Image and mesh annotation web application   
https://github.com/menpo/landmarker.io

dbolkensteyn/vatic.js: vatic.js A pure Javascript video annotation tool   
https://github.com/dbolkensteyn/vatic.js

highkite/pdfAnnotate: Javascript library for creating annotations in PDF documents   
https://github.com/highkite/pdfAnnotate

quantombone/LabelMeAnnotationTool: Source code for the LabelMe annotation tool.   
https://github.com/quantombone/LabelMeAnnotationTool

linkeddata/dokieli: dokieli is a clientside editor for decentralised article publishing, annotations and social interactions   
https://github.com/linkeddata/dokieli

d5555/TagEditor: 🏖TagEditor - Annotation tool for spaCy   
https://github.com/d5555/TagEditor

Submitty/pdf-annotate.js: Annotation layer for pdf.js   
https://github.com/Submitty/pdf-annotate.js

andywang0607/AutoAnnotationTool: A label tool aim to reduce semantic segmentation label time, rectangle and polygon annotation is supported   
https://github.com/andywang0607/AutoAnnotationTool

agentcooper/pdf-annotation-service: Client and service for embedding highlights into PDF documents   
https://github.com/agentcooper/pdf-annotation-service

opencast/annotation-tool: A video annotation service that is suitable for research, teaching and learning   
https://github.com/opencast/annotation-tool

tryolabs/taggerine: Annotation tool for images   
https://github.com/tryolabs/taggerine

Robinlim/node-annotation: node注解式框架   
https://github.com/Robinlim/node-annotation

guoyaohua/BERT-Chinese-Annotation: BERT 代码中文注释   
https://github.com/guoyaohua/BERT-Chinese-Annotation

vivin/regula: Regula: An annotation-based form-validation framework in Javascript   
https://github.com/vivin/regula

camicroscope/caMicroscope: Digital pathology image viewer with support for human/machine generated annotations and markups.   
https://github.com/camicroscope/caMicroscope

larrybotha/testing-javascript: Notes and annotations from Kent C. Dodds' Testing Javascript course: https://testingjavascript.com   
https://github.com/larrybotha/testing-javascript

CrowdCurio/time-series-annotator: Time series annotation library.   
https://github.com/CrowdCurio/time-series-annotator

walzimmer/3d-bat: 3D Bounding Box Annotation Tool (3D-BAT) Point cloud and Image Labeling   
https://github.com/walzimmer/3d-bat

vbauer/herald: Log annotation for logging frameworks   
https://github.com/vbauer/herald

thomas-lowry/status-annotations: A Figma plugin for indicating the status of work   
https://github.com/thomas-lowry/status-annotations

PaddlePaddle/PaddleOCR: Awesome multilingual OCR toolkits based on PaddlePaddle （practical ultra lightweight OCR system, support 80+ languages recognition, provide data annotation and synthesis tools, support training and deployment among server, mobile, embedded and IoT devices）   
https://github.com/PaddlePaddle/PaddleOCR

thomashuston/HTML5-Annotation-Tool: A package of code for quickly and easily annotating videos in a web browser   
https://github.com/thomashuston/HTML5-Annotation-Tool

gong-io/gecko: Gecko - A Tool for Effective Annotation of Human Conversations   
https://github.com/gong-io/gecko

ProjectMirador/mirador-annotations: a Mirador 3 plugin that adds annotation creation tools to the user interface   
https://github.com/ProjectMirador/mirador-annotations

whacked/calibre-viewer-annotation: attempt to add annotation / highlight capability to calibre's ebook-viewer   
https://github.com/whacked/calibre-viewer-annotation

afrmtbl/AnnotationsRestored: Brings annotation support back to YouTube   
https://github.com/afrmtbl/AnnotationsRestored

cycomanic/Menextract2pdf: Extract Mendely annotations to PDF FIles   
https://github.com/cycomanic/Menextract2pdf

poluektov/pdfkit-ink-annotations   
https://github.com/poluektov/pdfkit-ink-annotations

cloud-annotations/iris: Source for the Cloud Annotations tool   
https://github.com/cloud-annotations/iris

recogito/annotorious-openseadragon: An OpenSeadragon plugin for annotating high-res zoomable images   
https://github.com/recogito/annotorious-openseadragon

antfu/live-draw: A tool allows you to draw on screen real-time.   
https://github.com/antfu/live-draw

recogito/annotorious-openseadragon: An OpenSeadragon plugin for annotating high-res zoomable images   
https://github.com/recogito/annotorious-openseadragon

judell/AnnotationPoweredSurvey   
https://github.com/judell/AnnotationPoweredSurvey

recogito/recogito-client-core: Core functions and components for RecogitoJS and Annotorious   
https://github.com/recogito/recogito-client-core

proycon/flat: FoLiA Linguistic Annotation Tool -- Flat is a web-based linguistic annotation environment based around the FoLiA format (http://proycon.github.io/folia), a rich XML-based format for linguistic annotation. Flat allows users to view annotated FoLiA documents and enrich these documents with new annotations, a wide variety of linguistic annotation types is supported through the FoLiA paradigm.   
https://github.com/proycon/flat

EdvaldoMartins/flutter_annotations: Flutter annotations is a notebook.   
https://github.com/EdvaldoMartins/flutter_annotations

greenelab/pubtator: Retrieve and process PubTator annotations   
https://github.com/greenelab/pubtator

rwthmoodle/moodle-mod_pdfannotator: PDF Annotation Tool   
https://github.com/rwthmoodle/moodle-mod_pdfannotator

zhanxw/anno: Anno is a variant annotation tool   
https://github.com/zhanxw/anno

recogito/recogito-js: A JavaScript library for text annotation   
https://github.com/recogito/recogito-js

l3p-cv/lost: Label Objects and Save Time (LOST) - Design your own smart Image Annotation process in a web-based environment.   
https://github.com/l3p-cv/lost

goodmansasha/annotation-model: Javascript implementation of the W3C Web Annotation Data Model, useful for Web Extensions and serializing references to specific resources on a HTML page. Demo: https://goodmansasha.github.io/annotation-model/   
https://github.com/goodmansasha/annotation-model

brentp/slivar: variant expressions, annotation, and filtering for great good.   
https://github.com/brentp/slivar

gquental/node-annotation: Annotations for JavaScript   
https://github.com/gquental/node-annotation

dynilib/dynitag: Collaborative audio annotation tool   
https://github.com/dynilib/dynitag

nadeesha/saul: Experimental annotation-based javascript unit tests   
https://github.com/nadeesha/saul

w3c/web-annotation-tests: Tests for the Web Annotation Working Group's deliverables   
https://github.com/w3c/web-annotation-tests

ahmadassaf/code-notes: Tool to summarise all code annotation like TODO or FIXME   
https://github.com/ahmadassaf/code-notes

meedan/check-web: Web client for Meedan Check, a collaborative media annotation platform   
https://github.com/meedan/check-web

chartaccent/chartaccent: A chart annotation tool.   
https://github.com/chartaccent/chartaccent

Varal7/ieturk: Intuitive Annotation Tool for Information Extraction / Named Entity Recognition using localturk / Amazon Mechanical Turk   
https://github.com/Varal7/ieturk

ProjectMirador/mirador: An open-source, web-based 'multi-up' viewer that supports zoom-pan-rotate functionality, ability to display/compare simple images, and images with annotations.   
https://github.com/ProjectMirador/mirador

RichardLitt/awesome-annotation: State of the annotation field   
https://github.com/RichardLitt/awesome-annotation

mnyrop/annotate: low tech iiif annotations via jekyll 📜📝   
https://github.com/mnyrop/annotate

wiany11/jsoda: JavaScript for Object Detection Annotation   
https://github.com/wiany11/jsoda

smileinnovation/imannotate: Image annotation tool to make Machine Learning or others stuffs   
https://github.com/smileinnovation/imannotate

yg838457845/ORB_SLAM2-Chinese-annotation: ORB-SLAM2中文注释版（适用入门学习）   
https://github.com/yg838457845/ORB_SLAM2-Chinese-annotation

asyml/stave: An extensible framework for building visualization and annotation tools to enable better interaction with NLP and Artificial Intelligence. This is part of the CASL project: http://casl-project.ai/   
https://github.com/asyml/stave

npm/annotation-poller: poll for third-party annotations for entities, such as packages, and display them   
https://github.com/npm/annotation-poller

lanwuwei/Twitter-URL-Corpus: Large scale sentential paraphrases collection and annotation   
https://github.com/lanwuwei/Twitter-URL-Corpus

armollica/d3-ring-note: D3 plugin for placing circle and text annotation   
https://github.com/armollica/d3-ring-note

Attest/annotations-action: GitHub action for creating annotations from JSON file   
https://github.com/Attest/annotations-action

kyamagu/js-graph-annotator: Javascript widget to draw a graph annotation on an image.   
https://github.com/kyamagu/js-graph-annotator

aikuma/aikuma-ng: Speech annotation web app for regular folk   
https://github.com/aikuma/aikuma-ng

mitmedialab/ajl.ai: A web application for crowdsourcing image annotations.   
https://github.com/mitmedialab/ajl.ai

alvinwan/antsy3d: in-browser point cloud annotation tool for instance-level segmentation with fat markers   
https://github.com/alvinwan/antsy3d

mitmedialab/ajl.ai: A web application for crowdsourcing image annotations.   
https://github.com/mitmedialab/ajl.ai

xournalpp/xournalpp: Xournal++ is a handwriting notetaking software with PDF annotation support. Written in C++ with GTK3, supporting Linux (e.g. Ubuntu, Debian, Arch, SUSE), macOS and Windows 10. Supports pen input from devices such as Wacom Tablets.   
https://github.com/xournalpp/xournalpp

novalabs/grafana-annotations-panel: Annotations panel for Grafana   
https://github.com/novalabs/grafana-annotations-panel

anychart-solutions/anystock-drawing-tools-and-annotations-demo: AnyStock - Drawing Tools and Annotations Demo   
https://github.com/anychart-solutions/anystock-drawing-tools-and-annotations-demo

albertjuhe/annotator_view: Plug in to view annotations in a right panel.   
https://github.com/albertjuhe/annotator_view

neisbut/textAnnotator: Small and simple JS tool for making powerful underline, highlight and strike text annotations   
https://github.com/neisbut/textAnnotator

orussakovsky/annotation-UIs: Image annotation UIs (good for AMT tasks)   
https://github.com/orussakovsky/annotation-UIs

vellengs/typerx: A lightweight typescript annotation rest based extra (express、 mongoose、 angular、zorro、ng-alain ...).   
https://github.com/vellengs/typerx

smart-audio/audio_diarization_annotation: Audio Diarization Annotation tool   
https://github.com/smart-audio/audio_diarization_annotation

DigitalSlideArchive-Legacy/openseadragon-annotations   
https://github.com/DigitalSlideArchive-Legacy/openseadragon-annotations

burtonator/pdf-annotation-exporter   
https://github.com/burtonator/pdf-annotation-exporter

DataTurks/DataTurks: ML data annotations made super easy for teams. Just upload data, add your team and build training/evaluation dataset in hours.   
https://github.com/DataTurks/DataTurks

Moder112/MassAnnotationDownloader: Quick and slightly messy solution to mass download youtube annotations from many channels for archival purpouses   
https://github.com/Moder112/MassAnnotationDownloader

DataTurks/DataTurks: ML data annotations made super easy for teams. Just upload data, add your team and build training/evaluation dataset in hours.   
https://github.com/DataTurks/DataTurks

ismailmayat/MvcUmbracoDataAnnotations: Umbraco mvc annotations for localised attributes in umbraco mvc   
https://github.com/ismailmayat/MvcUmbracoDataAnnotations

ncbi-nlp/TeamTat: Text annotation tool for team collaboration   
https://github.com/ncbi-nlp/TeamTat

localjo/react-tater: A React component to add annotations to any element on a page 🥔   
https://github.com/localjo/react-tater

svrhovac/videoAnnotations: HTML5 Video player with annotations.   
https://github.com/svrhovac/videoAnnotations

oist/DenseObjectAnnotation: Tool to annotate objects in images.   
https://github.com/oist/DenseObjectAnnotation

thuiar/MMSA: CH-SIMS: A Chinese Multimodal Sentiment Analysis Dataset with Fine-grained Annotations of Modality (ACL2020)   
https://github.com/thuiar/MMSA

omriabnd/UCCA-App: A web-application for phrase-structure annotation in general, and UCCA annotation in particular   
https://github.com/omriabnd/UCCA-App

jaumard/trailpack-annotations: Add annotation support for Tails.js applications   
https://github.com/jaumard/trailpack-annotations

davosmith/moodle-uploadpdf: Moodle assignment type Uploadpdf allows online annotation of uploaded PDFs   
https://github.com/davosmith/moodle-uploadpdf

kbatyrbayev/ng-pdf-highlighter: PDF annotation with angular7   
https://github.com/kbatyrbayev/ng-pdf-highlighter

defaultstr/annotation_platform   
https://github.com/defaultstr/annotation_platform

w3c/wpub-ann: Web Annotation Extensions for Web Publications   
https://github.com/w3c/wpub-ann

tagtog/tagtog-doc: 📖 Documentation for 🍃tagtog: The Text Annotation Tool to Train AI   
https://github.com/tagtog/tagtog-doc

Helsinki-NLP/sentimentator: Tool for sentiment analysis annotation   
https://github.com/Helsinki-NLP/sentimentator

camomile-project/camomile-server: Collaborative annotation of multimedia documents   
https://github.com/camomile-project/camomile-server

cgogolin/penandpdf: Pen&PDF is a PDF viewer and annotation app for Android built on top of MuPDF.   
https://github.com/cgogolin/penandpdf

zhixuhao/unet: unet for image segmentation   
https://github.com/zhixuhao/unet

qubvel/segmentation_models: Segmentation models with pretrained backbones. Keras and TensorFlow Keras.   
https://github.com/qubvel/segmentation_models

GeorgeSeif/Semantic-Segmentation-Suite: Semantic Segmentation Suite in TensorFlow. Implement, train, and test new Semantic Segmentation models easily!   
https://github.com/GeorgeSeif/Semantic-Segmentation-Suite

divamgupta/image-segmentation-keras: Implementation of Segnet, FCN, UNet , PSPNet and other models in Keras.   
https://github.com/divamgupta/image-segmentation-keras

LeeJunHyun/Image_Segmentation: Pytorch implementation of U-Net, R2U-Net, Attention U-Net, and Attention R2U-Net.   
https://github.com/LeeJunHyun/Image_Segmentation

HRNet/HRNet-Semantic-Segmentation: The OCR approach is rephrased as Segmentation Transformer: https://arxiv.org/abs/1909.11065. This is an official implementation of semantic segmentation for HRNet. https://arxiv.org/abs/1908.07919   
https://github.com/HRNet/HRNet-Semantic-Segmentation

preritj/segmentation: Tensorflow implementation : U-net and FCN with global convolution   
https://github.com/preritj/segmentation

Tramac/awesome-semantic-segmentation-pytorch: Semantic Segmentation on PyTorch (include FCN, PSPNet, Deeplabv3, Deeplabv3+, DANet, DenseASPP, BiSeNet, EncNet, DUNet, ICNet, ENet, OCNet, CCNet, PSANet, CGNet, ESPNet, LEDNet, DFANet)   
https://github.com/Tramac/awesome-semantic-segmentation-pytorch

tangzhenyu/SemanticSegmentation_DL: Resources of semantic segmantation based on Deep Learning model   
https://github.com/tangzhenyu/SemanticSegmentation_DL

Hitachi-Automotive-And-Industry-Lab/semantic-segmentation-editor: Web labeling tool for bitmap images and point clouds   
https://github.com/Hitachi-Automotive-And-Industry-Lab/semantic-segmentation-editor

wutianyiRosun/Segmentation.X: Papers and Benchmarks about semantic segmentation, instance segmentation, panoptic segmentation and video segmentation   
https://github.com/wutianyiRosun/Segmentation.X

matterport/Mask_RCNN: Mask R-CNN for object detection and instance segmentation on Keras and TensorFlow   
https://github.com/matterport/Mask_RCNN

PaddlePaddle/PaddleDetection: Object detection and instance segmentation toolkit based on PaddlePaddle.   
https://github.com/PaddlePaddle/PaddleDetection

yaksoy/SemanticSoftSegmentation: Spectral segmentation described in Aksoy et al., "Semantic Soft Segmentation", ACM TOG (Proc. SIGGRAPH), 2018   
https://github.com/yaksoy/SemanticSoftSegmentation

xiaoyufenfei/Efficient-Segmentation-Networks: Lightweight models for real-time semantic segmentationon PyTorch (include SQNet, LinkNet, SegNet, UNet, ENet, ERFNet, EDANet, ESPNet, ESPNetv2, LEDNet, ESNet, FSSNet, CGNet, DABNet, Fast-SCNN, ContextNet, FPENet, etc.)   
https://github.com/xiaoyufenfei/Efficient-Segmentation-Networks

Moonshile/ChineseWordSegmentation: Chinese word segmentation algorithm without corpus（无需语料库的中文分词）   
https://github.com/Moonshile/ChineseWordSegmentation

lorenwel/linefit_ground_segmentation: Ground Segmentation   
https://github.com/lorenwel/linefit_ground_segmentation

facebookresearch/detectron2: Detectron2 is FAIR's next-generation platform for object detection, segmentation and other visual recognition tasks.   
https://github.com/facebookresearch/detectron2

arahusky/Tensorflow-Segmentation: Semantic image segmentation in Tensorflow   
https://github.com/arahusky/Tensorflow-Segmentation

Visceral-Project/EvaluateSegmentation: A program to evaluate the quality of image segmentations.   
https://github.com/Visceral-Project/EvaluateSegmentation

TuSimple/TuSimple-DUC: Understanding Convolution for Semantic Segmentation   
https://github.com/TuSimple/TuSimple-DUC

Tramac/Lightweight-Segmentation: Lightweight models for real-time semantic segmentation(include mobilenetv1-v3, shufflenetv1-v2, igcv3, efficientnet).   
https://github.com/Tramac/Lightweight-Segmentation

saic-vul/fbrs_interactive_segmentation: [CVPR2020] f-BRS: Rethinking Backpropagating Refinement for Interactive Segmentation https://arxiv.org/abs/2001.10331   
https://github.com/saic-vul/fbrs_interactive_segmentation

JunMa11/SegLoss: A collection of loss functions for medical image segmentation   
https://github.com/JunMa11/SegLoss

xiaohulugo/PointCloudSegmentation: A point cloud segmentation algorithm based on clustering analysis   
https://github.com/xiaohulugo/PointCloudSegmentation

lsh1994/keras-segmentation: Get started with Semantic Segmentation based on Keras, including FCN32/FCN8/SegNet/U-Net   
https://github.com/lsh1994/keras-segmentation

unicode-rs/unicode-segmentation: Grapheme Cluster and Word boundaries according to UAX#29 rules   
https://github.com/unicode-rs/unicode-segmentation

iArunava/ENet-Real-Time-Semantic-Segmentation: ENet - A Neural Net Architecture for real time Semantic Segmentation   
https://github.com/iArunava/ENet-Real-Time-Semantic-Segmentation

Yonv1943/Unsupervised-Segmentation: A high performance impermentation of Unsupervised Image Segmentation by Backpropagation - Asako Kanezaki   
https://github.com/Yonv1943/Unsupervised-Segmentation

koomri/text-segmentation: Implementation of the paper: Text Segmentation as a Supervised Learning Task   
https://github.com/koomri/text-segmentation

ckiplab/ckiptagger: CKIP Neural Chinese Word Segmentation, POS Tagging, and NER   
https://github.com/ckiplab/ckiptagger

carlren/gSLICr: gSLICr: Real-time super-pixel segmentation   
https://github.com/carlren/gSLICr

ankitdhall/imageSegmentation: Image Segmentation using Texture and Color features in C++   
https://github.com/ankitdhall/imageSegmentation

suyogduttjain/fusionseg: Video Object Segmentation   
https://github.com/suyogduttjain/fusionseg

nv-tlabs/GSCNN: Gated-Shape CNN for Semantic Segmentation (ICCV 2019)   
https://github.com/nv-tlabs/GSCNN

MegviiDetection/video_analyst: A series of basic algorithms that are useful for video understanding, including Single Object Tracking (SOT), Video Object Segmentation (VOS) and so on.   
https://github.com/MegviiDetection/video_analyst

nikhilroxtomar/UNet-Segmentation-in-Keras-TensorFlow: UNet is a fully convolutional network(FCN) that does image segmentation. Its goal is to predict each pixel's class. It is built upon the FCN and modified in a way that it yields better segmentation in medical imaging.   
https://github.com/nikhilroxtomar/UNet-Segmentation-in-Keras-TensorFlow

MarkusEich/cpf_segmentation: Constrained Plane Fitting library for unsupervised segmentation of 3D point clouds   
https://github.com/MarkusEich/cpf_segmentation

xiaomengyc/Few-Shot-Semantic-Segmentation-Papers: Few Shot Semantic Segmentation Papers   
https://github.com/xiaomengyc/Few-Shot-Semantic-Segmentation-Papers

kyamagu/js-segment-annotator: Javascript image annotation tool based on image segmentation.   
https://github.com/kyamagu/js-segment-annotator

lim-anggun/FgSegNet: FgSegNet: Foreground Segmentation Network, Foreground Segmentation Using Convolutional Neural Networks for Multiscale Feature Encoding   
https://github.com/lim-anggun/FgSegNet

twke18/Adaptive_Affinity_Fields: Adaptive Affinity Fields for Semantic Segmentation   
https://github.com/twke18/Adaptive_Affinity_Fields

hfslyc/AdvSemiSeg: Adversarial Learning for Semi-supervised Semantic Segmentation, BMVC 2018   
https://github.com/hfslyc/AdvSemiSeg

suyogduttjain/pixelobjectness: Generic Foreground Segmentation in Images   
https://github.com/suyogduttjain/pixelobjectness

Slava/label-tool: Web application for image labeling and segmentation   
https://github.com/Slava/label-tool

ethz-asl/depth_segmentation: A collection of segmentation methods working on depth images   
https://github.com/ethz-asl/depth_segmentation

yaq007/Autofocus-Layer: Autofocus Layer for Semantic Segmentation   
https://github.com/yaq007/Autofocus-Layer

LidarPerception/segmenters_lib: The LiDAR segmenters library, for segmentation-based detection.   
https://github.com/LidarPerception/segmenters_lib

hoya012/semantic-segmentation-tutorial-pytorch: A simple PyTorch codebase for semantic segmentation using Cityscapes.   
https://github.com/hoya012/semantic-segmentation-tutorial-pytorch

ozan-oktay/Attention-Gated-Networks: Use of Attention Gates in a Convolutional Neural Network / Medical Image Classification and Segmentation   
https://github.com/ozan-oktay/Attention-Gated-Networks

YimingCuiCuiCui/awesome-weakly-supervised-segmentation   
https://github.com/YimingCuiCuiCui/awesome-weakly-supervised-segmentation

AllentDan/LibtorchSegmentation: A c++ trainable semantic segmentation library based on libtorch (pytorch c++). Backbone: VGG, ResNet, ResNext. Architecture: FPN, U-Net, PAN, LinkNet, PSPNet, DeepLab-V3, DeepLab-V3+ by now.   
https://github.com/AllentDan/LibtorchSegmentation

jsbroks/coco-annotator: :pencil2: Web-based image segmentation tool for object detection, localization, and keypoints   
https://github.com/jsbroks/coco-annotator

mohaps/ImageSegmentation: Perform image segmentation and background removal in javascript using canvas element and computer vision superpixel algorithms   
https://github.com/mohaps/ImageSegmentation

kozistr/Awesome-Segmentations: Lots of semantic image segmentation implementations in Tensorflow/Keras   
https://github.com/kozistr/Awesome-Segmentations

mohaps/ImageSegmentation: Perform image segmentation and background removal in javascript using canvas element and computer vision superpixel algorithms   
https://github.com/mohaps/ImageSegmentation

cvjena/cn24: Convolutional (Patch) Networks for Semantic Segmentation   
https://github.com/cvjena/cn24

nqanh/affordance-net: AffordanceNet - Multiclass Instance Segmentation Framework - ICRA 2018   
https://github.com/nqanh/affordance-net

DIAGNijmegen/neural-odes-segmentation: Neural Ordinary Differential Equations for Semantic Segmentation of Individual Colon Glands   
https://github.com/DIAGNijmegen/neural-odes-segmentation

owang/BilateralVideoSegmentation: Implementation of Bilateral Space Video Segmentation [Maerki et al CVPR 2016]   
https://github.com/owang/BilateralVideoSegmentation

WorksApplications/Sudachi: A Japanese Tokenizer for Business   
https://github.com/WorksApplications/Sudachi

ZJULearning/pixel_link: Implementation of our paper 'PixelLink: Detecting Scene Text via Instance Segmentation' in AAAI2018   
https://github.com/ZJULearning/pixel_link

NathanZabriskie/GraphCut: Graph cut image segmentation with custom GUI.   
https://github.com/NathanZabriskie/GraphCut

cvxgrp/GGS: Greedy Gaussian Segmentation   
https://github.com/cvxgrp/GGS

MarcoForte/DeepInteractiveSegmentation: Getting to 99% Accuracy in Interactive Segmentation and Interactive Training and Architecture for Deep Object Selection   
https://github.com/MarcoForte/DeepInteractiveSegmentation

JialeCao001/D2Det: D2Det: Towards High Quality Object Detection and Instance Segmentation (CVPR2020)   
https://github.com/JialeCao001/D2Det

alchemyst/Segmentation: Timeseries segmentation library   
https://github.com/alchemyst/Segmentation

hugozanini/realtime-semantic-segmentation: Implementation of RefineNet to perform real time instance segmentation in the browser using TensorFlow.js   
https://github.com/hugozanini/realtime-semantic-segmentation

GothicAi/Instaboost: Code for ICCV2019 paper "InstaBoost: Boosting Instance Segmentation Via Probability Map Guided Copy-Pasting"   
https://github.com/GothicAi/Instaboost

kevin-ssy/FishNet: Implementation code of the paper: FishNet: A Versatile Backbone for Image, Region, and Pixel Level Prediction, NeurIPS 2018   
https://github.com/kevin-ssy/FishNet

zju3dv/snake: Code for "Deep Snake for Real-Time Instance Segmentation" CVPR 2020 oral   
https://github.com/zju3dv/snake

DBobkov/segmentation: Supplementary material for paper D. Bobkov et al. "Noise-resistant Unsupervised Object Segmentation in Multi-view Indoor Point Clouds", 2017. The paper is presented in 12th International Joint Conference on Computer Vision, Imaging and Computer Graphics Theory and Applications in Porto, Portugal in February 2017   
https://github.com/DBobkov/segmentation

tc39/proposal-intl-segmenter: Unicode text segmentation for ECMAScript   
https://github.com/tc39/proposal-intl-segmenter

daviddoria/InteractiveImageGraphCutSegmentation: An ITK/VTK implementation of graph cuts based image segmentation   
https://github.com/daviddoria/InteractiveImageGraphCutSegmentation

ansleliu/LightNet: LightNet: Light-weight Networks for Semantic Image Segmentation (Cityscapes and Mapillary Vistas Dataset)   
https://github.com/ansleliu/LightNet

ansleliu/LightNetPlusPlus: LightNet++: Boosted Light-weighted Networks for Real-time Semantic Segmentation   
https://github.com/ansleliu/LightNetPlusPlus

higherhu/BasicImageSegmentation: Some basic image segmentation algorithms, include mean-shift, graph-based, SLIC, SEEDS, GrabCut, OneCut, et al.   
https://github.com/higherhu/BasicImageSegmentation

rockkhuya/DongDu: A Vietnamese word segmentation tool   
https://github.com/rockkhuya/DongDu

torrvision/spaint: A framework for interactive, real-time 3D scene segmentation   
https://github.com/torrvision/spaint

zhangbin0917/Deep-Learning-Semantic-Segmentation: A paper list of semantic segmentation using deep learning.   
https://github.com/zhangbin0917/Deep-Learning-Semantic-Segmentation

YanchaoYang/FDA: Fourier Domain Adaptation for Semantic Segmentation   
https://github.com/YanchaoYang/FDA

fjean/pymeanshift: Python Module for Mean Shift Image Segmentation   
https://github.com/fjean/pymeanshift

livezingy/merged-characters-segmentation: Algorithm implementation of the "The Robustness of “Connecting Characters Together” CAPTCHAs". The test data set is the TAOBAO CAPTCHA that composed of blue character and white background.   
https://github.com/livezingy/merged-characters-segmentation

Captainarash/The_Holy_Book_of_X86: A simple guide to x86 architecture, assembly, memory management, paging, segmentation, SMM, BIOS....   
https://github.com/Captainarash/The_Holy_Book_of_X86

xiaolonw/nips14_loc_seg_testonly: Object Segmentation (NIPS 2014)   
https://github.com/xiaolonw/nips14_loc_seg_testonly

davidstutz/extended-berkeley-segmentation-benchmark: Extended version of the Berkeley Segmentation Benchmark [1] used for evaluation in [2].   
https://github.com/davidstutz/extended-berkeley-segmentation-benchmark

martinruenz/co-fusion: Co-Fusion: Real-time Segmentation, Tracking and Fusion of Multiple Objects   
https://github.com/martinruenz/co-fusion

jiesutd/RichWordSegmentor: Neural word segmentation with rich pretraining, code for ACL 2017 paper   
https://github.com/jiesutd/RichWordSegmentor

jponttuset/seism: Supervised Evaluation of Image Segmentation Methods   
https://github.com/jponttuset/seism

lzzcd001/OSLSM: One-shot Learning for Semantic Segmentation   
https://github.com/lzzcd001/OSLSM

davidstutz/graph-based-image-segmentation: Implementation of efficient graph-based image segmentation as proposed by Felzenswalb and Huttenlocher [1] that can be used to generate oversegmentations.   
https://github.com/davidstutz/graph-based-image-segmentation

horvitzs/Interactive_Segmentation_Models: Literature Review for Interactive annotation/segmentation   
https://github.com/horvitzs/Interactive_Segmentation_Models

abhineet123/Deep-Learning-for-Tracking-and-Detection: Collection of papers, datasets, code and other resources for object tracking and detection using deep learning   
https://github.com/abhineet123/Deep-Learning-for-Tracking-and-Detection

Horacehxw/Dynamic_ORB_SLAM2: Visual SLAM system that can identify and exclude dynamic objects.   
https://github.com/Horacehxw/Dynamic_ORB_SLAM2

nithi89/unet_darknet: U-Net implementation on darknet (semantic segmentation)   
https://github.com/nithi89/unet_darknet

mikeroberts3000/EfficientHierarchicalGraphBasedVideoSegmentationExporter: This repository contains C++ code to export the video segmentations from the system described in the paper Efficient Hierarchical Graph-Based Video Segmentation.   
https://github.com/mikeroberts3000/EfficientHierarchicalGraphBasedVideoSegmentationExporter

zaidalyafeai/zaidalyafeai.github.io: Implementation of web friendly ML models using TensorFlow.js. pix2pix, face segmentation, fast style transfer and many more ...   
https://github.com/zaidalyafeai/zaidalyafeai.github.io

xiaoyufenfei/LEDNet: LEDNet: A Lightweight Encoder-Decoder Network for Real-time Semantic Segmentation   
https://github.com/xiaoyufenfei/LEDNet

daviddoria/ClusteringSegmentation: Point cloud segmentation using radially bounded nearest neighbor clustering   
https://github.com/daviddoria/ClusteringSegmentation

daviddoria/SuperPixelSegmentation: Segment an image into super pixels   
https://github.com/daviddoria/SuperPixelSegmentation

wolfgarbe/WordSegmentationTM: Fast Word Segmentation with Triangular Matrix   
https://github.com/wolfgarbe/WordSegmentationTM

Yusheng-Xu/VGS-SVGS-Segmentation: The source code for point cloud segmentation.   
https://github.com/Yusheng-Xu/VGS-SVGS-Segmentation

cb1711/Image-Segmentation: C++ codes for image segmentation   
https://github.com/cb1711/Image-Segmentation

YexingWan/Fast-Portrait-Segmentation: The MNN base implementation of SINet for CPU realtime portrait segmentation   
https://github.com/YexingWan/Fast-Portrait-Segmentation

kerry-Cho/SemanticSegmentation-Libtorch: Libtorch Examples   
https://github.com/kerry-Cho/SemanticSegmentation-Libtorch

cheng-01037/Self-supervised-Fewshot-Medical-Image-Segmentation: [ECCV'20] Self-supervision with Superpixels: Training Few-shot Medical Image Segmentation without Annotation (code&data-processing pipeline)   
https://github.com/cheng-01037/Self-supervised-Fewshot-Medical-Image-Segmentation

ixartz/Markov-segmentation: Image segmentation with a Markov random field   
https://github.com/ixartz/Markov-segmentation

yun-liu/HFS: HFS: Hierarchical Feature Selection for Efficient Image Segmentation   
https://github.com/yun-liu/HFS

davidstutz/hierarchical-graph-based-video-segmentation: Implementation of the hierarchical graph-based video segmentation algorithm proposed by Grundmann et al. [1] based on the image segmentation algorithm by Felzenswalb and Huttenlocher [2].   
https://github.com/davidstutz/hierarchical-graph-based-video-segmentation

wctu/SEAL: Learning Superpixels with Segmentation-Aware Affinity Loss   
https://github.com/wctu/SEAL

wangzhaodong123/ORB_Segmentation: 使用语义分割将图像中的人分离出来,将原始图像中提取的ORB特征点落在人身上的剔除(用于SLAM的位姿估计).   
https://github.com/wangzhaodong123/ORB_Segmentation

bbbbyang/Mean-Shift-Segmentation: Mean Shift Filtering and Segmentation C++ (OpenCV)   
https://github.com/bbbbyang/Mean-Shift-Segmentation

davidspringer/Springer-Segmentation-Code: Heart sound segmentation code based on duration-dependant HMM   
https://github.com/davidspringer/Springer-Segmentation-Code

UMich-BipedLab/SegmentationMapping   
https://github.com/UMich-BipedLab/SegmentationMapping

suryanshkumar/GraphSegmentation: Efficient Graph-Based Image Segmentation in OpenCV(C++) for other image formats   
https://github.com/suryanshkumar/GraphSegmentation

suryanshkumar/GraphSegmentation: Efficient Graph-Based Image Segmentation in OpenCV(C++) for other image formats   
https://github.com/suryanshkumar/GraphSegmentation

huaifeng1993/DFANet: reimpliment of DFANet: Deep Feature Aggregation for Real-Time Semantic Segmentation   
https://github.com/huaifeng1993/DFANet

BIDS/BSDS500: Mirror of the Berkeley Segmentation Data Set   
https://github.com/BIDS/BSDS500

jianboqi/CSF: LiDAR point cloud ground filtering / segmentation (bare earth extraction) method based on cloth simulation   
https://github.com/jianboqi/CSF

linonetwo/segmentit: 任何 JS 环境可用的中文分词包，fork from leizongmin/node-segment   
https://github.com/linonetwo/segmentit

suhas-nithyanand/Image-Segmentation-using-Region-Growing: Image Segmentation using Region gropwing   
https://github.com/suhas-nithyanand/Image-Segmentation-using-Region-Growing

fukuball/Head-first-Chinese-text-segmentation: Head first Chinese text segmentation   
https://github.com/fukuball/Head-first-Chinese-text-segmentation

SmallMunich/FloorSegmentation: Ground Segmentation, Floor Segmentation   
https://github.com/SmallMunich/FloorSegmentation

linonetwo/segmentit: 任何 JS 环境可用的中文分词包，fork from leizongmin/node-segment   
https://github.com/linonetwo/segmentit

InsightSoftwareConsortium/ITK: Insight Toolkit (ITK) -- Official Repository. ITK builds on a proven, spatially-oriented architecture for processing, segmentation, and registration of scientific images in two, three, or more dimensions.   
https://github.com/InsightSoftwareConsortium/ITK

StevenHickson/4D_Segmentation   
https://github.com/StevenHickson/4D_Segmentation

laughtervv/SGPN: SGPN:Similarity Group Proposal Network for 3D Point Cloud Instance Segmentation, CVPR, 2018   
https://github.com/laughtervv/SGPN

ChanChiChoi/awesome-video-segmentation: papers about video segmentation   
https://github.com/ChanChiChoi/awesome-video-segmentation

johnnylu305/Simple-does-it-weakly-supervised-instance-and-semantic-segmentation: Weakly Supervised Segmentation by Tensorflow. Implements semantic segmentation in Simple Does It: Weakly Supervised Instance and Semantic Segmentation, by Khoreva et al. (CVPR 2017).   
https://github.com/johnnylu305/Simple-does-it-weakly-supervised-instance-and-semantic-segmentation

fgnt/LatticeWordSegmentation: Software to apply unsupervised word segmentation on lattices or text sequences using a nested hierarchical Pitman Yor language model   
https://github.com/fgnt/LatticeWordSegmentation

WAMAWAMA/TNSCUI2020-Seg-Rank1st: This is the source code of the 1st place solution for segmentation task in MICCAI 2020 TN-SCUI challenge.   
https://github.com/WAMAWAMA/TNSCUI2020-Seg-Rank1st

DerThorsten/nifty: A nifty library for graph based image segmentation.   
https://github.com/DerThorsten/nifty

mountain/nseg: Node.js Version of MMSG for Chinese Word Segmentation   
https://github.com/mountain/nseg

caoshen/wordseg: Chinese Word Segmentation using CRF++   
https://github.com/caoshen/wordseg

supengufo/depth_clustering_ros: Clustering Algorithm，Point Cloud Segmentation   
https://github.com/supengufo/depth_clustering_ros

alexanderrichard/squirrel: An open source deep learning action recognition and segmentation framework   
https://github.com/alexanderrichard/squirrel

walktree/libtorch-yolov3: A Libtorch implementation of the YOLO v3 object detection algorithm   
https://github.com/walktree/libtorch-yolov3

BIGBALLON/PyTorch-CPP: PyTorch C++ inference with LibTorch   
https://github.com/BIGBALLON/PyTorch-CPP

weixu000/libtorch-yolov3-deepsort   
https://github.com/weixu000/libtorch-yolov3-deepsort

yasenh/libtorch-yolov5: A LibTorch inference implementation of the yolov5   
https://github.com/yasenh/libtorch-yolov5

AllentDan/LibtorchSegmentation: A c++ trainable semantic segmentation library based on libtorch (pytorch c++). Backbone: VGG, ResNet, ResNext. Architecture: FPN, U-Net, PAN, LinkNet, PSPNet, DeepLab-V3, DeepLab-V3+ by now.   
https://github.com/AllentDan/LibtorchSegmentation

AllentDan/LibtorchTutorials: This is a code repository for pytorch c++ (or libtorch) tutorial.   
https://github.com/AllentDan/LibtorchTutorials

kerry-Cho/SemanticSegmentation-Libtorch: Libtorch Examples   
https://github.com/kerry-Cho/SemanticSegmentation-Libtorch

Jack-An/TorchDemo: Pytorch libtorch demo   
https://github.com/Jack-An/TorchDemo

dendisuhubdy/libtorch_examples: Libtorch C++ Examples   
https://github.com/dendisuhubdy/libtorch_examples

Nebula4869/YOLOv5-LibTorch: Real time object detection with deployment of YOLOv5 through LibTorch C++ API   
https://github.com/Nebula4869/YOLOv5-LibTorch

Keson96/ResNet_LibTorch: A small example of using new PyTorch C++ frontend to implement ResNet   
https://github.com/Keson96/ResNet_LibTorch

mhubii/ppo_libtorch   
https://github.com/mhubii/ppo_libtorch

threeYANG/libtorch-SSD: SSD net Training on pytorch and Implementation on libtorch   
https://github.com/threeYANG/libtorch-SSD

mhubii/libtorch_custom_dataset   
https://github.com/mhubii/libtorch_custom_dataset

vvmnnnkv/libtorchjs: Node.js N-API wrapper for libtorch   
https://github.com/vvmnnnkv/libtorchjs

ShigekiKarita/thxx: thxx: libtorch C++ API extentions and examples   
https://github.com/ShigekiKarita/thxx

LieluoboAi/radish: C++ model train&inference framework   
https://github.com/LieluoboAi/radish

threeYANG/libtorch-yolov3-tracker: Integrate libtorch-yolov3 with tracking algorithm   
https://github.com/threeYANG/libtorch-yolov3-tracker

Sanaxen/cpp_torch: It is tiny-dnn based on libtorch. Only headers without dependencies other than libtorch, deep learning framework   
https://github.com/Sanaxen/cpp_torch

rystylee/ofxLibTorch: an openFrameworks wrapper for LibTorch   
https://github.com/rystylee/ofxLibTorch

liushuan/YoloV3-Libtorch1.2-Windows10-VS2017: YoloV3 deploy with windows   
https://github.com/liushuan/YoloV3-Libtorch1.2-Windows10-VS2017

kerry-Cho/transfer-learning-Libtorch: Libtorch Examples   
https://github.com/kerry-Cho/transfer-learning-Libtorch

Maverobot/libtorch_examples: Examples of libtorch, which is C++ front end of PyTorch   
https://github.com/Maverobot/libtorch_examples

microsoft/libtorchWithDeps: C++ bindings for pytorch   
https://github.com/microsoft/libtorchWithDeps

AllentDan/LibtorchDetection: C++ trainable detection library based on libtorch (or pytorch c++). Yolov4 tiny provided now.   
https://github.com/AllentDan/LibtorchDetection

andrewssobral/dtt: A C++ header-only for data transfer between linear algebra libraries (Eigen, Armadillo, OpenCV, ArrayFire, LibTorch).   
https://github.com/andrewssobral/dtt

EmmiOcean/DDPG_LibTorch: An Implementation of the DDPG Algorithm in LibTorch   
https://github.com/EmmiOcean/DDPG_LibTorch

ericperfect/libtorch_tokenizer: BERT Tokenizer in C++   
https://github.com/ericperfect/libtorch_tokenizer

syedmohsinbukhari/resnet_libtorch: This repository saves resnet from PyTorch in TorchScript and loads it in libtorch for C++ deployment.   
https://github.com/syedmohsinbukhari/resnet_libtorch

QuantScientist/TorchRayLib: A CMake based integration of the RayLib library with the Libtorch C++ Deep Learning Library.   
https://github.com/QuantScientist/TorchRayLib

laggui/pytorch-cpp: Just messing around with PyTorch 1.0's JIT compiler and their new C++ API Libtorch.   
https://github.com/laggui/pytorch-cpp

Whu-wxy/Qt-libtorch: Qt+libtorch+opencv，image classifier   
https://github.com/Whu-wxy/Qt-libtorch

rb0518/libtorch_maskrcnn: one maskrcnn visual studio 2017 project with libtorch and opencv, codes from mlcpp   
https://github.com/rb0518/libtorch_maskrcnn

ouening/libtorch1.6-yolov3: libtorch1.6-yolov3 implementation, modified from https://github.com/walktree/libtorch-yolov3   
https://github.com/ouening/libtorch1.6-yolov3

ciderpark/Libtorch_YOLOv3_train_demo: A libtorch implementation of YOLOv3, supports training on custom dataset, evaluation and detection.   
https://github.com/ciderpark/Libtorch_YOLOv3_train_demo

Whu-wxy/PSENet-libtorch: Text detection network psenet deployed by libtorch and Qt.   
https://github.com/Whu-wxy/PSENet-libtorch

zqfang/YOLOv3_CPP: YOLOv3 C++   
https://github.com/zqfang/YOLOv3_CPP

JuliusSuryaS/pytorch_cpp_example: Example of loading pytorch model in C++ with libtorch   
https://github.com/JuliusSuryaS/pytorch_cpp_example

rockyzhengwu/libtorch-yolov4: yolov4 implement by lobtorch   
https://github.com/rockyzhengwu/libtorch-yolov4

divideconcept/PyTorch-libtorch-U-Net: A customizable 1D/2D U-Net model for libtorch (PyTorch C++ UNet)   
https://github.com/divideconcept/PyTorch-libtorch-U-Net

BuffetCodes/Transfer-Learning-Dogs-Cats-Libtorch: Transfer Learning on Dogs vs Cats dataset using PyTorch C+ API   
https://github.com/BuffetCodes/Transfer-Learning-Dogs-Cats-Libtorch

SwordSing/libtorch-yolov5   
https://github.com/SwordSing/libtorch-yolov5

Omegastick/pytorch-cpp-rl: PyTorch C++ Reinforcement Learning   
https://github.com/Omegastick/pytorch-cpp-rl

interesaaat/LibTorchSharp   
https://github.com/interesaaat/LibTorchSharp

prabhuomkar/pytorch-cpp: C++ Implementation of PyTorch Tutorials for Everyone   
https://github.com/prabhuomkar/pytorch-cpp

TengFeiHan0/deepDSO   
https://github.com/TengFeiHan0/deepDSO

huntzhan/pytorch-stateful-lstm: Pytorch LSTM implementation powered by Libtorch   
https://github.com/huntzhan/pytorch-stateful-lstm

whyang78/libtorch: libtorch c++ 学习   
https://github.com/whyang78/libtorch

runrunrun1994/Person_Segmentation: The inference implementation of the deeplabV3+ person segementation algorithm.   
https://github.com/runrunrun1994/Person_Segmentation

laizewei/libtorch-thundernet: libtorch thundernet model   
https://github.com/laizewei/libtorch-thundernet

boycehbz/libtorch-SMPL: C++ libtorch-SMPL   
https://github.com/boycehbz/libtorch-SMPL

jkulhanek/bazel-libtorch-cpp   
https://github.com/jkulhanek/bazel-libtorch-cpp

WuLoing/libtorchCppExtension: libtorch Cpp扩展与使用流程   
https://github.com/WuLoing/libtorchCppExtension

LucasWaelti/RL_Webots: Webots project to show how to use Deep Reinforcement Learning with Webots in C++.   
https://github.com/LucasWaelti/RL_Webots

zanazakaryaie/libtorch_examples: Tutorials for computer vision applications with libtorch   
https://github.com/zanazakaryaie/libtorch_examples

CivilNet/libtorch: build libtorch for various platform   
https://github.com/CivilNet/libtorch

msminhas93/libtorch-mnist-visual-studio: This repository contains a visual studio project for training a classifier on the mnist dataset using the libtorch c++ wrapper.   
https://github.com/msminhas93/libtorch-mnist-visual-studio

BuffetCodes/Linear-Regression-using-PyTorch-CPP: Implementing Linear Regression on a CSV file using PyTorch C++ Frontend API   
https://github.com/BuffetCodes/Linear-Regression-using-PyTorch-CPP

dk-liang/pytorch2libtorch: A demo for using C++ to inference the pytorch model   
https://github.com/dk-liang/pytorch2libtorch

koba-jon/pytorch_cpp: Deep Learning sample programs using PyTorch in C++   
https://github.com/koba-jon/pytorch_cpp

sainttelant/libtorchYoloV3   
https://github.com/sainttelant/libtorchYoloV3

sshuair/mmseg-libtorch: mmseg model export to libtorch examples   
https://github.com/sshuair/mmseg-libtorch

lighttransport/c-libtorch: Experimental C binding for libtorch   
https://github.com/lighttransport/c-libtorch

Biu-G/libtorch_source   
https://github.com/Biu-G/libtorch_source

wangf1978/VGGNet: VGG Net based on libtorch   
https://github.com/wangf1978/VGGNet

appcypher/libtorch: LibTorch   
https://github.com/appcypher/libtorch

joserphlin/libtorch   
https://github.com/joserphlin/libtorch

jasjuang/libtorch   
https://github.com/jasjuang/libtorch

huangheruhai/LIBTORCH   
https://github.com/huangheruhai/LIBTORCH

ZHEQIUSHUI/RFBnet-cpp-libtorch: deploy rfbnet object detection in c++   
https://github.com/ZHEQIUSHUI/RFBnet-cpp-libtorch

dnbaker/libtorch-kseq-demo: Demo using libtorch and one-hot encoding for fastx files   
https://github.com/dnbaker/libtorch-kseq-demo

gsdust/libtorchDemo   
https://github.com/gsdust/libtorchDemo

CChBen/LibtorchLoad: 使用libtorch加载网络模型   
https://github.com/CChBen/LibtorchLoad

wsx000/ROS-YOLOv4-LibTorch: this is a Deployment of YOLOv4 on ROS-Melodic with LibTorch   
https://github.com/wsx000/ROS-YOLOv4-LibTorch

YZJ6GitHub/LibTorch: C++ libPyTorch the training result of PyTorch is used by C++   
https://github.com/YZJ6GitHub/LibTorch

syys96/libtorch_learn: libtorch notes   
https://github.com/syys96/libtorch_learn

hanjialeOK/Hello-Torch: LibTorch learning~   
https://github.com/hanjialeOK/Hello-Torch

wuzuowuyou/crnn_libtorch: crnn,libtorch   
https://github.com/wuzuowuyou/crnn_libtorch

zhongqingyang/LibtorchModelEncryption   
https://github.com/zhongqingyang/LibtorchModelEncryption

cedrickchee/tch-js: A JavaScript and TypeScript port of PyTorch C++ library (libtorch) - Node.js N-API bindings for libtorch.   
https://github.com/cedrickchee/tch-js

the-robot/cpptorch: Libtorch setup in C++   
https://github.com/the-robot/cpptorch

yzqxmuex/libtorch-unet: libtorch C++ pytorch unet   
https://github.com/yzqxmuex/libtorch-unet

ActiveIntelligentSystemsLab/pytorch_enet_ros: ROS wrapper for the inference using libtorch and PyTorch models   
https://github.com/ActiveIntelligentSystemsLab/pytorch_enet_ros

jerry-jho/libtorch_examples: Some examples of libtorch   
https://github.com/jerry-jho/libtorch_examples

yzqxmuex/libtorch-fcn: libtorch C++ pytorch fcn   
https://github.com/yzqxmuex/libtorch-fcn

alexismailov2/UNetDarknetTorch: Project for training unet with libtorch in darknet format.   
https://github.com/alexismailov2/UNetDarknetTorch

shotahirama/pytorch_cpp_samples: PyTorch C++(libtorch) samples   
https://github.com/shotahirama/pytorch_cpp_samples

yytdfc/libtorch-mnasnet   
https://github.com/yytdfc/libtorch-mnasnet

1348722633/libtorch_yolov3: yolov3 with libtorch   
https://github.com/1348722633/libtorch_yolov3

shyney7/libtorch_dataloader: Example on how to create a custom dataset & dataloader with libtorch   
https://github.com/shyney7/libtorch_dataloader

Page not found · GitHub   
https://github.com/ezietsman/annotator/blob/master/src/plugin/categories.coffee

annotator/marginviewer.coffee at master · habeanf/annotator   
https://github.com/habeanf/annotator/blob/master/src/plugin/marginviewer.coffee

danielcebrian/richText-annotator: Plugin to use rich text in Annotator   
https://github.com/danielcebrian/richText-annotator

lduarte1991/tags-annotator: Plugin for Annotator that allows a tokenized system of tagging annotations and a way for them to be distinguished based on different colors for different tags   
https://github.com/lduarte1991/tags-annotator

PoeticMediaLab/Annotator-Categories: A plugin for Annotator.js that allows users to select categories for annotations, changes colors.   
https://github.com/PoeticMediaLab/Annotator-Categories

Home : Hypothesis   
https://web.hypothes.is/

edX | Free Online Courses by Harvard, MIT, & more | edX   
https://www.edx.org/

Annotation Studio |   
https://www.annotationstudio.org/

PeerLibrary   
https://peerlibrary.org/

Showcase - Annotator - Annotating the Web   
https://annotatorjs.org/showcase.html

openannotation/annotator: Annotation tools for the web. Select text, images, or (nearly) anything else, and add your notes.   
https://github.com/openannotation/annotator/

html - User annotation overlay in HTML5/JavaScript - Stack Overflow   
https://stackoverflow.com/questions/23205202/user-annotation-overlay-in-html5-javascript

Installing h in a development environment — h 0.0.2 documentation   
https://h.readthedocs.io/en/latest/developing/install/

  
   

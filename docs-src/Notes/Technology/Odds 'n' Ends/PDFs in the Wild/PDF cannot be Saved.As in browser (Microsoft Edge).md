# Odds 'n' Ends :: PDF cannot be 'Saved As' in browser (Microsoft Edge)


> **Note**: Also check these for more PDF download/fetching woes:
>
> - [[../curl - command-line and notes]] (sections about *nasty PDFs*)
> - [[Testing - Nasty URLs for PDFs]]
> - [[Testing - PDF URLs with problems]]
> 
 
Open the file [`stamp.html`](stamp.html) in your browser. It will look like it's fetching a PDF file from IEEE website (it is) and display it. The Print and Save buttons show up as usual.

But when you click the Save button or pick Save As via the right-click popup menu, you'll end up with a *HTML file* instead of the PDF proper.

The PDF is loaded in an `iframe`, which will produce an `embed` node in the HTML.

As observed in the browser's Inspector:

### The Magic Bit (?)

```
<embed id="plugin" type="application/x-google-chrome-pdf" src="https://ieeexplore.ieee.org/ielx7/6287639/8948470/09047963.pdf?tp=&amp;arnumber=9047963&amp;isnumber=8948470&amp;ref=aHR0cHM6Ly9zY2hvbGFyLmdvb2dsZS5ubC8=" stream-url="chrome-extension://mhjfbmdgcfjbbpaeojofohoefgiehjai/ba091cdc-27aa-4dda-b4a9-ab84d3d182a9" headers="Accept-Ranges: bytes
Cache-Control: private, max-age=0, no-cache, no-store, must-revalidate
Connection: keep-alive
Content-Length: 2666424
Content-Type: application/pdf
Date: Mon, 28 Sep 2020 20:18:02 GMT
Expires: Wed, 11 Jan 1984 05:00:00 GMT
Last-Modified: Sun, 19 Apr 2020 08:03:35 GMT
Pragma: no-cache
X-XSS-Protection: 1
inst: 0
licenseowner: 0
member: 0
product: UNDEFINED
roamingip: NA
" background-color="0xFFE6E6E6" first-page-separator="4" style="top: 41px;height: calc(100% - 41px)" javascript="allow" stream_timestamp="1500180854472" top-level-url="undefined" class="">
```


### The Entire Thing

(Note that the page at the time of this DOM snapshot was also saved the usual way for Web Pages in the [IEEE Xplore Full-Text PDF Embed Tag HTML file](IEEE Xplore Full-Text PDF Embed Tag.html).)

```
<html dir="ltr" lang="en"><head>
  <meta charset="utf-8">
  <link rel="import" href="elements/viewer-error-screen/viewer-error-screen.html">
  <link rel="import" href="elements/viewer-page-indicator/viewer-page-indicator.html">
  <link rel="import" href="elements/viewer-annotation-pop-up/viewer-annotation-pop-up.html">
  <link rel="import" href="elements/viewer-password-screen/viewer-password-screen.html">
  <link rel="import" href="elements/shared-vars.html">
  <link rel="import" href="edge://resources/edge_html/cr/event_target.html">
  <link rel="import" href="edge://resources/edge_html/event_tracker.html">
  <link rel="stylesheet" href="edge://resources/css/text_defaults_md.css">
  <link rel="stylesheet" href="index.css">
  <!-- Enable error-reporting -->
  <script src="pdf-error-reporting.js"></script>
<custom-style>
  <style is="custom-style">html {
  --google-red-100-rgb: 244, 199, 195;  
      --google-red-100: rgb(var(--google-red-100-rgb));
      --google-red-300-rgb: 230, 124, 115;  
      --google-red-300: rgb(var(--google-red-300-rgb));
      --google-red-500-rgb: 219, 68, 55;  
      --google-red-500: rgb(var(--google-red-500-rgb));
      --google-red-700-rgb: 197, 57, 41;  
      --google-red-700: rgb(var(--google-red-700-rgb));

      --google-blue-100-rgb: 198, 218, 252;  
      --google-blue-100: rgb(var(--google-blue-100-rgb));
      --google-blue-300-rgb: 123, 170, 247;  
      --google-blue-300: rgb(var(--google-blue-300-rgb));
      --google-blue-500-rgb: 66, 133, 244;  
      --google-blue-500: rgb(var(--google-blue-500-rgb));
      --google-blue-700-rgb: 51, 103, 214;  
      --google-blue-700: rgb(var(--google-blue-700-rgb));

      --google-green-100-rgb: 183, 225, 205;  
      --google-green-100: rgb(var(--google-green-100-rgb));
      --google-green-300-rgb: 87, 187, 138;  
      --google-green-300: rgb(var(--google-green-300-rgb));
      --google-green-500-rgb: 15, 157, 88;  
      --google-green-500: rgb(var(--google-green-500-rgb));
      --google-green-700-rgb: 11, 128, 67;  
      --google-green-700: rgb(var(--google-green-700-rgb));

      --google-yellow-100-rgb: 252, 232, 178;  
      --google-yellow-100: rgb(var(--google-yellow-100-rgb));
      --google-yellow-300-rgb: 247, 203, 77;  
      --google-yellow-300: rgb(var(--google-yellow-300-rgb));
      --google-yellow-500-rgb: 244, 180, 0;  
      --google-yellow-500: rgb(var(--google-yellow-500-rgb));
      --google-yellow-700-rgb: 240, 147, 0;  
      --google-yellow-700: rgb(var(--google-yellow-700-rgb));

      --google-grey-100-rgb: 245, 245, 245;  
      --google-grey-100: rgb(var(--google-grey-100-rgb));
      --google-grey-300-rgb: 224, 224, 224;  
      --google-grey-300: rgb(var(--google-grey-300-rgb));
      --google-grey-500-rgb: 158, 158, 158;  
      --google-grey-500: rgb(var(--google-grey-500-rgb));
      --google-grey-700-rgb: 97, 97, 97;  
      --google-grey-700: rgb(var(--google-grey-700-rgb));

      --paper-red-50: #ffebee;
      --paper-red-100: #ffcdd2;
      --paper-red-200: #ef9a9a;
      --paper-red-300: #e57373;
      --paper-red-400: #ef5350;
      --paper-red-500: #f44336;
      --paper-red-600: #e53935;
      --paper-red-700: #d32f2f;
      --paper-red-800: #c62828;
      --paper-red-900: #b71c1c;
      --paper-red-a100: #ff8a80;
      --paper-red-a200: #ff5252;
      --paper-red-a400: #ff1744;
      --paper-red-a700: #d50000;

      --paper-pink-50: #fce4ec;
      --paper-pink-100: #f8bbd0;
      --paper-pink-200: #f48fb1;
      --paper-pink-300: #f06292;
      --paper-pink-400: #ec407a;
      --paper-pink-500: #e91e63;
      --paper-pink-600: #d81b60;
      --paper-pink-700: #c2185b;
      --paper-pink-800: #ad1457;
      --paper-pink-900: #880e4f;
      --paper-pink-a100: #ff80ab;
      --paper-pink-a200: #ff4081;
      --paper-pink-a400: #f50057;
      --paper-pink-a700: #c51162;

      --paper-purple-50: #f3e5f5;
      --paper-purple-100: #e1bee7;
      --paper-purple-200: #ce93d8;
      --paper-purple-300: #ba68c8;
      --paper-purple-400: #ab47bc;
      --paper-purple-500: #9c27b0;
      --paper-purple-600: #8e24aa;
      --paper-purple-700: #7b1fa2;
      --paper-purple-800: #6a1b9a;
      --paper-purple-900: #4a148c;
      --paper-purple-a100: #ea80fc;
      --paper-purple-a200: #e040fb;
      --paper-purple-a400: #d500f9;
      --paper-purple-a700: #aa00ff;

      --paper-deep-purple-50: #ede7f6;
      --paper-deep-purple-100: #d1c4e9;
      --paper-deep-purple-200: #b39ddb;
      --paper-deep-purple-300: #9575cd;
      --paper-deep-purple-400: #7e57c2;
      --paper-deep-purple-500: #673ab7;
      --paper-deep-purple-600: #5e35b1;
      --paper-deep-purple-700: #512da8;
      --paper-deep-purple-800: #4527a0;
      --paper-deep-purple-900: #311b92;
      --paper-deep-purple-a100: #b388ff;
      --paper-deep-purple-a200: #7c4dff;
      --paper-deep-purple-a400: #651fff;
      --paper-deep-purple-a700: #6200ea;

      --paper-indigo-50: #e8eaf6;
      --paper-indigo-100: #c5cae9;
      --paper-indigo-200: #9fa8da;
      --paper-indigo-300: #7986cb;
      --paper-indigo-400: #5c6bc0;
      --paper-indigo-500: #3f51b5;
      --paper-indigo-600: #3949ab;
      --paper-indigo-700: #303f9f;
      --paper-indigo-800: #283593;
      --paper-indigo-900: #1a237e;
      --paper-indigo-a100: #8c9eff;
      --paper-indigo-a200: #536dfe;
      --paper-indigo-a400: #3d5afe;
      --paper-indigo-a700: #304ffe;

      --paper-blue-50: #e3f2fd;
      --paper-blue-100: #bbdefb;
      --paper-blue-200: #90caf9;
      --paper-blue-300: #64b5f6;
      --paper-blue-400: #42a5f5;
      --paper-blue-500: #2196f3;
      --paper-blue-600: #1e88e5;
      --paper-blue-700: #1976d2;
      --paper-blue-800: #1565c0;
      --paper-blue-900: #0d47a1;
      --paper-blue-a100: #82b1ff;
      --paper-blue-a200: #448aff;
      --paper-blue-a400: #2979ff;
      --paper-blue-a700: #2962ff;

      --paper-light-blue-50: #e1f5fe;
      --paper-light-blue-100: #b3e5fc;
      --paper-light-blue-200: #81d4fa;
      --paper-light-blue-300: #4fc3f7;
      --paper-light-blue-400: #29b6f6;
      --paper-light-blue-500: #03a9f4;
      --paper-light-blue-600: #039be5;
      --paper-light-blue-700: #0288d1;
      --paper-light-blue-800: #0277bd;
      --paper-light-blue-900: #01579b;
      --paper-light-blue-a100: #80d8ff;
      --paper-light-blue-a200: #40c4ff;
      --paper-light-blue-a400: #00b0ff;
      --paper-light-blue-a700: #0091ea;

      --paper-cyan-50: #e0f7fa;
      --paper-cyan-100: #b2ebf2;
      --paper-cyan-200: #80deea;
      --paper-cyan-300: #4dd0e1;
      --paper-cyan-400: #26c6da;
      --paper-cyan-500: #00bcd4;
      --paper-cyan-600: #00acc1;
      --paper-cyan-700: #0097a7;
      --paper-cyan-800: #00838f;
      --paper-cyan-900: #006064;
      --paper-cyan-a100: #84ffff;
      --paper-cyan-a200: #18ffff;
      --paper-cyan-a400: #00e5ff;
      --paper-cyan-a700: #00b8d4;

      --paper-teal-50: #e0f2f1;
      --paper-teal-100: #b2dfdb;
      --paper-teal-200: #80cbc4;
      --paper-teal-300: #4db6ac;
      --paper-teal-400: #26a69a;
      --paper-teal-500: #009688;
      --paper-teal-600: #00897b;
      --paper-teal-700: #00796b;
      --paper-teal-800: #00695c;
      --paper-teal-900: #004d40;
      --paper-teal-a100: #a7ffeb;
      --paper-teal-a200: #64ffda;
      --paper-teal-a400: #1de9b6;
      --paper-teal-a700: #00bfa5;

      --paper-green-50: #e8f5e9;
      --paper-green-100: #c8e6c9;
      --paper-green-200: #a5d6a7;
      --paper-green-300: #81c784;
      --paper-green-400: #66bb6a;
      --paper-green-500: #4caf50;
      --paper-green-600: #43a047;
      --paper-green-700: #388e3c;
      --paper-green-800: #2e7d32;
      --paper-green-900: #1b5e20;
      --paper-green-a100: #b9f6ca;
      --paper-green-a200: #69f0ae;
      --paper-green-a400: #00e676;
      --paper-green-a700: #00c853;

      --paper-light-green-50: #f1f8e9;
      --paper-light-green-100: #dcedc8;
      --paper-light-green-200: #c5e1a5;
      --paper-light-green-300: #aed581;
      --paper-light-green-400: #9ccc65;
      --paper-light-green-500: #8bc34a;
      --paper-light-green-600: #7cb342;
      --paper-light-green-700: #689f38;
      --paper-light-green-800: #558b2f;
      --paper-light-green-900: #33691e;
      --paper-light-green-a100: #ccff90;
      --paper-light-green-a200: #b2ff59;
      --paper-light-green-a400: #76ff03;
      --paper-light-green-a700: #64dd17;

      --paper-lime-50: #f9fbe7;
      --paper-lime-100: #f0f4c3;
      --paper-lime-200: #e6ee9c;
      --paper-lime-300: #dce775;
      --paper-lime-400: #d4e157;
      --paper-lime-500: #cddc39;
      --paper-lime-600: #c0ca33;
      --paper-lime-700: #afb42b;
      --paper-lime-800: #9e9d24;
      --paper-lime-900: #827717;
      --paper-lime-a100: #f4ff81;
      --paper-lime-a200: #eeff41;
      --paper-lime-a400: #c6ff00;
      --paper-lime-a700: #aeea00;

      --paper-yellow-50: #fffde7;
      --paper-yellow-100: #fff9c4;
      --paper-yellow-200: #fff59d;
      --paper-yellow-300: #fff176;
      --paper-yellow-400: #ffee58;
      --paper-yellow-500: #ffeb3b;
      --paper-yellow-600: #fdd835;
      --paper-yellow-700: #fbc02d;
      --paper-yellow-800: #f9a825;
      --paper-yellow-900: #f57f17;
      --paper-yellow-a100: #ffff8d;
      --paper-yellow-a200: #ffff00;
      --paper-yellow-a400: #ffea00;
      --paper-yellow-a700: #ffd600;

      --paper-amber-50: #fff8e1;
      --paper-amber-100: #ffecb3;
      --paper-amber-200: #ffe082;
      --paper-amber-300: #ffd54f;
      --paper-amber-400: #ffca28;
      --paper-amber-500: #ffc107;
      --paper-amber-600: #ffb300;
      --paper-amber-700: #ffa000;
      --paper-amber-800: #ff8f00;
      --paper-amber-900: #ff6f00;
      --paper-amber-a100: #ffe57f;
      --paper-amber-a200: #ffd740;
      --paper-amber-a400: #ffc400;
      --paper-amber-a700: #ffab00;

      --paper-orange-50: #fff3e0;
      --paper-orange-100: #ffe0b2;
      --paper-orange-200: #ffcc80;
      --paper-orange-300: #ffb74d;
      --paper-orange-400: #ffa726;
      --paper-orange-500: #ff9800;
      --paper-orange-600: #fb8c00;
      --paper-orange-700: #f57c00;
      --paper-orange-800: #ef6c00;
      --paper-orange-900: #e65100;
      --paper-orange-a100: #ffd180;
      --paper-orange-a200: #ffab40;
      --paper-orange-a400: #ff9100;
      --paper-orange-a700: #ff6500;

      --paper-deep-orange-50: #fbe9e7;
      --paper-deep-orange-100: #ffccbc;
      --paper-deep-orange-200: #ffab91;
      --paper-deep-orange-300: #ff8a65;
      --paper-deep-orange-400: #ff7043;
      --paper-deep-orange-500: #ff5722;
      --paper-deep-orange-600: #f4511e;
      --paper-deep-orange-700: #e64a19;
      --paper-deep-orange-800: #d84315;
      --paper-deep-orange-900: #bf360c;
      --paper-deep-orange-a100: #ff9e80;
      --paper-deep-orange-a200: #ff6e40;
      --paper-deep-orange-a400: #ff3d00;
      --paper-deep-orange-a700: #dd2c00;

      --paper-brown-50: #efebe9;
      --paper-brown-100: #d7ccc8;
      --paper-brown-200: #bcaaa4;
      --paper-brown-300: #a1887f;
      --paper-brown-400: #8d6e63;
      --paper-brown-500: #795548;
      --paper-brown-600: #6d4c41;
      --paper-brown-700: #5d4037;
      --paper-brown-800: #4e342e;
      --paper-brown-900: #3e2723;

      --paper-grey-50: #fafafa;
      --paper-grey-100: #f5f5f5;
      --paper-grey-200: #eeeeee;
      --paper-grey-300: #e0e0e0;
      --paper-grey-400: #bdbdbd;
      --paper-grey-500: #9e9e9e;
      --paper-grey-600: #757575;
      --paper-grey-700: #616161;
      --paper-grey-800: #424242;
      --paper-grey-900: #212121;

      --paper-blue-grey-50: #eceff1;
      --paper-blue-grey-100: #cfd8dc;
      --paper-blue-grey-200: #b0bec5;
      --paper-blue-grey-300: #90a4ae;
      --paper-blue-grey-400: #78909c;
      --paper-blue-grey-500: #607d8b;
      --paper-blue-grey-600: #546e7a;
      --paper-blue-grey-700: #455a64;
      --paper-blue-grey-800: #37474f;
      --paper-blue-grey-900: #263238;

      
      --dark-divider-opacity: 0.12;
      --dark-disabled-opacity: 0.38; 
      --dark-secondary-opacity: 0.54;
      --dark-primary-opacity: 0.87;

      
      --light-divider-opacity: 0.12;
      --light-disabled-opacity: 0.3; 
      --light-secondary-opacity: 0.7;
      --light-primary-opacity: 1.0;
}

</style>
</custom-style><custom-style>
<style is="custom-style">html {
  --edge-actionable_-_cursor:  pointer;;

    --edge-button-edge-spacing: 12px;

    
    --edge-controlled-by-spacing: 24px;

    
    --edge-icon-ripple-size: 36px;
    --edge-icon-ripple-padding: 8px;

    --edge-icon-size: 16px;

    --edge-icon-height-width_-_height:  var(--edge-icon-size); --edge-icon-height-width_-_width:  var(--edge-icon-size);

    --edge-icon-button-margin-start: 16px;

    --edge-icon-ripple-margin: 0px;

    --edge-sidebar-width: 340px;
    --edge-content-pane-padding: 0px 50px;

    
    --edge-page-host_-_color:  var(--edge-primary-text-color); --edge-page-host_-_line-height:  154%; --edge-page-host_-_overflow:  hidden; --edge-page-host_-_user-select:  text; --edge-page-host_-_background-color:  var(--edge-content-background-color);;

    --edge-paper-icon-button-margin_-_margin-inline-end:  var(--edge-icon-ripple-margin); --edge-paper-icon-button-margin_-_margin-inline-start:  var(--edge-icon-button-margin-start);

    --edge-title-text_-_color:  var(--edge-black); --edge-title-text_-_font-size:  153.846%; --edge-title-text_-_font-weight:  600; --edge-title-text_-_line-height:  28px;;

    --edge-primary-text_-_color:  var(--edge-primary-text-color); --edge-primary-text_-_font-family:  system-ui, sans-serif; --edge-primary-text_-_font-weight:  600; --edge-primary-text_-_font-size:  107.6923%; --edge-primary-text_-_line-height:  20px;

     --edge-light-secondary-text_-_color:  var(--edge-light-secondary-text-color); --edge-light-secondary-text_-_font-family:  system-ui, sans-serif; --edge-light-secondary-text_-_font-size:  92.308%; --edge-light-secondary-text_-_font-weight:  normal; --edge-light-secondary-text_-_line-height:  16px;

    --edge-secondary-text_-_color:  var(--edge-secondary-text-color); --edge-secondary-text_-_font-family:  system-ui, sans-serif; --edge-secondary-text_-_font-size:  92.308%; --edge-secondary-text_-_font-weight:  normal; --edge-secondary-text_-_line-height:  16px;

    --edge-regular-text_-_color:  var(--edge-primary-text-color); --edge-regular-text_-_font-family:  system-ui, sans-serif; --edge-regular-text_-_font-size:  107.6923%; --edge-regular-text_-_font-weight:  normal; --edge-regular-text_-_line-height:  20px;

    --edge-button-text_-_color:  var(--edge-primary-text-color); --edge-button-text_-_font-family:  system-ui, sans-serif; --edge-button-text_-_font-weight:  600; --edge-button-text_-_font-size:  92.308%; --edge-button-text_-_line-height:  16px;

    
    
    --edge-section-min-height: 40px;
    --edge-section-two-line-min-height: 60px;
    --edge-section-three-line-min-height: 76px;

    --edge-section-dialog-min-height: 36px;

    --edge-section-padding: 10px;
    --edge-section-indent-width: 0px;
    --edge-section-indent-padding: calc(
        var(--edge-section-padding) + var(--edge-section-indent-width));

    --edge-section_-_border-top:  var(--edge-separator-line); --edge-section_-_display:  flex; --edge-section_-_min-height:  var(--edge-section-min-height); --edge-section_-_padding:  0 var(--edge-section-padding);;

    --edge-text-elide_-_overflow:  hidden; --edge-text-elide_-_text-overflow:  ellipsis; --edge-text-elide_-_white-space:  nowrap;;

    --edge-tooltip_-_font-size:  92.31%; --edge-tooltip_-_font-weight:  500; --edge-tooltip_-_max-width:  330px; --edge-tooltip_-_min-width:  200px; --edge-tooltip_-_padding:  10px 8px;

    --edge-selectable-focus_-_outline:  solid 2px var(--edge-border-focused);
    --edge-separator-height: 1px;
    --edge-separator-line: var(--edge-separator-height) solid var(--edge-content-background-color);

    --edge-toolbar-overlay-animation-duration: 150ms;

    --edge-container-shadow_-_box-shadow:  inset 0 5px 6px -3px rgba(0, 0, 0, 0.4); --edge-container-shadow_-_height:  6px; --edge-container-shadow_-_left:  0; --edge-container-shadow_-_margin-bottom:  -6px; --edge-container-shadow_-_opacity:  0; --edge-container-shadow_-_pointer-events:  none; --edge-container-shadow_-_position:  relative; --edge-container-shadow_-_right:  0; --edge-container-shadow_-_top:  0px; --edge-container-shadow_-_transition:  opacity 500ms; --edge-container-shadow_-_z-index:  1;

    --edge-container-shadow-max-opacity: 1;

    
    --edge-card-border-radius: 4px;
    --edge-card-elevation_-_box-shadow:  0 0.3px 0.9px rgba(0, 0, 0, 0.11),
                  0 1.6px 3.6px rgba(0, 0, 0, 0.13);

    --edge-form-field-bottom-spacing: 18px;
    --edge-form-field-label-font-size: 0.75rem;
    --edge-form-field-label-height: 0.75rem;
    --edge-form-field-label-line-height: 0.75rem;
    --edge-form-field-label_-_color:  var(--edge-black); --edge-form-field-label_-_display:  block; --edge-form-field-label_-_font-size:  var(--edge-form-field-label-font-size); --edge-form-field-label_-_font-weight:  normal; --edge-form-field-label_-_letter-spacing:  0.4px; --edge-form-field-label_-_line-height:  var(--edge-form-field-label-line-height); --edge-form-field-label_-_margin-bottom:  10px;
    --google-blue-50: #E8F0FE;
    --google-blue-600: #1A73E8;
    
    --google-green-refresh-700: #188038;
    --google-blue-600-opacity-24: rgba(26, 115, 232, 0.24);
    
    --google-grey-refresh-100: #F1F3F4;
    --google-grey-200: #E8EAED;
    --google-grey-400: #BDC1C6;
    --google-grey-600: #80868B;
    --google-grey-600-opacity-24: rgba(128, 134, 139, 0.24);
    
    --google-grey-refresh-700: #5F6368;
    --google-grey-900: #202124;
    --google-red-600: #D93025;

    
    --edge-disabled-opacity: 0.30;

    
    --edge-blue-hover: #006CBE;
    --edge-blue-rest: #0078D4;
    --edge-blue-pressed: #1683D8;
    --edge-blue-selected: #005393;

    
    --edge-light-grey-hover: #E5E5E5;
    --edge-light-grey-rest: #EDEDED;
    --edge-light-grey-pressed: #F2F2F2;

    
    --edge-grey-hover: #EAEAEA;
    --edge-grey-rest: #E6E6E6;
    --edge-grey-pressed: #EFEFEF;
    --edge-grey-selected: #C6C6C6;

    --edge-light-border-hover: #9C9C9C;
    --edge-light-border-rest:#CECECE;
    --edge-light-border-pressed: #B5B5B5;

    --edge-border-hover:#949494;
    --edge-border-rest: #C5C5C5;
    --edge-border-pressed: #ADADAD;
    --edge-border-focused: #838383;

    
    --edge-text-light-grey-rest: #737373;

    
    --edge-text-grey-rest: #6F6F6F;

    
    --edge-text-light-blue-hover: #0078D4;
    --edge-text-light-blue-rest: #0069B9;
    --edge-text-light-blue-pressed: #2189D4;

    
    --edge-text-blue-hover: #0070C6;
    --edge-text-blue-rest: #0061AB;
    --edge-text-blue-pressed: #1081D7;

    
    --edge-error-hover: #D32121;
    --edge-error-rest: #CC0000;
    --edge-error-pressed: #D94242;

    
    --edge-success-rest: #1E8E3E;

    
    --edge-warning-rest: #F29900;

    --edge-text-green-rest: #0E700E;
    --edge-search-highlight: #FFEB3B;

    
    --edge-white: #FFFFFF;
    --edge-black: #101010;
    --edge-grey-background: #F7F7F7;
    --edge-sidebar-selected-indicator-color: #0061AB;
    --edge-sidebar-selected-button-color: #D6D6D6;
    --edge-card-color: #FFFFFF;
    --edge-card-control-white: #FFFFFF;
    --backdrop-color: rgba(0,0,0,0.6);

    
    --edge-domain-rest: #C4C4C4;

    
    --edge-background: var(--edge-grey-background);
    --edge-content-grey: var(--edge-grey-background);
    --edge-sidebar-grey: var(--edge-grey-background);
    
    --edge-grey-background-pdf: #E6E6E6;
    --edge-primary-text-color: var(--edge-black);
    --edge-light-secondary-text-color: var(--edge-text-light-grey-rest);
    --edge-secondary-text-color: var(--edge-text-grey-rest);

    --edge-sidebar-background-color: var(--edge-sidebar-grey);
    --edge-content-background-color: var(--edge-content-grey);

    
    --edge-print-preview-area-message-black: #101010;
}

@media (prefers-color-scheme: dark) {
html {
  --edge-disabled-opacity: 0.30;

      
      --edge-blue-hover: #0078D4;
      --edge-blue-rest: #006CBE;
      --edge-blue-pressed: #005CA3;
      --edge-blue-selected: #238ADA;

      
      --edge-light-grey-hover: #545454;
      --edge-light-grey-rest: #4D4D4D;
      --edge-light-grey-pressed: #484848;

      
      --edge-grey-hover: #3A3A3A;
      --edge-grey-rest: #3E3E3E;
      --edge-grey-pressed: #353535;
      --edge-grey-selected: #676767;

      --edge-light-border-hover: #999999;
      --edge-light-border-rest:#676767;
      --edge-light-border-pressed: #808080;

      --edge-border-hover:#909090;
      --edge-border-rest: #575757;
      --edge-border-pressed: #787878;
      --edge-border-focused: #909090;

      
      --edge-text-light-grey-rest: #A1A1A1;

      
      --edge-text-grey-rest: #949494;

      
      --edge-text-light-blue-hover: #5AA8E3;
      --edge-text-light-blue-rest: #7BB9E9;
      --edge-text-light-blue-pressed: #3A96DE;

      
      --edge-text-blue-hover: #429BDF;
      --edge-text-blue-rest: #63ACE5;
      --edge-text-blue-pressed: #2189DA;

      
      --edge-error-hover: #E68484;
      --edge-error-rest: #EDA5A5;
      --edge-error-pressed: #E06363;

      
      --edge-success-rest: #81C995;

      
      --edge-warning-rest: #FDD663;

      --edge-text-green-rest: #7CB77C;
      --edge-search-highlight: #FFEB3B;

      
      --edge-white: #1D1D1D;
      --edge-black: #FFFFFF;
      --edge-grey-background: #2D2D2D;
      --edge-sidebar-selected-indicator-color: #63ACE5;
      --edge-sidebar-selected-button-color: #4E4E4E;
      --edge-card-color: #363636;
      --edge-card-control-white: #252525;

      
      --edge-domain-rest: #F2F2F2;

      
      --edge-background: var(--edge-grey-background);
      --edge-content-grey: var(--edge-grey-background);
      --edge-sidebar-grey: var(--edge-grey-background);
      
      --edge-grey-background-pdf: #333333;
      --edge-primary-text-color: var(--edge-black);
      --edge-light-secondary-text-color: var(--edge-text-light-grey-rest);
      --edge-secondary-text-color: var(--edge-text-grey-rest);

      --edge-sidebar-background-color: var(--edge-sidebar-grey);
      --edge-content-background-color: var(--edge-content-grey);
}

}

</style>
</custom-style><custom-style>
  <style is="custom-style">[hidden] {
  display: none !important;
}

</style>
</custom-style><custom-style>
  <style is="custom-style">html {
  --layout_-_display:  flex;;

      --layout-inline_-_display:  inline-flex;;

      --layout-horizontal_-_display:  var(--layout_-_display); --layout-horizontal_-_flex-direction:  row;;

      --layout-horizontal-reverse_-_display:  var(--layout_-_display); --layout-horizontal-reverse_-_flex-direction:  row-reverse;;

      --layout-vertical_-_display:  var(--layout_-_display); --layout-vertical_-_flex-direction:  column;;

      --layout-vertical-reverse_-_display:  var(--layout_-_display); --layout-vertical-reverse_-_flex-direction:  column-reverse;;

      --layout-wrap_-_flex-wrap:  wrap;;

      --layout-wrap-reverse_-_flex-wrap:  wrap-reverse;;

      --layout-flex-auto_-_flex:  1 1 auto;;

      --layout-flex-none_-_flex:  none;;

      --layout-flex_-_flex:  1; --layout-flex_-_flex-basis:  0.000000001px;;

      --layout-flex-2_-_flex:  2;;

      --layout-flex-3_-_flex:  3;;

      --layout-flex-4_-_flex:  4;;

      --layout-flex-5_-_flex:  5;;

      --layout-flex-6_-_flex:  6;;

      --layout-flex-7_-_flex:  7;;

      --layout-flex-8_-_flex:  8;;

      --layout-flex-9_-_flex:  9;;

      --layout-flex-10_-_flex:  10;;

      --layout-flex-11_-_flex:  11;;

      --layout-flex-12_-_flex:  12;;

      

      --layout-start_-_align-items:  flex-start;;

      --layout-center_-_align-items:  center;;

      --layout-end_-_align-items:  flex-end;;

      --layout-baseline_-_align-items:  baseline;;

      

      --layout-start-justified_-_justify-content:  flex-start;;

      --layout-center-justified_-_justify-content:  center;;

      --layout-end-justified_-_justify-content:  flex-end;;

      --layout-around-justified_-_justify-content:  space-around;;

      --layout-justified_-_justify-content:  space-between;;

      --layout-center-center_-_align-items:  var(--layout-center_-_align-items); --layout-center-center_-_justify-content:  var(--layout-center-justified_-_justify-content);;

      

      --layout-self-start_-_align-self:  flex-start;;

      --layout-self-center_-_align-self:  center;;

      --layout-self-end_-_align-self:  flex-end;;

      --layout-self-stretch_-_align-self:  stretch;;

      --layout-self-baseline_-_align-self:  baseline;;

      

      --layout-start-aligned_-_align-content:  flex-start;;

      --layout-end-aligned_-_align-content:  flex-end;;

      --layout-center-aligned_-_align-content:  center;;

      --layout-between-aligned_-_align-content:  space-between;;

      --layout-around-aligned_-_align-content:  space-around;;

      

      --layout-block_-_display:  block;;

      --layout-invisible_-_visibility:  hidden !important;;

      --layout-relative_-_position:  relative;;

      --layout-fit_-_position:  absolute; --layout-fit_-_top:  0; --layout-fit_-_right:  0; --layout-fit_-_bottom:  0; --layout-fit_-_left:  0;;

      --layout-scroll_-_-webkit-overflow-scrolling:  touch; --layout-scroll_-_overflow:  auto;;

      --layout-fullbleed_-_margin:  0; --layout-fullbleed_-_height:  100vh;;

      

      --layout-fixed-top_-_position:  fixed; --layout-fixed-top_-_top:  0; --layout-fixed-top_-_left:  0; --layout-fixed-top_-_right:  0;;

      --layout-fixed-right_-_position:  fixed; --layout-fixed-right_-_top:  0; --layout-fixed-right_-_right:  0; --layout-fixed-right_-_bottom:  0;;

      --layout-fixed-bottom_-_position:  fixed; --layout-fixed-bottom_-_right:  0; --layout-fixed-bottom_-_bottom:  0; --layout-fixed-bottom_-_left:  0;;

      --layout-fixed-left_-_position:  fixed; --layout-fixed-left_-_top:  0; --layout-fixed-left_-_bottom:  0; --layout-fixed-left_-_left:  0;;
}

</style>
</custom-style><custom-style>
<style is="custom-style">html {
  --iron-icon-height: 20px;
    --iron-icon-width: 20px;
    --paper-icon-button_-_height:  32px; --paper-icon-button_-_padding:  6px; --paper-icon-button_-_width:  32px;;
    --paper-icon-button-ink-color: rgb(189, 189, 189);
    --viewer-icon-ink-color: rgb(189, 189, 189);
}

</style>
</custom-style><custom-style>
  <style is="custom-style">html {
  --primary-text-color: var(--light-theme-text-color);
      --primary-background-color: var(--light-theme-background-color);
      --secondary-text-color: var(--light-theme-secondary-color);
      --disabled-text-color: var(--light-theme-disabled-color);
      --divider-color: var(--light-theme-divider-color);
      --error-color: var(--paper-deep-orange-a700);

      
      --primary-color: var(--paper-indigo-500);
      --light-primary-color: var(--paper-indigo-100);
      --dark-primary-color: var(--paper-indigo-700);

      --accent-color: var(--paper-pink-a200);
      --light-accent-color: var(--paper-pink-a100);
      --dark-accent-color: var(--paper-pink-a400);


      
      --light-theme-background-color: #ffffff;
      --light-theme-base-color: #000000;
      --light-theme-text-color: var(--paper-grey-900);
      --light-theme-secondary-color: #737373;  
      --light-theme-disabled-color: #9b9b9b;  
      --light-theme-divider-color: #dbdbdb;

      
      --dark-theme-background-color: var(--paper-grey-900);
      --dark-theme-base-color: #ffffff;
      --dark-theme-text-color: #ffffff;
      --dark-theme-secondary-color: #bcbcbc;  
      --dark-theme-disabled-color: #646464;  
      --dark-theme-divider-color: #3c3c3c;

      
      --text-primary-color: var(--dark-theme-text-color);
      --default-primary-color: var(--primary-color);
}

</style>
</custom-style><custom-style>
  <style is="custom-style">html {
  --shadow-transition_-_transition:  box-shadow 0.28s cubic-bezier(0.4, 0, 0.2, 1);;

      --shadow-none_-_box-shadow:  none;;

      

      --shadow-elevation-2dp_-_box-shadow:  0 2px 2px 0 rgba(0, 0, 0, 0.14),
                    0 1px 5px 0 rgba(0, 0, 0, 0.12),
                    0 3px 1px -2px rgba(0, 0, 0, 0.2);;

      --shadow-elevation-3dp_-_box-shadow:  0 3px 4px 0 rgba(0, 0, 0, 0.14),
                    0 1px 8px 0 rgba(0, 0, 0, 0.12),
                    0 3px 3px -2px rgba(0, 0, 0, 0.4);;

      --shadow-elevation-4dp_-_box-shadow:  0 4px 5px 0 rgba(0, 0, 0, 0.14),
                    0 1px 10px 0 rgba(0, 0, 0, 0.12),
                    0 2px 4px -1px rgba(0, 0, 0, 0.4);;

      --shadow-elevation-6dp_-_box-shadow:  0 6px 10px 0 rgba(0, 0, 0, 0.14),
                    0 1px 18px 0 rgba(0, 0, 0, 0.12),
                    0 3px 5px -1px rgba(0, 0, 0, 0.4);;

      --shadow-elevation-8dp_-_box-shadow:  0 8px 10px 1px rgba(0, 0, 0, 0.14),
                    0 3px 14px 2px rgba(0, 0, 0, 0.12),
                    0 5px 5px -3px rgba(0, 0, 0, 0.4);;

      --shadow-elevation-12dp_-_box-shadow:  0 12px 16px 1px rgba(0, 0, 0, 0.14),
                    0 4px 22px 3px rgba(0, 0, 0, 0.12),
                    0 6px 7px -4px rgba(0, 0, 0, 0.4);;

      --shadow-elevation-16dp_-_box-shadow:  0 16px 24px 2px rgba(0, 0, 0, 0.14),
                    0  6px 30px 5px rgba(0, 0, 0, 0.12),
                    0  8px 10px -5px rgba(0, 0, 0, 0.4);;

      --shadow-elevation-24dp_-_box-shadow:  0 24px 38px 3px rgba(0, 0, 0, 0.14),
                    0 9px 46px 8px rgba(0, 0, 0, 0.12),
                    0 11px 15px -7px rgba(0, 0, 0, 0.4);;
}

</style>
</custom-style><style type="text/css">/** Copyright (C) Microsoft Corporation. All rights reserved.
 * Use of this source code is governed by a BSD-style license that can be
 * found in the LICENSE file.
 */

.msreadout-word-highlight {
    background: #FCFCFC !important;
    color: black !important;
    mix-blend-mode: multiply !important;
}

.msreadout-line-highlight {
    background: #C5C5C5 !important;
    color: black !important;
    mix-blend-mode: multiply !important;
}

.msreadout-background-highlight {
    background-color: rgba(184,184,184,0.82) !important;
    color: black !important;
}

.msreadout-inactive-highlight {
    background: #C5C5C5 !important;
    color: black !important;
    mix-blend-mode: multiply !important;
}

/* The below apart of the code for high-contrast handling */
@media screen and (-ms-high-contrast: active) {
    .msreadout-word-highlight {
        -ms-high-contrast-adjust: none;
        background: none !important;
        border-style: solid !important;
        border-width: medium !important;
        border-color: Highlight !important;
        mix-blend-mode: normal !important;
    }

    .msreadout-line-highlight {
        -ms-high-contrast-adjust: none;
        background: none !important;
        color: black !important;
        mix-blend-mode: multiply !important;
    }

    .msreadout-background-highlight {
        background-color: rgba(184,184,184,0.82) !important;
        color: black !important;
    }

    .msreadout-inactive-highlight {
        background: Highlight !important;
        color: HighlightText !important;
        mix-blend-mode: multiply !important;
    }
}</style><style data-jss="" data-meta="N">
.c0191 {
  width: 100%;
  z-index: 3;
  position: fixed;
}
</style><style data-jss="" data-meta="ProgressBar">
.c0190 {
  top: 41px;
  left: 0;
  right: 0;
  width: auto;
  bottom: 0;
  margin: 0;
  position: absolute;
}
</style><style data-jss="" data-meta="MSFTButton">
.c0174 {
  cursor: pointer;
  display: inline-flex;
  overflow: hidden;
  max-width: 374px;
  box-sizing: border-box;
  transition: all 0.1s ease-in-out;
  line-height: 1;
  font-family: inherit;
  align-items: center;
  white-space: nowrap;
  justify-content: center;
  text-decoration: none;
  font-size: 14px;
  min-width: 32px;
  padding: 0 10px;
  height: 32px;
  border: 2px solid transparent;
  border-radius: 2px;
  color: #262626;
  fill: #262626;
  background: #E5E5E5;
}
.c0174:hover:enabled, a.c0174:not(.c0181):hover {
  background: #DDDDDD;
}
.c0174:active:enabled, a.c0174:not(.c0181):active {
  background: #EAEAEA;
}
.c0174:focus {
  outline: none;
}
body:not(.js-focus-visible) .c0174:focus, .js-focus-visible .c0174.focus-visible, .js-focus-visible [data-focus-visible-added].c0174 {
  border-color: #838383;
}
.c0174:disabled {
}
.c0174::-moz-focus-inner {
  border: 0;
}
@media (-ms-high-contrast:active) {
  .c0174 {
    fill: ButtonText;
    color: ButtonText;
    background: ButtonFace;
    border-color: ButtonText;
    -ms-high-contrast-adjust: none;
  }
}
a.c0174:not(.c0181) {
}
@media (-ms-high-contrast:active) {
  a.c0174:not(.c0181) {
    fill: LinkText !important;
    color: LinkText !important;
    background: Window;
    border-color: LinkText !important;
  }
}
a.c0174:not(.c0181):not(.c0181):hover {
}
a.c0174:not(.c0181).c0181 {
}
@media (-ms-high-contrast:active) {
  a.c0174:not(.c0181).c0181 {
    fill: GrayText !important;
    color: GrayText !important;
    opacity: 1;
    background: ButtonFace !important;
    border-color: GrayText !important;
  }
}
a.c0174:not(.c0181).c0181:hover {
}
@media (-ms-high-contrast:active) {
  a.c0174:not(.c0181).c0181:hover {
    box-shadow: none !important;
  }
}
@media (-ms-high-contrast:active) {
  a.c0174:not(.c0181):not(.c0181):hover {
    background: ButtonFace;
    box-shadow: 0 0 0 1px inset LinkText !important;
  }
}
a.c0174:not(.c0181):not(.c0181):hover .c0182, a.c0174:not(.c0181):not(.c0181):hover .c0183 {
}
@media (-ms-high-contrast:active) {
  a.c0174:not(.c0181):not(.c0181):hover .c0182, a.c0174:not(.c0181):not(.c0181):hover .c0183 {
    fill: LinkText !important;
    color: LinkText !important;
  }
}
@media (-ms-high-contrast:active) {
  .c0174:disabled {
    fill: GrayText !important;
    color: GrayText !important;
    opacity: 1;
    background: ButtonFace !important;
    border-color: GrayText !important;
  }
}
@media (-ms-high-contrast:active) {
  body:not(.js-focus-visible) .c0174:focus, .js-focus-visible .c0174.focus-visible, .js-focus-visible [data-focus-visible-added].c0174 {
    border-color: ButtonText;
    box-shadow: 0 0 0 1px inset ButtonText;
  }
}
@media (-ms-high-contrast:active) {
  .c0174:hover:enabled, a.c0174:not(.c0181):hover {
    fill: HighlightText;
    color: HighlightText;
    background: Highlight;
  }
}
.c0174:hover:enabled .c0182, .c0174:hover:enabled .c0183, a.c0174:not(.c0181):hover .c0182, a.c0174:not(.c0181):hover .c0183 {
}
@media (-ms-high-contrast:active) {
  .c0174:hover:enabled .c0182, .c0174:hover:enabled .c0183, a.c0174:not(.c0181):hover .c0182, a.c0174:not(.c0181):hover .c0183 {
    fill: HighlightText !important;
    color: HighlightText !important;
  }
}
.c0175 {
  color: #FFFFFF;
  fill: #FFFFFF;
  background: #0078D4;
}
.c0175:hover:enabled, a.c0175:not(.c0181):hover {
  background: #006CBE;
}
.c0175:active:enabled, a.c0175:not(.c0181):active {
  background: #1683D8;
}
.c0175:focus {
  outline: none;
}
body:not(.js-focus-visible) .c0175:focus, .js-focus-visible .c0175.focus-visible, .js-focus-visible [data-focus-visible-added].c0175 {
  border-color: #838383;
  box-shadow: 0 0 0 2px inset #F2F8FD;
}
.c0175 .c0182, .c0175 .c0183 {
  fill: #FFFFFF;
}
@media (-ms-high-contrast:active) {
  .c0175 {
    fill: HighlightText;
    color: HighlightText;
    background: Highlight;
    border-color: Highlight;
    -ms-high-contrast-adjust: none;
  }
}
a.c0175:not(.c0181) {
}
a.c0175:not(.c0181) .c0182, a.c0175:not(.c0181) .c0183 {
}
@media (-ms-high-contrast:active) {
  a.c0175:not(.c0181) .c0182, a.c0175:not(.c0181) .c0183 {
    fill: LinkText !important;
    color: LinkText !important;
  }
}
@media (-ms-high-contrast:active) {
  body:not(.js-focus-visible) .c0175:focus, .js-focus-visible .c0175.focus-visible, .js-focus-visible [data-focus-visible-added].c0175 {
    border-color: ButtonText !important;
    box-shadow: 0 0 0 2px inset ButtonFace;
  }
}
@media (-ms-high-contrast:active) {
  .c0175:hover:enabled, a.c0175:not(.c0181):hover {
    fill: Highlight;
    color: Highlight;
    background: HighlightText;
    border-color: Highlight;
  }
}
.c0176 {
  background: transparent;
  border: 1px solid #B6B6B6;
  padding: 0 11px;
}
.c0176:hover:enabled, a.c0176:not(.c0181):hover {
  background: transparent;
  border: 1px solid #909090;
}
.c0176:active:enabled, a.c0176:not(.c0181):active {
  background: transparent;
  border: 1px solid #CECECE;
}
.c0176:focus {
  outline: none;
}
body:not(.js-focus-visible) .c0176:focus, .js-focus-visible .c0176.focus-visible, .js-focus-visible [data-focus-visible-added].c0176 {
  box-shadow: 0 0 0 1px #838383 inset;
  border-color: #838383;
}
@media (-ms-high-contrast:active) {
  .c0176 {
    fill: ButtonText;
    color: ButtonText;
    background: ButtonFace;
    border-color: ButtonText;
    -ms-high-contrast-adjust: none;
  }
}
@media (-ms-high-contrast:active) {
  body:not(.js-focus-visible) .c0176:focus, .js-focus-visible .c0176.focus-visible, .js-focus-visible [data-focus-visible-added].c0176 {
    border-color: ButtonText;
    box-shadow: 0 0 0 1px inset ButtonText;
  }
}
@media (-ms-high-contrast:active) {
  .c0176:hover:enabled, a.c0176:not(.c0181):hover {
    fill: HighlightText;
    color: HighlightText;
    background: Highlight;
  }
}
.c0177 {
  background-color: transparent;
  color: #0072C9;
  fill: #0072C9;
}
.c0177:focus {
  outline: none;
}
body:not(.js-focus-visible) .c0177:focus, .js-focus-visible .c0177.focus-visible, .js-focus-visible [data-focus-visible-added].c0177 {
  box-shadow: none;
  border-color: transparent;
}
.c0177 .c0180::before {
}
.c0177:hover .c0180::before {
  background: #0060A9;
}
.c0177:hover.c0181 .c0180::before {
  display: none;
}
.c0177:active .c0180::before {
  background: #097DD5;
}
.c0177.c0181, .c0177.c0181 .c0180::before {
  background-color: transparent;
}
.c0177:hover:enabled, a.c0177:not(.c0181):hover {
  background-color: transparent;
  color: #0060A9;
}
.c0177:active:enabled, a.c0177:not(.c0181):active {
  background-color: transparent;
  color: #097DD5;
  fill: #097DD5;
}
@media (-ms-high-contrast:active) {
  .c0177 {
    fill: ButtonText;
    color: ButtonText;
    border: none;
    background: ButtonFace;
    -ms-high-contrast-adjust: none;
  }
}
a.c0177:not(.c0181) {
}
a.c0177:not(.c0181):not(.c0181):hover {
}
a.c0177:not(.c0181).c0181 {
}
a.c0177:not(.c0181) .c0180::before {
}
@media (-ms-high-contrast:active) {
  a.c0177:not(.c0181) .c0180::before {
    background: transparent;
  }
}
@media (-ms-high-contrast:active) {
  a.c0177:not(.c0181).c0181 {
    fill: GrayText !important;
    color: GrayText !important;
    opacity: 1;
    background: ButtonFace !important;
    border-color: GrayText !important;
  }
}
@media (-ms-high-contrast:active) {
  a.c0177:not(.c0181):not(.c0181):hover {
    fill: LinkText !important;
    color: LinkText !important;
    box-shadow: none !important;
  }
}
a.c0177:not(.c0181):not(.c0181):hover .c0180::before {
}
@media (-ms-high-contrast:active) {
  a.c0177:not(.c0181):not(.c0181):hover .c0180::before {
    background: LinkText !important;
  }
}
@media (-ms-high-contrast:active) {
  .c0177:hover:enabled, a.c0177:not(.c0181):hover {
    fill: Highlight !important;
    color: Highlight !important;
  }
}
.c0177:hover:enabled .c0182, .c0177:hover:enabled .c0183, a.c0177:not(.c0181):hover .c0182, a.c0177:not(.c0181):hover .c0183 {
  fill: #0060A9;
}
@media (-ms-high-contrast:active) {
  .c0177:hover:enabled .c0182, .c0177:hover:enabled .c0183, a.c0177:not(.c0181):hover .c0182, a.c0177:not(.c0181):hover .c0183 {
    fill: Highlight !important;
    color: Highlight !important;
  }
}
@media (-ms-high-contrast:active) {
  .c0177:hover .c0180::before {
    background: Highlight;
  }
}
@media (-ms-high-contrast:active) {
  .c0177 .c0180::before {
    background: ButtonText;
  }
}
@media (-ms-high-contrast:active) {
  body:not(.js-focus-visible) .c0177:focus, .js-focus-visible .c0177.focus-visible, .js-focus-visible [data-focus-visible-added].c0177 {
    fill: Highlight !important;
    color: Highlight !important;
  }
}
body:not(.js-focus-visible) .c0177:focus .c0180::before, .js-focus-visible .c0177.focus-visible .c0180::before, .js-focus-visible [data-focus-visible-added].c0177 .c0180::before {
  background: #262626;
  height: 2px;
}
@media (-ms-high-contrast:active) {
  body:not(.js-focus-visible) .c0177:focus .c0180::before, .js-focus-visible .c0177.focus-visible .c0180::before, .js-focus-visible [data-focus-visible-added].c0177 .c0180::before {
    background: Highlight;
  }
}
.c0178 {
  min-width: 74px;
  padding-left: 0;
  border-width: 0;
  padding-right: 0;
  justify-content: flex-start;
  background-color: transparent;
  color: #0072C9;
  fill: #0072C9;
}
.c0178:focus {
  outline: none;
}
body:not(.js-focus-visible) .c0178:focus, .js-focus-visible .c0178.focus-visible, .js-focus-visible [data-focus-visible-added].c0178 {
  box-shadow: none;
  border-color: transparent;
}
.c0178 .c0180::before {
}
.c0178:hover .c0180::before {
  background: #0060A9;
}
.c0178:hover.c0181 .c0180::before {
  display: none;
}
.c0178:active .c0180::before {
  background: #097DD5;
}
.c0178.c0181, .c0178.c0181 .c0180::before {
  background-color: transparent;
}
.c0178:hover:enabled, a.c0178:not(.c0181):hover {
  background-color: transparent;
  color: #0060A9;
}
.c0178:active:enabled, a.c0178:not(.c0181):active {
  background-color: transparent;
  color: #097DD5;
  fill: #097DD5;
}
@media (-ms-high-contrast:active) {
  .c0178 {
    fill: ButtonText;
    color: ButtonText;
    border: none;
    background: ButtonFace;
    -ms-high-contrast-adjust: none;
  }
}
a.c0178:not(.c0181) {
}
a.c0178:not(.c0181):not(.c0181):hover {
}
a.c0178:not(.c0181).c0181 {
}
@media (-ms-high-contrast:active) {
  a.c0178:not(.c0181).c0181 {
    fill: GrayText !important;
    color: GrayText !important;
    opacity: 1;
    background: ButtonFace !important;
    border-color: GrayText !important;
  }
}
@media (-ms-high-contrast:active) {
  a.c0178:not(.c0181):not(.c0181):hover {
    fill: LinkText !important;
    color: LinkText !important;
    box-shadow: none !important;
  }
}
a.c0178:not(.c0181):not(.c0181):hover .c0180::before {
}
@media (-ms-high-contrast:active) {
  a.c0178:not(.c0181):not(.c0181):hover .c0180::before {
    background: LinkText !important;
  }
}
@media (-ms-high-contrast:active) {
  .c0178:hover:enabled, a.c0178:not(.c0181):hover {
    fill: Highlight !important;
    color: Highlight !important;
  }
}
.c0178:hover:enabled .c0182, .c0178:hover:enabled .c0183, a.c0178:not(.c0181):hover .c0182, a.c0178:not(.c0181):hover .c0183 {
  fill: #0060A9;
}
@media (-ms-high-contrast:active) {
  .c0178:hover:enabled .c0182, .c0178:hover:enabled .c0183, a.c0178:not(.c0181):hover .c0182, a.c0178:not(.c0181):hover .c0183 {
    fill: Highlight !important;
    color: Highlight !important;
  }
}
@media (-ms-high-contrast:active) {
  .c0178:hover .c0180::before {
    background: Highlight;
  }
}
@media (-ms-high-contrast:active) {
  .c0178 .c0180::before {
    background: ButtonText;
  }
}
@media (-ms-high-contrast:active) {
  body:not(.js-focus-visible) .c0178:focus, .js-focus-visible .c0178.focus-visible, .js-focus-visible [data-focus-visible-added].c0178 {
    fill: Highlight !important;
    color: Highlight !important;
  }
}
body:not(.js-focus-visible) .c0178:focus .c0180::before, .js-focus-visible .c0178.focus-visible .c0180::before, .js-focus-visible [data-focus-visible-added].c0178 .c0180::before {
  background: #262626;
  height: 2px;
}
@media (-ms-high-contrast:active) {
  body:not(.js-focus-visible) .c0178:focus .c0180::before, .js-focus-visible .c0178.focus-visible .c0180::before, .js-focus-visible [data-focus-visible-added].c0178 .c0180::before {
    background: Highlight;
  }
}
.c0179 {
  background: #F7F7F7;
}
.c0179:hover:enabled, a.c0179:not(.c0181):hover {
  background-color: #EAEAEA;
}
.c0179:active:enabled, a.c0179:not(.c0181):active {
  background-color: #EFEFEF;
}
.c0179:focus {
  outline: none;
}
body:not(.js-focus-visible) .c0179:focus, .js-focus-visible .c0179.focus-visible, .js-focus-visible [data-focus-visible-added].c0179 {
  border-color: #838383;
}
@media (-ms-high-contrast:active) {
  .c0179 {
    fill: ButtonText;
    color: ButtonText;
    border: none;
    background: ButtonFace;
    -ms-high-contrast-adjust: none;
  }
}
@media (-ms-high-contrast:active) {
  body:not(.js-focus-visible) .c0179:focus, .js-focus-visible .c0179.focus-visible, .js-focus-visible [data-focus-visible-added].c0179 {
    border-color: ButtonText;
    box-shadow: 0 0 0 1px inset ButtonText;
  }
}
@media (-ms-high-contrast:active) {
  .c0179:hover:enabled, a.c0179:not(.c0181):hover {
    fill: HighlightText;
    color: HighlightText;
    background: Highlight;
  }
}
.c0180 {
  position: relative;
}
.c0180::before {
  width: 100%;
  bottom: -3px;
  content: '';
  display: block;
  position: absolute;
  height: 1px;
  left: 0;
}
.c0180 svg {
  width: 16px;
  height: 16px;
}
.c0181 {
  cursor: not-allowed !important;
  opacity: 0.3;
}
@media (-ms-high-contrast:active) {
  .c0181 {
    fill: GrayText !important;
    color: GrayText !important;
    opacity: 1;
    background: ButtonFace !important;
    border-color: GrayText !important;
  }
}
.c0181 .c0182, .c0181 .c0183 {
}
@media (-ms-high-contrast:active) {
  .c0181 .c0182, .c0181 .c0183 {
    fill: GrayText !important;
    color: GrayText !important;
    opacity: 1;
  }
}
.c0182 {
  width: 16px;
  height: 16px;
  margin-right: 12px;
}
.c0183 {
  width: 16px;
  height: 16px;
  margin-left: 12px;
}
</style><style data-jss="" data-meta="PdfBarButton">
.c0166 {
  width: 40px;
  height: 32px;
  cursor: default;
}
.c0166:disabled {
  cursor: default !important;
}
@media (-ms-high-contrast:active) {
  .c0166 {
    background: Window;
    -ms-high-contrast-adjust: none;
  }
}
.c0166 + .c0173 {
}
.c0166 + .c0173:focus {
  outline: none;
}
body:not(.js-focus-visible) .c0166 + .c0173:focus, .js-focus-visible .c0166 + .c0173.focus-visible, .js-focus-visible [data-focus-visible-added].c0166 + .c0173 {
  background-color: transparent;
}
.c0167 {
  width: auto;
  user-select: none;
  padding-left: 12px;
  padding-right: 12px;
}
.c0168 {
  display: flex;
  align-items: center;
  justify-content: center;
}
.c0169 {
  width: 16px;
  height: 16px;
}
.c0170 {
  white-space: nowrap;
  margin-left: 8px;
  font-weight: 400;
}
.c0171 {
  cursor: default !important;
}
.c0172 {
  background-color: #E5E5E5;
}
.c0172:hover:enabled {
  background-color: #E5E5E5;
}
@media (prefers-color-scheme: dark) {
  .c0172 {
    background-color: #4D4D4D;
  }
  .c0172:hover:enabled {
    background-color: #4D4D4D;
  }
}
@media (-ms-high-contrast:active) {
  .c0172 {
    fill: HighlightText;
    color: HighlightText;
    background-color: Highlight;
  }
  .c0172:hover:enabled {
    fill: HighlightText;
    color: HighlightText;
    background-color: Highlight;
  }
}
.c0173 {
  width: 100%;
  height: 2px;
  position: absolute;
  margin-top: -6px;
  border-bottom-left-radius: 2px;
  border-bottom-right-radius: 2px;
  background-color: #0072C9;
}
@media (-ms-high-contrast:active) {
  .c0173 {
    background: Highlight;
    -ms-high-contrast-adjust: none;
  }
}
</style><style data-jss="" data-meta="MSFTButton - jssStyleSheet">
.c0184 {
  min-width: unset;
  margin-top: 4px;
  margin-bottom: 4px;
}
</style><style data-jss="" data-meta="BaseTypography">
.c0156 {
  margin-top: 0;
  transition: all 0.2s ease-in-out;
  margin-bottom: 0;
  color: #262626;
}
.c0157 {
  font-size: 60px;
  line-height: 72px;
}
.c0158 {
  font-size: 46px;
  line-height: 56px;
}
.c0159 {
  font-size: 34px;
  line-height: 44px;
}
.c0160 {
  font-size: 28px;
  line-height: 36px;
}
.c0161 {
  font-size: 20px;
  line-height: 28px;
}
.c0162 {
  font-size: 16px;
  line-height: 24px;
}
.c0163 {
  font-size: 14px;
  line-height: 20px;
}
.c0164 {
  font-size: 12px;
  line-height: 16px;
}
.c0165 {
  font-size: 10px;
  line-height: 16px;
}
</style><style data-jss="" data-meta="PdfBarButton - jssStyleSheet">
.c0185 {
  border-top-right-radius: 0;
  border-bottom-right-radius: 0;
  padding-right: 8px;
}
.c0186 {
  border-bottom-right-radius: 0;
}
</style><style data-jss="" data-meta="PdfBarButton - jssStyleSheet">
.c0187 {
  width: 28px;
  border-top-left-radius: 0;
  border-bottom-left-radius: 0;
  padding-left: 8px;
  padding-right: 8px;
}
.c0188 {
  width: 12px;
  height: 12px;
}
.c0188 svg {
  width: 12px;
  height: 12px;
}
.c0189 {
  border-bottom-left-radius: 0;
}
</style><style data-jss="" data-meta="MSFTTextField">
.c0154 {
  margin: 0;
  box-sizing: border-box;
  transition: all 0.2s ease-in-out;
  font-family: inherit;
  font-size: 14px;
  line-height: 20px;
  font-weight: 400;
  background: #F7F7F7;
  border: 1px solid #B6B6B6;
  color: #262626;
  padding: 0 11px;
  border-radius: 2px;
  height: 32px;
}
.c0154:hover:enabled {
  background: #F7F7F7;
  border-color: #909090;
}
.c0154:active:enabled {
  background: #F7F7F7;
  border-color: #CECECE;
}
.c0154:focus:enabled {
  outline: none;
  box-shadow: 0 0 0 1px #838383 inset;
  border-color: #838383;
}
.c0154:disabled {
  cursor: not-allowed !important;
  opacity: 0.3;
}
.c0154::placeholder {
  color: #717171;
}
@media (-ms-high-contrast:active) {
  .c0154 {
    color: ButtonText;
    background: ButtonFace;
    border-color: ButtonText;
    -ms-high-contrast-adjust: none;
  }
}
@media (-ms-high-contrast:active) {
  .c0154::placeholder {
    color: GrayText;
  }
}
@media (-ms-high-contrast:active) {
  .c0154:disabled {
    fill: GrayText !important;
    color: GrayText !important;
    opacity: 1;
    background: ButtonFace !important;
    border-color: GrayText !important;
  }
}
@media (-ms-high-contrast:active) {
  .c0154:focus:enabled {
    border-color: ButtonText;
    box-shadow: 0 0 0 1px inset ButtonText;
  }
}
@media (-ms-high-contrast:active) {
  .c0154:hover:enabled {
    background: ButtonFace;
    border: 1px solid Highlight;
  }
}
.c0155 {
  margin: 0;
  box-sizing: border-box;
  transition: all 0.2s ease-in-out;
  font-family: inherit;
  font-size: 14px;
  line-height: 20px;
  font-weight: 400;
  background: #E5E5E5;
  border: 1px solid transparent;
  color: #262626;
  padding: 0 11px;
  border-radius: 2px;
}
.c0155:hover:enabled {
  border-color: transparent;
  background: #DDDDDD;
}
.c0155:active:enabled {
  border-color: transparent;
}
.c0155:focus:enabled {
  border-color: #838383;
}
.c0155:disabled {
  cursor: not-allowed !important;
  opacity: 0.3;
}
.c0155::placeholder {
  color: #666666;
}
@media (-ms-high-contrast:active) {
  .c0155 {
    background: ButtonFace;
    border-color: ButtonText;
    -ms-high-contrast-adjust: none;
  }
}
@media (-ms-high-contrast:active) {
  .c0155::placeholder {
    color: GrayText;
  }
}
@media (-ms-high-contrast:active) {
  .c0155:disabled {
    fill: GrayText !important;
    color: GrayText !important;
    opacity: 1;
    background: ButtonFace !important;
    border-color: GrayText !important;
  }
}
@media (-ms-high-contrast:active) {
  .c0155:hover:enabled {
    background: ButtonFace;
    border: 1px solid Highlight;
  }
}
</style><style data-jss="" data-meta="PageSelector">
.c0151 {
  display: flex;
  align-items: center;
}
.c0152 {
  width: 40px;
  height: 32px;
  text-align: center;
  padding-left: 4px;
  border-radius: 2px;
  padding-right: 4px;
  font-size: 12px;
  line-height: 16px;
}
.c0152:disabled {
  cursor: default !important;
}
.c0153 {
  margin: 0 8px 0 10px;
  user-select: none;
  white-space: nowrap;
}
</style><style data-jss="" data-meta="Toolbar">
.c0142 {
  width: 100%;
  display: flex;
  padding: 0 8px;
  position: relative;
  box-sizing: border-box;
  align-items: center;
  justify-content: space-between;
  height: 41px;
  border-bottom: 1px solid #BEBEBE;
  box-shadow: 0px 4.8px 10.8px rgba(0,0,0,0.13), 0px 0px 4.7px rgba(0,0,0,0.11);
}
@media (max-width: 1470px) {
  .c0142 {
    justify-content: unset;
  }
}
@media (-ms-high-contrast:active) {
  .c0142 {
    border: 1px solid WindowText;
    background: Background;
  }
}
.c0143 {
  top: 0;
  position: fixed;
  box-shadow: none;
}
.c0144 {
  display: flex;
  align-items: center;
}
.c0145 {
  width: 100%;
  display: grid;
  align-items: center;
  grid-template-columns: auto 420px;
}
@media (max-width: 1470px) {
  .c0145 {
    grid-template-columns: auto;
  }
}
@media (max-width: 790px) {
  .c0145 {
    display: none;
  }
}
.c0146 {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}
@media (max-width: 1470px) {
  .c0146 {
    margin: 0 auto;
  }
}
.c0147 {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: flex-end;
}
@media (max-width: 790px) {
  .c0147 {
    display: none;
  }
}
</style><style data-jss="" data-meta="w">
.c013 {
  position: fixed;
  transition: transform .5s cubic-bezier(0.25,0.10,0.25,1.00);
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI';
  padding-right: 4px;
  direction: ltr;
}
.c014 {
  position: static;
}
.c015 {
  display: flex;
}
.c016 {
  display: flex;
}
.c017 {
  display: flex;
  align-items: center;
}
.c018 {
  visibility: hidden;
}
.c019 {
  box-shadow: none;
  border-bottom: 0;
  background-color: transparent !important;
}
.c0110 {
  z-index: 1;
}
.c0111 {
  z-index: 2;
}
.c0112 {
  background-color: #E5E5E5;
}
.c0112:hover:enabled {
  background-color: #E5E5E5;
}
@media (prefers-color-scheme: dark) {
  .c0112 {
    background-color: #4D4D4D;
  }
  .c0112:hover:enabled {
    background-color: #4D4D4D;
  }
}
@media (-ms-high-contrast:active) {
  .c0112 {
    fill: HighlightText;
    color: HighlightText;
    background-color: Highlight;
  }
  .c0112:hover:enabled {
    fill: HighlightText;
    color: HighlightText;
    background-color: Highlight;
  }
}
.c0113 {
  width: 280px;
  z-index: 1;
  position: absolute;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI';
  background-color: inherit;
  box-shadow: 0px 12.8px 28.8px rgba(0,0,0,0.13908558841119492), 0px 0px 9.2px rgba(0,0,0,0.1176878055787034);
  padding-top: 8px;
  padding-bottom: 8px;
  direction: ltr;
}
@media not all and (prefers-reduced-motion: reduce) {
  .c0113 {
    border-radius: 4px;
  }
}
@media (-ms-high-contrast:active) {
  .c0113 {
    border: 1px solid WindowText;
    background-color: Window;
  }
}
.c0114 {
  transform: translateY(-100%);
  box-shadow: none;
}
.c0115 {
  height: 100%;
}
.c0116 {
  width: 16px;
  height: 16px;
}
.c0117 {
}
@media not all and (-ms-high-contrast:active) {
  .c0117 {
    fill: #F29900;
  }
@media (prefers-color-scheme: dark) {
  .c0117 {
    fill: #FDD663;
  }
}
}
.c0118 {
}
.c0118 .shouldAnimate {
  opacity: 0;
}
.c0119 {
}
.c0119 .shouldAnimate {
  opacity: 100;
  transition: opacity 500ms cubic-bezier(0.25, 0.10, 0.25, 1.00);
  transition-delay: 300ms;
}
.c0120 {
}
.c0120 .shouldAnimate {
  opacity: 100;
}
.c0121 {
}
.c0121 .shouldAnimate {
  opacity: 0;
  transition: opacity 500ms cubic-bezier(0.25, 0.10, 0.25, 1.00);
}
.c0122 {
}
.c0122 .shouldAnimate {
  transform: translateY(-100%);
}
.c0123 {
}
.c0123 .shouldAnimate {
  transform: translate(0px, 0px);
  transition: transform 500ms cubic-bezier(0.25, 0.10, 0.25, 1.00);
  transition-delay: 300ms;
}
.c0124 {
}
.c0124 .shouldAnimate {
  transform: translate(0px, 0px);
  transition: transform 500ms cubic-bezier(0.25, 0.10, 0.25, 1.00);
}
.c0125 {
}
.c0125 .shouldAnimate {
  transform: translate(0px, 0px);
}
.c0126 {
}
.c0126 .shouldAnimate {
  transform: translateY(-100%);
  transition: transform 500ms cubic-bezier(0.25, 0.10, 0.25, 1.00);
}
.c0127 {
}
.c0128 {
  menu-top-padding: 3;
}
.c0129 {
}
.c0130 {
  display: flex;
  align-items: center;
}
.c0131 {
  display: flex;
  align-items: center;
}
.c0132 {
  display: flex;
  align-items: center;
}
.c0133 {
  display: flex;
  align-items: center;
}
.c0134 {
  display: flex;
  align-items: center;
}
.c0135 {
  margin: 0 4px;
  width: 1px;
  height: 16px;
  background: #B6B6B6;
}
@media (-ms-high-contrast:active) {
  .c0135 {
    background: WindowText;
    -ms-high-contrast-adjust: none;
  }
}
.c0136 {
}
.c0137 {
  box-shadow: none;
}
.c0138 {
  box-shadow: none;
}
.c0139 {
  height: 16px;
  width: 16px;
}
.c0140 {
}
.c0140 path:last-child {
  fill: #107C10;
}
@media (-ms-high-contrast:active) {
  .c0140 path:last-child {
    fill: Window;
    stroke: WindowText;
    -ms-high-contrast-adjust: none;
  }
}
.c0141 {
}
.c0141 path:last-child {
  fill: #FFB900;
}
@media (-ms-high-contrast:active) {
  .c0141 path:last-child {
    fill: Window;
    stroke: WindowText;
    -ms-high-contrast-adjust: none;
  }
}
</style><style data-jss="" data-meta="Toolbar - jssStyleSheet">
.c0148 {
  position: relative;
  transition: transform .5s cubic-bezier(0.25,0.10,0.25,1.00);
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI';
  padding-right: 4px;
  direction: ltr;
}
.c0149 {
  display: flex;
}
.c0150 {
  display: flex;
}
</style><style data-jss="" data-meta="PdfBarTransitionWrapper">
.c011 {
  transform: translateY(-100%);
}
.c012 {
  transform: translateY(0%);
}
</style><title>Twitter and Research: A Systematic Literature Review Through Text Mining</title></head>
<body class="js-focus-visible">

  <!-- the contents of ui-container are react-based PDF UI components -->
  <div id="ui-container"><div style="background-color: rgb(247, 247, 247);"><div id="toolbar"><div class="c012" style="position: relative; width: 100%; height: auto; transition: transform 0.5s cubic-bezier(0.25, 0.1, 0.25, 1) 0s;"><div style="background-color: rgb(255, 255, 255);"><div class="c0142 c0148 c0129 c0137" role="region" aria-label="" style="background-color: rgb(247, 247, 247);"><div class="c0145 c0149"><div style="visibility: visible;"><div class="c0130"><div class="c0131"><div id="pageselector-component-container" class="c0151"><input data-element-focusable="true" id="pageselector" class="c0154 c0152" type="text" value="1" title="Page number (Ctrl+Alt+G)" aria-label="Go to any page between 1 and 20"><p class="c0156 c0164 c0153" id="pagelength" aria-hidden="true" title="Number of pages">of 20</p></div></div></div></div></div><div class="c0146"></div><div class="c0147 c0150"><div style="visibility: visible;"><div class="c0130"><div class="c0132"><div class="c0130"><div style="position: relative;"><button id="zoom-out" class="c0174 c0179 c0184 c0166" data-element-focusable="true" title="Zoom out (Ctrl+Minus key)"><span class="c0180"><div class="c0168"><div class="c0169"><svg class="" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 2048 2048" width="16" height="16"><path d="M0 896h1920v128H0V896z"></path></svg></div></div></span></button></div><div style="position: relative;"><button id="zoom-in" class="c0174 c0179 c0184 c0166" data-element-focusable="true" title="Zoom in (Ctrl+Plus key)"><span class="c0180"><div class="c0168"><div class="c0169"><svg class="" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 2048 2048" width="16" height="16"><path d="M1920 896v128h-896v896H896v-896H0V896h896V0h128v896h896z"></path></svg></div></div></span></button></div><div style="position: relative;"><button id="rotate" class="c0174 c0179 c0184 c0166" data-element-focusable="true" title="Rotate (Ctrl+])"><span class="c0180"><div class="c0168"><div class="c0169"><svg viewBox="0 0 16 16" width="16px" height="16px" xmlns="http://www.w3.org/2000/svg"><path d="M8 6C8.27604 6 8.53385 6.05208 8.77344 6.15625C9.01823 6.26042 9.23177 6.40365 9.41406 6.58594C9.59635 6.76823 9.73958 6.98177 9.84375 7.22656C9.94792 7.46615 10 7.72396 10 8C10 8.27604 9.94792 8.53646 9.84375 8.78125C9.73958 9.02083 9.59635 9.23177 9.41406 9.41406C9.23177 9.59635 9.01823 9.73958 8.77344 9.84375C8.53385 9.94792 8.27604 10 8 10C7.72396 10 7.46354 9.94792 7.21875 9.84375C6.97917 9.73958 6.76823 9.59635 6.58594 9.41406C6.40365 9.23177 6.26042 9.02083 6.15625 8.78125C6.05208 8.53646 6 8.27604 6 8C6 7.72396 6.05208 7.46615 6.15625 7.22656C6.26042 6.98177 6.40365 6.76823 6.58594 6.58594C6.76823 6.40365 6.97917 6.26042 7.21875 6.15625C7.46354 6.05208 7.72396 6 8 6ZM8 9C8.14062 9 8.27083 8.97396 8.39062 8.92188C8.51042 8.86979 8.61458 8.79948 8.70312 8.71094C8.79688 8.61719 8.86979 8.51042 8.92188 8.39062C8.97396 8.27083 9 8.14062 9 8C9 7.85938 8.97396 7.72917 8.92188 7.60938C8.86979 7.48958 8.79688 7.38542 8.70312 7.29688C8.61458 7.20312 8.51042 7.13021 8.39062 7.07812C8.27083 7.02604 8.14062 7 8 7C7.85938 7 7.72917 7.02604 7.60938 7.07812C7.48958 7.13021 7.38281 7.20312 7.28906 7.29688C7.20052 7.38542 7.13021 7.48958 7.07812 7.60938C7.02604 7.72917 7 7.85938 7 8C7 8.14062 7.02604 8.27083 7.07812 8.39062C7.13021 8.51042 7.20052 8.61719 7.28906 8.71094C7.38281 8.79948 7.48958 8.86979 7.60938 8.92188C7.72917 8.97396 7.85938 9 8 9ZM16 8C16 8.72917 15.9036 9.4375 15.7109 10.125C15.5182 10.8125 15.2422 11.4609 14.8828 12.0703C14.5286 12.6745 14.0964 13.2266 13.5859 13.7266C13.0755 14.2266 12.5 14.651 11.8594 15H14V16H10V12H11V14.3203C11.6094 14.0339 12.1589 13.6693 12.6484 13.2266C13.1432 12.7839 13.5651 12.2891 13.9141 11.7422C14.263 11.1901 14.5312 10.5964 14.7188 9.96094C14.9062 9.32552 15 8.67188 15 8C15 7.35938 14.9167 6.74219 14.75 6.14844C14.5833 5.54948 14.3464 4.99219 14.0391 4.47656C13.737 3.95573 13.3724 3.48177 12.9453 3.05469C12.5182 2.6276 12.0443 2.26302 11.5234 1.96094C11.0078 1.65365 10.4505 1.41667 9.85156 1.25C9.25781 1.08333 8.64062 1 8 1C7.35938 1 6.73958 1.08333 6.14062 1.25C5.54688 1.41667 4.98958 1.65365 4.46875 1.96094C3.95312 2.26302 3.48177 2.6276 3.05469 3.05469C2.6276 3.48177 2.26042 3.95573 1.95312 4.47656C1.65104 4.99219 1.41667 5.54948 1.25 6.14844C1.08333 6.74219 1 7.35938 1 8H0C0 7.26562 0.09375 6.55729 0.28125 5.875C0.473958 5.19271 0.742188 4.55469 1.08594 3.96094C1.4349 3.36719 1.85156 2.82812 2.33594 2.34375C2.82552 1.85417 3.36719 1.4375 3.96094 1.09375C4.55469 0.744792 5.1901 0.476562 5.86719 0.289062C6.54948 0.0963542 7.26042 0 8 0C8.73438 0 9.44271 0.0963542 10.125 0.289062C10.8073 0.476562 11.4453 0.744792 12.0391 1.09375C12.6328 1.4375 13.1719 1.85417 13.6562 2.34375C14.1458 2.82812 14.5625 3.36719 14.9062 3.96094C15.2552 4.55469 15.5234 5.19271 15.7109 5.875C15.9036 6.55729 16 7.26562 16 8Z"></path></svg></div></div></span></button></div><div style="position: relative;"><button id="pagefit" class="c0174 c0179 c0184 c0166" data-element-focusable="true" title="Fit to width (Ctrl+\)"><span class="c0180"><div class="c0168"><div class="c0169"><svg id="fit-to-width-icon" viewBox="0 0 16 10" width="16px" height="16px" xmlns="http://www.w3.org/2000/svg"><path d="M0 0H16V12H0V0ZM15 11V1H1V11H15ZM4.33594 6.66406L3.66406 7.33594L1.82031 5.5L3.66406 3.66406L4.33594 4.33594L3.67188 5H7V6H3.67188L4.33594 6.66406ZM11.6641 6.66406L12.3281 6H9V5H12.3281L11.6641 4.33594L12.3359 3.66406L14.1797 5.5L12.3359 7.33594L11.6641 6.66406Z"></path></svg></div></div></span></button></div></div></div><div class="c0133"><div class="c0135"></div><div class="c0130"><div style="position: relative;"><button id="read-aloud" class="c0174 c0179 c0184 c0166 c0167" data-element-focusable="true" title="Read aloud"><span class="c0180"><div class="c0168"><div class="c0169"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 2048 2048" width="16" height="16"><path d="M1767 153q136 137 208 311t73 368q0 194-72 368t-209 311l-91-91q118-118 181-269t63-319q0-167-63-318t-181-270l91-91zm-272 272q81 81 125 186t44 221q0 115-44 220t-125 187l-90-90q63-63 96-145t34-172q0-90-33-172t-97-145l90-90zm-724-41l555 1664h-135l-170-512H387l-170 512H82L637 384h134zm207 1024L704 586l-274 822h548z"></path></svg></div><div class="c0170">Read aloud</div></div></span></button></div></div><div class="c0135"></div><div class="c0130"><div style="position: relative;"><button id="ink" aria-pressed="false" class="c0174 c0179 c0184 c0166 c0167 c0185" data-element-focusable="true" title="Draw"><span class="c0180"><div class="c0168"><div class="c0169"><svg id="pen-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 2048 2048" width="16" height="16"><path d="M1024 1984q-40 0-70-40t-51-104-35-139-22-147-11-128-4-82h386q-1 25-4 82t-11 128-22 147-35 139-51 103-70 41zM1856 0v448H192V0h1664z" fill="none"></path><path d="M1920 512h-120l-449 896h-72q-1 28-3 70t-8 93-15 105-23 108-33 101-44 84-57 57-72 22q-53 0-92-37t-67-95-46-131-29-144-15-133-6-100h-72L248 512H128V0h128v384h1536V0h128v512zm-896 1408q23-12 41-47t32-85 23-105 16-109 10-97 5-69H897q1 25 4 69t10 97 16 109 24 105 32 84 41 48zm632-1408H392l384 768h496l384-768z"></path></svg></div><div class="c0170">Draw</div></div></span></button></div><div style="position: relative;"><button id="ink-button-split" aria-expanded="false" aria-controls="ink-customisation-flyout" class="c0174 c0179 c0184 c0166 c0187" data-element-focusable="true" title="Select ink properties"><span class="c0180"><div class="c0168"><div class="c0169 c0188"><svg id="drop-down-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 2048 2048" width="12" height="12"><path d="M1939 467l90 90-1005 1005L19 557l90-90 915 915 915-915z"></path></svg></div></div></span></button></div><div style="position: relative;"><button id="highlight" aria-pressed="false" class="c0174 c0179 c0184 c0166 c0167 c0185" data-element-focusable="true" title="Highlight"><span class="c0180"><div class="c0168"><div class="c0169"><svg id="highlight-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" width="16" height="16"><path d="M5.5 15.1875V9.5H10.5V12.6875L5.5 15.1875ZM0.5 0H15.5V3.5H0.5V0Z" fill="none"></path><path d="M16 0V4C15.5469 4 15.1589 4.05208 14.8359 4.15625C14.513 4.25521 14.237 4.39583 14.0078 4.57812C13.7839 4.76042 13.6016 4.97656 13.4609 5.22656C13.3255 5.47135 13.2214 5.74219 13.1484 6.03906C13.0755 6.33594 13.026 6.64844 13 6.97656C12.974 7.30469 12.9609 7.64062 12.9609 7.98438C12.9609 8.32812 12.9661 8.66927 12.9766 9.00781C12.9922 9.34635 13 9.67708 13 10H11V13L5 16V10H3C3 9.67708 3.00521 9.34635 3.01562 9.00781C3.03125 8.66927 3.03906 8.32812 3.03906 7.98438C3.03906 7.64062 3.02604 7.30469 3 6.97656C2.97396 6.64844 2.92448 6.33594 2.85156 6.03906C2.77865 5.74219 2.67188 5.47135 2.53125 5.22656C2.39583 4.97656 2.21354 4.76042 1.98438 4.57812C1.76042 4.39583 1.48438 4.25521 1.15625 4.15625C0.833333 4.05208 0.447917 4 0 4V0H1V3H15V0H16ZM10 10H6V14.3828L10 12.3828V10ZM12 9C12 8.76562 11.9974 8.53646 11.9922 8.3125C11.987 8.08854 11.9844 7.86458 11.9844 7.64062C11.9844 7.30729 11.9974 6.97917 12.0234 6.65625C12.0495 6.33333 12.1016 6.02083 12.1797 5.71875C12.263 5.41146 12.3828 5.11458 12.5391 4.82812C12.7005 4.54167 12.9141 4.26562 13.1797 4H2.82031C3.08594 4.26562 3.29688 4.54427 3.45312 4.83594C3.61458 5.1224 3.73438 5.41927 3.8125 5.72656C3.89583 6.02865 3.95052 6.34115 3.97656 6.66406C4.0026 6.98177 4.01562 7.30729 4.01562 7.64062C4.01562 7.86458 4.01302 8.08854 4.00781 8.3125C4.0026 8.53646 4 8.76562 4 9H12Z"></path></svg></div><div class="c0170">Highlight</div></div></span></button></div><div style="position: relative;"><button id="highlight-button-split" aria-expanded="false" aria-controls="highlight-flyout" class="c0174 c0179 c0184 c0166 c0187" data-element-focusable="true" title="Select a highlight color"><span class="c0180"><div class="c0168"><div class="c0169 c0188"><svg id="drop-down-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 2048 2048" width="12" height="12"><path d="M1939 467l90 90-1005 1005L19 557l90-90 915 915 915-915z"></path></svg></div></div></span></button></div><div style="position: relative;"><button id="erase" aria-pressed="false" class="c0174 c0179 c0184 c0166 c0167" data-element-focusable="true" title="Erase"><span class="c0180"><div class="c0168"><div class="c0169"><svg viewBox="0 0 16 16" width="16px" height="16px" xmlns="http://www.w3.org/2000/svg"><path d="M8.71094 14H12V15H3.53906L0.390625 11.8438C0.265625 11.7188 0.169271 11.5729 0.101562 11.4062C0.0338542 11.2344 0 11.0573 0 10.875C0 10.6927 0.0338542 10.5182 0.101562 10.3516C0.169271 10.1797 0.268229 10.0286 0.398438 9.89844L9.75 0.539062L15.9531 6.75L8.71094 14ZM9.75 1.95312L4.20312 7.5L9 12.2891L14.5391 6.75L9.75 1.95312ZM7.28906 14L8.28906 13L3.5 8.20312L1.10938 10.6016C1.03646 10.6745 1 10.7656 1 10.875C1 10.9844 1.03646 11.0755 1.10938 11.1484L3.95312 14H7.28906Z"></path></svg></div><div class="c0170">Erase</div></div></span></button></div></div></div><div class="c0134"><div class="c0135"></div><div class="c0130"><div style="position: relative;"><button id="print" class="c0174 c0179 c0184 c0166" data-element-focusable="true" title="Print (Ctrl+P)"><span class="c0180"><div class="c0168"><div class="c0169"><svg viewBox="0 0 16 16" width="16px" height="16px" xmlns="http://www.w3.org/2000/svg"><path d="M15 6C15.1354 6 15.263 6.02604 15.3828 6.07812C15.5078 6.13021 15.6146 6.20312 15.7031 6.29688C15.7969 6.38542 15.8698 6.49219 15.9219 6.61719C15.974 6.73698 16 6.86458 16 7V14H12V16H4V14H0V7C0 6.86458 0.0260417 6.73698 0.078125 6.61719C0.130208 6.49219 0.200521 6.38542 0.289062 6.29688C0.382812 6.20312 0.489583 6.13021 0.609375 6.07812C0.734375 6.02604 0.864583 6 1 6H4V0H12V6H15ZM5 6H11V1H5V6ZM11 11H5V15H11V11ZM15 7H1V13H4V10H12V13H15V7ZM2.5 8C2.63542 8 2.7526 8.04948 2.85156 8.14844C2.95052 8.2474 3 8.36458 3 8.5C3 8.63542 2.95052 8.7526 2.85156 8.85156C2.7526 8.95052 2.63542 9 2.5 9C2.36458 9 2.2474 8.95052 2.14844 8.85156C2.04948 8.7526 2 8.63542 2 8.5C2 8.36458 2.04948 8.2474 2.14844 8.14844C2.2474 8.04948 2.36458 8 2.5 8Z"></path></svg></div></div></span></button></div><div style="position: relative;"><button id="save" class="c0174 c0179 c0184 c0166" data-element-focusable="true" title="Save (Ctrl+S)"><span class="c0180"><div class="c0168"><div class="c0169"><svg viewBox="0 0 16 16" width="16px" height="16px" xmlns="http://www.w3.org/2000/svg"><path d="M14 1C14.1406 1 14.2708 1.02604 14.3906 1.07812C14.5104 1.13021 14.6146 1.20312 14.7031 1.29688C14.7969 1.38542 14.8698 1.48958 14.9219 1.60938C14.974 1.72917 15 1.85938 15 2V15H2.78906L1 13.2031V2C1 1.85938 1.02604 1.72917 1.07812 1.60938C1.13021 1.48958 1.20052 1.38542 1.28906 1.29688C1.38281 1.20312 1.48958 1.13021 1.60938 1.07812C1.72917 1.02604 1.85938 1 2 1H14ZM4 7H12V2H4V7ZM10 11H5V14H6V12H7V14H10V11ZM14 2H13V8H3V2H2V12.7891L3.20312 14H4V10H11V14H14V2Z"></path></svg></div></div></span></button></div></div></div><div class="c0130"><div class="c0135"></div><div style="position: relative;"><button id="pin" class="c0174 c0179 c0184 c0166 c0172" data-element-focusable="true" title="Unpin toolbar"><span class="c0180"><div class="c0168"><div class="c0169"><svg class="c0116" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 2048 2048" width="16" height="16"><path d="M1990 748q-33 33-64 60t-66 47-73 29-89 11q-34 0-65-6l-379 379q13 38 19 78t6 80q0 65-13 118t-37 100-60 89-79 87l-386-386-568 569-136 45 45-136 569-568-386-386q44-44 86-79t89-59 100-38 119-13q40 0 80 6t78 19l379-379q-6-31-6-65 0-49 10-88t30-74 46-65 61-65l690 690z"></path></svg></div></div></span></button></div></div></div></div></div></div></div></div></div><div class="c0191" id="infobar-list" style="top: 41px;"></div></div></div>

  <!-- This is container div for the currently open modal if exists -->
  <div id="modal-root"><div id="reading_bar_modal"></div></div>

  <div id="sizer" style="top: 41px; width: 778px; height: 21140px;"></div>

  <div id="document-container">
    <div id="embed-border" style="visibility: hidden;"></div>
  <embed id="plugin" type="application/x-google-chrome-pdf" src="https://ieeexplore.ieee.org/ielx7/6287639/8948470/09047963.pdf?tp=&amp;arnumber=9047963&amp;isnumber=8948470&amp;ref=aHR0cHM6Ly9zY2hvbGFyLmdvb2dsZS5ubC8=" stream-url="chrome-extension://mhjfbmdgcfjbbpaeojofohoefgiehjai/ba091cdc-27aa-4dda-b4a9-ab84d3d182a9" headers="Accept-Ranges: bytes
Cache-Control: private, max-age=0, no-cache, no-store, must-revalidate
Connection: keep-alive
Content-Length: 2666424
Content-Type: application/pdf
Date: Mon, 28 Sep 2020 20:18:02 GMT
Expires: Wed, 11 Jan 1984 05:00:00 GMT
Last-Modified: Sun, 19 Apr 2020 08:03:35 GMT
Pragma: no-cache
X-XSS-Protection: 1
inst: 0
licenseowner: 0
member: 0
product: UNDEFINED
roamingip: NA
" background-color="0xFFE6E6E6" first-page-separator="4" style="top: 41px;height: calc(100% - 41px)" javascript="allow" stream_timestamp="1500180854472" top-level-url="undefined" class=""></div>

  <viewer-password-screen id="password-screen"></viewer-password-screen>

  <viewer-page-indicator id="page-indicator" hidden=""></viewer-page-indicator>

  <viewer-error-screen id="error-screen"></viewer-error-screen>

<viewer-annotation-pop-up id="annotation-pop-up"></viewer-annotation-pop-up>

<!-- Defining loadtimedata interface. -->
<script src="edge://resources/edge_js/load_time_data.js"></script>
<!-- Defining JobRunner construct. -->
<script src="helpers/job_runner.js"></script>
<script src="./setup_job_runners.js"></script>
<!-- Initialize job runners and initialize loadtimedata. -->
<script src="focus-visible.min.js"></script>
<script src="edge://resources/edge_js/util.js"></script>
<script src="edge://resources/js/promise_resolver.js"></script>
<!-- The generated bundles from webpack. -->
<script src="dist/pdfui_component.chunk.js"></script>
<script src="dist/vendors~main.chunk.js"></script>
<script src="dist/vendors~pdfui_component.chunk.js"></script>
<script src="dist/bundle.js"></script>

<script src="../main.js" type="module"></script>

</body></html>
```
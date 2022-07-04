var Haml;

(function () {

  var matchers, self_close_tags, embedder, forceXML, escaperName, escapeHtmlByDefault;

  function trim(text){
    return text.replace(/^\s+|\s+$/g,"");
  }

  function html_escape(text) {
    return (text + "").
      replace(/&/g, "&amp;").
      replace(/</g, "&lt;").
      replace(/>/g, "&gt;").
      replace(/\"/g, "&quot;");
  }

  function render_attribs(attribs) {
    var key, value, result = [];
    for (key in attribs) {
      if (key !== '_content' && attribs.hasOwnProperty(key)) {
        switch (attribs[key]) {
        case 'undefined':
        case 'false':
        case 'null':
        case '""':
          break;
        default:
          try {
            value = JSON.parse("[" + attribs[key] +"]")[0];
            if (value === true) {
              value = key;
            } else if (typeof value === 'string' && embedder.test(value)) {
              value = '" +\n' + parse_interpol(html_escape(value)) + ' +\n"';
            } else {
              value = html_escape(value);
            }
            result.push(" " + key + '=\\"' + value + '\\"');
          } catch (e) {
            result.push(" " + key + '=\\"" + '+escaperName+'(' + attribs[key] + ') + "\\"');
          }
        }
      }
    }
    return result.join("");
  }

  // Parse the attribute block using a state machine
  function parse_attribs(line) {
    var attributes = {},
        l = line.length,
        i, c,
        count = 1,
        quote = false,
        skip = false,
        open, close, joiner, seperator,
        pair = {
          start: 1,
          middle: null,
          end: null
        };

    if (!(l > 0 && (line.charAt(0) === '{' || line.charAt(0) === '('))) {
      return {
        _content: line[0] === ' ' ? line.substr(1, l) : line
      };
    }
    open = line.charAt(0);
    close = (open === '{') ? '}' : ')';
    joiner = (open === '{') ? ':' : '=';
    seperator = (open === '{') ? ',' : ' ';

    function process_pair() {
      if (typeof pair.start === 'number' &&
          typeof pair.middle === 'number' &&
          typeof pair.end === 'number') {
        var key = trim(line.substr(pair.start, pair.middle - pair.start)),
            value = trim(line.substr(pair.middle + 1, pair.end - pair.middle - 1));
        attributes[key] = value;
      }
      pair = {
        start: null,
        middle: null,
        end: null
      };
    }

    for (i = 1; count > 0; i += 1) {

      // If we reach the end of the line, then there is a problem
      if (i > l) {
        throw "Malformed attribute block";
      }

      c = line.charAt(i);
      if (skip) {
        skip = false;
      } else {
        if (quote) {
          if (c === '\\') {
            skip = true;
          }
          if (c === quote) {
            quote = false;
          }
        } else {
          if (c === '"' || c === "'") {
            quote = c;
          }

          if (count === 1) {
            if (c === joiner) {
              pair.middle = i;
            }
            if (c === seperator || c === close) {
              pair.end = i;
              process_pair();
              if (c === seperator) {
                pair.start = i + 1;
              }
            }
          }

          if (c === open || c === "(") {
            count += 1;
          }
          if (c === close || (count > 1 && c === ")")) {
            count -= 1;
          }
        }
      }
    }
    attributes._content = line.substr(i, line.length);
    return attributes;
  }

  // Split interpolated strings into an array of literals and code fragments.
  function parse_interpol(value) {
    var items = [],
        pos = 0,
        next = 0,
        match;
    while (true) {
      // Match up to embedded string
      next = value.substr(pos).search(embedder);
      if (next < 0) {
        if (pos < value.length) {
          items.push(JSON.stringify(value.substr(pos)));
        }
        break;
      }
      items.push(JSON.stringify(value.substr(pos, next)));
      pos += next;

      // Match embedded string
      match = value.substr(pos).match(embedder);
      next = match[0].length;
      if (next < 0) { break; }
      if(match[1] === "#"){
        items.push(escaperName+"("+(match[2] || match[3])+")");
      }else{
        //unsafe!!!
        items.push(match[2] || match[3]);
      }

      pos += next;
    }
    return _.filter(items, function (part) { return part && part.length > 0}).join(" +\n");
  }

  // Used to find embedded code in interpolated strings.
  embedder = /([#!])\{([^}]*)\}/;

  self_close_tags = ["meta", "img", "link", "br", "hr", "input", "area", "base"];

  // All matchers' regexps should capture leading whitespace in first capture
  // and trailing content in last capture
  matchers = [
    // html tags
    {
      name: "html tags",
      regexp: /^(\s*)((?:[.#%][a-z_\-][a-z0-9_:\-]*)+)(.*)$/i,
      process: function () {
        var line_beginning, tag, classes, ids, attribs, content, whitespaceSpecifier, whitespace={}, output;
        line_beginning = this.matches[2];
        classes = line_beginning.match(/\.([a-z_\-][a-z0-9_\-]*)/gi);
        ids = line_beginning.match(/\#([a-z_\-][a-z0-9_\-]*)/gi);
        tag = line_beginning.match(/\%([a-z_\-][a-z0-9_:\-]*)/gi);

        // Default to <div> tag
        tag = tag ? tag[0].substr(1, tag[0].length) : 'div';

        attribs = this.matches[3];
        if (attribs) {
          attribs = parse_attribs(attribs);
          if (attribs._content) {
            var leader0 = attribs._content.charAt(0),
                leader1 = attribs._content.charAt(1),
                leaderLength = 0;

            if(leader0 == "<"){
              leaderLength++;
              whitespace.inside = true;
              if(leader1 == ">"){
                leaderLength++;
                whitespace.around = true;
              }
            }else if(leader0 == ">"){
              leaderLength++;
              whitespace.around = true;
              if(leader1 == "<"){
                leaderLength++;
                whitespace.inside = true;
              }
            }
            attribs._content = attribs._content.substr(leaderLength);
            //once we've identified the tag and its attributes, the rest is content.
            // this is currently trimmed for neatness.
            this.contents.unshift(trim(attribs._content));
            delete(attribs._content);
          }
        } else {
          attribs = {};
        }

        if (classes) {
          classes = _.map(classes, function (klass) {
            return klass.substr(1, klass.length);
          }).join(' ');
          if (attribs['class']) {
            try {
              attribs['class'] = JSON.stringify(classes + " " + JSON.parse(attribs['class']));
            } catch (e) {
              attribs['class'] = JSON.stringify(classes + " ") + " + " + attribs['class'];
            }
          } else {
            attribs['class'] = JSON.stringify(classes);
          }
        }
        if (ids) {
          ids = _.map(ids, function (id) {
            return id.substr(1, id.length);
          }).join(' ');
          if (attribs.id) {
            attribs.id = JSON.stringify(ids + " ") + attribs.id;
          } else {
            attribs.id = JSON.stringify(ids);
          }
        }

        attribs = render_attribs(attribs);

        content = this.render_contents();
        if (content === '""') {
          content = '';
        }

        if(whitespace.inside){
          if(content.length==0){
            content='"  "'
          }else{
            try{ //remove quotes if they are there
              content = '" '+JSON.parse(content)+' "';
            }catch(e){
              content = '" "+\n'+content+'+\n" "';
            }
          }
        }

        if (forceXML ? content.length > 0 : _.indexOf(self_close_tags, tag) == -1) {
          output = '"<' + tag + attribs + '>"' +
            (content.length > 0 ? ' + \n' + content : "") +
            ' + \n"</' + tag + '>"';
        } else {
          output = '"<' + tag + attribs + ' />"';
        }

        if(whitespace.around){
          //output now contains '"<b>hello</b>"'
          //we need to crack it open to insert whitespace.
          output = '" '+output.substr(1, output.length - 2)+' "';
        }

        return output;
      }
    },

    // each loops
    {
      name: "each loop",
      regexp: /^(\s*)(?::for|:each)\s+(?:([a-z_][a-z_\-]*),\s*)?([a-z_][a-z_\-]*)\s+in\s+(.*)(\s*)$/i,
      process: function () {
        var ivar = this.matches[2] || '__key__', // index
            vvar = this.matches[3],              // value
            avar = this.matches[4],              // array
            rvar = '__result__';                 // results

        if (this.matches[5]) {
          this.contents.unshift(this.matches[5]);
        }
        return '(function () { ' +
          'var ' + rvar + ' = [], ' + ivar + ', ' + vvar + '; ' +
          'for (' + ivar + ' in ' + avar + ') { ' +
          'if (' + avar + '.hasOwnProperty(' + ivar + ')) { ' +
          vvar + ' = ' + avar + '[' + ivar + ']; ' +
          rvar + '.push(\n' + (this.render_contents() || "''") + '\n); ' +
          '} } return ' + rvar + '.join(""); }).call(this)';
      }
    },

    // if statements
    {
      name: "if",
      regexp: /^(\s*):if\s+(.*)\s*$/i,
      process: function () {
        var condition = this.matches[2];
        return '(function () { ' +
          'if (' + condition + ') { ' +
          'return (\n' + (this.render_contents() || '') + '\n);' +
          '} else { return ""; } }).call(this)';
      }
    },

    // silent-comments
    {
      name: "silent-comments",
      regexp: /^(\s*)-#\s*(.*)\s*$/i,
      process: function () {
        return '""';
      }
    },

    //html-comments
    {
      name: "silent-comments",
      regexp: /^(\s*)\/\s*(.*)\s*$/i,
      process: function () {
        this.contents.unshift(this.matches[2]);

        return '"<!--'+this.contents.join('\\n')+'-->"';
      }
    },

    // raw js
    {
      name: "raw",
      regexp: /^(\s*)-\s*(.*)\s*$/i,
      process: function () {
        this.contents.unshift(this.matches[2]);
        return '"";' + this.contents.join("\n")+"; _$output = _$output ";
      }
    },

    // declarations
    {
      name: "doctype",
      regexp: /^()!!!(?:\s*(.*))\s*$/,
      process: function () {
        var line = '';
        switch ((this.matches[2] || '').toLowerCase()) {
        case '':
          // XHTML 1.0 Transitional
          line = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">';
          break;
        case 'strict':
        case '1.0':
          // XHTML 1.0 Strict
          line = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">';
          break;
        case 'frameset':
          // XHTML 1.0 Frameset
          line = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">';
          break;
        case '5':
          // XHTML 5
          line = '<!DOCTYPE html>';
          break;
        case '1.1':
          // XHTML 1.1
          line = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">';
          break;
        case 'basic':
          // XHTML Basic 1.1
          line = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML Basic 1.1//EN" "http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd">';
          break;
        case 'mobile':
          // XHTML Mobile 1.2
          line = '<!DOCTYPE html PUBLIC "-//WAPFORUM//DTD XHTML Mobile 1.2//EN" "http://www.openmobilealliance.org/tech/DTD/xhtml-mobile12.dtd">';
          break;
        case 'xml':
          // XML
          line = "<?xml version='1.0' encoding='utf-8' ?>";
          break;
        case 'xml iso-8859-1':
          // XML iso-8859-1
          line = "<?xml version='1.0' encoding='iso-8859-1' ?>";
          break;
        }
        return JSON.stringify(line + "\n");
      }
    },

    // Embedded markdown. Needs to be added to exports externally.
    {
      name: "markdown",
      regexp: /^(\s*):markdown\s*$/i,
      process: function () {
        return parse_interpol(exports.Markdown.encode(this.contents.join("\n")));
      }
    },

    // script blocks
    {
      name: "script",
      regexp: /^(\s*):(?:java)?script\s*$/,
      process: function () {
        return parse_interpol('\n<script type="text/javascript">\n' +
          '//<![CDATA[\n' +
          this.contents.join("\n") +
          "\n//]]>\n</script>\n");
      }
    },

    // css blocks
    {
      name: "css",
      regexp: /^(\s*):css\s*$/,
      process: function () {
        return JSON.stringify('<style type="text/css">\n' +
          this.contents.join("\n") +
          "\n</style>");
      }
    }

  ];

  function compile(lines) {
    var block = false,
        output = [];

    // If lines is a string, turn it into an array
    if (typeof lines === 'string') {
      lines = trim(lines).replace(/\n\r|\r/g, '\n').split('\n');
    }

    _.each(lines, function(line){
      var match, found = false;

      // Collect all text as raw until outdent
      if (block) {
        match = block.check_indent.exec(line);
        if (match) {
          block.contents.push(match[1] || "");
          return;
        } else {
          output.push(block.process());
          block = false;
        }
      }

      _.each(matchers, function(matcher){
        if (!found) {
          match = matcher.regexp.exec(line);
          if (match) {
            block = {
              contents: [],
              indent_level: (match[1]),
              matches: match,
              check_indent: new RegExp("^(?:\\s*|" + match[1] + "  (.*))$"),
              process: matcher.process,
              render_contents: function () {
                return compile(this.contents);
              }
            };
            found = true;
          }
        }
      });

      // Match plain text
      if (!found) {
        output.push(function () {
          // Escaped plain text
          if (line[0] === '\\') {
            return parse_interpol(line.substr(1, line.length));
          }


          function escapedLine(){
            try {
              return escaperName+'('+JSON.stringify(JSON.parse(line)) +')';
            } catch (e2) {
              return escaperName+'(' + line + ')';
            }
          }

          function unescapedLine(){
            try {
              return parse_interpol(JSON.parse(line));
            } catch (e) {
              return line;
            }
          }

          // always escaped
          if((line.substr(0, 2) === "&=")) {
            line = trim(line.substr(2, line.length));
            return escapedLine();
          }

          //never escaped
          if((line.substr(0, 2) === "!=")) {
            line = trim(line.substr(2, line.length));
            return unescapedLine();
          }

          // sometimes escaped
          if ( (line[0] === '=')) {
            line = trim(line.substr(1, line.length));
            if(escapeHtmlByDefault){
              return escapedLine();
            }else{
              return unescapedLine();
            }
          }

          // Plain text
          return parse_interpol(line);
        }());
      }

    });

    if (block) {
      output.push(block.process());
    }

    var txt = _.filter(output, function (part) { return part && part.length > 0}).join(" +\n");
    if(txt.length == 0){
      txt = '""';
    }
    return txt;
  };

  function optimize(js) {
    var new_js = [], buffer = [], part, end;

    function flush() {
      if (buffer.length > 0) {
        new_js.push(JSON.stringify(buffer.join("")) + end);
        buffer = [];
      }
    }
    _.each(js.replace(/\n\r|\r/g, '\n').split('\n'), function (line) {
      part = line.match(/^(\".*\")(\s*\+\s*)?$/);
      if (!part) {
        flush();
        new_js.push(line);
        return;
      }
      end = part[2] || "";
      part = part[1];
      try {
        buffer.push(JSON.parse(part));
      } catch (e) {
        flush();
        new_js.push(line);
      }
    });
    flush();
    return new_js.join("\n");
  };

  function render(text, options) {
    options = options || {};
    text = text || "";
    var js = compile(text, options);
    if (options.optimize) {
      js = Haml.optimize(js);
    }
    return execute(js, options.context || Haml, options.locals);
  };

  function execute(js, self, locals) {
    return (function () {
      with(locals || {}) {
        try {
          var _$output;
          eval("(" + js + ")");
          return _$output; //set in eval
        } catch (e) {
          return "\n<pre class='error'>" + html_escape(e.stack) + "</pre>\n";
        }

      }
    }).call(self);
  };

  Haml = function (haml, config) {
    if(typeof(config) != "object"){
      forceXML = config;
      config = {};
    }

    var escaper;
    if(config.customEscape){
      escaper = "";
      escaperName = config.customEscape;
    }else{
      escaper = html_escape.toString() + "\n";
      escaperName = "html_escape";
    }

    escapeHtmlByDefault = (config.escapeHtmlByDefault || config.escapeHTML || config.escape_html);

    var js = optimize(compile(haml));

    var str = "with(locals || {}) {\n" +
    "  try {\n" +
    "   var _$output=" + js + ";\n return _$output;" +
    "  } catch (e) {\n" +
    "    return \"\\n<pre class='error'>\" + "+escaperName+"(e.stack) + \"</pre>\\n\";\n" +
    "  }\n" +
    "}"

    return new Function("locals",  escaper + str );
  }

  Haml.compile = compile;
  Haml.optimize = optimize;
  Haml.render = render;
  Haml.execute = execute;
  Haml.html_escape = html_escape;
}());

// Hook into module system
if (typeof module !== 'undefined') {
  module.exports = Haml;
}
;
/*
 * NOTE: Wrapped by function to initialize w/ loader's specific jquery reference!
 * 
 * jqModal - Minimalist Modaling with jQuery
 *   (http://dev.iceburg.net/jquery/jqModal/)
 *
 * Copyright (c) 2007,2008 Brice Burgess <bhb@iceburg.net>
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 * 
 * $Version: 03/01/2009 +r14
 */

(function(){
  if (!window.GSFN){ window.GSFN = {}; }
  if (!GSFN.Util) { GSFN.Util = {}; }
})();

// Initialize the jqModal jQuery plugin with the supplied jQuery reference 
GSFN.Util.jqmModalInit = function(jQueryRef){
    (function($) {
        $.fn.jqm=function(o){
        var p={
            overlay: 50,
            overlayClass: 'jqmOverlay',
            closeClass: 'jqmClose',
            trigger: '.jqModal',
            ajax: F,
            ajaxText: '',
            target: F,
            modal: F,
            toTop: F,
            onShow: F,
            onHide: F,
            onLoad: F
        };
        return this.each(function(){if(this._jqm)return H[this._jqm].c=$.extend({},H[this._jqm].c,o);s++;this._jqm=s;
        H[s]={c:$.extend(p,$.jqm.params,o),a:F,w:$(this).addClass('jqmID'+s),s:s};
        if(p.trigger)$(this).jqmAddTrigger(p.trigger);
        });};
        
        $.fn.jqmAddClose=function(e){return hs(this,e,'jqmHide');};
        $.fn.jqmAddTrigger=function(e){return hs(this,e,'jqmShow');};
        $.fn.jqmShow=function(t){return this.each(function(){t=t||window.event;$.jqm.open(this._jqm,t);});};
        $.fn.jqmHide=function(t){return this.each(function(){t=t||window.event;$.jqm.close(this._jqm,t)});};
        
        $.jqm = {
        hash:{},
        open:function(s,t){var h=H[s],c=h.c,cc='.'+c.closeClass,z=(parseInt(h.w.css('z-index'))),z=(z>0)?z:110000,o=$('<div></div>').css({height:'100%',width:'100%',position:'fixed',left:0,top:0,'z-index':z-1,opacity:c.overlay/100});if(h.a)return F;h.t=t;h.a=true;h.w.css('z-index',z);
         if(c.modal) {if(!A[0])L('bind');A.push(s);}
         else if(c.overlay > 0)h.w.jqmAddClose(o);
         else o=F;
        
         h.o=(o)?o.addClass(c.overlayClass).prependTo('body'):F;
         if(ie6){$('html,body').css({height:'100%',width:'100%'});if(o){o=o.css({position:'absolute'})[0];for(var y in {Top:1,Left:1})o.style.setExpression(y.toLowerCase(),"(_=(document.documentElement.scroll"+y+" || document.body.scroll"+y+"))+'px'");}}
        
         if(c.ajax) {var r=c.target||h.w,u=c.ajax,r=(typeof r == 'string')?$(r,h.w):$(r),u=(u.substr(0,1) == '@')?$(t).attr(u.substring(1)):u;
          r.html(c.ajaxText).load(u,function(){if(c.onLoad)c.onLoad.call(this,h);if(cc)h.w.jqmAddClose($(cc,h.w));e(h);});}
         else if(cc)h.w.jqmAddClose($(cc,h.w));
        
         if(c.toTop&&h.o)h.w.before('<span id="jqmP'+h.w[0]._jqm+'"></span>').insertAfter(h.o);	
         (c.onShow)?c.onShow(h):h.w.show();e(h);return F;
        },
        close:function(s){var h=H[s];if(!h.a)return F;h.a=F;
         if(A[0]){A.pop();if(!A[0])L('unbind');}
         if(h.c.toTop&&h.o)$('#jqmP'+h.w[0]._jqm).after(h.w).remove();
         if(h.c.onHide)h.c.onHide(h);else{h.w.hide();if(h.o)h.o.remove();} return F;
        },
        params:{}};
        var s=0,H=$.jqm.hash,A=[],ie6=$.browser.msie&&($.browser.version == "6.0"),F=false,
        i=$('<iframe src="javascript:false;document.write(\'\');" class="jqm"></iframe>').css({opacity:0}),
        e=function(h){if(ie6)if(h.o)h.o.html('<p style="width:100%;height:100%"/>').prepend(i);else if(!$('iframe.jqm',h.w)[0])h.w.prepend(i); f(h);},
        f=function(h){try{$(':input:visible',h.w)[0].focus();}catch(_){}},
        L=function(t){$()[t]("keypress",m)[t]("keydown",m)[t]("mousedown",m);},
        m=function(e){var h=H[A[A.length-1]],r=(!$(e.target).parents('.jqmID'+h.s)[0]);if(r)f(h);return !r;},
        hs=function(w,t,c){return w.each(function(){var s=this._jqm;$(t).each(function() {
         if(!this[c]){this[c]=[];$(this).click(function(){for(var i in {jqmShow:1,jqmHide:1})for(var s in this[i])if(H[this[i][s]])H[this[i][s]].w[i](this);return F;});}this[c].push(s);});});};
    })(jQueryRef);
}
;
(function() {
  this.JST || (this.JST = {});
  this.JST["modal/templates/alert_modal_content"] = Haml('.gsfn-jqm-alert-template{style: \'display: none\'}\n .gsfn-jqm-alert-content\n    .jargon\n    .banner\n      %h3 #{title}\n    .gsfn-jqm-alert-message\n      !{content}\n    .footer\n      .content\n        !{footerContent}\n', {escapeHtmlByDefault:true});
}).call(this);
(function() {
  this.JST || (this.JST = {});
  this.JST["modal/templates/alert_modal"] = Haml('.gsfn-jqm-alert{id: modalId, style: "top: #{offsetTop}; height: #{contentHeight};"}\n  .gsfn-jqm-alert-container\n  %a.gsfn-jqmClose Close\n', {escapeHtmlByDefault:true});
}).call(this);
(function() {

  (function($) {
    if (!$.jqm) {
      if (window.GSFN && window.GSFN.Util && (typeof window.GSFN.Util.jqmModalInit($) === 'function')) {
        window.GSFN.Util.jqmModalInit($);
      }
      if ($.jqm && (typeof $.jqm.open === 'function')) {
        if (!window.GSFN) window.GSFN = {};
        if (!window.GSFN.Util) window.GSFN.Util = {};
        if (!window.GSFN.Util.Modal) window.GSFN.Util.Modal = {};
        return window.GSFN.Util.Modal.alert = function(incoming_configuration) {
          var $container, $modal, configuration, copy, modal_barebone, _ref;
          configuration = {
            content: incoming_configuration.content || '',
            contentHeight: incoming_configuration.contentHeight || '280px',
            footerContent: incoming_configuration.footerContent || '',
            modalId: incoming_configuration.modalId || ("gsfn-modal-alert" + (new Date().getTime())),
            offsetTop: incoming_configuration.offsetTop || "40%",
            title: incoming_configuration.title || ''
          };
          _ref = [JST["modal/templates/alert_modal"](configuration), JST['modal/templates/alert_modal_content'](configuration)], modal_barebone = _ref[0], copy = _ref[1];
          $modal = $(modal_barebone).appendTo($('body'));
          $container = $modal.find('.gsfn-jqm-alert-container');
          return $modal.jqm({
            modal: true,
            overlay: 60,
            toTop: true,
            overlayClass: 'gsfn-jqmOverlay',
            onShow: function(hash) {
              var hide;
              hash.o.show();
              $container.html(copy);
              hide = function(event) {
                return hash.w.jqmHide();
              };
              $modal.find('.jqmClose, .gsfn-jqmClose, .closeDialog').click(hide);
              hash.o.click(hide);
              return hash.w.show();
            },
            onHide: function(modal) {
              modal.w.hide();
              modal.o.unbind("click").remove();
              $modal.remove();
              return true;
            }
          }).jqmAddClose('.gsfn-jqmClose').jqmShow();
        };
      }
    }
  })(jQuery);

}).call(this);
(function() {
  this.JST || (this.JST = {});
  this.JST["templates/companies/leaving_community"] = Haml('.summary\n  You are about to leave the #{communityName} community. If you\'re looking for help from #{communityName} you will need to !{contactThem}\n.returnToCommunity\n  %button.closeDialog{href: \'#\'} Return to #{communityName} community\n', {escapeHtmlByDefault:true});
}).call(this);
(function() {
  this.JST || (this.JST = {});
  this.JST["templates/companies/legal_usage_explanation"] = Haml('.legalExplanation\n  #{communityName} uses software from Get Satisfaction to communicate with their customers. If you\'d like to learn more about Get Satisfaction \n  %a{href: \'/about\'} please click here.\n', {escapeHtmlByDefault:true});
}).call(this);
(function() {

  (function($) {
    return $(document).ready(function() {
      var companyAttr, domain, links, _base;
      links = $("a.powered_by");
      companyAttr = function(attrName) {
        return $('body').data("company-" + attrName);
      };
      domain = companyAttr('domain');
      if (links.length && domain && domain !== 'getsatisfaction' && domain !== 'devcommunity') {
        if (window.GSFN == null) window.GSFN = {};
        if ((_base = window.GSFN).Alert == null) _base.Alert = {};
        GSFN.initialize_leaving_community_alert = function(community) {
          window.GSFN.Alert.leavingCommunity = function() {
            var contactCopy, content, marginTop;
            if (window.GSFN.Util && window.GSFN.Util.Modal && (typeof window.GSFN.Util.Modal.alert === 'function')) {
              contactCopy = "contact them directly.";
              community = {
                name: companyAttr('name'),
                website: companyAttr('website')
              };
              marginTop = $(window).height() / 4;
              content = JST['templates/companies/leaving_community']({
                communityName: community.name,
                contactThem: community.website ? "<a href='" + community.website + "'>" + contactCopy + "</a>" : contactCopy
              });
              return GSFN.Util.Modal.alert({
                content: content,
                contentHeight: "300px",
                footerContent: JST['templates/companies/legal_usage_explanation']({
                  communityName: community.name
                }),
                modalId: 'gsfn-leaving-community',
                offsetTop: "" + marginTop + "px",
                title: "You're leaving the community"
              });
            }
          };
          return links.bind('click', function(event) {
            event.preventDefault();
            return window.GSFN.Alert.leavingCommunity();
          });
        };
        GSFN.initialize_leaving_community_alert();
        return delete GSFN.initialize_leaving_community_alert;
      }
    });
  })(jQuery);

}).call(this);

/*  Prototype JavaScript framework, version 1.7
 *  (c) 2005-2010 Sam Stephenson
 *
 *  Prototype is freely distributable under the terms of an MIT-style license.
 *  For details, see the Prototype web site: http://www.prototypejs.org/
 *
 *--------------------------------------------------------------------------*/


var Prototype = {

  Version: '1.7',

  Browser: (function() {
    var ua = navigator.userAgent;
    var isOpera = Object.prototype.toString.call(window.opera) == '[object Opera]';
    return {
      IE: !! window.attachEvent && !isOpera,
      Opera: isOpera,
      WebKit: ua.indexOf('AppleWebKit/') > -1,
      Gecko: ua.indexOf('Gecko') > -1 && ua.indexOf('KHTML') === -1,
      MobileSafari: /Apple.*Mobile/.test(ua)
    }
  })(),

  BrowserFeatures: {
    XPath: !! document.evaluate,

    SelectorsAPI: !! document.querySelector,

    ElementExtensions: (function() {
      var constructor = window.Element || window.HTMLElement;
      return !!(constructor && constructor.prototype);
    })(),
    SpecificElementExtensions: (function() {
      if (typeof window.HTMLDivElement !== 'undefined') return true;

      var div = document.createElement('div'),
          form = document.createElement('form'),
          isSupported = false;

      if (div['__proto__'] && (div['__proto__'] !== form['__proto__'])) {
        isSupported = true;
      }

      div = form = null;

      return isSupported;
    })()
  },

  ScriptFragment: '<script[^>]*>([\\S\\s]*?)<\/script>',
  JSONFilter: /^\/\*-secure-([\s\S]*)\*\/\s*$/,

  emptyFunction: function() {},

  K: function(x) {
    return x
  }
};

if (Prototype.Browser.MobileSafari) Prototype.BrowserFeatures.SpecificElementExtensions = false;


var Abstract = {};


var Try = {
  these: function() {
    var returnValue;

    for (var i = 0, length = arguments.length; i < length; i++) {
      var lambda = arguments[i];
      try {
        returnValue = lambda();
        break;
      } catch (e) {}
    }

    return returnValue;
  }
};

/* Based on Alex Arnell's inheritance implementation. */

var Class = (function() {

  var IS_DONTENUM_BUGGY = (function() {
    for (var p in {
      toString: 1
    }) {
      if (p === 'toString') return false;
    }
    return true;
  })();

  function subclass() {};

  function create() {
    var parent = null,
        properties = $A(arguments);
    if (Object.isFunction(properties[0])) parent = properties.shift();

    function klass() {
      this.initialize.apply(this, arguments);
    }

    Object.extend(klass, Class.Methods);
    klass.superclass = parent;
    klass.subclasses = [];

    if (parent) {
      subclass.prototype = parent.prototype;
      klass.prototype = new subclass;
      parent.subclasses.push(klass);
    }

    for (var i = 0, length = properties.length; i < length; i++)
    klass.addMethods(properties[i]);

    if (!klass.prototype.initialize) klass.prototype.initialize = Prototype.emptyFunction;

    klass.prototype.constructor = klass;
    return klass;
  }

  function addMethods(source) {
    var ancestor = this.superclass && this.superclass.prototype,
        properties = Object.keys(source);

    if (IS_DONTENUM_BUGGY) {
      if (source.toString != Object.prototype.toString) properties.push("toString");
      if (source.valueOf != Object.prototype.valueOf) properties.push("valueOf");
    }

    for (var i = 0, length = properties.length; i < length; i++) {
      var property = properties[i],
          value = source[property];
      if (ancestor && Object.isFunction(value) && value.argumentNames()[0] == "$super") {
        var method = value;
        value = (function(m) {
          return function() {
            return ancestor[m].apply(this, arguments);
          };
        })(property).wrap(method);

        value.valueOf = method.valueOf.bind(method);
        value.toString = method.toString.bind(method);
      }
      this.prototype[property] = value;
    }

    return this;
  }

  return {
    create: create,
    Methods: {
      addMethods: addMethods
    }
  };
})();
(function() {

  var _toString = Object.prototype.toString,
      NULL_TYPE = 'Null',
      UNDEFINED_TYPE = 'Undefined',
      BOOLEAN_TYPE = 'Boolean',
      NUMBER_TYPE = 'Number',
      STRING_TYPE = 'String',
      OBJECT_TYPE = 'Object',
      FUNCTION_CLASS = '[object Function]',
      BOOLEAN_CLASS = '[object Boolean]',
      NUMBER_CLASS = '[object Number]',
      STRING_CLASS = '[object String]',
      ARRAY_CLASS = '[object Array]',
      DATE_CLASS = '[object Date]',
      NATIVE_JSON_STRINGIFY_SUPPORT = window.JSON && typeof JSON.stringify === 'function' && JSON.stringify(0) === '0' && typeof JSON.stringify(Prototype.K) === 'undefined';

  function Type(o) {
    switch (o) {
    case null:
      return NULL_TYPE;
    case (void 0):
      return UNDEFINED_TYPE;
    }
    var type = typeof o;
    switch (type) {
    case 'boolean':
      return BOOLEAN_TYPE;
    case 'number':
      return NUMBER_TYPE;
    case 'string':
      return STRING_TYPE;
    }
    return OBJECT_TYPE;
  }

  function extend(destination, source) {
    for (var property in source)
    destination[property] = source[property];
    return destination;
  }

  function inspect(object) {
    try {
      if (isUndefined(object)) return 'undefined';
      if (object === null) return 'null';
      return object.inspect ? object.inspect() : String(object);
    } catch (e) {
      if (e instanceof RangeError) return '...';
      throw e;
    }
  }

  function toJSON(value) {
    return Str('', {
      '': value
    }, []);
  }

  function Str(key, holder, stack) {
    var value = holder[key],
        type = typeof value;

    if (Type(value) === OBJECT_TYPE && typeof value.toJSON === 'function') {
      value = value.toJSON(key);
    }

    var _class = _toString.call(value);

    switch (_class) {
    case NUMBER_CLASS:
    case BOOLEAN_CLASS:
    case STRING_CLASS:
      value = value.valueOf();
    }

    switch (value) {
    case null:
      return 'null';
    case true:
      return 'true';
    case false:
      return 'false';
    }

    type = typeof value;
    switch (type) {
    case 'string':
      return value.inspect(true);
    case 'number':
      return isFinite(value) ? String(value) : 'null';
    case 'object':

      for (var i = 0, length = stack.length; i < length; i++) {
        if (stack[i] === value) {
          throw new TypeError();
        }
      }
      stack.push(value);

      var partial = [];
      if (_class === ARRAY_CLASS) {
        for (var i = 0, length = value.length; i < length; i++) {
          var str = Str(i, value, stack);
          partial.push(typeof str === 'undefined' ? 'null' : str);
        }
        partial = '[' + partial.join(',') + ']';
      } else {
        var keys = Object.keys(value);
        for (var i = 0, length = keys.length; i < length; i++) {
          var key = keys[i],
              str = Str(key, value, stack);
          if (typeof str !== "undefined") {
            partial.push(key.inspect(true) + ':' + str);
          }
        }
        partial = '{' + partial.join(',') + '}';
      }
      stack.pop();
      return partial;
    }
  }

  function stringify(object) {
    return JSON.stringify(object);
  }

  function toQueryString(object) {
    return $H(object).toQueryString();
  }

  function toHTML(object) {
    return object && object.toHTML ? object.toHTML() : String.interpret(object);
  }

  function keys(object) {
    if (Type(object) !== OBJECT_TYPE) {
      throw new TypeError();
    }
    var results = [];
    for (var property in object) {
      if (object.hasOwnProperty(property)) {
        results.push(property);
      }
    }
    return results;
  }

  function values(object) {
    var results = [];
    for (var property in object)
    results.push(object[property]);
    return results;
  }

  function clone(object) {
    return extend({}, object);
  }

  function isElement(object) {
    return !!(object && object.nodeType == 1);
  }

  function isArray(object) {
    return _toString.call(object) === ARRAY_CLASS;
  }

  var hasNativeIsArray = (typeof Array.isArray == 'function') && Array.isArray([]) && !Array.isArray({});

  if (hasNativeIsArray) {
    isArray = Array.isArray;
  }

  function isHash(object) {
    return object instanceof Hash;
  }

  function isFunction(object) {
    return _toString.call(object) === FUNCTION_CLASS;
  }

  function isString(object) {
    return _toString.call(object) === STRING_CLASS;
  }

  function isNumber(object) {
    return _toString.call(object) === NUMBER_CLASS;
  }

  function isDate(object) {
    return _toString.call(object) === DATE_CLASS;
  }

  function isUndefined(object) {
    return typeof object === "undefined";
  }

  extend(Object, {
    extend: extend,
    inspect: inspect,
    toJSON: NATIVE_JSON_STRINGIFY_SUPPORT ? stringify : toJSON,
    toQueryString: toQueryString,
    toHTML: toHTML,
    keys: Object.keys || keys,
    values: values,
    clone: clone,
    isElement: isElement,
    isArray: isArray,
    isHash: isHash,
    isFunction: isFunction,
    isString: isString,
    isNumber: isNumber,
    isDate: isDate,
    isUndefined: isUndefined
  });
})();
Object.extend(Function.prototype, (function() {
  var slice = Array.prototype.slice;

  function update(array, args) {
    var arrayLength = array.length,
        length = args.length;
    while (length--) array[arrayLength + length] = args[length];
    return array;
  }

  function merge(array, args) {
    array = slice.call(array, 0);
    return update(array, args);
  }

  function argumentNames() {
    var names = this.toString().match(/^[\s\(]*function[^(]*\(([^)]*)\)/)[1].replace(/\/\/.*?[\r\n]|\/\*(?:.|[\r\n])*?\*\//g, '').replace(/\s+/g, '').split(',');
    return names.length == 1 && !names[0] ? [] : names;
  }

  function bind(context) {
    if (arguments.length < 2 && Object.isUndefined(arguments[0])) return this;
    var __method = this,
        args = slice.call(arguments, 1);
    return function() {
      var a = merge(args, arguments);
      return __method.apply(context, a);
    }
  }

  function bindAsEventListener(context) {
    var __method = this,
        args = slice.call(arguments, 1);
    return function(event) {
      var a = update([event || window.event], args);
      return __method.apply(context, a);
    }
  }

  function curry() {
    if (!arguments.length) return this;
    var __method = this,
        args = slice.call(arguments, 0);
    return function() {
      var a = merge(args, arguments);
      return __method.apply(this, a);
    }
  }

  function delay(timeout) {
    var __method = this,
        args = slice.call(arguments, 1);
    timeout = timeout * 1000;
    return window.setTimeout(function() {
      return __method.apply(__method, args);
    }, timeout);
  }

  function defer() {
    var args = update([0.01], arguments);
    return this.delay.apply(this, args);
  }

  function wrap(wrapper) {
    var __method = this;
    return function() {
      var a = update([__method.bind(this)], arguments);
      return wrapper.apply(this, a);
    }
  }

  function methodize() {
    if (this._methodized) return this._methodized;
    var __method = this;
    return this._methodized = function() {
      var a = update([this], arguments);
      return __method.apply(null, a);
    };
  }

  return {
    argumentNames: argumentNames,
    bind: bind,
    bindAsEventListener: bindAsEventListener,
    curry: curry,
    delay: delay,
    defer: defer,
    wrap: wrap,
    methodize: methodize
  }
})());



(function(proto) {


  function toISOString() {
    return this.getUTCFullYear() + '-' + (this.getUTCMonth() + 1).toPaddedString(2) + '-' + this.getUTCDate().toPaddedString(2) + 'T' + this.getUTCHours().toPaddedString(2) + ':' + this.getUTCMinutes().toPaddedString(2) + ':' + this.getUTCSeconds().toPaddedString(2) + 'Z';
  }


  function toJSON() {
    return this.toISOString();
  }

  if (!proto.toISOString) proto.toISOString = toISOString;
  if (!proto.toJSON) proto.toJSON = toJSON;

})(Date.prototype);


RegExp.prototype.match = RegExp.prototype.test;

RegExp.escape = function(str) {
  return String(str).replace(/([.*+?^=!:${}()|[\]\/\\])/g, '\\$1');
};
var PeriodicalExecuter = Class.create({
  initialize: function(callback, frequency) {
    this.callback = callback;
    this.frequency = frequency;
    this.currentlyExecuting = false;

    this.registerCallback();
  },

  registerCallback: function() {
    this.timer = setInterval(this.onTimerEvent.bind(this), this.frequency * 1000);
  },

  execute: function() {
    this.callback(this);
  },

  stop: function() {
    if (!this.timer) return;
    clearInterval(this.timer);
    this.timer = null;
  },

  onTimerEvent: function() {
    if (!this.currentlyExecuting) {
      try {
        this.currentlyExecuting = true;
        this.execute();
        this.currentlyExecuting = false;
      } catch (e) {
        this.currentlyExecuting = false;
        throw e;
      }
    }
  }
});
Object.extend(String, {
  interpret: function(value) {
    return value == null ? '' : String(value);
  },
  specialChar: {
    '\b': '\\b',
    '\t': '\\t',
    '\n': '\\n',
    '\f': '\\f',
    '\r': '\\r',
    '\\': '\\\\'
  }
});

Object.extend(String.prototype, (function() {
  var NATIVE_JSON_PARSE_SUPPORT = window.JSON && typeof JSON.parse === 'function' && JSON.parse('{"test": true}').test;

  function prepareReplacement(replacement) {
    if (Object.isFunction(replacement)) return replacement;
    var template = new Template(replacement);
    return function(match) {
      return template.evaluate(match)
    };
  }

  function gsub(pattern, replacement) {
    var result = '',
        source = this,
        match;
    replacement = prepareReplacement(replacement);

    if (Object.isString(pattern)) pattern = RegExp.escape(pattern);

    if (!(pattern.length || pattern.source)) {
      replacement = replacement('');
      return replacement + source.split('').join(replacement) + replacement;
    }

    while (source.length > 0) {
      if (match = source.match(pattern)) {
        result += source.slice(0, match.index);
        result += String.interpret(replacement(match));
        source = source.slice(match.index + match[0].length);
      } else {
        result += source, source = '';
      }
    }
    return result;
  }

  function sub(pattern, replacement, count) {
    replacement = prepareReplacement(replacement);
    count = Object.isUndefined(count) ? 1 : count;

    return this.gsub(pattern, function(match) {
      if (--count < 0) return match[0];
      return replacement(match);
    });
  }

  function scan(pattern, iterator) {
    this.gsub(pattern, iterator);
    return String(this);
  }

  function truncate(length, truncation) {
    length = length || 30;
    truncation = Object.isUndefined(truncation) ? '...' : truncation;
    return this.length > length ? this.slice(0, length - truncation.length) + truncation : String(this);
  }

  function strip() {
    return this.replace(/^\s+/, '').replace(/\s+$/, '');
  }

  function stripTags() {
    return this.replace(/<\w+(\s+("[^"]*"|'[^']*'|[^>])+)?>|<\/\w+>/gi, '');
  }

  function stripScripts() {
    return this.replace(new RegExp(Prototype.ScriptFragment, 'img'), '');
  }

  function extractScripts() {
    var matchAll = new RegExp(Prototype.ScriptFragment, 'img'),
        matchOne = new RegExp(Prototype.ScriptFragment, 'im');
    return (this.match(matchAll) || []).map(function(scriptTag) {
      return (scriptTag.match(matchOne) || ['', ''])[1];
    });
  }

  function evalScripts() {
    return this.extractScripts().map(function(script) {
      return eval(script)
    });
  }

  function escapeHTML() {
    return this.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
  }

  function unescapeHTML() {
    return this.stripTags().replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&amp;/g, '&');
  }


  function toQueryParams(separator) {
    var match = this.strip().match(/([^?#]*)(#.*)?$/);
    if (!match) return {};

    return match[1].split(separator || '&').inject({}, function(hash, pair) {
      if ((pair = pair.split('='))[0]) {
        var key = decodeURIComponent(pair.shift()),
            value = pair.length > 1 ? pair.join('=') : pair[0];

        if (value != undefined) value = decodeURIComponent(value);

        if (key in hash) {
          if (!Object.isArray(hash[key])) hash[key] = [hash[key]];
          hash[key].push(value);
        } else hash[key] = value;
      }
      return hash;
    });
  }

  function toArray() {
    return this.split('');
  }

  function succ() {
    return this.slice(0, this.length - 1) + String.fromCharCode(this.charCodeAt(this.length - 1) + 1);
  }

  function times(count) {
    return count < 1 ? '' : new Array(count + 1).join(this);
  }

  function camelize() {
    return this.replace(/-+(.)?/g, function(match, chr) {
      return chr ? chr.toUpperCase() : '';
    });
  }

  function capitalize() {
    return this.charAt(0).toUpperCase() + this.substring(1).toLowerCase();
  }

  function underscore() {
    return this.replace(/::/g, '/').replace(/([A-Z]+)([A-Z][a-z])/g, '$1_$2').replace(/([a-z\d])([A-Z])/g, '$1_$2').replace(/-/g, '_').toLowerCase();
  }

  function dasherize() {
    return this.replace(/_/g, '-');
  }

  function inspect(useDoubleQuotes) {
    var escapedString = this.replace(/[\x00-\x1f\\]/g, function(character) {
      if (character in String.specialChar) {
        return String.specialChar[character];
      }
      return '\\u00' + character.charCodeAt().toPaddedString(2, 16);
    });
    if (useDoubleQuotes) return '"' + escapedString.replace(/"/g, '\\"') + '"';
    return "'" + escapedString.replace(/'/g, '\\\'') + "'";
  }

  function unfilterJSON(filter) {
    return this.replace(filter || Prototype.JSONFilter, '$1');
  }

  function isJSON() {
    var str = this;
    if (str.blank()) return false;
    str = str.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@');
    str = str.replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']');
    str = str.replace(/(?:^|:|,)(?:\s*\[)+/g, '');
    return (/^[\],:{}\s]*$/).test(str);
  }

  function evalJSON(sanitize) {
    var json = this.unfilterJSON(),
        cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g;
    if (cx.test(json)) {
      json = json.replace(cx, function(a) {
        return '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
      });
    }
    try {
      if (!sanitize || json.isJSON()) return eval('(' + json + ')');
    } catch (e) {}
    throw new SyntaxError('Badly formed JSON string: ' + this.inspect());
  }

  function parseJSON() {
    var json = this.unfilterJSON();
    return JSON.parse(json);
  }

  function include(pattern) {
    return this.indexOf(pattern) > -1;
  }

  function startsWith(pattern) {
    return this.lastIndexOf(pattern, 0) === 0;
  }

  function endsWith(pattern) {
    var d = this.length - pattern.length;
    return d >= 0 && this.indexOf(pattern, d) === d;
  }

  function empty() {
    return this == '';
  }

  function blank() {
    return /^\s*$/.test(this);
  }

  function interpolate(object, pattern) {
    return new Template(this, pattern).evaluate(object);
  }

  return {
    gsub: gsub,
    sub: sub,
    scan: scan,
    truncate: truncate,
    strip: String.prototype.trim || strip,
    stripTags: stripTags,
    stripScripts: stripScripts,
    extractScripts: extractScripts,
    evalScripts: evalScripts,
    escapeHTML: escapeHTML,
    unescapeHTML: unescapeHTML,
    toQueryParams: toQueryParams,
    parseQuery: toQueryParams,
    toArray: toArray,
    succ: succ,
    times: times,
    camelize: camelize,
    capitalize: capitalize,
    underscore: underscore,
    dasherize: dasherize,
    inspect: inspect,
    unfilterJSON: unfilterJSON,
    isJSON: isJSON,
    evalJSON: NATIVE_JSON_PARSE_SUPPORT ? parseJSON : evalJSON,
    include: include,
    startsWith: startsWith,
    endsWith: endsWith,
    empty: empty,
    blank: blank,
    interpolate: interpolate
  };
})());

var Template = Class.create({
  initialize: function(template, pattern) {
    this.template = template.toString();
    this.pattern = pattern || Template.Pattern;
  },

  evaluate: function(object) {
    if (object && Object.isFunction(object.toTemplateReplacements)) object = object.toTemplateReplacements();

    return this.template.gsub(this.pattern, function(match) {
      if (object == null) return (match[1] + '');

      var before = match[1] || '';
      if (before == '\\') return match[2];

      var ctx = object,
          expr = match[3],
          pattern = /^([^.[]+|\[((?:.*?[^\\])?)\])(\.|\[|$)/;

      match = pattern.exec(expr);
      if (match == null) return before;

      while (match != null) {
        var comp = match[1].startsWith('[') ? match[2].replace(/\\\\]/g, ']') : match[1];
        ctx = ctx[comp];
        if (null == ctx || '' == match[3]) break;
        expr = expr.substring('[' == match[3] ? match[1].length : match[0].length);
        match = pattern.exec(expr);
      }

      return before + String.interpret(ctx);
    });
  }
});
Template.Pattern = /(^|.|\r|\n)(#\{(.*?)\})/;

var $break = {};

var Enumerable = (function() {
  function each(iterator, context) {
    var index = 0;
    try {
      this._each(function(value) {
        iterator.call(context, value, index++);
      });
    } catch (e) {
      if (e != $break) throw e;
    }
    return this;
  }

  function eachSlice(number, iterator, context) {
    var index = -number,
        slices = [],
        array = this.toArray();
    if (number < 1) return array;
    while ((index += number) < array.length)
    slices.push(array.slice(index, index + number));
    return slices.collect(iterator, context);
  }

  function all(iterator, context) {
    iterator = iterator || Prototype.K;
    var result = true;
    this.each(function(value, index) {
      result = result && !! iterator.call(context, value, index);
      if (!result) throw $break;
    });
    return result;
  }

  function any(iterator, context) {
    iterator = iterator || Prototype.K;
    var result = false;
    this.each(function(value, index) {
      if (result = !! iterator.call(context, value, index)) throw $break;
    });
    return result;
  }

  function collect(iterator, context) {
    iterator = iterator || Prototype.K;
    var results = [];
    this.each(function(value, index) {
      results.push(iterator.call(context, value, index));
    });
    return results;
  }

  function detect(iterator, context) {
    var result;
    this.each(function(value, index) {
      if (iterator.call(context, value, index)) {
        result = value;
        throw $break;
      }
    });
    return result;
  }

  function findAll(iterator, context) {
    var results = [];
    this.each(function(value, index) {
      if (iterator.call(context, value, index)) results.push(value);
    });
    return results;
  }

  function grep(filter, iterator, context) {
    iterator = iterator || Prototype.K;
    var results = [];

    if (Object.isString(filter)) filter = new RegExp(RegExp.escape(filter));

    this.each(function(value, index) {
      if (filter.match(value)) results.push(iterator.call(context, value, index));
    });
    return results;
  }

  function include(object) {
    if (Object.isFunction(this.indexOf)) if (this.indexOf(object) != -1) return true;

    var found = false;
    this.each(function(value) {
      if (value == object) {
        found = true;
        throw $break;
      }
    });
    return found;
  }

  function inGroupsOf(number, fillWith) {
    fillWith = Object.isUndefined(fillWith) ? null : fillWith;
    return this.eachSlice(number, function(slice) {
      while (slice.length < number) slice.push(fillWith);
      return slice;
    });
  }

  function inject(memo, iterator, context) {
    this.each(function(value, index) {
      memo = iterator.call(context, memo, value, index);
    });
    return memo;
  }

  function invoke(method) {
    var args = $A(arguments).slice(1);
    return this.map(function(value) {
      return value[method].apply(value, args);
    });
  }

  function max(iterator, context) {
    iterator = iterator || Prototype.K;
    var result;
    this.each(function(value, index) {
      value = iterator.call(context, value, index);
      if (result == null || value >= result) result = value;
    });
    return result;
  }

  function min(iterator, context) {
    iterator = iterator || Prototype.K;
    var result;
    this.each(function(value, index) {
      value = iterator.call(context, value, index);
      if (result == null || value < result) result = value;
    });
    return result;
  }

  function partition(iterator, context) {
    iterator = iterator || Prototype.K;
    var trues = [],
        falses = [];
    this.each(function(value, index) {
      (iterator.call(context, value, index) ? trues : falses).push(value);
    });
    return [trues, falses];
  }

  function pluck(property) {
    var results = [];
    this.each(function(value) {
      results.push(value[property]);
    });
    return results;
  }

  function reject(iterator, context) {
    var results = [];
    this.each(function(value, index) {
      if (!iterator.call(context, value, index)) results.push(value);
    });
    return results;
  }

  function sortBy(iterator, context) {
    return this.map(function(value, index) {
      return {
        value: value,
        criteria: iterator.call(context, value, index)
      };
    }).sort(function(left, right) {
      var a = left.criteria,
          b = right.criteria;
      return a < b ? -1 : a > b ? 1 : 0;
    }).pluck('value');
  }

  function toArray() {
    return this.map();
  }

  function zip() {
    var iterator = Prototype.K,
        args = $A(arguments);
    if (Object.isFunction(args.last())) iterator = args.pop();

    var collections = [this].concat(args).map($A);
    return this.map(function(value, index) {
      return iterator(collections.pluck(index));
    });
  }

  function size() {
    return this.toArray().length;
  }

  function inspect() {
    return '#<Enumerable:' + this.toArray().inspect() + '>';
  }









  return {
    each: each,
    eachSlice: eachSlice,
    all: all,
    every: all,
    any: any,
    some: any,
    collect: collect,
    map: collect,
    detect: detect,
    findAll: findAll,
    select: findAll,
    filter: findAll,
    grep: grep,
    include: include,
    member: include,
    inGroupsOf: inGroupsOf,
    inject: inject,
    invoke: invoke,
    max: max,
    min: min,
    partition: partition,
    pluck: pluck,
    reject: reject,
    sortBy: sortBy,
    toArray: toArray,
    entries: toArray,
    zip: zip,
    size: size,
    inspect: inspect,
    find: detect
  };
})();

function $A(iterable) {
  if (!iterable) return [];
  if ('toArray' in Object(iterable)) return iterable.toArray();
  var length = iterable.length || 0,
      results = new Array(length);
  while (length--) results[length] = iterable[length];
  return results;
}


function $w(string) {
  if (!Object.isString(string)) return [];
  string = string.strip();
  return string ? string.split(/\s+/) : [];
}

Array.from = $A;


(function() {
  var arrayProto = Array.prototype,
      slice = arrayProto.slice,
      _each = arrayProto.forEach; // use native browser JS 1.6 implementation if available

  function each(iterator, context) {
    for (var i = 0, length = this.length >>> 0; i < length; i++) {
      if (i in this) iterator.call(context, this[i], i, this);
    }
  }
  if (!_each) _each = each;

  function clear() {
    this.length = 0;
    return this;
  }

  function first() {
    return this[0];
  }

  function last() {
    return this[this.length - 1];
  }

  function compact() {
    return this.select(function(value) {
      return value != null;
    });
  }

  function flatten() {
    return this.inject([], function(array, value) {
      if (Object.isArray(value)) return array.concat(value.flatten());
      array.push(value);
      return array;
    });
  }

  function without() {
    var values = slice.call(arguments, 0);
    return this.select(function(value) {
      return !values.include(value);
    });
  }

  function reverse(inline) {
    return (inline === false ? this.toArray() : this)._reverse();
  }

  function uniq(sorted) {
    return this.inject([], function(array, value, index) {
      if (0 == index || (sorted ? array.last() != value : !array.include(value))) array.push(value);
      return array;
    });
  }

  function intersect(array) {
    return this.uniq().findAll(function(item) {
      return array.detect(function(value) {
        return item === value
      });
    });
  }


  function clone() {
    return slice.call(this, 0);
  }

  function size() {
    return this.length;
  }

  function inspect() {
    return '[' + this.map(Object.inspect).join(', ') + ']';
  }

  function indexOf(item, i) {
    i || (i = 0);
    var length = this.length;
    if (i < 0) i = length + i;
    for (; i < length; i++)
    if (this[i] === item) return i;
    return -1;
  }

  function lastIndexOf(item, i) {
    i = isNaN(i) ? this.length : (i < 0 ? this.length + i : i) + 1;
    var n = this.slice(0, i).reverse().indexOf(item);
    return (n < 0) ? n : i - n - 1;
  }

  function concat() {
    var array = slice.call(this, 0),
        item;
    for (var i = 0, length = arguments.length; i < length; i++) {
      item = arguments[i];
      if (Object.isArray(item) && !('callee' in item)) {
        for (var j = 0, arrayLength = item.length; j < arrayLength; j++)
        array.push(item[j]);
      } else {
        array.push(item);
      }
    }
    return array;
  }

  Object.extend(arrayProto, Enumerable);

  if (!arrayProto._reverse) arrayProto._reverse = arrayProto.reverse;

  Object.extend(arrayProto, {
    _each: _each,
    clear: clear,
    first: first,
    last: last,
    compact: compact,
    flatten: flatten,
    without: without,
    reverse: reverse,
    uniq: uniq,
    intersect: intersect,
    clone: clone,
    toArray: clone,
    size: size,
    inspect: inspect
  });

  var CONCAT_ARGUMENTS_BUGGY = (function() {
    return [].concat(arguments)[0][0] !== 1;
  })(1, 2)

  if (CONCAT_ARGUMENTS_BUGGY) arrayProto.concat = concat;

  if (!arrayProto.indexOf) arrayProto.indexOf = indexOf;
  if (!arrayProto.lastIndexOf) arrayProto.lastIndexOf = lastIndexOf;
})();

function $H(object) {
  return new Hash(object);
};

var Hash = Class.create(Enumerable, (function() {
  function initialize(object) {
    this._object = Object.isHash(object) ? object.toObject() : Object.clone(object);
  }


  function _each(iterator) {
    for (var key in this._object) {
      var value = this._object[key],
          pair = [key, value];
      pair.key = key;
      pair.value = value;
      iterator(pair);
    }
  }

  function set(key, value) {
    return this._object[key] = value;
  }

  function get(key) {
    if (this._object[key] !== Object.prototype[key]) return this._object[key];
  }

  function unset(key) {
    var value = this._object[key];
    delete this._object[key];
    return value;
  }

  function toObject() {
    return Object.clone(this._object);
  }



  function keys() {
    return this.pluck('key');
  }

  function values() {
    return this.pluck('value');
  }

  function index(value) {
    var match = this.detect(function(pair) {
      return pair.value === value;
    });
    return match && match.key;
  }

  function merge(object) {
    return this.clone().update(object);
  }

  function update(object) {
    return new Hash(object).inject(this, function(result, pair) {
      result.set(pair.key, pair.value);
      return result;
    });
  }

  function toQueryPair(key, value) {
    if (Object.isUndefined(value)) return key;
    return key + '=' + encodeURIComponent(String.interpret(value));
  }

  function toQueryString() {
    return this.inject([], function(results, pair) {
      var key = encodeURIComponent(pair.key),
          values = pair.value;

      if (values && typeof values == 'object') {
        if (Object.isArray(values)) {
          var queryValues = [];
          for (var i = 0, len = values.length, value; i < len; i++) {
            value = values[i];
            queryValues.push(toQueryPair(key, value));
          }
          return results.concat(queryValues);
        }
      } else results.push(toQueryPair(key, values));
      return results;
    }).join('&');
  }

  function inspect() {
    return '#<Hash:{' + this.map(function(pair) {
      return pair.map(Object.inspect).join(': ');
    }).join(', ') + '}>';
  }

  function clone() {
    return new Hash(this);
  }

  return {
    initialize: initialize,
    _each: _each,
    set: set,
    get: get,
    unset: unset,
    toObject: toObject,
    toTemplateReplacements: toObject,
    keys: keys,
    values: values,
    index: index,
    merge: merge,
    update: update,
    toQueryString: toQueryString,
    inspect: inspect,
    toJSON: toObject,
    clone: clone
  };
})());

Hash.from = $H;
Object.extend(Number.prototype, (function() {
  function toColorPart() {
    return this.toPaddedString(2, 16);
  }

  function succ() {
    return this + 1;
  }

  function times(iterator, context) {
    $R(0, this, true).each(iterator, context);
    return this;
  }

  function toPaddedString(length, radix) {
    var string = this.toString(radix || 10);
    return '0'.times(length - string.length) + string;
  }

  function abs() {
    return Math.abs(this);
  }

  function round() {
    return Math.round(this);
  }

  function ceil() {
    return Math.ceil(this);
  }

  function floor() {
    return Math.floor(this);
  }

  return {
    toColorPart: toColorPart,
    succ: succ,
    times: times,
    toPaddedString: toPaddedString,
    abs: abs,
    round: round,
    ceil: ceil,
    floor: floor
  };
})());

function $R(start, end, exclusive) {
  return new ObjectRange(start, end, exclusive);
}

var ObjectRange = Class.create(Enumerable, (function() {
  function initialize(start, end, exclusive) {
    this.start = start;
    this.end = end;
    this.exclusive = exclusive;
  }

  function _each(iterator) {
    var value = this.start;
    while (this.include(value)) {
      iterator(value);
      value = value.succ();
    }
  }

  function include(value) {
    if (value < this.start) return false;
    if (this.exclusive) return value < this.end;
    return value <= this.end;
  }

  return {
    initialize: initialize,
    _each: _each,
    include: include
  };
})());



var Ajax = {
  getTransport: function() {
    return Try.these(

    function() {
      return new XMLHttpRequest()
    }, function() {
      return new ActiveXObject('Msxml2.XMLHTTP')
    }, function() {
      return new ActiveXObject('Microsoft.XMLHTTP')
    }) || false;
  },

  activeRequestCount: 0
};

Ajax.Responders = {
  responders: [],

  _each: function(iterator) {
    this.responders._each(iterator);
  },

  register: function(responder) {
    if (!this.include(responder)) this.responders.push(responder);
  },

  unregister: function(responder) {
    this.responders = this.responders.without(responder);
  },

  dispatch: function(callback, request, transport, json) {
    this.each(function(responder) {
      if (Object.isFunction(responder[callback])) {
        try {
          responder[callback].apply(responder, [request, transport, json]);
        } catch (e) {}
      }
    });
  }
};

Object.extend(Ajax.Responders, Enumerable);

Ajax.Responders.register({
  onCreate: function() {
    Ajax.activeRequestCount++
  },
  onComplete: function() {
    Ajax.activeRequestCount--
  }
});
Ajax.Base = Class.create({
  initialize: function(options) {
    this.options = {
      method: 'post',
      asynchronous: true,
      contentType: 'application/x-www-form-urlencoded',
      encoding: 'UTF-8',
      parameters: '',
      evalJSON: true,
      evalJS: true
    };
    Object.extend(this.options, options || {});

    this.options.method = this.options.method.toLowerCase();

    if (Object.isHash(this.options.parameters)) this.options.parameters = this.options.parameters.toObject();
  }
});
Ajax.Request = Class.create(Ajax.Base, {
  _complete: false,

  initialize: function($super, url, options) {
    $super(options);
    this.transport = Ajax.getTransport();
    this.request(url);
  },

  request: function(url) {
    this.url = url;
    this.method = this.options.method;
    var params = Object.isString(this.options.parameters) ? this.options.parameters : Object.toQueryString(this.options.parameters);

    if (!['get', 'post'].include(this.method)) {
      params += (params ? '&' : '') + "_method=" + this.method;
      this.method = 'post';
    }

    if (params && this.method === 'get') {
      this.url += (this.url.include('?') ? '&' : '?') + params;
    }

    this.parameters = params.toQueryParams();

    try {
      var response = new Ajax.Response(this);
      if (this.options.onCreate) this.options.onCreate(response);
      Ajax.Responders.dispatch('onCreate', this, response);

      this.transport.open(this.method.toUpperCase(), this.url, this.options.asynchronous);

      if (this.options.asynchronous) this.respondToReadyState.bind(this).defer(1);

      this.transport.onreadystatechange = this.onStateChange.bind(this);
      this.setRequestHeaders();

      this.body = this.method == 'post' ? (this.options.postBody || params) : null;
      this.transport.send(this.body);

      /* Force Firefox to handle ready state 4 for synchronous requests */
      if (!this.options.asynchronous && this.transport.overrideMimeType) this.onStateChange();

    } catch (e) {
      this.dispatchException(e);
    }
  },

  onStateChange: function() {
    var readyState = this.transport.readyState;
    if (readyState > 1 && !((readyState == 4) && this._complete)) this.respondToReadyState(this.transport.readyState);
  },

  setRequestHeaders: function() {
    var headers = {
      'X-Requested-With': 'XMLHttpRequest',
      'X-Prototype-Version': Prototype.Version,
      'Accept': 'text/javascript, text/html, application/xml, text/xml, */*'
    };

    if (this.method == 'post') {
      headers['Content-type'] = this.options.contentType + (this.options.encoding ? '; charset=' + this.options.encoding : '');

/* Force "Connection: close" for older Mozilla browsers to work
       * around a bug where XMLHttpRequest sends an incorrect
       * Content-length header. See Mozilla Bugzilla #246651.
       */
      if (this.transport.overrideMimeType && (navigator.userAgent.match(/Gecko\/(\d{4})/) || [0, 2005])[1] < 2005) headers['Connection'] = 'close';
    }

    if (typeof this.options.requestHeaders == 'object') {
      var extras = this.options.requestHeaders;

      if (Object.isFunction(extras.push)) for (var i = 0, length = extras.length; i < length; i += 2)
      headers[extras[i]] = extras[i + 1];
      else
      $H(extras).each(function(pair) {
        headers[pair.key] = pair.value
      });
    }

    for (var name in headers)
    this.transport.setRequestHeader(name, headers[name]);
  },

  success: function() {
    var status = this.getStatus();
    return !status || (status >= 200 && status < 300) || status == 304;
  },

  getStatus: function() {
    try {
      if (this.transport.status === 1223) return 204;
      return this.transport.status || 0;
    } catch (e) {
      return 0
    }
  },

  respondToReadyState: function(readyState) {
    var state = Ajax.Request.Events[readyState],
        response = new Ajax.Response(this);

    if (state == 'Complete') {
      try {
        this._complete = true;
        (this.options['on' + response.status] || this.options['on' + (this.success() ? 'Success' : 'Failure')] || Prototype.emptyFunction)(response, response.headerJSON);
      } catch (e) {
        this.dispatchException(e);
      }

      var contentType = response.getHeader('Content-type');
      if (this.options.evalJS == 'force' || (this.options.evalJS && this.isSameOrigin() && contentType && contentType.match(/^\s*(text|application)\/(x-)?(java|ecma)script(;.*)?\s*$/i))) this.evalResponse();
    }

    try {
      (this.options['on' + state] || Prototype.emptyFunction)(response, response.headerJSON);
      Ajax.Responders.dispatch('on' + state, this, response, response.headerJSON);
    } catch (e) {
      this.dispatchException(e);
    }

    if (state == 'Complete') {
      this.transport.onreadystatechange = Prototype.emptyFunction;
    }
  },

  isSameOrigin: function() {
    var m = this.url.match(/^\s*https?:\/\/[^\/]*/);
    return !m || (m[0] == '#{protocol}//#{domain}#{port}'.interpolate({
      protocol: location.protocol,
      domain: document.domain,
      port: location.port ? ':' + location.port : ''
    }));
  },

  getHeader: function(name) {
    try {
      return this.transport.getResponseHeader(name) || null;
    } catch (e) {
      return null;
    }
  },

  evalResponse: function() {
    try {
      return eval((this.transport.responseText || '').unfilterJSON());
    } catch (e) {
      this.dispatchException(e);
    }
  },

  dispatchException: function(exception) {
    (this.options.onException || Prototype.emptyFunction)(this, exception);
    Ajax.Responders.dispatch('onException', this, exception);
  }
});

Ajax.Request.Events = ['Uninitialized', 'Loading', 'Loaded', 'Interactive', 'Complete'];








Ajax.Response = Class.create({
  initialize: function(request) {
    this.request = request;
    var transport = this.transport = request.transport,
        readyState = this.readyState = transport.readyState;

    if ((readyState > 2 && !Prototype.Browser.IE) || readyState == 4) {
      this.status = this.getStatus();
      this.statusText = this.getStatusText();
      this.responseText = String.interpret(transport.responseText);
      this.headerJSON = this._getHeaderJSON();
    }

    if (readyState == 4) {
      var xml = transport.responseXML;
      this.responseXML = Object.isUndefined(xml) ? null : xml;
      this.responseJSON = this._getResponseJSON();
    }
  },

  status: 0,

  statusText: '',

  getStatus: Ajax.Request.prototype.getStatus,

  getStatusText: function() {
    try {
      return this.transport.statusText || '';
    } catch (e) {
      return ''
    }
  },

  getHeader: Ajax.Request.prototype.getHeader,

  getAllHeaders: function() {
    try {
      return this.getAllResponseHeaders();
    } catch (e) {
      return null
    }
  },

  getResponseHeader: function(name) {
    return this.transport.getResponseHeader(name);
  },

  getAllResponseHeaders: function() {
    return this.transport.getAllResponseHeaders();
  },

  _getHeaderJSON: function() {
    var json = this.getHeader('X-JSON');
    if (!json) return null;
    json = decodeURIComponent(escape(json));
    try {
      return json.evalJSON(this.request.options.sanitizeJSON || !this.request.isSameOrigin());
    } catch (e) {
      this.request.dispatchException(e);
    }
  },

  _getResponseJSON: function() {
    var options = this.request.options;
    if (!options.evalJSON || (options.evalJSON != 'force' && !(this.getHeader('Content-type') || '').include('application/json')) || this.responseText.blank()) return null;
    try {
      return this.responseText.evalJSON(options.sanitizeJSON || !this.request.isSameOrigin());
    } catch (e) {
      this.request.dispatchException(e);
    }
  }
});

Ajax.Updater = Class.create(Ajax.Request, {
  initialize: function($super, container, url, options) {
    this.container = {
      success: (container.success || container),
      failure: (container.failure || (container.success ? null : container))
    };

    options = Object.clone(options);
    var onComplete = options.onComplete;
    options.onComplete = (function(response, json) {
      this.updateContent(response.responseText);
      if (Object.isFunction(onComplete)) onComplete(response, json);
    }).bind(this);

    $super(url, options);
  },

  updateContent: function(responseText) {
    var receiver = this.container[this.success() ? 'success' : 'failure'],
        options = this.options;

    if (!options.evalScripts) responseText = responseText.stripScripts();

    if (receiver = $(receiver)) {
      if (options.insertion) {
        if (Object.isString(options.insertion)) {
          var insertion = {};
          insertion[options.insertion] = responseText;
          receiver.insert(insertion);
        } else options.insertion(receiver, responseText);
      } else receiver.update(responseText);
    }
  }
});

Ajax.PeriodicalUpdater = Class.create(Ajax.Base, {
  initialize: function($super, container, url, options) {
    $super(options);
    this.onComplete = this.options.onComplete;

    this.frequency = (this.options.frequency || 2);
    this.decay = (this.options.decay || 1);

    this.updater = {};
    this.container = container;
    this.url = url;

    this.start();
  },

  start: function() {
    this.options.onComplete = this.updateComplete.bind(this);
    this.onTimerEvent();
  },

  stop: function() {
    this.updater.options.onComplete = undefined;
    clearTimeout(this.timer);
    (this.onComplete || Prototype.emptyFunction).apply(this, arguments);
  },

  updateComplete: function(response) {
    if (this.options.decay) {
      this.decay = (response.responseText == this.lastText ? this.decay * this.options.decay : 1);

      this.lastText = response.responseText;
    }
    this.timer = this.onTimerEvent.bind(this).delay(this.decay * this.frequency);
  },

  onTimerEvent: function() {
    this.updater = new Ajax.Updater(this.container, this.url, this.options);
  }
});


function $(element) {
  if (arguments.length > 1) {
    for (var i = 0, elements = [], length = arguments.length; i < length; i++)
    elements.push($(arguments[i]));
    return elements;
  }
  if (Object.isString(element)) element = document.getElementById(element);
  return Element.extend(element);
}

if (Prototype.BrowserFeatures.XPath) {
  document._getElementsByXPath = function(expression, parentElement) {
    var results = [];
    var query = document.evaluate(expression, $(parentElement) || document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
    for (var i = 0, length = query.snapshotLength; i < length; i++)
    results.push(Element.extend(query.snapshotItem(i)));
    return results;
  };
}

/*--------------------------------------------------------------------------*/

if (!Node) var Node = {};

if (!Node.ELEMENT_NODE) {
  Object.extend(Node, {
    ELEMENT_NODE: 1,
    ATTRIBUTE_NODE: 2,
    TEXT_NODE: 3,
    CDATA_SECTION_NODE: 4,
    ENTITY_REFERENCE_NODE: 5,
    ENTITY_NODE: 6,
    PROCESSING_INSTRUCTION_NODE: 7,
    COMMENT_NODE: 8,
    DOCUMENT_NODE: 9,
    DOCUMENT_TYPE_NODE: 10,
    DOCUMENT_FRAGMENT_NODE: 11,
    NOTATION_NODE: 12
  });
}



(function(global) {
  function shouldUseCache(tagName, attributes) {
    if (tagName === 'select') return false;
    if ('type' in attributes) return false;
    return true;
  }

  var HAS_EXTENDED_CREATE_ELEMENT_SYNTAX = (function() {
    try {
      var el = document.createElement('<input name="x">');
      return el.tagName.toLowerCase() === 'input' && el.name === 'x';
    } catch (err) {
      return false;
    }
  })();

  var element = global.Element;

  global.Element = function(tagName, attributes) {
    attributes = attributes || {};
    tagName = tagName.toLowerCase();
    var cache = Element.cache;

    if (HAS_EXTENDED_CREATE_ELEMENT_SYNTAX && attributes.name) {
      tagName = '<' + tagName + ' name="' + attributes.name + '">';
      delete attributes.name;
      return Element.writeAttribute(document.createElement(tagName), attributes);
    }

    if (!cache[tagName]) cache[tagName] = Element.extend(document.createElement(tagName));

    var node = shouldUseCache(tagName, attributes) ? cache[tagName].cloneNode(false) : document.createElement(tagName);

    return Element.writeAttribute(node, attributes);
  };

  Object.extend(global.Element, element || {});
  if (element) global.Element.prototype = element.prototype;

})(this);

Element.idCounter = 1;
Element.cache = {};

Element._purgeElement = function(element) {
  var uid = element._prototypeUID;
  if (uid) {
    Element.stopObserving(element);
    element._prototypeUID = void 0;
    delete Element.Storage[uid];
  }
}

Element.Methods = {
  visible: function(element) {
    return $(element).style.display != 'none';
  },

  toggle: function(element) {
    element = $(element);
    Element[Element.visible(element) ? 'hide' : 'show'](element);
    return element;
  },

  hide: function(element) {
    element = $(element);
    element.style.display = 'none';
    return element;
  },

  show: function(element) {
    element = $(element);
    element.style.display = '';
    return element;
  },

  remove: function(element) {
    element = $(element);
    element.parentNode.removeChild(element);
    return element;
  },

  update: (function() {

    var SELECT_ELEMENT_INNERHTML_BUGGY = (function() {
      var el = document.createElement("select"),
          isBuggy = true;
      el.innerHTML = "<option value=\"test\">test</option>";
      if (el.options && el.options[0]) {
        isBuggy = el.options[0].nodeName.toUpperCase() !== "OPTION";
      }
      el = null;
      return isBuggy;
    })();

    var TABLE_ELEMENT_INNERHTML_BUGGY = (function() {
      try {
        var el = document.createElement("table");
        if (el && el.tBodies) {
          el.innerHTML = "<tbody><tr><td>test</td></tr></tbody>";
          var isBuggy = typeof el.tBodies[0] == "undefined";
          el = null;
          return isBuggy;
        }
      } catch (e) {
        return true;
      }
    })();

    var LINK_ELEMENT_INNERHTML_BUGGY = (function() {
      try {
        var el = document.createElement('div');
        el.innerHTML = "<link>";
        var isBuggy = (el.childNodes.length === 0);
        el = null;
        return isBuggy;
      } catch (e) {
        return true;
      }
    })();

    var ANY_INNERHTML_BUGGY = SELECT_ELEMENT_INNERHTML_BUGGY || TABLE_ELEMENT_INNERHTML_BUGGY || LINK_ELEMENT_INNERHTML_BUGGY;

    var SCRIPT_ELEMENT_REJECTS_TEXTNODE_APPENDING = (function() {
      var s = document.createElement("script"),
          isBuggy = false;
      try {
        s.appendChild(document.createTextNode(""));
        isBuggy = !s.firstChild || s.firstChild && s.firstChild.nodeType !== 3;
      } catch (e) {
        isBuggy = true;
      }
      s = null;
      return isBuggy;
    })();


    function update(element, content) {
      element = $(element);
      var purgeElement = Element._purgeElement;

      var descendants = element.getElementsByTagName('*'),
          i = descendants.length;
      while (i--) purgeElement(descendants[i]);

      if (content && content.toElement) content = content.toElement();

      if (Object.isElement(content)) return element.update().insert(content);

      content = Object.toHTML(content);

      var tagName = element.tagName.toUpperCase();

      if (tagName === 'SCRIPT' && SCRIPT_ELEMENT_REJECTS_TEXTNODE_APPENDING) {
        element.text = content;
        return element;
      }

      if (ANY_INNERHTML_BUGGY) {
        if (tagName in Element._insertionTranslations.tags) {
          while (element.firstChild) {
            element.removeChild(element.firstChild);
          }
          Element._getContentFromAnonymousElement(tagName, content.stripScripts()).each(function(node) {
            element.appendChild(node)
          });
        } else if (LINK_ELEMENT_INNERHTML_BUGGY && Object.isString(content) && content.indexOf('<link') > -1) {
          while (element.firstChild) {
            element.removeChild(element.firstChild);
          }
          var nodes = Element._getContentFromAnonymousElement(tagName, content.stripScripts(), true);
          nodes.each(function(node) {
            element.appendChild(node)
          });
        } else {
          element.innerHTML = content.stripScripts();
        }
      } else {
        element.innerHTML = content.stripScripts();
      }

      content.evalScripts.bind(content).defer();
      return element;
    }

    return update;
  })(),

  replace: function(element, content) {
    element = $(element);
    if (content && content.toElement) content = content.toElement();
    else if (!Object.isElement(content)) {
      content = Object.toHTML(content);
      var range = element.ownerDocument.createRange();
      range.selectNode(element);
      content.evalScripts.bind(content).defer();
      content = range.createContextualFragment(content.stripScripts());
    }
    element.parentNode.replaceChild(content, element);
    return element;
  },

  insert: function(element, insertions) {
    element = $(element);

    if (Object.isString(insertions) || Object.isNumber(insertions) || Object.isElement(insertions) || (insertions && (insertions.toElement || insertions.toHTML))) insertions = {
      bottom: insertions
    };

    var content, insert, tagName, childNodes;

    for (var position in insertions) {
      content = insertions[position];
      position = position.toLowerCase();
      insert = Element._insertionTranslations[position];

      if (content && content.toElement) content = content.toElement();
      if (Object.isElement(content)) {
        insert(element, content);
        continue;
      }

      content = Object.toHTML(content);

      tagName = ((position == 'before' || position == 'after') ? element.parentNode : element).tagName.toUpperCase();

      childNodes = Element._getContentFromAnonymousElement(tagName, content.stripScripts());

      if (position == 'top' || position == 'after') childNodes.reverse();
      childNodes.each(insert.curry(element));

      content.evalScripts.bind(content).defer();
    }

    return element;
  },

  wrap: function(element, wrapper, attributes) {
    element = $(element);
    if (Object.isElement(wrapper)) $(wrapper).writeAttribute(attributes || {});
    else if (Object.isString(wrapper)) wrapper = new Element(wrapper, attributes);
    else wrapper = new Element('div', wrapper);
    if (element.parentNode) element.parentNode.replaceChild(wrapper, element);
    wrapper.appendChild(element);
    return wrapper;
  },

  inspect: function(element) {
    element = $(element);
    var result = '<' + element.tagName.toLowerCase();
    $H({
      'id': 'id',
      'className': 'class'
    }).each(function(pair) {
      var property = pair.first(),
          attribute = pair.last(),
          value = (element[property] || '').toString();
      if (value) result += ' ' + attribute + '=' + value.inspect(true);
    });
    return result + '>';
  },

  recursivelyCollect: function(element, property, maximumLength) {
    element = $(element);
    maximumLength = maximumLength || -1;
    var elements = [];

    while (element = element[property]) {
      if (element.nodeType == 1) elements.push(Element.extend(element));
      if (elements.length == maximumLength) break;
    }

    return elements;
  },

  ancestors: function(element) {
    return Element.recursivelyCollect(element, 'parentNode');
  },

  descendants: function(element) {
    return Element.select(element, "*");
  },

  firstDescendant: function(element) {
    element = $(element).firstChild;
    while (element && element.nodeType != 1) element = element.nextSibling;
    return $(element);
  },

  immediateDescendants: function(element) {
    var results = [],
        child = $(element).firstChild;
    while (child) {
      if (child.nodeType === 1) {
        results.push(Element.extend(child));
      }
      child = child.nextSibling;
    }
    return results;
  },

  previousSiblings: function(element, maximumLength) {
    return Element.recursivelyCollect(element, 'previousSibling');
  },

  nextSiblings: function(element) {
    return Element.recursivelyCollect(element, 'nextSibling');
  },

  siblings: function(element) {
    element = $(element);
    return Element.previousSiblings(element).reverse().concat(Element.nextSiblings(element));
  },

  match: function(element, selector) {
    element = $(element);
    if (Object.isString(selector)) return Prototype.Selector.match(element, selector);
    return selector.match(element);
  },

  up: function(element, expression, index) {
    element = $(element);
    if (arguments.length == 1) return $(element.parentNode);
    var ancestors = Element.ancestors(element);
    return Object.isNumber(expression) ? ancestors[expression] : Prototype.Selector.find(ancestors, expression, index);
  },

  down: function(element, expression, index) {
    element = $(element);
    if (arguments.length == 1) return Element.firstDescendant(element);
    return Object.isNumber(expression) ? Element.descendants(element)[expression] : Element.select(element, expression)[index || 0];
  },

  previous: function(element, expression, index) {
    element = $(element);
    if (Object.isNumber(expression)) index = expression, expression = false;
    if (!Object.isNumber(index)) index = 0;

    if (expression) {
      return Prototype.Selector.find(element.previousSiblings(), expression, index);
    } else {
      return element.recursivelyCollect("previousSibling", index + 1)[index];
    }
  },

  next: function(element, expression, index) {
    element = $(element);
    if (Object.isNumber(expression)) index = expression, expression = false;
    if (!Object.isNumber(index)) index = 0;

    if (expression) {
      return Prototype.Selector.find(element.nextSiblings(), expression, index);
    } else {
      var maximumLength = Object.isNumber(index) ? index + 1 : 1;
      return element.recursivelyCollect("nextSibling", index + 1)[index];
    }
  },


  select: function(element) {
    element = $(element);
    var expressions = Array.prototype.slice.call(arguments, 1).join(', ');
    return Prototype.Selector.select(expressions, element);
  },

  adjacent: function(element) {
    element = $(element);
    var expressions = Array.prototype.slice.call(arguments, 1).join(', ');
    return Prototype.Selector.select(expressions, element.parentNode).without(element);
  },

  identify: function(element) {
    element = $(element);
    var id = Element.readAttribute(element, 'id');
    if (id) return id;
    do {
      id = 'anonymous_element_' + Element.idCounter++
    } while ($(id));
    Element.writeAttribute(element, 'id', id);
    return id;
  },

  readAttribute: function(element, name) {
    element = $(element);
    if (Prototype.Browser.IE) {
      var t = Element._attributeTranslations.read;
      if (t.values[name]) return t.values[name](element, name);
      if (t.names[name]) name = t.names[name];
      if (name.include(':')) {
        return (!element.attributes || !element.attributes[name]) ? null : element.attributes[name].value;
      }
    }
    return element.getAttribute(name);
  },

  writeAttribute: function(element, name, value) {
    element = $(element);
    var attributes = {},
        t = Element._attributeTranslations.write;

    if (typeof name == 'object') attributes = name;
    else attributes[name] = Object.isUndefined(value) ? true : value;

    for (var attr in attributes) {
      name = t.names[attr] || attr;
      value = attributes[attr];
      if (t.values[attr]) name = t.values[attr](element, value);
      if (value === false || value === null) element.removeAttribute(name);
      else if (value === true) element.setAttribute(name, name);
      else element.setAttribute(name, value);
    }
    return element;
  },

  getHeight: function(element) {
    return Element.getDimensions(element).height;
  },

  getWidth: function(element) {
    return Element.getDimensions(element).width;
  },

  classNames: function(element) {
    return new Element.ClassNames(element);
  },

  hasClassName: function(element, className) {
    if (!(element = $(element))) return;
    var elementClassName = element.className;
    return (elementClassName.length > 0 && (elementClassName == className || new RegExp("(^|\\s)" + className + "(\\s|$)").test(elementClassName)));
  },

  addClassName: function(element, className) {
    if (!(element = $(element))) return;
    if (!Element.hasClassName(element, className)) element.className += (element.className ? ' ' : '') + className;
    return element;
  },

  removeClassName: function(element, className) {
    if (!(element = $(element))) return;
    element.className = element.className.replace(
    new RegExp("(^|\\s+)" + className + "(\\s+|$)"), ' ').strip();
    return element;
  },

  toggleClassName: function(element, className) {
    if (!(element = $(element))) return;
    return Element[Element.hasClassName(element, className) ? 'removeClassName' : 'addClassName'](element, className);
  },

  cleanWhitespace: function(element) {
    element = $(element);
    var node = element.firstChild;
    while (node) {
      var nextNode = node.nextSibling;
      if (node.nodeType == 3 && !/\S/.test(node.nodeValue)) element.removeChild(node);
      node = nextNode;
    }
    return element;
  },

  empty: function(element) {
    return $(element).innerHTML.blank();
  },

  descendantOf: function(element, ancestor) {
    element = $(element), ancestor = $(ancestor);

    if (element.compareDocumentPosition) return (element.compareDocumentPosition(ancestor) & 8) === 8;

    if (ancestor.contains) return ancestor.contains(element) && ancestor !== element;

    while (element = element.parentNode)
    if (element == ancestor) return true;

    return false;
  },

  scrollTo: function(element) {
    element = $(element);
    var pos = Element.cumulativeOffset(element);
    window.scrollTo(pos[0], pos[1]);
    return element;
  },

  getStyle: function(element, style) {
    element = $(element);
    style = style == 'float' ? 'cssFloat' : style.camelize();
    var value = element.style[style];
    if (!value || value == 'auto') {
      var css = document.defaultView.getComputedStyle(element, null);
      value = css ? css[style] : null;
    }
    if (style == 'opacity') return value ? parseFloat(value) : 1.0;
    return value == 'auto' ? null : value;
  },

  getOpacity: function(element) {
    return $(element).getStyle('opacity');
  },

  setStyle: function(element, styles) {
    element = $(element);
    var elementStyle = element.style,
        match;
    if (Object.isString(styles)) {
      element.style.cssText += ';' + styles;
      return styles.include('opacity') ? element.setOpacity(styles.match(/opacity:\s*(\d?\.?\d*)/)[1]) : element;
    }
    for (var property in styles)
    if (property == 'opacity') element.setOpacity(styles[property]);
    else
    elementStyle[(property == 'float' || property == 'cssFloat') ? (Object.isUndefined(elementStyle.styleFloat) ? 'cssFloat' : 'styleFloat') : property] = styles[property];

    return element;
  },

  setOpacity: function(element, value) {
    element = $(element);
    element.style.opacity = (value == 1 || value === '') ? '' : (value < 0.00001) ? 0 : value;
    return element;
  },

  makePositioned: function(element) {
    element = $(element);
    var pos = Element.getStyle(element, 'position');
    if (pos == 'static' || !pos) {
      element._madePositioned = true;
      element.style.position = 'relative';
      if (Prototype.Browser.Opera) {
        element.style.top = 0;
        element.style.left = 0;
      }
    }
    return element;
  },

  undoPositioned: function(element) {
    element = $(element);
    if (element._madePositioned) {
      element._madePositioned = undefined;
      element.style.position =
      element.style.top =
      element.style.left =
      element.style.bottom =
      element.style.right = '';
    }
    return element;
  },

  makeClipping: function(element) {
    element = $(element);
    if (element._overflow) return element;
    element._overflow = Element.getStyle(element, 'overflow') || 'auto';
    if (element._overflow !== 'hidden') element.style.overflow = 'hidden';
    return element;
  },

  undoClipping: function(element) {
    element = $(element);
    if (!element._overflow) return element;
    element.style.overflow = element._overflow == 'auto' ? '' : element._overflow;
    element._overflow = null;
    return element;
  },

  clonePosition: function(element, source) {
    var options = Object.extend({
      setLeft: true,
      setTop: true,
      setWidth: true,
      setHeight: true,
      offsetTop: 0,
      offsetLeft: 0
    }, arguments[2] || {});

    source = $(source);
    var p = Element.viewportOffset(source),
        delta = [0, 0],
        parent = null;

    element = $(element);

    if (Element.getStyle(element, 'position') == 'absolute') {
      parent = Element.getOffsetParent(element);
      delta = Element.viewportOffset(parent);
    }

    if (parent == document.body) {
      delta[0] -= document.body.offsetLeft;
      delta[1] -= document.body.offsetTop;
    }

    if (options.setLeft) element.style.left = (p[0] - delta[0] + options.offsetLeft) + 'px';
    if (options.setTop) element.style.top = (p[1] - delta[1] + options.offsetTop) + 'px';
    if (options.setWidth) element.style.width = source.offsetWidth + 'px';
    if (options.setHeight) element.style.height = source.offsetHeight + 'px';
    return element;
  }
};

Object.extend(Element.Methods, {
  getElementsBySelector: Element.Methods.select,

  childElements: Element.Methods.immediateDescendants
});

Element._attributeTranslations = {
  write: {
    names: {
      className: 'class',
      htmlFor: 'for'
    },
    values: {}
  }
};

if (Prototype.Browser.Opera) {
  Element.Methods.getStyle = Element.Methods.getStyle.wrap(

  function(proceed, element, style) {
    switch (style) {
    case 'height':
    case 'width':
      if (!Element.visible(element)) return null;

      var dim = parseInt(proceed(element, style), 10);

      if (dim !== element['offset' + style.capitalize()]) return dim + 'px';

      var properties;
      if (style === 'height') {
        properties = ['border-top-width', 'padding-top', 'padding-bottom', 'border-bottom-width'];
      } else {
        properties = ['border-left-width', 'padding-left', 'padding-right', 'border-right-width'];
      }
      return properties.inject(dim, function(memo, property) {
        var val = proceed(element, property);
        return val === null ? memo : memo - parseInt(val, 10);
      }) + 'px';
    default:
      return proceed(element, style);
    }
  });

  Element.Methods.readAttribute = Element.Methods.readAttribute.wrap(

  function(proceed, element, attribute) {
    if (attribute === 'title') return element.title;
    return proceed(element, attribute);
  });
} else if (Prototype.Browser.IE) {
  Element.Methods.getStyle = function(element, style) {
    element = $(element);
    style = (style == 'float' || style == 'cssFloat') ? 'styleFloat' : style.camelize();
    var value = element.style[style];
    if (!value && element.currentStyle) value = element.currentStyle[style];

    if (style == 'opacity') {
      if (value = (element.getStyle('filter') || '').match(/alpha\(opacity=(.*)\)/)) if (value[1]) return parseFloat(value[1]) / 100;
      return 1.0;
    }

    if (value == 'auto') {
      if ((style == 'width' || style == 'height') && (element.getStyle('display') != 'none')) return element['offset' + style.capitalize()] + 'px';
      return null;
    }
    return value;
  };

  Element.Methods.setOpacity = function(element, value) {
    function stripAlpha(filter) {
      return filter.replace(/alpha\([^\)]*\)/gi, '');
    }
    element = $(element);
    var currentStyle = element.currentStyle;
    if ((currentStyle && !currentStyle.hasLayout) || (!currentStyle && element.style.zoom == 'normal')) element.style.zoom = 1;

    var filter = element.getStyle('filter'),
        style = element.style;
    if (value == 1 || value === '') {
      (filter = stripAlpha(filter)) ? style.filter = filter : style.removeAttribute('filter');
      return element;
    } else if (value < 0.00001) value = 0;
    style.filter = stripAlpha(filter) + 'alpha(opacity=' + (value * 100) + ')';
    return element;
  };

  Element._attributeTranslations = (function() {

    var classProp = 'className',
        forProp = 'for',
        el = document.createElement('div');

    el.setAttribute(classProp, 'x');

    if (el.className !== 'x') {
      el.setAttribute('class', 'x');
      if (el.className === 'x') {
        classProp = 'class';
      }
    }
    el = null;

    el = document.createElement('label');
    el.setAttribute(forProp, 'x');
    if (el.htmlFor !== 'x') {
      el.setAttribute('htmlFor', 'x');
      if (el.htmlFor === 'x') {
        forProp = 'htmlFor';
      }
    }
    el = null;

    return {
      read: {
        names: {
          'class': classProp,
          'className': classProp,
          'for': forProp,
          'htmlFor': forProp
        },
        values: {
          _getAttr: function(element, attribute) {
            return element.getAttribute(attribute);
          },
          _getAttr2: function(element, attribute) {
            return element.getAttribute(attribute, 2);
          },
          _getAttrNode: function(element, attribute) {
            var node = element.getAttributeNode(attribute);
            return node ? node.value : "";
          },
          _getEv: (function() {

            var el = document.createElement('div'),
                f;
            el.onclick = Prototype.emptyFunction;
            var value = el.getAttribute('onclick');

            if (String(value).indexOf('{') > -1) {
              f = function(element, attribute) {
                attribute = element.getAttribute(attribute);
                if (!attribute) return null;
                attribute = attribute.toString();
                attribute = attribute.split('{')[1];
                attribute = attribute.split('}')[0];
                return attribute.strip();
              };
            } else if (value === '') {
              f = function(element, attribute) {
                attribute = element.getAttribute(attribute);
                if (!attribute) return null;
                return attribute.strip();
              };
            }
            el = null;
            return f;
          })(),
          _flag: function(element, attribute) {
            return $(element).hasAttribute(attribute) ? attribute : null;
          },
          style: function(element) {
            return element.style.cssText.toLowerCase();
          },
          title: function(element) {
            return element.title;
          }
        }
      }
    }
  })();

  Element._attributeTranslations.write = {
    names: Object.extend({
      cellpadding: 'cellPadding',
      cellspacing: 'cellSpacing'
    }, Element._attributeTranslations.read.names),
    values: {
      checked: function(element, value) {
        element.checked = !! value;
      },

      style: function(element, value) {
        element.style.cssText = value ? value : '';
      }
    }
  };

  Element._attributeTranslations.has = {};

  $w('colSpan rowSpan vAlign dateTime accessKey tabIndex ' + 'encType maxLength readOnly longDesc frameBorder').each(function(attr) {
    Element._attributeTranslations.write.names[attr.toLowerCase()] = attr;
    Element._attributeTranslations.has[attr.toLowerCase()] = attr;
  });

  (function(v) {
    Object.extend(v, {
      href: v._getAttr2,
      src: v._getAttr2,
      type: v._getAttr,
      action: v._getAttrNode,
      disabled: v._flag,
      checked: v._flag,
      readonly: v._flag,
      multiple: v._flag,
      onload: v._getEv,
      onunload: v._getEv,
      onclick: v._getEv,
      ondblclick: v._getEv,
      onmousedown: v._getEv,
      onmouseup: v._getEv,
      onmouseover: v._getEv,
      onmousemove: v._getEv,
      onmouseout: v._getEv,
      onfocus: v._getEv,
      onblur: v._getEv,
      onkeypress: v._getEv,
      onkeydown: v._getEv,
      onkeyup: v._getEv,
      onsubmit: v._getEv,
      onreset: v._getEv,
      onselect: v._getEv,
      onchange: v._getEv
    });
  })(Element._attributeTranslations.read.values);

  if (Prototype.BrowserFeatures.ElementExtensions) {
    (function() {
      function _descendants(element) {
        var nodes = element.getElementsByTagName('*'),
            results = [];
        for (var i = 0, node; node = nodes[i]; i++)
        if (node.tagName !== "!") // Filter out comment nodes.
        results.push(node);
        return results;
      }

      Element.Methods.down = function(element, expression, index) {
        element = $(element);
        if (arguments.length == 1) return element.firstDescendant();
        return Object.isNumber(expression) ? _descendants(element)[expression] : Element.select(element, expression)[index || 0];
      }
    })();
  }

} else if (Prototype.Browser.Gecko && /rv:1\.8\.0/.test(navigator.userAgent)) {
  Element.Methods.setOpacity = function(element, value) {
    element = $(element);
    element.style.opacity = (value == 1) ? 0.999999 : (value === '') ? '' : (value < 0.00001) ? 0 : value;
    return element;
  };
} else if (Prototype.Browser.WebKit) {
  Element.Methods.setOpacity = function(element, value) {
    element = $(element);
    element.style.opacity = (value == 1 || value === '') ? '' : (value < 0.00001) ? 0 : value;

    if (value == 1) if (element.tagName.toUpperCase() == 'IMG' && element.width) {
      element.width++;
      element.width--;
    } else
    try {
      var n = document.createTextNode(' ');
      element.appendChild(n);
      element.removeChild(n);
    } catch (e) {}

    return element;
  };
}

if ('outerHTML' in document.documentElement) {
  Element.Methods.replace = function(element, content) {
    element = $(element);

    if (content && content.toElement) content = content.toElement();
    if (Object.isElement(content)) {
      element.parentNode.replaceChild(content, element);
      return element;
    }

    content = Object.toHTML(content);
    var parent = element.parentNode,
        tagName = parent.tagName.toUpperCase();

    if (Element._insertionTranslations.tags[tagName]) {
      var nextSibling = element.next(),
          fragments = Element._getContentFromAnonymousElement(tagName, content.stripScripts());
      parent.removeChild(element);
      if (nextSibling) fragments.each(function(node) {
        parent.insertBefore(node, nextSibling)
      });
      else
      fragments.each(function(node) {
        parent.appendChild(node)
      });
    } else element.outerHTML = content.stripScripts();

    content.evalScripts.bind(content).defer();
    return element;
  };
}

Element._returnOffset = function(l, t) {
  var result = [l, t];
  result.left = l;
  result.top = t;
  return result;
};

Element._getContentFromAnonymousElement = function(tagName, html, force) {
  var div = new Element('div'),
      t = Element._insertionTranslations.tags[tagName];

  var workaround = false;
  if (t) workaround = true;
  else if (force) {
    workaround = true;
    t = ['', '', 0];
  }

  if (workaround) {
    div.innerHTML = '&nbsp;' + t[0] + html + t[1];
    div.removeChild(div.firstChild);
    for (var i = t[2]; i--;) {
      div = div.firstChild;
    }
  } else {
    div.innerHTML = html;
  }
  return $A(div.childNodes);
};

Element._insertionTranslations = {
  before: function(element, node) {
    element.parentNode.insertBefore(node, element);
  },
  top: function(element, node) {
    element.insertBefore(node, element.firstChild);
  },
  bottom: function(element, node) {
    element.appendChild(node);
  },
  after: function(element, node) {
    element.parentNode.insertBefore(node, element.nextSibling);
  },
  tags: {
    TABLE: ['<table>', '</table>', 1],
    TBODY: ['<table><tbody>', '</tbody></table>', 2],
    TR: ['<table><tbody><tr>', '</tr></tbody></table>', 3],
    TD: ['<table><tbody><tr><td>', '</td></tr></tbody></table>', 4],
    SELECT: ['<select>', '</select>', 1]
  }
};

(function() {
  var tags = Element._insertionTranslations.tags;
  Object.extend(tags, {
    THEAD: tags.TBODY,
    TFOOT: tags.TBODY,
    TH: tags.TD
  });
})();

Element.Methods.Simulated = {
  hasAttribute: function(element, attribute) {
    attribute = Element._attributeTranslations.has[attribute] || attribute;
    var node = $(element).getAttributeNode(attribute);
    return !!(node && node.specified);
  }
};

Element.Methods.ByTag = {};

Object.extend(Element, Element.Methods);

(function(div) {

  if (!Prototype.BrowserFeatures.ElementExtensions && div['__proto__']) {
    window.HTMLElement = {};
    window.HTMLElement.prototype = div['__proto__'];
    Prototype.BrowserFeatures.ElementExtensions = true;
  }

  div = null;

})(document.createElement('div'));

Element.extend = (function() {

  function checkDeficiency(tagName) {
    if (typeof window.Element != 'undefined') {
      var proto = window.Element.prototype;
      if (proto) {
        var id = '_' + (Math.random() + '').slice(2),
            el = document.createElement(tagName);
        proto[id] = 'x';
        var isBuggy = (el[id] !== 'x');
        delete proto[id];
        el = null;
        return isBuggy;
      }
    }
    return false;
  }

  function extendElementWith(element, methods) {
    for (var property in methods) {
      var value = methods[property];
      if (Object.isFunction(value) && !(property in element)) element[property] = value.methodize();
    }
  }

  var HTMLOBJECTELEMENT_PROTOTYPE_BUGGY = checkDeficiency('object');

  if (Prototype.BrowserFeatures.SpecificElementExtensions) {
    if (HTMLOBJECTELEMENT_PROTOTYPE_BUGGY) {
      return function(element) {
        if (element && typeof element._extendedByPrototype == 'undefined') {
          var t = element.tagName;
          if (t && (/^(?:object|applet|embed)$/i.test(t))) {
            extendElementWith(element, Element.Methods);
            extendElementWith(element, Element.Methods.Simulated);
            extendElementWith(element, Element.Methods.ByTag[t.toUpperCase()]);
          }
        }
        return element;
      }
    }
    return Prototype.K;
  }

  var Methods = {},
      ByTag = Element.Methods.ByTag;

  var extend = Object.extend(function(element) {
    if (!element || typeof element._extendedByPrototype != 'undefined' || element.nodeType != 1 || element == window) return element;

    var methods = Object.clone(Methods),
        tagName = element.tagName.toUpperCase();

    if (ByTag[tagName]) Object.extend(methods, ByTag[tagName]);

    extendElementWith(element, methods);

    element._extendedByPrototype = Prototype.emptyFunction;
    return element;

  }, {
    refresh: function() {
      if (!Prototype.BrowserFeatures.ElementExtensions) {
        Object.extend(Methods, Element.Methods);
        Object.extend(Methods, Element.Methods.Simulated);
      }
    }
  });

  extend.refresh();
  return extend;
})();

if (document.documentElement.hasAttribute) {
  Element.hasAttribute = function(element, attribute) {
    return element.hasAttribute(attribute);
  };
} else {
  Element.hasAttribute = Element.Methods.Simulated.hasAttribute;
}

Element.addMethods = function(methods) {
  var F = Prototype.BrowserFeatures,
      T = Element.Methods.ByTag;

  if (!methods) {
    Object.extend(Form, Form.Methods);
    Object.extend(Form.Element, Form.Element.Methods);
    Object.extend(Element.Methods.ByTag, {
      "FORM": Object.clone(Form.Methods),
      "INPUT": Object.clone(Form.Element.Methods),
      "SELECT": Object.clone(Form.Element.Methods),
      "TEXTAREA": Object.clone(Form.Element.Methods),
      "BUTTON": Object.clone(Form.Element.Methods)
    });
  }

  if (arguments.length == 2) {
    var tagName = methods;
    methods = arguments[1];
  }

  if (!tagName) Object.extend(Element.Methods, methods || {});
  else {
    if (Object.isArray(tagName)) tagName.each(extend);
    else extend(tagName);
  }

  function extend(tagName) {
    tagName = tagName.toUpperCase();
    if (!Element.Methods.ByTag[tagName]) Element.Methods.ByTag[tagName] = {};
    Object.extend(Element.Methods.ByTag[tagName], methods);
  }

  function copy(methods, destination, onlyIfAbsent) {
    onlyIfAbsent = onlyIfAbsent || false;
    for (var property in methods) {
      var value = methods[property];
      if (!Object.isFunction(value)) continue;
      if (!onlyIfAbsent || !(property in destination)) destination[property] = value.methodize();
    }
  }

  function findDOMClass(tagName) {
    var klass;
    var trans = {
      "OPTGROUP": "OptGroup",
      "TEXTAREA": "TextArea",
      "P": "Paragraph",
      "FIELDSET": "FieldSet",
      "UL": "UList",
      "OL": "OList",
      "DL": "DList",
      "DIR": "Directory",
      "H1": "Heading",
      "H2": "Heading",
      "H3": "Heading",
      "H4": "Heading",
      "H5": "Heading",
      "H6": "Heading",
      "Q": "Quote",
      "INS": "Mod",
      "DEL": "Mod",
      "A": "Anchor",
      "IMG": "Image",
      "CAPTION": "TableCaption",
      "COL": "TableCol",
      "COLGROUP": "TableCol",
      "THEAD": "TableSection",
      "TFOOT": "TableSection",
      "TBODY": "TableSection",
      "TR": "TableRow",
      "TH": "TableCell",
      "TD": "TableCell",
      "FRAMESET": "FrameSet",
      "IFRAME": "IFrame"
    };
    if (trans[tagName]) klass = 'HTML' + trans[tagName] + 'Element';
    if (window[klass]) return window[klass];
    klass = 'HTML' + tagName + 'Element';
    if (window[klass]) return window[klass];
    klass = 'HTML' + tagName.capitalize() + 'Element';
    if (window[klass]) return window[klass];

    var element = document.createElement(tagName),
        proto = element['__proto__'] || element.constructor.prototype;

    element = null;
    return proto;
  }

  var elementPrototype = window.HTMLElement ? HTMLElement.prototype : Element.prototype;

  if (F.ElementExtensions) {
    copy(Element.Methods, elementPrototype);
    copy(Element.Methods.Simulated, elementPrototype, true);
  }

  if (F.SpecificElementExtensions) {
    for (var tag in Element.Methods.ByTag) {
      var klass = findDOMClass(tag);
      if (Object.isUndefined(klass)) continue;
      copy(T[tag], klass.prototype);
    }
  }

  Object.extend(Element, Element.Methods);
  delete Element.ByTag;

  if (Element.extend.refresh) Element.extend.refresh();
  Element.cache = {};
};


document.viewport = {

  getDimensions: function() {
    return {
      width: this.getWidth(),
      height: this.getHeight()
    };
  },

  getScrollOffsets: function() {
    return Element._returnOffset(
    window.pageXOffset || document.documentElement.scrollLeft || document.body.scrollLeft, window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop);
  }
};

(function(viewport) {
  var B = Prototype.Browser,
      doc = document,
      element, property = {};

  function getRootElement() {
    if (B.WebKit && !doc.evaluate) return document;

    if (B.Opera && window.parseFloat(window.opera.version()) < 9.5) return document.body;

    return document.documentElement;
  }

  function define(D) {
    if (!element) element = getRootElement();

    property[D] = 'client' + D;

    viewport['get' + D] = function() {
      return element[property[D]]
    };
    return viewport['get' + D]();
  }

  viewport.getWidth = define.curry('Width');

  viewport.getHeight = define.curry('Height');
})(document.viewport);


Element.Storage = {
  UID: 1
};

Element.addMethods({
  getStorage: function(element) {
    if (!(element = $(element))) return;

    var uid;
    if (element === window) {
      uid = 0;
    } else {
      if (typeof element._prototypeUID === "undefined") element._prototypeUID = Element.Storage.UID++;
      uid = element._prototypeUID;
    }

    if (!Element.Storage[uid]) Element.Storage[uid] = $H();

    return Element.Storage[uid];
  },

  store: function(element, key, value) {
    if (!(element = $(element))) return;

    if (arguments.length === 2) {
      Element.getStorage(element).update(key);
    } else {
      Element.getStorage(element).set(key, value);
    }

    return element;
  },

  retrieve: function(element, key, defaultValue) {
    if (!(element = $(element))) return;
    var hash = Element.getStorage(element),
        value = hash.get(key);

    if (Object.isUndefined(value)) {
      hash.set(key, defaultValue);
      value = defaultValue;
    }

    return value;
  },

  clone: function(element, deep) {
    if (!(element = $(element))) return;
    var clone = element.cloneNode(deep);
    clone._prototypeUID = void 0;
    if (deep) {
      var descendants = Element.select(clone, '*'),
          i = descendants.length;
      while (i--) {
        descendants[i]._prototypeUID = void 0;
      }
    }
    return Element.extend(clone);
  },

  purge: function(element) {
    if (!(element = $(element))) return;
    var purgeElement = Element._purgeElement;

    purgeElement(element);

    var descendants = element.getElementsByTagName('*'),
        i = descendants.length;

    while (i--) purgeElement(descendants[i]);

    return null;
  }
});

(function() {

  function toDecimal(pctString) {
    var match = pctString.match(/^(\d+)%?$/i);
    if (!match) return null;
    return (Number(match[1]) / 100);
  }

  function getPixelValue(value, property, context) {
    var element = null;
    if (Object.isElement(value)) {
      element = value;
      value = element.getStyle(property);
    }

    if (value === null) {
      return null;
    }

    if ((/^(?:-)?\d+(\.\d+)?(px)?$/i).test(value)) {
      return window.parseFloat(value);
    }

    var isPercentage = value.include('%'),
        isViewport = (context === document.viewport);

    if (/\d/.test(value) && element && element.runtimeStyle && !(isPercentage && isViewport)) {
      var style = element.style.left,
          rStyle = element.runtimeStyle.left;
      element.runtimeStyle.left = element.currentStyle.left;
      element.style.left = value || 0;
      value = element.style.pixelLeft;
      element.style.left = style;
      element.runtimeStyle.left = rStyle;

      return value;
    }

    if (element && isPercentage) {
      context = context || element.parentNode;
      var decimal = toDecimal(value);
      var whole = null;
      var position = element.getStyle('position');

      var isHorizontal = property.include('left') || property.include('right') || property.include('width');

      var isVertical = property.include('top') || property.include('bottom') || property.include('height');

      if (context === document.viewport) {
        if (isHorizontal) {
          whole = document.viewport.getWidth();
        } else if (isVertical) {
          whole = document.viewport.getHeight();
        }
      } else {
        if (isHorizontal) {
          whole = $(context).measure('width');
        } else if (isVertical) {
          whole = $(context).measure('height');
        }
      }

      return (whole === null) ? 0 : whole * decimal;
    }

    return 0;
  }

  function toCSSPixels(number) {
    if (Object.isString(number) && number.endsWith('px')) {
      return number;
    }
    return number + 'px';
  }

  function isDisplayed(element) {
    var originalElement = element;
    while (element && element.parentNode) {
      var display = element.getStyle('display');
      if (display === 'none') {
        return false;
      }
      element = $(element.parentNode);
    }
    return true;
  }

  var hasLayout = Prototype.K;
  if ('currentStyle' in document.documentElement) {
    hasLayout = function(element) {
      if (!element.currentStyle.hasLayout) {
        element.style.zoom = 1;
      }
      return element;
    };
  }

  function cssNameFor(key) {
    if (key.include('border')) key = key + '-width';
    return key.camelize();
  }

  Element.Layout = Class.create(Hash, {
    initialize: function($super, element, preCompute) {
      $super();
      this.element = $(element);

      Element.Layout.PROPERTIES.each(function(property) {
        this._set(property, null);
      }, this);

      if (preCompute) {
        this._preComputing = true;
        this._begin();
        Element.Layout.PROPERTIES.each(this._compute, this);
        this._end();
        this._preComputing = false;
      }
    },

    _set: function(property, value) {
      return Hash.prototype.set.call(this, property, value);
    },

    set: function(property, value) {
      throw "Properties of Element.Layout are read-only.";
    },

    get: function($super, property) {
      var value = $super(property);
      return value === null ? this._compute(property) : value;
    },

    _begin: function() {
      if (this._prepared) return;

      var element = this.element;
      if (isDisplayed(element)) {
        this._prepared = true;
        return;
      }

      var originalStyles = {
        position: element.style.position || '',
        width: element.style.width || '',
        visibility: element.style.visibility || '',
        display: element.style.display || ''
      };

      element.store('prototype_original_styles', originalStyles);

      var position = element.getStyle('position'),
          width = element.getStyle('width');

      if (width === "0px" || width === null) {
        element.style.display = 'block';
        width = element.getStyle('width');
      }

      var context = (position === 'fixed') ? document.viewport : element.parentNode;

      element.setStyle({
        position: 'absolute',
        visibility: 'hidden',
        display: 'block'
      });

      var positionedWidth = element.getStyle('width');

      var newWidth;
      if (width && (positionedWidth === width)) {
        newWidth = getPixelValue(element, 'width', context);
      } else if (position === 'absolute' || position === 'fixed') {
        newWidth = getPixelValue(element, 'width', context);
      } else {
        var parent = element.parentNode,
            pLayout = $(parent).getLayout();

        newWidth = pLayout.get('width') - this.get('margin-left') - this.get('border-left') - this.get('padding-left') - this.get('padding-right') - this.get('border-right') - this.get('margin-right');
      }

      element.setStyle({
        width: newWidth + 'px'
      });

      this._prepared = true;
    },

    _end: function() {
      var element = this.element;
      var originalStyles = element.retrieve('prototype_original_styles');
      element.store('prototype_original_styles', null);
      element.setStyle(originalStyles);
      this._prepared = false;
    },

    _compute: function(property) {
      var COMPUTATIONS = Element.Layout.COMPUTATIONS;
      if (!(property in COMPUTATIONS)) {
        throw "Property not found.";
      }

      return this._set(property, COMPUTATIONS[property].call(this, this.element));
    },

    toObject: function() {
      var args = $A(arguments);
      var keys = (args.length === 0) ? Element.Layout.PROPERTIES : args.join(' ').split(' ');
      var obj = {};
      keys.each(function(key) {
        if (!Element.Layout.PROPERTIES.include(key)) return;
        var value = this.get(key);
        if (value != null) obj[key] = value;
      }, this);
      return obj;
    },

    toHash: function() {
      var obj = this.toObject.apply(this, arguments);
      return new Hash(obj);
    },

    toCSS: function() {
      var args = $A(arguments);
      var keys = (args.length === 0) ? Element.Layout.PROPERTIES : args.join(' ').split(' ');
      var css = {};

      keys.each(function(key) {
        if (!Element.Layout.PROPERTIES.include(key)) return;
        if (Element.Layout.COMPOSITE_PROPERTIES.include(key)) return;

        var value = this.get(key);
        if (value != null) css[cssNameFor(key)] = value + 'px';
      }, this);
      return css;
    },

    inspect: function() {
      return "#<Element.Layout>";
    }
  });

  Object.extend(Element.Layout, {
    PROPERTIES: $w('height width top left right bottom border-left border-right border-top border-bottom padding-left padding-right padding-top padding-bottom margin-top margin-bottom margin-left margin-right padding-box-width padding-box-height border-box-width border-box-height margin-box-width margin-box-height'),

    COMPOSITE_PROPERTIES: $w('padding-box-width padding-box-height margin-box-width margin-box-height border-box-width border-box-height'),

    COMPUTATIONS: {
      'height': function(element) {
        if (!this._preComputing) this._begin();

        var bHeight = this.get('border-box-height');
        if (bHeight <= 0) {
          if (!this._preComputing) this._end();
          return 0;
        }

        var bTop = this.get('border-top'),
            bBottom = this.get('border-bottom');

        var pTop = this.get('padding-top'),
            pBottom = this.get('padding-bottom');

        if (!this._preComputing) this._end();

        return bHeight - bTop - bBottom - pTop - pBottom;
      },

      'width': function(element) {
        if (!this._preComputing) this._begin();

        var bWidth = this.get('border-box-width');
        if (bWidth <= 0) {
          if (!this._preComputing) this._end();
          return 0;
        }

        var bLeft = this.get('border-left'),
            bRight = this.get('border-right');

        var pLeft = this.get('padding-left'),
            pRight = this.get('padding-right');

        if (!this._preComputing) this._end();

        return bWidth - bLeft - bRight - pLeft - pRight;
      },

      'padding-box-height': function(element) {
        var height = this.get('height'),
            pTop = this.get('padding-top'),
            pBottom = this.get('padding-bottom');

        return height + pTop + pBottom;
      },

      'padding-box-width': function(element) {
        var width = this.get('width'),
            pLeft = this.get('padding-left'),
            pRight = this.get('padding-right');

        return width + pLeft + pRight;
      },

      'border-box-height': function(element) {
        if (!this._preComputing) this._begin();
        var height = element.offsetHeight;
        if (!this._preComputing) this._end();
        return height;
      },

      'border-box-width': function(element) {
        if (!this._preComputing) this._begin();
        var width = element.offsetWidth;
        if (!this._preComputing) this._end();
        return width;
      },

      'margin-box-height': function(element) {
        var bHeight = this.get('border-box-height'),
            mTop = this.get('margin-top'),
            mBottom = this.get('margin-bottom');

        if (bHeight <= 0) return 0;

        return bHeight + mTop + mBottom;
      },

      'margin-box-width': function(element) {
        var bWidth = this.get('border-box-width'),
            mLeft = this.get('margin-left'),
            mRight = this.get('margin-right');

        if (bWidth <= 0) return 0;

        return bWidth + mLeft + mRight;
      },

      'top': function(element) {
        var offset = element.positionedOffset();
        return offset.top;
      },

      'bottom': function(element) {
        var offset = element.positionedOffset(),
            parent = element.getOffsetParent(),
            pHeight = parent.measure('height');

        var mHeight = this.get('border-box-height');

        return pHeight - mHeight - offset.top;
      },

      'left': function(element) {
        var offset = element.positionedOffset();
        return offset.left;
      },

      'right': function(element) {
        var offset = element.positionedOffset(),
            parent = element.getOffsetParent(),
            pWidth = parent.measure('width');

        var mWidth = this.get('border-box-width');

        return pWidth - mWidth - offset.left;
      },

      'padding-top': function(element) {
        return getPixelValue(element, 'paddingTop');
      },

      'padding-bottom': function(element) {
        return getPixelValue(element, 'paddingBottom');
      },

      'padding-left': function(element) {
        return getPixelValue(element, 'paddingLeft');
      },

      'padding-right': function(element) {
        return getPixelValue(element, 'paddingRight');
      },

      'border-top': function(element) {
        return getPixelValue(element, 'borderTopWidth');
      },

      'border-bottom': function(element) {
        return getPixelValue(element, 'borderBottomWidth');
      },

      'border-left': function(element) {
        return getPixelValue(element, 'borderLeftWidth');
      },

      'border-right': function(element) {
        return getPixelValue(element, 'borderRightWidth');
      },

      'margin-top': function(element) {
        return getPixelValue(element, 'marginTop');
      },

      'margin-bottom': function(element) {
        return getPixelValue(element, 'marginBottom');
      },

      'margin-left': function(element) {
        return getPixelValue(element, 'marginLeft');
      },

      'margin-right': function(element) {
        return getPixelValue(element, 'marginRight');
      }
    }
  });

  if ('getBoundingClientRect' in document.documentElement) {
    Object.extend(Element.Layout.COMPUTATIONS, {
      'right': function(element) {
        var parent = hasLayout(element.getOffsetParent());
        var rect = element.getBoundingClientRect(),
            pRect = parent.getBoundingClientRect();

        return (pRect.right - rect.right).round();
      },

      'bottom': function(element) {
        var parent = hasLayout(element.getOffsetParent());
        var rect = element.getBoundingClientRect(),
            pRect = parent.getBoundingClientRect();

        return (pRect.bottom - rect.bottom).round();
      }
    });
  }

  Element.Offset = Class.create({
    initialize: function(left, top) {
      this.left = left.round();
      this.top = top.round();

      this[0] = this.left;
      this[1] = this.top;
    },

    relativeTo: function(offset) {
      return new Element.Offset(
      this.left - offset.left, this.top - offset.top);
    },

    inspect: function() {
      return "#<Element.Offset left: #{left} top: #{top}>".interpolate(this);
    },

    toString: function() {
      return "[#{left}, #{top}]".interpolate(this);
    },

    toArray: function() {
      return [this.left, this.top];
    }
  });

  function getLayout(element, preCompute) {
    return new Element.Layout(element, preCompute);
  }

  function measure(element, property) {
    return $(element).getLayout().get(property);
  }

  function getDimensions(element) {
    element = $(element);
    var display = Element.getStyle(element, 'display');

    if (display && display !== 'none') {
      return {
        width: element.offsetWidth,
        height: element.offsetHeight
      };
    }

    var style = element.style;
    var originalStyles = {
      visibility: style.visibility,
      position: style.position,
      display: style.display
    };

    var newStyles = {
      visibility: 'hidden',
      display: 'block'
    };

    if (originalStyles.position !== 'fixed') newStyles.position = 'absolute';

    Element.setStyle(element, newStyles);

    var dimensions = {
      width: element.offsetWidth,
      height: element.offsetHeight
    };

    Element.setStyle(element, originalStyles);

    return dimensions;
  }

  function getOffsetParent(element) {
    element = $(element);

    if (isDocument(element) || isDetached(element) || isBody(element) || isHtml(element)) return $(document.body);

    var isInline = (Element.getStyle(element, 'display') === 'inline');
    if (!isInline && element.offsetParent) return $(element.offsetParent);

    while ((element = element.parentNode) && element !== document.body) {
      if (Element.getStyle(element, 'position') !== 'static') {
        return isHtml(element) ? $(document.body) : $(element);
      }
    }

    return $(document.body);
  }


  function cumulativeOffset(element) {
    element = $(element);
    var valueT = 0,
        valueL = 0;
    if (element.parentNode) {
      do {
        valueT += element.offsetTop || 0;
        valueL += element.offsetLeft || 0;
        element = element.offsetParent;
      } while (element);
    }
    return new Element.Offset(valueL, valueT);
  }

  function positionedOffset(element) {
    element = $(element);

    var layout = element.getLayout();

    var valueT = 0,
        valueL = 0;
    do {
      valueT += element.offsetTop || 0;
      valueL += element.offsetLeft || 0;
      element = element.offsetParent;
      if (element) {
        if (isBody(element)) break;
        var p = Element.getStyle(element, 'position');
        if (p !== 'static') break;
      }
    } while (element);

    valueL -= layout.get('margin-top');
    valueT -= layout.get('margin-left');

    return new Element.Offset(valueL, valueT);
  }

  function cumulativeScrollOffset(element) {
    var valueT = 0,
        valueL = 0;
    do {
      valueT += element.scrollTop || 0;
      valueL += element.scrollLeft || 0;
      element = element.parentNode;
    } while (element);
    return new Element.Offset(valueL, valueT);
  }

  function viewportOffset(forElement) {
    element = $(element);
    var valueT = 0,
        valueL = 0,
        docBody = document.body;

    var element = forElement;
    do {
      valueT += element.offsetTop || 0;
      valueL += element.offsetLeft || 0;
      if (element.offsetParent == docBody && Element.getStyle(element, 'position') == 'absolute') break;
    } while (element = element.offsetParent);

    element = forElement;
    do {
      if (element != docBody) {
        valueT -= element.scrollTop || 0;
        valueL -= element.scrollLeft || 0;
      }
    } while (element = element.parentNode);
    return new Element.Offset(valueL, valueT);
  }

  function absolutize(element) {
    element = $(element);

    if (Element.getStyle(element, 'position') === 'absolute') {
      return element;
    }

    var offsetParent = getOffsetParent(element);
    var eOffset = element.viewportOffset(),
        pOffset = offsetParent.viewportOffset();

    var offset = eOffset.relativeTo(pOffset);
    var layout = element.getLayout();

    element.store('prototype_absolutize_original_styles', {
      left: element.getStyle('left'),
      top: element.getStyle('top'),
      width: element.getStyle('width'),
      height: element.getStyle('height')
    });

    element.setStyle({
      position: 'absolute',
      top: offset.top + 'px',
      left: offset.left + 'px',
      width: layout.get('width') + 'px',
      height: layout.get('height') + 'px'
    });

    return element;
  }

  function relativize(element) {
    element = $(element);
    if (Element.getStyle(element, 'position') === 'relative') {
      return element;
    }

    var originalStyles =
    element.retrieve('prototype_absolutize_original_styles');

    if (originalStyles) element.setStyle(originalStyles);
    return element;
  }

  if (Prototype.Browser.IE) {
    getOffsetParent = getOffsetParent.wrap(

    function(proceed, element) {
      element = $(element);

      if (isDocument(element) || isDetached(element) || isBody(element) || isHtml(element)) return $(document.body);

      var position = element.getStyle('position');
      if (position !== 'static') return proceed(element);

      element.setStyle({
        position: 'relative'
      });
      var value = proceed(element);
      element.setStyle({
        position: position
      });
      return value;
    });

    positionedOffset = positionedOffset.wrap(function(proceed, element) {
      element = $(element);
      if (!element.parentNode) return new Element.Offset(0, 0);
      var position = element.getStyle('position');
      if (position !== 'static') return proceed(element);

      var offsetParent = element.getOffsetParent();
      if (offsetParent && offsetParent.getStyle('position') === 'fixed') hasLayout(offsetParent);

      element.setStyle({
        position: 'relative'
      });
      var value = proceed(element);
      element.setStyle({
        position: position
      });
      return value;
    });
  } else if (Prototype.Browser.Webkit) {
    cumulativeOffset = function(element) {
      element = $(element);
      var valueT = 0,
          valueL = 0;
      do {
        valueT += element.offsetTop || 0;
        valueL += element.offsetLeft || 0;
        if (element.offsetParent == document.body) if (Element.getStyle(element, 'position') == 'absolute') break;

        element = element.offsetParent;
      } while (element);

      return new Element.Offset(valueL, valueT);
    };
  }


  Element.addMethods({
    getLayout: getLayout,
    measure: measure,
    getDimensions: getDimensions,
    getOffsetParent: getOffsetParent,
    cumulativeOffset: cumulativeOffset,
    positionedOffset: positionedOffset,
    cumulativeScrollOffset: cumulativeScrollOffset,
    viewportOffset: viewportOffset,
    absolutize: absolutize,
    relativize: relativize
  });

  function isBody(element) {
    return element.nodeName.toUpperCase() === 'BODY';
  }

  function isHtml(element) {
    return element.nodeName.toUpperCase() === 'HTML';
  }

  function isDocument(element) {
    return element.nodeType === Node.DOCUMENT_NODE;
  }

  function isDetached(element) {
    return element !== document.body && !Element.descendantOf(element, document.body);
  }

  if ('getBoundingClientRect' in document.documentElement) {
    Element.addMethods({
      viewportOffset: function(element) {
        element = $(element);
        if (isDetached(element)) return new Element.Offset(0, 0);

        var rect = element.getBoundingClientRect(),
            docEl = document.documentElement;
        return new Element.Offset(rect.left - docEl.clientLeft, rect.top - docEl.clientTop);
      }
    });
  }
})();
window.$$ = function() {
  var expression = $A(arguments).join(', ');
  return Prototype.Selector.select(expression, document);
};

Prototype.Selector = (function() {

  function select() {
    throw new Error('Method "Prototype.Selector.select" must be defined.');
  }

  function match() {
    throw new Error('Method "Prototype.Selector.match" must be defined.');
  }

  function find(elements, expression, index) {
    index = index || 0;
    var match = Prototype.Selector.match,
        length = elements.length,
        matchIndex = 0,
        i;

    for (i = 0; i < length; i++) {
      if (match(elements[i], expression) && index == matchIndex++) {
        return Element.extend(elements[i]);
      }
    }
  }

  function extendElements(elements) {
    for (var i = 0, length = elements.length; i < length; i++) {
      Element.extend(elements[i]);
    }
    return elements;
  }


  var K = Prototype.K;

  return {
    select: select,
    match: match,
    find: find,
    extendElements: (Element.extend === K) ? K : extendElements,
    extendElement: Element.extend
  };
})();
Prototype._original_property = window.Sizzle;
/*!
 * Sizzle CSS Selector Engine - v1.0
 *  Copyright 2009, The Dojo Foundation
 *  Released under the MIT, BSD, and GPL Licenses.
 *  More information: http://sizzlejs.com/
 */
(function() {

  var chunker = /((?:\((?:\([^()]+\)|[^()]+)+\)|\[(?:\[[^[\]]*\]|['"][^'"]*['"]|[^[\]'"]+)+\]|\\.|[^ >+~,(\[\\]+)+|[>+~])(\s*,\s*)?((?:.|\r|\n)*)/g,
      done = 0,
      toString = Object.prototype.toString,
      hasDuplicate = false,
      baseHasDuplicate = true;

  [0, 0].sort(function() {
    baseHasDuplicate = false;
    return 0;
  });

  var Sizzle = function(selector, context, results, seed) {
    results = results || [];
    var origContext = context = context || document;

    if (context.nodeType !== 1 && context.nodeType !== 9) {
      return [];
    }

    if (!selector || typeof selector !== "string") {
      return results;
    }

    var parts = [],
        m, set, checkSet, check, mode, extra, prune = true,
        contextXML = isXML(context),
        soFar = selector;

    while ((chunker.exec(""), m = chunker.exec(soFar)) !== null) {
      soFar = m[3];

      parts.push(m[1]);

      if (m[2]) {
        extra = m[3];
        break;
      }
    }

    if (parts.length > 1 && origPOS.exec(selector)) {
      if (parts.length === 2 && Expr.relative[parts[0]]) {
        set = posProcess(parts[0] + parts[1], context);
      } else {
        set = Expr.relative[parts[0]] ? [context] : Sizzle(parts.shift(), context);

        while (parts.length) {
          selector = parts.shift();

          if (Expr.relative[selector]) selector += parts.shift();

          set = posProcess(selector, set);
        }
      }
    } else {
      if (!seed && parts.length > 1 && context.nodeType === 9 && !contextXML && Expr.match.ID.test(parts[0]) && !Expr.match.ID.test(parts[parts.length - 1])) {
        var ret = Sizzle.find(parts.shift(), context, contextXML);
        context = ret.expr ? Sizzle.filter(ret.expr, ret.set)[0] : ret.set[0];
      }

      if (context) {
        var ret = seed ? {
          expr: parts.pop(),
          set: makeArray(seed)
        } : Sizzle.find(parts.pop(), parts.length === 1 && (parts[0] === "~" || parts[0] === "+") && context.parentNode ? context.parentNode : context, contextXML);
        set = ret.expr ? Sizzle.filter(ret.expr, ret.set) : ret.set;

        if (parts.length > 0) {
          checkSet = makeArray(set);
        } else {
          prune = false;
        }

        while (parts.length) {
          var cur = parts.pop(),
              pop = cur;

          if (!Expr.relative[cur]) {
            cur = "";
          } else {
            pop = parts.pop();
          }

          if (pop == null) {
            pop = context;
          }

          Expr.relative[cur](checkSet, pop, contextXML);
        }
      } else {
        checkSet = parts = [];
      }
    }

    if (!checkSet) {
      checkSet = set;
    }

    if (!checkSet) {
      throw "Syntax error, unrecognized expression: " + (cur || selector);
    }

    if (toString.call(checkSet) === "[object Array]") {
      if (!prune) {
        results.push.apply(results, checkSet);
      } else if (context && context.nodeType === 1) {
        for (var i = 0; checkSet[i] != null; i++) {
          if (checkSet[i] && (checkSet[i] === true || checkSet[i].nodeType === 1 && contains(context, checkSet[i]))) {
            results.push(set[i]);
          }
        }
      } else {
        for (var i = 0; checkSet[i] != null; i++) {
          if (checkSet[i] && checkSet[i].nodeType === 1) {
            results.push(set[i]);
          }
        }
      }
    } else {
      makeArray(checkSet, results);
    }

    if (extra) {
      Sizzle(extra, origContext, results, seed);
      Sizzle.uniqueSort(results);
    }

    return results;
  };

  Sizzle.uniqueSort = function(results) {
    if (sortOrder) {
      hasDuplicate = baseHasDuplicate;
      results.sort(sortOrder);

      if (hasDuplicate) {
        for (var i = 1; i < results.length; i++) {
          if (results[i] === results[i - 1]) {
            results.splice(i--, 1);
          }
        }
      }
    }

    return results;
  };

  Sizzle.matches = function(expr, set) {
    return Sizzle(expr, null, null, set);
  };

  Sizzle.find = function(expr, context, isXML) {
    var set, match;

    if (!expr) {
      return [];
    }

    for (var i = 0, l = Expr.order.length; i < l; i++) {
      var type = Expr.order[i],
          match;

      if ((match = Expr.leftMatch[type].exec(expr))) {
        var left = match[1];
        match.splice(1, 1);

        if (left.substr(left.length - 1) !== "\\") {
          match[1] = (match[1] || "").replace(/\\/g, "");
          set = Expr.find[type](match, context, isXML);
          if (set != null) {
            expr = expr.replace(Expr.match[type], "");
            break;
          }
        }
      }
    }

    if (!set) {
      set = context.getElementsByTagName("*");
    }

    return {
      set: set,
      expr: expr
    };
  };

  Sizzle.filter = function(expr, set, inplace, not) {
    var old = expr,
        result = [],
        curLoop = set,
        match, anyFound, isXMLFilter = set && set[0] && isXML(set[0]);

    while (expr && set.length) {
      for (var type in Expr.filter) {
        if ((match = Expr.match[type].exec(expr)) != null) {
          var filter = Expr.filter[type],
              found, item;
          anyFound = false;

          if (curLoop == result) {
            result = [];
          }

          if (Expr.preFilter[type]) {
            match = Expr.preFilter[type](match, curLoop, inplace, result, not, isXMLFilter);

            if (!match) {
              anyFound = found = true;
            } else if (match === true) {
              continue;
            }
          }

          if (match) {
            for (var i = 0;
            (item = curLoop[i]) != null; i++) {
              if (item) {
                found = filter(item, match, i, curLoop);
                var pass = not ^ !! found;

                if (inplace && found != null) {
                  if (pass) {
                    anyFound = true;
                  } else {
                    curLoop[i] = false;
                  }
                } else if (pass) {
                  result.push(item);
                  anyFound = true;
                }
              }
            }
          }

          if (found !== undefined) {
            if (!inplace) {
              curLoop = result;
            }

            expr = expr.replace(Expr.match[type], "");

            if (!anyFound) {
              return [];
            }

            break;
          }
        }
      }

      if (expr == old) {
        if (anyFound == null) {
          throw "Syntax error, unrecognized expression: " + expr;
        } else {
          break;
        }
      }

      old = expr;
    }

    return curLoop;
  };

  var Expr = Sizzle.selectors = {
    order: ["ID", "NAME", "TAG"],
    match: {
      ID: /#((?:[\w\u00c0-\uFFFF-]|\\.)+)/,
      CLASS: /\.((?:[\w\u00c0-\uFFFF-]|\\.)+)/,
      NAME: /\[name=['"]*((?:[\w\u00c0-\uFFFF-]|\\.)+)['"]*\]/,
      ATTR: /\[\s*((?:[\w\u00c0-\uFFFF-]|\\.)+)\s*(?:(\S?=)\s*(['"]*)(.*?)\3|)\s*\]/,
      TAG: /^((?:[\w\u00c0-\uFFFF\*-]|\\.)+)/,
      CHILD: /:(only|nth|last|first)-child(?:\((even|odd|[\dn+-]*)\))?/,
      POS: /:(nth|eq|gt|lt|first|last|even|odd)(?:\((\d*)\))?(?=[^-]|$)/,
      PSEUDO: /:((?:[\w\u00c0-\uFFFF-]|\\.)+)(?:\((['"]*)((?:\([^\)]+\)|[^\2\(\)]*)+)\2\))?/
    },
    leftMatch: {},
    attrMap: {
      "class": "className",
      "for": "htmlFor"
    },
    attrHandle: {
      href: function(elem) {
        return elem.getAttribute("href");
      }
    },
    relative: {
      "+": function(checkSet, part, isXML) {
        var isPartStr = typeof part === "string",
            isTag = isPartStr && !/\W/.test(part),
            isPartStrNotTag = isPartStr && !isTag;

        if (isTag && !isXML) {
          part = part.toUpperCase();
        }

        for (var i = 0, l = checkSet.length, elem; i < l; i++) {
          if ((elem = checkSet[i])) {
            while ((elem = elem.previousSibling) && elem.nodeType !== 1) {}

            checkSet[i] = isPartStrNotTag || elem && elem.nodeName === part ? elem || false : elem === part;
          }
        }

        if (isPartStrNotTag) {
          Sizzle.filter(part, checkSet, true);
        }
      },
      ">": function(checkSet, part, isXML) {
        var isPartStr = typeof part === "string";

        if (isPartStr && !/\W/.test(part)) {
          part = isXML ? part : part.toUpperCase();

          for (var i = 0, l = checkSet.length; i < l; i++) {
            var elem = checkSet[i];
            if (elem) {
              var parent = elem.parentNode;
              checkSet[i] = parent.nodeName === part ? parent : false;
            }
          }
        } else {
          for (var i = 0, l = checkSet.length; i < l; i++) {
            var elem = checkSet[i];
            if (elem) {
              checkSet[i] = isPartStr ? elem.parentNode : elem.parentNode === part;
            }
          }

          if (isPartStr) {
            Sizzle.filter(part, checkSet, true);
          }
        }
      },
      "": function(checkSet, part, isXML) {
        var doneName = done++,
            checkFn = dirCheck;

        if (!/\W/.test(part)) {
          var nodeCheck = part = isXML ? part : part.toUpperCase();
          checkFn = dirNodeCheck;
        }

        checkFn("parentNode", part, doneName, checkSet, nodeCheck, isXML);
      },
      "~": function(checkSet, part, isXML) {
        var doneName = done++,
            checkFn = dirCheck;

        if (typeof part === "string" && !/\W/.test(part)) {
          var nodeCheck = part = isXML ? part : part.toUpperCase();
          checkFn = dirNodeCheck;
        }

        checkFn("previousSibling", part, doneName, checkSet, nodeCheck, isXML);
      }
    },
    find: {
      ID: function(match, context, isXML) {
        if (typeof context.getElementById !== "undefined" && !isXML) {
          var m = context.getElementById(match[1]);
          return m ? [m] : [];
        }
      },
      NAME: function(match, context, isXML) {
        if (typeof context.getElementsByName !== "undefined") {
          var ret = [],
              results = context.getElementsByName(match[1]);

          for (var i = 0, l = results.length; i < l; i++) {
            if (results[i].getAttribute("name") === match[1]) {
              ret.push(results[i]);
            }
          }

          return ret.length === 0 ? null : ret;
        }
      },
      TAG: function(match, context) {
        return context.getElementsByTagName(match[1]);
      }
    },
    preFilter: {
      CLASS: function(match, curLoop, inplace, result, not, isXML) {
        match = " " + match[1].replace(/\\/g, "") + " ";

        if (isXML) {
          return match;
        }

        for (var i = 0, elem;
        (elem = curLoop[i]) != null; i++) {
          if (elem) {
            if (not ^ (elem.className && (" " + elem.className + " ").indexOf(match) >= 0)) {
              if (!inplace) result.push(elem);
            } else if (inplace) {
              curLoop[i] = false;
            }
          }
        }

        return false;
      },
      ID: function(match) {
        return match[1].replace(/\\/g, "");
      },
      TAG: function(match, curLoop) {
        for (var i = 0; curLoop[i] === false; i++) {}
        return curLoop[i] && isXML(curLoop[i]) ? match[1] : match[1].toUpperCase();
      },
      CHILD: function(match) {
        if (match[1] == "nth") {
          var test = /(-?)(\d*)n((?:\+|-)?\d*)/.exec(
          match[2] == "even" && "2n" || match[2] == "odd" && "2n+1" || !/\D/.test(match[2]) && "0n+" + match[2] || match[2]);

          match[2] = (test[1] + (test[2] || 1)) - 0;
          match[3] = test[3] - 0;
        }

        match[0] = done++;

        return match;
      },
      ATTR: function(match, curLoop, inplace, result, not, isXML) {
        var name = match[1].replace(/\\/g, "");

        if (!isXML && Expr.attrMap[name]) {
          match[1] = Expr.attrMap[name];
        }

        if (match[2] === "~=") {
          match[4] = " " + match[4] + " ";
        }

        return match;
      },
      PSEUDO: function(match, curLoop, inplace, result, not) {
        if (match[1] === "not") {
          if ((chunker.exec(match[3]) || "").length > 1 || /^\w/.test(match[3])) {
            match[3] = Sizzle(match[3], null, null, curLoop);
          } else {
            var ret = Sizzle.filter(match[3], curLoop, inplace, true ^ not);
            if (!inplace) {
              result.push.apply(result, ret);
            }
            return false;
          }
        } else if (Expr.match.POS.test(match[0]) || Expr.match.CHILD.test(match[0])) {
          return true;
        }

        return match;
      },
      POS: function(match) {
        match.unshift(true);
        return match;
      }
    },
    filters: {
      enabled: function(elem) {
        return elem.disabled === false && elem.type !== "hidden";
      },
      disabled: function(elem) {
        return elem.disabled === true;
      },
      checked: function(elem) {
        return elem.checked === true;
      },
      selected: function(elem) {
        elem.parentNode.selectedIndex;
        return elem.selected === true;
      },
      parent: function(elem) {
        return !!elem.firstChild;
      },
      empty: function(elem) {
        return !elem.firstChild;
      },
      has: function(elem, i, match) {
        return !!Sizzle(match[3], elem).length;
      },
      header: function(elem) {
        return /h\d/i.test(elem.nodeName);
      },
      text: function(elem) {
        return "text" === elem.type;
      },
      radio: function(elem) {
        return "radio" === elem.type;
      },
      checkbox: function(elem) {
        return "checkbox" === elem.type;
      },
      file: function(elem) {
        return "file" === elem.type;
      },
      password: function(elem) {
        return "password" === elem.type;
      },
      submit: function(elem) {
        return "submit" === elem.type;
      },
      image: function(elem) {
        return "image" === elem.type;
      },
      reset: function(elem) {
        return "reset" === elem.type;
      },
      button: function(elem) {
        return "button" === elem.type || elem.nodeName.toUpperCase() === "BUTTON";
      },
      input: function(elem) {
        return /input|select|textarea|button/i.test(elem.nodeName);
      }
    },
    setFilters: {
      first: function(elem, i) {
        return i === 0;
      },
      last: function(elem, i, match, array) {
        return i === array.length - 1;
      },
      even: function(elem, i) {
        return i % 2 === 0;
      },
      odd: function(elem, i) {
        return i % 2 === 1;
      },
      lt: function(elem, i, match) {
        return i < match[3] - 0;
      },
      gt: function(elem, i, match) {
        return i > match[3] - 0;
      },
      nth: function(elem, i, match) {
        return match[3] - 0 == i;
      },
      eq: function(elem, i, match) {
        return match[3] - 0 == i;
      }
    },
    filter: {
      PSEUDO: function(elem, match, i, array) {
        var name = match[1],
            filter = Expr.filters[name];

        if (filter) {
          return filter(elem, i, match, array);
        } else if (name === "contains") {
          return (elem.textContent || elem.innerText || "").indexOf(match[3]) >= 0;
        } else if (name === "not") {
          var not = match[3];

          for (var i = 0, l = not.length; i < l; i++) {
            if (not[i] === elem) {
              return false;
            }
          }

          return true;
        }
      },
      CHILD: function(elem, match) {
        var type = match[1],
            node = elem;
        switch (type) {
        case 'only':
        case 'first':
          while ((node = node.previousSibling)) {
            if (node.nodeType === 1) return false;
          }
          if (type == 'first') return true;
          node = elem;
        case 'last':
          while ((node = node.nextSibling)) {
            if (node.nodeType === 1) return false;
          }
          return true;
        case 'nth':
          var first = match[2],
              last = match[3];

          if (first == 1 && last == 0) {
            return true;
          }

          var doneName = match[0],
              parent = elem.parentNode;

          if (parent && (parent.sizcache !== doneName || !elem.nodeIndex)) {
            var count = 0;
            for (node = parent.firstChild; node; node = node.nextSibling) {
              if (node.nodeType === 1) {
                node.nodeIndex = ++count;
              }
            }
            parent.sizcache = doneName;
          }

          var diff = elem.nodeIndex - last;
          if (first == 0) {
            return diff == 0;
          } else {
            return (diff % first == 0 && diff / first >= 0);
          }
        }
      },
      ID: function(elem, match) {
        return elem.nodeType === 1 && elem.getAttribute("id") === match;
      },
      TAG: function(elem, match) {
        return (match === "*" && elem.nodeType === 1) || elem.nodeName === match;
      },
      CLASS: function(elem, match) {
        return (" " + (elem.className || elem.getAttribute("class")) + " ").indexOf(match) > -1;
      },
      ATTR: function(elem, match) {
        var name = match[1],
            result = Expr.attrHandle[name] ? Expr.attrHandle[name](elem) : elem[name] != null ? elem[name] : elem.getAttribute(name),
            value = result + "",
            type = match[2],
            check = match[4];

        return result == null ? type === "!=" : type === "=" ? value === check : type === "*=" ? value.indexOf(check) >= 0 : type === "~=" ? (" " + value + " ").indexOf(check) >= 0 : !check ? value && result !== false : type === "!=" ? value != check : type === "^=" ? value.indexOf(check) === 0 : type === "$=" ? value.substr(value.length - check.length) === check : type === "|=" ? value === check || value.substr(0, check.length + 1) === check + "-" : false;
      },
      POS: function(elem, match, i, array) {
        var name = match[2],
            filter = Expr.setFilters[name];

        if (filter) {
          return filter(elem, i, match, array);
        }
      }
    }
  };

  var origPOS = Expr.match.POS;

  for (var type in Expr.match) {
    Expr.match[type] = new RegExp(Expr.match[type].source + /(?![^\[]*\])(?![^\(]*\))/.source);
    Expr.leftMatch[type] = new RegExp(/(^(?:.|\r|\n)*?)/.source + Expr.match[type].source);
  }

  var makeArray = function(array, results) {
    array = Array.prototype.slice.call(array, 0);

    if (results) {
      results.push.apply(results, array);
      return results;
    }

    return array;
  };

  try {
    Array.prototype.slice.call(document.documentElement.childNodes, 0);

  } catch (e) {
    makeArray = function(array, results) {
      var ret = results || [];

      if (toString.call(array) === "[object Array]") {
        Array.prototype.push.apply(ret, array);
      } else {
        if (typeof array.length === "number") {
          for (var i = 0, l = array.length; i < l; i++) {
            ret.push(array[i]);
          }
        } else {
          for (var i = 0; array[i]; i++) {
            ret.push(array[i]);
          }
        }
      }

      return ret;
    };
  }

  var sortOrder;

  if (document.documentElement.compareDocumentPosition) {
    sortOrder = function(a, b) {
      if (!a.compareDocumentPosition || !b.compareDocumentPosition) {
        if (a == b) {
          hasDuplicate = true;
        }
        return 0;
      }

      var ret = a.compareDocumentPosition(b) & 4 ? -1 : a === b ? 0 : 1;
      if (ret === 0) {
        hasDuplicate = true;
      }
      return ret;
    };
  } else if ("sourceIndex" in document.documentElement) {
    sortOrder = function(a, b) {
      if (!a.sourceIndex || !b.sourceIndex) {
        if (a == b) {
          hasDuplicate = true;
        }
        return 0;
      }

      var ret = a.sourceIndex - b.sourceIndex;
      if (ret === 0) {
        hasDuplicate = true;
      }
      return ret;
    };
  } else if (document.createRange) {
    sortOrder = function(a, b) {
      if (!a.ownerDocument || !b.ownerDocument) {
        if (a == b) {
          hasDuplicate = true;
        }
        return 0;
      }

      var aRange = a.ownerDocument.createRange(),
          bRange = b.ownerDocument.createRange();
      aRange.setStart(a, 0);
      aRange.setEnd(a, 0);
      bRange.setStart(b, 0);
      bRange.setEnd(b, 0);
      var ret = aRange.compareBoundaryPoints(Range.START_TO_END, bRange);
      if (ret === 0) {
        hasDuplicate = true;
      }
      return ret;
    };
  }

  (function() {
    var form = document.createElement("div"),
        id = "script" + (new Date).getTime();
    form.innerHTML = "<a name='" + id + "'/>";

    var root = document.documentElement;
    root.insertBefore(form, root.firstChild);

    if ( !! document.getElementById(id)) {
      Expr.find.ID = function(match, context, isXML) {
        if (typeof context.getElementById !== "undefined" && !isXML) {
          var m = context.getElementById(match[1]);
          return m ? m.id === match[1] || typeof m.getAttributeNode !== "undefined" && m.getAttributeNode("id").nodeValue === match[1] ? [m] : undefined : [];
        }
      };

      Expr.filter.ID = function(elem, match) {
        var node = typeof elem.getAttributeNode !== "undefined" && elem.getAttributeNode("id");
        return elem.nodeType === 1 && node && node.nodeValue === match;
      };
    }

    root.removeChild(form);
    root = form = null; // release memory in IE
  })();

  (function() {

    var div = document.createElement("div");
    div.appendChild(document.createComment(""));

    if (div.getElementsByTagName("*").length > 0) {
      Expr.find.TAG = function(match, context) {
        var results = context.getElementsByTagName(match[1]);

        if (match[1] === "*") {
          var tmp = [];

          for (var i = 0; results[i]; i++) {
            if (results[i].nodeType === 1) {
              tmp.push(results[i]);
            }
          }

          results = tmp;
        }

        return results;
      };
    }

    div.innerHTML = "<a href='#'></a>";
    if (div.firstChild && typeof div.firstChild.getAttribute !== "undefined" && div.firstChild.getAttribute("href") !== "#") {
      Expr.attrHandle.href = function(elem) {
        return elem.getAttribute("href", 2);
      };
    }

    div = null; // release memory in IE
  })();

  if (document.querySelectorAll)(function() {
    var oldSizzle = Sizzle,
        div = document.createElement("div");
    div.innerHTML = "<p class='TEST'></p>";

    if (div.querySelectorAll && div.querySelectorAll(".TEST").length === 0) {
      return;
    }

    Sizzle = function(query, context, extra, seed) {
      context = context || document;

      if (!seed && context.nodeType === 9 && !isXML(context)) {
        try {
          return makeArray(context.querySelectorAll(query), extra);
        } catch (e) {}
      }

      return oldSizzle(query, context, extra, seed);
    };

    for (var prop in oldSizzle) {
      Sizzle[prop] = oldSizzle[prop];
    }

    div = null; // release memory in IE
  })();

  if (document.getElementsByClassName && document.documentElement.getElementsByClassName)(function() {
    var div = document.createElement("div");
    div.innerHTML = "<div class='test e'></div><div class='test'></div>";

    if (div.getElementsByClassName("e").length === 0) return;

    div.lastChild.className = "e";

    if (div.getElementsByClassName("e").length === 1) return;

    Expr.order.splice(1, 0, "CLASS");
    Expr.find.CLASS = function(match, context, isXML) {
      if (typeof context.getElementsByClassName !== "undefined" && !isXML) {
        return context.getElementsByClassName(match[1]);
      }
    };

    div = null; // release memory in IE
  })();

  function dirNodeCheck(dir, cur, doneName, checkSet, nodeCheck, isXML) {
    var sibDir = dir == "previousSibling" && !isXML;
    for (var i = 0, l = checkSet.length; i < l; i++) {
      var elem = checkSet[i];
      if (elem) {
        if (sibDir && elem.nodeType === 1) {
          elem.sizcache = doneName;
          elem.sizset = i;
        }
        elem = elem[dir];
        var match = false;

        while (elem) {
          if (elem.sizcache === doneName) {
            match = checkSet[elem.sizset];
            break;
          }

          if (elem.nodeType === 1 && !isXML) {
            elem.sizcache = doneName;
            elem.sizset = i;
          }

          if (elem.nodeName === cur) {
            match = elem;
            break;
          }

          elem = elem[dir];
        }

        checkSet[i] = match;
      }
    }
  }

  function dirCheck(dir, cur, doneName, checkSet, nodeCheck, isXML) {
    var sibDir = dir == "previousSibling" && !isXML;
    for (var i = 0, l = checkSet.length; i < l; i++) {
      var elem = checkSet[i];
      if (elem) {
        if (sibDir && elem.nodeType === 1) {
          elem.sizcache = doneName;
          elem.sizset = i;
        }
        elem = elem[dir];
        var match = false;

        while (elem) {
          if (elem.sizcache === doneName) {
            match = checkSet[elem.sizset];
            break;
          }

          if (elem.nodeType === 1) {
            if (!isXML) {
              elem.sizcache = doneName;
              elem.sizset = i;
            }
            if (typeof cur !== "string") {
              if (elem === cur) {
                match = true;
                break;
              }

            } else if (Sizzle.filter(cur, [elem]).length > 0) {
              match = elem;
              break;
            }
          }

          elem = elem[dir];
        }

        checkSet[i] = match;
      }
    }
  }

  var contains = document.compareDocumentPosition ?
  function(a, b) {
    return a.compareDocumentPosition(b) & 16;
  } : function(a, b) {
    return a !== b && (a.contains ? a.contains(b) : true);
  };

  var isXML = function(elem) {
    return elem.nodeType === 9 && elem.documentElement.nodeName !== "HTML" || !! elem.ownerDocument && elem.ownerDocument.documentElement.nodeName !== "HTML";
  };

  var posProcess = function(selector, context) {
    var tmpSet = [],
        later = "",
        match, root = context.nodeType ? [context] : context;

    while ((match = Expr.match.PSEUDO.exec(selector))) {
      later += match[0];
      selector = selector.replace(Expr.match.PSEUDO, "");
    }

    selector = Expr.relative[selector] ? selector + "*" : selector;

    for (var i = 0, l = root.length; i < l; i++) {
      Sizzle(selector, root[i], tmpSet);
    }

    return Sizzle.filter(later, tmpSet);
  };


  window.Sizzle = Sizzle;

})();

;
(function(engine) {
  var extendElements = Prototype.Selector.extendElements;

  function select(selector, scope) {
    return extendElements(engine(selector, scope || document));
  }

  function match(element, selector) {
    return engine.matches(selector, [element]).length == 1;
  }

  Prototype.Selector.engine = engine;
  Prototype.Selector.select = select;
  Prototype.Selector.match = match;
})(Sizzle);

window.Sizzle = Prototype._original_property;
delete Prototype._original_property;

var Form = {
  reset: function(form) {
    form = $(form);
    form.reset();
    return form;
  },

  serializeElements: function(elements, options) {
    if (typeof options != 'object') options = {
      hash: !! options
    };
    else if (Object.isUndefined(options.hash)) options.hash = true;
    var key, value, submitted = false,
        submit = options.submit,
        accumulator, initial;

    if (options.hash) {
      initial = {};
      accumulator = function(result, key, value) {
        if (key in result) {
          if (!Object.isArray(result[key])) result[key] = [result[key]];
          result[key].push(value);
        } else result[key] = value;
        return result;
      };
    } else {
      initial = '';
      accumulator = function(result, key, value) {
        return result + (result ? '&' : '') + encodeURIComponent(key) + '=' + encodeURIComponent(value);
      }
    }

    return elements.inject(initial, function(result, element) {
      if (!element.disabled && element.name) {
        key = element.name;
        value = $(element).getValue();
        if (value != null && element.type != 'file' && (element.type != 'submit' || (!submitted && submit !== false && (!submit || key == submit) && (submitted = true)))) {
          result = accumulator(result, key, value);
        }
      }
      return result;
    });
  }
};

Form.Methods = {
  serialize: function(form, options) {
    return Form.serializeElements(Form.getElements(form), options);
  },

  getElements: function(form) {
    var elements = $(form).getElementsByTagName('*'),
        element, arr = [],
        serializers = Form.Element.Serializers;
    for (var i = 0; element = elements[i]; i++) {
      arr.push(element);
    }
    return arr.inject([], function(elements, child) {
      if (serializers[child.tagName.toLowerCase()]) elements.push(Element.extend(child));
      return elements;
    })
  },

  getInputs: function(form, typeName, name) {
    form = $(form);
    var inputs = form.getElementsByTagName('input');

    if (!typeName && !name) return $A(inputs).map(Element.extend);

    for (var i = 0, matchingInputs = [], length = inputs.length; i < length; i++) {
      var input = inputs[i];
      if ((typeName && input.type != typeName) || (name && input.name != name)) continue;
      matchingInputs.push(Element.extend(input));
    }

    return matchingInputs;
  },

  disable: function(form) {
    form = $(form);
    Form.getElements(form).invoke('disable');
    return form;
  },

  enable: function(form) {
    form = $(form);
    Form.getElements(form).invoke('enable');
    return form;
  },

  findFirstElement: function(form) {
    var elements = $(form).getElements().findAll(function(element) {
      return 'hidden' != element.type && !element.disabled;
    });
    var firstByIndex = elements.findAll(function(element) {
      return element.hasAttribute('tabIndex') && element.tabIndex >= 0;
    }).sortBy(function(element) {
      return element.tabIndex
    }).first();

    return firstByIndex ? firstByIndex : elements.find(function(element) {
      return /^(?:input|select|textarea)$/i.test(element.tagName);
    });
  },

  focusFirstElement: function(form) {
    form = $(form);
    var element = form.findFirstElement();
    if (element) element.activate();
    return form;
  },

  request: function(form, options) {
    form = $(form), options = Object.clone(options || {});

    var params = options.parameters,
        action = form.readAttribute('action') || '';
    if (action.blank()) action = window.location.href;
    options.parameters = form.serialize(true);

    if (params) {
      if (Object.isString(params)) params = params.toQueryParams();
      Object.extend(options.parameters, params);
    }

    if (form.hasAttribute('method') && !options.method) options.method = form.method;

    return new Ajax.Request(action, options);
  }
};

/*--------------------------------------------------------------------------*/


Form.Element = {
  focus: function(element) {
    $(element).focus();
    return element;
  },

  select: function(element) {
    $(element).select();
    return element;
  }
};

Form.Element.Methods = {

  serialize: function(element) {
    element = $(element);
    if (!element.disabled && element.name) {
      var value = element.getValue();
      if (value != undefined) {
        var pair = {};
        pair[element.name] = value;
        return Object.toQueryString(pair);
      }
    }
    return '';
  },

  getValue: function(element) {
    element = $(element);
    var method = element.tagName.toLowerCase();
    return Form.Element.Serializers[method](element);
  },

  setValue: function(element, value) {
    element = $(element);
    var method = element.tagName.toLowerCase();
    Form.Element.Serializers[method](element, value);
    return element;
  },

  clear: function(element) {
    $(element).value = '';
    return element;
  },

  present: function(element) {
    return $(element).value != '';
  },

  activate: function(element) {
    element = $(element);
    try {
      element.focus();
      if (element.select && (element.tagName.toLowerCase() != 'input' || !(/^(?:button|reset|submit)$/i.test(element.type)))) element.select();
    } catch (e) {}
    return element;
  },

  disable: function(element) {
    element = $(element);
    element.disabled = true;
    return element;
  },

  enable: function(element) {
    element = $(element);
    element.disabled = false;
    return element;
  }
};

/*--------------------------------------------------------------------------*/

var Field = Form.Element;

var $F = Form.Element.Methods.getValue;

/*--------------------------------------------------------------------------*/

Form.Element.Serializers = (function() {
  function input(element, value) {
    switch (element.type.toLowerCase()) {
    case 'checkbox':
    case 'radio':
      return inputSelector(element, value);
    default:
      return valueSelector(element, value);
    }
  }

  function inputSelector(element, value) {
    if (Object.isUndefined(value)) return element.checked ? element.value : null;
    else element.checked = !! value;
  }

  function valueSelector(element, value) {
    if (Object.isUndefined(value)) return element.value;
    else element.value = value;
  }

  function select(element, value) {
    if (Object.isUndefined(value)) return (element.type === 'select-one' ? selectOne : selectMany)(element);

    var opt, currentValue, single = !Object.isArray(value);
    for (var i = 0, length = element.length; i < length; i++) {
      opt = element.options[i];
      currentValue = this.optionValue(opt);
      if (single) {
        if (currentValue == value) {
          opt.selected = true;
          return;
        }
      } else opt.selected = value.include(currentValue);
    }
  }

  function selectOne(element) {
    var index = element.selectedIndex;
    return index >= 0 ? optionValue(element.options[index]) : null;
  }

  function selectMany(element) {
    var values, length = element.length;
    if (!length) return null;

    for (var i = 0, values = []; i < length; i++) {
      var opt = element.options[i];
      if (opt.selected) values.push(optionValue(opt));
    }
    return values;
  }

  function optionValue(opt) {
    return Element.hasAttribute(opt, 'value') ? opt.value : opt.text;
  }

  return {
    input: input,
    inputSelector: inputSelector,
    textarea: valueSelector,
    select: select,
    selectOne: selectOne,
    selectMany: selectMany,
    optionValue: optionValue,
    button: valueSelector
  };
})();

/*--------------------------------------------------------------------------*/


Abstract.TimedObserver = Class.create(PeriodicalExecuter, {
  initialize: function($super, element, frequency, callback) {
    $super(callback, frequency);
    this.element = $(element);
    this.lastValue = this.getValue();
  },

  execute: function() {
    var value = this.getValue();
    if (Object.isString(this.lastValue) && Object.isString(value) ? this.lastValue != value : String(this.lastValue) != String(value)) {
      this.callback(this.element, value);
      this.lastValue = value;
    }
  }
});

Form.Element.Observer = Class.create(Abstract.TimedObserver, {
  getValue: function() {
    return Form.Element.getValue(this.element);
  }
});

Form.Observer = Class.create(Abstract.TimedObserver, {
  getValue: function() {
    return Form.serialize(this.element);
  }
});

/*--------------------------------------------------------------------------*/

Abstract.EventObserver = Class.create({
  initialize: function(element, callback) {
    this.element = $(element);
    this.callback = callback;

    this.lastValue = this.getValue();
    if (this.element.tagName.toLowerCase() == 'form') this.registerFormCallbacks();
    else
    this.registerCallback(this.element);
  },

  onElementEvent: function() {
    var value = this.getValue();
    if (this.lastValue != value) {
      this.callback(this.element, value);
      this.lastValue = value;
    }
  },

  registerFormCallbacks: function() {
    Form.getElements(this.element).each(this.registerCallback, this);
  },

  registerCallback: function(element) {
    if (element.type) {
      switch (element.type.toLowerCase()) {
      case 'checkbox':
      case 'radio':
        Event.observe(element, 'click', this.onElementEvent.bind(this));
        break;
      default:
        Event.observe(element, 'change', this.onElementEvent.bind(this));
        break;
      }
    }
  }
});

Form.Element.EventObserver = Class.create(Abstract.EventObserver, {
  getValue: function() {
    return Form.Element.getValue(this.element);
  }
});

Form.EventObserver = Class.create(Abstract.EventObserver, {
  getValue: function() {
    return Form.serialize(this.element);
  }
});
(function() {

  var Event = {
    KEY_BACKSPACE: 8,
    KEY_TAB: 9,
    KEY_RETURN: 13,
    KEY_ESC: 27,
    KEY_LEFT: 37,
    KEY_UP: 38,
    KEY_RIGHT: 39,
    KEY_DOWN: 40,
    KEY_DELETE: 46,
    KEY_HOME: 36,
    KEY_END: 35,
    KEY_PAGEUP: 33,
    KEY_PAGEDOWN: 34,
    KEY_INSERT: 45,

    cache: {}
  };

  var docEl = document.documentElement;
  var MOUSEENTER_MOUSELEAVE_EVENTS_SUPPORTED = 'onmouseenter' in docEl && 'onmouseleave' in docEl;



  var isIELegacyEvent = function(event) {
    return false;
  };

  if (window.attachEvent) {
    if (window.addEventListener) {
      isIELegacyEvent = function(event) {
        return !(event instanceof window.Event);
      };
    } else {
      isIELegacyEvent = function(event) {
        return true;
      };
    }
  }

  var _isButton;

  function _isButtonForDOMEvents(event, code) {
    return event.which ? (event.which === code + 1) : (event.button === code);
  }

  var legacyButtonMap = {
    0: 1,
    1: 4,
    2: 2
  };

  function _isButtonForLegacyEvents(event, code) {
    return event.button === legacyButtonMap[code];
  }

  function _isButtonForWebKit(event, code) {
    switch (code) {
    case 0:
      return event.which == 1 && !event.metaKey;
    case 1:
      return event.which == 2 || (event.which == 1 && event.metaKey);
    case 2:
      return event.which == 3;
    default:
      return false;
    }
  }

  if (window.attachEvent) {
    if (!window.addEventListener) {
      _isButton = _isButtonForLegacyEvents;
    } else {
      _isButton = function(event, code) {
        return isIELegacyEvent(event) ? _isButtonForLegacyEvents(event, code) : _isButtonForDOMEvents(event, code);
      }
    }
  } else if (Prototype.Browser.WebKit) {
    _isButton = _isButtonForWebKit;
  } else {
    _isButton = _isButtonForDOMEvents;
  }

  function isLeftClick(event) {
    return _isButton(event, 0)
  }

  function isMiddleClick(event) {
    return _isButton(event, 1)
  }

  function isRightClick(event) {
    return _isButton(event, 2)
  }

  function element(event) {
    event = Event.extend(event);

    var node = event.target,
        type = event.type,
        currentTarget = event.currentTarget;

    if (currentTarget && currentTarget.tagName) {
      if (type === 'load' || type === 'error' || (type === 'click' && currentTarget.tagName.toLowerCase() === 'input' && currentTarget.type === 'radio')) node = currentTarget;
    }

    if (node.nodeType == Node.TEXT_NODE) node = node.parentNode;

    return Element.extend(node);
  }

  function findElement(event, expression) {
    var element = Event.element(event);

    if (!expression) return element;
    while (element) {
      if (Object.isElement(element) && Prototype.Selector.match(element, expression)) {
        return Element.extend(element);
      }
      element = element.parentNode;
    }
  }

  function pointer(event) {
    return {
      x: pointerX(event),
      y: pointerY(event)
    };
  }

  function pointerX(event) {
    var docElement = document.documentElement,
        body = document.body || {
        scrollLeft: 0
        };

    return event.pageX || (event.clientX + (docElement.scrollLeft || body.scrollLeft) - (docElement.clientLeft || 0));
  }

  function pointerY(event) {
    var docElement = document.documentElement,
        body = document.body || {
        scrollTop: 0
        };

    return event.pageY || (event.clientY + (docElement.scrollTop || body.scrollTop) - (docElement.clientTop || 0));
  }


  function stop(event) {
    Event.extend(event);
    event.preventDefault();
    event.stopPropagation();

    event.stopped = true;
  }


  Event.Methods = {
    isLeftClick: isLeftClick,
    isMiddleClick: isMiddleClick,
    isRightClick: isRightClick,

    element: element,
    findElement: findElement,

    pointer: pointer,
    pointerX: pointerX,
    pointerY: pointerY,

    stop: stop
  };

  var methods = Object.keys(Event.Methods).inject({}, function(m, name) {
    m[name] = Event.Methods[name].methodize();
    return m;
  });

  if (window.attachEvent) {
    function _relatedTarget(event) {
      var element;
      switch (event.type) {
      case 'mouseover':
      case 'mouseenter':
        element = event.fromElement;
        break;
      case 'mouseout':
      case 'mouseleave':
        element = event.toElement;
        break;
      default:
        return null;
      }
      return Element.extend(element);
    }

    var additionalMethods = {
      stopPropagation: function() {
        this.cancelBubble = true
      },
      preventDefault: function() {
        this.returnValue = false
      },
      inspect: function() {
        return '[object Event]'
      }
    };

    Event.extend = function(event, element) {
      if (!event) return false;

      if (!isIELegacyEvent(event)) return event;

      if (event._extendedByPrototype) return event;
      event._extendedByPrototype = Prototype.emptyFunction;

      var pointer = Event.pointer(event);

      Object.extend(event, {
        target: event.srcElement || element,
        relatedTarget: _relatedTarget(event),
        pageX: pointer.x,
        pageY: pointer.y
      });

      Object.extend(event, methods);
      Object.extend(event, additionalMethods);

      return event;
    };
  } else {
    Event.extend = Prototype.K;
  }

  if (window.addEventListener) {
    Event.prototype = window.Event.prototype || document.createEvent('HTMLEvents').__proto__;
    Object.extend(Event.prototype, methods);
  }

  function _createResponder(element, eventName, handler) {
    var registry = Element.retrieve(element, 'prototype_event_registry');

    if (Object.isUndefined(registry)) {
      CACHE.push(element);
      registry = Element.retrieve(element, 'prototype_event_registry', $H());
    }

    var respondersForEvent = registry.get(eventName);
    if (Object.isUndefined(respondersForEvent)) {
      respondersForEvent = [];
      registry.set(eventName, respondersForEvent);
    }

    if (respondersForEvent.pluck('handler').include(handler)) return false;

    var responder;
    if (eventName.include(":")) {
      responder = function(event) {
        if (Object.isUndefined(event.eventName)) return false;

        if (event.eventName !== eventName) return false;

        Event.extend(event, element);
        handler.call(element, event);
      };
    } else {
      if (!MOUSEENTER_MOUSELEAVE_EVENTS_SUPPORTED && (eventName === "mouseenter" || eventName === "mouseleave")) {
        if (eventName === "mouseenter" || eventName === "mouseleave") {
          responder = function(event) {
            Event.extend(event, element);

            var parent = event.relatedTarget;
            while (parent && parent !== element) {
              try {
                parent = parent.parentNode;
              } catch (e) {
                parent = element;
              }
            }

            if (parent === element) return;

            handler.call(element, event);
          };
        }
      } else {
        responder = function(event) {
          Event.extend(event, element);
          handler.call(element, event);
        };
      }
    }

    responder.handler = handler;
    respondersForEvent.push(responder);
    return responder;
  }

  function _destroyCache() {
    for (var i = 0, length = CACHE.length; i < length; i++) {
      Event.stopObserving(CACHE[i]);
      CACHE[i] = null;
    }
  }

  var CACHE = [];

  if (Prototype.Browser.IE) window.attachEvent('onunload', _destroyCache);

  if (Prototype.Browser.WebKit) window.addEventListener('unload', Prototype.emptyFunction, false);


  var _getDOMEventName = Prototype.K,
      translations = {
      mouseenter: "mouseover",
      mouseleave: "mouseout"
      };

  if (!MOUSEENTER_MOUSELEAVE_EVENTS_SUPPORTED) {
    _getDOMEventName = function(eventName) {
      return (translations[eventName] || eventName);
    };
  }

  function observe(element, eventName, handler) {
    element = $(element);

    var responder = _createResponder(element, eventName, handler);

    if (!responder) return element;

    if (eventName.include(':')) {
      if (element.addEventListener) element.addEventListener("dataavailable", responder, false);
      else {
        element.attachEvent("ondataavailable", responder);
        element.attachEvent("onlosecapture", responder);
      }
    } else {
      var actualEventName = _getDOMEventName(eventName);

      if (element.addEventListener) element.addEventListener(actualEventName, responder, false);
      else
      element.attachEvent("on" + actualEventName, responder);
    }

    return element;
  }

  function stopObserving(element, eventName, handler) {
    element = $(element);

    var registry = Element.retrieve(element, 'prototype_event_registry');
    if (!registry) return element;

    if (!eventName) {
      registry.each(function(pair) {
        var eventName = pair.key;
        stopObserving(element, eventName);
      });
      return element;
    }

    var responders = registry.get(eventName);
    if (!responders) return element;

    if (!handler) {
      responders.each(function(r) {
        stopObserving(element, eventName, r.handler);
      });
      return element;
    }

    var i = responders.length,
        responder;
    while (i--) {
      if (responders[i].handler === handler) {
        responder = responders[i];
        break;
      }
    }
    if (!responder) return element;

    if (eventName.include(':')) {
      if (element.removeEventListener) element.removeEventListener("dataavailable", responder, false);
      else {
        element.detachEvent("ondataavailable", responder);
        element.detachEvent("onlosecapture", responder);
      }
    } else {
      var actualEventName = _getDOMEventName(eventName);
      if (element.removeEventListener) element.removeEventListener(actualEventName, responder, false);
      else
      element.detachEvent('on' + actualEventName, responder);
    }

    registry.set(eventName, responders.without(responder));

    return element;
  }

  function fire(element, eventName, memo, bubble) {
    element = $(element);

    if (Object.isUndefined(bubble)) bubble = true;

    if (element == document && document.createEvent && !element.dispatchEvent) element = document.documentElement;

    var event;
    if (document.createEvent) {
      event = document.createEvent('HTMLEvents');
      event.initEvent('dataavailable', bubble, true);
    } else {
      event = document.createEventObject();
      event.eventType = bubble ? 'ondataavailable' : 'onlosecapture';
    }

    event.eventName = eventName;
    event.memo = memo || {};

    if (document.createEvent) element.dispatchEvent(event);
    else
    element.fireEvent(event.eventType, event);

    return Event.extend(event);
  }

  Event.Handler = Class.create({
    initialize: function(element, eventName, selector, callback) {
      this.element = $(element);
      this.eventName = eventName;
      this.selector = selector;
      this.callback = callback;
      this.handler = this.handleEvent.bind(this);
    },

    start: function() {
      Event.observe(this.element, this.eventName, this.handler);
      return this;
    },

    stop: function() {
      Event.stopObserving(this.element, this.eventName, this.handler);
      return this;
    },

    handleEvent: function(event) {
      var element = Event.findElement(event, this.selector);
      if (element) this.callback.call(this.element, event, element);
    }
  });

  function on(element, eventName, selector, callback) {
    element = $(element);
    if (Object.isFunction(selector) && Object.isUndefined(callback)) {
      callback = selector, selector = null;
    }

    return new Event.Handler(element, eventName, selector, callback).start();
  }

  Object.extend(Event, Event.Methods);

  Object.extend(Event, {
    fire: fire,
    observe: observe,
    stopObserving: stopObserving,
    on: on
  });

  Element.addMethods({
    fire: fire,

    observe: observe,

    stopObserving: stopObserving,

    on: on
  });

  Object.extend(document, {
    fire: fire.methodize(),

    observe: observe.methodize(),

    stopObserving: stopObserving.methodize(),

    on: on.methodize(),

    loaded: false
  });

  if (window.Event) Object.extend(window.Event, Event);
  else window.Event = Event;
})();

(function() {
/* Support for the DOMContentLoaded event is based on work by Dan Webb,
     Matthias Miller, Dean Edwards, John Resig, and Diego Perini. */

  var timer;

  function fireContentLoadedEvent() {
    if (document.loaded) return;
    if (timer) window.clearTimeout(timer);
    document.loaded = true;
    document.fire('dom:loaded');
  }

  function checkReadyState() {
    if (document.readyState === 'complete') {
      document.stopObserving('readystatechange', checkReadyState);
      fireContentLoadedEvent();
    }
  }

  function pollDoScroll() {
    try {
      document.documentElement.doScroll('left');
    } catch (e) {
      timer = pollDoScroll.defer();
      return;
    }
    fireContentLoadedEvent();
  }

  if (document.addEventListener) {
    document.addEventListener('DOMContentLoaded', fireContentLoadedEvent, false);
  } else {
    document.observe('readystatechange', checkReadyState);
    if (window == top) timer = pollDoScroll.defer();
  }

  Event.observe(window, 'load', fireContentLoadedEvent);
})();

Element.addMethods();

/*------------------------------- DEPRECATED -------------------------------*/

Hash.toQueryString = Object.toQueryString;

var Toggle = {
  display: Element.toggle
};

Element.Methods.childOf = Element.Methods.descendantOf;

var Insertion = {
  Before: function(element, content) {
    return Element.insert(element, {
      before: content
    });
  },

  Top: function(element, content) {
    return Element.insert(element, {
      top: content
    });
  },

  Bottom: function(element, content) {
    return Element.insert(element, {
      bottom: content
    });
  },

  After: function(element, content) {
    return Element.insert(element, {
      after: content
    });
  }
};

var $continue = new Error('"throw $continue" is deprecated, use "return" instead');

var Position = {
  includeScrollOffsets: false,

  prepare: function() {
    this.deltaX = window.pageXOffset || document.documentElement.scrollLeft || document.body.scrollLeft || 0;
    this.deltaY = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
  },

  within: function(element, x, y) {
    if (this.includeScrollOffsets) return this.withinIncludingScrolloffsets(element, x, y);
    this.xcomp = x;
    this.ycomp = y;
    this.offset = Element.cumulativeOffset(element);

    return (y >= this.offset[1] && y < this.offset[1] + element.offsetHeight && x >= this.offset[0] && x < this.offset[0] + element.offsetWidth);
  },

  withinIncludingScrolloffsets: function(element, x, y) {
    var offsetcache = Element.cumulativeScrollOffset(element);

    this.xcomp = x + offsetcache[0] - this.deltaX;
    this.ycomp = y + offsetcache[1] - this.deltaY;
    this.offset = Element.cumulativeOffset(element);

    return (this.ycomp >= this.offset[1] && this.ycomp < this.offset[1] + element.offsetHeight && this.xcomp >= this.offset[0] && this.xcomp < this.offset[0] + element.offsetWidth);
  },

  overlap: function(mode, element) {
    if (!mode) return 0;
    if (mode == 'vertical') return ((this.offset[1] + element.offsetHeight) - this.ycomp) / element.offsetHeight;
    if (mode == 'horizontal') return ((this.offset[0] + element.offsetWidth) - this.xcomp) / element.offsetWidth;
  },


  cumulativeOffset: Element.Methods.cumulativeOffset,

  positionedOffset: Element.Methods.positionedOffset,

  absolutize: function(element) {
    Position.prepare();
    return Element.absolutize(element);
  },

  relativize: function(element) {
    Position.prepare();
    return Element.relativize(element);
  },

  realOffset: Element.Methods.cumulativeScrollOffset,

  offsetParent: Element.Methods.getOffsetParent,

  page: Element.Methods.viewportOffset,

  clone: function(source, target, options) {
    options = options || {};
    return Element.clonePosition(target, source, options);
  }
};

/*--------------------------------------------------------------------------*/

if (!document.getElementsByClassName) document.getElementsByClassName = function(instanceMethods) {
  function iter(name) {
    return name.blank() ? null : "[contains(concat(' ', @class, ' '), ' " + name + " ')]";
  }

  instanceMethods.getElementsByClassName = Prototype.BrowserFeatures.XPath ?
  function(element, className) {
    className = className.toString().strip();
    var cond = /\s/.test(className) ? $w(className).map(iter).join('') : iter(className);
    return cond ? document._getElementsByXPath('.//*' + cond, element) : [];
  } : function(element, className) {
    className = className.toString().strip();
    var elements = [],
        classNames = (/\s/.test(className) ? $w(className) : null);
    if (!classNames && !className) return elements;

    var nodes = $(element).getElementsByTagName('*');
    className = ' ' + className + ' ';

    for (var i = 0, child, cn; child = nodes[i]; i++) {
      if (child.className && (cn = ' ' + child.className + ' ') && (cn.include(className) || (classNames && classNames.all(function(name) {
        return !name.toString().blank() && cn.include(' ' + name + ' ');
      })))) elements.push(Element.extend(child));
    }
    return elements;
  };

  return function(className, parentElement) {
    return $(parentElement || document.body).getElementsByClassName(className);
  };
}(Element.Methods);

/*--------------------------------------------------------------------------*/

Element.ClassNames = Class.create();
Element.ClassNames.prototype = {
  initialize: function(element) {
    this.element = $(element);
  },

  _each: function(iterator) {
    this.element.className.split(/\s+/).select(function(name) {
      return name.length > 0;
    })._each(iterator);
  },

  set: function(className) {
    this.element.className = className;
  },

  add: function(classNameToAdd) {
    if (this.include(classNameToAdd)) return;
    this.set($A(this).concat(classNameToAdd).join(' '));
  },

  remove: function(classNameToRemove) {
    if (!this.include(classNameToRemove)) return;
    this.set($A(this).without(classNameToRemove).join(' '));
  },

  toString: function() {
    return $A(this).join(' ');
  }
};

Object.extend(Element.ClassNames.prototype, Enumerable);

/*--------------------------------------------------------------------------*/

(function() {
  window.Selector = Class.create({
    initialize: function(expression) {
      this.expression = expression.strip();
    },

    findElements: function(rootElement) {
      return Prototype.Selector.select(this.expression, rootElement);
    },

    match: function(element) {
      return Prototype.Selector.match(element, this.expression);
    },

    toString: function() {
      return this.expression;
    },

    inspect: function() {
      return "#<Selector: " + this.expression + ">";
    }
  });

  Object.extend(Selector, {
    matchElements: function(elements, expression) {
      var match = Prototype.Selector.match,
          results = [];

      for (var i = 0, length = elements.length; i < length; i++) {
        var element = elements[i];
        if (match(element, expression)) {
          results.push(Element.extend(element));
        }
      }
      return results;
    },

    findElement: function(elements, expression, index) {
      index = index || 0;
      var matchIndex = 0,
          element;
      for (var i = 0, length = elements.length; i < length; i++) {
        element = elements[i];
        if (Prototype.Selector.match(element, expression) && index === matchIndex++) {
          return Element.extend(element);
        }
      }
    },

    findChildElements: function(element, expressions) {
      var selector = expressions.toArray().join(', ');
      return Prototype.Selector.select(selector, element || document);
    }
  });
})();
// Copyright (c) 2005-2008 Thomas Fuchs (http://script.aculo.us, http://mir.aculo.us)
// Contributors:
//  Justin Palmer (http://encytemedia.com/)
//  Mark Pilgrim (http://diveintomark.org/)
//  Martin Bialasinki
//
// script.aculo.us is freely distributable under the terms of an MIT-style license.
// For details, see the script.aculo.us web site: http://script.aculo.us/

// converts rgb() and #xxx to #xxxxxx format,
// returns self (or first argument) if not convertable
String.prototype.parseColor = function() {
  var color = '#';
  if (this.slice(0,4) == 'rgb(') {
    var cols = this.slice(4,this.length-1).split(',');
    var i=0; do { color += parseInt(cols[i]).toColorPart() } while (++i<3);
  } else {
    if (this.slice(0,1) == '#') {
      if (this.length==4) for(var i=1;i<4;i++) color += (this.charAt(i) + this.charAt(i)).toLowerCase();
      if (this.length==7) color = this.toLowerCase();
    }
  }
  return (color.length==7 ? color : (arguments[0] || this));
};

/*--------------------------------------------------------------------------*/

Element.collectTextNodes = function(element) {
  return $A($(element).childNodes).collect( function(node) {
    return (node.nodeType==3 ? node.nodeValue :
      (node.hasChildNodes() ? Element.collectTextNodes(node) : ''));
  }).flatten().join('');
};

Element.collectTextNodesIgnoreClass = function(element, className) {
  return $A($(element).childNodes).collect( function(node) {
    return (node.nodeType==3 ? node.nodeValue :
      ((node.hasChildNodes() && !Element.hasClassName(node,className)) ?
        Element.collectTextNodesIgnoreClass(node, className) : ''));
  }).flatten().join('');
};

Element.setContentZoom = function(element, percent) {
  element = $(element);
  element.setStyle({fontSize: (percent/100) + 'em'});
  if (Prototype.Browser.WebKit) window.scrollBy(0,0);
  return element;
};

Element.getInlineOpacity = function(element){
  return $(element).style.opacity || '';
};

Element.forceRerendering = function(element) {
  try {
    element = $(element);
    var n = document.createTextNode(' ');
    element.appendChild(n);
    element.removeChild(n);
  } catch(e) { }
};

/*--------------------------------------------------------------------------*/

var Effect = {
  _elementDoesNotExistError: {
    name: 'ElementDoesNotExistError',
    message: 'The specified DOM element does not exist, but is required for this effect to operate'
  },
  Transitions: {
    linear: Prototype.K,
    sinoidal: function(pos) {
      return (-Math.cos(pos*Math.PI)/2) + .5;
    },
    reverse: function(pos) {
      return 1-pos;
    },
    flicker: function(pos) {
      var pos = ((-Math.cos(pos*Math.PI)/4) + .75) + Math.random()/4;
      return pos > 1 ? 1 : pos;
    },
    wobble: function(pos) {
      return (-Math.cos(pos*Math.PI*(9*pos))/2) + .5;
    },
    pulse: function(pos, pulses) {
      return (-Math.cos((pos*((pulses||5)-.5)*2)*Math.PI)/2) + .5;
    },
    spring: function(pos) {
      return 1 - (Math.cos(pos * 4.5 * Math.PI) * Math.exp(-pos * 6));
    },
    none: function(pos) {
      return 0;
    },
    full: function(pos) {
      return 1;
    }
  },
  DefaultOptions: {
    duration:   1.0,   // seconds
    fps:        100,   // 100= assume 66fps max.
    sync:       false, // true for combining
    from:       0.0,
    to:         1.0,
    delay:      0.0,
    queue:      'parallel'
  },
  tagifyText: function(element) {
    var tagifyStyle = 'position:relative';
    if (Prototype.Browser.IE) tagifyStyle += ';zoom:1';

    element = $(element);
    $A(element.childNodes).each( function(child) {
      if (child.nodeType==3) {
        child.nodeValue.toArray().each( function(character) {
          element.insertBefore(
            new Element('span', {style: tagifyStyle}).update(
              character == ' ' ? String.fromCharCode(160) : character),
              child);
        });
        Element.remove(child);
      }
    });
  },
  multiple: function(element, effect) {
    var elements;
    if (((typeof element == 'object') ||
        Object.isFunction(element)) &&
       (element.length))
      elements = element;
    else
      elements = $(element).childNodes;

    var options = Object.extend({
      speed: 0.1,
      delay: 0.0
    }, arguments[2] || { });
    var masterDelay = options.delay;

    $A(elements).each( function(element, index) {
      new effect(element, Object.extend(options, { delay: index * options.speed + masterDelay }));
    });
  },
  PAIRS: {
    'slide':  ['SlideDown','SlideUp'],
    'blind':  ['BlindDown','BlindUp'],
    'appear': ['Appear','Fade']
  },
  toggle: function(element, effect) {
    element = $(element);
    effect = (effect || 'appear').toLowerCase();
    var options = Object.extend({
      queue: { position:'end', scope:(element.id || 'global'), limit: 1 }
    }, arguments[2] || { });
    Effect[element.visible() ?
      Effect.PAIRS[effect][1] : Effect.PAIRS[effect][0]](element, options);
  }
};

Effect.DefaultOptions.transition = Effect.Transitions.sinoidal;

/* ------------- core effects ------------- */

Effect.ScopedQueue = Class.create(Enumerable, {
  initialize: function() {
    this.effects  = [];
    this.interval = null;
  },
  _each: function(iterator) {
    this.effects._each(iterator);
  },
  add: function(effect) {
    var timestamp = new Date().getTime();

    var position = Object.isString(effect.options.queue) ?
      effect.options.queue : effect.options.queue.position;

    switch(position) {
      case 'front':
        // move unstarted effects after this effect
        this.effects.findAll(function(e){ return e.state=='idle' }).each( function(e) {
            e.startOn  += effect.finishOn;
            e.finishOn += effect.finishOn;
          });
        break;
      case 'with-last':
        timestamp = this.effects.pluck('startOn').max() || timestamp;
        break;
      case 'end':
        // start effect after last queued effect has finished
        timestamp = this.effects.pluck('finishOn').max() || timestamp;
        break;
    }

    effect.startOn  += timestamp;
    effect.finishOn += timestamp;

    if (!effect.options.queue.limit || (this.effects.length < effect.options.queue.limit))
      this.effects.push(effect);

    if (!this.interval)
      this.interval = setInterval(this.loop.bind(this), 15);
  },
  remove: function(effect) {
    this.effects = this.effects.reject(function(e) { return e==effect });
    if (this.effects.length == 0) {
      clearInterval(this.interval);
      this.interval = null;
    }
  },
  loop: function() {
    var timePos = new Date().getTime();
    for(var i=0, len=this.effects.length;i<len;i++)
      this.effects[i] && this.effects[i].loop(timePos);
  }
});

Effect.Queues = {
  instances: $H(),
  get: function(queueName) {
    if (!Object.isString(queueName)) return queueName;

    return this.instances.get(queueName) ||
      this.instances.set(queueName, new Effect.ScopedQueue());
  }
};
Effect.Queue = Effect.Queues.get('global');

Effect.Base = Class.create({
  position: null,
  start: function(options) {
    function codeForEvent(options,eventName){
      return (
        (options[eventName+'Internal'] ? 'this.options.'+eventName+'Internal(this);' : '') +
        (options[eventName] ? 'this.options.'+eventName+'(this);' : '')
      );
    }
    if (options && options.transition === false) options.transition = Effect.Transitions.linear;
    this.options      = Object.extend(Object.extend({ },Effect.DefaultOptions), options || { });
    this.currentFrame = 0;
    this.state        = 'idle';
    this.startOn      = this.options.delay*1000;
    this.finishOn     = this.startOn+(this.options.duration*1000);
    this.fromToDelta  = this.options.to-this.options.from;
    this.totalTime    = this.finishOn-this.startOn;
    this.totalFrames  = this.options.fps*this.options.duration;

    this.render = (function() {
      function dispatch(effect, eventName) {
        if (effect.options[eventName + 'Internal'])
          effect.options[eventName + 'Internal'](effect);
        if (effect.options[eventName])
          effect.options[eventName](effect);
      }

      return function(pos) {
        if (this.state === "idle") {
          this.state = "running";
          dispatch(this, 'beforeSetup');
          if (this.setup) this.setup();
          dispatch(this, 'afterSetup');
        }
        if (this.state === "running") {
          pos = (this.options.transition(pos) * this.fromToDelta) + this.options.from;
          this.position = pos;
          dispatch(this, 'beforeUpdate');
          if (this.update) this.update(pos);
          dispatch(this, 'afterUpdate');
        }
      };
    })();

    this.event('beforeStart');
    if (!this.options.sync)
      Effect.Queues.get(Object.isString(this.options.queue) ?
        'global' : this.options.queue.scope).add(this);
  },
  loop: function(timePos) {
    if (timePos >= this.startOn) {
      if (timePos >= this.finishOn) {
        this.render(1.0);
        this.cancel();
        this.event('beforeFinish');
        if (this.finish) this.finish();
        this.event('afterFinish');
        return;
      }
      var pos   = (timePos - this.startOn) / this.totalTime,
          frame = (pos * this.totalFrames).round();
      if (frame > this.currentFrame) {
        this.render(pos);
        this.currentFrame = frame;
      }
    }
  },
  cancel: function() {
    if (!this.options.sync)
      Effect.Queues.get(Object.isString(this.options.queue) ?
        'global' : this.options.queue.scope).remove(this);
    this.state = 'finished';
  },
  event: function(eventName) {
    if (this.options[eventName + 'Internal']) this.options[eventName + 'Internal'](this);
    if (this.options[eventName]) this.options[eventName](this);
  },
  inspect: function() {
    var data = $H();
    for(property in this)
      if (!Object.isFunction(this[property])) data.set(property, this[property]);
    return '#<Effect:' + data.inspect() + ',options:' + $H(this.options).inspect() + '>';
  }
});

Effect.Parallel = Class.create(Effect.Base, {
  initialize: function(effects) {
    this.effects = effects || [];
    this.start(arguments[1]);
  },
  update: function(position) {
    this.effects.invoke('render', position);
  },
  finish: function(position) {
    this.effects.each( function(effect) {
      effect.render(1.0);
      effect.cancel();
      effect.event('beforeFinish');
      if (effect.finish) effect.finish(position);
      effect.event('afterFinish');
    });
  }
});

Effect.Tween = Class.create(Effect.Base, {
  initialize: function(object, from, to) {
    object = Object.isString(object) ? $(object) : object;
    var args = $A(arguments), method = args.last(),
      options = args.length == 5 ? args[3] : null;
    this.method = Object.isFunction(method) ? method.bind(object) :
      Object.isFunction(object[method]) ? object[method].bind(object) :
      function(value) { object[method] = value };
    this.start(Object.extend({ from: from, to: to }, options || { }));
  },
  update: function(position) {
    this.method(position);
  }
});

Effect.Event = Class.create(Effect.Base, {
  initialize: function() {
    this.start(Object.extend({ duration: 0 }, arguments[0] || { }));
  },
  update: Prototype.emptyFunction
});

Effect.Opacity = Class.create(Effect.Base, {
  initialize: function(element) {
    this.element = $(element);
    if (!this.element) throw(Effect._elementDoesNotExistError);
    // make this work on IE on elements without 'layout'
    if (Prototype.Browser.IE && (!this.element.currentStyle.hasLayout))
      this.element.setStyle({zoom: 1});
    var options = Object.extend({
      from: this.element.getOpacity() || 0.0,
      to:   1.0
    }, arguments[1] || { });
    this.start(options);
  },
  update: function(position) {
    this.element.setOpacity(position);
  }
});

Effect.Move = Class.create(Effect.Base, {
  initialize: function(element) {
    this.element = $(element);
    if (!this.element) throw(Effect._elementDoesNotExistError);
    var options = Object.extend({
      x:    0,
      y:    0,
      mode: 'relative'
    }, arguments[1] || { });
    this.start(options);
  },
  setup: function() {
    this.element.makePositioned();
    this.originalLeft = parseFloat(this.element.getStyle('left') || '0');
    this.originalTop  = parseFloat(this.element.getStyle('top')  || '0');
    if (this.options.mode == 'absolute') {
      this.options.x = this.options.x - this.originalLeft;
      this.options.y = this.options.y - this.originalTop;
    }
  },
  update: function(position) {
    this.element.setStyle({
      left: (this.options.x  * position + this.originalLeft).round() + 'px',
      top:  (this.options.y  * position + this.originalTop).round()  + 'px'
    });
  }
});

// for backwards compatibility
Effect.MoveBy = function(element, toTop, toLeft) {
  return new Effect.Move(element,
    Object.extend({ x: toLeft, y: toTop }, arguments[3] || { }));
};

Effect.Scale = Class.create(Effect.Base, {
  initialize: function(element, percent) {
    this.element = $(element);
    if (!this.element) throw(Effect._elementDoesNotExistError);
    var options = Object.extend({
      scaleX: true,
      scaleY: true,
      scaleContent: true,
      scaleFromCenter: false,
      scaleMode: 'box',        // 'box' or 'contents' or { } with provided values
      scaleFrom: 100.0,
      scaleTo:   percent
    }, arguments[2] || { });
    this.start(options);
  },
  setup: function() {
    this.restoreAfterFinish = this.options.restoreAfterFinish || false;
    this.elementPositioning = this.element.getStyle('position');

    this.originalStyle = { };
    ['top','left','width','height','fontSize'].each( function(k) {
      this.originalStyle[k] = this.element.style[k];
    }.bind(this));

    this.originalTop  = this.element.offsetTop;
    this.originalLeft = this.element.offsetLeft;

    var fontSize = this.element.getStyle('font-size') || '100%';
    ['em','px','%','pt'].each( function(fontSizeType) {
      if (fontSize.indexOf(fontSizeType)>0) {
        this.fontSize     = parseFloat(fontSize);
        this.fontSizeType = fontSizeType;
      }
    }.bind(this));

    this.factor = (this.options.scaleTo - this.options.scaleFrom)/100;

    this.dims = null;
    if (this.options.scaleMode=='box')
      this.dims = [this.element.offsetHeight, this.element.offsetWidth];
    if (/^content/.test(this.options.scaleMode))
      this.dims = [this.element.scrollHeight, this.element.scrollWidth];
    if (!this.dims)
      this.dims = [this.options.scaleMode.originalHeight,
                   this.options.scaleMode.originalWidth];
  },
  update: function(position) {
    var currentScale = (this.options.scaleFrom/100.0) + (this.factor * position);
    if (this.options.scaleContent && this.fontSize)
      this.element.setStyle({fontSize: this.fontSize * currentScale + this.fontSizeType });
    this.setDimensions(this.dims[0] * currentScale, this.dims[1] * currentScale);
  },
  finish: function(position) {
    if (this.restoreAfterFinish) this.element.setStyle(this.originalStyle);
  },
  setDimensions: function(height, width) {
    var d = { };
    if (this.options.scaleX) d.width = width.round() + 'px';
    if (this.options.scaleY) d.height = height.round() + 'px';
    if (this.options.scaleFromCenter) {
      var topd  = (height - this.dims[0])/2;
      var leftd = (width  - this.dims[1])/2;
      if (this.elementPositioning == 'absolute') {
        if (this.options.scaleY) d.top = this.originalTop-topd + 'px';
        if (this.options.scaleX) d.left = this.originalLeft-leftd + 'px';
      } else {
        if (this.options.scaleY) d.top = -topd + 'px';
        if (this.options.scaleX) d.left = -leftd + 'px';
      }
    }
    this.element.setStyle(d);
  }
});

Effect.Highlight = Class.create(Effect.Base, {
  initialize: function(element) {
    this.element = $(element);
    if (!this.element) throw(Effect._elementDoesNotExistError);
    var options = Object.extend({ startcolor: '#ffff99' }, arguments[1] || { });
    this.start(options);
  },
  setup: function() {
    // Prevent executing on elements not in the layout flow
    if (this.element.getStyle('display')=='none') { this.cancel(); return; }
    // Disable background image during the effect
    this.oldStyle = { };
    if (!this.options.keepBackgroundImage) {
      this.oldStyle.backgroundImage = this.element.getStyle('background-image');
      this.element.setStyle({backgroundImage: 'none'});
    }
    if (!this.options.endcolor)
      this.options.endcolor = this.element.getStyle('background-color').parseColor('#ffffff');
    if (!this.options.restorecolor)
      this.options.restorecolor = this.element.getStyle('background-color');
    // init color calculations
    this._base  = $R(0,2).map(function(i){ return parseInt(this.options.startcolor.slice(i*2+1,i*2+3),16) }.bind(this));
    this._delta = $R(0,2).map(function(i){ return parseInt(this.options.endcolor.slice(i*2+1,i*2+3),16)-this._base[i] }.bind(this));
  },
  update: function(position) {
    this.element.setStyle({backgroundColor: $R(0,2).inject('#',function(m,v,i){
      return m+((this._base[i]+(this._delta[i]*position)).round().toColorPart()); }.bind(this)) });
  },
  finish: function() {
    this.element.setStyle(Object.extend(this.oldStyle, {
      backgroundColor: this.options.restorecolor
    }));
  }
});

Effect.ScrollTo = function(element) {
  var options = arguments[1] || { },
  scrollOffsets = document.viewport.getScrollOffsets(),
  elementOffsets = $(element).cumulativeOffset();

  if (options.offset) elementOffsets[1] += options.offset;

  return new Effect.Tween(null,
    scrollOffsets.top,
    elementOffsets[1],
    options,
    function(p){ scrollTo(scrollOffsets.left, p.round()); }
  );
};

/* ------------- combination effects ------------- */

Effect.Fade = function(element) {
  element = $(element);
  var oldOpacity = element.getInlineOpacity();
  var options = Object.extend({
    from: element.getOpacity() || 1.0,
    to:   0.0,
    afterFinishInternal: function(effect) {
      if (effect.options.to!=0) return;
      effect.element.hide().setStyle({opacity: oldOpacity});
    }
  }, arguments[1] || { });
  return new Effect.Opacity(element,options);
};

Effect.Appear = function(element) {
  element = $(element);
  var options = Object.extend({
  from: (element.getStyle('display') == 'none' ? 0.0 : element.getOpacity() || 0.0),
  to:   1.0,
  // force Safari to render floated elements properly
  afterFinishInternal: function(effect) {
    effect.element.forceRerendering();
  },
  beforeSetup: function(effect) {
    effect.element.setOpacity(effect.options.from).show();
  }}, arguments[1] || { });
  return new Effect.Opacity(element,options);
};

Effect.Puff = function(element) {
  element = $(element);
  var oldStyle = {
    opacity: element.getInlineOpacity(),
    position: element.getStyle('position'),
    top:  element.style.top,
    left: element.style.left,
    width: element.style.width,
    height: element.style.height
  };
  return new Effect.Parallel(
   [ new Effect.Scale(element, 200,
      { sync: true, scaleFromCenter: true, scaleContent: true, restoreAfterFinish: true }),
     new Effect.Opacity(element, { sync: true, to: 0.0 } ) ],
     Object.extend({ duration: 1.0,
      beforeSetupInternal: function(effect) {
        Position.absolutize(effect.effects[0].element);
      },
      afterFinishInternal: function(effect) {
         effect.effects[0].element.hide().setStyle(oldStyle); }
     }, arguments[1] || { })
   );
};

Effect.BlindUp = function(element) {
  element = $(element);
  element.makeClipping();
  return new Effect.Scale(element, 0,
    Object.extend({ scaleContent: false,
      scaleX: false,
      restoreAfterFinish: true,
      afterFinishInternal: function(effect) {
        effect.element.hide().undoClipping();
      }
    }, arguments[1] || { })
  );
};

Effect.BlindDown = function(element) {
  element = $(element);
  var elementDimensions = element.getDimensions();
  return new Effect.Scale(element, 100, Object.extend({
    scaleContent: false,
    scaleX: false,
    scaleFrom: 0,
    scaleMode: {originalHeight: elementDimensions.height, originalWidth: elementDimensions.width},
    restoreAfterFinish: true,
    afterSetup: function(effect) {
      effect.element.makeClipping().setStyle({height: '0px'}).show();
    },
    afterFinishInternal: function(effect) {
      effect.element.undoClipping();
    }
  }, arguments[1] || { }));
};

Effect.SwitchOff = function(element) {
  element = $(element);
  var oldOpacity = element.getInlineOpacity();
  return new Effect.Appear(element, Object.extend({
    duration: 0.4,
    from: 0,
    transition: Effect.Transitions.flicker,
    afterFinishInternal: function(effect) {
      new Effect.Scale(effect.element, 1, {
        duration: 0.3, scaleFromCenter: true,
        scaleX: false, scaleContent: false, restoreAfterFinish: true,
        beforeSetup: function(effect) {
          effect.element.makePositioned().makeClipping();
        },
        afterFinishInternal: function(effect) {
          effect.element.hide().undoClipping().undoPositioned().setStyle({opacity: oldOpacity});
        }
      });
    }
  }, arguments[1] || { }));
};

Effect.DropOut = function(element) {
  element = $(element);
  var oldStyle = {
    top: element.getStyle('top'),
    left: element.getStyle('left'),
    opacity: element.getInlineOpacity() };
  return new Effect.Parallel(
    [ new Effect.Move(element, {x: 0, y: 100, sync: true }),
      new Effect.Opacity(element, { sync: true, to: 0.0 }) ],
    Object.extend(
      { duration: 0.5,
        beforeSetup: function(effect) {
          effect.effects[0].element.makePositioned();
        },
        afterFinishInternal: function(effect) {
          effect.effects[0].element.hide().undoPositioned().setStyle(oldStyle);
        }
      }, arguments[1] || { }));
};

Effect.Shake = function(element) {
  element = $(element);
  var options = Object.extend({
    distance: 20,
    duration: 0.5
  }, arguments[1] || {});
  var distance = parseFloat(options.distance);
  var split = parseFloat(options.duration) / 10.0;
  var oldStyle = {
    top: element.getStyle('top'),
    left: element.getStyle('left') };
    return new Effect.Move(element,
      { x:  distance, y: 0, duration: split, afterFinishInternal: function(effect) {
    new Effect.Move(effect.element,
      { x: -distance*2, y: 0, duration: split*2,  afterFinishInternal: function(effect) {
    new Effect.Move(effect.element,
      { x:  distance*2, y: 0, duration: split*2,  afterFinishInternal: function(effect) {
    new Effect.Move(effect.element,
      { x: -distance*2, y: 0, duration: split*2,  afterFinishInternal: function(effect) {
    new Effect.Move(effect.element,
      { x:  distance*2, y: 0, duration: split*2,  afterFinishInternal: function(effect) {
    new Effect.Move(effect.element,
      { x: -distance, y: 0, duration: split, afterFinishInternal: function(effect) {
        effect.element.undoPositioned().setStyle(oldStyle);
  }}); }}); }}); }}); }}); }});
};

Effect.SlideDown = function(element) {
  element = $(element).cleanWhitespace();
  // SlideDown need to have the content of the element wrapped in a container element with fixed height!
  var oldInnerBottom = element.down().getStyle('bottom');
  var elementDimensions = element.getDimensions();
  return new Effect.Scale(element, 100, Object.extend({
    scaleContent: false,
    scaleX: false,
    scaleFrom: window.opera ? 0 : 1,
    scaleMode: {originalHeight: elementDimensions.height, originalWidth: elementDimensions.width},
    restoreAfterFinish: true,
    afterSetup: function(effect) {
      effect.element.makePositioned();
      effect.element.down().makePositioned();
      if (window.opera) effect.element.setStyle({top: ''});
      effect.element.makeClipping().setStyle({height: '0px'}).show();
    },
    afterUpdateInternal: function(effect) {
      effect.element.down().setStyle({bottom:
        (effect.dims[0] - effect.element.clientHeight) + 'px' });
    },
    afterFinishInternal: function(effect) {
      effect.element.undoClipping().undoPositioned();
      effect.element.down().undoPositioned().setStyle({bottom: oldInnerBottom}); }
    }, arguments[1] || { })
  );
};

Effect.SlideUp = function(element) {
  element = $(element).cleanWhitespace();
  var oldInnerBottom = element.down().getStyle('bottom');
  var elementDimensions = element.getDimensions();
  return new Effect.Scale(element, window.opera ? 0 : 1,
   Object.extend({ scaleContent: false,
    scaleX: false,
    scaleMode: 'box',
    scaleFrom: 100,
    scaleMode: {originalHeight: elementDimensions.height, originalWidth: elementDimensions.width},
    restoreAfterFinish: true,
    afterSetup: function(effect) {
      effect.element.makePositioned();
      effect.element.down().makePositioned();
      if (window.opera) effect.element.setStyle({top: ''});
      effect.element.makeClipping().show();
    },
    afterUpdateInternal: function(effect) {
      effect.element.down().setStyle({bottom:
        (effect.dims[0] - effect.element.clientHeight) + 'px' });
    },
    afterFinishInternal: function(effect) {
      effect.element.hide().undoClipping().undoPositioned();
      effect.element.down().undoPositioned().setStyle({bottom: oldInnerBottom});
    }
   }, arguments[1] || { })
  );
};

// Bug in opera makes the TD containing this element expand for a instance after finish
Effect.Squish = function(element) {
  return new Effect.Scale(element, window.opera ? 1 : 0, {
    restoreAfterFinish: true,
    beforeSetup: function(effect) {
      effect.element.makeClipping();
    },
    afterFinishInternal: function(effect) {
      effect.element.hide().undoClipping();
    }
  });
};

Effect.Grow = function(element) {
  element = $(element);
  var options = Object.extend({
    direction: 'center',
    moveTransition: Effect.Transitions.sinoidal,
    scaleTransition: Effect.Transitions.sinoidal,
    opacityTransition: Effect.Transitions.full
  }, arguments[1] || { });
  var oldStyle = {
    top: element.style.top,
    left: element.style.left,
    height: element.style.height,
    width: element.style.width,
    opacity: element.getInlineOpacity() };

  var dims = element.getDimensions();
  var initialMoveX, initialMoveY;
  var moveX, moveY;

  switch (options.direction) {
    case 'top-left':
      initialMoveX = initialMoveY = moveX = moveY = 0;
      break;
    case 'top-right':
      initialMoveX = dims.width;
      initialMoveY = moveY = 0;
      moveX = -dims.width;
      break;
    case 'bottom-left':
      initialMoveX = moveX = 0;
      initialMoveY = dims.height;
      moveY = -dims.height;
      break;
    case 'bottom-right':
      initialMoveX = dims.width;
      initialMoveY = dims.height;
      moveX = -dims.width;
      moveY = -dims.height;
      break;
    case 'center':
      initialMoveX = dims.width / 2;
      initialMoveY = dims.height / 2;
      moveX = -dims.width / 2;
      moveY = -dims.height / 2;
      break;
  }

  return new Effect.Move(element, {
    x: initialMoveX,
    y: initialMoveY,
    duration: 0.01,
    beforeSetup: function(effect) {
      effect.element.hide().makeClipping().makePositioned();
    },
    afterFinishInternal: function(effect) {
      new Effect.Parallel(
        [ new Effect.Opacity(effect.element, { sync: true, to: 1.0, from: 0.0, transition: options.opacityTransition }),
          new Effect.Move(effect.element, { x: moveX, y: moveY, sync: true, transition: options.moveTransition }),
          new Effect.Scale(effect.element, 100, {
            scaleMode: { originalHeight: dims.height, originalWidth: dims.width },
            sync: true, scaleFrom: window.opera ? 1 : 0, transition: options.scaleTransition, restoreAfterFinish: true})
        ], Object.extend({
             beforeSetup: function(effect) {
               effect.effects[0].element.setStyle({height: '0px'}).show();
             },
             afterFinishInternal: function(effect) {
               effect.effects[0].element.undoClipping().undoPositioned().setStyle(oldStyle);
             }
           }, options)
      );
    }
  });
};

Effect.Shrink = function(element) {
  element = $(element);
  var options = Object.extend({
    direction: 'center',
    moveTransition: Effect.Transitions.sinoidal,
    scaleTransition: Effect.Transitions.sinoidal,
    opacityTransition: Effect.Transitions.none
  }, arguments[1] || { });
  var oldStyle = {
    top: element.style.top,
    left: element.style.left,
    height: element.style.height,
    width: element.style.width,
    opacity: element.getInlineOpacity() };

  var dims = element.getDimensions();
  var moveX, moveY;

  switch (options.direction) {
    case 'top-left':
      moveX = moveY = 0;
      break;
    case 'top-right':
      moveX = dims.width;
      moveY = 0;
      break;
    case 'bottom-left':
      moveX = 0;
      moveY = dims.height;
      break;
    case 'bottom-right':
      moveX = dims.width;
      moveY = dims.height;
      break;
    case 'center':
      moveX = dims.width / 2;
      moveY = dims.height / 2;
      break;
  }

  return new Effect.Parallel(
    [ new Effect.Opacity(element, { sync: true, to: 0.0, from: 1.0, transition: options.opacityTransition }),
      new Effect.Scale(element, window.opera ? 1 : 0, { sync: true, transition: options.scaleTransition, restoreAfterFinish: true}),
      new Effect.Move(element, { x: moveX, y: moveY, sync: true, transition: options.moveTransition })
    ], Object.extend({
         beforeStartInternal: function(effect) {
           effect.effects[0].element.makePositioned().makeClipping();
         },
         afterFinishInternal: function(effect) {
           effect.effects[0].element.hide().undoClipping().undoPositioned().setStyle(oldStyle); }
       }, options)
  );
};

Effect.Pulsate = function(element) {
  element = $(element);
  var options    = arguments[1] || { },
    oldOpacity = element.getInlineOpacity(),
    transition = options.transition || Effect.Transitions.linear,
    reverser   = function(pos){
      return 1 - transition((-Math.cos((pos*(options.pulses||5)*2)*Math.PI)/2) + .5);
    };

  return new Effect.Opacity(element,
    Object.extend(Object.extend({  duration: 2.0, from: 0,
      afterFinishInternal: function(effect) { effect.element.setStyle({opacity: oldOpacity}); }
    }, options), {transition: reverser}));
};

Effect.Fold = function(element) {
  element = $(element);
  var oldStyle = {
    top: element.style.top,
    left: element.style.left,
    width: element.style.width,
    height: element.style.height };
  element.makeClipping();
  return new Effect.Scale(element, 5, Object.extend({
    scaleContent: false,
    scaleX: false,
    afterFinishInternal: function(effect) {
    new Effect.Scale(element, 1, {
      scaleContent: false,
      scaleY: false,
      afterFinishInternal: function(effect) {
        effect.element.hide().undoClipping().setStyle(oldStyle);
      } });
  }}, arguments[1] || { }));
};

Effect.Morph = Class.create(Effect.Base, {
  initialize: function(element) {
    this.element = $(element);
    if (!this.element) throw(Effect._elementDoesNotExistError);
    var options = Object.extend({
      style: { }
    }, arguments[1] || { });

    if (!Object.isString(options.style)) this.style = $H(options.style);
    else {
      if (options.style.include(':'))
        this.style = options.style.parseStyle();
      else {
        this.element.addClassName(options.style);
        this.style = $H(this.element.getStyles());
        this.element.removeClassName(options.style);
        var css = this.element.getStyles();
        this.style = this.style.reject(function(style) {
          return style.value == css[style.key];
        });
        options.afterFinishInternal = function(effect) {
          effect.element.addClassName(effect.options.style);
          effect.transforms.each(function(transform) {
            effect.element.style[transform.style] = '';
          });
        };
      }
    }
    this.start(options);
  },

  setup: function(){
    function parseColor(color){
      if (!color || ['rgba(0, 0, 0, 0)','transparent'].include(color)) color = '#ffffff';
      color = color.parseColor();
      return $R(0,2).map(function(i){
        return parseInt( color.slice(i*2+1,i*2+3), 16 );
      });
    }
    this.transforms = this.style.map(function(pair){
      var property = pair[0], value = pair[1], unit = null;

      if (value.parseColor('#zzzzzz') != '#zzzzzz') {
        value = value.parseColor();
        unit  = 'color';
      } else if (property == 'opacity') {
        value = parseFloat(value);
        if (Prototype.Browser.IE && (!this.element.currentStyle.hasLayout))
          this.element.setStyle({zoom: 1});
      } else if (Element.CSS_LENGTH.test(value)) {
          var components = value.match(/^([\+\-]?[0-9\.]+)(.*)$/);
          value = parseFloat(components[1]);
          unit = (components.length == 3) ? components[2] : null;
      }

      var originalValue = this.element.getStyle(property);
      return {
        style: property.camelize(),
        originalValue: unit=='color' ? parseColor(originalValue) : parseFloat(originalValue || 0),
        targetValue: unit=='color' ? parseColor(value) : value,
        unit: unit
      };
    }.bind(this)).reject(function(transform){
      return (
        (transform.originalValue == transform.targetValue) ||
        (
          transform.unit != 'color' &&
          (isNaN(transform.originalValue) || isNaN(transform.targetValue))
        )
      );
    });
  },
  update: function(position) {
    var style = { }, transform, i = this.transforms.length;
    while(i--)
      style[(transform = this.transforms[i]).style] =
        transform.unit=='color' ? '#'+
          (Math.round(transform.originalValue[0]+
            (transform.targetValue[0]-transform.originalValue[0])*position)).toColorPart() +
          (Math.round(transform.originalValue[1]+
            (transform.targetValue[1]-transform.originalValue[1])*position)).toColorPart() +
          (Math.round(transform.originalValue[2]+
            (transform.targetValue[2]-transform.originalValue[2])*position)).toColorPart() :
        (transform.originalValue +
          (transform.targetValue - transform.originalValue) * position).toFixed(3) +
            (transform.unit === null ? '' : transform.unit);
    this.element.setStyle(style, true);
  }
});

Effect.Transform = Class.create({
  initialize: function(tracks){
    this.tracks  = [];
    this.options = arguments[1] || { };
    this.addTracks(tracks);
  },
  addTracks: function(tracks){
    tracks.each(function(track){
      track = $H(track);
      var data = track.values().first();
      this.tracks.push($H({
        ids:     track.keys().first(),
        effect:  Effect.Morph,
        options: { style: data }
      }));
    }.bind(this));
    return this;
  },
  play: function(){
    return new Effect.Parallel(
      this.tracks.map(function(track){
        var ids = track.get('ids'), effect = track.get('effect'), options = track.get('options');
        var elements = [$(ids) || $$(ids)].flatten();
        return elements.map(function(e){ return new effect(e, Object.extend({ sync:true }, options)) });
      }).flatten(),
      this.options
    );
  }
});

Element.CSS_PROPERTIES = $w(
  'backgroundColor backgroundPosition borderBottomColor borderBottomStyle ' +
  'borderBottomWidth borderLeftColor borderLeftStyle borderLeftWidth ' +
  'borderRightColor borderRightStyle borderRightWidth borderSpacing ' +
  'borderTopColor borderTopStyle borderTopWidth bottom clip color ' +
  'fontSize fontWeight height left letterSpacing lineHeight ' +
  'marginBottom marginLeft marginRight marginTop markerOffset maxHeight '+
  'maxWidth minHeight minWidth opacity outlineColor outlineOffset ' +
  'outlineWidth paddingBottom paddingLeft paddingRight paddingTop ' +
  'right textIndent top width wordSpacing zIndex');

Element.CSS_LENGTH = /^(([\+\-]?[0-9\.]+)(em|ex|px|in|cm|mm|pt|pc|\%))|0$/;

String.__parseStyleElement = document.createElement('div');
String.prototype.parseStyle = function(){
  var style, styleRules = $H();
  if (Prototype.Browser.WebKit)
    style = new Element('div',{style:this}).style;
  else {
    String.__parseStyleElement.innerHTML = '<div style="' + this + '"></div>';
    style = String.__parseStyleElement.childNodes[0].style;
  }

  Element.CSS_PROPERTIES.each(function(property){
    if (style[property]) styleRules.set(property, style[property]);
  });

  if (Prototype.Browser.IE && this.include('opacity'))
    styleRules.set('opacity', this.match(/opacity:\s*((?:0|1)?(?:\.\d*)?)/)[1]);

  return styleRules;
};

if (document.defaultView && document.defaultView.getComputedStyle) {
  Element.getStyles = function(element) {
    var css = document.defaultView.getComputedStyle($(element), null);
    return Element.CSS_PROPERTIES.inject({ }, function(styles, property) {
      styles[property] = css[property];
      return styles;
    });
  };
} else {
  Element.getStyles = function(element) {
    element = $(element);
    var css = element.currentStyle, styles;
    styles = Element.CSS_PROPERTIES.inject({ }, function(results, property) {
      results[property] = css[property];
      return results;
    });
    if (!styles.opacity) styles.opacity = element.getOpacity();
    return styles;
  };
}

Effect.Methods = {
  morph: function(element, style) {
    element = $(element);
    new Effect.Morph(element, Object.extend({ style: style }, arguments[2] || { }));
    return element;
  },
  visualEffect: function(element, effect, options) {
    element = $(element);
    var s = effect.dasherize().camelize(), klass = s.charAt(0).toUpperCase() + s.substring(1);
    new Effect[klass](element, options);
    return element;
  },
  highlight: function(element, options) {
    element = $(element);
    new Effect.Highlight(element, options);
    return element;
  }
};

$w('fade appear grow shrink fold blindUp blindDown slideUp slideDown '+
  'pulsate shake puff squish switchOff dropOut').each(
  function(effect) {
    Effect.Methods[effect] = function(element, options){
      element = $(element);
      Effect[effect.charAt(0).toUpperCase() + effect.substring(1)](element, options);
      return element;
    };
  }
);

$w('getInlineOpacity forceRerendering setContentZoom collectTextNodes collectTextNodesIgnoreClass getStyles').each(
  function(f) { Effect.Methods[f] = Element[f]; }
);

Element.addMethods(Effect.Methods);
/*
 * Registers a callback which copies the csrf token into the
 * X-CSRF-Token header with each ajax request.  Necessary to
 * work with rails applications which have fixed
 * CVE-2011-0447
*/


Ajax.Responders.register({
  onCreate: function(request) {
    var csrf_meta_tag = $$('meta[name=csrf-token]')[0];

    if (csrf_meta_tag) {
      var header = 'X-CSRF-Token',
      token = csrf_meta_tag.readAttribute('content');

      if (!request.options.requestHeaders) {
        request.options.requestHeaders = {};
      }
      request.options.requestHeaders[header] = token;
    }
  }
});

InlineLogin = {
  
  // ===== DOCUMENT ASSET METHODS ==================================================================
  
  /* Returns the container element used to load the inline login flow. */
  container: function() {
    if(this.current_action !== null) {
      return (this.current_action.container || $('inline_login'));
    }
  },
  
  // ===== LOGIN LIFECYCLE METHODS =================================================================
  // Methods & callbacks invoked at various points within the login process.
  
  /**
   * Convenience method that calls login_an_do, and reloads the current page when the user has
   * logged in or signed up.
   */
  login_and_reload: function(options) {
    this.login_and_do(null, function() { window.location.reload(); }, options);
    return false;
  },
  
  login_and_redirect: function(options) {
    this.login_and_do(null,
                      function() { this.login_redirect_url(); }.bind(this),
                      options);
    return false;
  },
  
  login_redirect_url: function() {
    var self = this;
    
    new Ajax.Request('/login_redirect', {
      asynchronous: true,
      method: 'get',
      onSuccess: function(request) {
        if (request.responseJSON) {
          window.location.href = self.get_redirect_location(request.responseJSON.login_trackback);
        }
      },
      onFailure: function(request) {
        window.location.href =  self.get_redirect_location(null);
      }
    });
  },
  
  get_redirect_location: function(redirect_url) {
    if (redirect_url === undefined || redirect_url.blank()) {
      // no login_trackback cookie set, so try to determine a redirect URL from query parameters, 
      // etc
      var params = window.location.href.toQueryParams();
      if (params.redirect && !params.redirect.blank()) {
        // use the explicit return url
        redirect_url = params.redirect;
      } else if (page.company) {
        // redirect to the current company
        redirect_url = "/"+ page.company;
      } else if (params.company && !params.company.blank()) {
        // redirect to the specified company
        redirect_url = "/" + params.company;
      } else if (current_user) {
        // redirect to the user's dashboard
        redirect_url = '/me';
      } else {
        // default to the home page
        redirect_url = "/";
      }
    }
    
    return redirect_url;
  },

  /**
   * Like #login_and_do, but it presents the user the registration screen by default. This is used
   * by all 'Sign up' links.
   */
  sign_up_and_do: function(position_anchor, callback, options) {
    options = options || {};
    options.params = options.params || {};
    options.params.login_mode = 'signup';
    
    this.login_and_do(position_anchor, callback, options);
  },
  
  /*
   * Initiates the inline login flow, performing the given callback function when login has
   * completed.
   *
   * `position_anchor` is an element to align the login with. (deprecated)
   * `callback` is a Function to call once the user has logged in.
   * `options` is an object with any of the following properties:
   *    
   *    `cancel_login`:  A function invoked when the user cancels the login.
   *    `fail`:          A function invoked when an Ajax request fails.
   *    `params`:        Parameter hash provided to the Ajax request.
   *    `prompt`:        A customized login prompt.
   *    `force_login`:   Ask the user to log in even if they are already.
   *                     Prevents infinite redirect for logged_in fastpass users.
   */
  login_and_do: function(position_anchor, callback, options) {
    if (options === null)  { options = {}; }
    if (page.logged_in() && ! options.force_login) {
      // already logged in, so just execute the callback
      if (callback !== null) { callback(); }
    } else {
      this.current_action = {
        position_anchor:  $(position_anchor),
        container:        options.container, 
        options:          options,
        
        onLoad:           (options.onLoad || function() {}),
        
        callback:         (callback || function() {}),
        fail:             (options.fail || function() {}),
        cancel_login:     (options.cancel_login || function() {}), 
        params:           (options.params || {}),
        
        login_prompt:     options.prompt
      };
      if (page.company) { this.current_action.params.company = page.company; }
      
      this.embed();
    }
  },
  
  /**
   * Makes a call to /login to determine the appropriate login mode and to get a copy of the login
   * panels. It then embeds the panels and starts the appropriate login based on the server's 
   * response.
   */
  embed: function() {
    var self = this
    new Ajax.Request("/logins/new", {
      asynchronous: true,
      method:       'get',
      parameters:   this.current_action.params,
      evalScripts:  false,
      onSuccess:    function(response) {
        result = response.responseText.evalJSON();
        self.load(result);
      },
      onFailure:    function() { self.request_failed(); }
    });

    this.checkForLogin = setInterval(function() {
      if (page.logged_in()) {
        self.complete();
      };
    }, 250);
  },
  
  unembed: function() {
    window.clearInterval(this.checkForLogin);
    this.container().hide();
    this.container().update('');
  },
  
  /*
   * Called when the initial Ajax request fails.
   */
  request_failed: function() {
    if (this.current_action) {
      this.current_action.fail();
      this.current_action = null;
    }
  },
  
  /*
   * Called when the user wants to cancel the current login attempt.
   */
  cancel: function() {
    this.unembed();
    
    if (this.current_action != null) {
      this.current_action.cancel_login();
      this.current_action = null;
    }
  },
  
  /*
   * Called when the user has been successfully logged in.
   */
  complete: function() {
    if(this.container() != null)  {        
    this.unembed();
    page.on_login(); // notify page elements of login
    if (this.current_action) {
      this.current_action.callback();
      this.current_action = null;
    }
    }
  },
  
  load: function(result) {
    this.container().update(result.login_panels);
    
    var self = this
    page.async(function() {
      self.init();
      if (self.current_action) {
        self.current_action.onLoad(result.login_mode);
      }
    });
    
    switch (result.login_mode) {
      case 'fastpass':
        this.start_fastpass_registration();
        break;
      case 'link_fastpass_via_login_token':
        this.start_link_fastpass_via_login_token(result.login_token_hash);
        break;
      case 'signover':
        this.start_signover();
        break;
      case 'signup':
        this.start_registration();
        break;
      default:
        this.start_login();
    }
  },
  
  init: function() {
    // introspect all init_XXX methods and invoke them
    for (property in this) {
      if (property.startsWith("init_") && (typeof this[property] == 'function')) {
        this[property].bind(this)();
      }
    }
  },
  
  // ===== ACCOUNT LOGIN ===========================================================================
  // Methods for handling logging into an existing account, or via a third-party provider.
  
  init_login: function() {
    var form = this.login_form();
    if (form) {
      form.observe('submit', this.submit_login_form.bind(this));
    }
  },
  
  start_login: function() {
    this.show_only('#sign_in_pane');
    
    if (typeof(this.login_form()) !== "undefined") {
      var email_field = this.login_form().down('#email');
      email_field.setAttribute("autocorrect", "off");
      email_field.setAttribute("autocapitalize", "off");
      email_field.focus();
    }
  },
  
  login_form: function() {
    return this.container().down('#login_form');
  },
  
  /*
   * Submits the login form, and completes the login if successful.
   */
  submit_login_form: function(ev) {
    if (ev != null) { Event.stop(ev); }
    if (page.validate_form(this.login_form())) {
      this.login_form().request({
        method: $(this.login_form()).readAttribute('method'),
        evalScripts:true,
        headers: {Accept: 'text/javascript'},
        onSuccess: this.complete.bind(this)
      })
    }
  },
  
  /* Displays the closed account pane when the user's accunt has previously been closed. */
  show_closed_account: function() {
    this.show_only('#closed_account');
  },
  
  /*
   * Called when the user clicks on an identity provider button.
   */
  provider_tab_clicked: function(tab_elem) {
    var tab = $(tab_elem);
    var match = tab.id.match(/^([^_]\w+)+_provider_tab$/);
    if (match != null) {
      var provider_name = match[1];
      this.select_provider(provider_name);
    }
  },
  
  /*
   * Selects an identity provider to login with, displaying its login panel.
   */
  select_provider: function(provider_name) {
    this.select_element_in_set(provider_name + '_provider_tab', '.provider_tab', 'on')
    this.select_element_in_set(provider_name + '_login_panel', '.provider_login_panel')
  },
  
  // ===== THIRD PARTY LOGIN ===========================================================================================
  
  init_third_party_login: function() {
    var self = this;
    // bind submit listeners to all login forms.
    this.container().select('.provider_login_panel').each(function(panel) {
      if (panel.id != ('getsatisfaction_login_panel') && panel.id != ('sso_login_panel')) {
        var submit = panel.down('form .submit');
        if (submit != null) {
          submit.observe('click', self.start_third_party_login.bind(self, panel));
        } else {
          console.log("Expected panel to have a .submit descendent!");
          console.log(panel);
        }
      }
    })
    
    this.receive_form().observe('submit', this.submit_receive_form.bind(this));
    
    // disable password entry by default, but provide a checkbox to allow them to click if they have
    // an account already. Displayed when logging in with a new identity that doesn't have a 
    // checkbox
    this.disable_identity_password_field();
    this.disable_identity_reset_password_fields();
    this.disable_login_token_fields();
    
    this.existing_account_checkbox().observe('click', 
                                             this.existing_account_checkbox_clicked.bind(this));
  },
  
  submit_receive_form: function(ev) {
    if (ev != null) { Event.stop(ev); }
    
    var receive_form = this.receive_form();
    if (page.validate_form(receive_form)) {
      receive_form.request({
        headers: {Accept: 'text/javascript'},
        onComplete: function(response) {
          var result = response.responseText.evalJSON();
          if (result == null || result.status == null) { 
            console.log("Unexpected response from server:");
            console.log(response);
            /*!<sl:translate>*/
            InlineLogin.show_message("The server returned an unexpected response. Please try again, or contact us if the error continues.");
            /*!</sl:translate>*/
          } else {
            InlineLogin.receive_identity_from_server(result);
          }
        }
      })
    }
  },
  
  receive_form: function() {
    return this.container().down("#receive_identity_pane form#receive_form");
  },
  
  start_third_party_login: function(panel, ev) {
    if (ev != null) { Event.stop(ev); }
    var login_form = panel.down('form');
    if (page.validate_form(login_form)) {
      var panel_type = panel.id.gsub(/_login_panel$/, '');
      var login_window_name = panel_type + "_login_window";
      login_form.writeAttribute('target', login_window_name);
      
      // open a new window...
      this.display_login_window("", login_window_name);
      
      // submit the form to the new window
      login_form.submit();
      
      //
      var panel_tab_id = "#" + panel_type + "_provider_tab";
      var provider_tab_name = this.container().down(panel_tab_id + " .tab_name");
      if (provider_tab_name) {
        var provider_name = provider_tab_name.innerHTML || ''
        this.container().select('#waiting_for_identity_pane .provider_name').each(function(e) {
          e.innerHTML = provider_name;
        });
      } else {
        console.log("Failed to find tab_name within " + panel_tab_id);
      }
      
      this.show_only('#waiting_for_identity_pane');
    }
  },
  
  
  // ----- RECEIVE/ACCEPT IDENTITY CALLBACKS -------------------------------------------------------
  // These methods are called via RJS or the popup window after processing a third party identity.
  // An 'accepted' identity is one that was successfully bound to a (possibly new) user account. A
  // 'received' identity is one that could not be bound to an account, due to a conflict or missing
  // profile data.
  
  /**
   * Proxy method that accepts a JSON response object from a receive/accept attempt and routes it to
   * `identity_received` or `identity_accepted`. This method will be called by the inline login
   * popup window after the user has successfully authenticated themselves. It's also called by
   * `submit_receive_form` after the completion of the request.
   */
  receive_identity_from_server: function(result) {
    if (result.status == 'accepted') {
      this.identity_accepted(result.identity);
    } else {
      this.identity_received(result);
    }
  },
  
  /**
   * Callback invoked when an identity has been received, but could not be accepted.
   */
  identity_received: function(result) {
    var status    = result.status;
    var message   = result.message;
    var token     = result.token;
    var identity  = result.identity;
    
    this.show_only('#receive_identity_pane');
    
    // set the hidden token field
    this.update_elements_with_value("#receive_identity_pane input.auth_info_token", token);
    
    // Focus the email field
    var email_field = this.receive_form_email_field();
    email_field.setAttribute("autocorrect", "off");
    email_field.setAttribute("autocapitalize", "off");
    
    // update the real name
    var real_name =
      (identity.display_name || identity.formatted_name || identity.preferred_username || '');
    if (real_name.blank()) {
      this.enable_identity_name_input();
      this.receive_form_name_field().value = result.nick || real_name;
    } else {
      this.disable_identity_name_input();
    }
    
    this.update_elements_with_value('#receive_identity_pane .name', real_name);
    
    // if the identity didn't come with a preferred_username or a formatted_name, show the name
    // field
    
    // update the avatar photo
    var photo_url = identity.photo_url || 'https://dwxmyiyf7jg6.cloudfront.net/assets/user_default_medium-cad5999aab9f22bef18bd28b5a9bc0eb.png'
    this.container().select('#receive_identity_pane .image').each(function(photo_elem) {
      if (photo_elem.name == 'img') {
        photo_elem.src = photo_url;
      } else {
        photo_elem.innerHTML =
          "<img src='" + photo_url.escapeHTML() + "' " +
                "alt='" + real_name + "' " + 
                "width='55' height='55' " +
                "/>";
      }
    });
    
    if (status == 'passwordless_unverified_account') {
      // The email matches an unverified account, but a password doesn't exist so we must
      // ask the user to confirm their email.
      var email = (result.email || identity.verified_email)
      email_field.value = email;
      
      this.prompt_for_login_token_code(email, result.login_token_hash);
    } else if (status == 'invalid_login_token_code') {
      // the user entered a login token, but it failed
      this.show_message(/*!<sl:translate>*/"The login code is incorrect. Please try again."/*!</sl:translate>*/);
      // TODO: show a link allowing a new login token to be sent
    } else if (status == 'unverified_account') {
      // The verified email matches an unverified account. A password exists, so ask them to confirm
      // their email.
      email_field.value = identity.verified_email;
      this.prompt_for_existing_account_password();
    } else if (status == 'unverified_email') {
      // the email from this identity hasn' been verified. Ask them to confirm it...
      email_field.focus();
    } else if (status == 'invalid_email') {
      this.show_identity_response_message(result, /*!<sl:translate>*/"Email is invalid"/*!</sl:translate>*/);
      email_field.focus();
    } else if (status == 'email_required') {
      this.show_identity_response_message(result, /*!<sl:translate>*/"Email is required"/*!</sl:translate>*/);
      email_field.focus();
    } else if (status == 'invalid_password') {
      this.show_identity_response_message(result, /*!<sl:translate>*/"Invalid password"/*!</sl:translate>*/);
      this.receive_form().down('#identity_password').focus();
    } else if (status == 'password_required') {
      // The provided (unverified) email is associated with an account, and a password is required
      // to bind the identity.
      email_field.value = result.email;
      this.prompt_for_existing_account_password();
    } else if (status == 'request_from_banned_ip') {
      this.show_message(result.message.escapeHTML());
    } else {
      if (result.message != null) {
        this.show_message(result.message.escapeHTML());
      } else {
        this.show_message(/*!<sl:translate>*/"An unknown error occurred"/*!</sl:translate>*/);
      }
      
      console.log("Unknown response status: ");
      console.log(status);
    }
  },
  
  show_identity_response_message: function(result, default_msg) {
    if (result.message != null && !result.message.blank()) {
      this.show_message(result.message.escapeHTML());
    } else if (default_msg != null) {
      this.show_message(default_msg);
    }
  },
  
  /*
   * Invoked when the user clicks the 'Forgot password?' link after an identity is received.
   */
  forgot_identity_password: function() {
    var email = this.receive_form().down('.email input.text').value
    this.request_reset_password(email, function(token) {
      // hide password fields
      this.disable_identity_email_input();
      this.hide_existing_account_selector();
      this.disable_identity_password_field();
      this.enable_identity_reset_password_fields(token);
      /*!<sl:translate>*/
      this.show_message("We've sent a verification code to {0}. Check your email and enter the code below, along with your new password.".format(email.escapeHTML()));
      /*!</sl:translate>*/
    }.bind(this))
  },
  
  enable_identity_reset_password_fields: function(token) {
    this.enable_form_row(this.receive_form().down('.reset_password'), true);
    this.enable_form_row(this.receive_form().down('.reset_password_confirmation'), true);
    this.enable_form_row(this.receive_form().down('.reset_password_token'), true);
    
    $('reset_password_token_hash').value = token
  },
  
  disable_identity_reset_password_fields: function(token) {
    this.disable_form_row(this.receive_form().down('.reset_password'));
    this.disable_form_row(this.receive_form().down('.reset_password_confirmation'));
    this.disable_form_row(this.receive_form().down('.reset_password_token'));
  },
  
  receive_form_name_field: function() {
    return this.receive_form().down('.real_name input');
  },
  
  disable_identity_name_input: function() {
    this.disable_field_input(this.receive_form_name_field());
    this.receive_form().down('.real_name').hide();
  },
  
  enable_identity_name_input: function() {
    this.enable_field_input(this.receive_form_name_field());
    this.receive_form().down('.real_name').show();
  },
  
  receive_form_email_field: function() {
    return this.receive_form().down('.email input');
  },
  
  disable_identity_email_input: function() {
    this.disable_field_input(this.receive_form_email_field());
  },
  
  enable_identity_email_input: function() {
    this.enable_field_input(this.receive_form_email_field());
  },
  
  /*
   * Returns the checkbox a user can click to provide a password for an email.
   */
  existing_account_checkbox: function() {
    return this.receive_form().down('#existing_account');
  },
  
  /*
   * Called when the user clicks the checkbox to enter a password for an email.
   */
  existing_account_checkbox_clicked: function(ev) {
    this.toggle_identity_password_field();
  },
  
  
  /* Hides the checkbox that allows the user to choose to login with an existing account. */
  hide_existing_account_selector: function() {
    var selector = this.receive_form().down('.selector');
    selector.hide();
  },
  
  prompt_for_existing_account_password: function() {
    /*!<sl:translate>*/                                      
    this.show_message("This email is in use with an existing account. Please login with the password for this account.");
    /*!</sl:translate>*/
    // show the password field
    this.hide_existing_account_selector();
    this.disable_identity_email_input();
    this.enable_identity_password_field();
    this.disable_login_token_fields();
    
    this.receive_form().down('#identity_password').focus();
  },
  
  /**
   * Determines if the password field is enabled, and toggles its state.
   */
  toggle_identity_password_field: function() {
    var password_row = this.receive_form().down('div.password');
    var password_input = password_row.down('input');
    if (password_input.disabled) {
      this.enable_identity_password_field();
    } else {
      this.disable_identity_password_field();
    }
  },
  
  /*
   * Displays the account password field, and marks it as required. This is done whenever the user
   * attempts to login with an existing email address.
   */
  enable_identity_password_field: function() {
    this.enable_form_row($('identity_password').up('.row'), true)
    $('existing_account').checked = true
  },
  
  /*
   * Hides and unrequires the password field within the receive_identity form. This is the default
   * state, and is invoked to reset the form, and when the user unchecks the existing account 
   * selector checkbox.
   */
  disable_identity_password_field: function() {
    this.disable_form_row($('identity_password').up('.row'));
    $('existing_account').checked = false
  },
  
  /*!<sl:translate>*/
  prompt_for_login_token_code: function(email, hash) {
    this.show_message("This email is in use with an existing account. We've sent an email to containing a one-time login token. Please check your email and enter the email in the field below.".format(email.escapeHTML()));
    /*!</sl:translate>*/
    
    // show the password field
    this.hide_existing_account_selector();
    this.disable_identity_email_input();
    this.disable_identity_password_field();
    this.disable_identity_reset_password_fields();
    
    this.enable_login_token_fields(hash);
    this.receive_form().down('#login_token_code').focus();
  },
  
  enable_login_token_fields: function(hash) {
    var token_hash_field = $('login_token_hash');
    token_hash_field.value = hash;
    token_hash_field.enable();
    
    this.enable_form_row($('login_token_code').up('.row'), true)
  },
  
  disable_login_token_fields: function() {
    var token_hash_field = $('login_token_hash');
    token_hash_field.disable();
    
    this.disable_form_row($('login_token_code').up('.row'))
  },
  
  /**
   * Opens an external login window. Currently used for third-party login, while 
   * display_signover_window() is used for the signover page.
   *
   * @param url The login URL.
   * @param window_name The name of the opened window.
   */
  display_login_window: function(url, window_name) {
    var width   = document.viewport.getWidth() * 0.9;
    var height  = document.viewport.getHeight() * 0.9;
    var left    = (screen.width / 2)  - (width / 2);
    var top     = (screen.height / 2) - (height / 2);
    
    login_window =
      window.open(url, window_name,
                  "menubar=1,resizable=1,scrollbars=yes," +
                  "width=" + width + 
                  ",height=" + height +
                  ",left=" + left +
                  ",top=" + top);
    login_window.focus();
  },
  
  /**
   * Callback invoked after an identity has been accepted.
   */
  identity_accepted: function(identity) {
    this.complete();
  },
  
  // ===== ACCOUNT CREATION ============================================================================================
  
  init_registration: function() {
    this.registration_form().observe('submit', this.submit_registration_form.bind(this));
    this.initialize_recaptcha();
  },
  
  /* Initializes the Recaptcha container. */
  initialize_recaptcha: function() {
    if (typeof window.GSFN.noCaptcha !== "undefined") {
      var noCaptchaCallback = function(response) {
//        $("recaptcha_response_field").setValue(response); // fill in the original recaptcha field to appease the CreateAccount model's validations
        $("g-recaptcha-response").setValue(response);
      };

      window.GSFN.noCaptcha.render($('nocaptcha'), {
        'sitekey' : window.GSFN.noCaptchaPublicKey,
        'callback' : noCaptchaCallback
      });
    } else {
      setTimeout(this.initialize_recaptcha, 100);
    }
  },
  
  start_registration: function() {
    this.show_only('#start_registration');
    $('user_email').setAttribute("autocorrect", "off");
    $('user_email').setAttribute("autocapitalize", "off");
    $('user_nick').focus();
  },
  
  submit_registration_form: function(ev) {
    if (ev != null) { Event.stop(ev); }
    if (page.validate_form(this.registration_form())) {
      this.registration_form().request({
        evalScripts:  true,
        headers:      {Accept: 'text/javascript'},
        onSuccess:    this.registration_succeeded.bind(this),
        on500:        this.registration_failed.bind(this)
      })
    }
  },
  
  registration_succeeded: function(response) {
    page.set_login_cookies(response.responseText); 
    page.on_login();
    var name_el = this.container().down('#registration_done').down(".name")
    name_el.update(current_user.name());
    
    var source = (this.registration_source() || 'inline');
    page.post_tracker("/tracking/signup/" + this.registration_source());
    page.adwords_conversion();
    this.show_only('#registration_done');
  },
  
  registration_failed: function(response) {
    this.show_message("<strong>"+/*!<sl:translate>*/"Sorry!"/*!</sl:translate>*/+"</strong><br />" + response.responseText)
    this.initialize_recaptcha();
    window.GSFN.noCaptcha.reset();
  },
  
  registration_form: function() {
    return $('start_registration');
  },
  
  registration_source: function() {
    return this.registration_form().action.toQueryParams().source
  },
  
  // ===== RESET PASSWORD ==============================================================================================
  
  /* Initializes the password reset markup after it's been loaded. */
  init_password_reset: function() {
    this.password_reset_form().observe('submit', this.submit_password_reset_form.bind(this));
  },
  
  /* Displays the password reset form. */
  start_password_reset: function() {
    this.show_only('#password_reset_form');
    $('email_for_reset').focus();
  },
  
  /* Returns the password reset form. */
  password_reset_form: function() {
    return this.container().down('#password_reset_form');
  },
  
  /* Submits the password_reset_form via an AJAX call. */
  submit_password_reset_form: function(ev) {
    if (ev != null) { Event.stop(ev); }
    if (page.validate_form(this.password_reset_form())) {
      this.password_reset_form().request({
        evalScripts:  true,
        parameters:   {inline: true},
        headers:      {Accept: 'text/javascript'},
        onSuccess:    this.password_reset_finished.bind(this),
        on404:        this.password_reset_failed.bind(this)
      })
    }
  },
  
  prompt_for_password_reset_token_code: function(email, hash) {
    /*!<sl:translate>*/                                     
    this.show_message("We've sent you an email with a special code to reset your password. Check your email and enter the code below, along with your new password.");
    /*!</sl:translate>*/
    
    this.disable_password_reset_email_input();
    this.enable_password_reset_token_fields(hash);
    this.enable_password_reset_password_fields();
    
    this.password_reset_form().down('#password_reset_token_code').focus();
  },
  
  disable_password_reset_email_input: function(hash) {
    
    var token_hash_field = this.password_reset_form().down('#password_reset_token_hash')
    token_hash_field.enable();
    token_hash_field.value = hash;
    
    this.enable_form_row($('password_reset_token_code').up('.row'), true);
  },
  
  enable_password_reset_token_fields: function(hash) {
    var token_hash_field = this.password_reset_form().down('#password_reset_token_hash')
    token_hash_field.enable();
    token_hash_field.value = hash;
    
    this.enable_form_row($('password_reset_token_code').up('.row'), true);
  },
  
  disable_password_reset_token_fields: function() {
    var token_hash_field = this.password_reset_form().down('#password_reset_token_hash')
    token_hash_field.disable();
    this.disable_form_row($('password_reset_token_code').up('.row'));
  },
  
  enable_password_reset_password_fields: function(hash) {
    this.enable_form_row($('password_reset_password').up('.row'), true)
    this.enable_form_row($('password_reset_password_confirmation').up('.row'), true)
  },
  
  disable_password_reset_password_fields: function(hash) {
    this.disable_form_row($('password_reset_password').up('.row'))
    this.disable_form_row($('password_reset_password_confirmation').up('.row'))
  },
  
  /* Callback invoked when a reset attempt succeeds. */
  password_reset_finished: function() {
    this.show_message("<strong>"+/*!<sl:translate>*/"Thanks!"/*!</sl:translate>*/+"</strong> " + /*!<sl:translate>*/"Check your email for a special link that will let you reset your password."/*!</sl:translate>*/);
    LoginAction.start_login();
  },
  
  /* Callback invoked when a reset attempt fails. */
  password_reset_failed: function() {
    this.show_message("<strong>"+/*!<sl:translate>*/"Sorry!"/*!</sl:translate>*/+"</strong> " + /*!<sl:translate>*/"that email isn't registered"/*!</sl:translate>*/)
  },
  
  request_reset_password: function(email, callback) {
    new Ajax.Request('/login/request_reset_password', {
      method: 'get',
      parameters: {email: email},
      onSuccess: function(response) {
        if (response.responseText.isJSON()) {
          var token = response.responseText.evalJSON()
          if (token == null) {
            InlineLogin.show_message(email.escapeHTML() + /*!<sl:translate>*/" isn't associated with any account"/*!</sl:translate>*/);
          } else {
            callback(response.responseText.evalJSON());
          }
        } else {
          InlineLogin.show_message(
            /*!<sl:translate>*/'Received an unknown response after asking to reset your password.'/*!</sl:translate>*/
          )
        }
      },
      onError: function() {
        InlineLogin.show_message(/*!<sl:translate>*/'An unknown error occurred while asking to reset your password.'/*!</sl:translate>*/)
      } 
    })
  },
  
  // ===== FASTPASS REGISTRATION =======================================================================================
  
  start_fastpass_registration: function() {
    this.show_only('#new_from_fastpass');
  },
  
  customize_fastpass: function(link) {
    var row = $(link).up(".row");
    row.down(".readonly").hide();
    row.down(".text").show().select();
    $(link).hide();
  },
  
  // ===== FASTPASS SIGNOVER ===========================================================================================
  // "Signover" is the process of asking the user to log into their account on another site. The
  // external site then handles the process of authenticating the user. Once they are done, the
  // external site forwards the user via FastPass.
  
  init_signover: function() {
    this.container().down('#signover_button').observe('click',
                                                      this.display_signover_window.bind(this));
  },
  
  start_signover: function() {
    this.show_only('#signover_form');
  },
  
  display_signover_window: function(ev) {
    if (ev != null) { Event.stop(ev); }
    
    var width   = 800;
    var height  = 600;
    var left    = (screen.width / 2)  - (width / 2);
    var top     = (screen.height / 2) - (height / 2);

    another_window =
      window.open(this.signover_url(), /*!<sl:translate>*/"Signover"/*!</sl:translate>*/, 
                  "menubar=1,resizable=1,scrollbars=yes," +
                  "width=" + width + 
                  ",height=" + height +
                  ",left=" + left +
                  ",top=" + top);
    another_window.focus();
    
    this.show_signover_wait();
    setTimeout(this.wait_for_login.bind(this), 100);
    return false;
  },
  
  signover_url: function() {
    return this.signover_button().href;
  },
  
  signover_button: function() {
    return this.container().down('#signover_button');
  },
  
  /* Shows a small panel while we wait for them to login via the popup window. */
  show_signover_wait: function() {
    this.show_only('#signover_wait');
  },
  
  // ----- INLINE RESET PASSWORD ---------------------------------------------------------------------------------------
  // Generic code for handling password resets inline, without opening another page. Used by 3rd party identities and
  // fastpass logins.
  
  forgot_inline_password: function(elem, callback) {
    var form = $(elem).up('form');
    var email_elem = form.down('.email')
    var value_elem = email_elem.down('input.text') || email_elem.down('.value')
    
    var email = this.get_value_from_element(value_elem)
    this.request_reset_password(email, function(token) {
      if (callback) callback(email, token);
      
      this.disable_inline_password_field(form);
      this.enable_inline_reset_password_fields(form, token);
      /*!<sl:translate>*/ 
      this.show_message("We've sent a verification code to {0}. Check your email and enter the code below, along with your new password.".format(email.escapeHTML()));
      /*!</sl:translate>*/

    }.bind(this))
  },
  
  disable_inline_password_field: function(form) {
    this.disable_form_row(form.down('input.password').up('.row'));
    //    $('existing_account').checked = false
  },
  
  enable_inline_reset_password_fields: function(form, token) {
    this.enable_form_row(form.down('.reset_password'), true);
    this.enable_form_row(form.down('.reset_password_confirmation'), true);
    this.enable_form_row(form.down('.reset_password_token'), true);
    
    $(form).down('input.reset_password_token_hash').value = token
  },
  
  disable_inline_reset_password_fields: function(form, token) {
    this.disable_form_row(form.down('.reset_password'));
    this.disable_form_row(form.down('.reset_password_confirmation'));
    this.disable_form_row(form.down('.reset_password_token'));
  },

  enable_inline_login_token_fields: function(form, token) {
    this.enable_form_row(form.down('.login_token'), true);

    $(form).down('input.login_token_hash').value = token
  },

  disable_inline_login_token_fields: function(form) {
    this.disable_form_row(form.down('.login_token'));
  },

  // ===== LINK FASTPASS VIA VERIFICATION TOKEN - WITHOUT PASSWORD RESET ===============================================

  init_link_fastpass_via_login_token: function() {
     this.link_fastpass_via_login_token_form()
         .observe('submit', this.submit_link_fastpass_via_login_token_form.bind(this));
  },

  start_link_fastpass_via_login_token: function(token_hash) {
    this.show_only('#link_fastpass_via_login_token');
    this.enable_inline_login_token_fields(this.link_fastpass_via_login_token_form(), token_hash)
  },

  link_fastpass_via_login_token_form: function() {
    return $('link_fastpass_via_login_token_form')
  },

  submit_link_fastpass_via_login_token_form: function(ev) {
    if (ev != null) { Event.stop(ev); }
    if (page.validate_form(this.link_fastpass_via_login_token_form())) {
      this.link_fastpass_via_login_token_form().request({
        evalScripts:false,
        headers: {Accept: 'text/javascript'},
        onSuccess: this.complete.bind(this),
        onFailure: function(response) {
          InlineLogin.show_message(response.responseText.evalJSON())
        }
      })
    }
  },
 
  // Callback: waits for the signover cookie to be removed and checks if login is required or login screen needs to be reloaded to validate the fastpass identity
  wait_for_login: function() {
    if (!Cookie.get('signover') && page.logged_in()) {
      this.complete();
    //signover needs password validation or confirmation because fastpass identity reports errors.
    } else if(!Cookie.get('signover') && Cookie.get('signovererror')) {
      Cookie.erase('signovererror');
      this.unembed();
      page.login_and_reload();
    } else {
      setTimeout(this.wait_for_login.bind(this), 100);
    }
  },
  
  // ===== MESSAGING ===================================================================================================
  
  /*
   * Displays a message to the user in the visible panel. Typically used to display results of a
   * request.
   */
  show_message: function(message) {
    var msg_box = this.message_box();
    if (msg_box != null) {
      Effect.Appear(msg_box, { duration: 0.5 });
      msg_box.update(message);
    }
  },
  
  /*
   * Hides the active message box.
   */
  hide_message: function() {
    var msg_box = this.message_box();
    if (msg_box) {
      Effect.Fade(msg_box, { duration: 0.5 });
    }
  },
  
  /*
   * Returns the currently
   */
  message_box: function() {
    var pane_el = this.active_pane();
    if (!pane_el) { return; }
    
    return pane_el.down("div.msg");
  },
  
  // ===== PANEL MANAGEMENT ========================================================================
  // The inline login is composed of a variety of panels for each action (logging in, creating an
  // account, generating a temporary password, etc.). These methods show and hid various panels.
  
  wide_panes: $A(['#registration_done', '#sign_in_pane', 
                  '#start_registration', '#closed_account']),
  
  thin_panes: $A(['#signover_form', '#signover_wait', '#new_from_fastpass', '#password_reset_form',
                  '#receive_identity_pane', "#waiting_for_identity_pane",
                  '#link_fastpass_via_login_token']),
  
  panes: function() {
    return this.wide_panes.concat(this.thin_panes);
  },
  
  /*
   * Returns the visible pane, or null if not found.
   */
  active_pane: function() {
    panes = this.container().select("*.pane");
    return $A(panes).detect(function(el) { return el.visible(); })
  },
  
  /*
   * Shows only a single panel, based on the given selector.
   */
  show_only: function(selector_to_show) {
    this.panes().each(function(selector) {
      var pane_el = this.container().down(selector);
      if (selector == selector_to_show) {
        pane_el.show();
      } else {
        pane_el.hide();
      }
    }.bind(this));
    
    if (this.thin_panes.include(selector_to_show)) {
      this.container().addClassName("thin");
    } else {
      this.container().removeClassName("thin");      
    }
  },
  
  // ===== DOM UTILITIES ===========================================================================
  
  /*
   * Adds the 'selected' class to the given element, making sure it is the only one within the
   * selected elements.
   */
  select_element_in_set: function(element, selector, class_name) {
    if (class_name == null) { class_name = 'selected' }
    this.container().select(selector).each(function(elem) {
      elem.removeClassName(class_name);
    })
    $(element).addClassName(class_name);
  },
  
  get_value_from_element: function(value_elem) {
    if (value_elem) {
      if (value_elem.tagName == 'input') {
        return value_elem.value
      } else {
        return value_elem.innerHTML.strip()
      }
    }
  },
  
  /*
   * Selects elements within the container element and updates their content to the given value. If
   * INPUT elements are selected, their value is updated, otherwise the content of the element is
   * replaced with the HTML-escaped value.
   */
  update_elements_with_value: function(selector, value) {
    this.container().select(selector).each(function(elem) {
      if (elem.tagName.toLowerCase() == 'input') {
        elem.value = value;
      } else {
        elem.update(value.escapeHTML());
      }
    });
  },
  
  /**
   * Enables all input elements within the given row (which can be an element or selector) and then
   * unhides the row. If required is `true`, the row is marked as a required. Used when a field is
   * only needed in certain contexts.
   */
  enable_form_row: function(row, required) {
    row = $(row);
    if (required == true) { row.addClassName('required') }
    row.select('input').each(function(input) {
      input.enable();
    });
    
    row.show();
  },
  
  /**
   * Disables all input elements within the given row (which can be an element or selector) and then
   * hides the row.
   */
  disable_form_row: function(row) {
    row = $(row);
    row.removeClassName('required');
    row.select('input').each(function(input) {
      input.disable();
    });
    row.hide();
  },
  
  /**
   * Enables input for the given form field, hiding any rendered version.
   */
  enable_field_input: function(form_field) {
    var row = form_field.up('.row');
    var display_field = row.down('.display')
    if (display_field != null) {
      form_field.show();
      
      display_field.down('.value').update('');
      display_field.hide();
    } else {
      form_field.enable();
    }
  },
  
  /**
   * Disables input for the given form field, and displays a rendered version of the value if 
   * possible. The field value is "rendered" by placing its value in a '.display span.value' 
   * element within the field's '.row' parent. Otherwise the field will simply be marked as 
   * disabled, and displayed as-is.
   */
  disable_field_input: function(form_field) {
    form_field = $(form_field)
    
    var row = form_field.up('.row');
    var display_field = row.down('.display')
    if (display_field != null) {
      form_field.hide();
      
      display_field.down('.value').update(form_field.value.escapeHTML());
      display_field.show();
    } else {
      form_field.disable();
    }
  }
  
};

LoginFunctions = {
  
  // ===================
  // = state functions =
  // ===================
  logged_in: function() {
    isInAdminContext = window.location.href.match(/admin\/login/) !== null;
    if (!isInAdminContext && Cookie.get('logged_in') && Cookie.get('token_issue_date')) {
      return true;
    } else if (isInAdminContext && Cookie.get('admin_logged_in')) {
      return true;
    } else {
      return false;
    }
  },
  // 
  // =========================
  // = Login/Logout Function =
  // =========================
  login: function(email, password) {
    this.form_login({'email':email, 'password':password});
  },
  
  form_login: function(params) {
    new Ajax.Request("/login.js", {
      method: 'post',
      asynchronous:true,
      parameters: params,
      evalScripts:true
    });
  },
  
  login_and_reload: function(options) {
    this.login_and_do(null, function() { window.location.reload(); }, options);
    return false;
  },
  
  login_and_redirect: function(options) {
    this.login_and_do(null,
                      function() { this.login_redirect_url(); }.bind(this),
                      options);
    return false;
  },
  
  login_redirect_url: InlineLogin.login_redirect_url.bind(InlineLogin),
  
  login_and_do: function(position_anchor, callback, options) {
    options = options || {}
    var onLoad = options.onLoad;
    options.onLoad = function(login_mode) {
      $('overlay').show();
      var inline_login = $('inline_login');
      inline_login.hide();
      page.center(inline_login);
      inline_login.show();
      
      if (onLoad) { onLoad(login_mode) }
    }
    
    var onCancel = options.cancel_login;
    options.cancel_login = function() {
      $('overlay').hide();
      $('inline_login').hide();
      var the_content = $('inline_login').down('div.content');
      if (the_content) { the_content.update('') }
      
      if (onCancel) { onCancel() }
    }
    
    options.params = options.params || {}
    options.params.inline = 'true'
    
    InlineLogin.login_and_do(position_anchor, function() {
      $('inline_login').fade({
        delay: 0.4,
        duration: 0.4,
        afterFinish: function() {
          $('overlay').hide();
          if (callback) { callback() }
        }
      });
    }, options);
  },
  
  sign_up_and_do: function(position_anchor, callback, options) {
    options = options || {}
    var onLoad = options.onLoad;
    options.onLoad = function(login_mode) {
      $('overlay').show();
      var inline_login = $('inline_login');
      inline_login.hide();
      page.center(inline_login);
      inline_login.show();
      
      if (onLoad) { onLoad(login_mode) }
    }

    var onCancel = options.cancel_login;
    options.cancel_login = function() {
      $('overlay').hide();
      if (onCancel) { onCancel() }
    }
    
    options.params = options.params || {}
    options.params.inline = 'true'
    
    InlineLogin.sign_up_and_do(position_anchor, function() {
      $('inline_login').fade({
        delay: 0.4,
        duration: 0.4,
        afterFinish: function() {
          $('overlay').hide();
          if (callback) { callback() }
        }
      });
    }, options);
  },

  sign_up_and_reload: function(options) {
    this.sign_up_and_do(null, function() { window.location.reload(); }, options);
    return false;
  },
  
  // ====================
  // = Cookie Functions =
  // ====================
  
  set_login_cookies: function(text) {
    json = eval('(' + text + ')');
    $H(json).each(function(keyvalue) {
      Cookie.set(keyvalue[0], keyvalue[1].value, keyvalue[1].days_to_expire);
    });
  },
  
  // =====================
  // = Event Dispatchers =
  // =====================
  on_login: function() {
    page.notify('login');
  }
};
// Error Report
error_occurred = function(msg, url, line, data) {
  return false;
}

log_exception = function(e) {
  error_occurred(e.message, e.fileName, e.lineNumber, e.stack)
  
  try{ console.error(e) } catch(f){}
};

protect = function(fun) {
  return function() {
    try {
      fun();
    } catch(e) {log_exception(e)}
  };
};


Notifiers = {
	notify: function(operation, options) {		
		this.ensure_notifiers(operation);
		if(!options) { options = $H({}); }
		
		this.notifiers[operation].each(function(callback) { 
			try {
        callback(options);
      } catch(e) {log_exception(e)}
      
		});	
	},
	
	watch: function(operation, callback) {
		this.ensure_notifiers(operation);
		this.notifiers[operation].push(callback);
		return callback;
	},
	
	unwatch:function(operation, callback) {
		this.ensure_notifiers(operation);
		this.notifiers[operation] = this.notifiers[operation].reject(function(cb){ return cb == callback;});
		return callback;
	},
	
	ensure_notifiers: function() {
		if(!this.notifiers){ this.notifiers = $H(); }
		$A(arguments).each(function(e) { 
			if(!this.notifiers[e]){this.notifiers[e] = $A();}
		}.bind(this));
		
	}
}
;
  save_note = function(form) {
    form.down('.spinner').show();
    new Ajax.Request(form.action, {
      parameters: form.serialize(true),
      onSuccess: function(response)  { form.down('.btn').value = 'note saved'; },
      onFailure: function(response)  { alert("Didn't save! Try again."); },
      onComplete: function(response) { form.down('.spinner').hide(); }
    });
  };
showTopicCheck = function() {
  $('topic_label').update('Paste in the full URL of the topic you\'ll merge this topic into:');
  $('selected_topic_result').hide();
  $('topic').show();
  $('topic_example').show();
  $('change_authoritative_topic_id').setValue(null);
}

showTopicSearch = function() {
  $('search_label').update('Search for the topic you\'ll merge this topic into:');
  $('selected_result').hide();
  $('search').show();
  $('change_authoritative_topic_id').setValue(null);
}

showInlineTopic = function(link, title, details, topic_url, user_name, emotion) {
  $('inline_topic').down('a.title').update(title).setAttribute('href', topic_url);
  $('inline_topic').down('.details').update(details);
  $('inline_topic').down('.name').update('Created by '+user_name);
  $('inline_topic').dockTo(link, {target_point: 'bl', offset_type: 'positionedOffset'})
  $('inline_topic').autoHide();
}

document.observe("dom:loaded", function() {
  var query_field = $('raw_query');

  if (query_field) {
    query_field.observe("focus", function(){
      $$('.search_results').invoke("show");
    });
  }
});
PromptedField = Class.create();
PromptedField.prepare_submit = function(form) {
	form.select('.prompted').each( function(el) {
		if (el.onfocus) { el.onfocus() };
		if(el.prompter) {
			el.prompter.enabled = false;
			el.prompter.prepare_submit();
		}
  });
}

PromptedField.prototype = {
  initialize: function(element, prompt, dont_focus) {
    this.element = $(element);
    this.element.addClassName('prompted');
		this.enabled = true;
		if(this.element.prompter){ this.element.prompter.destroy(); }
		this.element.prompter = this;
    this.element.onfocus = null;

    this.focuser = this.focus.bind(this);
    Event.observe(this.element, 'focus', this.focuser);
    this.blurer = this.blur.bind(this);
    Event.observe(this.element, 'blur', this.blurer);
    
    this.prompt = prompt;
    if(!dont_focus) this.focus();
  },
  
  set_prompt: function() {
    field = $F(this.element);
    return field == this.prompt || field == "";
  }, 
  
  focus: function() {
    if(this.set_prompt() && this.enabled) {
      this.element.clear();
    }
    this.element.addClassName('focus');
  },
  
  blur: function() {
    if(this.set_prompt() && this.enabled) {
      this.element.value = this.prompt;
    }

    this.element.removeClassName('focus');
  },

	prepare_submit: function() {
		if(this.set_prompt()) {
			this.element.clear();
		}
	},
	
	destroy: function() {
	  Event.stopObserving(this.element, 'focus', this.focuser);
	  Event.stopObserving(this.element, 'blur', this.blurer);
	  this.element = null
	},
	
	transition_to: function(new_prompt) {
	  if(this.element.value == this.prompt || this.element.value == "") {
	    this.element.value = new_prompt;
	  } else {
	    this.element.focus()
	  }
	  
	  this.prompt = new_prompt;
	}
};

// ==============
// = new school =
// ==============

setupPromptedField = function(prompted_label) {
  var f = prompted_label.getAttribute("for") || prompted_label.getAttribute('htmlFor');

  if(!f){ return; }

  var input = $(f)

  input.observe("focus", function(){
    prompted_label.setStyle({ display: "none" })
  });

  input.observe("blur", function() {
    if ($F(input) == '') prompted_label.setStyle({ display: "block" })
  });

  prompted_label.observe("click", function() {
    input.focus();
  });

  if ($F(input) != '') prompted_label.hide();
}

removeFieldPrompt = function(field) {
 $$('label[for='+$(field).id+']').each(function(prompt) {
   prompt.setStyle({ textIndent: "-10000px" });
 });
}
;
EmotionPicker = Class.create();

Object.extend(EmotionPicker, {
  get_picker: function(link) {
    tagger = $(link);
    return this.ensure_for((tagger.identify() == 'emotitagger') ? tagger : $('emotitagger'));
  },
  
  find_in: function(el) {
    tagger = $(el);    
    return this.ensure_for((tagger.identify() == 'emotitagger') ? tagger : $('emotitagger'));
  },
  
  cancel_emotitagging: function(link) {
    this.get_picker(link).cancel_emotitagging();
  },
  
  face_click: function(link, face) {
    this.get_picker(link).face_click(face);
  },
  
  ensure_for: function(container) {
    container = $(container);
    return container.emotion_picker ? container.emotion_picker : new EmotionPicker(container);
  }
});

EmotionPicker.prototype = {
  initialize: function(container) {
    this.container = $(container);
    this.container.emotion_picker = this;
    
    this.face_field = this.container.down(".emotitagger_face");
    this.feeling_field = this.container.down(".emotitagger_feeling");
    this.face_list = $$('.picker_list')[0];
    this.face_links = this.face_list.select("a");
    this.feeling_area = this.container.down(".feeling_entry");
    this.example_feelings = this.container.select(".example_feelings");
    
    this.previous_face = null;
  },
  
  // ========================================
  // = API functions, you should use these! =
  // ========================================
  
  face_click: function(face) {
    console.log(face);
    this.set_values(face);
    return false;
  },
  
  set_feeling: function(face, feeling) {
    this.face_field.value = face;
    this.feeling_field.value = feeling;
    jQuery('#topic_emotitag_feeling').change();
  },
  
  start_emotitagging: function(face) {
    this.update_picker_class(face);
    this.example_feelings.each(function(f) { f.hasClassName(face) ? f.show() : f.hide() });
    if (!this.feeling_area.visible()) { this.feeling_area.show(); }
  },

  cancel_emotitagging: function() {
    this.update_picker_class(null);
    this.face_links.invoke('removeClassName', 'on');
    if (this.feeling_area.visible()) { 
      this.feeling_area.hide();
      $A([this.face_field, this.feeling_field]).invoke('clear');
      jQuery('#topic_emotitag_face').change();
      jQuery('#topic_emotitag_feeling').change();
    }
    this.example_feelings.invoke('hide');
    this.previous_feeling = null;
    this.previous_face = null;
    this.container.emotion_picker = null;
  },
  
  // ===========================================
  // = My Privates are below, don't touch them =
  // ===========================================
  
  set_values: function(face) {
    this.set_face(face);
  },
  
  set_face: function(face) {
    if (!face) { face = ''; }
    this.update_picker_class(face);
    this.face_field.value = face;
    this.previous_face = face;
    jQuery('#topic_emotitag_face').change();
    this.face_links.each(function(el) {
      el.removeClassName("on");
    });
    if(face.blank()) {
      if (this.feeling_area.visible()) { this.feeling_area.hide(); }
    } else {
      link = this.face_list.down('a.' + face);
      link.addClassName("on")
      this.example_feelings.each(function(f) { f.hasClassName(face) ? f.show() : f.hide() });
      if (!this.feeling_area.visible()) { this.feeling_area.show(); }
    }
  },
  
  
  update_picker_class: function(new_class) {
    // out with the old
    if(this.face_field.value != "") this.face_list.removeClassName("picker_"+this.face_field.value);
    // in with the new
    if(new_class != null) this.face_list.addClassName("picker_"+new_class);
  },
  
  selected_face: function() {
    var face = this.face_field.getValue();
    return face == "" ? null : this.container.down("." + face);
  }
};
var Cookie = {
  // mockery hooks
  assignDocumentCookie : function(cookie_string){ document.cookie = cookie_string },
  getDocumentCookie : function(){ return document.cookie; },
  getWindowLocationHostname : function(){ return window.location.hostname },
  getNavigatorCookieEnabled : function(){return navigator.cookieEnabled},
  
  set: function(name, value, daysToExpire) {
    var expire = '';
    if (daysToExpire != undefined) {
      var d = new Date();
      d.setTime(d.getTime() + (86400000 * parseFloat(daysToExpire)));
      expire = '; expires=' + d.toGMTString();
    }
    var cookieString = escape(name) + '=' + escape(value || '') +
            expire +
            "; domain=" + escape(this.domain()) +
            "; path=/";
    this.assignDocumentCookie(cookieString);
    return cookieString;
  },

  domain: function() {
    var hostname = this.getWindowLocationHostname();
    var components = hostname.split('.');
    if (components.length >= 2) {
      components = components.reverse();
      return '.' + components[1] + '.' + components[0];
    } else {
      return hostname;
    }
  },

  get: function(name) {
    var cookie = this.getDocumentCookie().match(new RegExp('(^|;)\\s*' + escape(name) + '=([^;\\s]*)'));
    return (cookie ? unescape(cookie[2]) : null);
  },

  erase: function(name) {
    var cookie = this.get(name) || true;
    this.set(name, '', -1);
    return cookie;
  },

  accept: function() {
    if (typeof this.getNavigatorCookieEnabled() == 'boolean') {
      return this.getNavigatorCookieEnabled();
    }
    this.set('_test', '1');
    return (this.erase('_test') === '1');
  },

  acceptsCookiesFromStrangers: function() {
    return Cookie.get('cross_domain_feedback_enabled') != null
  }
};
Element.addMethods({
  
  autoHide: function(element, hide_element, callback) {
    var hide_element = hide_element === undefined ? element : hide_element;
    var cb = callback === undefined ? null : callback;
    document.getElementsByTagName('body')[0].onmousedown = hide_element.do_on_click_outside.bind({element: element, to_hide: hide_element, callback: cb});
  },
  
  do_on_click_outside: function(options, event) {
    var target = ie ? window.event.srcElement : event.target;
    element = options.element;
    if($(target).outside(element)) {
      to_hide = options.to_hide || element;
      document.onmouseup = null; document.onmousedown = null;
      to_hide.hide();
      if(options.callback != null) options.callback();
    }
  },
  
  dockTo: function(element, target, options) {
    client_width = document.documentElement.clientWidth;
    target_pos = options.offset_type == undefined ? $(target).fixedCumulativeOffset() : $(target)[options.offset_type]();
    target_dim = $(target).getDimensions();
    element_dim = element.getDimensions();
    options.offset = options.offset == undefined ? [0,0] : options.offset;
    element.target = target;
    options.target_point = options.target_point == undefined ? 'bl' : options.target_point
    
    if(options.target_point == 'tl') {
      new_top = target_pos.top + options.offset[0];
      new_left = target_pos.left + options.offset[1];
    } else if(options.target_point == 'tr') {
      new_top = target_pos.top + options.offset[0];
      new_left = target_pos.left+target_dim.width + options.offset[1];
    } else if(options.target_point == 'bl') {
      new_top = target_pos.top+target_dim.height + options.offset[0];
      new_left = target_pos.left + options.offset[1];
    } else if(options.target_point == 'br') {
      new_top = target_pos.top+target_dim.height + options.offset[0];
      new_left = target_pos.left+target_dim.width + options.offset[1];
    }
    
    if(new_left+element_dim.width<client_width) {
      element.removeClassName('pos_left');
      element.addClassName('pos_right');
      client_left = new_left;
    } else { // flip horizontally
      element.removeClassName('pos_right');
      element.addClassName('pos_left');
      if (options.target_point == 'tl' || options.target_point == 'bl') {
        client_left = (new_left+target_dim.width)-element_dim.width;
      } else {
        client_left = new_left-element_dim.width-target_dim.width;
      }
    }
    element.setStyle({left: client_left+'px', top: new_top+'px'})
    if(options.should_show || options.should_show == null) { 
      element.show();
      options.should_show = true;
    }

  },
  
  dockedTo: function(docked_element) {
    return $(docked_element).target;
  },
  
  undock: function(docked_element, should_hide) {
    if(should_hide == null){ should_hide = true}
    $(docked_element).target = null;
    
    if(should_hide){ $(docked_element).hide(); }
  },
  
  fixedCumulativeOffset: function(element) {
    // Fix parentNode borderWidth / clientLeft/Top bug [#188]
    // http://prototype.lighthouseapp.com/projects/8886/tickets/188
    offset = element.cumulativeOffset();
    borderLeftWidth = parseInt(element.up().getStyle("borderLeftWidth")) || 0;
    borderTopWidth = parseInt(element.up().getStyle("borderTopWidth")) || 0;
    valueL = parseInt(offset.left+borderLeftWidth);
    valueT = parseInt(offset.top+borderTopWidth);
    return Element._returnOffset(valueL, valueT);
  },
  
  outside: function(element, scope) {
    element = $(element).firstChild != null ? $(element).firstChild : $(element);
    scope = $(scope);
    return !Element.childOf(element, scope);
  },
  
  onmouseinside: function(element, callback) {
    element = $(element);
    element.inside_callback = callback;
    element.onmouseover = Element.handle_inside_outside.bind(this, element);
    element.onmouseout = Element.handle_inside_outside.bind(this, element);
  },
  
  onmouseoutside: function(element, callback) {
    element = $(element);
    element.outside_callback = callback;
    element.onmouseover = Element.handle_inside_outside.bind(this, element);
    element.onmouseout = Element.handle_inside_outside.bind(this, element);
  },
  
  handle_inside_outside: function(el, event) {
    if (!event) event = window.event;
    // trace(event);
    el = $(el);
    
    switch(event.type) {
      case 'mouseout':
        var related = (typeof event.relatedTarget != 'undefined' ? event.relatedTarget : event.toElement); 
      
       // trace(event.target + ", " + el)
        if(el.outside_callback && $(related).outside(el)) {
          el.outside_callback(el);
          el._outside_callback = el.outside_callback;
          el.outside_callback = null;
        }
        if(el._inside_callback) {
          el.inside_callback = el._inside_callback;
          el._inside_callback = null;
        }
      break;
      case 'mouseover':
        var related = (typeof event.relatedTarget != 'undefined' ? event.relatedTarget : event.fromElement); 
      
    	  // trace(event.target + ", " + el)
        if(el.inside_callback && $(related).outside(el)) {
          el.inside_callback(el);
          el._inside_callback = el.inside_callback;
          el.inside_callback = null;
        }
        if(el._outside_callback) {
          el.outside_callback = el._outside_callback;
          el._outside_callback = null;
        }
      break;
      default: trace("bad juju");
    }
  },
  
  tabToggle: function(element, selector, klassName) {
    $(element).up().select(selector).invoke('removeClassName', klassName);
    $(element).addClassName(klassName);
  },
  
  swap: function(element, swap_element) {
    $(element).hide();
    $(swap_element).show();
  }
  
});
Object.extend(Object, {
	reverse_merge: function(destination, defaults) {
		return Object.extend(Object.clone(defaults), destination);
	}
});

Array.prototype.sum = function() {
  return (! this.length) ? 0 : this.slice(1).sum() +
      ((typeof this[0] == 'number') ? this[0] : 0);
};


Number.prototype.commify = function () {
  var numStr = this.toString();
  var num = numStr.split('');

  // simplify by only accepting integers longer than 3 digits
  if ( numStr.match(/\D/) || num.length < 3 ) return numStr;
  num.reverse();
  numStr = num.join('');

  numStr = numStr.replace(/(\d\d\d)/g, "$1_marker_");
  // if we did one too many, take it back
  numStr = numStr.replace(/_marker_$/g, '');
  // replace the thousandth _marker_ with ',' or '.'
  numStr = numStr.replace(/_marker_/g, ",");

  num = numStr.split('');
  num.reverse();
  return num.join('');
}
;
EnumerableExt = {
  async_each: function(iterator) {
   var index = 0;
    this._each(function(value) {
        setTimeout(protect(iterator.bind(this, value, index++)), 10);
    }.bind(this));
    return this;
  }
};

Object.extend(Enumerable, EnumerableExt);
Object.extend(Array.prototype, EnumerableExt);
// Object.extend(Hash.prototype, EnumerableExt);
// Object.extend(ObjectRange.prototype, EnumerableExt);
// Object.extend(Ajax.Responders, EnumerableExt);
// Object.extend(Element.ClassNames.prototype, EnumerableExt);
Object.extend(String.prototype, {
  pluralize: function(num) {
    if (num == '1') {
      return this;
    } else {
      if (this.endsWith('y')) {
        plural = this.slice(0, this.lastIndexOf('y')) + 'ies';
      } else {
        if (this.endsWith('s')) {
          plural = this;
        } else {
          plural = this + 's';
        }
      }
      return plural;
    }
  },

  blank: function() {
    return /^\s*$/.test(this);
  },

  startsWith: function(pattern) {
		if(this.length < pattern.length){ return false; }
    return this.indexOf(pattern) == 0;
  },

  endsWith: function(pattern) {
		if(this.length < pattern.length){ return false; }
    return this.lastIndexOf(pattern) == (this.length - pattern.length);
  },
  
  times: function(count) {
    var result = '';
    for (var i = 0; i < count; i++) result += this;
    return result;
  },
  possessify: function() {
    if (this.endsWith('y')) {
      plural = this + "'s";
    } else {
      if (this.endsWith('s')) {
        plural = this + "'";
      } else {
        plural = this + "'s";
      }
    }
    return plural;
  }
});
Object.extend(Selector, {
  findElements: function(elements, expression) {
    if (typeof expression == 'number') index = expression, expression = false;
    return Selector.matchElements(elements, expression || '*');
  }
});
Object.extend(Form.Element.Methods, {
  getSafeValue: function(element) {
  	try {
  		return this.getValue(element);
  	} catch(e) {
  		return '';
  	}
  }
});

Object.extend(Form.Element, Form.Element.Methods);
Element.addMethods();

Form.Element.DelayedEventObserver = Class.create(Form.Element.EventObserver, {
  
  initialize: function($super, element, frequency, callback) {
    $super(element, callback);
    this.frequency = frequency;
  },
  
  onElementEvent: function() {
    var value = this.getValue();
    if (this.lastValue != value) {
      if (this.timeout){ window.clearTimeout(this.timeout); }
        
      this.timeout = window.setTimeout(this.fire.bind(this), this.frequency * 1000);
    }
  },
  
  fire: function() {
    var value = this.getValue();
    this.callback(this.element, value);
    this.lastValue = value;
  },
  
  registerCallback: function(element) {
    if (element.type) {
      switch (element.type.toLowerCase()) {
        case 'checkbox':
        case 'radio':
          Event.observe(element, 'click', this.onElementEvent.bind(this));
          break;
        case 'text':
        case 'password':
        case 'textarea':
          Event.observe(element, 'keyup', this.onElementEvent.bind(this));
          break;
        default:
          Event.observe(element, 'change', this.onElementEvent.bind(this));
          break;
      }
    }
  }
});
Object.extend(Element.ClassNames.prototype, {
  clear: function() {
    this.each(function(klass) { 
      this.remove(klass);
    }.bind(this));
  }
});

// following code is MIT licensed (C) Gary Haran 2007

/**
* Provide the same behavior as window.scrollTo to divs with overflow without removing
* the ability to scroll a page to a given element.
*/

Element.addMethods({
  scrollTo: function(element, left, top){
    var element = $(element);
    if (arguments.length == 1){
      var pos = element.cumulativeOffset();
      window.scrollTo(pos[0], pos[1]);
    } else {
      element.scrollLeft = left;
      element.scrollTop  = top;
    }
    return element;
  }
});

/**
* Effect.Scroll allows you to animate scrolling on a page (or div w/ overflow: scroll || auto)
*/
Effect.Scroll = Class.create();
Object.extend(Object.extend(Effect.Scroll.prototype, Effect.Base.prototype), {
  initialize: function(element) {
    this.element = $(element);
    if(!this.element) throw(Effect._elementDoesNotExistError);
    this.start(Object.extend({x: 0, y: 0}, arguments[1] || {}));
  },
  setup: function() {
    var scrollOffsets = (this.element == window)
                ? document.viewport.getScrollOffsets()
                : {left: this.element.scrollLeft, top: this.element.scrollTop};
    this.originalScrollLeft = scrollOffsets.left;
    this.originalScrollTop  = scrollOffsets.top;
  },
  update: function(pos) {
    this.element.scrollTo(Math.round(this.options.x * pos + this.originalScrollLeft), Math.round(this.options.y * pos + this.originalScrollTop));
  }
});
Page = Class.create();
Page.prototype = {};
Object.extend(Page.prototype, LoginFunctions);
Object.extend(Page.prototype, Notifiers);

Object.extend(Page.prototype, {
  initialize: function() {
    this.scroll_interval = null;
    this.msg_el;
  },
  
  dismiss_feature_msg: function(value, container) {
    var feature_cookie = Cookie.get('feature_msg');
    var get_msgs = feature_cookie == null ? [] : feature_cookie.split("|");
    get_msgs.push(value);
    set_msgs = get_msgs.join("|");
    Cookie.set('feature_msg', set_msgs, 100000);
    
    if(!container) {
      container = 'feature_msg';
    }
    
    new Effect.BlindUp(container, {duration: 0.4});
  },
  
  scrollTop: function() {
    return window.pageYOffset
                || document.documentElement.scrollTop
                || document.body.scrollTop
                || 0;
  },
  
  show_mini_modal: function() {
    var ov = $('overlay');
    $('mini_modal').style.top = this.scrollTop()+"px"
    if(!ov.visible()) {
      $('mini_modal').show();
      $('mini_modal_content').update("");
      $('mini_modal_loader').show();
      $('overlay_screen').show();
      ov.style.height = document.documentElement.scrollHeight-70+"px";
      ov.show();
    }
  },
  
  populate_mini_modal: function(content) {
    $("mini_modal_content").update(content);
    $('mini_modal_loader').hide();
  },
  
  hide_overlay: function() {
    var ov = $('overlay');
    ov.immediateDescendants().invoke("hide");
    ov.hide();
  },
  
  show_overlay: function() {
    $('overlay').show();
  },
  
  center_overlay: function() {
    el = $('overlay').immediateDescendants().last();
    page.center(el);
  },

  init_msg_el: function() {
    this.msg_el = $$("div.main_msg").first(); 
    if(!this.msg_el) {
      var $notification = $j(
        '<div id="page_notification" class="translate main_msg clearfix">'
          + '<a href="#" class="cancel_btn">x</a>'
          + '<div class="top"></div>'
          + '<div class="content clearfix"></div>'
        + '</div>'
      ).hide().insertBefore('#page > #container');

      $notification.find('.cancel_btn').click(function() { $notification.hide(); return false; })

      this.msg_el = $$("div.main_msg").first();
    }
    this.msg_el.removeClassName("good bad");
  },
  
  fixed_message: function(message, style) {
    this.init_msg_el();
    if(message) {
      if(style === 'good')
        this.msg_el.removeClassName('bad');
      if(style === 'bad')
        this.msg_el.removeClassName('good');

      this.msg_el.addClassName(style);
      this.show_notification(message);
    } else {
      this.hide_notification();
    }
  },

  show_notification: function(message) {
    Element.scrollTo(this.msg_el); 
    if (message) {  
      if(this.msg_el.down(".content")) { 
        this.msg_el.down(".content").update(message);  
      } else { 
        this.msg_el.update("<div class=\"content\">"+message+"</div>");  
      } 
    } 
    this.msg_el.show();
  },

  hide_notification: function(clear) {
    this.msg_el.hide();
  },

  auto_hide_this: function(element, hide_element, callback) {
    if (hide_element == "undefined") {
      hide_element = element;
    }
    $(element).autoHide(hide_element, callback);
  },
  
  center: function(element) {
    element_middle = ($(element).getDimensions().height / 2) * 1;
    viewport_middle =(document.viewport.getHeight() * 1) / 2;
    scroll_offset = document.viewport.getScrollOffsets()[1];
    
    top_px = scroll_offset + viewport_middle - element_middle;

    //prevent negative offset when the viewport is smaller than the element :-/
    top_px = Math.max(110, top_px);

    $(element).style.top = top_px + "px";
  },

  adjust_scroll_position: function(element, overshoot, callback) {
    element = $(element);
    scroll_top = page.scrollTop();
    overshoot = overshoot ? overshoot : 0;
    if (this.scroll_interval != null) clearInterval(this.scroll_interval);
    document_height = document.documentElement.clientHeight;
    screen_bottom = scroll_top+document_height;
    new_position = Position.cumulativeOffset(element)[1]+Element.getDimensions(element).height;
    
    end_scroll_position = new_position-document_height+overshoot;

    if(new_position > screen_bottom) {
      this.scroll_interval = setInterval(function() {
        if(scroll_top < end_scroll_position) {
          scroll_top += Math.ceil((end_scroll_position-page.scrollTop())/10)
          window.scrollTo(0, scroll_top);
        } else {
          if (callback != undefined) {
            callback()
          }
          clearInterval(this.scroll_interval);
        }
      }.bind(this), 10);
    }
  },
  
  safe_redirect:function(url) {
    window.location.href = url;
    return false;
  },
  
  validate_form: function(form) {
    var form = $(form);
    var required = form.select(".required");
    var one_required = form.select(".one_required");
    var msg = form.down(".msg");
    var spinner = $(form).down('.spinner');

    var fail = function(requirement_string) {
      if(!page.on_screen(form)) Element.scrollTo(form);
      if (msg) {
        msg.update(I18n.t('page.something_missing', {
          zing: '<strong>' + I18n.t('sfn.zing') + '</strong>',
          requirement: '<span>' + requirement_string.sub(':', '') + '</span>'
        }));
        if(!msg.visible()) Effect.Appear(msg, {duration: 0.5});
      }
      if(spinner) {
        page.async(function() { SpinnerButton.reset(spinner); });
      }
    };

    for(var r=0; r<required.length; r++) {
      var required_el = required[r].down("input") || required[r].down("textarea");
      if($F(required_el) == "") {
        required[r].addClassName("highlight");
        if (required[r].down("label")) {
          fail(required[r].down("label").innerHTML);
        }
        required_el.focus();
        return false; break;
      } else {
        required[r].removeClassName("highlight");
      }
    }

    for(var r=0; r<one_required.length; r++) {
      var passed = false;
      var sufficient_els = one_required[r].select("input, textarea");
      var requirement_name = one_required[r].down(".requirement_name").innerHTML;
      for(var s=0; s<sufficient_els.length; s++) {
        if ($F(sufficient_els[s]) != "") {
          passed = true;
          break;
        }
      }
      if (!passed) {
        fail(requirement_name);
        return false; break;
      }
    }

    msg.hide();
    return true;
  },
  
  focus_blank_input: function(form_el) {
    force_break = false;
    $(form_el).select('.text').each(function(el) {
      if($F(el) == "" && !force_break) {
        el.focus(); 
        force_break = true;
      }
    });
  },
  
  on_screen: function(el) {
    return (Position.cumulativeOffset(el)[1] > document.documentElement.scrollTop)
  },
  
  // ===================
  // = Company & Product History =
  // ===================
  
  history_json: function(c_history) {
    items = $A(c_history).map(function(history_item){
      return "[\"" + history_item[0] + "\",\"" + history_item[1] + "\"]"
    });
    return "[" + items.join(',') + "]"
  },
  
  get_history: function(cookie_id) {
    try {
      val = eval("(" + Cookie.get(cookie_id)+ ")");
    } catch(e) {
      val = [];
    }
    
    if(val) { return val; }
    else { return []; }
  },
  
  update_history: function(domain, label, cookie_id) {
    c_history = this.get_history(cookie_id);
    domain = domain.toString();
    set_value = [domain, label];
    if(c_history != null) {
      exists = false; index = null;
      for(i=0; i<c_history.length; i++) {
        if(c_history[i][0] == domain) {
          exists = true; index = i;
          break;
        }
      }
      if(!exists) {
        if(c_history.length == 5) c_history.pop();
        c_history.unshift(set_value);
      } else {
        c_history.splice(index, 1);
        c_history.unshift(set_value);
      }
    } else {
      c_history.unshift(set_value);
    }
    Cookie.set(cookie_id, this.history_json(c_history), 3560);
  },
  
  render_history: function() {
    // Render recently viewed companies and products
    this.update_history_html($('c_history_list'), this.get_history("company_history"), /*!<sl:translate>*/'companies'/*!</sl:translate>*/); // Companies
    this.update_history_html($('p_history_list'), this.get_history("product_history"), /*!<sl:translate>*/'products'/*!</sl:translate>*/); // Products
  },
  
  update_history_html: function(el, h, l) {
    if ($A(h).any()) {
      var string = /*!<sl:translate>*/"<dt>Recently viewed<br/>{0}</dt>".format(l)/*!</sl:translate>*/;
      for(i=0; i<h.length; i++) {
        if(i==0) {
          klass = " class=\"first\"";
        } else {
          klass = "";
        }
        string += "<dd"+klass+"><a href=\""+ h[i][0] + "\"><span>"+ h[i][1] +"</span></a></dd>";
      }
      el.update(string)
    } else {
      el.hide();
    }
  },

  
  // ===================
  // = Login Trackback =
  // ===================
  
  dont_set_login_trackback: function() {
    this.set_login_trackback = function() {}; //just unset the function
  },
  
  async:function(callback) {
    setTimeout(protect(callback), 10);
  },
  
  post_tracker: function(url) {
    try {
      var pageTracker = _gat._getTracker("UA-2005202-1");
      pageTracker._trackPageview(url);      
    } catch(e) { }
  },
  adwords_conversion: function() {
    target = $('adwords_target');
    if(target) {
      target.update(adwords_conversion_markup);
    }
  },
  
  searcher_setup: function(searcher, container_el, find_label, label) {
    $('header_topic_search').hide(); $('header_product_search').hide(); $('header_company_search').hide();
    var c = $(container_el).show(); 
    c.down('div.menu').hide();
    $('search_type_txt').value = find_label;
    var search_field = c.down('input');
    search_field.prompted_text = label;
    search_field.value = label;
    search_field.observe("keyup", function() {
      searcher.search($F(search_field), search_field, 'header')
    });
  },

  // ===================
  // == JS Templating Methods ==
  // ===================

  user_console: function() {
    if(page.logged_in()) {
      user_console_inside = '<a href="/me">'+ /*!<sl:translate>*/ 'Your Dashboard ' + /*!</sl:translate>*/ '&amp; '/*!<sl:translate>*/ + 'Account' /*!</sl:translate>*/ + '</a> | <a href="/logout">'+/*!<sl:translate>*/'Sign out'/*!</sl:translate>*/+'</a>';
    } else {
      user_console_inside = '<a href="/logins/new?company=' + page.company + '" onclick="return page.login_and_reload();">' +/*!<sl:translate>*/ 'Log in' + /*!</sl:translate>*/ '</a>'+/*!<sl:translate>*/' or ' + /*!</sl:translate>*/'<a href="/people/new?company=' + page.company + '" onclick="return page.login_and_reload();">' +/*!<sl:translate>*/'Sign up'/*!</sl:translate>*/+'</a>';
    }
    user_console_html = '<span id="gsfn_user_console">' + user_console_inside + '</span>';
    document.write(user_console_html);
  }
});

Object.extend(Page, {
  // ===================
  // == Tagging Stuff ==
  // ===================
  
  split_tags: function(list) {
    tag_names = [];
    // first, pull out the quoted tags
    list = list.gsub(/\"(.*?)\"\s*/, function(m) { tag_names.push(m[1]); });
    // then, get whatever's left
    all_tags = tag_names.concat(list.split(/,/));
    // strip whitespace from the names
    clean_tags = all_tags.invoke('strip');
    // reject empty tags
    return clean_tags.reject(function(name) { return name == ''; });
  }
});

page = new Page();
Searcher = Class.create();
Object.extend(Searcher, {
  search:function(query, input_field, search_type) {
    this.input_field = input_field;
    this.query = query;
    if(this.input_field.up('.combo')) { this.combo = this.input_field.up('.combo').combo; }
    if(query.blank()) {
      if(this.search_timeout){ clearTimeout(this.search_timeout)};
      this.search_form(input_field).delayed_query = null;
      this.results(input_field).hide();
      this.searching_indicator(input_field).hide();
    } else if(this.search_timeout && query == this.search_form(input_field).delayed_query) {
      return;
    } else {
      if(this.search_timeout){ clearTimeout(this.search_timeout)};
      this.search_form(input_field).delayed_query = query
      this.searching_indicator(input_field).show();
      
      this.search_timeout = setTimeout(function(){
        new Ajax.Request(this.search_url, {
          asynchronous:true, 
          method:'get',
          parameters:{'query': query, 'search_type': search_type},
          evalScripts:true,
          onComplete: function(){
            this.searching_indicator(input_field).hide();
          }.bind(this),
          onSuccess: function(request) {
            if(this.search_form(input_field).delayed_query != query) {return;}
            this.results(input_field).update(request.responseText);
            this.results(input_field).show();
            if(this.combo) {this.combo.formatResults(this.results(input_field));}
          }.bind(this),
          on404: function(request) {
            this.result_404();
            this.results(input_field).show();
          }.bind(this)
        });
      }.bind(this),1000)
    }
  },
  
  result_404: function() {
    if(this.search_type == 'companies') {
      this.results(this.input_field).update("<dl class=\"no_results\"><dt>We couldn't find &quot;"+this.query.stripTags()+"&quot;</dt><dd><a href=\"/companies/new?name="+this.query.stripTags()+"\" class=\"on\"><span>Add it to Get Satisfaction!</span></a></dd></dl>");
    } else if(this.search_type == 'products') {
      this.results(this.input_field).update("<dl class=\"no_results\"><dt>No products found matching: \""+this.query.stripTags()+"\"</dt><dl>");
    }
    this.results(this.input_field).show();
  },
  
  activate_search: function(input_field) {
    page.auto_hide_this(this.search_form(input_field), this.results(input_field));
  },
  
  prepare_search: function(input_field) {
    $(input_field).setAttribute("autocomplete", "off");
  },
  
  search_form: function(input_field) {
    return $(input_field).up('form');
  },
  
  results: function(input_field) {
    return this.search_form(input_field).down('.search_results');
  },
  
  searching_indicator: function(input_field) {
    return this.search_form(input_field).down('.searching');
  }
});


Products = Object.clone(Searcher);
Object.extend(Products, {
  search_type: 'products',
  search_url:  '/products/search'
});


Companies = Object.clone(Searcher);
Object.extend(Companies, {
  search_type: 'companies',
  search_url:  '/companies/search'
});
MeToo = Class.create();
MeToo.prototype = {
  initialize: function(element) {
    this.element = $(element);
    this.overlay = this.element.down('.overlay');
    this.me_too_link = this.element.down('.me_too_link');
    this.timer; // create the timeout container
    this.element.onmouseover = null; // remove initializer
    this.element.onmouseinside(this.show_overlay.bind(this)); // tasty non-bubbling mouseover
    this.element.onmouseoutside(this.hide_overlay.bind(this)); // tasty non-bubbling mouseout
    
    if(this.me_too_link) {
      this.me_too_link.observe("click", this.click.bind(this));
    }
    
    this.show_overlay();
  },

  show_overlay: function() {
    this.timer = setTimeout(function() {
      this.overlay.show();
      this.overlay.setOpacity(0);
      this.overlay.addClassName('active');

      // this.overlay.morph('active', {duration: 0.3, transition: Effect.Transitions.EaseTo});
      this.overlay.appear({duration: 0.3});
    }.bind(this), 100);
  },

  hide_overlay: function() {
    clearTimeout(this.timer)
    
    this.overlay.fade({duration: 0.5, afterFinish: this.reset.bind(this)});
  },
  
  reset: function() {
    this.overlay.removeClassName('hidden');
    this.overlay.removeClassName('active');
    this.overlay.hide();
  },
  
  click: function(event) {
    event.stop();
    this.element.addClassName("active");
    page.login_and_do(this.me_too_link, function() {
      new Ajax.Request(this.me_too_link.href, {
        asynchronous:true, 
        method: "post",
        evalScripts:true
      })
    }.bind(this), { prompt: "continue"});
  }
};

Links = {
  add_new_link: function(link, link_type) {
    var container = $(link).hasClassName('links_container') ? $(link) : $(link).up('.links_container');
    var label = link_type
    new Insertion.Bottom(container.down('.links'), 
      new EvalTemplate(Page.new_link_template).evaluate({index: this.new_link_index, label: label, link_type: link_type})
    );
    this.new_link_index += 1;
  },
  edit_link: function(link) {
    el = $(link).up('li');
    el.down('.link_display').hide();
    el.down('.link_fields').show();
  },
  remove_link: function(link) {
    el = $(link).up('li');
    new Effect.Fade(el, {duration: 0.3, afterFinish: el.remove.bind(el)});
  },
  
  new_link_index: 2
};

// ==========================================================================
// = EvalTemplate does ruby-style interpretation on a string.               = 
// = optionally, you can pass a context through which it will get data from =
// ==========================================================================

// Example:
// new EvalTemplate("Hey #{user_name}!").evaluate({user_name:"Scott"}); 
// 
// Both return "Hey Scott!"

// EvalTemplate.update_run will interpolate on an elements innerHTML and set that back into the page
// Example:
// 
// HTML = <span id="foo">Hello #{user_name}!</span>
// EvalTemplate.update_run($('foo'), {user_name:"Scott"}));
// 
// This will update the html on the page to read:  <span id="foo">Hello Scott!</span>

var EvalTemplate = Class.create();
EvalTemplate.Pattern = /(^|.|\r|\n)(#\{(.*?)\})/;
EvalTemplate.run = function(template, pattern) {
	return new EvalTemplate(template, pattern).evaluate();
};

EvalTemplate.update_run = function(element, context) {
	template = new EvalTemplate(unescape(element.innerHTML));
	new_html = template.evaluate(context);
	element.update(new_html);
};

EvalTemplate.prototype = {
  initialize: function(template, pattern) {
    this.template = template.toString();
    this.pattern  = pattern || EvalTemplate.Pattern;
  },

  evaluate: function(context) {
		if(context) {Object.extend(this, context);}
		
    return this.template.gsub(this.pattern, function(match) {
      var before = match[1];
      if (before == '\\') return match[2];
			return before + String.interpret(eval(match[3], this));
    }.bind(this));
  }
}

TextHelper = Class.create();
Object.extend(TextHelper, {

  tweet_length: function(string) {
    return string.escapeHTML().length
  }
})

SpinnerButton = {
  click: function(button, label) {
    if(!label){ label = 'Posting...';}
    
    page.async(function() { Field.disable($(button)) }); //cant immediately disable the button because the form wont submit.
    orig = $(button).down('span');
    orig.hide();
    new Insertion.After(orig, "<span>" + label + "</span>");
    //button.down('img').show();
  },
  
  reset: function(button) {
    //button.down('img').hide();
    orig = button.down('span');
    if (orig.next('span')) { orig.next('span').remove() };
    orig.show();
    button.disabled = false;
    Field.enable(button);
  },
  
  reset_all: function(scope) {
    $(scope).select(".spinner").each(function(spinner) {
      SpinnerButton.reset(spinner);
    })
  }
};

SmartTextarea = Class.create();

SmartTextarea.prototype = {
  initialize: function(el, options) {
    if(navigator.userAgent.indexOf('iPhone') > -1){ return; }
    
    this.el = $(el);
    this.el.style.height = "auto"
    this.start_h = this.el.scrollHeight;
    this.start_w = this.el.scrollWidth;
    this.observing = false;
    this.el.onfocus = this.start.bind(this);
    this.el.onblur = this.stop.bind(this);
    this.n_cols = this.el.cols //(this.el.width*.05);
    this.min_rows = 5;
    this.update_rows();
    this.start();
  },
  start: function() {
    if(!this.observing) {
      this.observing = true;
      Event.observe(this.el, "keyup", this.update_rows.bind(this));
    }
  },
  update_rows: function() {
    ln = this.el.value.split("\n"); // Get all rows
    this.n_rows = ln.length;
    for (var i=0; i < ln.length; i++) {
      this.n_rows += Math.floor(ln[i].length/this.n_cols); // for each row divide by cols
    };
    rows = this.n_rows < this.min_rows ? this.min_rows : this.n_rows;
    this.el.setAttribute("rows", rows)
  },
  stop: function() {
    this.observing = false;
  },
  reset: function() {
    this.el.style.height = this.start_h + "px";
  }
}

Combo = Class.create();
Combo.prototype = {
  initialize: function(el) {
    this.el = el;
    this.all_inputs = this.el.select("input");
    this.text_el = this.all_inputs.first();
    this.text_el.setAttribute('autocomplete', 'off');
    // if there are two inputs, the second holds the value (like the <option> element)
    if(this.all_inputs.length == 2) { this.value_el = this.all_inputs.last(); }
    this.click_el = this.el.down('img');
    this.menu = this.el.down('div.menu');
    this.locked = this.el.hasClassName('no_select');
    this.picker = this.el.hasClassName('picker');
    this.observers();
    this.menu_observers();

    var on = el.select(".on")[0];
    if(on) {
      this.menu_click((on.down("a").down("span") || on.down("a")).innerHTML);      
    }
  },
  observers: function() {
    this.click_el.observe("click", this.click.bind(this));
    this.text_el.observe("focus", this.click.bind(this));
    if(this.locked) {
      this.text_el.observe("focus", this.text_el.blur);
    } else {
      this.text_el.observe("focus", function() {
        if($F(this.text_el) == this.text_el.prompted_text) {
          this.text_el.value = '';
          if(!this.keys_bound) {
            this.keys = new key_navigate(this.menu, {observe_field: this.text_el, selector: 'a', active_class: 'on', callback: this.handle_callback.bind(this)});
            
            if(this.el.next() && this.el.next().hasClassName('submit')) {
              this.el.next().observe('click', this.keys.do_callback.bind(this.keys))
            }
            this.keys_bound = true;
          }
        }
      }.bind(this));
    }
  },
  menu_observers: function() {
    this.menu.select("a").each(function(el) {
      el.observe("click", this.menu_click.bind(this, (el.down('span') || el).innerHTML));
    }.bind(this));
  },
  menu_click: function(value) {
    page.notify(value);
    this.text_el.value = value;
    this.menu.hide();
    this.onmousedown = null;
  },
  click: function() {
    if(!this.locked) {
      this.menu.show();
    } else {
      this.menu.toggle();
    }
    page.auto_hide_this(this.el, this.menu, this.call_out.bind(this)); 
  },
  blur: function() {
    this.call_out();
    this.menu.hide();
  },
  call_out: function() {
    if($F(this.text_el) == '')
    this.text_el.value = this.text_el.prompted_text;
  },
  handle_callback: function(el) {
    if(!el) return;
    if(this.picker) {
      this.set_selection(el);
    } else {
      this.follow_link(el);
    }
  },
  follow_link: function(el) {
    this.text_el.addClassName('disabled');
    if(el.hasClassName('attach_query')) {
      var query_str = $F(this.text_el);
      HREF = el.getAttribute("title")+"?query="+query_str;
    } else {
      HREF = el.getAttribute("title");
      this.set_value(el.down('span').innerHTML, el.getAttribute("title"));
    }
    document.location.href = HREF;    
  },
  set_selection: function(el) {
    this.set_value(el.down('span').innerHTML, el.getAttribute("title"));
  },
  formatResults: function(results_container) {
    this.menu.show();
    $(results_container).select('a').each(function(el) {
      if (!el.hasClassName('exempt')) {
        el.href = '#';
        el.onclick = function() {
          this.handle_callback(el);
          return false;
        }.bind(this);
      }
    }.bind(this))
  },
  set_value: function(text, value) {
    this.text_el.value = text;
    if(this.value_el) { this.value_el.value = value; }
    this.menu.hide();
  },
  get_value: function() {
    return $F(this.value_el);
  }
};

key_navigate = Class.create();
key_navigate.prototype = {
  initialize: function(el, options) {
    this.options = options;
    this.el = el;
    this.update_selectable();
    this.options.observe_field.observe("keydown", this.key_events.bind(this));
  },
  key_events: function(event) {
    var key_code = ie ? window.event.keyCode : event.keyCode;
    if(key_code == Event.KEY_UP) {
      this.move_selection(-1);
    } else if(key_code == Event.KEY_DOWN) {
      this.move_selection(1);
    } else if(key_code == Event.KEY_RETURN) {
      this.do_callback();
    } else if(key_code == Event.KEY_TAB) {
      this.el.up('.combo').combo.blur();
    }
  },
  get_selected_element: function() {
    return this.el.down("."+this.options.active_class)
  },
  get_selected_index: function() {
    this.update_selectable();
    for(i=0; i<this.selectable.length; i++) {
      if(this.selectable[i] == this.get_selected_element()) {
        return i; break;
      }
    } return -1;
  },
  move_selection: function(dir) {
    var next_index = this.get_selected_index()+dir;
    if((next_index == -1 && dir == -1) || (next_index == -2 && dir == -1)) {
      if(this.get_selected_element()) {
        this.get_selected_element().removeClassName(this.options.active_class); 
      } return;
    }
    if(next_index == this.selectable.length && dir == 1) return;
    if(this.get_selected_element()) this.get_selected_element().removeClassName(this.options.active_class);
    this.selectable[next_index].addClassName(this.options.active_class);
  },
  do_callback: function() {
    this.options.callback(this.get_selected_element())
  },
  update_selectable: function() {
    this.selectable = this.el.select(this.options.selector);
  }
};

ContextMenu = Class.create();
ContextMenu.prototype = {
  initialize: function(el, callback, options) {
    evt = options && options.event ? options.event : 'click'; // Click, MouseOver
    this.el = $(el);
    this.menu = this.el.next();
    this.callback = callback;
    this.el.observe(evt, this.activate.bind(this))
  },
  activate: function() {
    this.menu.toggle();
    page.auto_hide_this(this.menu);
    if(this.callback) this.callback();
  }
}

User = Class.create();
User.prototype = {
  initialize:function() {
    
  },

  nick: function() {
    return String.interpret(Cookie.get('user_nick')).gsub(/\+/, ' ').escapeHTML();
  },
  
  name: function() {
    field = String.interpret(Cookie.get('use_real_name')) == 'true' ? 'user_name' : 'user_nick'
    name = String.interpret(Cookie.get(field)).gsub(/\+/, ' ');
    if (name.blank()) {
      return this.nick();
    } else {
      return name.escapeHTML();
    }
  },

  avatar_url: function(version) {
    file_base = String.interpret(Cookie.get('avatar_url'));
    if(version) {
      if (file_base.blank()) {
        return '#';
      } else {
        components = file_base.split('.');
        return String.interpret(components.first() + "_" + version + '.' + components.last());
      }
    } else {
      return file_base;
    }
  }
};

Followable = Class.create();
Followable.login_success_template = "\n  Thanks!\n";
Followable.following_data = $H();
Followable.followable_elements = $A();

Followable.prototype = {
  
  initialize: function() {
    page.watch('login', function() { this.get_status() }.bind(this));
  },
  
  detect_followables: function() {
    setTimeout(function() {
      Followable.followable_elements = $('page').select(".followable");
      if(Followable.followable_elements.size() > 0) {
        this.get_status();
      }
    }.bind(this), 1000);
  },

  get_followable_dom_ids: function() {
    if(Followable.followable_elements.size() > 0) {
      return Followable.followable_elements.map(function(f) { return $w($(f).className).first()})
    } else {
      return [];
    }
  },

  get_status: function() {
    if(page.logged_in()) {
      new Ajax.Request('/me/followed.js', {
        method: 'get',
        parameters: {'followables[]': this.get_followable_dom_ids()},
        evalJSON: 'force',
        onSuccess: function(request) {
          Followable.following_data = $H(request.responseJSON);
          this.activate_followable_elements();
        }.bind(this)
      });
    } else {
      Followable.following_data = $H();
      this.activate_followable_elements();
    }
  },

  activate_followable_elements: function() {
    Followable.followable_elements.async_each(function(el) {
      if(!el.hasClassName("empty")) {
        el.followable = new FollowableElement(el);
      }
    }.bind(this));
  }
}

FollowableElement = Class.create();

FollowableElement.prototype = {
  
  initialize: function(el) {
    this.element = $(el);
    this.dom_id = $w(el.className).first();
    Object.extend(this, Followable.following_data.get(this.dom_id));
    this.follow_template = $('followable_dropdown');
    this.followable_type = this.get_followable_type();
    this.enable_activate_link();
    this.enable_delete_button();
    Followable.following_data.get(this.dom_id) ? this.set_as_followed(el) : this.set_as_followable(el);
  },
  
  enable_activate_link: function() {
    this.activate_link = this.element.down(".activate");
    this.activate_link.show();
  },
  
  enable_delete_button: function() {
    if(this.delete_button = this.element.down(".delete")) {
      this.delete_button.show();
    }
  },
  
  activate_overlay: function() {
    this.follow_template.followable = this;
    this.follow_template.dockTo(this.activate_link, {target_point: 'bl'});
    page.auto_hide_this(this.follow_template, this.follow_template, this.hide_dropdown.bind(this));
    page.adjust_scroll_position(this.follow_template, 5);
    this.follow_template.update("<div class=\"pad\"><img src='https://dv4uxy777adjt.cloudfront.net/assets/spinner-164fbf2733162527099b117ed342c39b.gif'></img></div>");
    this.follow_template.show();
  },
  
  get_followable_type: function() {
    return $A(this.dom_id.split('_')).first();
  },
  
  hide_dropdown: function() {
    if (this.follow_template.visible()) { this.follow_template.hide(); }
    document.onmousedown = null;
  },
  
  set_as_followable: function() {
    this.element.select(".activate").invoke('show');
    this.element.select(".activate").invoke('update', '?');
  },

  set_as_followed: function(el) {
    this.element.addClassName("following");
    this.element.select(".activate").invoke('show');
    this.update_activate_link_with_score();
  },
  
  update_activate_link_with_score: function() {
    if(this.activate_link != null) {
      this.activate_link.update(this.net_promoter_score);
      this.activate_link.addClassName("nps_" + this.net_promoter_score);
    }
  }
}

Breadcrumb = {
  expand : function(link) {
    var link = $(link);
    var crumb = link.up('.crumb');
    var hide_element = crumb.down('.crumb_select');
    crumb.toggleClassName('open');
    crumb.autoHide(hide_element, Breadcrumb.clean);
    document.getElementsByTagName('body')[0].onmousedown = function(event) {
      Breadcrumb.hide({element: crumb, callback: Breadcrumb.clean}, event);
    }
  },

  hide : function(options, event) {
    var target = ie ? window.event.srcElement : event.target;
    element = options.element;
    if($(target).outside(element)) {
      element.removeClassName('open')
      document.onmouseup = null; document.onmousedown = null;
      if(options.callback != null) options.callback();
    }
  },

  clean : function() {
    $('crumbs').select('.crumb').invoke("removeClassName", "open");
  },

  over : function(link) {
    $(link).up('.crumb').addClassName('hover');
  },
  
  out : function(link) {
    $(link).up('.crumb').removeClassName('hover');
  }
};

DigestersForm = {
  after: function(form) {
    $(form).down('.error_message').hide();
    $(form).select('label.enabled input').each(function(e) { disabled = true });
  },
  
  complete: function(form, response) {
    if (response.status != 200 || /\<errors\>/.match(response.responseText)) {
      $(form).down('.error_message').show();
    }
    $(form).select('label.enabled input').each(function(e) { e.disabled = false });
  }
};
/*
 * As we refine the widget, please refactor methods into this future-sparkle space.
 */

GSFNFeedbackWidget = {

  // ===== CONFIGURATION / INIT ========================================================================================

  config: function() {
    return this._current_config;
  },

  init: function() {
    this._current_config = {}
  },
  
  // ===== VIEW MANAGEMENT =============================================================================================

  topicForm: function() {
    return $('topic_form');
  },

  topicDetail: function() {
    return $('topic_additional_detail').value
  },

  // ===== ACTIONS =====================================================================================================

  // Invoked when the user tries to submit a topics
  submit: function() {
    try {
      if (this.validateTopic()) {
        submitForm(this.topicForm())
      }
    } catch (e) {
      alert(e)
    }
    
    return false;
  },

  // ===== TOPIC VALIDATION ============================================================================================

  validateTopic: function() {
    if (this.topicContainsSensitiveData()) {
      return confirm(
          "Whoa there! It looks like you're trying to submit some personally identifiable information.  " +
          "This information will be posted in a public forum where everyone can read it.\n\n\n" +
          "Are you sure you would like to post this information where everyone can see it?"
      );
    } else {
      return true;
    }
  },

  topicContainsSensitiveData: function() {
    return this.fieldContainsSensitiveData(this.topicDetail());
  },

  fieldContainsSensitiveData: function(fieldValue) {
    if (page.company == 'harrahs') {
      var match;
      var regex = /\b(([\s\.-]*\d)+)\b/g

      while ((match = regex.exec(fieldValue)) != null) {
        // git rid of non digits
        var matched_value = match[1].split(/\D/).join("")
        if (matched_value.search(/^\d{11}$/) >= 0) {
          return true;
        }
      }
    }
    
    return false;
  }

};
// Prototype port of Farbtastic 1.2


Element.addMethods({
  protofarbtastic: function(container, callback) {
    var container = $(container);
    return container._protofarbtastic || (container._protofarbtastic = new ProtoFarbtastic(container, callback));
  }
});


var ProtoFarbtastic = Class.create();
ProtoFarbtastic.prototype = {
  initialize: function (container, callback) {

    // Store farbtastic object
    var fb = this;
    
    // Insert markup
    $(container).innerHTML = '<div class="farbtastic"><div class="color"></div><div class="wheel"></div><div class="overlay"></div><div class="h-marker marker"></div><div class="sl-marker marker"></div></div>';
    var e = $A(container.select('.farbtastic'));
    fb.wheel = container.select('.wheel')[0];
    // Dimensions
    fb.radius = 84;
    fb.square = 100;
    fb.width = 194;

    // Fix background PNGs in IE6
    if (navigator.appVersion.match(/MSIE [0-6]\./)) {
      e.each(function (el) {
          if (el.style.backgroundImage != 'none') {
            var image = el.style.backgroundImage;
            image = el.style.backgroundImage.substring(5, image.length - 2);
            $(el).setStyle({
              backgroundImage: 'none',
              filter: "progid:DXImageTransform.Microsoft.AlphaImageLoader(enabled=true, sizingMethod=crop, src='" + image + "')"
            });
          }
        });
    }
    
    /**
     * Link to the given element(s) or callback.
     */
    fb.linkTo = function (callback) {
      // Unbind previous nodes
      if (typeof fb.callback == 'object') {
        Event.stopObserving(fb, 'keyup', fb.updateValue, false);
      }
      
      // Reset color
      fb.color = null;
      
      // Bind callback or elements
      if (typeof callback == 'function') {
        fb.callback = callback;
      }
      else if (typeof callback == 'object' || typeof callback == 'string') {
        fb.callback = $(callback);
        Event.observe(fb.callback, 'keyup', fb.updateValue, false);
        if (fb.callback.value) {
          fb.setColor(fb.callback.value);
        }
      }
      return this;
    }

    fb.updateValue = function (event) {
      if (this.value && this.value != fb.color) {
        // Force upper-case hex values...
        this.value = this.value.toUpperCase();
        
        if (this.value.indexOf('#') != 0)
          this.value = '#' + this.value;
        
        fb.setColor(this.value);
      }
      Event.stop(event);
    }
    
    /**
     * Change color with HTML syntax #123456
     */
    fb.setColor = function (color) {
      var unpack = fb.unpack(color);
      if (fb.color != color && unpack) {
        fb.color = color;
        fb.rgb = unpack;
        fb.hsl = fb.RGBToHSL(fb.rgb);
        fb.updateDisplay();
      }
      return this;
    }

    /**
     * Change color with HSL triplet [0..1, 0..1, 0..1]
     */
    fb.setHSL = function (hsl) {
      fb.hsl = hsl;
      fb.rgb = fb.HSLToRGB(hsl);
      fb.color = fb.pack(fb.rgb);
      fb.updateDisplay();
      return this;
    }

    /////////////////////////////////////////////////////

    /**
     * Retrieve the coordinates of the given event relative to the center
     * of the widget.
     */
    fb.widgetCoords = function (event) {
      var x, y;
      var el = event.target || event.srcElement;
      var reference = fb.wheel;

      if (typeof event.offsetX != 'undefined') {
        // Use offset coordinates and find common offsetParent
        var pos = { x: event.offsetX, y: event.offsetY };

        // Send the coordinates upwards through the offsetParent chain.
        var e = el;
        while (e) {
          e.mouseX = pos.x;
          e.mouseY = pos.y;
          pos.x += e.offsetLeft;
          pos.y += e.offsetTop;
          e = e.offsetParent;
        }

        // Look for the coordinates starting from the wheel widget.
        var e = reference;
        var offset = { x: 0, y: 0 }
        while (e) {
          if (typeof e.mouseX != 'undefined') {
            x = e.mouseX - offset.x;
            y = e.mouseY - offset.y;
            break;
          }
          offset.x += e.offsetLeft;
          offset.y += e.offsetTop;
          e = e.offsetParent;
        }

        // Reset stored coordinates
        e = el;
        while (e) {
          e.mouseX = undefined;
          e.mouseY = undefined;
          e = e.offsetParent;
        }
      }
      else {
        // Use absolute coordinates
        var pos = fb.absolutePosition(reference);
        x = (event.pageX || 0*(event.clientX + document.body.scrollLeft)) - pos.x;
        y = (event.pageY || 0*(event.clientY + document.body.scrollTop)) - pos.y;
      }
      
      Event.stop(event);
      
      // Subtract distance to middle
      return { x: x - fb.width / 2, y: y - fb.width / 2 };
    }

    /**
     * Mousedown handler
     */
    fb.mousedown = function (event) {
      // Capture mouse
      if (!document.dragging) {
        Event.observe(document, 'mousemove', fb.mousemove, false);
        Event.observe(document, 'mouseup',   fb.mouseup, false);
        document.dragging = true;
      }

      // Check which area is being dragged
      var pos = fb.widgetCoords(event);
      fb.circleDrag = Math.max(Math.abs(pos.x), Math.abs(pos.y)) * 2 > fb.square;

      // Process
      fb.mousemove(event);
      
      if (event) Event.stop(event);
      
      return false;
    }

    /**
     * Mousemove handler
     */
    fb.mousemove = function (event) {
      // Get coordinates relative to color picker center
      var pos = fb.widgetCoords(event);

      // Set new HSL parameters
      if (fb.circleDrag) {
        var hue = Math.atan2(pos.x, -pos.y) / 6.28;
        if (hue < 0) hue += 1;
        fb.setHSL([hue, fb.hsl[1], fb.hsl[2]]);
      }
      else {
        var sat = Math.max(0, Math.min(1, -(pos.x / fb.square) + .5));
        var lum = Math.max(0, Math.min(1, -(pos.y / fb.square) + .5));
        fb.setHSL([fb.hsl[0], sat, lum]);
      }

      Event.stop(event);
      return false;
    }

    /**
     * Mouseup handler
     */
    fb.mouseup = function () {
      // Uncapture mouse
      Event.stopObserving(document, 'mousemove', fb.mousemove, false);
      Event.stopObserving(document, 'mouseup',   fb.mouseup, false);
      document.dragging = false;
    }

    /**
     * Update the markers and styles
     */
    fb.updateDisplay = function () {
      // Markers
      var angle = fb.hsl[0] * 6.28;
      $A(e).each(function(itE) {
          $A(itE.select('.h-marker')).each(function(el) {
              el.setStyle({
                left: Math.round(Math.sin(angle) * fb.radius + fb.width / 2) + 'px',
                    top: Math.round(-Math.cos(angle) * fb.radius + fb.width / 2) + 'px'
                    });
            });
        });

      $A(e).each(function(itE) {
          $A(itE.select('.sl-marker')).each(function(el) {
              el.setStyle({
                left: Math.round(fb.square * (.5 - fb.hsl[1]) + fb.width / 2) + 'px',
                    top: Math.round(fb.square * (.5 - fb.hsl[2]) + fb.width / 2) + 'px'              
                    });
            });
        });
      
      // Saturation/Luminance gradient
      $A(e).each(function(itE) {
          $A(itE.select('.color')).each(function(el) {
              el.setStyle({
                backgroundColor: fb.pack(fb.HSLToRGB([fb.hsl[0], 1, 0.5]))
                    });
            });
        });
      
      // Linked elements or callback
      if (typeof fb.callback == 'object') {
        // Set background/foreground color

        $A([fb.callback]).flatten().each(function(el) {
            el.setStyle({
              backgroundColor: fb.color,
              color: fb.hsl[2] > 0.5 ? '#000' : '#fff'
            });
          });

        // Change linked value
        $A([fb.callback]).flatten().each(function(el) {
            if (el.value && el.value != fb.color) {
              el.value = fb.color;
            }
          });
      }
      else if (typeof fb.callback == 'function') {
        fb.callback.call(fb, fb.color);
      }
    }

    /**
     * Get absolute position of element
     */
    fb.absolutePosition = function (el) {
      var r = { x: el.offsetLeft, y: el.offsetTop };
      // Resolve relative to offsetParent
      if (el.offsetParent) {
        var tmp = fb.absolutePosition(el.offsetParent);
        r.x += tmp.x;
        r.y += tmp.y;
      }
      return r;
    };

    /* Various color utility functions */
    fb.pack = function (rgb) {
      var r = Math.round(rgb[0] * 255);
      var g = Math.round(rgb[1] * 255);
      var b = Math.round(rgb[2] * 255);
      return ('#' + (r < 16 ? '0' : '') + r.toString(16) +
                    (g < 16 ? '0' : '') + g.toString(16) +
                    (b < 16 ? '0' : '') + b.toString(16)).toUpperCase();
    }

    fb.unpack = function (color) {
      if (color.length == 7) {
        return [parseInt('0x' + color.substring(1, 3)) / 255,
                parseInt('0x' + color.substring(3, 5)) / 255,
                parseInt('0x' + color.substring(5, 7)) / 255];
      }
      else if (color.length == 4) {
        return [parseInt('0x' + color.substring(1, 2)) / 15,
                parseInt('0x' + color.substring(2, 3)) / 15,
                parseInt('0x' + color.substring(3, 4)) / 15];
      }
    }

    fb.HSLToRGB = function (hsl) {
      var m1, m2, r, g, b;
      var h = hsl[0], s = hsl[1], l = hsl[2];
      m2 = (l <= 0.5) ? l * (s + 1) : l + s - l*s;
      m1 = l * 2 - m2;
      return [this.hueToRGB(m1, m2, h+0.33333),
              this.hueToRGB(m1, m2, h),
              this.hueToRGB(m1, m2, h-0.33333)];
    }

    fb.hueToRGB = function (m1, m2, h) {
      h = (h < 0) ? h + 1 : ((h > 1) ? h - 1 : h);
      if (h * 6 < 1) return m1 + (m2 - m1) * h * 6;
      if (h * 2 < 1) return m2;
      if (h * 3 < 2) return m1 + (m2 - m1) * (0.66666 - h) * 6;
      return m1;
    }

    fb.RGBToHSL = function (rgb) {
      var min, max, delta, h, s, l;
      var r = rgb[0], g = rgb[1], b = rgb[2];
      min = Math.min(r, Math.min(g, b));
      max = Math.max(r, Math.max(g, b));
      delta = max - min;
      l = (min + max) / 2;
      s = 0;
      if (l > 0 && l < 1) {
        s = delta / (l < 0.5 ? (2 * l) : (2 - 2 * l));
      }
      h = 0;
      if (delta > 0) {
        if (max == r && max != g) h += (g - b) / delta;
        if (max == g && max != b) h += (2 + (b - r) / delta);
        if (max == b && max != r) h += (4 + (r - g) / delta);
        h /= 6;
      }
      return [h, s, l];
    }

    // Install mousedown handler (the others are set on the document on-demand)
    // $('*', e).mousedown(fb.mousedown);
    e.each(function (el) {
        Event.observe(el, 'mousedown', fb.mousedown, false);
      });
    // Init color
    fb.setColor('#000000');

    // Set linked elements/callback
    if (callback) {
      fb.linkTo(callback);
    }
  }
};




























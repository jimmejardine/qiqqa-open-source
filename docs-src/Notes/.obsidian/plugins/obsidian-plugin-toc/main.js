var __create = Object.create;
var __defProp = Object.defineProperty;
var __getProtoOf = Object.getPrototypeOf;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __markAsModule = (target) => __defProp(target, "__esModule", {value: true});
var __commonJS = (callback, module2) => () => {
  if (!module2) {
    module2 = {exports: {}};
    callback(module2.exports, module2);
  }
  return module2.exports;
};
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, {get: all[name], enumerable: true});
};
var __exportStar = (target, module2, desc) => {
  if (module2 && typeof module2 === "object" || typeof module2 === "function") {
    for (let key of __getOwnPropNames(module2))
      if (!__hasOwnProp.call(target, key) && key !== "default")
        __defProp(target, key, {get: () => module2[key], enumerable: !(desc = __getOwnPropDesc(module2, key)) || desc.enumerable});
  }
  return target;
};
var __toModule = (module2) => {
  if (module2 && module2.__esModule)
    return module2;
  return __exportStar(__markAsModule(__defProp(module2 != null ? __create(__getProtoOf(module2)) : {}, "default", {value: module2, enumerable: true})), module2);
};

// node_modules/dedent/dist/dedent.js
var require_dedent = __commonJS((exports2, module2) => {
  "use strict";
  function dedent(strings) {
    var raw = void 0;
    if (typeof strings === "string") {
      raw = [strings];
    } else {
      raw = strings.raw;
    }
    var result = "";
    for (var i = 0; i < raw.length; i++) {
      result += raw[i].replace(/\\\n[ \t]*/g, "").replace(/\\`/g, "`");
      if (i < (arguments.length <= 1 ? 0 : arguments.length - 1)) {
        result += arguments.length <= i + 1 ? void 0 : arguments[i + 1];
      }
    }
    var lines = result.split("\n");
    var mindent = null;
    lines.forEach(function(l) {
      var m = l.match(/^(\s+)\S+/);
      if (m) {
        var indent = m[1].length;
        if (!mindent) {
          mindent = indent;
        } else {
          mindent = Math.min(mindent, indent);
        }
      }
    });
    if (mindent !== null) {
      result = lines.map(function(l) {
        return l[0] === " " ? l.slice(mindent) : l;
      }).join("\n");
    }
    result = result.trim();
    return result.replace(/\\n/g, "\n");
  }
  if (typeof module2 !== "undefined") {
    module2.exports = dedent;
  }
});

// node_modules/objectorarray/index.js
var require_objectorarray = __commonJS((exports2, module2) => {
  module2.exports = (val) => {
    return val != null && typeof val === "object" && val.constructor !== RegExp;
  };
});

// node_modules/fast-json-parse/parse.js
var require_parse = __commonJS((exports2, module2) => {
  "use strict";
  function Parse(data) {
    if (!(this instanceof Parse)) {
      return new Parse(data);
    }
    this.err = null;
    this.value = null;
    try {
      this.value = JSON.parse(data);
    } catch (err) {
      this.err = err;
    }
  }
  module2.exports = Parse;
});

// node_modules/endent/lib/index.js
var require_lib = __commonJS((exports2) => {
  "use strict";
  var __importDefault = exports2 && exports2.__importDefault || function(mod) {
    return mod && mod.__esModule ? mod : {default: mod};
  };
  Object.defineProperty(exports2, "__esModule", {value: true});
  var dedent_1 = __importDefault(require_dedent());
  var objectorarray_1 = __importDefault(require_objectorarray());
  var fast_json_parse_1 = __importDefault(require_parse());
  var ENDENT_ID = "twhZNwxI1aFG3r4";
  function endent2(strings, ...values) {
    let result = "";
    for (let i = 0; i < strings.length; i++) {
      result += strings[i];
      if (i < values.length) {
        let value = values[i];
        let isJson = false;
        if (fast_json_parse_1.default(value).value) {
          value = fast_json_parse_1.default(value).value;
          isJson = true;
        }
        if (value && value[ENDENT_ID] || isJson) {
          let rawlines = result.split("\n");
          let l = rawlines[rawlines.length - 1].search(/\S/);
          let endentation = l > 0 ? " ".repeat(l) : "";
          let valueJson = isJson ? JSON.stringify(value, null, 2) : value[ENDENT_ID];
          let valueLines = valueJson.split("\n");
          valueLines.forEach((l2, index) => {
            if (index > 0) {
              result += "\n" + endentation + l2;
            } else {
              result += l2;
            }
          });
        } else if (typeof value === "string" && value.includes("\n")) {
          let endentations = result.match(/(?:^|\n)( *)$/);
          if (typeof value === "string") {
            let endentation = endentations ? endentations[1] : "";
            result += value.split("\n").map((str, i2) => {
              str = ENDENT_ID + str;
              return i2 === 0 ? str : `${endentation}${str}`;
            }).join("\n");
          } else {
            result += value;
          }
        } else {
          result += value;
        }
      }
    }
    result = dedent_1.default(result);
    return result.split(ENDENT_ID).join("");
  }
  endent2.pretty = (data) => {
    return objectorarray_1.default(data) ? {[ENDENT_ID]: JSON.stringify(data, null, 2)} : data;
  };
  exports2.default = endent2;
});

// src/main.ts
__markAsModule(exports);
__export(exports, {
  default: () => main_default
});
var import_obsidian2 = __toModule(require("obsidian"));

// src/create-toc.ts
var import_endent = __toModule(require_lib());
var import_obsidian = __toModule(require("obsidian"));
var getCurrentHeaderDepth = (headings, cursor) => {
  const previousHeadings = headings.filter((heading) => heading.position.end.line < cursor.line);
  if (!previousHeadings.length) {
    return 0;
  }
  return previousHeadings[previousHeadings.length - 1].level;
};
var getSubsequentHeadings = (headings, cursor) => {
  return headings.filter((heading) => heading.position.end.line > cursor.line);
};
var getPreviousLevelHeading = (headings, currentHeading) => {
  const index = headings.indexOf(currentHeading);
  const targetHeadings = headings.slice(0, index).reverse();
  return targetHeadings.find((item, _index, _array) => {
    return item.level == currentHeading.level - 1;
  });
};
var createToc = ({headings = []}, cursor, settings) => {
  const currentDepth = getCurrentHeaderDepth(headings, cursor);
  const subsequentHeadings = getSubsequentHeadings(headings, cursor);
  const includedHeadings = [];
  for (const heading of subsequentHeadings) {
    if (heading.level <= currentDepth) {
      break;
    }
    if (heading.level >= settings.minimumDepth && heading.level <= settings.maximumDepth) {
      includedHeadings.push(heading);
    }
  }
  if (!includedHeadings.length) {
    new import_obsidian.Notice(import_endent.default`
        No headings below cursor matched settings 
        (min: ${settings.minimumDepth}) (max: ${settings.maximumDepth})
      `);
    return;
  }
  const firstHeadingDepth = includedHeadings[0].level;
  const links = includedHeadings.map((heading) => {
    const itemIndication = settings.listStyle === "number" && "1." || "-";
    const indent = new Array(Math.max(0, heading.level - firstHeadingDepth)).fill("	").join("");
    const previousLevelHeading = getPreviousLevelHeading(includedHeadings, heading);
    if (typeof previousLevelHeading == "undefined") {
      return `${indent}${itemIndication} [[#${heading.heading}|${heading.heading}]]`;
    } else {
      return `${indent}${itemIndication} [[#${previousLevelHeading.heading}#${heading.heading}|${heading.heading}]]`;
    }
  });
  return import_endent.default`
    ${settings.title ? `${settings.title}
` : ""}
    ${`${links.join("\n")}
`}
  `;
};

// src/main.ts
var TableOfContentsSettingsTab = class extends import_obsidian2.PluginSettingTab {
  constructor(app, plugin) {
    super(app, plugin);
    this.plugin = plugin;
  }
  display() {
    const {containerEl} = this;
    containerEl.empty();
    containerEl.createEl("h2", {text: "Table of Contents - Settings"});
    new import_obsidian2.Setting(containerEl).setName("List Style").setDesc("The type of list to render the table of contents as.").addDropdown((dropdown) => dropdown.setValue(this.plugin.settings.listStyle).addOption("bullet", "Bullet").addOption("number", "Number").onChange((value) => {
      this.plugin.settings.listStyle = value;
      this.plugin.saveData(this.plugin.settings);
      this.display();
    }));
    new import_obsidian2.Setting(containerEl).setName("Title").setDesc("Optional title to put before the table of contents").addText((text) => text.setPlaceholder("**Table of Contents**").setValue(this.plugin.settings.title || "").onChange((value) => {
      this.plugin.settings.title = value;
      this.plugin.saveData(this.plugin.settings);
    }));
    new import_obsidian2.Setting(containerEl).setName("Minimum Header Depth").setDesc("The lowest header depth to add to the table of contents. Defaults to 2").addSlider((text) => text.setValue(this.plugin.settings.minimumDepth).setDynamicTooltip().setLimits(1, 6, 1).onChange((value) => {
      this.plugin.settings.minimumDepth = value;
      this.plugin.saveData(this.plugin.settings);
    }));
    new import_obsidian2.Setting(containerEl).setName("Maximum Header Depth").setDesc("The highest header depth to add to the table of contents. Defaults to 6").addSlider((text) => text.setValue(this.plugin.settings.maximumDepth).setDynamicTooltip().setLimits(1, 6, 1).onChange((value) => {
      this.plugin.settings.maximumDepth = value;
      this.plugin.saveData(this.plugin.settings);
    }));
  }
};
var TableOfContentsPlugin = class extends import_obsidian2.Plugin {
  constructor() {
    super(...arguments);
    this.settings = {
      minimumDepth: 2,
      maximumDepth: 6,
      listStyle: "bullet"
    };
    this.createTocForActiveFile = (settings = this.settings) => () => {
      const activeView = this.app.workspace.getActiveViewOfType(import_obsidian2.MarkdownView);
      if (activeView && activeView.file) {
        const editor = activeView.sourceMode.cmEditor;
        const cursor = editor.getCursor();
        const data = this.app.metadataCache.getFileCache(activeView.file) || {};
        const toc = createToc(data, cursor, typeof settings === "function" ? settings(data, cursor) : settings);
        if (toc) {
          editor.replaceRange(toc, cursor);
        }
      }
    };
  }
  async onload() {
    console.log("Load Table of Contents plugin");
    this.settings = {
      ...this.settings,
      ...await this.loadData()
    };
    this.addCommand({
      id: "create-toc",
      name: "Create table of contents",
      callback: this.createTocForActiveFile()
    });
    this.addCommand({
      id: "create-toc-next-level",
      name: "Create table of contents for next heading level",
      callback: this.createTocForActiveFile((data, cursor) => {
        const currentHeaderDepth = getCurrentHeaderDepth(data.headings || [], cursor);
        const depth = Math.max(currentHeaderDepth + 1, this.settings.minimumDepth);
        return {
          ...this.settings,
          minimumDepth: depth,
          maximumDepth: depth
        };
      })
    });
    this.addSettingTab(new TableOfContentsSettingsTab(this.app, this));
  }
};
var main_default = TableOfContentsPlugin;

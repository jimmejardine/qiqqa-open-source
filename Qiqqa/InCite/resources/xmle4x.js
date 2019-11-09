var CSL_E4X = function () {};
CSL_E4X.prototype.clean = function (xml) {
    xml = xml.replace(/<\?[^?]+\?>/g, "");
    xml = xml.replace(/<![^>]+>/g, "");
    xml = xml.replace(/^\s+/g, "");
    xml = xml.replace(/\s+$/g, "");
    return xml;
};
CSL_E4X.prototype.getStyleId = function (myxml) {
    var text = "";
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    var node = myxml..id;
    if (node && node.length()) {
        text = node[0].toString();
    }
    return text;
};
CSL_E4X.prototype.children = function (myxml) {
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    return myxml.children();
};
CSL_E4X.prototype.nodename = function (myxml) {
    var ret = myxml.localName();
    return ret;
};
CSL_E4X.prototype.attributes = function (myxml) {
    var ret, attrs, attr, key, xml;
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    ret = new Object();
    attrs = myxml.attributes();
    for each (attr in attrs) {
        key = "@" + attr.localName();
        if (key.slice(0,5) == "@e4x_") {
            continue;
        }
        ret[key] = attr.toString();
    }
    return ret;
};
CSL_E4X.prototype.content = function (myxml) {
    return myxml.toString();
};
CSL_E4X.prototype.namespace = {
    "xml":"http://www.w3.org/XML/1998/namespace"
}
CSL_E4X.prototype.numberofnodes = function (myxml) {
    if (typeof myxml === "xml") {
        return myxml.length();
    } else if (myxml) {
        return myxml.length;
    } else {
        return 0;
    }
};
CSL_E4X.prototype.getAttributeName = function (attr) {
    var ret = attr.localName();
    return ret;
}
CSL_E4X.prototype.getAttributeValue = function (myxml,name,namespace) {
    var xml;
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    if (namespace) {
        var ns = new Namespace(this.namespace[namespace]);
        var ret = myxml.@ns::[name].toString();
    } else {
        if (name) {
            var ret = myxml.attribute(name).toString();
        } else {
            var ret = myxml.toString();
        }
    }
    return ret;
}
CSL_E4X.prototype.getNodeValue = function (myxml,name) {
    var xml;
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    if (name){
        return myxml[name].toString();
    } else {
        return myxml.toString();
    }
}
CSL_E4X.prototype.setAttributeOnNodeIdentifiedByNameAttribute = function (myxml,nodename,attrname,attr,val) {
    var xml;
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    if (attr[0] != '@'){
        attr = '@'+attr;
    }
    myxml[nodename].(@name == attrname)[0][attr] = val;
}
CSL_E4X.prototype.deleteNodeByNameAttribute = function (myxml,val) {
    delete myxml.*.(@name==val)[0];
}
CSL_E4X.prototype.deleteAttribute = function (myxml,attr) {
    delete myxml["@"+attr];
}
CSL_E4X.prototype.setAttribute = function (myxml,attr,val) {
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    myxml['@'+attr] = val;
}
CSL_E4X.prototype.nodeCopy = function (myxml) {
    return myxml.copy();
}
CSL_E4X.prototype.getNodesByName = function (myxml,name,nameattrval) {
    var xml, ret, retnodes;
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    retnodes = myxml.descendants(name);
    ret = [];
    if (nameattrval){
        retnodes = retnodes.(@name == nameattrval);
        if (retnodes.toXMLString()) {
            ret.push(retnodes);
        }
    } else {
        for each(var retnode in retnodes) {
            ret.push(retnode);
        }
    }
    return ret;
}
CSL_E4X.prototype.nodeNameIs = function (myxml,name) {
    var xml;
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    if (myxml.localName() && myxml.localName().toString() == name){
        return true;
    }
    return false;
}
CSL_E4X.prototype.makeXml = function (myxml) {
    var xml;
    XML.ignoreComments = true;
    XML.ignoreProcessingInstructions = true;
     XML.ignoreWhitespace = true;
    XML.prettyPrinting = true;
    XML.prettyIndent = 2;
    if ("xml" == typeof myxml){
        myxml = myxml.toXMLString();
    };
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    xml = new Namespace("http://www.w3.org/XML/1998/namespace");
    if (myxml){
        myxml = myxml.replace(/\s*<\?[^>]*\?>\s*\n*/g, "");
        myxml = new XML(myxml);
    } else {
        myxml = new XML();
    }
    return myxml;
};
CSL_E4X.prototype.insertChildNodeAfter = function (parent,node,pos,datexml) {
    var myxml, xml;
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    myxml = XML(datexml.toXMLString());
    parent.insertChildAfter(node,myxml);
    delete parent.*[pos];
    return parent;
};
CSL_E4X.prototype.insertPublisherAndPlace = function(myxml) {
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    for each (var node in myxml..group) {
            if (node.children().length() === 2) {
                var twovars = [];
                for each (var child in node.children()) {
                        if (child.children().length() === 0
) {
                            twovars.push(child.@variable.toString());
                            if (child.@suffix.toString()
                                || child.@prefix.toString()) {
                                twovars = [];
                                break;
                            }
                        }
                    }
                if (twovars.indexOf("publisher") > -1 && twovars.indexOf("publisher-place") > -1) {
                    node["@has-publisher-and-publisher-place"] = "true";
                }
            }
        }
};
CSL_E4X.prototype.addMissingNameNodes = function(myxml) {
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    for each (node in myxml..names) {
        if ("xml" == typeof node && node.parent().localName() !== "substitute" && node.elements("name").length() === 0) {
            var name = <name/>;
            node.appendChild(name);
        }
    }
};
CSL_E4X.prototype.addInstitutionNodes = function(myxml) {
    var institution_long, institution_short, name_part, children, node, xml;
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    for each (node in myxml..names) {
        if ("xml" == typeof node && node.elements("name").length() > 0) {
            var name = node.name[0];
            if (node.institution.length() === 0) {
                institution_long = <institution
                    institution-parts="long"
                    substitute-use-first="1"
                    use-last="1"/>
                institution_part = <institution-part name="long"/>;
                node.insertChildAfter(name,institution_long);
                node.institution.@delimiter = node.name.@delimiter.toString();
                if (node.name.@and.toString()) {
                    node.institution.@and = "text";
                }
                node.institution[0].appendChild(institution_part);
                for each (var attr in CSL.INSTITUTION_KEYS) {
                    if (node.name.@[attr].toString()) {
                        node.institution['institution-part'][0].@[attr] = node.name.@[attr].toString();
                    }
                }
                for each (var namepartnode in node.name['name-part']) {
                       if (namepartnode.@name.toString() === 'family') {
                        for each (var attr in CSL.INSTITUTION_KEYS) {
                            if (namepartnode.@[attr].toString()) {
                                node.institution['institution-part'][0].@[attr] = namepartnode.@[attr].toString();
                            }
                        }
                       }
                }
            }
        }
    }
};
CSL_E4X.prototype.flagDateMacros = function(myxml) {
    default xml namespace = "http://purl.org/net/xbiblio/csl"; with({});
    for each (node in myxml..macro) {
        if (node..date.length()) {
            node.@['macro-has-date'] = 'true';
        }
    }
};

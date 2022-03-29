# Tagging Woes :: Maybe Another Approach Needed?

Have had plenty trouble with *categorizing* / *tagging* articles, notes, etc., also in Qiqqa -- or should I say: *particularly* in Qiqqa? (since I utterly failed to get anywhere near satisfactory results after a while any time before I ran into Qiqqa *anyway*.)

Thought:

- Hierarchy in tagging doesn't work for me. (I notice that, reading [this zettelkasten blog article](https://zettelkasten.de/posts/object-tags-vs-topic-tags/), several others ran into the same issue. They call it 'namespacing' but it's the same thing underpinning it all: *hierarchy*)
- There is indeed some very useful differentiation between Sascha's terms of a (_thematic_) **topic** and an (_ontological_) **object**. (See also [comments in blog](https://zettelkasten.de/posts/object-tags-vs-topic-tags/))

  However, that's not going to fly in my opinion, as:
  
  1. Sascha considers broad tags to be useless (or that's what I read between the lines there), while I disagree: **often** those are not what you're looking for at **that** moment, but I have often enough wanted to extract a ream of notes or articles from my collection for **personal use of recall / refresher after hiatus or for distribution / _sharing_, where the _ideal search result_ would be a collection of notes & articles you can bundle and ship off for others to go through and use / critique**: then I know the general subject (*generic tag*) and all relevant and related stuff should show up, a bit like a graph (Obsidian: Vault) to be perused without obnoxious missing links in the subject area. Hence a need, **right there**, for **generic tags**, which would serve as a graph-based categorization -- rather than the too-limited hierarchical categorization.
  2. If you differentiate hard between those two *meta-categories* (thematic and ontological), you have again succumbed to a restrictive hierarchy. I can't get away from the perception that this tags stuff will only work when you keep it N-dimensional. 
    
	 Sure, some hierarchies are useful, e.g. the `author/<name>` example used in the comments there, and thus *namespacing* is not *wrong* per se, but it's the *minor* important bit in tag organization and use, I feel.
	 
 After reading that blog, which addresses a *lot* of my concerns with tags and their use, the (still vague) idea is a question:
 
 Can we come up with a system where you can add tags to articles, and then *edit* the tag *type* (*kind*? _**meta**-category_?) without changing the tag itself, so the edit would be a kind of drag&drop from one *meta*-zone to another? Plus such an edit would be minimal and not require any tags in documents/notes to be updated, as the tags remain as they are.
 
 > Counterpoint: at the time you added those tags, their *meaning* was different then they will be after you change the **meta-type** of the tag that way, so you *will have to go through the linked set of articles and review (+ edit) those tags anyway*. 
>
> *However*, such a *follow-up edit* of a document set would still be *partial*, because there's a *reason* you just changed the meta-type of that tag: you felt it fits the bill better after all, at least for a (probable?) *majority* of the tagged documents! **OR** you just discovered the tag is used incorrectly and needs to move its semantic meaning, after which you know you'll have to go through that list of docs to change/replace that tag. --> bulk editing document sets: remove/add/rename tag.

## Filtering on meta-type: when we are looking for specific *kinds of tags* (as [per Sascha's scenario](https://zettelkasten.de/posts/object-tags-vs-topic-tags/))

This is the second part of that UI/UX thought on tags management: when tags have meta-types, you can then answer his scenario where *generic, broad tags are in the way of a good result* by adding a filter on *which **types of tags** you want to see? (hmm, maybe these are better called meta-tags? And then have those N levels deep, i.e. we don't stop at one level of 'meta', but you get a (*mostly* *hierarchical*?) graph of **tag categorization** instead?)


## Summary? Conclusion?

The above train of thought is now becoming clearer:

- tags are themselves not hierarchical but **yet another graph structure**.
- tags have semantic meaning, which is a posh way of saying: **you can tag the tags**: a tag has then become just another (*extremely short*!) note/article.
- Hence, **tags are a _recursive_ meta-structure**: a tag is a bit of _categorization_ of the tagged _item_. 

  _Recursive_ is an **important** realization just now, as nothing says you should not _tag a tag_: _namespacing_ is the purely hierarchical version thereof, and namespacing can be multiple levels, but that *fails*, not because of the multiple levels, but because, once you arrive at multiple namespace levels like that, you have already run into the brick wall of the restrictiveness of a pure hierarchy: a lot of stuff isn't exactly hierarchical, because hierarchies cannot clearly and cleanly map N-dimensional models, which is what you have when you consider *multiple aspects*, e.g. *social*/*health* vs. *chemistry* (the science and technology) and the relations that become visible when regarding a subject matter from a certain *aspect*. And then *changing* your *aspect* and re-evaluate: that leads to different relationships coming to the fore. That's a *recursive graph*, where the recursion would be representing the *level of abstraction* (*level of detail*): hence a 'looking for a bundle collecting this subject matter' would be a search for a certain aspect at a higher abstraction level (recursion level) then looking for, say, 'insulin reactions' at the chemical and (body) physical level.
  
---
  
## How would we implement that (in Qiqqa)?
  
Two things have been identified here (or is it three? four?):
  
1. you add tags to articles without regard for *hierarchy*. The tag list assigned to an item contains both broad and precise, thematic and ontological tags. This doesn't change what we already have.
2. Tags themselves are not a flat set structure, but they have links: thus tags should be 'categorized' by assigning other tags (*meta-tags*) to them. The *recursive* nature then dictates that those meta-tags can be 'categorized' by assigning other tags to them (meta-meta-tags).
  
   > Do we differentiate between meta<sup>n</sup> tags, i.e. are those levels each a separate set? Intuition says: NO. One set of words, the assignments determine if those are serving as tag, meta-tag, meta-meta-tag, and so forth, in the *given situation* -- otherwise we don't mind as this entire tag structure has then become a multi-dimensional graph.
     
3. You probably will want to *visualize* these graph-like relationships too, so that requires yet another graph-view, this time for tags.

   > Or is the smarter approach there to filter on *what abstraction levels* show up in the graph, so we can have tagged documents, etc. (the *leaf nodes* in that entire tag graph, after all!) in there too: then we can see which general categories collect which clusters of documents: a 3D graph.

4. You definitely will want to **filter** the tag graph to view/search/browse only a specific subgraph of tags ('subset of tags' not exactly covering this very well anymore): I imagine you pick a few tags in such a filter as the tags that dictate the subset: the tag graph is *directed* (meta --> lower level) so we can choose a tag and then go *down* the DAG to find tags (and articles) thus related, which makes immediate sense.

   > I wonder if it would make sense to filter in the *upwards direction* as well: pick a tag, then have all its *metas*, i.e. 'parents', in the graph. 
   >
   > If you travel downwards from there again, you either end up with the entire library (everything is interconnected) *or* you discover "orphan clusters" that way. Hmmmmm. A diagnostic tool then, perhaps.
   > 






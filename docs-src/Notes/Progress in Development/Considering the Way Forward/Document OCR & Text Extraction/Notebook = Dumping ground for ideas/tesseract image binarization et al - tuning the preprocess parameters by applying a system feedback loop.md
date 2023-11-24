# Re `tesseract` image binarization et al: *tuning* the preprocess parameters by applying a system feedback loop

Modern man would sell this as *AI training* but let's forego the modern sales jazz. 

The key observation here is that it significantly how your image binarization/greyscaling process behaves when you watch the (tesseract) OCR engine output quality: just a little less or a little more can do wonders to your text output -- or ruin it entirely.

So the idea is this: run the process as usual, but define a band/range within which you allow the system to twiddle the knobs. Then you can do either of these while you hunt or optimum result = maximum text output quality:

- the naïve feedback approach, which would be to twiddle one knob (in round-robin fashion? so all knobs get their turn in "parallel"?) and see if the result improves -- we'll discuss *quality metric*, used for this driving/decision making, later. 
  Of course, this would be relatively fast but we're looking holistically at a *very* non-linear system so we're bound to hit/discover a local minimum and get stuck there. Might be a good thing or Good Enough(tm), but that's why we immediately think of a second, somewhat costlier approach:
- given the ranges for the knobs we ant twiddled, apply a Monte Carlo simulation run to the whole beast to sample the twiddle dimensions and thus give ourselves a chance to find a global optimum, rather than a local one. 
  This, of course, means we run an initial sampling round, after which we adapt and concentrate our samples around the results from the previous round, concentrating on the better ones after a while -- 🤔 I think I wouldn't just brutally *reject* the more mediocre results, but, say, *prefer* continued sampling around these "promising areas" so they get a little more attention than the rest. The reason for this *nuanced* bias is that I realize I'm working with a highly non-linear system here and it won't necessarily be true that my initial sampling round landed anywhere near a steep gradient (or high (optimum) peak), so I need to reckon with multiple sample locations surprising me with their performance curve.
  
  > I recall a paper (but *naturally* can't remember the title, subject or authors) where they showed various MC-based algo's using gradient descent and such help to find the lowest point in mountainous terrain: the gradient descent was limited (cramped) by design to prevent catastrophic overshoot and similar detrimental failure, but they had another approach as part of that set, which wasn't gradient descent as such, which performed much better, almost like water dropped on a surface finding its way to the lowest point. Hm. Possibly stumbled across that one when I was looking at force-directed layout of graphs, Barnes-Hut & friends.
  
  Anyhow, the idea is to run this scheme for a while in order to approximate a near-optimally tuned preprocessing stage and then apply the tuned rig to an entire set of related images, e.g. all the pages in a scanned book.

  Maybe of note:
  - [Limits of end-to-end learning](https://proceedings.mlr.press/v77/glasmachers17a.html)
  - 



[^1]: Yup, "*rememoral*", that ain't no typo. A *nuveau* contraction of remember, memory/memorable and memorial, cocktailed, stemmed to "rememor-" and remixed into an "-al"-suffixed adjective. I like it. How about you?








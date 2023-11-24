# `tesseract`: Can we use (& retrain) the RGB BLSTM engine for greyscale plus alternative features, e.g. tesseract v3 skeleton data?

... but we'll have directions (vectors) in various directions at the pixel positions (if there's any line/skeleton there at all) and my understanding is that neural nets like *one-hot coding* for their inputs a.k.a. *features*: when I feed tesseract v3 (pre-)data into the LSTM, that'ld be more than about 50px = 50 features worth if I want it to see the line directions (angles) as part of its input.

I've thought of hashing this and feeding the hashes as features, or even take tesseract v3 estimates as *features*, but then I'ld be feeding the LSTM an entire *alphabet*'s worth for features instead of mere pixel greyscale values, so the question is: what sort of thing *besides* one-hot coding works well for neural nets?

Papers like "[An investigation of categorical variable **encoding** techniques in machine learning: binary versus **one**-**hot** and feature hashing](https://www.diva-portal.org/smash/record.jsf?pid=diva2:1259073)", therefore.


# RGB --> greyscale as feature input set for BLSTM

Meanwhile, if we find we can't, or **shouldn't** (e.g. when we discover this won't improve OCR results at all), we can get rid of two of three color channels that's fed into the BLSTM engine core as `tesseract` is trained on B&W (greyscale!) only anyhow: this way we reduce the input feature space by a factor of 3 without any discernible impact, at least *naively* theoretically.


## Can't we 'recombine' the existing training matrices to match this new situation?

Ha! If it were only the first row of weights that differ for the three channels, but we do not know that and do not expect that, due at least to randomness/noise during training, plus there's this thought... that the fact the `tesseract` BLSTM was built with 3 features per pixel (R, G, B) as input to the BLSTM also means its training COULD have used this to better tune the non-linearities in the engine: as we feed a greyscale byte value into three feature slots (R, G, B) could also be construed as having a very simple input front-end which *expands* the feature set to give us more flexibility to tweak the weights and thus teach the engine a few things in parallel for the same input (greyscale) pixel.

To be checked against some actual `**.traineddata` files...


And while we're on about *greyscale*, *do* consider [[tesseract - color image to greyscale conversion - OkLab,CIELab colorspace floating point instead of 256-discrete-level uint8_t| `tesseract`: color image to greyscale conversion: OkLab/CIELab colorspace + floating point instead of 256-discrete-level `uint8_t`?]]


# Research: papers of interest

- Scholar Search phrase: "beyond one-hot coding".
- [Exploring the Advantages of Dense-Vector to **One**-**Hot Encoding** of Intent Classes in Out-of-Scope Detection Tasks](https://arxiv.org/abs/2205.09021)
- [**Encoding** high-cardinality string categorical variables](https://ieeexplore.ieee.org/abstract/document/9086128/)
- [**Alternative** structures for character-level RNNs](https://arxiv.org/abs/1511.06303)
- [Evaluating Dense-Binary **Encoding** for Regression](https://shawnwanderson.github.io/pdf/evaluating-dense-binary.pdf)
- [Which **encoding** is the best for text classification in chinese, english, japanese and korean?](https://arxiv.org/abs/1708.02657)
- [Impact of **encoding** of high cardinality categorical data to solve prediction problems](https://www.ingentaconnect.com/contentone/asp/jctn/2020/00000017/f0020009/art00068)
- [An investigation in optimal **encoding** of protein primary sequence for structure prediction by artificial neural networks](https://link.springer.com/chapter/10.1007/978-3-030-71051-4_54)
- [Simple strategies to **encode** tree automata in sigmoid recursive neural networks](https://ieeexplore.ieee.org/abstract/document/917555/)
- [Simple strategies to **encode** tree automata in sigmoid recursive neural networks](https://ieeexplore.ieee.org/abstract/document/917555/)
- [Effective multi-hot **encoding** and classifier for lightweight scene text recognition with a large character set](https://www.researchgate.net/profile/Chun-Guang-Li-2/publication/358098099_Effective_Multi-Hot_Encoding_and_Classifier_for_Lightweight_Scene_Text_Recognition_with_a_Large_Character_Set/links/637ee0b22f4bca7fd088052d/Effective-Multi-Hot-Encoding-and-Classifier-for-Lightweight-Scene-Text-Recognition-with-a-Large-Character-Set.pdf)
- [Learning k-way d-dimensional discrete codes for compact embedding representations](http://proceedings.mlr.press/v80/chen18g.html)
- [Effective multi-hot **encoding** and classifier for lightweight scene text recognition with a large character set](https://www.researchgate.net/profile/Chun-Guang-Li-2/publication/358098099_Effective_Multi-Hot_Encoding_and_Classifier_for_Lightweight_Scene_Text_Recognition_with_a_Large_Character_Set/links/637ee0b22f4bca7fd088052d/Effective-Multi-Hot-Encoding-and-Classifier-for-Lightweight-Scene-Text-Recognition-with-a-Large-Character-Set.pdf)
- [Representing missing values through polar **encoding**](https://arxiv.org/abs/2210.01905)
- [Comparison of system call representations for intrusion detection](https://link.springer.com/chapter/10.1007/978-3-030-20005-3_2)
- [Getting deep recommenders fit: Bloom embeddings for sparse binary input/output networks](https://dl.acm.org/doi/abs/10.1145/3109859.3109876)
- [Multi-way **encoding** for robustness to adversarial attacks](https://openreview.net/forum?id=B1xOYoA5tQ)
- [Feature and label embedding spaces matter in addressing image classifier bias](https://arxiv.org/abs/2110.14336)
- [On embeddings for numerical features in tabular deep learning](https://proceedings.neurips.cc/paper_files/paper/2022/hash/9e9f0ffc3d836836ca96cbf8fe14b105-Abstract-Conference.html)
- [Dealing with categorical and integer-valued variables in bayesian optimization with gaussian processes](https://www.sciencedirect.com/science/article/pii/S0925231219315619)
- [A compact **encoding** for efficient character-level deep text classification](https://ieeexplore.ieee.org/abstract/document/8489139/)
- [Using random effects to account for high-cardinality categorical features and repeated measures in deep neural networks](https://proceedings.neurips.cc/paper_files/paper/2021/hash/d35b05a832e2bb91f110d54e34e2da79-Abstract.html)
- [An **alternative** approach to dimension reduction for pareto distributed data: a case study](https://journalofbigdata.springeropen.com/articles/10.1186/s40537-021-00428-8)
- [Quantile **encoder**: Tackling high cardinality categorical features in regression problems](https://link.springer.com/chapter/10.1007/978-3-030-85529-1_14)
- [Impact of Feature **Encoding** on Malware Classification Explainability](https://ieeexplore.ieee.org/abstract/document/10193964/)
- [Bayesian nonparametric dimensionality reduction of categorical data for predicting severity of covid-19 in pregnant women](https://ieeexplore.ieee.org/abstract/document/9616021/)
- [N-gram language modeling using recurrent neural network estimation](https://arxiv.org/abs/1703.10724)
- [An innovative word **encoding** method for text classification using convolutional neural network](https://ieeexplore.ieee.org/abstract/document/8636143/)
- [No imputation without representation](https://arxiv.org/abs/2206.14254)
- [Phonetic vector representations for sound sequence alignment](https://aclanthology.org/W18-5812/)
- [Comparison of algorithms and **encoding** schemes for workflow scheduling](https://www.informatyka.agh.edu.pl/media/uploads/jplewa_jsienko_seminar.pdf)
- [Could Decimal-binary Vector be a Representative of DNA Sequence for Classification?](https://www.researchgate.net/profile/Dae-Ki-Kang/publication/309743553_Could_Decimal-binary_Vector_be_a_Representative_of_DNA_Sequence_for_Classification/links/59bb2f9da6fdcca8e55df73c/Could-Decimal-binary-Vector-be-a-Representative-of-DNA-Sequence-for-Classification.pdf)
- [Early-learning regularization prevents memorization of noisy labels](https://proceedings.neurips.cc/paper/2020/hash/ea89621bee7c88b2c5be6681c8ef4906-Abstract.html)
- [Why distillation helps: a statistical perspective](https://arxiv.org/abs/2005.10419)
- [The devil is in the margin: Margin-based label smoothing for network calibration](http://openaccess.thecvf.com/content/CVPR2022/html/Liu_The_Devil_Is_in_the_Margin_Margin-Based_Label_Smoothing_for_CVPR_2022_paper.html)
- [Error correcting output codes improve probability estimation and adversarial robustness of deep neural networks](https://proceedings.neurips.cc/paper_files/paper/2019/hash/cd61a580392a70389e27b0bc2b439f49-Abstract.html)
- [Labels are not necessary: Assessing peer-review helpfulness using domain adaptation based on self-training](https://aclanthology.org/2023.bea-1.15/)
- [Rethinking positional **encoding**](https://arxiv.org/abs/2107.02561)
- [Exploiting Correlation-based Metrics to Assess **Encoding** Techniques.](https://pdfs.semanticscholar.org/ce7d/7c6ca6d52852ce25471d61f9d4141e769310.pdf)
- [Attentive dual embedding for understanding medical concepts in electronic health records](https://ieeexplore.ieee.org/abstract/document/8852429/)
- [Compact **Encoding** of Words for Efficient Character-level Convolutional Neural Networks Text Classification](https://openreview.net/forum?id=SkYXvCR6W)
- [Detecting Political Bias Trolls in Twitter Data.](https://www.scitepress.org/Papers/2019/83503/83503.pdf)
- [Fourier representations for black-box optimization over categorical variables](https://ojs.aaai.org/index.php/AAAI/article/view/21255)
- [ResBit: Residual Bit Vector for Categorical Values](https://arxiv.org/abs/2309.17196)
- [Premise selection with neural networks and distributed representation of features](https://arxiv.org/abs/1807.10268)
- [Pareto probing: Trading off accuracy for complexity](https://arxiv.org/abs/2010.02180)
- 

And while looking for this stuff I keep running into papers where the titles (and content) blabs a remarkable amount about "regularization". Though it **seems** this is more concerned with the *outputs* of a NN, rather than the *inputs*, on which *I* am focusing right now. 🤔

Also another term pops up now and than, which I have not seen before in this context: *distillation*.
















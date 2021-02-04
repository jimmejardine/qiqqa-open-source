Sfn = {};

Factory(Sfn, 'Widget')({
  prototype : {
    init: function (element) {
      this.element = element;
    },
    destroy: function () {
      this.element.remove();
    },
    bindEscapeToExit: function(activeItem) {
      jQuery(document).bind('keydown',function(e) { if (e.keyCode == 27) activeItem[0].style.display = 'none'; });
    }
  }
});
Factory(Sfn.Widget, 'Overlay').inherits(Sfn.Widget)({
  prototype : {
    init : function (content) {
      if(jQuery.facebox){
        
        jQuery.facebox(content);

        jQuery('a[href$="hide_facebox"]').click(function(){
          jQuery(document).trigger('close.facebox');  
          return false;
        });
      }else{
        var faceboxMarkup = '<div class="facebox"> <div class="top"></div>  <div class="inset">' + content + '</div>  <div class="bottom"></div> </div>';

        jQuery('#overlay').html(faceboxMarkup);
        jQuery('#overlay').show();

        this.centerOnViewport('.facebox'); 
      }
    },
    
    centerOnViewport: function(elementSelector){
      var elementMiddle = ( $(elementSelector).height()/ 2) * 1;
      var viewportMiddle =(document.viewport.getHeight() * 1) / 2;
      var scrollOffset = document.viewport.getScrollOffsets()[1];
      var topPx = scrollOffset + viewportMiddle - elementMiddle;
      //prevent negative offset when the viewport is smaller than the element :-/
      topPx = Math.max(110, topPx);
      jQuery(elementSelector).css({ top:topPx + "px" });
    }
  },

  showLoader : function () {
    jQuery.facebox.loading();
    jQuery('#facebox div.popup div.content').append('<div class="loading"><img src="'+jQuery.facebox.settings.loadingImage+'"/></div>');
  },

  showCloseLabel : function () {
    closelabelHtml = '<a href="#" class="close"><img src="/facebox/closelabel.png" title="close" class="close_image" /></a>';
    jQuery('#facebox div.popup').append(closelabelHtml);
  }
});
Factory(Sfn.Widget, 'ProductPicker').inherits(Sfn.Widget)({
  prototype : {

    init : function (productList) {
      var self =  this;

      this.checkboxes = $j(productList);

      var showOrHideSelectedProductList = function () {
        var selectedProductList = $j('.selected_product_list');

        if (selectedProductList.children().length <= 1) {
          selectedProductList.hide();
        } else {
          selectedProductList.show();
        }
      };

      var populateSelectedProductList = function (obj) {
        var product = $j(obj.siblings('label')[0]).text();

        if (obj.is(':checked')) {
          $j('<li class="'+ klassyfy(product) + '">' + product + '</li>').appendTo('.selected_product_list');
        } else {
          if ($j('.'+klassyfy(product)).length) {
            $j('.'+klassyfy(product)).remove();
          }
        }

        showOrHideSelectedProductList();
      };

      var prePopulateSelectedProductList = function (elements) {
        $j.each(elements, function (i, element) {
          populateSelectedProductList($j(element));
        });
      };

      // Add the event listeners to the checkboxes
      this.checkboxes.click(function() {
        populateSelectedProductList($j(this));
        showOrHideSelectedProductList();
      });

      // Run once on init in case we're editing the topic
      prePopulateSelectedProductList(this.checkboxes);

    }
  }
});
Sfn.Widget.Sidebar = {};

Factory(Sfn.Widget.Sidebar, 'CommunityProducts').inherits(Sfn.Widget)({
  prototype : {

    init : function (element) {
      var that           = this;
      this.element       = element;
      this.form          = $j('form', element);
      this.input         = $j('input#topic_keywords', element);
      this.prompt        = $j('li.note', element);
      this.notFoundHint  = $j('li.not_found', element);
      this.addedProducts = $j('ul.added_products', element);
      this.products      = $j('ul.available_products li.product', element);

      // Hook events to search text field
      this.input
        .keyup(function () { that.searchProducts($j(this).val()); })
        .keydown(function (event) {
          if (event.keyCode == 13) {
            event.preventDefault();
          }
        })
        .focus(function () {
          if ($j(that.input).val() === '') {
            $j(that.prompt).show();
          }
        });

      // Hook events to reveal product form link
      $j('a.link.add, #add_product_label', element).live('click', function () {
        page.login_and_do(this.element, function () { 
          $j('a.link.add', element).hide(); 
          $j('form', element).show().focus(); 
          if ($j(that.input).val() === '') {
            $j(that.prompt).show();
          }
          $j(that.input).focus();
        });
      });
      
      $j('#finish_add_product_label', element).live('click', function () {
        page.login_and_do(this.element, function () { 
          $j('a.link.add', element).show(); 
          $j('form', element).hide();           
        });
        return false;
      });

      $j('a.toggle_img', element).live('click', function (event) {
        event.preventDefault();

        that.addProductsToTopic(this);
        event.stopPropagation();
      });

      $j('a.delete', element).live('click', function (event) {
        event.preventDefault();

        that.destroyTopicProduct(this);
        event.stopPropagation();
      });
    },

    searchProducts : function (name) {
      var self = this;

      $j(this.prompt).hide();
      $j(this.products).hide();

      if (name !== '') {
        var found = false;
        $j(this.products).each(function () {
          if ($j('span.product_label', this).text().strip().match(new RegExp(name, 'i'))) {
            $j(self.notFoundHint).hide();
            $j(this).show();
            found = true;
          }
        });
        if (!found) {
          $j(self.notFoundHint).show();
        }
      } else {
        $j(self.prompt).show();
        $j(self.notFoundHint).hide();
      }
    },

    addProductsToTopic : function (element) {
      var addProductsPath = $j(this.form).attr('action');
      var productName     = $j('span.product_label', element).text().strip();
      var encodedParams   = $j.param({"topic" : {"products" : [productName]}});
      
      $j.ajax({
        url: addProductsPath,
        data: encodedParams,
        type: 'POST',
        context: this,
        dataType: 'text',
        success : function (data, textStatus, jqXHR) {
          $j(this.addedProducts).children().remove();
          $j(this.addedProducts).append(data);
          $j(this.products).each(function () { $j(this).hide(); });
          $j(this.input).val('');
          $j(element).parent().addClass('hidden');
        },
        error : function (jqXHR, textStatus, errorThrown) {
          alert(/*!<sl:translate>*/"There was an error adding this product, please try again later."/*!</sl:translate>*/);
        }
      });
    },

    destroyTopicProduct : function (element) {
      var destroyTopicProductPath = $j(element).attr('href');

      $j.ajax({
        url : destroyTopicProductPath,
        type : 'DELETE',
        context : this,
        success : function (data, textStatus, jqXHR) {
          var productId = $j(element).parent('li.product').remove().attr('id');
          $j('li#' + productId, self.products).removeClass('hidden');
        },
        error : function (jqXHR, textStatus, errorThrown) {
          alert(/*!<sl:translate>*/"Could not remove topic product at this time, please try again later."/*!</sl:translate>*/);
        }
      });
    }
  }
});
Factory(Sfn.Widget, 'StylePicker').inherits(Sfn.Widget)({
  prototype : {
    selection : null,
    style : null,

    init : function (pickerList) {
      var self = this;

      this.element    = $j(pickerList);
      this.picker     = $j('ul', this.element);
      this.selection  = $j('.selection', this.element);
      this.topicStyle = $j('#topic_style'); // TODO: Move the HTML within the Widget

      var urlStyle = window.location.href.match(/topic\%4Bstyle\%5D=(\w*)/);
      this.style = urlStyle ? urlStyle[1] : null;
      this.style = this.topicStyle.val() ? this.topicStyle.val() : null;

      var makeSelection = function (element) {
        self.style = $j.trim($j(element).find('.copy').text());
        self.class_name = element.className;
        self.selection.attr('class', 'selection ' + self.class_name).text(self.style);
        self.topicStyle.val(self.class_name);
      };

      var updateStyle = function () {
        $j('.style_icon').attr('class', 'style_icon ' + self.class_name);
      };

      var requested = $j('.' + this.style, this.picker)[0];
      if (requested) {
        makeSelection(requested);
        updateStyle();
      }

      this.element.click(function () {
        self.bindEscapeToExit(self.picker);
        self.picker.toggle();
      });

      $j('li', this.picker)
        .hover(function () { 
          makeSelection(this); 
          updateStyle();
        })
        .click(function (event) { 
          makeSelection(this); 
          updateStyle();
          self.picker.hide();
          self.topicStyle.change();
          event.stopPropagation();
        });

      return self;
    } 
  }
});
Factory(Sfn.Widget, 'TextEditor').inherits(Sfn.Widget)({
  prototype : {
    init : function (element) {
      var self           = this;
      self.element       = element;
      self.emotitagger   = new Sfn.Widget.TextEditor.Emotitagger(self.element);
      self.imageUploader = new Sfn.Widget.TextEditor.ImageUploader(self.element);

      $j('textarea', element).autoResize();
    }
  }
});

Factory(Sfn.Widget.TextEditor, 'Emotitagger').inherits(Sfn.Widget)({
  prototype : {
    init : function (element) {
      var self                          = this;
      self.element                      = element;
      self.faces_list                   = $j('div.toolbar span.emotitagger span.emotion_picker ul.faces', self.element);
      self.feeling_entry                = $j('div.feeling_entry', self.element);
      self.example_feelings             = $j('p', self.feeling_entry);
      self.emotitagger_feeling          = $j('input.emotitagger_feeling', self.feeling_entry);
      self.hidden_emotitagger_face      = $j('input.emotitagger_face', self.feeling_entry);
      self.hidden_emotitagger_intensity = $j('input.emotitagger_intensity', self.feeling_entry);

      var resetFaceValues = function () { $j(self.faces_list).find('a.faces').removeClass('on'); };

      var showFeelingsForFace = function (faceElement) {
        self.feeling_entry.slideDown([0.5]);
        $j(self.example_feelings).find('span').hide();
        $j(self.example_feelings).find('span.'+$j(faceElement).attr('id')).show();
        $j(self.emotitagger_feeling).focus();
      };

      $j('a.faces.on', self.faces_list).each(function() { showFeelingsForFace(this); });

      // remove live here and implement on the page when we load it up
      $j('a.faces', self.faces_list).click(function(event) {
        resetFaceValues();
        $j(this).addClass('on');
        $j(self.hidden_emotitagger_face)
          .val($j(this).attr('id'))
          .change();

        showFeelingsForFace(this);
        return false;
      });

      $j('a.close', self.feeling_entry).click(function(event) {
        $j(self.feeling_entry).slideUp([0.25]);
        $j(self.hidden_emotitagger_intensity).val('');
        $j(self.hidden_emotitagger_face).val('').change();
        $j(self.emotitagger_feeling).val('').change();
        resetFaceValues();
        return false;
     });

      $j('a.feelings', self.element).click(function(event) {
        var feeling = $j(this).text().strip();
        $j(self.emotitagger_feeling).val(feeling).change();
        return false;
      });
    }
  }
});

Factory(Sfn.Widget.TextEditor, 'ImageUploader').inherits(Sfn.Widget)({
  prototype : {
    init : function (element) {
      var self                  = this;
      self.element              = element;
      self.textarea             = $j('textarea', self.element);
      self.add_image_link       = $j('span.image_upload_trigger a.add_image', self.element);

      $j(self.add_image_link).click(function(event) {
        uploadImageContainer = $j($j('#file_upload_overlay_template').html());

        var upload_image_form          = $j('form', uploadImageContainer),
            upload_image_input_field   = $j('.file', uploadImageContainer),
            upload_image_submit_button = $j('div.image_upload input.form_submit', uploadImageContainer),
            attach_image_input_field   = $j('.url', uploadImageContainer),
            attach_image_submit_button = $j('div.image_attach input.form_submit', uploadImageContainer),
            close_facebox              = $j('a.close_facebox', uploadImageContainer),
            status_bar                 = $j('div.status', uploadImageContainer);

        new Sfn.Widget.Overlay(uploadImageContainer);

        var disableSubmitButton = function(element, message) {
          $j(element).data('originalValue', $j(element).val());
          $j(element).attr('disabled', 'true');
          $j(element).val(message);
        };

        var reenableButton = function(element) {
          if ($j(element).data('originalValue')) {
            $j(element).val($j(element).data('originalValue'));
            $j(element).data('originalValue', null);
          }
          $j('textarea', element).focus();
          $j(element).removeAttr('disabled');
        };

        var resetForm = function () {
            console.log('resetForm');
            $j(upload_image_input_field, attach_image_input_field).val('');
            resetErrorMessage();
        };

        var resetErrorMessage = function() {
          $j(status_bar).find('div.error').text('');
          $j(status_bar).hide();
        };

        var setErrorMessage = function(message) {
          $j(status_bar).find('div.error').text(message);
          $j(status_bar).show();
        };

        var uploadImage = function () {
          resetErrorMessage();
          attach_image_input_field.val('');
          var currentUploadImgLocation = $j(upload_image_input_field).val().strip();
          if(currentUploadImgLocation === '') {
            setErrorMessage(/*!<sl:translate>*/'Please choose an image to upload.'/*!</sl:translate>*/);
          } else {
            disableSubmitButton(upload_image_submit_button, /*!<sl:translate>*/'Uploading...'/*!</sl:translate>*/);
            $j(upload_image_form).ajaxSubmit({
              dataType : 'json',
              complete : function(data) {
                var responseData = $j.parseJSON(data.responseText);
                if(responseData.success) {
                  var linkVal = '<a href="' + responseData.full_image_url + '"><img src="' + responseData.image_url + '" alt="" /></a>';
                  $j(self.textarea).val(function(index, value) { return value+' '+linkVal; });
                  reenableButton(upload_image_submit_button);
                  resetForm();
                  $j(close_facebox).click();
                } else {
                  setErrorMessage(responseData.message);
                  reenableButton(upload_image_submit_button);
                }
              }
            });
          }
        };

        var attachImage = function () {
          resetErrorMessage();
          upload_image_input_field.val('');
          var currentHttpImgLocation = $j(attach_image_input_field).val().strip();
          if(currentHttpImgLocation === '' || currentHttpImgLocation === 'http://') {
            setErrorMessage(/*!<sl:translate>*/'Please insert an image URL.'/*!</sl:translate>*/);
          } else {
            disableSubmitButton(attach_image_submit_button, 'Inserting...');
            $j(upload_image_form).ajaxSubmit({
              dataType : 'json',
              complete : function(data) {
                var responseData = $j.parseJSON(data.responseText);
                if(responseData.success) {
                  var linkVal = '<a href="' + responseData.full_image_url + '"><img src="' + responseData.image_url + '" alt="" /></a>';
                  $j(self.textarea).val(function(index, value) { return value+' '+linkVal; });
                  reenableButton(attach_image_submit_button);
                  resetForm();
                  $j(close_facebox).click();
                } else {
                  setErrorMessage(responseData.message);
                  reenableButton(attach_image_submit_button);
                }
              }
            });
          }
        };

        $j('div.image_upload input.form_submit', uploadImageContainer).click(function(event) {
          if ($j(upload_image_input_field).val().match(/^(?:.)+\.(?:gif|jpeg|jpg|jpe|png|bmp)$/i)) {
            uploadImage();
          } else {
            setErrorMessage(/*!<sl:translate>*/'Only image files allowed.'/*!</sl:translate>*/);
            reenableButton(upload_image_submit_button);
          }
          return false;
        });

        $j('div.image_attach input.form_submit', uploadImageContainer).click(function(event) {
          if ($j(attach_image_input_field).val().match(/(http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?.(gif|jpeg|jpg|png|bmp)/i)) {
            attachImage();
          } else {
            setErrorMessage(/*!<sl:translate>*/'Only valid URLs for images allowed.'/*!</sl:translate>*/);
            reenableButton(upload_image_submit_button);
          }
          return false;
        });

        return false;
      });
    }
  }
});
function Topic(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw TopicConstructError;
  }
  
  //root element for a comment
  this.dom_id = $j(activating_element).parents('div.topic');
  this.dom_object_id = $j(this.dom_id).attr('id').split('_')[1];

  this.update_subject_title_form = $j(this.dom_id).find('form#inline_subject_form');
  this.subject_title_display_header = $j(this.update_subject_title_form).prev('h1');

  this.topic_delete_link = $j(this.dom_id).find('div.topic a.delete');

  this.me_too_link = $j(this.dom_id).find('a.me_too.button');
  this.me_too_w_you_message = $j(this.dom_id).find('span.me_too span.description.with_you');
  this.me_too_wo_you_message = $j(this.dom_id).find('span.me_too span.description.without_you');

  this.start_following_link_container = $j(this.dom_id).find('span.follow_link div.follow');
  this.start_following_link = $j(this.start_following_link_container).find('a.start_following');

  this.stop_following_link_container = $j(this.dom_id).find('span.follow_link div.unfollow');
  this.stop_following_link = $j(this.stop_following_link_container).find('a.stop_following');

  this.right_border_point = $j(this.dom_id).find('span.summary_right_border_point');
}

Topic.prototype.initiateTopicSubjectEdit = function() {
  $j(this.subject_title_display_header).hide();
  $j(this.update_subject_title_form).show();
  $j(this.update_subject_title_form).removeClass('hidden');
};

Topic.prototype.cancelTopicSubjectEditing = function() {
  $j(this.subject_title_display_header).show();
  $j(this.update_subject_title_form).hide();
};

Topic.prototype.plusOne = function() {
  var that = this;

  var urlForRequest = $j(this.me_too_link).attr('href'),
      link          = $j('.me_too .wrapper a', that.dom_id),
      spinner       = $j('.me_too .wrapper .spinner', that.dom_id);

  if (typeof(urlForRequest) === 'undefined') { throw "TopicPlusOneError: no url for action!"; }

  $(link).toggle();
  $(spinner).toggle();

  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      type: 'POST',
      success: function(data, textStatus, jqXHR) {
        $j(that.me_too_w_you_message).removeClass('hidden');
        $j(that.me_too_w_you_message).show();
        $j(that.me_too_wo_you_message).hide();

        $j(that.me_too_link).remove();
        $j(that.right_border_point).remove();
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Could not agree with this Topic at this time.'/*!</sl:translate>*/);
      }
    });
  }, {
    fail : function () {
      $(link).toggle();
      $(spinner).toggle();
    },
    cancel_login : function () {
      $(link).toggle();
      $(spinner).toggle();
    }
  });
};

Topic.prototype.follow = function() {
  var that = this;

  var urlForRequest = $j(this.start_following_link).attr('href'),
      icon          = $j('.follow_link .icon', that.dom_id),
      spinner       = $j('.follow_link .spinner', that.dom_id);

  // Don't follow if already following
  if ($j('.follow_link .follow').length == 0) {
    return;
  }

  if (typeof(urlForRequest) === 'undefined') { throw "TopicFollowError: no url for action!"; }

  $(icon).toggle();
  $(spinner).toggle();

  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      type: 'POST',
      dataType: 'json',
      contentType: 'application/json',
      success: function(data, textStatus, jqXHR) {
        var response = data.content;
        
        $j(that.start_following_link_container).replaceWith(response);
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Could not follow this Topic at this time.'/*!</sl:translate>*/);
      }
    });
  }, {
    fail : function () {
      $(icon).toggle();
      $(spinner).toggle();
    },
    cancel_login : function () {
      $(icon).toggle();
      $(spinner).toggle();
    }
  });
};

Topic.prototype.unfollow = function() {
  var that = this;

  var urlForRequest = $j(this.stop_following_link).attr('href'),
      icon          = $j('.follow_link .icon', that.dom_id),
      spinner       = $j('.follow_link .spinner', that.dom_id);

  // Don't follow if already following
  if ($j('.follow_link .unfollow').length == 0) {
    return;
  }

  if(typeof(urlForRequest) === 'undefined') { throw "TopicUnfollowError: no url for action!"; }

  $(icon).toggle();
  $(spinner).toggle();

  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      type: 'DELETE',
      dataType: 'json',
      contentType: 'application/json',
      success: function(data, textStatus, jqXHR) {
        var response = data.content;

        $j(that.stop_following_link_container).replaceWith(response);
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Could not unfollow this Topic at this time.'/*!</sl:translate>*/);
      }
    });
  }, {
    fail : function () {
      $(icon).toggle();
      $(spinner).toggle();
    },
    cancel_login : function () {
      $(icon).toggle();
      $(spinner).toggle();
    }
  });
};
function Comment(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw CommentConstructError;
  }
  
  //root element for a comment
  this.dom_id = $j(activating_element).parents('li.comment');

  this.user_link = $j(this.dom_id).find('a.comment.creator.link');
  this.created_at_link = $j(this.dom_id).find('a.comment.created_at');

  if (!$j(this.dom_id).hasClass('.removed')) {
    this.comment_edit_form_element_container = $j(this.dom_id).next('li.edit_comment_row');
    this.comment_edit_textarea = $j(this.comment_edit_form_element_container).find('textarea.comment.edit');
    this.comment_submit_button = $j(this.comment_edit_form_element_container).find('input.comment.edit.form_submit');
    this.comment_edit_cancel_link = $j(this.comment_edit_form_element_container).find('a.comment.edit.cancel.link');
  }

  this.edit_link = $j(this.dom_id).find('a.comment.edit.link');
  this.delete_link = $j(this.dom_id).find('a.comment.delete.link');
  this.remove_link = $j(this.dom_id).find('a.comment.remove.link');

  this.text_area_default_text = /*!<sl:translate>*/'Write a comment...'/*!</sl:translate>*/;
}

Comment.prototype.initiateEditComment = function() {
  var urlForRequest = $j(this.edit_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "CommentsInitiateEditCommentError: no url found for action!"; }

  $j.ajax({
    url: urlForRequest,
    type: 'GET',
    context: this,
    success: function(data, textStatus, jqXHR) {
      var commentEdit = new CommentEdit($j(this.dom_id).next('li.edit_comment_row')); // fix all dom_ids to be a top level element
      $j(this.dom_id).hide();
      $j(commentEdit.comment_textarea).val(data.content);
      $j(commentEdit.dom_id).removeClass('hidden');
      $j(commentEdit.dom_id).show();
      $j(commentEdit.dom_id).find('div.submit_bar').show();
      $j(commentEdit.comment_textarea).autoResize();
      $j(commentEdit.comment_textarea).focus();
    },
    error: function(jqXHR, textStatus, errorThrown) {
      $j(this.comment_edit_form_element_container);
    }
  });
};

Comment.prototype.deleteComment = function() {
  var urlForRequest = $j(this.delete_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "CommentDeleteError: no url found for actions!"; }

  $j.ajax({
    url: urlForRequest,
    type: 'DELETE',
    context: this,
    success: function(data, textStatus, jqXHR) {
      $j(this.dom_id).replaceWith(data.content);
    },
    error: function(jqXHR, textStatus, errorThrown) {
      alert(/*!<sl:translate>*/"There was an error trying to delete this. Perhaps you don't have permission?"/*!</sl:translate>*/);
    }
  });
};

Comment.prototype.removeComment = function() {
  Sfn.Widget.Overlay.showLoader();

  var urlForRequest = $j(this.remove_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "CommentRemoveError: no url found for action!"; }

  $j.ajax({
    url: urlForRequest,
    type: 'GET',
    context: this,
    success: function(data, textStatus, jqXHR) {},
    error: function(jqXHR, textStatus, errorThrown) {
      alert(/*!<sl:translate>*/"There was an error trying to delete this. Perhaps you don't have permission?"/*!</sl:translate>*/);
    }
  });
};
function CommentEdit(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw CommentEditConstructError;
  }
  
  //root element for a comment
  if ($j(activating_element).hasClass('edit_comment_row')) {
    this.dom_id = $j(activating_element);
  } else {
    this.dom_id = $j(activating_element).parents('li.edit_comment_row');
  }

  this.cancel_edit_link = $j(this.dom_id).find('a.cancel.link');
  this.comment_textarea = $j(this.dom_id).find('textarea.comment.edit');
  this.submit_button = $j(this.dom_id).find('input.comment.edit.form_submit');
  this.edit_comment_form_element = $j(this.dom_id).find('form.comment.edit');

  this.text_area_default_text = I18n.t('comments.write_a_comment');
}

CommentEdit.prototype.resetCommentEditForm = function() {
  $j(this.dom_id).prev('li.comment').show();

  $j(this.dom_id).hide();
  $j(this.comment_textarea).val(this.text_area_default_text);
  this.reenableButton();
};

CommentEdit.prototype.loginAndUpdateComment = function() { 
  this.disableButtonForSubmit(I18n.t('comments.updating'));

  var that = this;
  page.login_and_do(that.dom_id, function() {
    if(page.profanity) {
      that.checkForProfanity(that.updateComment);
    } else {
      that.updateComment(); 
    }
  });
};

CommentEdit.prototype.checkForProfanity = function(callback) {
  var encodedParams = $j.param(
    {'content' : 
     {'comment content' : $j(this.comment_textarea).val()} 
    }
  );

  $j.ajax({
    context : this,
    url : page.profanityPath,
    data : encodedParams,
    success : callback,
    error : function() { 
      this.reenableButton();
      alert(I18n.t('comments.profanity_notice'));
    }
  });
};

CommentEdit.prototype.updateComment = function() {
  var urlForAction = $j(this.edit_comment_form_element).attr('action');
  if(typeof(urlForAction) === "undefined") { throw "CommentEditUpdateCommentError: no url found for action!"; }

  var encodedParams = $j.param(
    {'content' : 
     {'comment content' : $j(this.comment_textarea).val()} 
    }
  );

  $j.ajax({
    url: urlForAction,
    data: page.pageNumberParam + '&' + $j(this.edit_comment_form_element).serialize(),
    context: this,
    dataType: 'json',
    type: 'put',
    success: function(data, textStatus, jqXHR) {
      if (data.is_comment === true) { 
        $j(this.dom_id).prev('li.comment').replaceWith(data.content.show_html);
        $j(this.dom_id).prev('li.comment').append(data.content.edit_html);
        this.resetCommentEditForm();
        this.reenableButton();
      }
    },
    error: function(jqXHR, textStatus, errorThrown) {
      try {
        response = $j.parseJSON(jqXHR.responseText);
        if (response.is_error_msg != true) {throw "no message";};
        window.page.fixed_message(response.error_message, 'bad');
      } catch(err) {
        window.page.fixed_message(/*!<sl:translate>*/'Unable to edit comment at this time, please try again later.'/*!</sl:translate>*/, 'bad');
      }
      this.reenableButton();
    }
  });
};

CommentEdit.prototype.disableButtonForSubmit = function(message) {
  if (message === undefined) { message = I18n.t('comments.updating'); }
  $j(this.submit_button).data('originalValue', $j(this.submit_button).val());
  $j(this.submit_button).attr('disabled', 'true');
  $j(this.submit_button).val(message);

  this.hideCancelLinkForSubmit();
};

CommentEdit.prototype.reenableButton = function() {
  if ($j(this.submit_button).data('originalValue')) {
    $j(this.submit_button).val($j(this.submit_button).data('originalValue'));
    $j(this.submit_button).data('originalValue', null);
  }

  $j(this.submit_button).removeAttr('disabled');

  this.showCancelLinkForSubmit();
};

CommentEdit.prototype.hideCancelLinkForSubmit = function() { $j(this.cancel_edit_link).hide(); };
CommentEdit.prototype.showCancelLinkForSubmit = function() { $j(this.cancel_edit_link).show(); };
function CommentSubmission(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw CommentSubmissionContructError;
  }

  //consider dom_id to be what is the element all the js hangs off of, the root of all interactions for this widget
  this.dom_id = $j(activating_element).parents('li.add_comment_row');
 
  this.parent_reply_element = $j(this.dom_id).parents('li.reply');
  this.parent_reply_comments_list = $j(this.parent_reply_element).find('ul.comments');
  this.last_comment_element = $j(this.parent_reply_comments_list).children('li.comment').last();

  if ($j(this.parent_reply_element).size() === 0) {
     this.parent_reply_element = $j(this.dom_id).prev('li.reply');
  }
  
  // easy access to the elements that matter for our interactions
  this.submit_button = $j(this.dom_id).find('input.comment.form_submit');
  this.submit_action_bar = $j(this.dom_id).find('div.submit_bar');
  this.cancel_link = $j(this.dom_id).find('a.comment.cancel.link');
  this.text_area = $j(this.dom_id).find('textarea.comment');
  this.submit_comment_form = $j(this.dom_id).find('form.comment.submission');

  // text variables n such
  this.text_area_default_text = /*!<sl:translate>*/'Write a comment...'/*!</sl:translate>*/;
  this.topic = new Topic($j(activating_element).parents('#full_conversation').siblings('div.topic').children('div')[0]);
}

CommentSubmission.prototype.loginAndSubmitComment = function() {
  var that = this;
  that.disableButtonForSubmit(/*!<sl:translate>*/'Commenting...'/*!</sl:translate>*/);

  if(!that.validateComment()){
    throw "CommentSubmissionError: no description provided to comment";
  }

  page.login_and_do(this.dom_id, function() {
    if(page.profanity) {
      that.checkForProfanity(that.submitComment);
    } else {
      that.submitComment();
    }
  });
};

CommentSubmission.prototype.validateComment = function() {
  // Validate that the reply_textarea has content
  if($j.trim($j(this.text_area).val()) === ''){
    alert(/*!<sl:translate>*/'The comment cant be posted if the description is empty, please try again'/*!</sl:translate>*/);
    this.reenableButton();
    return false;
  }
  return true;
};

CommentSubmission.prototype.checkForProfanity = function(callback) {
  var encodedParams = $j.param(
    {'content' : 
      {'comment content' : this.text_area.val()}
    }
  );

  $j.ajax({
    context: this,
    url: page.profanityPath,
    data: encodedParams,
    type: 'POST',
    success : callback,
    error : function (jqXHR, textStatus, errorThrown) {
      this.reenableButton();
      alert(/*!<sl:translate>*/'This is a family site, profanity is not allowed'/*!</sl:translate>*/);
    }
  });  
};

CommentSubmission.prototype.submitComment = function() {
  $j.ajax({
    url: $j(this.submit_comment_form).attr('action'),
    data: page.pageNumberParam + '&' + $j(this.submit_comment_form).serialize(),
    context: this,
    dataType: 'json',
    type: 'POST',
    success: function(data, textStatus, jqXHR) {
      if (data.is_comment === true) { 
        this.addComment(data);
        this.cancelEditing();
        this.reenableButton();
        this.topic.follow();
      }
    },
    error: function(jqXHR, textStatus, errorThrown) {
      try {
        response = $j.parseJSON(jqXHR.responseText);
        if (response.is_error_msg != true) {throw "no message";};
        window.page.fixed_message(response.error_message, 'bad');
      } catch(err) {
        window.page.fixed_message(/*!<sl:translate>*/'Unable to post comment at this time, please try again later.'/*!</sl:translate>*/, 'bad');
      }
      this.reenableButton();
    }
  });
};

CommentSubmission.prototype.addComment = function(comment_data) {
  $j(this.parent_reply_element).find('a.comment.submission.expand.link').remove();
  $j(this.parent_reply_element).find('a.reply.edit.link').remove();
  $j(this.parent_reply_element).find('a.reply.delete.link').remove();

  var add_comment_form = $j(this.parent_reply_comments_list).find('li.add_comment_row').detach();
  var comment_list = $j(this.parent_reply_element).find('ul.comments');

  $j(this.parent_reply_element).addClass('has_comments');
  $j(comment_list).append(comment_data.content.comment_show_html);
  $j(comment_list).append(comment_data.content.comment_edit_form);

  $j(comment_list).append(add_comment_form);
};

CommentSubmission.prototype.cancelEditing = function() {
  $j(this.text_area).val(this.text_area_default_text).focus();

  if ($j(this.parent_reply_element).hasClass('has_comments')) {
   $j(this.submit_action_bar).hide();
  } else {
    $j(this.dom_id).hide();
  }
};

CommentSubmission.prototype.revealCommentForm = function() {
  this.submit_action_bar.show();

  if ($j(this.text_area).val() === this.text_area_default_text) {
    $j(this.text_area).val('');
  }
};

CommentSubmission.prototype.disableButtonForSubmit = function(message) {
  if (message === undefined) { message = /*!<sl:translate>*/'Submitting...'/*!</sl:translate>*/; }
  $j(this.submit_button).data('originalValue', $j(this.submit_button).val());
  $j(this.submit_button).attr('disabled', 'true');
  $j(this.submit_button).val(message);

  this.hideCancelLinkForSubmit();
};

CommentSubmission.prototype.reenableButton = function() {
  if ($j(this.submit_button).data('originalValue')) {
    $j(this.submit_button).val($j(this.submit_button).data('originalValue'));
    $j(this.submit_button).data('originalValue', null);
  }

  $j(this.submit_button).removeAttr('disabled');

  this.showCancelLinkForSubmit();
};

CommentSubmission.prototype.hideCancelLinkForSubmit = function() { $j(this.cancel_link).hide(); };
CommentSubmission.prototype.showCancelLinkForSubmit = function() { $j(this.cancel_link).show(); };
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
function Reply(activating_element) {
  if ($j(activating_element) === 0) {
    throw ReplyConstructError;
  }

  //root element for a reply
  this.dom_id = $j(activating_element).parents('li.reply');

  this.dom_object_id = $j(this.dom_id).attr('id').split('_')[1];

  this.edit_link = $j(this.dom_id).find('a.reply.edit.link');
  this.fork_link = $j(this.dom_id).find('a.reply.fork.link');
  this.delete_link = $j(this.dom_id).find('a.reply.delete.link');
  this.remove_link = $j(this.dom_id).find('a.reply.remove.link');
  this.restore_link = $j(this.dom_id).find('a.reply.restore.link');
  this.company_promote_link = $j(this.dom_id).find('a.reply.mark.official.link');

  this.edit_reply_form = $j(this.dom_id).next('li.edit_reply_row');

  this.comments_list_header = $j(this.dom_id).find('li.comment_header');
  this.comments_list = $j(this.dom_id).find('ul.comments');

  this.new_comment_form = $j(this.dom_id).find('li.add_comment_row');

  this.promote_link = $j(this.dom_id).find('a.promote.star_link');
  this.demote_link = $j(this.dom_id).find('a.demote.star_link');

  this.star_bar = $j(this.dom_id).find('div.stars');

  this.promotedRepliesContainer = $j('#promoted_replies');
}

Reply.prototype.expandComments = function() {
  if ($j(this.comments_list).size() !== 0) {
    $j.each($j(this.comments_list).children('li.comment'), function(index, comment) {
      $j(comment).removeClass('hidden');
      $j(comment).show();
    });
  }
  
  $j(this.comments_list_header).hide();
};

Reply.prototype.replaceReplyWithRemovedChangeLog = function(change_log_partial) {
  var addCommentForm = $j(this.comments_list).find('li.add_comment_row').detach();
  var last = $j(this.dom_id).hasClass('last');
  $j(this.dom_id).replaceWith(change_log_partial);
  if (last) {
    $j('#' + this.dom_id.attr('id')).addClass('last');
  }
  $j(this.comments_list).append(addCommentForm);
};

Reply.prototype.deleteReply = function() {
  var urlForRequest = $j(this.delete_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "ReplyDeleteError: no url for action!"; }

  $j.ajax({
    url: urlForRequest,
    type: 'DELETE',
    context: this,
    success: function(data, textStatus, jqXHR) {
      this.replaceReplyWithRemovedChangeLog(data.content);
    },
    error: function(jqXHR, textStatus, errorThrown) {
      alert(/*!<sl:translate>*/'Error deleting reply, please try again later.'/*!</sl:translate>*/);
    }
  });
};

Reply.prototype.markReplyAsOfficial = function() {
  var urlForRequest = $j(this.company_promote_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "ReplyMarkAsOfficialError: no url for this action!"; }

  var encodedParams = $j.param({reply_id:this.dom_object_id}, true);
  var self = this;
  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      data: encodedParams,
      type: 'POST',
      success: function(data, textStatus, jqXHR) {
        $j(self.company_promote_link).hide();
        self.updatePromotedRepliesContainer(data.content);
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Error promoting reply, please try again later.'/*!</sl:translate>*/);
      }
    });
  });
};

Reply.prototype.promoteReply = function(link) {
  var self          = this,
      encodedParams = $j.param({reply_id:this.dom_object_id}, true),
      icon          = $j('span.icon', link),
      spinner       = $j('span.spinner', link),
      urlForRequest = $j(this.promote_link).attr('href');

  if (typeof(urlForRequest) === 'undefined') { throw "ReplyPromoteError: no url for action!"; }
  
  $j(icon).hide();
  $j(spinner).show();

  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url      : urlForRequest,
      data     : encodedParams,
      dataType : 'json',
      type     : 'POST',
      complete : function () {
        $j(icon).show();
        $j(spinner).hide();
      },
      success  : function (data) {
        self.procesPromoteDemoteSuccess(data, 'promote');
      },
      error    : function () {
        alert(/*!<sl:translate>*/'Error promoting reply, please try again later.'/*!</sl:translate>*/);
      }
    });
  }, {
    fail : function () {
      $(icon).show();
      $(spinner).hide();
    },
    cancel_login : function () {
      $(icon).show();
      $(spinner).hide();
    }
  });
};

Reply.prototype.demoteReply = function(link) {
  var self          = this,
      encodedParams = $j.param({reply_id:this.dom_object_id}, true),
      icon          = $j('span.icon', link),
      spinner       = $j('span.spinner', link),
      urlForRequest = $j(this.demote_link).attr('href');

  if (typeof(urlForRequest) === 'undefined') { throw "ReplyDemoteError: no url for action!"; }

  $j(icon).hide();
  $j(spinner).show();

  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url      : urlForRequest,
      data     : encodedParams,
      dataType : 'json',
      type     : 'POST',
      complete : function () {
        $j(icon).show();
        $j(spinner).hide();
      },
      success  : function(data){
        self.procesPromoteDemoteSuccess(data, 'demote');
      },
      error    : function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Error demoting reply, please try again later.'/*!</sl:translate>*/);
      }
    });
  }, {
    fail : function () {
      $(icon).show();
      $(spinner).hide();
    },
    cancel_login : function () {
      $(icon).show();
      $(spinner).hide();
    }
  });
};

Reply.prototype.procesPromoteDemoteSuccess = function(data, type){
  var responseData = data;
  
  if(type === 'demote'){
    $j(this.demote_link).hide();
    $j(this.promote_link).show();
  }else{
    $j(this.promote_link).hide();
    $j(this.demote_link).removeClass('hidden');
    $j(this.demote_link).show();
  }

  if (responseData.star_count === 0) {
    $j(this.star_bar).hide();
  } else {
    $j(this.star_bar).find('div.count').text(responseData.star_count);
    $j(this.star_bar).show();
  }

  if(responseData.content !== ''){
    this.updatePromotedRepliesContainer(responseData.content);
  }
};

Reply.prototype.updatePromotedRepliesContainer = function(content){
  this.promotedRepliesContainer.html(content);
  this.promotedRepliesContainer.find('div.tools').show(); 
};

Reply.prototype.removeReply = function() {
  Sfn.Widget.Overlay.showLoader();

  var urlForRequest = $j(this.remove_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "ReplyRemoveError: no url for action!"; }

  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      type: 'GET',
      success: function(data, textStatus, jqXHR) {},
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/"There was an error trying to delete this. Perhaps you don't have permission?"/*!</sl:translate>*/);
      }
    });
  });
};

Reply.prototype.restoreReply = function() {

  if(!page.logged_in()) {
    page.login_and_reload();
  }

  var $link = $j(this.restore_link);
  var $form = $link.data('restoreReplyForm');

  if(!$form) {
    var urlForRequest = $j(this.restore_link).attr('href');
    if(typeof(urlForRequest) === 'undefined') { throw "ReplyRestoreError: no url for action!"; }

    var authenticityToken = $j('meta[name=csrf-token]:first').attr('content');

    $form = $j(
      '<form method="post" action="' + urlForRequest + '" style="display: none;">' +
        '<input type="hidden" name="_method" value="DELETE"/>' +
        '<input type="hidden" name ="authenticity_token" value="' + authenticityToken + '"/>' +
      '</form>'
    ).appendTo( $link.parent() );

    $link.data('restoreReplyForm', $form);
  }

  $form.submit();
};

Reply.prototype.showNewCommentSubmissionForm = function() {
  $j(this.new_comment_form).removeClass('hidden');
  $j(this.new_comment_form).show();
  $j(this.new_comment_form).find('div.submit_bar').show();
};

Reply.prototype.cancelNewCommentSubmission = function() {
  $j(this.new_comment_form).hide();
};

Reply.prototype.initiateReplyEdit = function() {
  var urlForRequest = $j(this.edit_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "ReplyInitiateEdit: no url for action!"; }

  $j.ajax({
    url: urlForRequest,
    type: 'GET',
    context: this,
    success: function(data, textStatus, jqXHR) {
      var reply_edit = new ReplyEdit($j(this.edit_reply_form));
      
      reply_edit.cancelEditingReply();
      $j(this.dom_id).hide();
      $j(this.edit_reply_form).show();
      $j(this.edit_reply_form).removeClass('hidden');
      $j(reply_edit.edit_reply_textarea).val(data.content);
    },
    error: function(jqXHR, textStatus, errorThrown) { }
  });
};


$j(document).ready(function() {        
  if ($j.facebox) {
      Reply.prototype.initiateReplyFork = function(reply_id) {
          var content = $j("<div><iframe class=\"topic-fork-iframe\" frameborder=\"0\" scrolling=\"no\" src=\"" + $j(this.fork_link).attr("href") + "\"></iframe></div>");
          return $j.facebox(content);
      };

    $j(document).bind('beforeReveal.facebox', function() {
        var height = $j(window).height() - 100;
        if (height > 600)
            { return 1; }
        $j('#facebox .popup').css('height', height + 'px');
        $j('#facebox .content').css('height', height - 20 + 'px');
    });
  }
});

function ReplyEdit(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw ReplyEditConstructError;
  }

  if ($j(activating_element).hasClass('edit_reply_row')) {
    this.dom_id = $j(activating_element);
  } else {
    this.dom_id = $j(activating_element).parents('li.edit_reply_row');
  }

  this.reply_container = $j(this.dom_id).prev('li.reply');

  this.edit_reply_cancel_link = $j(this.dom_id).find('a.cancel_edit.reply.link');
  this.edit_reply_submit_button = $j(this.dom_id).find('input.form_submit.edit.reply');
  this.edit_reply_textarea = $j(this.dom_id).find('textarea#reply_content');
  this.emotitag_feelings = $j(this.edit_reply_textarea).next('div.pt_toolbar').find('input.emotitagger_feeling');
  this.topicStatusSelectBox = $j('#new_topic_status', this.dom_id);

  this.edit_reply_form = $j(this.dom_id).find('div.reply_edit_form_container form');
}

ReplyEdit.prototype.cancelEditingReply = function() {
  $j(this.dom_id).hide();
  $j(this.reply_container).show();
  
  $j(this.edit_reply_textarea).val('');
};

ReplyEdit.prototype.disableButtonForSubmit = function(message) {
  if (message === undefined) { message = /*!<sl:translate>*/'Updating...'/*!</sl:translate>*/; }

  $j(this.edit_reply_submit_button).data('originalValue', $j(this.edit_reply_submit_button).val());
  $j(this.edit_reply_submit_button).attr('disabled', 'true');
  $j(this.edit_reply_submit_button).val(message);

  this.hideCancelLinkForSubmit();
};

ReplyEdit.prototype.reenableButton = function() {
  if ($j(this.edit_reply_submit_button).data('originalValue')) {
    $j(this.edit_reply_submit_button).val($j(this.edit_reply_submit_button).data('originalValue'));
    $j(this.edit_reply_submit_button).data('originalValue', null);
  }

  $j(this.edit_reply_submit_button).removeAttr('disabled');

  this.showCancelLinkForSubmit();
};

ReplyEdit.prototype.loginAndUpdateReply = function() { 
  this.disableButtonForSubmit(/*!<sl:translate>*/'Updating...'/*!</sl:translate>*/);

  var that = this;
  page.login_and_do(that.dom_id, function() { 
    if(page.profanity) {
      that.checkForProfanity(that.updateReply);
    } else {
      that.updateReply();
    }
  });
};

ReplyEdit.prototype.checkForProfanity = function(callback) {
  var encodedParams = $j.param({
    'content[reply content]'          : $j(this.edit_reply_textarea).val(),
    'content[reply emotitag feeling]' : $j(this.emotitag_feelings).val()
  });

  $j.ajax({
    url : page.profanityPath,
    context : this,
    type : 'POST',
    data : encodedParams,
    success : callback,
    error : function () {
      this.reenableButton();
      alert(/*!<sl:translate>*/'This is a family site, profanity is not allowed.'/*!</sl:translate>*/);
    }
  });
};

ReplyEdit.prototype.updateReply = function() {
  var urlForRequest = $j(this.edit_reply_form).attr('action');
  if(typeof(urlForRequest) === 'undefined') { throw "ReplyEditUpdate: no url for action!"; }

  var that = this;
  $j.ajax({
    url: urlForRequest,
    data: page.pageNumberParam + '&' + $j(this.edit_reply_form).serialize(),
    context: this,
    dataType: 'json',
    type: 'PUT',
    success: function(data, textStatus, jqXHR) {
      if (data.is_reply === true) { 
        if (that.topicStatusSelectBox.val() !== '') {
          $j('#moderator_updates_pod .topic_status .select_activator span.value, .status .status_frame .status_message')
            .text(that.topicStatusSelectBox.find(':selected').text());
        }
        $j(this.reply_container).replaceWith(data.content.show_html);
        $j(this.reply_container).append(data.content.edit_form);
        new Sfn.Widget.TextEditor($j('div.topic_or_reply_posting', this.reply_container));
        $j('textarea.comment.submission').autoResize();
        this.cancelEditingReply();
        this.reenableButton();
      }
    },
    error: function(jqXHR, textStatus, errorThrown) {
      try {
        response = $j.parseJSON(jqXHR.responseText);
        if (response.is_error_msg != true) {throw "no message";};
        window.page.fixed_message(response.error_message, 'bad');
      } catch(err) {
        window.page.fixed_message(/*!<sl:translate>*/'Unable to edit reply at this time, please try again later.'/*!</sl:translate>*/, 'bad');
      }
      this.reenableButton();
    }
  });
};

ReplyEdit.prototype.hideCancelLinkForSubmit = function() { $j(this.edit_reply_cancel_link).hide(); };
ReplyEdit.prototype.showCancelLinkForSubmit = function() { $j(this.edit_reply_cancel_link).show(); };
function ReplySubmission(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw ReplySubmissionConstructError;
  }

  this.dom_id = $j(activating_element).parents('li.create_reply_row');

  this.reply_submission_form = $j(this.dom_id).find('form.reply.submission.form');
  this.reply_submit_button = $j(this.dom_id).find('input.reply.form_submit');
  this.reply_textarea = $j(this.dom_id).find('textarea#reply_content');
  this.emotitag_feelings = $j(this.reply_textarea).find('input#reply_emotitag_feeling');
  this.topicStatusSelectBox = $j('#new_topic_status', this.dom_id);

  this.conversation_list = $j(this.dom_id).parents('div#topic_reply_box').siblings('ul#full_conversation');
  this.topic = new Topic($j(activating_element).parents('#topic_reply_box').siblings('div.topic').children('div')[0]);
}

ReplySubmission.prototype.loginAndSubmitReply = function() { 
  this.disableButtonForSubmit(/*!<sl:translate>*/'Replying...'/*!</sl:translate>*/);

  if(!this.validateReply()){
    throw "ReplyEntrySubmissionError: no description provided to reply";
  }

  var that = this;
  page.login_and_do(that.dom_id, function() {
    if(page.profanity) {
      that.checkForProfanity(that.submitReply);
    } else {
      that.submitReply(); 
    }
  });
};

ReplySubmission.prototype.checkForProfanity = function(callback) {
  var encodedParams = $j.param({
    'content[reply content]'          : $j(this.reply_textarea).val(),
    'content[reply emotitag feeling]' : $j(this.emotitag_feelings).val()
  });

  $j.ajax({
    url : page.profanityPath,
    context : this,
    type : 'POST',
    data : encodedParams,
    success : callback,
    error : function () {
      this.reenableButton();
      alert(/*</sl:translate>*/'This is a family site, profanity is not allowed'/*!</sl:translate>*/);
    }
  });
};

ReplySubmission.prototype.validateReply = function() {
  // Validate that the reply_textarea has content
  if($j.trim($j(this.reply_textarea).val()) === ''){
    alert(/*!<sl:translate>*/'The reply cant be posted if the description is empty, please try again'/*!</sl:translate>*/);
    this.reenableButton();
    return false;
  }
  return true;
};

ReplySubmission.prototype.submitReply = function() {
  var urlForRequest = $j(this.reply_submission_form).attr('action');
  if(typeof(urlForRequest) === 'undefined') {
    throw "ReplyEntrySubmissionError: no url for action!";
  }

  var that = this;
  $j.ajax({
    url : urlForRequest,
    data : page.pageNumberParam + '&' + $j(this.reply_submission_form).serialize(),
    dataType : 'json',
    type : 'POST',
    context : this,
    success: function(data, textStatus, jqXHR) {
      if (data.is_reply === true) {
        this.topic.follow();
        if (page.isLastPage) {
          if (that.topicStatusSelectBox.val() !== '') {
            $j('#moderator_updates_pod .topic_status .select_activator span.value, .status .status_frame .status_message')
              .text(that.topicStatusSelectBox.find(':selected').text());
          }
          $j(this.conversation_list).children('.reply').last().removeClass('last');
          $j(this.conversation_list).append(data.content.reply_show_html);
          $j(this.conversation_list).append(data.content.reply_edit_form);
          $j(this.reply_textarea).val('').focus();
          $j('textarea.comment.submission').autoResize();
          new Sfn.Widget.TextEditor($j('li.edit_reply_row', this.conversation_list).last());
          this.reenableButton();
          $j(this.conversation_list).children('.reply').last().addClass('last');
        } else {
          window.location = page.lastPageURL;
        }
      }
    },
    error: function(jqXHR, textStatus, errorThrown) {
      try {
        response = $j.parseJSON(jqXHR.responseText);
        if (response.is_error_msg != true) {throw "no message";};
        window.page.fixed_message(response.error_message, 'bad');
      } catch(err) {
        window.page.fixed_message(/*!<sl:translate>*/'Unable to post reply at this time, please try again later.'/*!</sl:translate>*/, 'bad');
      }
      this.reenableButton();
    }
  });
};

ReplySubmission.prototype.disableButtonForSubmit = function(message) {
  if (message === undefined) { message = /*!<sl:translate>*/'Replying...'/*!</sl:translate>*/; }
  $j(this.reply_submit_button).data('originalValue', $j(this.reply_submit_button).val());
  $j(this.reply_submit_button).attr('disabled', 'true');
  $j(this.reply_submit_button).val(message);

  this.hideCancelLinkForSubmit();
};

ReplySubmission.prototype.reenableButton = function() {
  if ($j(this.reply_submit_button).data('originalValue')) {
    $j(this.reply_submit_button).val($j(this.reply_submit_button).data('originalValue'));
    $j(this.reply_submit_button).data('originalValue', null);
  }

  $j(this.reply_submit_button).removeAttr('disabled');

  this.showCancelLinkForSubmit();
};

ReplySubmission.prototype.hideCancelLinkForSubmit = function() { $j(this.edit_reply_cancel_link).hide(); };
ReplySubmission.prototype.showCancelLinkForSubmit = function() { $j(this.edit_reply_cancel_link).show(); };
function PromotedReply(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw PromotedReplyConstructError;
  }

  this.dom_id = $j(activating_element).parents('li.reply');
  this.reply_dom_object_id = $j(this.dom_id).attr('id').split('_').last();

  this.remove_link = $j(this.dom_id).find('a.demote.promoted_reply.link');
}

PromotedReply.prototype.remove = function() {
  var urlForRequest = $j(this.remove_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "PromotedReplyRemoveError: no url for action!"; }

  var encodedParams = $j.param({reply_id:this.reply_dom_object_id}, true);
  var that = this;
  page.login_and_do(that.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      type: 'DELETE',
      data: encodedParams,
      success: function(data, textStatus, jqXHR) {
        var promotedReplies = $j('div#topic_overview div#promoted_replies');

        $j(promotedReplies).children().remove();
        $j(promotedReplies).append(data.content);

        $j('li#reply_' + that.reply_dom_object_id).find('a.mark.reply.official').show();
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Could not remove this tag at this time.'/*!</sl:translate>*/);
      }
    });
  });  
};
function Tag(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw TagConstructError;
  }

  this.dom_id = $j(activating_element).parents('li.tag');
  this.remove_tag_link = $j(this.dom_id).find('a.delete.tag');
}

Tag.prototype.removeTag = function() {
  var tagObject = this;

  var urlForRequest = $j(this.remove_tag_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "TagRemoveError: no url for action!"; }

  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      type: 'DELETE',
      context : this,
      success: function(data, textStatus, jqXHR) {
        $j(tagObject.dom_id).remove();
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Could not remove this tag at this time.'/*!</sl:translate>*/);
      }
    });
  });  
};
function TagSubmission(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw TagSubmissionConstructError;
  }

  this.dom_id = $j(activating_element).parents('div#tags');

  this.add_tag_form = $j(this.dom_id).find('form.tag_submission');
  this.add_tag_text_field = $j(this.dom_id).find('input.text');

  this.tag_list = $j(this.dom_id).find('ul#topic_tags_list');

  this.expand_tag_add_form_link = $j(this.dom_id).find('a.link.add');
}

TagSubmission.prototype.addTag = function() {
  var tagObject = this;

  var urlForRequest = $j(tagObject.add_tag_form).attr('action');
  if(typeof(urlForRequest) === 'undefined') { throw "TagSubmissionAddError: no url for action!"; }

  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      type: 'POST',
      context : this,
      data: $j(tagObject.add_tag_form).serialize(),
      success: function(data, textStatus, jqXHR) {
        $j(tagObject.tag_list).children().remove();
        $j(tagObject.tag_list).append(data);
        $j(tagObject.add_tag_text_field).val('');
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Could not create a tag at this time.'/*!</sl:translate>*/);
      }
    });
  });
};

TagSubmission.prototype.revealTagAddForm = function() {
  $j(this.add_tag_form).show();
  $j(this.expand_tag_add_form_link).hide();
  $j(this.add_tag_text_field).focus();
};

TagSubmission.prototype.hideTagAddForm = function() {
  $j(this.add_tag_form).hide();
  $j(this.expand_tag_add_form_link).show();
};
function LinkShortenerWidget(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw LinkShortenerWidgetConstructError;
  }

  this.dom_id = $j(activating_element).parents('div#share div.gs_short_url');

  this.shortening_link = $j(this.dom_id).find('a#get_shortened_link');

  this.short_link_modal = $j(this.dom_id).find('.modal');
  this.short_link_text_box = $j(this.short_link_modal).find('input#short_url');
}

LinkShortenerWidget.prototype.getAndDisplayLink = function() {
  var urlForRequest = $j(this.shortening_link).attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "LinkShortenerWidgetError: no url found for action!"; }

  var currentTextValue = $j(this.short_link_text_box).val();
  if(currentTextValue === '') {
    $j.ajax({
      url: urlForRequest,
      context: this,
      type: 'GET',
      success: function(data, textStatus, jqXHR) {
        this.showShortLink(data);
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Error, could not generate link at this time.'/*!</sl:translate>*/);
      }
    });
  } else {
    this.showShortLink(currentTextValue);
  }
};

LinkShortenerWidget.prototype.showShortLink = function(data) {
  if(data === null) { data = ''; }

  $j(this.short_link_text_box).val(data);
  $j(this.short_link_modal).removeClass('hidden');
  $j(this.short_link_modal).show();
};

LinkShortenerWidget.prototype.closeWidget = function() {
  $j(this.short_link_modal).hide();
};
function ShareTopicWidget(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw ShareTopicWidgetConstructError;
  }

  this.dom_id = $j(activating_element).parents('div#share div.mail');

  this.send_email_modal = $j(this.dom_id).find('div.modal');
  this.send_email_form = $j(this.send_email_modal).find('form#topic_share');
  this.submit_button = $j(this.send_email_modal).find('input.topic_share.form_submit');
  this.cancel_link = $j(this.send_email_modal).find('a.topic_share.cancel');

  this.recipient_address = $j(this.dom_id).find('input#email_to');
  this.optional_message_text = $j(this.dom_id).find('input#optional_message');
}

ShareTopicWidget.prototype.resetAndHideSendTopicForm = function() {
  $j(this.send_email_modal).addClass('hidden');
  $j(this.send_email_modal).hide();
  $j(this.recipient_address).val('');
  $j(this.optional_message_text).val('');
};

ShareTopicWidget.prototype.showSendTopicModal = function() {
  $j(this.send_email_modal).removeClass('hidden');
  $j(this.send_email_modal).show();
  $j(this.recipient_address).focus();
};

ShareTopicWidget.prototype.submitShareEmailForm = function() {
  this.disableButtonForSubmit();
 
  var urlForRequest = $j(this.send_email_form).attr('action');
  if(typeof(urlForRequest) === 'undefined') { throw "ShareTopicWidgetSubmitError: no url for action!"; }

  $j.ajax({
    url: urlForRequest,
    data: $j(this.send_email_form).serialize(),
    context: this,
    type: 'POST',
    success: function(data, textStatus, jqXHR) {
      this.reenableButton();
      this.resetAndHideSendTopicForm();
    },
    error: function(jqXHR, textStatus, errorThrown) {
      eval(jqXHR.responseText)
      this.reenableButton();
    }
  });
};

ShareTopicWidget.prototype.disableButtonForSubmit = function(message) {
  if (message === undefined) { message = /*!<sl:translate>*/'Sending...'/*!</sl:translate>*/; }
  $j(this.submit_button).data('originalValue', $j(this.submit_button).val());
  $j(this.submit_button).attr('disabled', 'true');
  $j(this.submit_button).val(message);
};

ShareTopicWidget.prototype.reenableButton = function() {
  if ($j(this.submit_button).data('originalValue')) {
    $j(this.submit_button).val($j(this.submit_button).data('originalValue'));
    $j(this.submit_button).data('originalValue', null);
  }

  $j(this.submit_button).removeAttr('disabled');
};
(function($) {

  var sfn_request = function(options) {
    $.ajax($.extend({ url : options.url, type : 'GET' }, options));    
    return false;
  };

  // remote links handler
  $('a[sfn-data-remote=true]').live('click', function() {
    return sfn_request({ url : this.href, type : 'GET', contentType: "text/javascript; charset=utf-8", dataType:  'script' });
  });

  // remote forms handler
  $('form[sfn-data-remote=true]').live('submit', function() {
    return sfn_request({ url : this.action, type : this.method, data : $(this).serialize() });
  });
  

})(jQuery);
/*
 *  Sfn.Form.Validator
 *  version   : 0.1
 *  requires  : jQuery v1.4 or later
 *              Neon.js
 *  authors   : Claude Nix (claude@getsatisfaction.com)
 *              Ivan Torres (ivan@getsatisfaction.com)
 *
 *  Usage:
 *    The validator takes two arguments, a CSS selector and a JSON object
 *    
 *      new Sfn.Form.Validator('.some-selector', { object });
 *
 *    The CSS selector should be an input (or group of inputs) that have a
 *    parent element with a class of .validatable
 *      
 *      <fieldset class="address validatable">
 *        <label for="city">City:</label>
 *        <input type="text" name="city" class="address-city" />
 *      </fieldset>
 *
 *      // JS (inside of document.ready)
 *      new Sfn.Form.Validator('.address-city', { rule : //... });
 *
 *    The JSON object looks like this:  
 *
 *      { notEmpty :                                    // validation name 
 *        { pattern     : /^$/,                         // pattern can be regex or function
 *          message     : "Field should not be blank",  // String message for false case
 *          skipOnLoad  : true,                         // Don't validate on page load
 *          required    : true }                        // Prevent form submission on false case
 *      }
 *
 *    The +pattern+ property can optionally take a function which receives a
 *    jQuery object containing the validatable element:
 *    
 *      { notMoreThan100Chars :
 *        { pattern   : function (element) {
 *                        // restrict field to 100 characters or less
 *                        return element.val().length <= 100;
 *                      },
 *          message   : "Field can't exceed 100 characters" }
 *      }
 *
 *    
 *
 */


Sfn.Form = {};

Factory(Sfn.Form, 'Validator')({
  validations : [],
  valid       : true,
  submit      : true,
  validateAll : function () {
    Sfn.Form.Validator.valid = true;
    Sfn.Form.Validator.submit = true;
    $j.each(Sfn.Form.Validator.validations, function () {
      this.validate();
    });
    return Sfn.Form.Validator.valid;
  },

  prototype : {
    element  : null,
    messages : null,
    rules    : {},

    init : function (selector, rules) {
      var self = this;
      this.element = $j(selector);
      if(this.element.size() === 0)
        return;
      this.rules   = rules;

      this.messages = this.element
        .parents('.validatable')
        .children('.messages').length > 0 ?
         this.element
        .parents('.validatable')
        .children('.messages') :
        $j('<ul class="messages"></ul>')
          .appendTo(this.element.parents('.validatable'));

      this.element
        .change(function (event) { self.validate(); event.stopPropagation(); })
        .blur(function (event) { self.validate(); event.stopPropagation(); });

      self.constructor.validations.push(this);
    },

    validate : function (options) {
      var self = this;

      this.constructor.valid = true;

      var addError = function (obj, error, rule) {
        var domClasses = error;

        if (rule.required) {
          domClasses += " required";
        }

        if (!$j('.' + error, obj.messages).length) {
          $j('<li class="' + domClasses + '">' + rule.message + '</li>').appendTo(obj.messages);
        }
      };

      var removeError = function (obj, error) {
        if ($j('.' + error, obj.messages).length) {
          $j('.' + error, obj.messages).remove();
        }
      };

      $j.each(this.rules, function (error, rule) {
        if (!rule.skipOnLoad) {
          if (this.pattern.constructor.toString().match(/RegExp/)) {
            if (self.element.val().match(rule.pattern)) {
              addError(self, error, rule);
              this.constructor.valid = false;
              if (rule.required) {
                Sfn.Form.Validator.submit = false;
              }
            } else {
              removeError(self, error);
            }
          } else if (this.pattern.constructor.toString().match(/Function/)) {
            if (rule.pattern(self.element)) {
              addError(self, error, rule);
              this.constructor.valid = false;
              if (rule.required) {
                Sfn.Form.Validator.submit = false;
              }
            } else {
              removeError(self, error);
            }
          }
        } else {
          rule.skipOnLoad = false;
        }
      });
    }
  }
});
function UserDefinedCode(activating_element) {
  if ($j(activating_element).size() === 0) {
    throw UserDefinedCodeConstructError;
  }

  this.dom_id = $j(activating_element).parents('div#udc_sidebar');

  this.udc_partial_container = this.dom_id.find('div.sidebar.pod');
  this.udc_form_container = this.dom_id.find('div#set_a_code_form');
  this.udc_form = this.udc_form_container.find('form');

  this.udc = this.dom_id.find('div#udc');
  this.udc_edit_link = this.udc.find('a.udc.edit');
  this.udc_destroy_link = this.udc.find('a.udc.destroy');
  this.udc_initiate_link = this.udc.find('a.udc.initiate');
}

UserDefinedCode.prototype.revealUdcForm = function() {
  this.udc_form.removeClass('hidden');
  this.udc_form_container.show();

  this.udc.remove();
};

UserDefinedCode.prototype.submitUdc = function() {
  var self = this;
  var urlForRequest = this.udc_form.attr('action');
  if(typeof(urlForRequest) === 'undefined') { throw "UserDefinedCodeSubmitError: no url for action!"; }
   
  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      type: 'POST',
      context : self,
      data: self.udc_form.serialize(),
      dataType: 'text',
      success: function(data, textStatus, jqXHR) {
        self.udc_partial_container.replaceWith(data);
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Could not create Private Tag at this time.'/*!</sl:translate>*/);
      }
    });
  });  
};

UserDefinedCode.prototype.destroyUdc = function() {
  var self = this;
  var urlForRequest = this.udc_destroy_link.attr('href');
  if(typeof(urlForRequest) === 'undefined') { throw "UserDefinedCodeDestroyError: no url for action!"; }

  page.login_and_do(this.dom_id, function() {
    $j.ajax({
      url: urlForRequest,
      type: 'DELETE',
      success: function(data, textStatus, jqXHR) {
        self.udc_partial_container.replaceWith(data);
      },
      error: function(jqXHR, textStatus, errorThrown) {
        alert(/*!<sl:translate>*/'Could not remove this Private Tag at this time.'/*!</sl:translate>*/);
      }
    });
  });  
};
MiniProfile = Class.create();
MiniProfile.prototype = {
  initialize:function(profile_link, user_id) {
    this.timer = null;
    this.options = null;
    this.person_id = user_id;
    this.profile_overlay = $('mini_profile');
    this.profile_overlay.hide();
    this.profile_link = $(profile_link);
    this.profile_link.onmouseover = this.show_profile.bind(this);
    this.profile_link.onmouseout = this.hide_profile.bind(this);
    this.show_profile();
  },

  show_profile:function() {
    this.profile_overlay.onmouseover = function() {
      clearTimeout(this.timer);
      this.profile_overlay.show();
    }.bind(this);
    this.profile_overlay.onmouseout = this.hide_profile.bind(this);
    clearTimeout(this.timer);
    this.profile_overlay.down(".icon").setAttribute('src', 'https://drp60gfj3y9kn.cloudfront.net/assets/sprite_screen-e4ae3f34f9f27e80a63a55045c5d1015.png');
    if (this.profile_link.hasClassName('employee')) {
      this.profile_overlay.addClassName('employee');
    } else {
      this.profile_overlay.removeClassName('employee');
    }
    if (this.profile_link.hasClassName('champion')) {
      this.profile_overlay.addClassName('champion');
    } else {
      this.profile_overlay.removeClassName('champion');
    }
    if(this.options) {
      this.apply_loaded_data();
    } else {
      this.load_delay = setTimeout(function() {
        this.show_after_load = true;
        this.show_progress_indicator();
        this.load_data();
      }.bind(this), 200)
    }
    this.timer = setTimeout(function() {
      this.show_overlay();
    }.bind(this), 400);
    return false;
  },
  
  apply_loaded_data: function() {
    this.profile_overlay.down(".icon").setAttribute('src', this.options.avatar_path);
    this.profile_overlay.down(".name").update(this.options.name);
    this.profile_overlay.down(".url").update(this.options.personal_url);
    if(this.options.tagline) { 
      this.profile_overlay.down(".tagline").update("\""+this.options.tagline+"\"");
    } else {
      this.profile_overlay.down(".tagline").update("");
    }    
    
    if(this.options.premium) { 
      this.profile_overlay.down(".premium").update("premium");
    } else {
      this.profile_overlay.down(".premium").update("");
    }
    
    this.profile_overlay.down(".getsatisfaction").update(this.options.getsatisfaction || '');
    this.profile_overlay.down(".private_fields").update(this.options.private_fields || '');
    this.profile_overlay.down(".identities").update(this.options.identities || '');

    this.profile_overlay.down(".created_at").update(/*!<sl:translate>*/"Joined "/*!</sl:translate>*/ + this.options.member_since);
    this.profile_overlay.down(".location").update(this.options.location);
    this.profile_overlay.down(".topics").update('<strong>'+this.options.topics+'</strong> ' + new String(/*!<sl:translate>*/"Topic"/*!</sl:translate>*/).pluralize(this.options.topics));
    this.profile_overlay.down(".replies").update('<strong>'+this.options.replies+'</strong> ' + new String(/*!<sl:translate>*/"Reply"/*!</sl:translate>*/).pluralize(this.options.replies));
    this.hide_progress_indicator();
  },
  
  show_progress_indicator: function() {
    $('mini_progress_indicator').show();
    $('mini_profile_data').hide();
  },

  hide_progress_indicator: function() {
    $('mini_profile_data').show();
    $('mini_progress_indicator').hide();
  },

  load_data: function() {
    new Ajax.Request('/people/' + this.person_id + '/mini_profile', {
      method: 'get',
      asynchronous:true,
      evalScripts:true,
      onSuccess: function(request) {
        this.options = eval('(' + request.responseText + ')');
        this.finish_loading_data();
      }.bind(this)
    });
  },
  
  finish_loading_data: function() {
    if(this.show_after_load) {
      this.apply_loaded_data()
    }
  },
  
  hide_profile:function() {
    clearTimeout(this.load_delay);
    clearTimeout(this.timer);
    this.timer = setTimeout(function() {
      this.show_after_load = false;
      this.profile_overlay.hide();
    }.bind(this), 400)
    return false;
  },
  
  show_overlay:function() {
    this.profile_overlay.dockTo(this.profile_link, {target_point: 'tr'});
  }
}

function mp(elem, param) {
  return new MiniProfile(elem, param);
}

;
var GSFN;
if (GSFN == undefined) {
  GSFN = {};
}

GSFN.setupFacebookInstaller = (function($) {

  return function() {

    var install_link = $('a.facebook_install')

    var disable = function() {
      alert('Please select a Facebook tab label.')
      return false
    }

    var set_involver_link = function() {
      if (this.checked) {
        var involver_url = install_link
          .attr('href')
          .replace(/involver_[^/]+/, this.value)
        
        install_link
          .attr('href', involver_url)
          .unbind('click', disable)
          .removeClass('disabled')
      }
    }

    if ($('input:checked[name=fb_tab_name]').length === 0) {
      install_link.click(disable).addClass('disabled')
    } else {
      set_involver_link.apply($('input:checked[name=fb_tab_name]')[0])
    }

    $('input[name=fb_tab_name]').change(set_involver_link)

    $('#show_install_instructions form li:odd').css({backgroundColor: '#fffbf0'});
    $('#show_install_instructions form li:even').css({backgroundColor: '#fffff7'});
  }


})(jQuery)
;
jQuery(function ($) {

  months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
  days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
  AdminStats = function () {
    var stats_container = $('#stats_container');
    this.color_palette = $.parseJSON(stats_container.attr('data-colors'));
    this.columns = $.parseJSON(stats_container.attr('data-columns'));
    this.rows = [], this.colors = [], this.data = null, this.stat_groups = {};
    this.view = "week";

  };

  AdminStats.prototype = {

    set_data: function (data) {
      this.data = data.row_data;
      this.columns = data.column_data;
      this.start_of_range = data.start_of_range;
      this.end_of_range = data.end_of_range;
      for (group in this.stat_groups) {
        try {
          this.updates_stats_for(group, this.stat_groups[group]);
        } catch (e) {
          console.error(e);
        }
        ;
      }
      this.populateActiveUsersTable(data);
      this.populateActiveTopicsTable(data);
      this.set_nps();
      this.finish_switch_view();
    },

    updates_stats_for: function (name, options) {
      $('#' + name)[0].display_options = {rows: options.rows, columns: this.columns, colors: options.colors, line_styles: options.line_styles, step_size: this.data.step_size}
      this.updateTableData(name, this.data[name], options.segments);
      this.updateFlashData(name + '_trending', this.data[name], $('#' + name)[0].display_options);
    },

    updateFlashData: function (name, stats, display_options) {
      var flashObject = this.getFlashObject(name);
      var flashUpdater = function () {
        if (flashObject.updateData) {
          flashObject.updateData(stats);
          flashObject.updateRange(display_options);
        } else {
          console.log("retry");
          _.delay(flashUpdater, 500);
        }
      };
      flashUpdater();
    },

    updateTableData: function (name, stats, segments) {
      $.each(segments, function (i, segment) {
        var segment_total = _.chain(stats[segment]).values().compact().value().sum().commify();
        $('table#' + group + ' tr#' + segment + '_row').find('td.segment_value').first().html(segment_total);
        if (segment == 'total') {
          $('#stats_bar').find('.' + name).first().html(segment_total);
        }
      });
    },

    set_nps: function () {
      var segment_value;
      if (this.data.nps) {
        segment_value = this.data.nps
        segment_value = (Math.round((segment_value * 10000) / 100));
      } else {
        segment_value = "N/A"
      }
      $('#stats_bar').find('.net_promoter_score').first().html(segment_value);
    },

    populateActiveUsersTable: function (data) {
      var users = data['row_data']['active.users']
      var tbody = $('table#active_users tbody');
      var template = _.template("<tr><td class='name text'><a href='<%= path %>'><%- name %></a></td><td class='topic-count number'><%= action_count %></td></tr>")
      tbody.empty();
      _(users).each(function (user) {
        tbody.append(template(user))
      });
    },

    populateActiveTopicsTable: function (data) {
      var topics = data['row_data']['active.topics']
      var tbody = $('table#active_topics tbody');
      var template = _.template("<tr><td class='subject text'><a href='<%= path %>'><%- subject %></a></td><td class='response-count number'><%= response_count %></td></tr>")
      tbody.empty();
      _(topics).each(function (topic) {
        tbody.append(template(topic))
      });
    },

    switch_view: function (new_view) {
      this.view = new_view;
      $('#filters dd').removeClass('active');
      $('#filters').find('.' + new_view).first().addClass('active');
      $('#stats_page').removeClass('loaded');
      $('#title_range').html("Loading " + new_view + " view...")
    },

    updateTitleBarDateRange: function () {
      $('#title_range').html(this.start_of_range + " - " + this.end_of_range);
    },

    finish_switch_view: function () {
      var col_data = this.columns;
      this.updateTitleBarDateRange();
      $('#stats_page').addClass('loaded', 500);
    },

    showMissingDataMessage: function() {
      $('#title_range').text("Generating -- please check back in 2 hours.");
      $('#stats_page').addClass('loaded', 500);
    },

    registerStatGroups: function () {
      var self = this;
      $('.stat_group').each(function (i, group) {
        group = $(group);
        self.register_stat_group(
          $.parseJSON(group.attr('data-stat-group')),
          {
            segments: $.parseJSON(group.attr('data-segments')),
            rows: $.parseJSON(group.attr('data-rows')),
            colors: $.parseJSON(group.attr('data-colors')),
            line_styles: $.parseJSON(group.attr('data-line-styles'))
          }
        );

      });
    },

    register_stat_group: function (name, options) {
      this.flash_ready = null;
      this.stat_groups[name] = options;
    },

    toggleSegment: function (row, name, segment, color, line_style) {
      var table_row = $(row);
      var thing = $('#' + name)[0];
      if (table_row.hasClass('active')) {
        table_row.find('img').attr('src', "/images/stats_button.png");
        thing.display_options.rows = $.grep(thing.display_options.rows, function (x) {
          return x === segment
        }, 'invert');
        thing.display_options.colors = $.grep(thing.display_options.colors, function (x) {
          return x === color
        }, 'invert');
        thing.display_options.line_styles = $.grep(thing.display_options.line_styles, function (x) {
          return x === line_style
        }, 'invert');
      } else {
        table_row.find('img').attr('src', "/images/stats_button_on.png");
        thing.display_options.rows.push(segment);
        thing.display_options.colors.push(color);
        thing.display_options.line_styles.push(line_style);
      }
      flashObject = this.getFlashObject(name + '_trending');
      flashObject.updateRange(thing.display_options);
      table_row.toggleClass('active');
    },

    getFlashObject: function (name) {
      return navigator.appName.indexOf("Microsoft") != -1 ? window[name] : document[name];
    }
  };

  if ($('#stats_container.v2').length > 0) {
    admin_stats = new AdminStats();
    admin_stats.registerStatGroups();

    $.getJSON('./stats_v2/data.json')
      .success(function (data) {
        admin_stats.set_data(data)
      })
      .error(admin_stats.showMissingDataMessage);
  }

});
(function ($) {

    var LABELS = {
        question:  /*!<sl:translate_html>*/"<span class='translate'><strong>Ask a question</strong>. We'll see if other people have this question, too.</span>"/*!</sl:translate_html>*/,
        idea:     "<strong>" + /*!<sl:translate>*/ "Share an idea" + /*!</sl:translate>*/ "</strong>. " + /*!<sl:translate>*/ "We'll see if other people have this idea, too." /*!</sl:translate>*/,
        problem:  "<strong>" + /*!<sl:translate>*/ "Report a problem" + /*!</sl:translate>*/ "</strong>. " + /*!<sl:translate>*/ "We'll look for solutions." /*!</sl:translate>*/,
        praise:   /*!<sl:translate>*/"What makes you happy about this company?"/*!</sl:translate>*/,
        update:   /*!<sl:translate>*/"News or updates for your customers go here."/*!</sl:translate>*/,
        tip:      "<strong>" + /*!<sl:translate>*/ "Got a tip?" + /*!</sl:translate>*/ "</strong>. " + /*!<sl:translate>*/ "Suggest it here." /*!</sl:translate>*/
    };

    var exc;

    window.StartTopicBox = function () {
        this.initialize.apply(this, arguments);
    };

    StartTopicBox.prototype = {

        initialize: function (elementId, path) {

            // Configuration values

            this.$element        = $( elementId[0] == '#' ? elementId : '#' + elementId );
            this.path            = path;

            // Useful DOM references

            this.$styleTabs      = $('.style_tabs li', this.$element);
            this.$topicPrompt    = $('.prompt', this.$element);
            this.$searchResults  = $('.results_container', this.$element);
            this.$form           = $('form', this.$element);
            this.$styleField     = $('.style', this.$element);
            this.$queryField     = $('.query', this.$element);
            this.$submitButton   = $(':submit', this.$element);
            this.$resetButton    = $('.reset', this.$element);

            this.initializeLayout();

            // Initialize data and DOM

            this.$styleTabs.each(function (i, styleTab) {
                var $styleTab = $(styleTab);
                $styleTab.data('style', styleTab.className.split(/\s+/).filter(function(c){return c != 'active';})); // active tab has "active" as a class, which we don't want to select
            });

            // State values

	    this.$activeStyleTab = this.$styleTabs.filter('.active');
	    this.selectStyleTab( this.$activeStyleTab );

            // Bind event handlers

            var self = this;

            $('a', this.$styleTabs).bind('click.startTopicBox', function (evt) {
                try {
                    self.selectStyleTab( $(this).parents('li') );
                } catch (exc) {
                    // ignore //
                } finally {
                    return false;
                }
            });

            this.$form.bind('submit.startTopicBox', function (evt) {
                try {
                    return self.searchIfNeeded();
                } catch (exc) {
                    return true;
                }
            });

            this.$resetButton.bind('click.startTopicBox', function (evt) {
                try {
                    self.reset();
                } catch (exc) {
                    // ignore //
                } finally {
                    return false;
                }
            });

            if (this.$form.data('autosubmit')) {
                self.searchIfNeeded();
            }
        },

        selectStyleTab: function (styleTab) {

            this.$activeStyleTab.removeClass('active');
            this.$activeStyleTab = $(styleTab).addClass('active');

            var style = this.$activeStyleTab.data('style');
            this.$topicPrompt.show().html( LABELS[style] );
            this.$styleField.val(style);
        },
  
        searchIfNeeded: function () {

            var formSubmit = true;

            if ($.trim( this.$queryField.val() ) == ''){

                this.$topicPrompt.show().html('<em>' + /*!<sl:translate>*/ 'Please enter text in the field above before continuing.' /*!</sl:translate>*/ + '</em>');
                formSubmit = false;

            } else if (this.$searchResults.is(':hidden')) {

                /*!<sl:translate>*/
                this.$submitButton.val("Continue...").blur();
                /*!</sl:translate>*/

                $.ajax({
                    url: this.path,
                    type: 'get',
                    async: true,
                    data: {query: this.$queryField.val()},
                    dataType: 'jsonp'
                });

                formSubmit = false;
            }

            return formSubmit;
        },
  
        showResults: function () {
            var self = this;
            this.$topicPrompt.show().html("<strong>" + /*!<sl:translate>*/ "Someone might have already asked your question." /*!</sl:translate>*/ + "</strong>" + /*!<sl:translate>*/ " Do any of these match?" /*!</sl:translate>*/ ).width(this.$queryField.width());
            this.$searchResults.slideDown('normal', function () {
                self.$resetButton.show();
                /*!<sl:translate>*/
                self.$submitButton.val('Nope. Finish posting my question');
                /*!</sl:translate>*/
            });
        },
  
        continuePosting: function () {
            this.$form.unbind('submit.startTopicBox').submit();
        },
        
        initializeLayout : function () {
            this.$topicPrompt.width(this.$queryField.width() - this.$submitButton.width() - 50);
        },

        reset: function () {
            /*!<sl:translate>*/
            this.$submitButton.val("Continue");
            /*!</sl:translate>*/

            this.initializeLayout();
            this.$resetButton.hide();
            this.$searchResults.slideUp();
            this.selectStyleTab( this.$styleTabs.filter('.question') );
        }
    };

})(jQuery);
$j(document).ready(function($) {

  // Register listening for keyup to calculate how many characters can still be handled in the subject line
  if ($('#topic_subject').length) {
    var topicSubject = $('#topic_subject');

    handleUpdatingCharacterCounter(topicSubject);
    topicSubject.keyup(function() {
      handleUpdatingCharacterCounter(topicSubject);
    });
  }

  // Topic Style Picker
  new Sfn.Widget.StylePicker('#style_picker');

  // Product Picker
  new Sfn.Widget.ProductPicker('.product_checkbox');

  // input validator
  // patterns should be regex or functions, which will be passed the
  // input element that the validator is given
  new Sfn.Form.Validator('#topic_subject', {
    tooManyQuestionMarks : {
      pattern : /\?{3,}/,
      message : /*!<sl:translate>*/ "What's with all the question marks in your title???? One or two ought to be enough."/*!</sl:translate>*/
    },

    allCaps : {
      pattern : /([A-Z]|\s+){6,}/,
      message : /*!<sl:translate>*/ "EASE UP ON THE ALL CAPS IN YOUR TITLE. It looks like you're shouting."/*!</sl:translate>*/
    },

    tooManyBangs : {
      pattern : /!{3,}/,
      message : /*!<sl:translate>*/ "Go easy on the exclamation points in your title!!!! One or two ought to be enough."/*!</sl:translate>*/
    },

    tooEmpty : {
      pattern : function(content) {
        return jQuery(content).val().length === 0;
      },
      skipOnLoad : true,
      required : true,
      message : /*!<sl:translate>*/ 'Add a title to your topic.'/*!</sl:translate>*/
    },

    subjectTooLong : {
      pattern : function(content) {
        return jQuery(content).val().length > 140;
      },
      required : true,
      message : /*!<sl:translate>*/ "Topic subject cannot be longer than 140 characters"/*!</sl:translate>*/
    }
  });

  // Toic Additional Detail validation
  new Sfn.Form.Validator('#topic_additional_detail', {
    addSomeDetail : {
      pattern : function(element) {
        return element.val().length < 200;
      },
      message :  /*!<sl:translate>*/"Add some detail. One or two paragraphs works best.",/*!</sl:translate>*/
      skipOnLoad : true
    }
  });

  // Selected Products validation
  new Sfn.Form.Validator('.product_checkbox', {
    selectProducts : {
      pattern : function() {
        return !jQuery('.product_checkbox').is(':checked');
      },
      message : /*!<sl:translate>*/"Add at least one product or service."  /*!</sl:translate>*/
    }
  });

  // Tags Validation
  new Sfn.Form.Validator('#topic_keywords', {
    addTags : {
      pattern : function(element) {
        return element.val().split(',').length < 3;
      },
      message: /*!<sl:translate>*/"Add three or more tags, separated by commas." /*!</sl:translate>*/
    }
  });

  // Emotion Feeling Validation
  new Sfn.Form.Validator('.emotitagger_feeling', {
    getEmotional : {
      pattern : /^$/,
      message : /*!<sl:translate>*/"Get emotional! Describe how this topic makes you feel." /*!</sl:translate>*/
    }
  });

  // Emotitag Face Validation
  new Sfn.Form.Validator('.emotitagger_face', {
    pickAFace : {
      pattern : /^$/,
      message : /*!<sl:translate>*/"Pick a face to let everybody know how you feel." /*!</sl:translate>*/
    }
  });

  // Topic Style Validation
  new Sfn.Form.Validator('#topic_style', {
    mustSetTopicStyle : {
      pattern : /^$/,
      message : /*!<sl:translate>*/"You must select a topic style." /*!</sl:translate>*/,
      skipOnLoad : true,
      required   : true
    }
  });

  // Profanity Validation
  var validateProfanity = function (elements, settings)  {
    if (page.profanity === true) {
      if (!elements.constructor.toString().match(/Array/)) {
        elements = new Array(elements);
      }

      // Prepare the params to be sent
      var params = {};

      $j.each(elements, function (i, element) {
          if (typeof $j(element).val() !== 'undefined') {
              params['content[' + i + ']'] = $j(element).val();
          }
      });

      $j.ajax({
        url     : page.profanityPath,
        type    : 'POST',
        data    : params,
        async   : false,
        success : settings.success,
        error   : settings.error
      });
    } else {
      settings.success();
    }
  };

  $j('#topic_submit_button').click(function () {
    var button = $j(this),
        label  = $j(button).val();

    $j(button).val(/*!<sl:translate>*/'Posting...'/*!</sl:translate>*/).attr('disabled', true);

    var restoreButton = function () {
      $j(button).val(label).removeAttr('disabled');
    };

    page.login_and_do(button, function () {
      Sfn.Form.Validator.validateAll();
      if (Sfn.Form.Validator.submit) {
        validateProfanity(['#topic_subject', '#topic_additional_detail', '#topic_emotitag_feeling', '#topic_keywords'], {
          success : function () {
            $j('#edit_topic_form, #new_topic_form').submit();
          },
          error   : function () {
            // Removes any previous message shown
            $j('#page_notification').remove();

            // Display message validation
            var message = $j('<div id="page_notification" class="main_msg clearfix bad" style="">\
              <a class="cancel_btn" href="#">x</a>\
              <div class="top"></div>\
              <div class="content clearfix">Profanity is not allowed.</div>\
            </div>');

            $j(message).appendTo('#header');

            $j('a.cancel_btn', message).click(function (event) { $j(message).hide(); event.stopPropagation(); });
            restoreButton();
          }
        });
      } else {
        restoreButton();
      }
    }, {
      cancel_login : function () { restoreButton(); },
      fail         : function () { restoreButton(); }
    });

    return false;
  });

  Sfn.Form.Validator.validateAll({ allowSkipping : false });
});

function klassyfy(string) {
  return string.toLowerCase().replace(/[^a-z0-9]+/g, '-');
};

function handleUpdatingCharacterCounter(element) {
  var length = $(element).val().length,
      warningThreshhold = 0,
      maxLength = 140,
      rVal = Math.ceil(((length - warningThreshhold) / (maxLength - warningThreshhold)) * 255),
      pluralizedCharacterString = /*!<sl:translate>*/' characters remaining'/*!</sl:translate>*/,
      singularizedCharacterString = /*!<sl:translate>*/' character remaining'/*!</sl:translate>*/,
      displayCharacterString = pluralizedCharacterString;

  if((maxLength - length) === 1) {
    displayCharacterString = singularizedCharacterString;
  }
  $(element).siblings('#subject_character_counter').html(maxLength - length + displayCharacterString).css({
    color: 'rgba(' + rVal + ', 0, 0, 0.5)'
  });
};
//call bulk action ajax call to display action confirmation overlay
$j('.moderator-action-button, .moderator-action-button *').live('click', function() {
  var self             = $j(this);
  var form_action      = self.attr('href');

  Sfn.Widget.Overlay.showLoader();

  //execute bulk action
  $j.ajax({
    url:        form_action,
    data_type:  'script',
    type:       'GET'
  });

  return false;
}); 

select_all_employees = function() {
  $$('#employee_share_list input').each(function(el) {
    el.setAttribute("checked", "checked");
  });
}
deselect_all_employees = function() {
  $$('#employee_share_list input').each(function(el) {
    el.removeAttribute("checked");
  });
}

// User Defined Code Interactions //
$j('div#udc_sidebar a.udc.initiate').live('click', function(event) {
  event.preventDefault();
  var udc = new UserDefinedCode(this);
  udc.revealUdcForm();
});
$j('div#udc_sidebar a.udc.destroy').live('click', function(event) {
  event.preventDefault();
  var udc = new UserDefinedCode(this);
  udc.destroyUdc();
});
$j('div#udc_sidebar a.udc.edit').live('click', function(event) {
  event.preventDefault();
  var udc = new UserDefinedCode(this);
  udc.revealUdcForm();
});
$j('div#udc_sidebar form.udc.submit').live('submit', function(event) {
  event.preventDefault();
  var udc = new UserDefinedCode(this);
  udc.submitUdc();
});
// End User Defined Code //
;
// User Data //
function requestAndApplyUserData() {
  if(typeof userDataPath !== 'undefined'){
    $j.ajax({
      url: userDataPath,
      type: 'GET',
      dataType: 'json',
      success: function (data, textStatus, jqXHR) { applyUserData(data); }
    });
  }
};

function applyUserData(userState) {
  if (userState.can_moderate) {
    $j('div.topic div.subject a.initiate.edit_subject').removeClass('hidden');
    $j('div.topic div.subject div.tools').show();
    $j('div.reply_tools div.topic_status_options').show();
  }

  if (userState.can_edit) {
    $j('div.topic a.edit.link').removeClass('hidden'); 
    $j('div.topic div.tools').show();
  }
  
  if (userState.can_delete) {
    $j('div.topic a.delete.link').removeClass('hidden');
    $j('div.topic div.tools').show();
  }
  
  if (userState.can_set_topic_status) {
    $j('div.reply_form_container div.topic_status_options').show();
  }

  $j.each(userState.replies, function () {
    var reply = $j(this.dom_id);

    if (this.can_set_company_promoted) {
      $j('.tools a.mark.reply.official', reply).removeClass('hidden');
    }
  });

  userState.tags.each(function(tag) {
    var tagElement = $j('li#' + tag.dom_id);

    if(tag.deletable) { $j(tagElement).find('a.delete.tag').removeClass('hidden'); }
  });

  userState.replies.each(function(reply) {
    var replyElement = $j('li#' + reply.dom_id);
    var promotedReplyElement = $j('li#promoted_' + reply.dom_id);

    if (promotedReplyElement.length){
      if (reply.company_promoted && reply.can_set_company_promoted) {
        $j(replyElement).find('a.mark.reply.official').hide();
        $j(promotedReplyElement).find('div.tools').show();
        $j(promotedReplyElement).find('a.demote.promoted_reply').removeClass('hidden');
      }
      
      if (reply.can_remove) {
        $j(promotedReplyElement).find('a.remove.promoted_reply').removeClass('hidden');
      }
    }

    if (reply.can_set_company_promoted) { $j(replyElement).find('a.mark.reply.official').removeClass('hidden'); }

    if(reply.can_edit) { 
      $j(replyElement).find('a.edit.reply').removeClass('hidden'); 
      $j(replyElement).find('a.edit.comment').removeClass('hidden'); 
    }
    
    if(reply.can_delete) { 
      $j(replyElement).find('a.delete.reply').removeClass('hidden');
      $j(replyElement).find('a.delete.comment').removeClass('hidden');
    }

    if (reply.can_remove) {
      $j(replyElement).find('a.remove.reply').removeClass('hidden');
      $j(replyElement).find('a.remove.comment').removeClass('hidden');
      $j(replyElement).find('a.fork.reply').removeClass('hidden');
      $j(replyElement).find('a.restore.link').removeClass('hidden');
    }

    if(reply.starred) {
      $j(replyElement).find('a.promote.star_link').hide(); 
      $j(replyElement).find('a.demote.star_link').show();
    }

    if (!reply.can_agree) {
      $j(replyElement).find('a.promote.star_link').addClass('hidden');
      $j(replyElement).find('a.demote.star_link').addClass('hidden');
    }
  });
};
// End User Data //

$j(document).ready(function() { 
  // Product Widget //
  var communityProduct = new Sfn.Widget.Sidebar.CommunityProducts($j('#products'));
  // End Product Widget //

 $j('div.topic_or_reply_posting').each(function() { new Sfn.Widget.TextEditor(this) });

 if(page.logged_in()) {
    requestAndApplyUserData();
  } else {
    page.watch('login', requestAndApplyUserData);
  }

  // Reply show/hide of its tools
  $j('li.reply div.main', $j('ul#full_conversation')).live('mouseenter mouseleave', function(event) {
    var toolsContainer = $j(this).children('div.tools');

    if (event.type == 'mouseenter') {
      $j(toolsContainer).show();
    } else {
      $j(toolsContainer).hide();
    }
  });

  // Comment show/hide of its tools
  $j('li.comment', $j('ul#full_conversation')).live('mouseenter mouseleave', function(event) {
    var toolsContainer = $j(this).children('div.tools');

    if (event.type == 'mouseenter') {
      $j(toolsContainer).show();
    } else {
      $j(toolsContainer).hide();
    }
  });

  $j('.select_activator').click(function(event) {
    $j(this).next('.select_options').toggle();
    $j(this).parent().toggleClass('open');
    return false;
  });

  $j(".select_options, .select_activator").click(function(event) {
    if(event.target.nodeName == 'A'){
      event.preventDefault();
    } else if(event.target.type == 'submit') {  
      //noop 
    } else{
      event.stopPropagation();
    }
  });
  
  $j(document).click(function(event){
    $j(".select_options:visible").parent().toggleClass('open');
    $j(".select_options:visible").toggle();
  });

  $j('a.reply_link').live('click', function(event) {
    event.preventDefault();

    $j(this).siblings('div#topic_reply_box').find('textarea#reply_content').focus();
  });

  var sharebox = $j('.topic .sharing');
  var topicHeight = parseInt(sharebox.parents().first().css('height'));
  if (topicHeight > 500) {
    jQuery.event.add(window, "scroll", sharebarScroll);
  }

  function sharebarScroll() {
    var start = start || sharebox.offset().top;
    var scroll = $j(window).scrollTop();

    if ((scroll + 25) > '221') {
      if ((scroll + 115) < topicHeight) {
        console.log('sharebox should be fixed now');
        sharebox.css('position','fixed');
        sharebox.css('left','783px');
        sharebox.css('top','25px');
      } else {
        console.log('sharebox should be absolute now');
        sharebox.css('position','absolute');
        sharebox.css('left','654px');
        sharebox.css('top',(topicHeight-302)+'px');
      }
    } else {
      console.log('sharebox should be absolute now');
      sharebox.css('position','absolute');
      sharebox.css('left','654px');
      sharebox.css('top','25px');
    }
  }
  
  // new jQuery backed object js
  // Link Shortener Widget Interactions //
  $j('a#get_shortened_link').live('click', function(event) {
    event.preventDefault();
    var linkShortenerWidget = new LinkShortenerWidget(this);
    linkShortenerWidget.getAndDisplayLink();
    return false;
  });
  $j('div.modal a.close').live('click', function(event) {
    event.preventDefault();
    var linkShortenerWidget = new LinkShortenerWidget(this);
    linkShortenerWidget.closeWidget();
  });
  // End Link Shortener Widget //
  // Share Topic Widget Interactions //
  $j('div#share div.mail a#topic_share_modal').live('click', function(event) {
    event.preventDefault();
    var shareEmailWidget = new ShareTopicWidget(this);
    shareEmailWidget.showSendTopicModal();
  });
  $j('a.topic_share.cancel').live('click', function(event) {
    event.preventDefault();
    var shareEmailWidget = new ShareTopicWidget(this);
    shareEmailWidget.resetAndHideSendTopicForm();
  });
  $j('form#topic_share').live('submit', function(event) {
    event.preventDefault();
    var shareEmailWidget = new ShareTopicWidget(this);
    shareEmailWidget.submitShareEmailForm();
  });
  // End Share Topic Widget //
  // Tag Submission Interactions //
  $j('div.add_a_tag form.tag_submission').live('submit', function(event) {
    event.preventDefault();
    var tagSubmission = new TagSubmission(this);
    tagSubmission.addTag();
  });
  $j('a.link.add, #add_tag_label').live('click', function(event) {
    event.preventDefault();
    var tagSubmission = new TagSubmission(this);
    tagSubmission.revealTagAddForm();
  });
  $j('#finish_add_tag_label').live('click', function(event) {
    event.preventDefault();
    var tagSubmission = new TagSubmission(this);
    tagSubmission.hideTagAddForm();
    return false;
  });
  // End Tag Submission //
  // Tag Interactions
  $j('ul#topic_tags_list li.tag a.delete.tag').live('click', function(event) {
    event.preventDefault();
    var tag = new Tag(this);
    tag.removeTag();
  });
  // End Tag //
  // Topic Interactions //
  $j('span.follow_link a.start_following').live('click', function(event) {
    event.preventDefault();
    var topic = new Topic(this);
    topic.follow();
  });
  $j('span.follow_link a.stop_following').live('click', function(event) {
    event.preventDefault();
    var topic = new Topic(this);
    topic.unfollow();
  });
  $j('a.initiate.edit_subject.link').live('click', function(event) {
    event.preventDefault();
    var topic = new Topic(this);
    topic.initiateTopicSubjectEdit();
  });
  $j('a.cancel.edit_subject.link').live('click', function(event) {
    event.preventDefault();
    var topic = new Topic(this);
    topic.cancelTopicSubjectEditing();
  });
  $j('span.me_too a.me_too').live('click', function(event) {
    event.preventDefault();
    var topic = new Topic(this);
    topic.plusOne();
  });

  // End Topic Interactions //
  // Promoted Reply Interactions
  $j('a.demote.promoted_reply').live('click', function(event) {
    event.preventDefault();
    var promotedReply = new PromotedReply(this);
    promotedReply.remove();
  });

  // End Promoted Reply Interactions
  // Reply Interactions //
  $j('a.comment.expand.link').live('click', function(event) {
    event.preventDefault();
    var reply = new Reply(this);
    reply.showNewCommentSubmissionForm();
  });
  $j('a.mark.reply.official.link').live('click', function(event) {
    event.preventDefault();
    var reply = new Reply(this);
    reply.markReplyAsOfficial();
  });
  $j('a.promote.star_link').live('click', function(event) {
    event.preventDefault(this);
    var reply = new Reply(this);
    reply.promoteReply(this);
  });
  $j('a.demote.star_link').live('click', function(event) {
    event.preventDefault(this);
    var reply = new Reply(this);
    reply.demoteReply(this);
  });
  $j('a.edit.reply.link').live('click', function(event) {
    event.preventDefault();
     var reply = new Reply(this);
    reply.initiateReplyEdit();
  });
  $j('a.fork.reply.link').live('click', function(event) {
    event.preventDefault();
     var reply = new Reply(this);
    reply.initiateReplyFork();
  });
  $j('a.reply.delete.link').live('click', function(event) {
    event.preventDefault();
    var reply = new Reply(this);
    if (confirm(/*!<sl:translate>*/'Are you sure you want to delete this reply? This cannot be undone!'/*!</sl:translate>*/)) {
      reply.deleteReply();
    }
  });
  $j('a.reply.remove.link').live('click', function(event) {
    event.preventDefault();
    var reply = new Reply(this);
    reply.removeReply();
  });
  $j('a.reply.restore.link').live('click', function(event) {
    event.preventDefault();
    var reply = new Reply(this);
    reply.restoreReply();
  });
  $j('a.slidedown.comments.link').live('click', function(event) {
    event.preventDefault();
    var reply = new Reply(this);
    reply.expandComments();
  });
  // End Reply //
  // Reply Submission Interactions //
  $j('form.reply.submission.form').live('submit', function(event) {
    event.preventDefault();
    var replySubmission = new ReplySubmission(this);
    replySubmission.loginAndSubmitReply();
  });
  // End Reply Submission //
  // Reply Edit Interactions //
  $j('a.cancel_edit.reply.link').live('click', function(event) {
    event.preventDefault();
    var replyEdit = new ReplyEdit(this);
    replyEdit.cancelEditingReply();
  });
  $j('form.edit.reply').live('submit', function(event) {
    event.preventDefault();
    
    var replyEdit = new ReplyEdit(this);
    replyEdit.loginAndUpdateReply();
  });
  // End Reply Edit //
  // Comment Interactions //
  $j('a.comment.edit.link').live('click', function(event) {
    event.preventDefault();

    var comment = new Comment(this);
    comment.initiateEditComment();
  });
  $j('a.comment.delete.link').live('click', function(event) {
    event.preventDefault();

    if (confirm(/*!<sl:translate>*/'Are you sure you want to delete this comment? This cannot be undone!'/*!</sl:translate>*/)) {
      var comment = new Comment(this);
      comment.deleteComment();
    }
  });
  $j('a.comment.remove.link').live('click', function(event) {
    event.preventDefault();

    var comment = new Comment(this);
    comment.removeComment();
  });
  // End Comment //
  // Comment Edit Interactions //
  $j('li.edit_comment_row a.cancel.link').live('click', function(event) {
    event.preventDefault();

    var commentEdit = new CommentEdit(this);
    commentEdit.resetCommentEditForm();
  });
  $j('form.comment.edit').live('submit', function(event) {
    event.preventDefault();

    var commentEdit = new CommentEdit(this);
    commentEdit.loginAndUpdateComment();
  });
  // End Comment Edit //
  // Comment Submission Interactions //
  $j('a.comment.submission.cancel.link').live('click', function(event) {
    event.preventDefault();

    var submission = new CommentSubmission(this);
    submission.cancelEditing();
    submission.reenableButton();
  });
  $j('textarea.comment.submission').live('focus', function(event) {
    event.preventDefault();
    var submission = new CommentSubmission(this);
    submission.revealCommentForm(); //reveal form actions really change this name
  });

  $j('textarea.comment.submission').autoResize();

  $j('form.comment.submission').live('submit', function(event) {
    event.preventDefault();

    var submission = new CommentSubmission(this);
    submission.loginAndSubmitComment();
  });

  $j('textarea.comment.submission').val(/*!<sl:translate>*/'Write a comment...'/*!</sl:translate>*/);

  // End Comment Submission Interactions //

});





























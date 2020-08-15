var ie = (document.all)? true:false;

if (typeof(console) !== 'undefined') {
  if ('debug' in console) {
    console.debug('Hey, you have your debugger enabled :)');
  } else {
    console.log('Hey, you have your IE debugger enabled :-/');
  }
} else {
  var Console = Class.create();
  Console.prototype = {
    initialize: function() {},
    info: function(){},
    error: function(){},
    debug: function(){},
    warning: function(){},
    log: function(){}
  };

  var console = new Console();
}

trace = function(str) {
  try {
    if(document.all) {
      alert(str)
    } else {
      if ('debug' in console) {
        console.debug.apply(console, arguments);
      } else {
        console.log.apply(console, arguments);
      }
    }
  } catch(e) {}

  return true;
}

Object.extend(Page.prototype, Links)

extend = function(destination, source) {
  for (var property in source) {
    destination[property] = source[property];
  }
  return destination;
}

roll_footer_quotes = function() {
  cur_q = 0;
  qi = setInterval(function() {
    if($('q_'+cur_q)) {
      $('q_'+cur_q).hide();
    }
    if(cur_q<5) {
      cur_q ++;
    } else {
      cur_q = 1;
    }
    if($('q_'+cur_q)) {
      $('q_'+cur_q).show();
    }
  }, 11000);
}

flash_object = function(movieName) {
  var isIE = navigator.appName.indexOf("Microsoft") != -1;
  return (isIE) ? window[movieName] : document[movieName];
}

copy_success = function() {
  // Flash as callback
}

toggle_more = function(link, group) {
  group.immediateDescendants().each(function(el) {
    if(el.hasClassName('collapse')) {
      el.hide();
      if(link != false) link.update('show more...');
      el.removeClassName('collapse')
    } else if(!el.visible()) {
      el.show();
      if(link != false) link.update('...show less');
      el.addClassName('collapse')
    }
  });
}

var current_user = new User();

$j(document).ready(function() {
  roll_footer_quotes();

	page.watch('login', function(){
	  if($('submit_style_buttons')) {
	    $('topic_user_avatar').src = current_user.avatar_url();
	    $('topic_submit_message').show();
	    $('topic_submit_message').update("Welcome back " + current_user.nick() + "! How would you like to submit your topic?");
	    $('topic_login_form').hide();
	    Effect.Appear('submit_style_buttons', {duration: 0.4})
	  }
	});
});

/*
Gradual Transitions
*/
// EaseFromTo (adapted from “Quart.EaseInOut”)
Effect.Transitions.EaseFromTo = function(pos) {
  if ((pos/=0.5) < 1) return 0.5*Math.pow(pos,4);
  return -0.5 * ((pos-=2)*Math.pow(pos,3) - 2);
};
// EaseFrom (adapted from “Quart.EaseIn”)
Effect.Transitions.EaseFrom = function(pos) {
  return Math.pow(pos,4);
};
// EaseTo (adapted from “Quart.EaseOut”)
Effect.Transitions.EaseTo = function(pos) {
  return Math.pow(pos,0.25);
};

function goto_parature(link, task) {
  if(!page.company){ return false; }
  var the_task
  if(!task){ the_task = ""; } else { the_task = task; }

  page.login_and_do(link, function() {
    page.safe_redirect("/" + page.company + "/parature_pass_through?task=" + escape(the_task))
  })

  return false;
}

reply_to_welcome = function(link) {
  page.login_and_do(link, function() {
    link.up('form').submit();
  }, {});
}

//opens a video overlay for blip.tv anywhere it's invoked
function video_launcher(url, width, height){
	contents = "<h2 style='margin:0'>Start your customer community today.</h2><p style=\"margin-bottom:10px\">Questions? Call 1-877-339-3997 or <a href=\"#\" onclick=\"new Ajax.Request('/marketing/web_to_lead_interest', {asynchronous:true, evalScripts:true, method:'get'}); return false;\">request more information</a></p><div class=\"video_container\"><embed src=\"" + url + "\" type=\"application/x-shockwave-flash\" width=\"" + width + "\" height=\"" + height + "\" allowscriptaccess=\"always\" allowfullscreen=\"true\"></embed></div>";
	page.show_mini_modal();
	page.populate_mini_modal(contents);
}

Cookie.set('is_human', true, 30);

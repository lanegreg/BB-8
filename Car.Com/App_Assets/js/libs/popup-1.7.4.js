/*!
 * jQuery Popup Overlay
 *
 * @version 1.7.4
 * @requires jQuery v1.7.1+
 * @link http://vast-engineering.github.com/jquery-popup-overlay/
 */
; (function ($) {

  var $window = $(window)
    , $body = $('body')
    , options = {}
    , zindexvalues = []
    , lastclicked = []
    , scrollbarwidth
    , bodymarginright = null
    , opensuffix = '_open'
    , closesuffix = '_close'
    , stack = []
    , transitionsupport = null
    , opentimer
    , ios = /(iPad|iPhone|iPod)/g.test(navigator.userAgent)
    , doc = document
    , docstyle = doc.body.style
    , settimeout = setTimeout
    , parseint = parseInt
    , parsefloat = parseFloat


  var htmlStr = 'html'
    , ariahiddenStr = 'aria-hidden'
    , arialabelledbyStr = 'aria-labelledby'
    , inlineblockStr = 'inline-block'
    , hiddenStr = 'hidden'
    , visibleStr = 'visible'
    , absoluteStr = 'absolute'
    , relativeStr = 'relative'
    , overflowStr = 'overflow'
    , overlayStr = 'overlay'
    , middleStr = 'middle'
    , centerStr = 'center'
    , topStr = 'top'
    , bottomStr = 'bottom'
    , rightStr = 'right'
    , leftStr = 'left'
    , edgeStr = 'edge'
    , datadashStr = 'data-'
    , marginrightStr = 'margin-right'
    , textalignStr = 'text-align'
    , verticalalignStr = 'vertical-align'
    , popupinitializedStr = 'popup-initialized'
    , popupvisibleStr = 'popup_visible'
    , popupdashvisibleStr = 'popup-visible'
    , popupdashordinalStr = 'popup-ordinal'
    , popupalignStr = 'popup_align'
    , popupoptionsStr = 'popupoptions'
    , popupcontentvisibleStr = 'popup_content_visible'
    , tooltipStr = 'tooltip'
    , transitionStr = 'transition'
    , transitionendStr = 'transitionend'
    , _backgroundStr = '_background'
    , _wrapperStr = '_wrapper'
    , focusStr = 'focus'
    , fixedStr = 'fixed'
    , zindexStr = 'z-index'
    , onehundredpercentStr = '100%'


  var methods = {

    _init: function (el) {
      var $el = $(el)
        , options = $el.data(popupoptionsStr);

      lastclicked[el.id] = false;
      zindexvalues[el.id] = 0;

      if (!$el.data(popupinitializedStr)) {
        $el.attr(datadashStr + popupinitializedStr, 'true');
        methods._initonce(el);
      }

      if (options.autoopen) {
        settimeout(function () {
          methods.show(el, 0);
        }, 0);
      }
    },

    _initonce: function (el) {
      var $el = $(el)
        , $wrapper
        , options = $el.data(popupoptionsStr)
        , css;

      bodymarginright = parseint($body.css(marginrightStr), 10);
      transitionsupport = docstyle.webkitTransition !== undefined ||
                          docstyle.MozTransition !== undefined ||
                          docstyle.msTransition !== undefined ||
                          docstyle.OTransition !== undefined ||
                          docstyle.transition !== undefined;

      if (options.type == tooltipStr) {
        options.background = false;
        options.scrolllock = false;
      }

      if (options.backgroundactive) {
        options.background = false;
        options.blur = false;
        options.scrolllock = false;
      }

      if (options.scrolllock) {
        // Calculate the browser's scrollbar width dynamically
        var parent
          , child;

        if (typeof scrollbarwidth === 'undefined') {
          parent = $('<div style="width:50px;height:50px;overflow:auto"><div/></div>').appendTo('body');
          child = parent.children();
          scrollbarwidth = child.innerWidth() - child.height(99).innerWidth();
          parent.remove();
        }
      }

      if (!$el.attr('id')) {
        $el.attr('id', 'j-popup-' + parseint((Math.random() * 100000000), 10));
      }

      $el.addClass('popup_content');

      $body.prepend(el);

      $el.wrap('<div id="' + el.id + _wrapperStr + '" class="popup_wrapper" />');

      $wrapper = $('#' + el.id + _wrapperStr);

      $wrapper.css({
        opacity: 0,
        visibility: hiddenStr,
        position: absoluteStr
      });

      // Make div clickable in iOS
      if (ios) {
        $wrapper.css('cursor', 'pointer');
      }

      if (options.type == overlayStr) {
        $wrapper.css(overflowStr, 'auto');
      }

      $el.css({
        opacity: 0,
        visibility: hiddenStr,
        display: inlineblockStr
      });

      if (options.setzindex && !options.autozindex) {
        $wrapper.css(zindexStr, '100001');
      }

      if (!options.outline) {
        $el.css('outline', 'none');
      }

      if (options.transition) {
        $el.css(transitionStr, options.transition);
        $wrapper.css(transitionStr, options.transition);
      }

      // Hide popup content from screen readers initially
      $el.attr(ariahiddenStr, true);

      if ((options.background) && (!$('#' + el.id + _backgroundStr).length)) {

        $body.prepend('<div id="' + el.id + _backgroundStr + '" class="popup' + _backgroundStr + '"></div>');

        var $background = $('#' + el.id + _backgroundStr);

        $background.css({
          opacity: 0,
          visibility: hiddenStr,
          backgroundColor: options.color,
          position: fixedStr,
          top: 0,
          right: 0,
          bottom: 0,
          left: 0
        });

        if (options.setzindex && !options.autozindex) {
          $background.css(zindexStr, '100000');
        }

        if (options.transition) {
          $background.css(transitionStr, options.transition);
        }
      }

      if (options.type == overlayStr) {
        $el.css({
          textAlign: leftStr,
          position: relativeStr,
          verticalAlign: middleStr
        });

        //==============================================================
        //position changed from fixedStr by Bill, Mioko and Sam 20150513
        //==============================================================

        if (ios) {
          
          css = {
            position: absoluteStr,
            width: onehundredpercentStr,
            height: onehundredpercentStr,
            top: 0,
            left: 0,
            textAlign: centerStr
          }
        } else {
          
          css = {
            position: fixedStr,
            width: onehundredpercentStr,
            height: onehundredpercentStr,
            top: 0,
            left: 0,
            textAlign: centerStr
          }
        }
        

        if (options.backgroundactive) {
          css.position = relativeStr;
          css.height = '0';
          css.overflow = visibleStr;
        }

        $wrapper.css(css);

        // CSS vertical align helper
        $wrapper.append('<div class="' + popupalignStr + '" />');

        $('.' + popupalignStr).css({
          display: inlineblockStr,
          verticalAlign: middleStr,
          height: onehundredpercentStr
        });
      }

      // Add WAI ARIA role to announce dialog to screen readers
      $el.attr('role', 'dialog');

      var openelement = (options.openelement) ? options.openelement : ('.' + el.id + opensuffix);

      $(openelement).each(function (i, item) {
        $(item).attr(datadashStr + popupdashordinalStr, i);

        if (!item.id) {
          $(item).attr('id', 'open_' + parseint((Math.random() * 100000000), 10));
        }
      });

      // Set aria-labelledby (if aria-label or aria-labelledby is not set in html)
      if (!($el.attr(arialabelledbyStr) || $el.attr('aria-label'))) {
        $el.attr(arialabelledbyStr, $(openelement).attr('id'));
      }

      // Show and hide tooltips on hover
      if (options.action == 'hover') {
        options.keepfocus = false;

        // Handler: mouseenter, focusin
        $(openelement).on('mouseenter', function (event) {
          methods.show(el, $(this).data(popupdashordinalStr));
        });

        // Handler: mouseleave, focusout
        $(openelement).on('mouseleave', function (event) {
          methods.hide(el);
        });

      } else {

        // Handler: Show popup when clicked on `open` element
        $(doc).on('click', openelement, function (event) {
          event.preventDefault();

          var ord = $(this).data(popupdashordinalStr);
          settimeout(function () { // setTimeout is to allow `close` method to finish (for issues with multiple tooltips)
            methods.show(el, ord);
          }, 0);
        });
      }

      if (options.detach) {
        $el.hide().detach();
      } else {
        $wrapper.hide();
      }
    },

    /**
     * Show method
     *
     * @param {object} el - popup instance DOM node
     * @param {number} ordinal - order number of an `open` element
     */
    show: function (el, ordinal) {
      var $el = $(el);

      // this is a UI fix.
      if (ios) {
        
        $body.addClass('noscroll-ios');
      } else {

        $body.addClass('noscroll');
      }

      if ($el.data(popupdashvisibleStr)) return;

      // Initialize if not initialized. Required for: $('#popup').popup('show')
      if (!$el.data(popupinitializedStr)) {
        methods._init(el);
      }
      $el.attr(datadashStr + popupinitializedStr, 'true');

      var options = $el.data(popupoptionsStr)
        , $wrapper = $('#' + el.id + _wrapperStr)
        , $background = $('#' + el.id + _backgroundStr);

      // `beforeopen` callback event
      callback(el, ordinal, options.beforeopen);

      // Remember last clicked place
      lastclicked[el.id] = ordinal;

      // Add popup id to popup stack
      settimeout(function () {
        stack.push(el.id);
      }, 0);

      // Calculating maximum z-index
      if (options.autozindex) {

        var elements = doc.getElementsByTagName('*')
          , len = elements.length
          , maxzindex = 0;

        for (var i = 0; i < len; i++) {

          var elementzindex = $(elements[i]).css(zindexStr);

          if (elementzindex !== 'auto') {

            elementzindex = parseint(elementzindex, 10);

            if (maxzindex < elementzindex) {
              maxzindex = elementzindex;
            }
          }
        }

        zindexvalues[el.id] = maxzindex;

        // Add z-index to the background
        if (options.background) {
          if (zindexvalues[el.id] > 0) {
            $('#' + el.id + _backgroundStr).css({
              zIndex: (zindexvalues[el.id] + 1)
            });
          }
        }

        // Add z-index to the wrapper
        if (zindexvalues[el.id] > 0) {
          $wrapper.css({
            zIndex: (zindexvalues[el.id] + 2)
          });
        }
      }

      if (options.detach) {
        $wrapper.prepend(el);
        $el.show();
      } else {
        $wrapper.show();
      }

      opentimer = settimeout(function () {
        $wrapper.css({
          visibility: visibleStr,
          opacity: 1
        });

        $(htmlStr).addClass(popupvisibleStr).addClass(popupvisibleStr = '_' + el.id);
        $el.addClass(popupcontentvisibleStr);
      }, 20); // 20ms required for opening animation to occur in FF

      // Disable background layer scrolling when popup is opened
      if (options.scrolllock) {
        $body.css(overflowStr, hiddenStr);
        if ($body.height() > $window.height()) {
          $body.css(marginrightStr, bodymarginright + scrollbarwidth);
        }
      }

      if (options.backgroundactive) {
        //calculates the vertical align
        $el.css({
          top: (
              $window.height() - (
                  $el.get(0).offsetHeight +
                  parseint($el.css('margin-top'), 10) +
                  parseint($el.css('margin-bottom'), 10)
              )
          ) / 2 + 'px'
        });
      }

      $el.css({
        'visibility': visibleStr,
        'opacity': 1
      });

      // Show background
      if (options.background) {
        $background.css({
          'visibility': visibleStr,
          'opacity': options.opacity
        });

        // Fix IE8 issue with background not appearing
        settimeout(function () {
          $background.css({
            'opacity': options.opacity
          });
        }, 0);
      }

      $el.data(popupdashvisibleStr, true);

      // Position popup
      methods.reposition(el, ordinal);

      // Remember which element had focus before opening a popup
      $el.data('focusedelementbeforepopup', doc.activeElement);

      // Handler: Keep focus inside dialog box
      if (options.keepfocus) {
        // Make holder div focusable
        $el.attr('tabindex', -1);

        // Focus popup or user specified element.
        // Initial timeout of 50ms is set to give some time to popup to show after clicking on
        // `open` element, and after animation is complete to prevent background scrolling.
        settimeout(function () {
          if (options.focuselement === 'closebutton') {
            $('#' + el.id + ' .' + el.id + closesuffix + ':first').trigger(focusStr);
          } else if (options.focuselement) {
            $(options.focuselement).trigger(focusStr);
          } else {
            $el.trigger(focusStr);
          }
        }, options.focusdelay);

      }

      // Hide main content from screen readers
      $(options.pagecontainer).attr(ariahiddenStr, true);

      // Reveal popup content to screen readers
      $el.attr(ariahiddenStr, false);

      callback(el, ordinal, options.onopen);

      if (transitionsupport) {
        $wrapper.one(transitionendStr, function () {
          callback(el, ordinal, options.opentransitionend);
        });
      } else {
        callback(el, ordinal, options.opentransitionend);
      }
    },

    /**
     * Hide method
     *
     * @param {object} el - popup instance DOM node
     */
    hide: function (el) {
      if (opentimer) clearTimeout(opentimer);

      // this is a UI fix.
      $body.removeClass('noscroll-ios');
      $body.removeClass('noscroll');

      var $el = $(el)
        , options = $el.data(popupoptionsStr)
        , $wrapper = $('#' + el.id + _wrapperStr)
        , $background = $('#' + el.id + _backgroundStr);

      $el.data(popupdashvisibleStr, false);


      if (stack.length === 1) {
        $(htmlStr).removeClass(popupvisibleStr).removeClass(popupvisibleStr + '_' + el.id);
      } else {
        if ($(htmlStr).hasClass(popupvisibleStr + '_' + el.id)) {
          $(htmlStr).removeClass(popupvisibleStr + '_' + el.id);
        }
      }

      // Remove last opened popup from the stack
      stack.pop();

      if ($(htmlStr).hasClass(popupcontentvisibleStr)) {
        $el.removeClass(popupcontentvisibleStr);
      }

      //if (options.keepfocus) {
      //  // Focus back on saved element
      //  settimeout(function () {
      //    if ($($el.data('focusedelementbeforepopup')).is(':visible')) {
      //      $el.data('focusedelementbeforepopup').focus();
      //    }
      //  }, 0);
      //}

      // Hide popup
      $wrapper.css({
        'visibility': hiddenStr,
        'opacity': 0
      });
      $el.css({
        'visibility': hiddenStr,
        'opacity': 0
      });

      // Hide background
      if (options.background) {
        $background.css({
          'visibility': hiddenStr,
          'opacity': 0
        });
      }

      // Reveal main content to screen readers
      $(options.pagecontainer).attr(ariahiddenStr, false);

      // Hide popup content from screen readers
      $el.attr(ariahiddenStr, true);

      // `onclose` callback event
      callback(el, lastclicked[el.id], options.onclose);

      if (transitionsupport && $el.css('transition-duration') !== '0s') {
        $el.one(transitionendStr, function (e) {

          if (!($el.data(popupdashvisibleStr))) {
            if (options.detach) {
              $el.hide().detach();
            } else {
              $wrapper.hide();
            }
          }

          // Re-enable scrolling of background layer
          if (options.scrolllock) {
            settimeout(function () {
              $body.css({
                overflow: visibleStr,
                'margin-right': bodymarginright
              });
            }, 10); // 10ms added for CSS transition in Firefox which doesn't like overflow:auto
          }

          callback(el, lastclicked[el.id], options.closetransitionend);
        });
      } else {
        if (options.detach) {
          $el.hide().detach();
        } else {
          $wrapper.hide();
        }

        // Re-enable scrolling of background layer
        if (options.scrolllock) {
          settimeout(function () {
            $body.css({
              overflow: visibleStr,
              'margin-right': bodymarginright
            });
          }, 10); // 10ms added for CSS transition in Firefox which doesn't like overflow:auto
        }

        callback(el, lastclicked[el.id], options.closetransitionend);
      }

    },

    /**
     * Toggle method
     *
     * @param {object} el - popup instance DOM node
     * @param {number} ordinal - order number of an `open` element
     */
    toggle: function (el, ordinal) {
      if ($(el).data(popupdashvisibleStr)) {
        methods.hide(el);
      } else {
        settimeout(function () {
          methods.show(el, ordinal);
        }, 0);
      }
    },

    /**
     * Reposition method
     *
     * @param {object} el - popup instance DOM node
     * @param {number} ordinal - order number of an `open` element
     */
    reposition: function (el, ordinal) {
      var $el = $(el)
        , options = $el.data(popupoptionsStr)
        , $wrapper = $('#' + el.id + _wrapperStr);

      ordinal = ordinal || 0;

      // Tooltip type
      if (options.type == tooltipStr) {
        $wrapper.css({
          'position': absoluteStr
        });

        var $tooltipanchor
          , popupordinalStr = '[data-' + popupdashordinalStr + '="' + ordinal + '"]'

        if (options.tooltipanchor) {
          $tooltipanchor = $(options.tooltipanchor);
        } else if (options.openelement) {
          $tooltipanchor = $(options.openelement).filter(popupordinalStr);
        } else {
          $tooltipanchor = $('.' + el.id + opensuffix + popupordinalStr);
        }

        var linkOffset = $tooltipanchor.offset();

        // Horizontal position for tooltip
        if (options.horizontal == rightStr) {
          $wrapper.css(leftStr, linkOffset.left + $tooltipanchor.outerWidth() + options.offsetleft);
        } else if (options.horizontal == leftStr + edgeStr) {
          $wrapper.css(leftStr, linkOffset.left + $tooltipanchor.outerWidth() - $tooltipanchor.outerWidth() + options.offsetleft);
        } else if (options.horizontal == leftStr) {
          $wrapper.css(rightStr, $window.width() - linkOffset.left - options.offsetleft);
        } else if (options.horizontal == rightStr + edgeStr) {
          $wrapper.css(rightStr, $window.width() - linkOffset.left - $tooltipanchor.outerWidth() - options.offsetleft);
        } else {
          $wrapper.css(leftStr, linkOffset.left + ($tooltipanchor.outerWidth() / 2) - ($el.outerWidth() / 2) - parsefloat($el.css('marginLeft')) + options.offsetleft);
        }

        // Vertical position for tooltip
        if (options.vertical == bottomStr) {
          $wrapper.css(topStr, linkOffset.top + $tooltipanchor.outerHeight() + options.offsettop);
        } else if (options.vertical == bottomStr + edgeStr) {
          $wrapper.css(topStr, linkOffset.top + $tooltipanchor.outerHeight() - $el.outerHeight() + options.offsettop);
        } else if (options.vertical == topStr) {
          $wrapper.css(bottomStr, $window.height() - linkOffset.top - options.offsettop);
        } else if (options.vertical == topStr + edgeStr) {
          $wrapper.css(bottomStr, $window.height() - linkOffset.top - $el.outerHeight() - options.offsettop);
        } else {
          $wrapper.css(topStr, linkOffset.top + ($tooltipanchor.outerHeight() / 2) - ($el.outerHeight() / 2) - parsefloat($el.css('marginTop')) + options.offsettop);
        }

        // Overlay type
      } else if (options.type == overlayStr) {

        // Horizontal position for overlay
        if (options.horizontal) {
          $wrapper.css(textalignStr, options.horizontal);
        } else {
          $wrapper.css(textalignStr, centerStr);
        }

        // Vertical position for overlay
        if (options.vertical) {
          $el.css(verticalalignStr, options.vertical);
        } else {
          $el.css(verticalalignStr, middleStr);
        }
      }
    }

  };

  /**
   * Callback event calls
   *
   * @param {object} el - popup instance DOM node
   * @param {number} ordinal - order number of an `open` element
   * @param {function} func - callback function
   */
  var callback = function (el, ordinal, func) {
    var options = $(el).data(popupoptionsStr)
      , openelement = (options.openelement) ? options.openelement : ('.' + el.id + opensuffix)
      , elementclicked = $(openelement + '[data-' + popupdashordinalStr + '="' + ordinal + '"]');

    if (typeof func == 'function') {
      func.call($(el), el, elementclicked);
    }
  };

  // Hide popup if ESC key is pressed
  $(doc).on('keydown', function (event) {
    if (stack.length) {
      var elementId = stack[stack.length - 1]
        , el = doc.getElementById(elementId);

      if ($(el).data(popupoptionsStr).escape && event.keyCode == 27) {
        methods.hide(el);
      }
    }
  });

  // Hide popup on click
  $(doc).on('click', function (event) {
    if (stack.length) {
      var elementId = stack[stack.length - 1]
        , el = doc.getElementById(elementId)
        , closeButton = ($(el).data(popupoptionsStr).closeelement) ? $(el).data(popupoptionsStr).closeelement : ('.' + el.id + closesuffix);

      // Click on Close button
      if ($(event.target).closest(closeButton).length) {
        event.preventDefault();
        methods.hide(el);
      }

      // Click outside of popup
      if ($(el).data(popupoptionsStr).blur && !$(event.target).closest('#' + elementId).length && event.which !== 2) {

        methods.hide(el);

        if ($(el).data(popupoptionsStr).type === overlayStr) {
          event.preventDefault(); // iOS will trigger click on the links below the overlay when clicked on the overlay if we don't prevent default action
        }
      }
    }
  });

  // Keep keyboard focus inside of popup
  $(doc).on('focusin', function (event) {
    if (stack.length) {
      var elementId = stack[stack.length - 1]
        , el = doc.getElementById(elementId);

      if ($(el).data(popupoptionsStr).keepfocus) {
        if (!el.contains(event.target)) {
          event.stopPropagation();
          el.focus();
        }
      }
    }
  });

  /**
   * Plugin API
   */
  $.fn.popup = function (customoptions) {
    return this.each(function () {
      var $el = $(this);

      if (typeof customoptions === 'object') {  // e.g. $('#popup').popup({'color':'blue'})
        var opt = $.extend({}, $.fn.popup.defaults, customoptions);
        $el.data(popupoptionsStr, opt);
        options = $el.data(popupoptionsStr);

        methods._init(this);

      } else if (typeof customoptions === 'string') { // e.g. $('#popup').popup('hide')
        if (!($el.data(popupoptionsStr))) {
          $el.data(popupoptionsStr, $.fn.popup.defaults);
          options = $el.data(popupoptionsStr);
        }

        methods[customoptions].call(this, this);

      } else { // e.g. $('#popup').popup()
        if (!($el.data(popupoptionsStr))) {
          $el.data(popupoptionsStr, $.fn.popup.defaults);
          options = $el.data(popupoptionsStr);
        }

        methods._init(this);
      }  
    });
  };


  $.fn.popup.defaults = {
    type: overlayStr,
    autoopen: false,
    background: true,
    backgroundactive: false,
    color: 'black',
    opacity: '0.5',
    horizontal: centerStr,
    vertical: middleStr,
    offsettop: 0,
    offsetleft: 0,
    escape: true,
    blur: true,
    setzindex: true,
    autozindex: false,
    scrolllock: false,
    keepfocus: true,
    focuselement: null,
    focusdelay: 50,
    outline: false,
    pagecontainer: null,
    detach: false,
    openelement: null,
    closeelement: null,
    transition: null,
    tooltipanchor: null,
    beforeopen: null,
    onclose: null,
    onopen: null,
    opentransitionend: null,
    closetransitionend: null
  };

})(jQuery);
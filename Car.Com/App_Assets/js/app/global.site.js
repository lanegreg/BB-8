/**
*	module that represents any logic which may be used on most pages and browser will cache
*	Autobytel (c)2015
*
*	@version: 0.0.1 
*	@last: 05/22/2015
*	@author: Greg Lane
*/


'use strict';

!function (win, $, abt, undefined) {
  //#region - Module level vars, minification enhancements, and DOM caching
  var utils = abt.UTILS
    , env = abt.ENV
    , staticJs = env.staticEndpoint
    , mergeObjs = utils.mergeObjs
    , click_event = 'click'
    , length = 'length'
    , body = 'body'
    , $body = $(body)
    , empty = ''
    
  env.libsPath = staticJs + '/libs'

  //#endregion


  $body.addClass(abt.DEVICE.getType())


  //#region - jQuery Related

  $.getCachedScript = function (url, options) {
    options = $.extend(options || {}, {
      dataType: "script",
      cache: true,
      url: url
    })

    return $.ajax(options)
  }

  $.getQuerystringValueByKey = function (key) {
    var keyvals = {}

    $.each(win.location.search.slice(1).split('&'), function (idx, item) {
      var parts = decodeURIComponent(item).split('=')
      keyvals[parts[0]] = parts[length] > 1 ? parts[1] : undefined
    })

    return keyvals[key]
  }

  //#endregion


  //#region - Extend our namespace with additional functionality

  var history = win.history

  utils.mergeObjs(env, {
    browser: {
      supportsHistoryApi: !!(history && history.pushState)
    }
  })

  //#endregion


  //#region - Namespace all our global modules

  var utilsModule = require('../modules/abt-utils')
    , adsModule = require('../modules/abt-ads')
    , trkModule = require('../modules/abt-trk')

  mergeObjs(utils, utilsModule)
  mergeObjs(abt.ADS, adsModule)
  mergeObjs(abt.TRK, trkModule)

  abt.CACHE = require('../modules/abt-cache')
  abt.CACHE.init()
  
  abt.Eventify = require('../modules/abt-eventify')
  abt.PERSONA = require('../modules/abt-persona')

  //#endregion


  //#region - Wireup Global Events

  // UX related - This handles hash href animated scrolling.
  $body.on(click_event, 'a[href*=#]:not([href=#])', function (e) {
    e.preventDefault()

    var self = this
      , regex = /^\//
      , location = win.location

    if (location.pathname.replace(regex, empty) === self.pathname.replace(regex, empty) && location.hostname === self.hostname) {
      var $target = $(self.hash)
        , offsetTweak = $target.data('offset-tweak') || 0
            
      if ($target[length]) {
        $('html,' + body).animate({
          scrollTop: $target.offset().top + offsetTweak
        }, 1000) // 1 sec
      }
    }
  })

  //#endregion


  //#region - Mapping of javascript filenames to aliases

  env.getJsFileNameByAlias = function (alias) {
    var min_js = '.min.js'
      , common_dot = 'common.'

      , filenames = {
          // these are 3rd party aliases
          slider: 'unslider-1.0.0-c1.0' + min_js,
          evmove: 'jquery.event.move-1.3.6' + min_js,
          popup: 'popup-1.7.4' + min_js,
          picfill: 'picturefill-2.1.0' + min_js,
          masonry: 'salvattore-1.0.8' + min_js,
          
          // these are common aliases
          ofrbstrap: common_dot + 'offerbootstrap',
          ofrsystem: common_dot + 'offersystem',
          c4swidget: common_dot + 'c4swidget',
          viewedrecentlywidget: common_dot + 'viewedrecentlywidget',
          c4svwrecwdg: common_dot + 'c4srecentlyviewedwdg',

          // these are aliases to external files
          salemove: '//api.salemove.com/salemove_integration.js?v=20150804'
      }

    return filenames[alias] || empty
  }

  //#endregion


  //#region - Bootstrap the Leads & Offers system

  var offerbootstrapFilename = env.getJsFileNameByAlias('ofrbstrap')
  $.getCachedScript(env.getScriptUriByName(offerbootstrapFilename))

  //#endregion


  //#region - Bootstrap the Leads & Offers system



  //#endregion

}(window, jQuery, ABT, []._)
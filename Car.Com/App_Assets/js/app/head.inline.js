/**
 *	Autobytel (c)2015
 *	Descr:    Head Inline Code
 *  Purpose:  All script that needs to be inlined in the page's <head> tag 
 *
 *	@version: 0.0.1 
 *	@last: 05/22/2015
 *	@author: Greg Lane
 */

'use strict';

(function (w) {
  w.ABT = w.ABT || {};

  var doc = w.document
    , ajax = require('../modules/ajax')
    , kizzy = require('../modules/kizzy')
    , abt = w.ABT
    , assets = abt.ENV.staticAssets
    , queue = []
    , type = 'type'
    , cache = kizzy('device')
    , cachedDevice = cache.get(type)


  var env = abt.ENV
    , utils = abt.UTILS = {}
    , device = abt.DEVICE = { isKnown: false }


  var _mergeObjs = function (obj1, obj2) {
    obj1 = obj1 || {}
    obj2 = obj2 || {}

    for (var propName in obj2) {
      obj1[propName] = obj2[propName]
    }
    return obj1
  }

  utils.mergeObjs = _mergeObjs


  // Ensure that our primary objects exist early.
  var set = function (d) { _mergeObjs(this, d); return this }
  abt.ADS = {}
  abt.TRK = {}
  abt.OFR = {}
  abt.PAGE = {}
  abt.PAGE.set = set
  abt.WDG = {}
  abt.WDG.set = set

  
  abt.ajax = ajax
  abt.storage = kizzy
  abt.noop = function() {}

  abt.ready = function(callback) {
    if (device.isKnown) {
      var cb = callback || abt.noop
      cb()
    }
    else {
      queue.push(callback)
    }
  }


  _mergeObjs(device, {
    getType: function() {
      return this.isDesk ? 'desk' : (this.isTabl ? 'tabl' : 'mobi')
    }
  });

  _mergeObjs(env, {
    getScriptUri: function () {
      return getAsset('js')
    },
    getScriptUriByName: function (name) {
      return getAsset('js', name)
    },
    getStylesUri: function () {
      return getAsset('css')
    }
  });

  var getAsset = function (fileType, name) {
    
    var filteredAssets = assets.filter(function (el) {
      var typeDeviceMatch = el.file_type === fileType && el.device_type === device.getType()

      if (name) {
        return typeDeviceMatch && el.page_name === name
      }

      return typeDeviceMatch && el.page_name.slice(0, 6) !== 'common'
    })

    if (filteredAssets.length < 1) {
      return ''
    }
    else {
      var asset = filteredAssets[0]

      var assetPath =
        env.staticEndpoint + '/' +
        fileType + '/' +
        asset.page_name + '.' +
        asset.device_type + '-' +
        asset.hash + '.min.' +
        asset.file_type

      if (env.isDev) {
        return assetPath.replace('-' + asset.hash, '')
      }
      else {
        return assetPath
      }
    }
  }

  var deviceKnownHandler = function () {
    var href = 'href'
      , elm = doc.getElementById('main_css')
      , setAttrib = elm.setAttribute.bind(elm)

    // set the stylesheet href and trigger the load
    setAttrib(href, env.getStylesUri())

    // run all other callbacks that are queued
    for (var i = 0; i < queue.length; i++) {
      (queue[i] || abt.noop)()
    }
  }

  // determine device type by looking at local cache or pinging server
  if (cachedDevice) {
    _mergeObjs(device, cachedDevice)
    device.isKnown = true
    deviceKnownHandler()
  }
  else {
    ajax('/api/device/detect/', function(resp) {
      var data = JSON.parse(resp).data
      _mergeObjs(device, data)
      device.isKnown = true
      deviceKnownHandler()
      cache.set(type, data, 600000) // (in ms) expire local-cached device-props every 10 mins
    })
  }

})(window)

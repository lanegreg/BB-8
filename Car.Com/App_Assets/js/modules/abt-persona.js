/**
 *	Autobytel (c)2015
 *	Module:   global persona module
 *  Purpose:  To provide client-side persona persistence capabilities.
 *
 *	@version: 0.0.1 
 *	@last: 01/01/2015
 *	@author: Greg Lane
 */


var persona = function (win, $, abt, undefined) {
  //#region - Module level vars and minification enhancements
  var isZip = abt.UTILS.isZip
    , cacheModule = abt.CACHE
    , true_bool = !0
    , false_bool = !true_bool
    , cache
    , changedelegate = new abt.Eventify()
    , storageName = 'persona'
    , cacheKey = 'data'
    , change_event = 'change'
    , string_str = 'string'
    , isPlainObject = $.isPlainObject
    , extend = $.extend
    , _personaData
    , queryCacheInterval = 3e3 // 3 seconds (in ms)

  //#endregion



  //#region - Default data structures

  var _defaultPersonaData = {
    zipcode: ''
    //comparables: {
    //  list: [],
    //  features: [],
    //  sortBy: {
    //    feature: '',
    //    dir: 'desc'
    //  }
    //}
  }

  //#endregion

  
  var serviceState = function () {
    var isReady = false_bool

    return {
      check: function () {
        if (!isReady)
          throw abt.UTILS.SVC_ERR_MSG
      },
      set: function (isready) {
        isReady = isready
      }
    }
  }()


  //#region - Private funcs

  var _set = function(obj) {
    _personaData = extend(true_bool, _personaData, obj)
    cache.set(cacheKey, _personaData, cache.permanent)
    changedelegate.trigger(_personaData)
  }


  var _queryCache = function() {
    _personaData = cache.get(cacheKey) || _defaultPersonaData

    setTimeout(function() {
      _queryCache()
    }.bind(this), queryCacheInterval)
  }

  //#endregion


  return {
    isReady: function () {
      var deferred = $.Deferred()

      $.when(cacheModule.init())
        .then(function () {
          cache = cacheModule.storage(storageName)
        })
        .then(function() {
          _queryCache()
          serviceState.set(true_bool)
          deferred.resolve(undefined)
        })

      return deferred.promise()
    },

    get: function () {
      serviceState.check()
      return _personaData
    },

    set: function (key, val) {
      serviceState.check()

      //if (typeof key !== string_str) return this

      if (key === 'zipcode' && isZip(val)) {
        _set({ zipcode: val })
      } else /*if (key === 'comparables' && isPlainObject(val))*/ {
        //_set({ comparables: extend(false_bool, _personaData.comparables, val) })
        _set(key)
      }

      return this
    },

    on: function (event, callback) {
      if (event === change_event)
        changedelegate.register(callback)
    }
  }
}(window, jQuery, ABT, []._)


module.exports = persona
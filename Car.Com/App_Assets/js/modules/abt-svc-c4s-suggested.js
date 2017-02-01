/**
 *	Autobytel (c)2015
 *	Module:   Cars-for-Sale Suggested Vehicles Service
 *  Purpose:  Service to provide inventory search capability.
 *
 *	@version: 0.0.1 
 *	@last: 08/28/2015
 *	@author: Greg Lane
 */


var suggestedC4sService = function (win, $, abt, undefined) {
  //#region - Module level vars and minification enhancements
  var cacheModule = abt.CACHE
    , personaService = abt.PERSONA
    , utils = abt.UTILS
    , resCache
    , reqCache
    , basePathForService = '/api/cars-for-sale'
    , Eventify = abt.Eventify
    , isPlainObject = $.isPlainObject
    , extend = $.extend
    , change_event = 'change'
    , ready_event = 'ready'
    , JSON = win.JSON
    , jsonStringify = JSON.stringify
    //, jsonParse = JSON.parse
    , pipe_separator = '|'
    , true_bool = !0
    , false_bool = !true_bool
    , suggC4s_prefix = 'sugg_c4s_'

  //#endregion



  //#region - Private funcs

  var _getFilterDomains = function(skipServiceCheck) {
    if (!skipServiceCheck)
      serviceState.check()

    var url = basePathForService + '/filter-domains/'
      , ttl = 9e5 // 15 minutes (in ms)
      , deferred = $.Deferred()
      , cacheKey = 'GET:' + url
      , cacheVal = resCache.get(cacheKey)

    if (cacheVal) {
      deferred.resolve(cacheVal.data)
    }
    else {
      $.get(url)
        .done(function(resp) {
          resCache.set(cacheKey, resp, ttl)
          deferred.resolve(resp.data)
        })
        .fail(function() {
          deferred.resolve(undefined)
        })
    }

    return deferred.promise()
  }


  var mergeDefaultCriteriaWithCurrentFilterDomains = function (filterDomains) {
    var years = filterDomains.years
      , matchVal = 'matchVal'
      , loYear = years[0][matchVal]
      , hiYear = years.slice(-1)[0][matchVal]
      , criteriaDefault = criteria.default

    criteriaDefault.filters.year_range = loYear + pipe_separator + hiYear
  }


  var serviceState = function () {
    var readyDelegate = new Eventify()
      , isReady = false_bool

    return {
      check: function () {
        if (!isReady)
          throw abt.UTILS.SVC_ERR_MSG
      },

      set: function (isready) {
        isReady = isready

        if (isReady)
          readyDelegate.trigger()
      },

      on: function (event, callback) {
        if (event === ready_event) {
          if (!isReady)
            readyDelegate.register(callback)
          else
            callback()
        }
      }
    }
  }()

  //#endregion


  //#region - Criteria Model

  var criteria = function () {
    var changeDelegate = new Eventify()
      , _criteriaObj = {}
      , defaultItemsPerPage = 5


    //#region - Private funcs

    var _cacheCriteria = function () {
      reqCache.set(criteria.cacheKey, _criteriaObj, reqCache.permanent)
    }


    var _triggerChangeEvent = function () {
      changeDelegate.trigger(_criteriaObj)
    }


    var allPropNamesAreTheSameForBothObjects = function (obj1, obj2) {
      var arr1 = arrayifyProperties(obj1)
        , arr2 = arrayifyProperties(obj2)

      return jsonStringify(arr1) === jsonStringify(arr2)
    }


    var arrayifyProperties = function(obj) {
      var arr = []

      for (var prop in obj) {
        var value = obj[prop]
        arr.push(prop)

        if (isPlainObject(value)) {
          arr = arr.concat(arrayifyProperties(value))
        }
      }

      return arr.sort()
    }

    //#endregion


    return {
      get: function () {
        serviceState.check()
        return _criteriaObj
      },

      set: function (obj) {
        serviceState.check()

        if (isPlainObject(obj)) {
          var newObj = extend(true_bool, {}, _criteriaObj, obj)
            , isValid = this.isValidStruct(newObj)

          if (isValid) {
            _criteriaObj = newObj
            _cacheCriteria()

            _triggerChangeEvent()
          }

          return isValid
        }

        return false_bool
      },

      reset: function () {
        serviceState.check()

        _criteriaObj = extend(true_bool, {}, this.default, { zipcode: personaService.get().zipcode || '' })

        _cacheCriteria()
        _triggerChangeEvent()

        return this
      },

      on: function (event, callback) {
        if (event === change_event)
          changeDelegate.register(callback)
      },

      isValidStruct: function (obj) {
        return allPropNamesAreTheSameForBothObjects(this.default, obj)
      },

      cacheKey: 'criteria',

      defaultItemsPerPage: defaultItemsPerPage,

      "default": {
        zipcode: '',
        vdp_price: '',
        vdp_mileage: '',
        radius_miles: 100,
        page: {
          current: 1,
          items_per_page: ''
        },
        sort: {
          by: 'suggested_ranking',
          dir: 'asc'
        },
        exclusions: {
          dealers: ''
        },
        filters: {
          price_range: '',
          mileage_range: '',
          year_range: '',
          make_models: ''
        }
      },

      packTemplate: {
        zipcode: 'z',
        vdp_price: 'vp',
        vdp_mileage: 'vm',
        radius_miles: 'rm',
        page: {
          '@pname': 'p',
          current: 'c',
          items_per_page: 'ipp'
        },
        sort: {
          '@pname': 's',
          by: 'b',
          dir: 'd'
        },
        exclusions: {
          '@pname': 'e',
          dealers: 'dls'
        },
        filters: {
          '@pname': 'f',
          price_range: 'pr',
          mileage_range: 'mr',
          year_range: 'yr',
          make_models: 'mm'
        }
      }
    }
  }()
  
  //#endregion


  //#region - Search Results Model

  var searchResults = function () {
    var changeDelegate = new Eventify()
      , _searchResultsData = {}

    return {
      get: function () {
        serviceState.check()
        return _searchResultsData
      },

      set: function (obj) {
        serviceState.check()

        if (isPlainObject(obj)) {
          _searchResultsData = obj
          changeDelegate.trigger(_searchResultsData)
        }
      },

      on: function (event, callback) {
        if (event === change_event)
          changeDelegate.register(callback)
      }
    }
  }()

  //#endregion



  return {

    isReady: function () {
      var deferred = $.Deferred()

      $.when(cacheModule.init(), personaService.isReady())
        .then(function () {
          resCache = cacheModule.storage(suggC4s_prefix + 'res')
          reqCache = cacheModule.storage(suggC4s_prefix + 'req')
        })

        .then(function () {
          serviceState.set(true_bool)

          // initialize criteria model with the latest cached version, or with the default
          var latestCachedVersion = reqCache.get(criteria.cacheKey)
          if (criteria.isValidStruct(latestCachedVersion))
            criteria.set(latestCachedVersion)
          else
            criteria.set(criteria.default)

          deferred.resolve(undefined)
        })

      return deferred.promise()
    },

    criteria: criteria,

    serviceState: {
      on: serviceState.on
    },

    inventory: {
      on: searchResults.on,
      get: searchResults.get
    },

    query: function () {
      serviceState.check()

      var url = basePathForService + '/suggested/'
        , ttl = 3e5 // 5 minutes (in ms)
        , deferred = $.Deferred()
        , reqObj = criteria.get()
        , reqStr = jsonStringify(reqObj)
        , cacheKey = 'POST:' + url + reqStr
        , cacheVal = resCache.get(cacheKey)
      
      if (cacheVal) {
        deferred.resolve(cacheVal)
        searchResults.set(cacheVal)
      }
      else {
        $.post(url, { '': reqStr })
          .done(function(resp) {
            resCache.set(cacheKey, resp.data, ttl)
            deferred.resolve(resp.data)
            searchResults.set(resp.data)
          })
          .fail(function() {
            deferred.resolve(undefined)
          })
      }

      return deferred.promise()
    }
  }
}(window, jQuery, ABT, []._)


module.exports = suggestedC4sService
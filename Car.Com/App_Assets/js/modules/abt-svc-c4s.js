/**
 *	Autobytel (c)2015
 *	Module:   Cars-for-Sale Service
 *  Purpose:  Service to provide inventory search capability.
 *
 *	@version: 0.0.1 
 *	@last: 01/01/2015
 *	@author: Greg Lane
 */


var carsforsaleService = function (win, $, abt, undefined) {
  //#region - Module level vars and minification enhancements
  var cacheModule = abt.CACHE
    , personaService = abt.PERSONA
    , utils = abt.UTILS
    , resCache
    , reqCache
    , bkmrksCache
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
    , c4s_prefix = 'c4s_'

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


  //#region - Bookmarker

  var bookmarker = function() {
    var maxBookmarks = 20
      , bookmarksCacheKey = 'bookmarks'
      , _resultsKey = 'results'  // results page
      , _detailsKey = 'details'  // vehicle details page
      , bookmarks = { results: [], details: [] }


    //#region - Private funcs

    var isEquivalent = function(obj1, obj2) {
      return jsonStringify(obj1) === jsonStringify(obj2)
    }

    //#endregion


    return {

      init: function () {
        bookmarks = bkmrksCache.get(bookmarksCacheKey) || bookmarks
      },

      push: function (type, obj) {

        if (type === _resultsKey) {

          // if new criteria obj is the same as the current, do not push on stack.
          if (isEquivalent(obj, bookmarks.results[0]))
            return

          bookmarks.results.unshift(obj)

          if (bookmarks.results.length > maxBookmarks) {
            bookmarks.results.pop()
          }
        } else if (type === _detailsKey) {

          // if new criteria obj is the same as the current, do not push on stack.
          if (isEquivalent(obj, bookmarks.details[0]))
            return

          bookmarks.details.unshift(obj)

          if (bookmarks.details.length > maxBookmarks) {
            bookmarks.details.pop()
          }
        }
        
        bkmrksCache.set(bookmarksCacheKey, bookmarks, bkmrksCache.permanent)
      },

      get: function (type, idx) {
        idx = idx || 0

        if (type === _resultsKey) {
          return bookmarks.results[idx]
        } else if (type === _detailsKey) {
          return bookmarks.details[idx]
        }

        return undefined
      },
      
      resultsKey: _resultsKey,  // results page

      detailsKey: _detailsKey   // vehicle details page
    }
  }()

  //#endregion


  //#region - Criteria Model

  var criteria = function () {
    var changeDelegate = new Eventify()
      , _criteriaObj = {}
      , defaultItemsPerPage = 12
      , ttl = 3e5 // 5 minutes (in ms)


    //#region - Private funcs

    var _cacheCriteria = function () {
        reqCache.set(criteria.cacheKey, _criteriaObj, ttl)
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

      set: function (obj, store) {
        serviceState.check()

        if (isPlainObject(obj)) {
          var newObj = extend(true_bool, {}, _criteriaObj, obj)
            , isValid = this.isValidStruct(newObj)

          if (isValid) {
            _criteriaObj = newObj
            _cacheCriteria()

            if (!!store) {
              bookmarker.push(bookmarker.resultsKey, _criteriaObj)
            }

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

      restore: function (idx) {
        serviceState.check()
        var bookmark = bookmarker.get(bookmarker.resultsKey, idx)

        if (bookmark) {
          _criteriaObj = bookmark
          _cacheCriteria()
          _triggerChangeEvent()
        }

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
        max_mileage: 0,
        max_price: 0,
        radius_miles: 100,
        page: {
          current: 1,
          items_per_page: defaultItemsPerPage
        },
        sort: {
          by: 'best_match',
          dir: 'desc'
        },
        filters: {
          price_range: '0|300',
          mileage_range: '0|300',
          year_range: '',
          makes: '',
          make_models: '',
          //city_mpg: '',
          //highway_mpg: '',
          categories: '',
          category_makes: '',
          //cylinders: '',
          //drive_types: '',
          fuel_types: '',
          //tranny_types: '',
          options: ''
        }
      },

      packTemplate: {
        zipcode: 'z',
        max_mileage: 'maxm',
        max_price: 'maxp',
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
        filters: {
          '@pname': 'f',
          price_range: 'pr',
          mileage_range: 'mr',
          year_range: 'yr',
          makes: 'ma',
          make_models: 'mm',
          //city_mpg: 'cm',
          //highway_mpg': 'hm',
          categories: 'ca',
          category_makes: 'cm',
          //cylinders: 'cy',
          //drive_types: 'dt',
          fuel_types: 'ft',
          //tranny_types: 'tt',
          options: 'o'
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
          resCache = cacheModule.storage(c4s_prefix + 'res')
          reqCache = cacheModule.storage(c4s_prefix + 'req')
          bkmrksCache = cacheModule.storage(c4s_prefix + 'bkmrks')
        })

        .then(function() {
          return _getFilterDomains(true_bool)
        })

        .then(function (filterDomains) {
          mergeDefaultCriteriaWithCurrentFilterDomains(filterDomains)
          bookmarker.init()
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

    bookmarker: {
      get: bookmarker.get
    },

    serviceState: {
      on: serviceState.on
    },

    inventory: {
      on: searchResults.on,
      get: searchResults.get
    },

    query: function () {
      serviceState.check()

      var url = basePathForService + '/search/'
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
    },

    getFilterDomains: function() {
      return _getFilterDomains()
    },

    getMakesWithInventoryCount: function() {
      serviceState.check()

      var url = basePathForService + '/makes/'
        , ttl = 9e5 // 15 minutes (in ms)
        , deferred = $.Deferred()
        , cacheKey = 'GET:' + url
        , cacheVal = resCache.get(cacheKey)

      if (cacheVal) {
        deferred.resolve(cacheVal.data)
      }
      else {
        $.get(url)
          .done(function (resp) {
            resCache.set(cacheKey, resp, ttl)
            deferred.resolve(resp.data)
          })
          .fail(function () {
            deferred.resolve(undefined)
          })
      }

      return deferred.promise()
    },

    getMakeByMakeId: function (id) {
      serviceState.check()

      var url = basePathForService + '/make/' + id
        , ttl = 18e5 // 30 minutes (in ms)
        , deferred = $.Deferred()
        , cacheKey = 'GET:' + url
        , cacheVal = resCache.get(cacheKey)

      if (cacheVal) {
        deferred.resolve(cacheVal.data)
      }
      else {
        $.get(url)
          .done(function (resp) {
            resCache.set(cacheKey, resp, ttl)
            deferred.resolve(resp.data)
          })
          .fail(function () {
            deferred.resolve(undefined)
          })
      }

      return deferred.promise()
    },

    getCategoryByCategoryId: function (id) {
      serviceState.check()

      var url = basePathForService + '/category/' + id
        , ttl = 18e5 // 30 minutes (in ms)
        , deferred = $.Deferred()
        , cacheKey = 'GET:' + url
        , cacheVal = resCache.get(cacheKey)

      if (cacheVal) {
        deferred.resolve(cacheVal.data)
      }
      else {
        $.get(url)
          .done(function (resp) {
            resCache.set(cacheKey, resp, ttl)
            deferred.resolve(resp.data)
          })
          .fail(function () {
            deferred.resolve(undefined)
          })
      }

      return deferred.promise()
    },

    getModelsByMakeId: function (id) {
      serviceState.check()

      var url = basePathForService + '/make/' + id + '/models/'
        , ttl = 18e5 // 30 minutes (in ms)
        , deferred = $.Deferred()
        , cacheKey = 'GET:' + url
        , cacheVal = resCache.get(cacheKey)

      if (cacheVal) {
        deferred.resolve(cacheVal.data)
      }
      else {
        $.get(url)
          .done(function (resp) {
            resCache.set(cacheKey, resp, ttl)
            deferred.resolve(resp.data)
          })
          .fail(function () {
            deferred.resolve(undefined)
          })
      }

      return deferred.promise()
    },

    getMakesByCategoryId: function (id) {
      serviceState.check()

      var url = basePathForService + '/category/' + id + '/makes/'
        , ttl = 18e5 // 30 minutes (in ms)
        , deferred = $.Deferred()
        , cacheKey = 'GET:' + url
        , cacheVal = resCache.get(cacheKey)

      if (cacheVal) {
        deferred.resolve(cacheVal.data)
      }
      else {
        $.get(url)
          .done(function (resp) {
            resCache.set(cacheKey, resp, ttl)
            deferred.resolve(resp.data)
          })
          .fail(function () {
            deferred.resolve(undefined)
          })
      }

      return deferred.promise()
    },

    ping: function (obj) {
      serviceState.check()

      var reqObj = extend(true_bool, { trim_id: undefined, make_id: undefined, make: '', model: '', zipcode: '' }, obj)

      var url = basePathForService + '/availability-ping/zipcode/' + obj.zipcode + '/'
        , ttl = 9e5 // 15 minutes (in ms)
        , deferred = $.Deferred()
        , reqStr = jsonStringify(reqObj)
        , cacheKey = 'POST:' + url + reqStr
        , cacheObj = resCache.get(cacheKey)


      if (!utils.isZip(reqObj.zipcode)) {
        deferred.resolve(undefined)
      }


      if (cacheObj) {
        deferred.resolve(cacheObj.data)
      }
      else {
        $.post(url, { '': reqStr })
          .done(function (resp) {
            resCache.set(cacheKey, resp, ttl)
            deferred.resolve(resp.data)
          })
          .fail(function () {
            deferred.resolve(undefined)
          })
      }

      return deferred.promise()
    }
  }
}(window, jQuery, ABT, []._)


module.exports = carsforsaleService
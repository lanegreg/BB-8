/**
 *	Autobytel (c)2015
 *	Module:   Compress and Cache Service
 *  Purpose:  Service to provide client-side caching with compression
 *
 *	@version: 0.0.1 
 *	@last: 01/01/2015
 *	@author: Greg Lane
 */


var cache = function (win, $, abt, undefined) {
  //#region - Module level vars and minification enhancements
  var defaultTimeToLive = 60000 // 1 minute (in ms)
    , JSON = win.JSON
    , jsonStringify = JSON.stringify
    , jsonParse = JSON.parse
    , isPlainObject = $.isPlainObject
    , libsPath = abt.ENV.libsPath
    , initHasBeenCalled = !1
    , deferred = $.Deferred()
    , crypto_dash = '/crypto-'
    , min_js = '.min.js'
    , permanent
    , sha1
    , lzCompress
    , lzDecompress

  //#endregion



  //#region - Private funcs

  var createHashKey = function (key) {
    return sha1(isPlainObject(key) ? jsonStringify(key) : key).toString()
  }

  //#endregion


  return {
    init: function () {
      if (initHasBeenCalled)
        return deferred.promise()
      
      initHasBeenCalled = !0

      $.when(
          $.getCachedScript(libsPath + crypto_dash + 'lz' + min_js),
          $.getCachedScript(libsPath + crypto_dash + 'sha1' + min_js)
        )
        .then(function () {
          sha1 = win.CryptoJS.SHA1
          var LZString = win.LZString
          lzCompress = LZString.compressToUTF16
          lzDecompress = LZString.decompressFromUTF16
          deferred.resolve(undefined)
        })

      return deferred.promise()
    },

    storage: function (name) {
      var cache = abt.storage(name)

      return {
        set: function (key, val, ttl) {
          if (!key || !val)
            return undefined

          var cacheKey = createHashKey(key)
            , cacheVal = lzCompress(isPlainObject(val) ? jsonStringify(val) : val)

          ttl = (ttl === permanent ? permanent : (ttl || defaultTimeToLive))

          return cache.set(cacheKey, cacheVal, ttl)
        },

        get: function (key) {
          if (!key)
            return undefined

          var cacheKey = createHashKey(key)
            , cacheVal = cache.get(cacheKey)

          if (cacheVal) {
            try {
              var dec = lzDecompress(cacheVal)
              return jsonParse(dec)
            }
            catch (err) {
              return cacheVal
            }
          }

          return cacheVal
        },

        permanent: permanent
      }
    }
  }
}(window, jQuery, ABT, []._)


module.exports = cache
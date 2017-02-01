/**
 *	global utilities module
 *	Autobytel (c)2015
 *
 *	@version: 0.0.1 
 *	@last: 01/01/2015
 *	@author: Greg Lane
 */


var utils = function(win) {
  var math = win.Math
    , Date = win.Date
    , x_char = 'x'
    , dash_char = '-'
    , empty = ''
    , length_str = 'length'


  var _repeat = function (s, n, d) {
    return --n ? s + (d || empty) + _repeat(s, n, d) : empty + s
  }

  var _isInt = function (n) {
    return n === +n && n === (n | 0)
  }

  var _hasSessionStorage = function() {
    return !!win.sessionStorage
  }

  return {

    repeat: _repeat,

    UID: function () {
      return {
        generate: function() {
          return math.floor(math.random() * 10e12)
        }
      }
    }(),

    GUID: function() {
      return {
        generate: function() {
          return (
              _repeat(x_char, 8) + dash_char +
              _repeat(x_char, 4) + '-4' +
              _repeat(x_char, 3) + '-y' +
              _repeat(x_char, 3) + dash_char +
              _repeat(x_char, 12))
            .replace(/[xy]/g, function(c) {
              var r = math.random() * 16 | 0,
                  v = c == x_char ? r : (r & 0x3 | 0x8)

              return v.toString(16)
            })
        }
      }
    }(),

    getTimestamp: function (timeToAdd) {
      if (timeToAdd) {
        var parts = timeToAdd.split(dash_char)
          , secs = 1000
          , mins = secs * 60
          , hrs = mins * 60
          , days = hrs * 24
 

        if (parts[length_str] === 2) {
          var value = parts[0],
              interval = parts[1]

          if (interval === 'secs') {
            timeToAdd = +value * secs
          }
          else if (interval === 'mins') {
            timeToAdd = +value * mins
          }
          else if (interval === 'hrs') {
            timeToAdd = +value * hrs
          }
          else if (interval === 'days') {
            timeToAdd = +value * days
          }
          else {
            timeToAdd = 0
          }
        }
        else {
          timeToAdd = 0
        }
      }

      return (1 * new Date()) + (timeToAdd || 0)
    },

    isFloat: function(n) {
      return n === +n && n !== (n | 0)
    },

    isInt: _isInt,

    isZip: function(z) {
      // should have a length of 5, and be coerceable as an integer
      return (empty + z)[length_str] === 5 && _isInt(+z)
    },

    zipify: function(value) {
      return value.replace(/[^0-9]/g, empty).substr(0, 5)
    },

    hasSessionStorage: _hasSessionStorage()
  }
}(window)

utils.SVC_ERR_MSG = 'Service has NOT been initialized by calling [svc-name].isReady()'
utils.WDG_ERR_MSG = 'Widget has NOT been properly initialized.'

module.exports = utils
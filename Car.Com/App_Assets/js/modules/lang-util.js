/**
 *	language utility module
 *	Autobytel (c)2014
 *
 *	@version: 0.0.1 
 *	@last: 07/08/2014
 *	@author: Greg Lane
 */


var langUtil = (function () {
  var
		TOSTR = Object.prototype.toString,
		STR_PROTO = String.prototype,
		NATIVE_FN_REGEX = /\{\s*\[(?:native code|function)\]\s*\}/i,
		SUBREGEX = /\{\s*([^|}]+?)\s*(?:\|([^}]*))?\s*\}/g,
		WHITESPACE = '\x09\x0A\x0B\x0C\x0D\x20\xA0\u1680\u180E\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200A\u2028\u2029\u202F\u205F\u3000\uFEFF',
		WHITESPACE_CLASS = '[\x09-\x0D\x20\xA0\u1680\u180E\u2000-\u200A\u2028\u2029\u202F\u205F\u3000\uFEFF]+',
		TRIM_LEFT_REGEX = new RegExp('^' + WHITESPACE_CLASS),
		TRIM_RIGHT_REGEX = new RegExp(WHITESPACE_CLASS + '$'),
		TRIMREGEX = new RegExp(TRIM_LEFT_REGEX.source + '|' + TRIM_RIGHT_REGEX.source, 'g'),
 		TYPES = {
 		  'undefined': 'undefined',
 		  'number': 'number',
 		  'boolean': 'boolean',
 		  'string': 'string',
 		  '[object Function]': 'function',
 		  '[object RegExp]': 'regexp',
 		  '[object Array]': 'array',
 		  '[object Date]': 'date',
 		  '[object Error]': 'error'
 		}

  var _type = function (o) {
    return TYPES[typeof o] || TYPES[TOSTR.call(o)] || (o ? 'object' : 'null')
  },

	_isNative = function (fn) {
	  return !!(fn && NATIVE_FN_REGEX.test(fn))
	},

	_isFunction = function (o) {
	  return _type(o) === 'function'
	}

  // polyfill some {String} functions if they don't already exist
  if (!_isFunction(STR_PROTO.startsWith)) {
    STR_PROTO.startsWith = function (s) {
      return this.slice(0, s.length) === s
    }
  }
  if (!_isFunction(STR_PROTO.endsWith)) {
    STR_PROTO.endsWith = function (s) {
      return this.slice(-s.length) === s
    }
  }



  return {
    isFunction: _isFunction,

    now: Date.now || function () {
      return 1 * new Date() // multiplying by 1 returns UTC time
    },

    isObject: function (o, failfn) {
      var t = typeof o

      return (o && (t === 'object' || (!failfn && (t === 'function' || isFunction(o))))) || false
    },

    isString: function (o) {
      return typeof o === 'string'
    },

    isBoolean: function (o) {
      return typeof o === 'boolean'
    },

    isNumber: function (o) {
      return typeof o === 'number' && isFinite(o)
    },

    isArray: _isNative(Array.isArray) ? Array.isArray : function (o) {
      return _type(o) === 'array'
    },

    isDate: function (o) {
      return _type(o) === 'date' && o.toString() !== 'Invalid Date' && !isNaN(o)
    },

    isUndefined: function (o) {
      return typeof o === 'undefined'
    },

    isNull: function (o) {
      return o === null
    },

    // isRegExp: function(value) {
    // 	return _type(value) === 'regexp';
    // },

    //  isValue: function(o) {
    // 	var t = _type(o)

    // 	switch (t) {
    // 		case 'number':
    // 			return isFinite(o)

    // 		case 'null': // fallthru
    // 		case 'undefined':
    // 			return false

    // 		default:
    // 			return !!t
    // 	}
    // },

    // sub: function(s, o) {
    // 	return s.replace ? s.replace(SUBREGEX, function (match, key) {
    // 		return isUndefined(o[key]) ? match : o[key]
    // 	}) : s
    // },

    // trim: _isNative(STR_PROTO.trim) && !WHITESPACE.trim() ? function(s) {
    // 	return s && s.trim ? s.trim() : s
    // } : function (s) {
    // 	try {
    // 		return s.replace(TRIMREGEX, '')
    // 	} catch (e) {
    // 		return s
    // 	}
    // },

    // trimLeft: _isNative(STR_PROTO.trimLeft) && !WHITESPACE.trimLeft() ? function (s) {
    // 	return s.trimLeft()
    // } : function (s) {
    // 	return s.replace(TRIM_LEFT_REGEX, '')
    // },

    // trimRight: _isNative(STR_PROTO.trimRight) && !WHITESPACE.trimRight() ? function (s) {
    // 	return s.trimRight()
    // } : function (s) {
    // 	return s.replace(TRIM_RIGHT_REGEX, '')
    // }
  }
})()

module.exports = langUtil
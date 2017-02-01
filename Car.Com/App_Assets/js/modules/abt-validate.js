/**
 *	global validation module
 *	Autobytel (c)2015
 *
 *	@version: 0.0.1 
 *	@last: 02/10/2015
 *	@author: Greg Lane
 */


var validate = function () {
  var numeric_only_regex = /[^0-9]/g


  return {
    zip: function (val) {
      return val.replace(numeric_only_regex, '').substr(0, 5)
    }
  }
}()


module.exports = validate
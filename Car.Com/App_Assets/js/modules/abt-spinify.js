/**
 *	wait spinner module
 *	Autobytel (c)2015
 *
 *	@version: 0.0.1 
 *	@last: 08/18/2015
 *	@author: Greg Lane
 */


function Spinify() { }


Spinify.createSpinner = function (config) {
  return new Spinify.Spinner(window, jQuery, ABT, config).api()
}

Spinify.Spinner = function (win, $, abt, config) {
  var _ = this
  _.win = win
  _.$ = $
  _.abt = abt
  _.config = $.extend(true, {}, config, Spinify.Spinner.default)
  _.id = ''
  _.$wrapper

  var Eventify = _.abt.Eventify
  _.startDel = new Eventify() // startDelegate
  _.stopDel = new Eventify()  // stopDelegate
  _.toutDel = new Eventify()  // timeoutDelegate
}

Spinify.Spinner.default = {
  tout: 4000 // default timeout is 4 secs
}


Spinify.Spinner.prototype.api = function () {
  var _ = this

  // #region - Prototype level variable declarations
  var start_event = 'start'
    , stop_event = 'stop'
    , timeout_event = 'timeout'
    , leftAngleBracket = '<'
    , rightAngleBracket = '>'
    , forwardSlash = '/'
    , style = 'style'

  //#endregion


  // Pure function which has NO idea about instance state
  var _initInstance = function (abt, $) {
    var instanceId = 'spin_' + (abt.UTILS.UID.generate() + '').slice(0, 4) + '_js'

    //#region - HTML frag

    var htmlFrag = '<div id="" class="loading" style="display:none">Loading&#8230;</div>'

    //#endregion

    //#region - Styles

    var styles = 
      '.loading{position:fixed;z-index:999;height:70%;width:1em;overflow:show;margin:25% 50%;top:0;left:0}' +
      '.loading:before{content:\'\';display:block;position:fixed;top:0;left:0;width:100%;height:100%;background-color:rgba(0,0,0,0.3)}' +
      '.loading:not(:required){font:0/0 a;color:transparent;text-shadow:none;background-color:transparent;border:0}' +
      '.loading:not(:required):after{content:\'\';display:block;height:9em;width:9em;font-size:8px;border-top:1.1em solid rgba(255,255,255,0.2);border-right:1.1em solid rgba(255,255,255,0.2);border-bottom:1.1em solid rgba(255,255,255,0.2);border-left:1.1em solid #fff;-webkit-transform:translateZ(0);-ms-transform:translateZ(0);transform:translateZ(0);-webkit-animation:spinner 1.1s infinite linear;animation:spinner 1.1s infinite linear;-moz-border-radius:50%;-webkit-border-radius:50%;border-radius:50%}' +
      '@-webkit-keyframes spinner{0%{-webkit-transform:rotate(0deg);transform:rotate(0deg)}100%{-webkit-transform:rotate(360deg);transform:rotate(360deg)}}' +
      '@keyframes spinner{0%{-webkit-transform:rotate(0deg);transform:rotate(0deg)}100%{-webkit-transform:rotate(360deg);transform:rotate(360deg)}}'

    //#endregion

    var stylesFrag = leftAngleBracket + style + rightAngleBracket + styles + leftAngleBracket + forwardSlash + style + rightAngleBracket

    $('body')
      .append(stylesFrag)
      .append(htmlFrag.replace('id=""', 'id="' + instanceId + '"'))

    return instanceId
  }


  return {
    start: function () {
      if (!_.id.length) {
        _.id = _initInstance(_.abt, _.$)
        _.$wrapper = $('#' + _.id)
      }
      
      _.$wrapper.show()
    },

    stop: function () {
      _.$wrapper.hide()
    },

    on: function (event, callback) {
      if (event === start_event) {
        _.startDel.register(callback)
      } else if (event === stop_event) {
        _.stopDel.register(callback)
      } else if (event === timeout_event) {
        _.toutDel.register(callback)
      }
    }
  }
}


module.exports = Spinify


//#region - a little bit of testeroo 
/**

  var spinner = Spinify.createSpinner({ size: 'medium', wrapper: 'overlay_js' })
  spinner.start()
  spinner.on('timeout', function() { console.log('timeout rang!') })
  spinner.stop()



  var spinify = new Spinify({})
  spinify.on('timeout', function() { console.log('timeout rang!') })
  spinify.on('start', function() { console.log('start rang!') })
  spinify.on('stop', function() { console.log('stop rang!') })
  spinify.start()

*/
//#endregion
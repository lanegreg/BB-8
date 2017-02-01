/**
 *	Autobytel (c)2015
 *	Module:   Micro Event Triggering and Notification
 *  Purpose:  To provide simple event triggering and notification functionality
 *
 *	@version: 0.0.1 
 *	@last: 01/01/2015
 *	@author: Greg Lane
 */


function Delegate() {
  this.delegate = []
}


Delegate.prototype = {
  register: function (callback, once) {
    if (typeof callback === 'function') {
      once = typeof once === 'boolean' ? once : false
      this.delegate.push({ cb: callback, once: once })
    }
  },

  trigger: function(args) {
    for (var i = 0; i < this.delegate.length; i++) {
      var obj = this.delegate[i]

      obj.cb({ timestamp: Date.now(), args: args })

      if (obj.once) {
        this.delegate.splice(i, 1)
      }
    }
  }
}

function Eventify() {
  Delegate.call(this)
}

Eventify.prototype = Object.create(Delegate.prototype)


module.exports = Eventify



//#region - a little bit of testeroo 
/* 

clear()
var go = function () {
  var delegate = new Eventify()

  return {
    init: function () {
      delegate.register(function (e){
        console.log('1st registered func called ' + e.args.name)
        console.log(e.source)
      })
      delegate.register(function (e) {
        console.log('2nd registered func called ' + e.args.name)
        console.log(e.source)
      })
    },
    fire: function () {
      delegate.trigger({name: 'Gergism'})  
    }
  }
}()

go.init()
go.fire()

*/
//#endregion
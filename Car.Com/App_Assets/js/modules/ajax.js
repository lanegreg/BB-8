/**
 * A micro library for ajax.
 */

 //// TODO: Need to fix this. Variables are leaking into the global!
function ajax(url, callback) {
  var self = this
  self.bindFunction = function(caller, object) {
    return function() {
      return caller.apply(object, [object])
    }
  }

  self.stateChange = function() {
    if (self.request.readyState == 4) {
      self.callback(self.request.responseText)
    }
  }

  self.getRequest = function() {
    if (window.ActiveXObject) {
      return new ActiveXObject('Microsoft.XMLHTTP')
    }
    else if (window.XMLHttpRequest) {
      return new XMLHttpRequest()
    }

    return false
  }

  self.postBody = (arguments[2] || '')

  self.callback = callback
  self.url = url
  self.request = self.getRequest()

  if (self.request) {
    var req = self.request,
        setRequestHeader = req.setRequestHeader.bind(req)
    req.onreadystatechange = self.bindFunction(self.stateChange, self)

    if (self.postBody !== '') {
      req.open('POST', url, true)
      setRequestHeader('X-Requested-With', 'XMLHttpRequest')
      setRequestHeader('Content-type', 'application/x-www-form-urlencoded')
      setRequestHeader('Connection', 'close')
    }
    else {
      req.open('GET', url, true)
    }

    req.send(self.postBody)
  }
}

module.exports = ajax
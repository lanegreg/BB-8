/**
 *	page session state and tracking module
 *	Autobytel (c)2015
 *
 *	@version: 0.0.1 
 *	@last: 02/20/2015
 *	@author: Greg Lane
 */


'use strict';

var trk = function (win, $, abt, undefined, s) {
  //#region - Module level vars and minification enhancements
  var utils = abt.UTILS
    , trk = abt.TRK
    , device = abt.DEVICE
    , pageCtx = trk.pageCtx
    , carDotComId = pageCtx.defaultId
    , cache = abt.storage('session_ctx')
    , getQuerystringValueByKey = $.getQuerystringValueByKey
    , true_bool = !!0
    , empty_str = ''
    , colon_str = ':'
    , undefined_str = 'undefined'
    , pageview_str = 'pageview'
    , event_str = 'event'
    , action_str = 'action'
    , body_str = 'body'
    , click_event = 'click'
    , cacheKey = 'session'

    //#region - Session_TTL (BR per Daly)
    //  RULE: A session is considered active unless the user is inactive for more than the Session_TTL (we are using a sliding ttl window approach)
    //#endregion
    , sessionTtl = 18e5 // 30 mins (in ms)
    //, sessionTtl = 3e2 // 3e4 = 30 secs (in ms) (** FOR TESTING ONLY **)

    , advanceTimeBy = (sessionTtl / 60000) + '-mins'
    , sessionCtx = {
        session: {
          id: 0,
          ts: 0
        },
        trackMeta: {
          affiliate: {
            id: 0,
            name: '',
            groupName: ''
          },
          channel: 'UNK'
        }
      }

  //#endregion


  return {
    comscore: function() {
      var trackPageview = function() {
        if (pageCtx.delayPageviewTracking) {
          return
        }

        var track = function () {
          if (typeof COMSCORE !== undefined_str)
            COMSCORE.beacon({ c1: '2', c2: '11112730', c3: '', c4: pageCtx.pageName || '' })
          else {
            setTimeout(function () { track() }.bind(this), 50)
          }
        }

        track()
      }


      return {
        track: function(type) {
          switch (type) {
            case pageview_str:
            {
              trackPageview()
              break
            }
          }
        }
      }
    }(),

    ga: function() {

      return {
        track: function(type) {
          switch (type) {
            case pageview_str:
            {
              ga('send', pageview_str)
              break
            }
          }
        }
      }
    }(),

    omni: function() {
      //#region - vars and minification enhancements
      var omni_str = 'omni'
        , omnipage_str = omni_str + 'page'
        , omnicallback_str = omni_str + 'callback'
        , omnitarget_str = omni_str + 'target'
        , $body = $(body_str)
        , page = ($body.data(omnipage_str) || empty_str).replace('.', colon_str)
        , callbacks

      //#endregion


      //#region - Private funcs

      var trackPageview = function () {
        if (pageCtx.delayPageviewTracking) {
          return
        }
        
        var space_str = ' '
          , adSelector = '[id^=js_tile]'
          , trackMeta = sessionCtx.trackMeta
          , affiliate = trackMeta.affiliate
          , channel = trackMeta.channel
          , silverPoopCampaignMsgId = 'sp_mid'
          , silverPoopReportId = 'sp_rid'


        s.pageName = pageCtx.pageName || empty_str
        s.channel = pageCtx.siteSection || empty_str
        s.prop1 = s.eVar1 = pageCtx.contentSection || empty_str
        s.prop2 = s.eVar2 = pageCtx.subSection || empty_str
        s.prop3 = s.eVar3 = $(adSelector).length
        s.prop4 = s.eVar4 = channel
        s.prop5 = affiliate.groupName
        s.prop6 = affiliate.name
        s.prop21 = s.eVar21 = device.getType()


        var make = pageCtx.make || empty_str
          , supermodel = pageCtx.superModel || empty_str
          , year = pageCtx.year || empty_str
          , trim = pageCtx.trim || empty_str
          , category = pageCtx.category || empty_str
          , articleId = pageCtx.articleId || empty_str

        s.prop7 = s.eVar7 = make
        s.prop8 = s.eVar8 = make !== empty_str && supermodel !== empty_str ? make + space_str + supermodel : empty_str
        s.prop9 = s.eVar9 = year !== empty_str && make !== empty_str && supermodel !== empty_str && trim !== empty_str ? year + space_str + make + space_str + supermodel + space_str + trim : empty_str
        s.prop10 = s.eVar10 = category
        s.prop11 = s.eVar11 = articleId
        s.prop14 = window.location.href.toLowerCase()
        s.eVar17 = getQuerystringValueByKey(silverPoopCampaignMsgId)
        s.eVar18 = getQuerystringValueByKey(silverPoopReportId)

        // send the track data
        s.t()
      }

      var trackEvent = function (text, event) {
        s.linkTrackVars = event_str + 's'
        s.linkTrackEvent = event
        s.events = event
        s.tl(true_bool, 'o', text)
      }

      var trackElement = function ($this) {
        var target = $this.data(omnitarget_str) || empty_str
          , track = $this.data(omnicallback_str) || empty_str
          , callback = callbacks[track]
          , clicked_str = '_clicked()'

        if (callback) {
          callback({ page: page, target: target, $this: $this })
        }
        else {
          trk.omni.track(action_str, page + colon_str + target + clicked_str)
        }
      }

      var trackAction = function (text) {
        if (typeof text === 'object') {
          trackElement(text)
          return
        }

        s.tl(true_bool, 'o', text)
      }

      //#endregion


      return {
        init: function () {
          s = win.s
          callbacks = trk.omni.callback

          // wireup event|action tracking
          $body.on(click_event, '[data-' + omnitarget_str + ']', function () {
            trackElement($(this))
          })

          return this
        },

        track: function(type, text, event) {
          switch (type) {
            case pageview_str:
            {
              trackPageview()
              break
            }

            case action_str:
            {
              trackAction(text)
              break
            }

            case event_str:
            {
              trackEvent(text, event)
              break
            }
          }
        },

        callback: {
          noop: function() {}
        }
      }
    }(),

    session: function() {

      return {
        init: function() {
          var getTimestamp = utils.getTimestamp
            , deferred = $.Deferred()


          //#region - ** SESSION **

          // First, check for existing session data in loc_stor cache.
          var storedSessionCtx = cache.get(cacheKey)

          // Then, check that the session is still valid and has not yet expired.
          if (storedSessionCtx && storedSessionCtx.session.ts >= getTimestamp()) {
            sessionCtx = storedSessionCtx
          }
          else {
            // Here, we've either we found no session_id in loc_stor, or it has expired past the ttl, 
            // so we'll establish some new session values.
            sessionCtx.session = {
              id: utils.UID.generate(),
              ts: getTimestamp(advanceTimeBy)
            }
          }

          //#endregion


          //#region - ** Track Meta **

          //#region - Some private funcs

          var reconcileTheTrackMeta = function(affId) {
            var trackMetaDeferred = $.Deferred()

            var fetchTrackMeta = function (id) {

              //if win.document.referrer is an empty space, this sets it to the baseURI value
              var tempReferrer = win.document.referrer;
              if (tempReferrer === "") {
                tempReferrer = win.document.baseURI;
              }

              var data = JSON.stringify({ affiliate_id: id, referrer: tempReferrer })

              $.post('/api/track/meta', {'': data}, function(resp) {
                var data = resp.data
                  , affiliate = data.affiliate

                sessionCtx.trackMeta = {
                  affiliate: {
                    id: affiliate.id,
                    name: affiliate.name,
                    groupName: affiliate.group_name
                  },

                  channel: data.traffic_channel
                }

                trackMetaDeferred.resolve()
              })
            }

            if (affId) {
              fetchTrackMeta(affId)
            }
            else if (sessionCtx.trackMeta.affiliate.id > 0) {
              trackMetaDeferred.resolve()
            }
            else {
              fetchTrackMeta(carDotComId)
            }

            return trackMetaDeferred.promise()
          }

          var cacheTheSessionCtx = function() {
            // reset ttl on the local-storage cache to maintain a sliding-window affect
            sessionCtx.session.ts = getTimestamp(advanceTimeBy)
            cache.set(cacheKey, sessionCtx, undefined) // undefined => store permanently
            trk.sessionCtx = sessionCtx // apply to our namespace for easy access by other use-cases
          }

          //#endregion

          // first, check for an existing affiliate_id on q_str, if none, then check loc_stor cache for any existing track-meta
          var affiliateId = getQuerystringValueByKey('id')

          $.when(reconcileTheTrackMeta(affiliateId))
            .then(function() {
              cacheTheSessionCtx()
              deferred.resolve()
            })

          //#endregion

          return deferred.promise()
        }
      }
    }()
  }
}(window, jQuery, ABT, []._)

module.exports = trk
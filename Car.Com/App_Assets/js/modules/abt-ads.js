
var ads = function (win, $, abt) {
  var utils = abt.UTILS
    , pageCtx = abt.ADS.pageCtx
    , env = abt.ENV
    , url = '/api/ad-route/' + env.routeName + '/tags'
    , js_tile = 'js_tile_'
    , tilesSelector = '[id^=' + js_tile + ']'
    , zero_str = '0'


  var setAdSpots = function($adTiles, adspots, refresh) {

    $adTiles.each(function () {
      var $this = $(this),
        thisTile = $this.attr('id').split('_')[2],
        adspot = adspots.filter(function (spot) { return spot.tile == thisTile })[0]

      if (adspot) {

        var adSpotDivId = 'abtl-gpt-ad-spot-' + thisTile;
        var paramProd = (adspot.section == null) ? '' : adspot.section;
        var paramSubProd = (adspot.subsection == null) ? '' : adspot.subsection;
        var paramType = (adspot.criteria.Category == null) ? '' : adspot.criteria.Category;
        var paramStyle = (adspot.criteria.Style == null) ? '' : adspot.criteria.Style;
        var paramTest = env.isProd ? '' : 'jumpstart';
        var paramTile = (adspot.tile == null) ? '' : adspot.tile;
        var paramYear = (adspot.criteria.Year == null) ? '' : adspot.criteria.Year;
        var paramMake = (adspot.criteria.Make == null) ? '' : adspot.criteria.Make;
        var paramModel = (adspot.criteria.Model == null) ? '' : adspot.criteria.Model;
        var paramFuel = (adspot.criteria.Fuel == null) ? '' : adspot.criteria.Fuel;
        var slotSite = "/2909/car." + adspot.firstlevel + "." + adspot.device;
        var slotSize = adspot.size;

        var paramJagAd = '';
        if (paramTile === 11) { paramJagAd = 'ad1'; }
        else if (paramTile === 12) { paramJagAd = 'ad2'; }

        var gptJsCode = "googletag.cmd.push(function () {\n";
        if (!refresh) {
          gptJsCode = gptJsCode + "  slot" + thisTile + " = googletag.defineSlot('" + slotSite + "'," + slotSize + ",'" + adSpotDivId + "').addService(googletag.pubads());\n";
        }
        if (refresh) {
          gptJsCode = gptJsCode + "  slot" + thisTile + ".clearTargeting();\n";
        }
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('prod', '" + paramProd + "');\n";
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('subprod', '" + paramSubProd + "');\n";
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('type', '" + paramType + "');\n";
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('style', '" + paramStyle + "');\n";
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('yr', '" + paramYear + "');\n";
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('mak', '" + paramMake + "');\n";
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('mod', '" + paramModel + "');\n";
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('fuel', '" + paramFuel + "');\n";
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('jag', '" + paramJagAd + "');\n";
        gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('tile', '" + paramTile + "');\n";
        if (paramTest.toLowerCase() == 'jumpstart') {
          gptJsCode = gptJsCode + "  slot" + thisTile + ".setTargeting('test', 'jumpstart');\n";
        }
        if (!refresh) {
          if (slotSize == "[400,40]") {
            gptJsCode = gptJsCode + "googletag.pubads().addEventListener('slotRenderEnded', function(event) {\n";
            gptJsCode = gptJsCode + "if (event.size[0] == '400') {\n";
            gptJsCode = gptJsCode + "$('#" + adSpotDivId + " iframe').attr('width','100%').attr('height','100');}})\n";
          }
          gptJsCode = gptJsCode + "googletag.enableServices();\n";
          gptJsCode = gptJsCode + "googletag.display('" + adSpotDivId + "');\n});\n";
        }
        if (refresh) {
          var slotName = "[slot" + thisTile + "]";
          gptJsCode = gptJsCode + "googletag.pubads().refresh(" + slotName + ");\n});\n";
        }

        var script = document.createElement("script");
        script.type = "text/javascript";
        script.text = gptJsCode;

        $('<div />', {
          id: adSpotDivId
        }).appendTo($this);

        $("#" + adSpotDivId).html(script);
        
      }
    });
  }

  var loadAdTiles = function($adTiles, refresh) {
    var jsonString = JSON.stringify(pageCtx)

    $.post(url, { '': jsonString }).done(function (resp) {
      var data = resp.data
        , ctx = data.ctx
        , cc5950 = _cc5950
        , empty_str = ''
        , colon_str = ':'
        , int_str = 'int'
        , fuel_str = 'fuel'
        , make_str = 'make'
        , model_str = 'model'
        , year_str = 'year'
        , type_str = 'type'

      $adTiles.empty()
      setAdSpots($adTiles, data.adSpots, refresh)

      cc5950.add(int_str, fuel_str + colon_str + (ctx[fuel_str] || empty_str))
      cc5950.add(int_str, make_str + colon_str + (ctx[make_str] || empty_str))
      cc5950.add(int_str, model_str + colon_str + (ctx[model_str] || empty_str))
      cc5950.add(int_str, year_str + colon_str + (ctx[year_str] || empty_str))
      cc5950.add(int_str, type_str + colon_str + (ctx[type_str] || empty_str))
      cc5950.bcp()
    })
  }

  return {
    init: function () {
      var device = abt.DEVICE

      pageCtx.set({
        isMobi: device.isMobi,
        isTabl: device.isTabl,
        isDesk: device.isDesk
      })

      if (pageCtx.delayAdsLoading)
        return

      loadAdTiles($(tilesSelector), false)
    },

    reload: function (tilenum) {
      pageCtx.set({
        delayAdsLoading: false
      })

      loadAdTiles($(tilenum ? '[id=' + js_tile + tilenum + ']' : tilesSelector), true)
    },

    loadC4SResults: function () {
      pageCtx.set({
        delayAdsLoading: false
      })

      loadAdTiles($(tilesSelector), false)
    },

    reloadC4SResults: function () {
      pageCtx.set({
        delayAdsLoading: false
      })

      loadAdTiles($(tilesSelector), true)
    }


  }
}(window, jQuery, ABT)


module.exports = ads
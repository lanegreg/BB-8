/**
 *  This js module represents the desktop version of comparecars.index http://www.autobytel.com/{path}
 */

//var tmpls = require('./{templates_path}/{device_group}/tmpls.js')

(function (win, $, abt) {
  $(function () {
    var env = abt.ENV
      , getJsFileNameByAlias = env.getJsFileNameByAlias
      , getScriptUriByName = env.getScriptUriByName
      , libsPath = env.libsPath
      , $body = $('body')
      , tmpls = require('./results_tmpls/desk/tmpls.js')
      , personaService = abt.PERSONA
      , currentCompareList = ''
      , $addChangeCarOverlay = $('#add_change_car_overlay_js')
      , $customizecompareOverlay = $('#customize_comparison_overlay_js')
      , $compareCustomizeMain = $('#compareCustomizeMain')
      , $compareCarModelSelect = ''
      , $compareCarTrimSelect = ''
      , $compareCarAddSelectedMake = ''
      , $compareCarAddSelectedModel = ''
      , $compareCarAddSelectedTrim = ''
      , $overlayCloseBtn = ''
      , $overlayCustomizeCompareCloseBtn = ''
      , selectedCategory = 0,
        categoryOverview = [],
        compareCarInfo = [],
        replaceMultSpace = new RegExp(' ', 'g'),
        staticJs = env.staticEndpoint,
        $leadformOverlay = '',
        sidePanelLeadFormInit = false,
        initPageLoad = false;

    categoryOverview[0] = ["MSRP", "MPG", "Horsepower", "Seating Capacity", "Doors"];
    categoryOverview[2] = ["MSRP", "MPG", "Horsepower", "Seating Capacity", "Doors"];
    categoryOverview[3] = ["MSRP", "MPG", "Horsepower", "Seating Capacity", "Roof Type", "Engine"];
    categoryOverview[4] = ["MSRP", "MPG", "Horsepower", "Seating Capacity", "Fuel Type", "Cargo Capacity"];
    categoryOverview[6] = ["MSRP", "MPG", "Seating Capacity", "Cargo Capacity", "Towing Capacity"];
    categoryOverview[7]=["MSRP", "MPG", "Horsepower", "Seating Capacity", "Fuel Type", "Cargo Capacity", "Towing Capacity", "Bed Length"];
    categoryOverview[8]=["MSRP", "MPG", "Horsepower", "Seating Capacity", "Doors", "Cargo Capacity"];
    categoryOverview[9]=["MSRP", "MPG", "Horsepower", "Seating Capacity", "Doors", "Cargo Capacity"];

    var jsCompareCarResults = {

      showComparison: function () {
        var url = '/api/compare/results/trimidlist/' +currentCompareList,
            $compareTableMain = $('#compareTableMain'),
            pData = personaService.get(),
            currentCustomOverview = pData.compareCustomOverview,
            compareSortbyItem = pData.compareSortbyItem;
        
        if (compareSortbyItem && compareSortbyItem.length > 0) 
          compareCarInfo[2] = compareSortbyItem;
        else
          compareCarInfo[2] = "";

        if (currentCompareList && currentCompareList.length > 0) {
          $.get(url).done(function (resp) {
            if (resp.data) {

              if (currentCustomOverview && currentCustomOverview.length > 0) {
                compareCarInfo[1] = currentCustomOverview.split(',');
              } else {
                for (var checkCat = 0; checkCat < resp.data.length; checkCat++) {
                  if (selectedCategory == 0 || selectedCategory == resp.data[checkCat].category_id) {
                    selectedCategory = resp.data[checkCat].category_id;
                  } else {
                    selectedCategory = 0;
                    break;
                  }
                }
                compareCarInfo[1] = categoryOverview[selectedCategory].slice();
              }

              compareCarInfo[0] = resp.data;
              $compareTableMain.empty().prepend(tmpls.compare_data({ carArr: compareCarInfo }));
            } else {
              $compareTableMain.empty().prepend(tmpls.compare_no_data({}));
            }
            jsCompareCarResults.wireupCompareButtons();

            var curTrHeight = 0;
            $(".js_row_filler").each(function (index) {
              curTrHeight = $(this).height();
              $(this).find(".rowhead").each(function () {
                $(this).find(".th-filler").each(function () {
                  $(this).css("height", curTrHeight);
                });
              });
            });

            var adMake = resp.data[0].make;
            var adModel = resp.data[0].model;
            var adYear = resp.data[0].year;
            var adCategoryId = resp.data[0].category_id;
            var adCategory = "sedan";

            if (adCategoryId === 2) {
              adCategory = "coupe";
            }
            else if (adCategoryId === 3) {
              //convertible is a sedan when it comes to type with jumpstart
              adCategory = "sedan";
            }
            else if (adCategoryId === 4) {
              adCategory = "sedan";
            }
            else if (adCategoryId === 6) {
              adCategory = "suv";
            }
            else if (adCategoryId === 7) {
              adCategory = "truck";
            }
            else if (adCategoryId === 8) {
              adCategory = "van";
            }
            else if (adCategoryId === 9) {
              adCategory = "wagon";
            }

            var cleanedMake = "";
            var cleanedModel = "";

            //replaces everything that's not letter or number globally, case insensitive
            if (adMake != null) {
              cleanedMake = adMake.replace(/[^a-z\d]+/gi, "").toLowerCase();
            }
            if (adModel != null) {
              cleanedModel = adModel.replace(/[^a-z\d]+/gi, "").toLowerCase();
            }

            abt.ADS.pageCtx.make = cleanedMake;
            abt.ADS.pageCtx.model = cleanedModel;
            abt.ADS.pageCtx.year = adYear;
            abt.ADS.pageCtx.category = adCategory;

            if (!initPageLoad) {
              initPageLoad = true;
              abt.ADS.reload(1);
            } else {
              abt.ADS.reload();
            }
            return true;
          }); //end done
        } else {
          $compareTableMain.empty().prepend(tmpls.compare_no_data({}));
          jsCompareCarResults.wireupCompareButtons();
        }
      },
      
      showCustomizeOverlay: function () {
        $compareCustomizeMain.empty().prepend(tmpls.compare_customize({ selItemArr: compareCarInfo[1] }));
        jsCompareCarResults.wireupCustomOverviewButtons();
        $customizecompareOverlay.popup('show');
      },

      wireupCustomOverviewButtons: function() {

        $('.js_delete_custom_overview').on('click', function (e) {
          e.preventDefault();
          var deleteItem = $(this).data('custitem').toString(),
              deletedItemId = $(this).data('custitem').toString().replace(replaceMultSpace, '-').replace('/', '-');
          $('#selectedOverview_' + deletedItemId).hide();
          jsCompareCarResults.deleteSelectedCustomOverview(deleteItem);
        });

        $('.js_toggle_custom_overview').on('click', function (e) {
          e.preventDefault();
          var addItem = $(this).data('custitem').toString();
          jsCompareCarResults.toggleSelectedCustomOverview(addItem);
        });

        $('.js_update_custom_overview').on('click', function (e) {
          e.preventDefault();
          jsCompareCarResults.updateSelectedCustomOverview();
        });

      },

      wireupCompareButtons: function () {
        $('.js_add_compare_car').on('click', function (e) {
          e.preventDefault();
          $('#changeCompId').val('');
          $addChangeCarOverlay.popup('show');
        });

        $('.js_delete_compare_car').on('click', function (e) {
          e.preventDefault();
          var deleteId = $(this).data('trimid').toString();
          jsCompareCarResults.deleteSelectedCompareCar(deleteId);
        });
        
        $('.js_change_compare_car').on('click', function (e) {
          e.preventDefault();
          var changeId = $(this).data('trimid').toString();
          $('#changeCompId').val(changeId);
          $addChangeCarOverlay.popup('show');
        });

        $('.js_change_delete_toggle').on('click', function (e) {
          e.preventDefault();
          var curTrimId = $(this).data('trimid').toString();
          $('#js_change_delete_section_' + curTrimId).toggle();
        });

        $('.js_toggle_comp_section').on('click', function (e) {
          e.preventDefault();
          var curSection = $(this).data('section').toString();
          $('#compareSection_' + curSection).toggle();
          var curImage = $('#compareSectionSvg_' + curSection).attr("xlink:href");
          if (curImage.indexOf('i_minus') >= 0)
            $('#compareSectionSvg_' + curSection).attr("xlink:href", "/assets/svg/global-sprite.svg#i_plus");
          else 
            $('#compareSectionSvg_' + curSection).attr("xlink:href", "/assets/svg/global-sprite.svg#i_minus");
        });

        if (currentCompareList && currentCompareList.length > 0 && currentCompareList.split(',').length > 9) {
          $('.js_add_compare_car').hide();
        } else {
          $('.js_add_compare_car').show();
        }

        $('#js_customize_compare_btn').on('click', function (e) {
          e.preventDefault();
          jsCompareCarResults.showCustomizeOverlay();
        });

        $('.js_compare_sortby').on('click', function (e) {
          e.preventDefault();
          var sortItem = $(this).data('sortitem').toString();
          personaService.set({ compareSortbyItem: sortItem });
          jsCompareCarResults.showComparison();
        });

        $overlayCustomizeCompareCloseBtn = $('#close_customize_compare_js'),
        $overlayCustomizeCompareCloseBtn.on('click', function (e) {
          e.preventDefault();
          $customizecompareOverlay.popup('hide');
        });

        $(".compare-head-cont").scroll(function () {
          $(".compare-data-cont").scrollLeft($(".compare-head-cont").scrollLeft());
        });

        $(".compare-data-cont").each(function () {
          $(this).scroll(function () {
            $(".compare-head-cont").scrollLeft($(this).scrollLeft());
          });
        });

        $('.js_get_quote_btn').on('click', function (e) {
          e.preventDefault();
          var quoteYear = $(this).data('year').toString(),
              quoteMake = $(this).data('make').toString(),
              quotemodel = $(this).data('supermodel').toString(),
              quoteTrim = $(this).data('trim').toString();

          if (ABT.pageJson && ABT.pageJson.getaquote) {
            ABT.pageJson.getaquote.quoteButtonSelected = true;
            ABT.pageJson.getaquote.year = quoteYear;
            ABT.pageJson.getaquote.make = quoteMake;
            ABT.pageJson.getaquote.supermodel = quotemodel;
            ABT.pageJson.getaquote.trim = quoteTrim;
          }

          if (!sidePanelLeadFormInit) {
            var offerbootstrapFilename = getJsFileNameByAlias('ofrsystem')
            $.getCachedScript(getScriptUriByName(offerbootstrapFilename))

            sidePanelLeadFormInit = true;
          } else {
            if (ABT.pageJson && ABT.pageJson.getaquote) {
              if (ABT.pageJson.getaquote.make && ABT.pageJson.getaquote.make.length > 0) {
                $('#ncLeadMakeSelect').val(ABT.pageJson.getaquote.make).change();
              }
            }
            $('#ol_display_ymm').hide();
            $('#ol_personalinfo_ymm').hide();
            $('#ol_form_ymm').show();

            $('#ol_Leadform').css("display", "block");
            $('#ol_ofr_Thankyou').css("display", "none");
            $leadformOverlay = $('#leadform_overlay_js');
            $leadformOverlay.popup('show');
            //$('#GetDealersButton').click(function () { return true; }).click();
          }

        });

      },

      updateSelectedCustomOverview: function () {
        personaService.set({ compareCustomOverview: compareCarInfo[1].join() });
        $customizecompareOverlay.popup('hide');
        jsCompareCarResults.showComparison();
      },

      deleteSelectedCustomOverview: function (deleteItem) {
        if (deleteItem.length > 0 && compareCarInfo[1].length > 0) {
          if (compareCarInfo[1].indexOf(deleteItem) >= 0) {
            compareCarInfo[1].splice(compareCarInfo[1].indexOf(deleteItem), 1);
          }
        }
      },

      toggleSelectedCustomOverview: function (addItem) {
        var addItemId = addItem.replace(replaceMultSpace, '-').replace('/', '-');

        if (addItem.length > 0) {
          if (compareCarInfo[1].indexOf(addItem) < 0) {
            compareCarInfo[1].push(addItem);
            $('#selectedOverview_' + addItemId).show();
            $('#addOverview_' + addItemId).addClass("selected");
          } else {
            if (addItem.length > 0 && compareCarInfo[1].length > 0) {
              if (compareCarInfo[1].indexOf(addItem) >= 0) {
                compareCarInfo[1].splice(compareCarInfo[1].indexOf(addItem), 1);
                $('#selectedOverview_' + addItemId).hide();
                $('#addOverview_' + addItemId).removeClass("selected");
              }
            }
            
          }
        }
      },

      deleteSelectedCompareCar: function (deleteId) {
        var currentCompareListArr = [];

        if (deleteId > 0) {

          if (currentCompareList && currentCompareList.length > 0) {
            currentCompareListArr = currentCompareList.split(',');
          }

          var delItemIndx = currentCompareListArr.indexOf(deleteId);
          if (delItemIndx >= 0) {
            currentCompareListArr.splice(delItemIndx, 1);
            currentCompareList = currentCompareListArr.join();
            personaService.set({ compareList: currentCompareList });
            if (currentCompareList.length > 0)
              jsCompareCarResults.showComparison();
            else {
              $('#compareTableMain').empty().prepend(tmpls.compare_no_data({}));
              jsCompareCarResults.wireupCompareButtons();
            }
          }
        }

      },
      
      isNumber: function(n) {
        return /^-?[\d.]+(?:e-?\d+)?$/.test(n);
      },

      initAddChangeOverlay: function () {
        $.when(
          $.get('/api/compare/makes/').done(function (resp) {
            if (resp.data) {
              $body.append(tmpls.add_change_car_overlay({ makeArr: resp.data }));
            }
          })
        )
        .then(function () {
          $addChangeCarOverlay = $('#add_change_car_overlay_js'),
          $overlayCloseBtn = $('.overlay-close'),
          $overlayCloseBtn.on('click', function (e) {
            e.preventDefault();
            $addChangeCarOverlay.popup('hide');
          });
          $compareCarModelSelect = $('#compareCarModelSelect');
          $compareCarTrimSelect = $('#compareCarTrimSelect');

          $('#compareCarMakeSelect').on('change', function (e) {
            $compareCarAddSelectedMake = $('#compareCarMakeSelect :selected').val();
            if ($compareCarAddSelectedMake.length > 0) {
              jsCompareCarResults.updateCompareModelDropdown();
            }
            else {
              $compareCarModelSelect.empty().prepend('<option value="">Select a Model</option>').prop('disabled', true);
              $compareCarTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $compareCarModelSelect.on('change', function (e) {
            $compareCarAddSelectedModel = $('#compareCarModelSelect :selected').val();
            if ($compareCarAddSelectedModel.length > 0) {
              jsCompareCarResults.updateCompareTrimDropdown();
            }
            else {
              $compareCarTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $compareCarTrimSelect.on('change', function (e) {
            var currentCompareListArr = [],
                $changeCompareCarId = $('#changeCompId').val();

            $compareCarAddSelectedTrim = $('#compareCarTrimSelect :selected').val();
            if ($compareCarAddSelectedTrim.length > 0) {
              
              if (currentCompareList && currentCompareList.length > 0) {
                currentCompareListArr = currentCompareList.split(',');
              }

              // if we are running the change process
              if (jsCompareCarResults.isNumber($changeCompareCarId)) {
                $('#changeCompId').val('');
                // If the Item beeing added is already in the list, delete current item before we replace
                if (currentCompareListArr.indexOf($compareCarAddSelectedTrim) >= 0) {
                  var delItemIndx = currentCompareListArr.indexOf($compareCarAddSelectedTrim);
                  currentCompareListArr.splice(delItemIndx, 1);
                  currentCompareList = currentCompareListArr.join();
                }
                // Now we need to replace the ChangeCarId with the AddCarId
                currentCompareListArr[currentCompareListArr.indexOf($changeCompareCarId)] = $compareCarAddSelectedTrim;
              }
              else if (currentCompareListArr.indexOf($compareCarAddSelectedTrim) < 0) {
                currentCompareListArr.push($compareCarAddSelectedTrim);
              }

              currentCompareList = currentCompareListArr.join();
              personaService.set({ compareList: currentCompareList });
              $addChangeCarOverlay.popup('hide');
              jsCompareCarResults.updateViewedRecentlyList($compareCarAddSelectedTrim);
              jsCompareCarResults.showComparison();
            }
          });

        }); // End of then function

        return true;

      },

      updateViewedRecentlyList: function (addTrimId) {
        var pData = personaService.get(),
            currentViewedRecentlyList = pData.viewedRecentlyList,
            currentViewedRecentlyArr = [],
            maxVrNum = 10;

        if (currentViewedRecentlyList && currentViewedRecentlyList.length > 0) {
          currentViewedRecentlyArr = currentViewedRecentlyList.split(',');
        }

        if (currentViewedRecentlyArr.indexOf(addTrimId) < 0) {
          currentViewedRecentlyArr.push(addTrimId);
          if (currentViewedRecentlyArr.length > maxVrNum) {
            currentViewedRecentlyArr.shift();
          }
        }

        currentViewedRecentlyList = currentViewedRecentlyArr.join();
        personaService.set({ viewedRecentlyList: currentViewedRecentlyList });
      },

      updateCompareModelDropdown: function () {
        var url = '/api/compare/make/' + $compareCarAddSelectedMake + '/super-models/';
          
        $.when(
          $.get(url).done(function (resp) {
            if (resp.data) {
              $compareCarModelSelect.empty().prepend(tmpls.model_select_options({ modelArr: resp.data })).prop('disabled', false);
              $compareCarTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          })
        )
        .then(function () {

        }); // End of then function

      },

      updateCompareTrimDropdown: function () {
        var url = '/api/compare/make/' + $compareCarAddSelectedMake + '/super-model/' + $compareCarAddSelectedModel + '/trims/';
          
        $.get(url).done(function(resp) {
          if (resp.data) {
            $compareCarTrimSelect.empty().prepend(tmpls.trim_select_options({ trimArr: resp.data })).prop('disabled', false);
          }
        });

      },





      init: function () {
        var pData = personaService.get();

        currentCompareList = pData.compareList;

        jsCompareCarResults.showComparison();
        jsCompareCarResults.initAddChangeOverlay();
      } // End init function
    };

    $(function () {
      $.when(
        $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('popup')),
        personaService.isReady())
      .then(function () {
        jsCompareCarResults.init();
      })
    });

  });
})(window, jQuery, ABT)
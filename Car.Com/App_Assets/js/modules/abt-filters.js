/**
 *	filter module
 *	Autobytel (c)2014-2015
 *	@author: Sam Schulte
 */

var filters = function () {

  var trimFilterAttributes = {};
  var retryCounter = 1;

  return {
    init: function () {
      //console.log('filters.init() was called');
    },

    setSuperModelFilterGroups: function (makename, supermodelname) {

      //console.log('filters.setSuperModelFilterGroups() was called with makename: ' + makename + ', supermodelname: ' + supermodelname);

      var url = '/api/research-filter/supermodelfiltergroups/'
      , jsonString = "{makename: '" + makename + "', supermodelname: '" + supermodelname + "'}";

      $.post(url, { '': jsonString })
        .done(function (resp) {

          var filterGroups = resp.data.supermodelfiltergroups;

          for (var g = 0; g < filterGroups.length; g++) {
            var filterName = filterGroups[g].filtergroupname.toLowerCase();
            var filterCode = filterGroups[g].code.toLowerCase();
            var mappedFilterCode = filters.mapFilter2Icon(filterName, filterCode);

            var filterIconIdToBeSet = "js-filter-icon-" + filterName.toLowerCase() + "-" + mappedFilterCode.toLowerCase();
            $('#' + filterIconIdToBeSet).css('display', 'table-cell');

            var filterOptionsIdToBeSet = "js-filter-options-" + filterName.toLowerCase();
            $('#' + filterOptionsIdToBeSet).css('display', 'inline-block');

          }
        });
    },
    setSuperModelTrimFilterAttributes: function (makename, supermodelname) {

      //console.log('filters.setSuperModelTrimFilterAttributes() was called with makename: ' + makename + ', supermodelname: ' + supermodelname);

      var url = '/api/research-filter/supermodeltrimfilterattributes/'
      , jsonString = "{makename: '" + makename + "', supermodelname: '" + supermodelname + "'}";

      $.post(url, { '': jsonString })
          .done(function (resp) {
            if (resp.data === null) {
              if (retryCounter < 2) {
                retryCounter++;
                filters.setSuperModelTrimFilterAttributes(makename, supermodelname);
              }
            } else {
              trimFilterAttributes = resp.data.supermodeltrimfilterattributes;
            }
          });
    },

    setSuperModelFilterGroupsByYear: function (makename, supermodelname, year) {

      //console.log('filters.setSuperModelFilterGroupsByYear() was called with makename: ' + makename + ', supermodelname: ' + supermodelname + ', year: ' + year);

      var url = '/api/research-filter/supermodelfiltergroupsbyyear/'
      , jsonString = "{makename: '" + makename + "', supermodelname: '" + supermodelname + "', year: '" + year + "'}";

      $.post(url, { '': jsonString })
        .done(function (resp) {

          var filterGroups = resp.data.supermodelfiltergroups;

          for (var g = 0; g < filterGroups.length; g++) {
            var filterName = filterGroups[g].filtergroupname.toLowerCase();
            var filterCode = filterGroups[g].code.toLowerCase();
            var mappedFilterCode = filters.mapFilter2Icon(filterName, filterCode);

            var filterIconIdToBeSet = "js-filter-icon-" + filterName.toLowerCase() + "-" + mappedFilterCode.toLowerCase();
            $('#' + filterIconIdToBeSet).css('display', 'table-cell');

            var filterOptionsIdToBeSet = "js-filter-options-" + filterName.toLowerCase();
            $('#' + filterOptionsIdToBeSet).css('display', 'inline-block');

          }
        });
    },
    setSuperModelTrimFilterAttributesByYear: function (makename, supermodelname, year) {

      //console.log('filters.setSuperModelTrimFilterAttributesByYear() was called with makename: ' + makename + ', supermodelname: ' + supermodelname + ', year: ' + year);

      var url = '/api/research-filter/supermodeltrimfilterattributesbyyear/',
          jsonString = "{makename: '" + makename + "', supermodelname: '" + supermodelname + "', year: '" + year + "'}";

      $.post(url, { '': jsonString })
          .done(function (resp) {
            if (resp.data === null) {
              if (retryCounter < 2) {
                retryCounter++;
                filters.setSuperModelTrimFilterAttributesByYear(makename, supermodelname, year);
              }
            } else {
              trimFilterAttributes = resp.data.supermodeltrimfilterattributes;
            }
          });
    },

    setCategoryFilterGroups: function (categoryname) {

      //console.log('filters.setCategoryFilterGroups() was called with categoryname: ' + categoryname);

      var url = '/api/research-filter/categoryfiltergroups/'
        , jsonString = "{categoryname: '" + categoryname + "'}";

      $.post(url, { '': jsonString })
        .done(function (resp) {

          var filterGroups = resp.data.categoryfiltergroups;

          for (var g = 0; g < filterGroups.length; g++) {
            var filterName = filterGroups[g].filtergroupname.toLowerCase();
            var filterCode = filterGroups[g].code.toLowerCase();
            var mappedFilterCode = filters.mapFilter2Icon(filterName, filterCode);

            var filterIconIdToBeSet = "js-filter-icon-" + filterName.toLowerCase() + "-" + mappedFilterCode.toLowerCase();
            $('#' + filterIconIdToBeSet).css('display', 'table-cell');

            var filterOptionsIdToBeSet = "js-filter-options-" + filterName.toLowerCase();
            $('#' + filterOptionsIdToBeSet).css('display', 'inline-block');

          }
        });
    },
    setCategoryTrimFilterAttributes: function (categoryname) {

      //console.log('filters.setCategoryTrimFilterAttributes() was called with categoryname: ' + categoryname);

      var url = '/api/research-filter/categorytrimfilterattributes/',
          jsonString = "{categoryname: '" + categoryname + "'}";

      $.post(url, { '': jsonString })
          .done(function (resp) {
            if (resp.data === null) {
              if (retryCounter < 2) {
                retryCounter++;
                filters.setCategoryTrimFilterAttributes(categoryname);
              }
            } else {
              trimFilterAttributes = resp.data.categorytrimfilterattributes;
            }
          });
    },

    setVehicleAttributeFilterGroups: function (categoryname, vehicleattributeseoname) {

      //console.log('filters.setVehicleAttributeFilterGroups() was called with categoryname: ' + categoryname + ', vehicleattributeseoname: ' + vehicleattributeseoname);

      var url = '/api/research-filter/vehicleattribute/categoryfiltergroups/'
        , jsonString = "{categoryname: '" + categoryname + "', vehicleattributeseoname: '" + vehicleattributeseoname + "'}";

      $.post(url, { '': jsonString })
        .done(function (resp) {

          var filterGroups = resp.data.categoryfiltergroups;

          for (var g = 0; g < filterGroups.length; g++) {
            var filterName = filterGroups[g].filtergroupname.toLowerCase();
            var filterCode = filterGroups[g].code.toLowerCase();
            var mappedFilterCode = filters.mapFilter2Icon(filterName, filterCode);

            var filterIconIdToBeSet = "js-filter-icon-" + filterName.toLowerCase() + "-" + mappedFilterCode.toLowerCase();
            $('#' + filterIconIdToBeSet).css('display', 'table-cell');

            var filterOptionsIdToBeSet = "js-filter-options-" + filterName.toLowerCase();
            $('#' + filterOptionsIdToBeSet).css('display', 'inline-block');

          }
        });
    },
    setVehicleAttributeTrimFilterAttributes: function (categoryname, vehicleattributeseoname) {

      //console.log('filters.setVehicleAttributeTrimFilterAttributes() was called with categoryname: ' + categoryname + ', vehicleattributeseoname: ' + vehicleattributeseoname);

      var url = '/api/research-filter/vehicleattribute/categorytrimfilterattributes/',
          jsonString = "{categoryname: '" + categoryname + "', vehicleattributeseoname: '" + vehicleattributeseoname + "'}";

      $.post(url, { '': jsonString })
          .done(function (resp) {
            if (resp.data === null) {
              if (retryCounter < 2) {
                retryCounter++;
                filters.setVehicleAttributeTrimFilterAttributes(categoryname, vehicleattributeseoname);
              }
            } else {
              trimFilterAttributes = resp.data.categorytrimfilterattributes;
            }
          });
    },

    setVehicleAttributeNoCategoryFilterGroups: function (vehicleattributeseoname) {

      //console.log('filters.setVehicleAttributeNoCategoryFilterGroups() was called with vehicleattributeseoname: ' + vehicleattributeseoname);

      var url = '/api/research-filter/vehicleattributeonly/categoryfiltergroups/'
        , jsonString = "{vehicleattributeseoname: '" + vehicleattributeseoname + "'}";

      $.post(url, { '': jsonString })
        .done(function (resp) {

          var filterGroups = resp.data.categoryfiltergroups;

          for (var g = 0; g < filterGroups.length; g++) {
            var filterName = filterGroups[g].filtergroupname.toLowerCase();
            var filterCode = filterGroups[g].code.toLowerCase();
            var mappedFilterCode = filters.mapFilter2Icon(filterName, filterCode);

            var filterIconIdToBeSet = "js-filter-icon-" + filterName.toLowerCase() + "-" + mappedFilterCode.toLowerCase();
            $('#' + filterIconIdToBeSet).css('display', 'table-cell');

            var filterOptionsIdToBeSet = "js-filter-options-" + filterName.toLowerCase();
            $('#' + filterOptionsIdToBeSet).css('display', 'inline-block');

          }
        });
    },
    setVehicleAttributeNoCategoryTrimFilterAttributes: function (vehicleattributeseoname) {

      //console.log('filters.setVehicleAttributeNoCategoryTrimFilterAttributes() was called with vehicleattributeseoname: ' + vehicleattributeseoname);

      var url = '/api/research-filter/vehicleattributeonly/categorytrimfilterattributes/',
          jsonString = "{vehicleattributeseoname: '" + vehicleattributeseoname + "'}";

      $.post(url, { '': jsonString })
          .done(function (resp) {
            if (resp.data === null) {
              if (retryCounter < 2) {
                retryCounter++;
                filters.setVehicleAttributeNoCategoryTrimFilterAttributes(vehicleattributeseoname);
              }
            } else {
              trimFilterAttributes = resp.data.categorytrimfilterattributes;
            }
          });
    },

    applyFilters: function (filterIconId) {

      //console.log('filters.applyFilters() was called with filterIconId: ' + filterIconId);

      var applyNewFilterFlag = true;

      if ($('#' + filterIconId).hasClass('selected')) {
        $('#' + filterIconId).removeClass("selected");
        applyNewFilterFlag = false;
      }

      //========================================================================================
      //build a list of filters to apply... previously selected ones + the new one if applicable
      //========================================================================================

      var allSelectedElements = $(".selected");
      var filtersToApply = [];

      for (var f = 0; f < allSelectedElements.length; f++) {
        if (allSelectedElements[f].id.indexOf("js-filter-icon") != -1) {
          filtersToApply.push(allSelectedElements[f].id);
        }
      }

      if (applyNewFilterFlag === true) {
        filtersToApply.push(filterIconId);
      }

      if (filtersToApply.length === 0) {
        var trimRowIds2Show = $("[id^='js-trimrow']");
        for (var r = 0; r < trimRowIds2Show.length; r++) {
          var trimRowId2Show = trimRowIds2Show[r].id;
          $('#' + trimRowId2Show).css('display', 'table-row-group');
        }

        //console.log('filters.applyFilters() - all trimrows set to display table-row-group');
        filters.updateButtonCounts();

      } else {
        filters.updateDomWithFilterCriteria(trimFilterAttributes, filtersToApply);
      }

    },
    updateDomWithFilterCriteria: function (trimFilterAttributes, filtersToApply) {

      //console.log('filters.updateDomWithFilterCriteria() was called with trimFilterAttributes.length: ' + trimFilterAttributes.length + ' and filtersToApply:' + filtersToApply);

      var trimRowIds = $("[id^='js-trimrow']");
      for (var r = 0; r < trimRowIds.length; r++) {
        var trimRowId = trimRowIds[r].id;
        $('#' + trimRowId).css('display', 'none');
      }

      //console.log('filters.updateDomWithFilterCriteria() - all trimrows set to display NONE');

      var filterGroupArrays = {};

      for (var x = 0; x < filtersToApply.length; x++) {

        var filterIdToApply = filtersToApply[x];
        var selectedIconFilterType = "";

        for (var t = 0; t < trimFilterAttributes.length; t++) {

          var trimId = trimFilterAttributes[t].id;

          var filterableAttributes = {};
          filterableAttributes.bodystyle = trimFilterAttributes[t].bodystyle;
          filterableAttributes.drivetrain = trimFilterAttributes[t].drivetrain;
          filterableAttributes.engine = trimFilterAttributes[t].enginetype;
          filterableAttributes.enginesize = trimFilterAttributes[t].enginesize;
          filterableAttributes.fueltype = trimFilterAttributes[t].fueltype;
          filterableAttributes.transmission = trimFilterAttributes[t].transmission;
          filterableAttributes.bedlength = trimFilterAttributes[t].bedlength;
          filterableAttributes.towingcapacity = trimFilterAttributes[t].towingcapacity;
          filterableAttributes.seating = trimFilterAttributes[t].seating;
          filterableAttributes.cargocapacity = trimFilterAttributes[t].cargocapacity;
          filterableAttributes.navigation = trimFilterAttributes[t].navigation;
          filterableAttributes.driverange = trimFilterAttributes[t].driverange;
          filterableAttributes.vehiclecategory = trimFilterAttributes[t].vehiclecategory;

          $.each(filterableAttributes, function (key, value) {

            var filterTypeName = key.toLowerCase();
            var filterName = filterIdToApply.split('-')[3].toLowerCase();

            if (filterName == filterTypeName) {

              var filterMatchValue = value.toLowerCase();
              var mappedFilterMatchValue = filters.mapFilter2Icon(filterTypeName, filterMatchValue);
              var trimIdToBeSet = "js-trimrow-" + trimId;

              var filterMatchValueCheck = filterIdToApply.split('-')[4].toLowerCase();
              if (mappedFilterMatchValue == filterMatchValueCheck) {

                //console.log('filters.updateDomWithFilterCriteria() - setting ' + trimIdToBeSet + ' to display table-row-group');

                $('#' + trimIdToBeSet).css('display', 'table-row-group');
                selectedIconFilterType = filterTypeName;

                //===============================================================================
                //for each filtertype, we'll capture which trim rows it affects... when all done,
                //we'll show only trims that are applicable for all the filter groups selected
                //===============================================================================

                var existingTrimIdsToBeSet = filterGroupArrays[filterTypeName];
                filterGroupArrays[filterTypeName] = existingTrimIdsToBeSet + "|" + trimIdToBeSet;

              }
            }
          });
        }

        $('#' + filterIdToApply).addClass("selected");

      }

      var keyCount = 0;
      $.each(filterGroupArrays, function (key, value) {
        keyCount++;
      });

      //========================================================================
      //if there's more than one key, there's more than one filter being applied
      //and we need to adjust displayed trims based on all filter groups chosen
      //========================================================================

      if (keyCount > 1) {

        for (var k = 0; k < trimFilterAttributes.length; k++) {
          var keyTrimIdRowId = "js-trimrow-" + trimFilterAttributes[k].id; //foreach trimid...
          var keyOkCheck = true;
          var keyOkFlag = true;

          //================================================================================
          //for each trim in the filterGroupArrays, check to see if trim exists
          //in the value string for the group... if it does not, we flag it and will hide it
          //================================================================================
          $.each(filterGroupArrays, function (key, value) {
            keyOkCheck = value.indexOf(keyTrimIdRowId) > 0;
            if (keyOkCheck === false) {
              keyOkFlag = false;
            }
          });

          if (keyOkFlag === false) {
            $('#' + keyTrimIdRowId).css('display', 'none');
          }
        }
      }

      filters.updateButtonCounts();
    },
    mapFilter2Icon: function (filterName, filterCode) {

      var returnCode = filterCode;

      if (filterName == "drivetrain" && filterCode == "4wheeldrive") { returnCode = "allwheeldrive"; }
      if (filterName == "engine" && filterCode == "10cylinderfamily") { returnCode = "8cylinderfamily"; }

      return returnCode;

    },
    showAllTrims: function () {

      var trimRowIds2Show = $("[id^='js-trimrow']");
      for (var r = 0; r < trimRowIds2Show.length; r++) {
        var trimRowId2Show = trimRowIds2Show[r].id;
        $('#' + trimRowId2Show).css('display', 'table-row-group');
      }

      var filterIcons = $("[id^='js-filter-icon']");
      for (var j = 0; j < filterIcons.length; j++) {
        var filterIconId = filterIcons[j].id;
        if ($('#' + filterIconId).hasClass('selected')) {
          $('#' + filterIconId).removeClass("selected");
        }

      }

      filters.updateButtonCounts();
    },
    updateButtonCounts: function () {

      //console.log('filters.updateButtonCounts() was called');

      var trimRowShowingCountTotal = 0;

      var trimCollections2BeExamined = $("[id^='js-trimcollection']");

      var supertrimImageLoadCounter = 1;
      for (var c = 0; c < trimCollections2BeExamined.length; c++) {
        var trimCollectionTrimRowShowingCount = 0;

        var trimMsrpWorking = 0;
        var trimMsrpFinal = 5000000;

        var workingTrimCollection = trimCollections2BeExamined[c];
        var workingTrimCollectionName = workingTrimCollection.id.replace("js-trimcollection-", "");

        var tabBodyWrappersCollection = workingTrimCollection.getElementsByClassName("tab-body-wrapper");
        var tabBodyWrapperDiv = tabBodyWrappersCollection[0]; //only one wrapper collection exists

        var tabBodyCollection = tabBodyWrapperDiv.getElementsByClassName("tab-body");
        var u = 0;

        //=======================
        //1st year tab (ex: 2016)
        //=======================
        if (tabBodyCollection[0] !== undefined) { 
          var sectionPadTab1Div = tabBodyCollection[0].childNodes[1]; //section-pad-tb within the 1st year tab (ex: 2016)
          var trimList1Div = sectionPadTab1Div.childNodes[1]; //trim-list-cont within the 1st year section pad (ex: 2016)

          //loop thru the first tab year trims (ex: 2016)
          for (u = 0; u < trimList1Div.childNodes.length; u++) {
            var workingTrimNode = trimList1Div.childNodes[u];
            if (workingTrimNode.id !== undefined && workingTrimNode.id.indexOf("js-trimrow") != -1) {
              if ($('#' + workingTrimNode.id).css('display') == 'table-row-group') {
                trimCollectionTrimRowShowingCount++;
                trimRowShowingCountTotal++;

                trimMsrpWorking = $('#' + workingTrimNode.id).data('trim-msrp');
                if (trimMsrpWorking < trimMsrpFinal) {
                  trimMsrpFinal = trimMsrpWorking;
                }

              }
            }
          }

        } else {
          //console.log('filters.updateButtonCounts() - tabBodyCollection[0] is not defined');
        }

        //=======================
        //2nd year tab (ex: 2015)
        //=======================
        if (tabBodyCollection[1] !== undefined) {
          var sectionPadTab2Div = tabBodyCollection[1].childNodes[1]; //section-pad-tb within the 2nd year tab (ex: 2015) 
          var trimList2Div = sectionPadTab2Div.childNodes[1]; //trim-list-cont within the 2nd year section pad (ex: 2015)

          //loop thru the second tab year trims (ex: 2015)
          for (u = 0; u < trimList2Div.childNodes.length; u++) {
            var workingTrimNode2 = trimList2Div.childNodes[u];
            if (workingTrimNode2.id !== undefined && workingTrimNode2.id.indexOf("js-trimrow") != -1) {
              if ($('#' + workingTrimNode2.id).css('display') == 'table-row-group') {
                trimCollectionTrimRowShowingCount++;
                trimRowShowingCountTotal++;

                trimMsrpWorking = $('#' + workingTrimNode2.id).data('trim-msrp');
                if (trimMsrpWorking < trimMsrpFinal) {
                  trimMsrpFinal = trimMsrpWorking;
                }
              }
            }
          }

        } else {
          //console.log('filters.updateButtonCounts() - tabBodyCollection[1] is not defined');
        }

        //=======================
        //3rd year tab (ex: 2014)
        //=======================
        if (tabBodyCollection[2] !== undefined) {
          var sectionPadTab3Div = tabBodyCollection[2].childNodes[1]; //section-pad-tb within the 3rd year tab (ex: 2014) 
          var trimList3Div = sectionPadTab3Div.childNodes[1]; //trim-list-cont within the 3rd year section pad (ex: 2014)

          //loop thru the third tab year trims (ex: 2014)
          for (u = 0; u < trimList3Div.childNodes.length; u++) {
            var workingTrimNode3 = trimList3Div.childNodes[u];
            if (workingTrimNode3.id !== undefined && workingTrimNode3.id.indexOf("js-trimrow") != -1) {
              if ($('#' + workingTrimNode3.id).css('display') == 'table-row-group') {
                trimCollectionTrimRowShowingCount++;
                trimRowShowingCountTotal++;

                trimMsrpWorking = $('#' + workingTrimNode3.id).data('trim-msrp');
                if (trimMsrpWorking < trimMsrpFinal) {
                  trimMsrpFinal = trimMsrpWorking;
                }
              }
            }
          }

        } else {
          //console.log('filters.updateButtonCounts() - tabBodyCollection[2] is not defined');
        }
 
        var workingButton = $('#js-viewtrimcollectionbutton-' + workingTrimCollectionName);
        workingButton[0].innerHTML = "<span class=\"trim-count\">" + trimCollectionTrimRowShowingCount + " <small>trims</small></span>";
        
        var workingMsrpDiv = $('#js-trimmsrp-' + workingTrimCollectionName);

        if (trimMsrpFinal < 5000000) {
          //console.log('filters.updateButtonCounts() - updating workingMsrpDiv for ' + workingTrimCollectionName + ' ' + trimMsrpFinal);
          workingMsrpDiv[0].innerHTML = "<small class=\"sub\">starting at</small><span class=\"item\">$" + trimMsrpFinal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span>";
        }
        
        //=============================================================
        //lines below cause the trims ShowingCount === 0 to not be seen
        //=============================================================
        var workingDisableSuperTrimName = "js-disablesupertrim-" + workingTrimCollectionName;

        if (trimCollectionTrimRowShowingCount === 0) {
          if ($('#' + workingDisableSuperTrimName).hasClass('reflow')) {
            $('#' + workingDisableSuperTrimName).removeClass("reflow");
          }
          $('#' + workingDisableSuperTrimName).css('display', 'none');
        } else {

          /*============================================================
          Updates img src to actual vehicle image when filters invoked. 
          50 seems to be a good number to make sure that when a filter
          is invoked enough images are loaded to fill the possible tiles
          in the viewport as they reflow up to the top of the view. 
          If we still see holes we can increase this number to update.
          ============================================================*/

          if (supertrimImageLoadCounter < 50) {
            var workingImage = $('#js-togglesupertrimimage-' + workingTrimCollectionName);
            var workingImageData = workingImage.data('img-src-placeholder');

            if (workingImageData !== undefined) {
              workingImage.attr("src", workingImageData);
              //console.log('filters.updateButtonCounts() - updating image ' + supertrimImageLoadCounter + ' for ' + workingImageData);
              supertrimImageLoadCounter++;
            }
          }

          $('#' + workingDisableSuperTrimName).css('display', 'block');
          $('#' + workingDisableSuperTrimName).addClass("reflow");
        }

      }

      var workingDiv1 = $('#js-totaltrimscount1');
      workingDiv1[0].innerHTML = trimRowShowingCountTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + " <small>trims</small>";

    }

  }
}();

module.exports = filters
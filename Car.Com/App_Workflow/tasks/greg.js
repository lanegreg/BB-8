
/* jshint node:true */
'use strict';


//#region - Variable Declarations
var gulp = require('gulp')
  , runSequence = require('run-sequence');

//#endregion



gulp.task('greg', function() {
  runSequence(
    //'clean',
    //'process-libs',
    //'process-inline-js',
    //'process-global-js',
    //'move-css-images',

    //'site.index-all',
    //'site.tools-all',
    //'site.contact-all',
    //'site.about-all',
    //'site.terms-all',
    //'site.privacy-all',
    //'site.copyright-all',
    //'site.fraud-all',
    //'site.sitemap-all',

    //'research.index-all',
    //'research.alternativefuel-all',
    //'research.category-all',
    //'research.make-all',
    //'research.supermodel-all',
    //'research.year-all',
    //'research.trimoverview-all',
    //'research.trimspecs-all',
    //'research.trimpicsnvids-all',
    //'research.trimwarranty-all',
    //'research.trimsafety-all',
    //'research.trimcolor-all',
    //'research.trimincentives-all',
    //'research.vehicleattribute-all',
    //'research.vehicleattributeresult-all',
    //'research.vehicleattributenocategoryresult-all',
    
    //'calculator.index-all',
    //'calculator.leaseorpurchase-all',
    //'calculator.loanvsfinancing-all',
    //'calculator.paymentestimate-all',
    //'calculator.fuelefficiency-all',
    //'calculator.acceleratedpayoff-all',
    //'calculator.affordability-all',

    //'carsforsale.index-all',
    //'carsforsale.make-all',
    //'carsforsale.results-all',
    //'carsforsale.vehicledetails-all',

    //'carvalue.index-all',

    //'comparecars.index-all',
    //'comparecars.results-all',

    //'article.index-all',
    //'article.articlegallery-all',

    //'httperror.status404-all',
    //'httperror.status500-all',
    
    'common.offerbootstrap-all',
    'common.offersystem-all',
    'common.c4swidget-all',
    'common.viewedrecentlywidget-all',

    'create-buster-manifest'
  )
})
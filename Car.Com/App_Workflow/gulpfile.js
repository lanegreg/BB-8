
/* jshint node:true */
'use strict';


//#region - Variable Declarations
var gulp = require('gulp')
  , runSequence = require('run-sequence')
  , buster = require('gulp-buster')
  , del = require('del')
  , fs = require('fs')
  , path = require('path')
  , busterfilename = 'buster_manifest.json';

//#endregion


///- Load all tasks from sub-folders
require('require-dir')('./tasks', { recurse: true });



//#region - Clean all artifacts and work folders.
gulp.task('clean', function (done) {
  del([
    '../app_assets/dist/js/**/*.*',
    '../app_assets/dist/css/**/*.*',
    '../app_assets/dist/libs/**/*.*',
    '../app_assets/tmp/**/',
    '../assets/above_the_fold/*.*',
    '../assets/*.*'
  ], { force: true }, done);
});

//#endregion


//#region - Move CSS and Images

gulp.task('move-css-images', function() {
  return gulp
    .src('../app_assets/sass/img/*')
    .pipe(gulp.dest('../app_assets/dist/css/img/'));
});

//#endregion


//#region - Cache Busting Page Level Assets

function getCacheBustedAssets() {
  var minSuffix = '.min'
    , cacheBustedAssets = []
    , manifest = require('./' + busterfilename);
    
  for (var filepath in manifest) {
    var hash = manifest[filepath]
      , fp = filepath.replace(minSuffix, '')
      , ext = path.extname(fp)
      , dirname = path.dirname(fp)
      , basename = path.basename(fp, ext)
      , bustedfilename = basename + '-' + hash + minSuffix + ext
      , bustedfilepath = path.join(dirname, bustedfilename);

    cacheBustedAssets.push({
      filepath: filepath,
      bustedfilepath: bustedfilepath.replace(/\\/g, '/')
    });
  }

  return cacheBustedAssets;
}

gulp.task('create-buster-manifest', function () {
  if (fs.existsSync('./' + busterfilename))
    fs.unlinkSync('./' + busterfilename);

  return gulp
    .src([
      '../app_assets/dist/js/*',
      '../app_assets/dist/css/*',
      '../app_assets/dist/libs/global.site.min.js'
    ])
    .pipe(buster({
      fileName: busterfilename,
      algo: 'sha1',
      length: 8
    }))
    .pipe(gulp.dest('.'))
    .pipe(gulp.dest('../assets/'));
});

//#endregion



//#region - High-level Gulp Tasks

//- Used by build machine
gulp.task('produce-busted-files', function (done) {
  var assets = getCacheBustedAssets()
    , numOfFiles = assets.length
    , count = 1;

  var checkForDone = function () {
    if (count++ == numOfFiles)
      done();
  };

  for (var i in assets) {
    var asset = assets[i];

    if (fs.existsSync(asset.bustedfilepath))
      fs.unlinkSync(asset.bustedfilepath);

    fs.createReadStream(asset.filepath)
      .on('close', function () {
        checkForDone();
      })
      .pipe(fs.createWriteStream(asset.bustedfilepath));

    del(asset.filepath, { force: true });
  }
});

  //#endregion


//#region - Our BIG Gulp

//- Used by local devs
gulp.task('default', function() {
  runSequence(
    'clean',
    'process-libs',
    'process-inline-js',
    'process-global-js',
    'move-css-images',

    'site.index-all',
    'site.tools-all',
    'site.contact-all',
    'site.about-all',
    'site.terms-all',
    'site.privacy-all',
    'site.copyright-all',
    'site.fraud-all',
    'site.sitemap-all',

    'research.index-all',
    'research.alternativefuel-all',
    'research.category-all',
    'research.make-all',
    'research.supermodel-all',
    'research.year-all',
    'research.trimoverview-all',
    'research.trimspecs-all',
    'research.trimpicsnvids-all',
    'research.trimwarranty-all',
    'research.trimsafety-all',
    'research.trimcolor-all',
    'research.trimincentives-all',
    'research.vehicleattribute-all',
    'research.vehicleattributeresult-all',
    'research.vehicleattributenocategoryresult-all',
    
    'calculator.index-all',
    'calculator.leaseorpurchase-all',
    'calculator.loanvsfinancing-all',
    'calculator.paymentestimate-all',
    'calculator.fuelefficiency-all',
    'calculator.acceleratedpayoff-all',
    'calculator.affordability-all',

    'carsforsale.index-all',
    'carsforsale.make-all',
    'carsforsale.results-all',
    'carsforsale.vehicledetails-all',
    'carsforsale.selectmodels-all',
    'carsforsale.selectcategorymakes-all',

    'carvalue.index-all',

    'comparecars.index-all',
    'comparecars.results-all',

    'buyingguides.index-all',
    'buyingguides.article-all',
    'buyingguides.articlecategory-all',

    'finance.index-all',
    'finance.article-all',

    'sponsored.nativo-all',

    'common.offerbootstrap-all',
    'common.offersystem-all',
    'common.c4swidget-all',
    'common.viewedrecentlywidget-all',
    'common.c4srecentlyviewedwdg-all',

    'httperror.status404-all',
    'httperror.status500-all',
    
    'create-buster-manifest'
  )
})

//#endregion
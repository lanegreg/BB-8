module.exports.add_byg_overlay=function anonymous(it
/**/) {
var out='<div id="byg_overlay_js" class="overlay byg-bg-b" style="display:none"> <button type="button" id="close_byg_ovl_btn_js" class="overlay-close" data-omnitarget="leadpop:closebtn"> <span class="icon"> <svg> <use xlink:href="/assets/svg/global-sprite.svg#i_close"></use> </svg> </span> </button> <div class="dead-middle"> <section class="leadform-thx-cont" id="ol_byg_section" style="display: block"> <div class="row inner-cont"> <div class="aw-feed-cont" id="ol_byg_AutowebAds" style="display: block"></div> </div> </section> </div></div>';return out;
};module.exports.add_side_panel=function anonymous(it
/**/) {
var out='<aside id="offerpanel_wrap_js" class="side-panel"> <a href="" id="offerpanel_close_btn_js"> <span class="overlay-close"> <span class="icon"> <svg> <use xlink:href="/assets/svg/global-sprite.svg#i_close"></use> </svg> </span> </span> </a> <div class="side-panel-inner-cont" id="sp_Parent"> <div class="col1"> <section id="sp_c4s_Lead" style="display: none;"> <p> <a href="" class="btn-primary lg js_sp_contact_dealer_btn" style="width: 225px;"> Contact the Seller </a> </p> <div id="sp_c4s_offer_links"></div> </section> <section id="sp_NewCar_Lead"> </section> <section id="sp_OfferZip" style="display: none;"> <hr />  <p> <span> Enter your zipcode for additional offers<br /> </span> <label> <input type="text" id="offersZipcode" name="offersZipcode" class="full js_offers_zipcode" pattern="[0-9]*" placeholder="ZIP Code" value="" maxlength="5" /> </label> </p> <p> <span id="offersZipErrorMessage" class="error" style="display: none;"> You must enter a valid ZIP Code<br /> </span> <button id="SetOffersZip" class="btn-primary block">See Offers</button> </p> </section> <section id="sp_c4s_inventory_link" style="display: none;"> </section> </div> <div class="col2"> <section class="special-offers" id="sp_Special_Offers" style="display: none;"> </section> </div> </div></aside>';return out;
};module.exports.nav_zip_overlay=function anonymous(it
/**/) {
var out='<div id="nav_zip_overlay_js" class="overlay overlay-dk max-450" style="display:none"> <button class="close_btn_js overlay-close"> <span class="icon"> <svg> <use xlink:href="/assets/svg/global-sprite.svg#i_close"></use> </svg> </span> </button> <div class="dead-middle"> <div class="form center"> <h1 class="h1">Update Zip Code</h1> <form class="car-form"> <p> <input id="nav_zip_js" type="text" placeholder="99999" class="sm lg-text" pattern="[0-9]{5}" required /> </p> <p> <button class="query_btn_js btn-primary block lg">Update</button> </p> </form> </div> </div></div>';return out;
};module.exports.overlay_byg_autoweb=function anonymous(it
/**/) {
var out=' <section id="v-b"> <h1 id="awBygMessage" class="hdr-cont"> <span class="byg">Before You Go!</span> <span class="h3">Get the Best Price on a new '+(it.adData.Make)+' '+(it.adData.Model)+' in '+(it.adData.Location)+'.</span> </h1> <ul class="aw-feed-list"> ';var arr1=it.adData.adList.Listings;if(arr1){var adItem,adidx=-1,l1=arr1.length-1;while(adidx<l1){adItem=arr1[adidx+=1];out+=' ';if(adidx < 5){out+=' <li class="aw-feed-item"> <a href="'+(adItem.LandingUrl)+'" data-omnitarget="leadpop:autoweb" data-omnicallback="trackAutoweb" target="_blank"> <div class="col-thumb"> <img src="'+(adItem.LogoUrl)+'" /> </div> <div class="col-desc"> <h3 class="header">'+(adItem.Title)+'</h3> <p> <span>'+(adItem.Description1)+'</span> <br /> <span>';if(adItem.Description2){out+=''+(adItem.Description2);}out+='</span> <br /> <span class="aw-url aw_url_js">'+(adItem.DisplayUrl)+'</span> </p> </div> <div class="col-btn"> <button class="btn-secondary lg">Get Quote <span class="icon inline"> <svg> <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/assets/svg/global-sprite.svg#i_arrow3_r"></use> </svg> </span> </button> </div> </a> </li> ';}out+=' ';} } out+=' </ul></section>';return out;
};
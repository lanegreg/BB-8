module.exports.car_for_sale=function anonymous(it
/**/) {
var out='';var arr1=it.cars;if(arr1){var t,idx=-1,l1=arr1.length-1;while(idx<l1){t=arr1[idx+=1];var img=t.imgUrl,p=t.price,m=t.mileage,c=t.city,s=t.state,y=t.year,k=t.make,l=t.model,u=t.url,v=t.vin;out+=' <div class="vwrec_card_js widget-list-item"> <!--<div class="vwrec_card_js widget-list-item" style="';if(idx >= it.maxToShow){out+='display:none';}out+='">--> <div class="img-container has-chkbox"> ';if(img.length){out+=' <img src="'+( img)+'" alt="" style="width:100%;" /> ';}else{out+=' <img src="/assets/svg/no-image-avail_4x3.svg" alt="" style="width:100%;"/> ';}out+=' ';if((idx - parseInt(it.recentsPerPage)) >= 0){out+=' <span id="suggested_vehicle_label" class="img-label suggested">SUGGESTED</span> ';}out+=' <label for="'+( v)+( idx)+'"> <input id="'+( v)+( idx)+'" name="'+( v)+( idx)+'" value="Selected" type="checkbox" /> Select </label> </div> <a href="/cars-for-sale/vehicle-details/?q='+( t.dealerId)+'|'+( t.id)+'|'+( it.zipcode)+'|'+( t.make_id)+'|'+( t.model)+'" title="View this vehicle"> <div class="desc"> <span class="data secondary-data">'+( y)+', '+( k)+', '+( l)+'</span> <span class="data secondary-data"><small>VIN '+( v)+'</small></span> <span class="data secondary-data">'+( m)+' Miles</span> <span class="data secondary-data">'+( c)+', '+( s)+'</span> <span class="data primary-data"><strong>$'+( p)+'</strong></span> <!--';if(idx === 0){out+=' <div class="cur-viewed-cont"> <FONT style="BACKGROUND-COLOR: white">&nbsp;Currently Viewing&nbsp;</FONT> </div> ';}out+='--> </div> <div> <input type="hidden" id="vin" name="vin" value="'+( v)+'">  <input type="hidden" id="carId" name="carId" value="'+( t.id)+'">  <input type="hidden" id="make" name="make" value="'+( k)+'">  <input type="hidden" id="model" name="model" value="'+( l)+'">  <input type="hidden" id="year" name="year" value="'+( y)+'">  <input type="hidden" id="dealerId" name="dealerId" value="'+( t.dealerId)+'">  <input type="hidden" id="dealerPhone" name="dealerPhone" value="'+( t.phone)+'">  </div> </a> </div> ';} } return out;
};module.exports.widget_wrap=function anonymous(it
/**/) {
var out='<div id="c4s_recvwd_wdg_wrap_js"> <h3 class="widget-heading">Recently Viewed</h3> <div id="c4s_varray"> <div id="c4s_recvwd_wdg_cars_wrap_js"></div> </div> <!--<button id="vwrec_wdg_showall_btn_js" class="btn-default block" style="display:none">See More +</button>--> <div align="center"> <p><label for="c4s_wdg_checkAll"><input type="checkbox" id="c4s_wdg_checkAll" />Select All</label></p> </div> <div align="center"> <button id="c4s_wdg_getpricing_btn_js" class="leadoverlay_pop_btn_js btn-primary block">Get Pricing</button> </div></div>';return out;
};
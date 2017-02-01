module.exports.dealer_car_contact=function anonymous(it
/**/) {
var out='';var car=it, dealerPhone=car.dealer.phone;out+='<div class="contact-dlr tmpl_replace_js"> ';if(dealerPhone.length===10){out+=' <div class="phone"> <span class="call">or Call</span> <span class="number"> <a href="tel:'+( dealerPhone)+'">'+( dealerPhone.replace(/(\d{3})(\d{3})(\d{4})/, '($1) $2-$3'))+'</a> </span> </div> <p class="center"> Call Internet Sales and refer to the '+( car.year)+' '+( car.make)+' '+( car.model)+' with Stock ID "#'+( car.id)+'" that you saw on Car.com. </p> ';}out+='</div>';return out;
};module.exports.dealer_hours=function anonymous(it
/**/) {
var out='';var car=it;out+='<div class="tmpl_replace_dealerhours_js"> <h2>Hours</h2><ul><li class="row"><span class="label">Friday</span><span class="value"> '+( car.dealerhours.salesfriopen)+'<span> - </span>'+( car.dealerhours.salesfriclose)+'</span></li><li class="row"><span class="label">Saturday</span><span class="value"> '+( car.dealerhours.salessatopen)+'<span> - </span>'+( car.dealerhours.salessatclose)+'</span></li><li class="row"><span class="label">Sunday</span><span class="value"> '+( car.dealerhours.salessunopen)+'<span> - </span>'+( car.dealerhours.salessunclose)+'</span></li><li class="row"><span class="label">Monday</span><span class="value"> '+( car.dealerhours.salesmonopen)+'<span> - </span>'+( car.dealerhours.salesmonclose)+'</span></li><li class="row"><span class="label">Tuesday</span><span class="value"> '+( car.dealerhours.salestueopen)+'<span> - </span>'+( car.dealerhours.salestueclose)+'</span></li><li class="row"><span class="label">Wednesday</span><span class="value"> '+( car.dealerhours.saleswedopen)+'<span> - </span>'+( car.dealerhours.saleswedclose)+'</span></li><li class="row"><span class="label">Thursday</span><span class="value"> '+( car.dealerhours.salesthropen)+'<span> - </span>'+( car.dealerhours.salesthrclose)+'</span></li></ul></div>';return out;
};module.exports.dealer_identifier=function anonymous(it
/**/) {
var out='';var car=it;out+='<div class="tmpl_replace_js"> ';if(car.dealer.autonationdealer){out+='  <div class="an-copy-container"> <img src="/assets/img/autonation/an-logo-wh-108x23.png" alt="AutoNation logo" class="an-logo" /> </div> ';}out+=' <h2 class="h2">'+( car.dealer.name)+'</h2> <span>'+( car.dealer.city)+'</span></div>';return out;
};module.exports.dealer_map=function anonymous(it
/**/) {
var out='';var car=it;out+='<div class="tmpl_replace_dealermap_js"> <div id="js_dealer_map" data-latitude="'+( car.dealer.latitude)+'" data-longitude="'+( car.dealer.longitude)+'"></div> <div id="map-canvas" style="display: block; width: 100%; height: 350px; border: 1px solid black"></div> <div style="display: block; width: 100%; text-align: center">'+( car.dealer.name)+' '+( car.dealer.city)+', '+( car.dealer.state)+'</div></div>';return out;
};module.exports.dlr_car_messaging=function anonymous(it
/**/) {
var out='';var car=it;out+='<ul>';if(car.hasDetails()){out+='<li><input type="checkbox" checked><i></i><h2>More Details</h2><p>'+( car.details)+'</p></li> ';}out+=' ';if(car.hasNotes()){out+='<li><input type="checkbox" checked> <i></i> <h2>Seller\'s Notes</h2> <p>'+( car.seller_notes)+'</p> </li> ';}out+='</ul>';if(car.hasDlrMsg()){out+='<div class="dlr-message-cont"> <p> '+( car.dealer.message)+' &ndash; <strong>'+( car.dealer.name)+'</strong> </p> </div>';}return out;
};module.exports.feature_grid=function anonymous(it
/**/) {
var out='';var arr1=it;if(arr1){var el,idx=-1,l1=arr1.length-1;while(idx<l1){el=arr1[idx+=1];out+='<li class="row"> <span class="label">'+( el.label)+'</span> <span class="value">'+( el.value)+'</span></li>';} } return out;
};module.exports.inventory_nav=function anonymous(it
/**/) {
var out='<ul> <li> <button class="prev" data-goto="prev" title="Previous"> <span class="icon inline"> <svg> <use xlink:href="/assets/svg/global-sprite.svg#i_arrow3_l"></use> </svg> </span> Previous Car </button> </li> <li> <button class="next" data-goto="next" title="Next"> Next Car <span class="icon inline"> <svg> <use xlink:href="/assets/svg/global-sprite.svg#i_arrow3_r"></use> </svg> </span> </button> </li></ul>';return out;
};module.exports.slider_frames=function anonymous(it
/**/) {
var out='';var car=it;out+='<ul> ';var arr1=car.image_urls;if(arr1){var imgUrl,idx=-1,l1=arr1.length-1;while(idx<l1){imgUrl=arr1[idx+=1];out+=' <li> <img src="';if(idx < 2){out+=''+( imgUrl);}out+='" data-lazysrc="';if(idx > 1){out+=''+( imgUrl);}out+='" class="frame_js" alt="'+( car.year)+' '+( car.make)+' '+( car.model)+'"> </li> ';} } out+='</ul>';return out;
};
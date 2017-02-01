module.exports.incentives_by_zip=function anonymous(it
/**/) {
var out=''; var currentIncentiveType = "";  var currentIncentiveCount = 0; var arr1=it.incentives;if(arr1){var incentive,index=-1,l1=arr1.length-1;while(index<l1){incentive=arr1[index+=1];out+=' ';if(incentive.groupdesc != currentIncentiveType){out+=' ';if(currentIncentiveCount > 0){out+=' </tbody> </table>  ';}out+=' '; currentIncentiveType = incentive.groupdesc; out+=' '; currentIncentiveCount++; out+='  <div class="r-tbl"> <table> <caption>'+(currentIncentiveType)+' Incentives and Rebates</caption> <thead> <tr> <th>Rebate</th> <th>24 Mo</th> <th>36 Mo</th> <th>48 Mo</th> <th>60 Mo</th> <th>72 Mo</th> <th>Expires</th> <th>Category</th> </tr> </thead> <tbody> ';}out+=' <tr> <td data-label="Rebate"> ';if(incentive.amount > 0){out+='$'+(incentive.amount);}else if(true){out+='-';}out+=' </td> <td data-label="24 month term"> ';if(incentive.amount === 0){out+=''+(incentive.apr_24)+'%';}else if(true){out+='-';}out+=' </td> <td data-label="36 month term"> ';if(incentive.amount === 0){out+=''+(incentive.apr_36)+'%';}else if(true){out+='-';}out+=' </td> <td data-label="48 month term"> ';if(incentive.amount === 0){out+=''+(incentive.apr_48)+'%';}else if(true){out+='-';}out+=' </td> <td data-label="60 month term"> ';if(incentive.amount === 0){out+=''+(incentive.apr_60)+'%';}else if(true){out+='-';}out+=' </td> <td data-label="72 month term"> ';if(incentive.amount === 0){out+=''+(incentive.apr_72)+'%';}else if(true){out+='-';}out+=' </td> <td data-label="Expires">'+(incentive.expires)+'</td> <td data-label="Category">'+(incentive.catdesc)+'</td> </tr> ';} } out+=' </tbody> </table></div>';return out;
};
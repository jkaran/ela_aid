
/**************************************************************
 * Script name : common
 * Created date : SEP-04-2015
 * Created by : Pointel, Inc.
 * Purpose : JavaScript source code for salesforce Objects
 * Modified Date : 31-10-2015
 *************************************************************/

function CommonProcess(AdapterData) {
    try {
        if (AdapterData) {
            var SearchData = AdapterData.CommonSearchData;
            SearchData = SearchData.replace(/\^\,/g, "");
            SearchData = SearchData.replace(/\^/g, "");
            var firstsearchkey = SearchData.split(',');
            var searchKey = SearchData.replace(/\,/g," "+ AdapterData.CommonSearchCondition+" ");
            var commonPagenames = AdapterData.CommonPopupObjects.split(',');
            sforce.interaction.runApex('SFDCSearchObject', 'Search', 'SFsearchkey=' + SearchData + '&SearchCondition='+AdapterData.CommonSearchCondition+'&format=' + AdapterData.CommonSearchFormats + '&Pagename=' + AdapterData.CommonPopupObjects + '', function (response) {
                try {
                    if (response.result) {
                        var record = $.parseJSON(response.result);
                        if (isArrayEmpty(record)) {
                            $.ajax({
                                url: Url + "/NoRecordFound?AdapterData.InteractionId=" + AdapterData.InteractionId + "&SearchData=" + SearchData + '&SearchCondition='+AdapterData.CommonSearchCondition+'&format=' + AdapterData.CommonSearchFormats + '&Pagename=' + AdapterData.CommonPopupObjects,
                                dataType: "jsonp",
                                crossDomain: true,
                                contentType: "application/json; charset=utf-8",
                                async: true
                            });
                            //No Record found handling
                            var norecordseachurl = '';
                            $.each(commonPagenames, function (id, item) {
                                if (item) {
                                    switch (item) {

                                        case "Contact":
                                            if (AdapterData.ContactData) {
                                                var ContactNewData = '';
                                                if (AdapterData.ContactData.NewRecordFieldIds) {
                                                    ContactNewData = '/003/e?' + NewFieldId(SearchData, AdapterData.ContactData, "con13");
                                                } else if (firstsearchkey && firstsearchkey[0].lenght == 10) {
                                                    ContactNewData = '/003/e?con13=' + firstsearchkey[0];
                                                } else {
                                                    ContactNewData = '/003/e';
                                                }
                                                if (AdapterData.ContactData.NoRecordFound == "searchpage") {
                                                    norecordseachurl += '&sen=003&';
                                                }
                                                NoRecordFound(AdapterData.ContactData, AdapterData.InteractionId, ContactNewData, '003', SearchData, '&whoId=');

                                            }
                                            break;
                                        case "Case":
                                            if (AdapterData.CaseData) {
                                                var CaseNewData = '';
                                                if (AdapterData.CaseData.NewRecordFieldIds) {
                                                    CaseNewData = '/500/e?' + NewFieldId(SearchData, AdapterData.CaseData, "");
                                                } else {
                                                    CaseNewData = '/500/e';
                                                }
                                                if (AdapterData.CaseData.NoRecordFound == "searchpage") {
                                                    norecordseachurl += '&sen=500&';
                                                }
                                                NoRecordFound(AdapterData.CaseData, AdapterData.InteractionId, CaseNewData, '500', SearchData, '&whatId=');

                                            }
                                            break;
                                        case "Account":

                                            if (AdapterData.AccountData) {
                                                var AccountNewData = '';
                                                if (AdapterData.AccountData.NewRecordFieldIds) {
                                                    AccountNewData = '/001/e?' + NewFieldId(SearchData, AdapterData.AccountData, "acc10");
                                                } else if (firstsearchkey && firstsearchkey[0].lenght == 10) {
                                                    AccountNewData = '/001/e?acc10=' + firstsearchkey[0];
                                                } else {
                                                    AccountNewData = '/001/e';
                                                }
                                                if (AdapterData.AccountData.NoRecordFound == "searchpage") {
                                                    norecordseachurl += '&sen=001&';
                                                }
                                                NoRecordFound(AdapterData.AccountData, AdapterData.InteractionId, AccountNewData, '001', SearchData, '&whatId=');

                                            } break;
                                        case "Lead":
                                            if (AdapterData.LeadData) {
                                                var LeadNewData = '';
                                                if (AdapterData.LeadData.NewRecordFieldIds) {
                                                    LeadNewData = '/00Q/e?' + NewFieldId(SearchData, AdapterData.LeadData, "lea8");
                                                } else if (firstsearchkey && firstsearchkey[0].lenght == 10) {
                                                    LeadNewData = '/00Q/e?lea8=' + firstsearchkey[0];
                                                } else {
                                                    LeadNewData = '/00Q/e';
                                                }
                                                if (AdapterData.LeadData.NoRecordFound == "searchpage") {
                                                    norecordseachurl += '&sen=00Q&';
                                                }
                                                NoRecordFound(AdapterData.LeadData, AdapterData.InteractionId, LeadNewData, '00Q', SearchData, '&whoId=');
                                            }
                                            break;
                                        case "Opportunity":

                                            if (AdapterData.OpportunityData) {
                                                var OpportunityNewData = '';
                                                if (AdapterData.OpportunityData.NewRecordFieldIds) {
                                                    OpportunityNewData = '/006/e?' + NewFieldId(SearchData, AdapterData.OpportunityData, "");
                                                } else {
                                                    OpportunityNewData = '/006/e';
                                                }
                                                if (AdapterData.OpportunityData.NoRecordFound == "searchpage") {
                                                    norecordseachurl += '&sen=006&';
                                                }
                                                NoRecordFound(AdapterData.OpportunityData, AdapterData.InteractionId, OpportunityNewData, '006', SearchData, '&whatId=');
                                            }
                                            break;
                                        default:
                                            if (item.indexOf("__c") > -1 && AdapterData.CustomObjectData) {
                                                $.each(AdapterData.CustomObjectData, function (i) {
                                                    if (AdapterData.CustomObjectData[i].ObjectName === item) {
                                                        if (AdapterData.CustomObjectData[i]) {
                                                            var CustomNewData = '';
                                                            if (AdapterData.CustomObjectData[i].CustomObjectURL) {
                                                                if (AdapterData.CustomObjectData[i].NewRecordFieldIds) {
                                                                    CustomNewData = '/' + AdapterData.CustomObjectData[i].CustomObjectURL + '/e?' + NewFieldId(SearchData, AdapterData.CustomObjectData[i], "");
                                                                } else {
                                                                    CustomNewData = '/' + AdapterData.CustomObjectData[i].CustomObjectURL + '/e';
                                                                }
                                                                if (AdapterData.CustomObjectData[i].NoRecordFound == "searchpage") {
                                                                    norecordseachurl += '&sen=' + AdapterData.CustomObjectData[i].CustomObjectURL + '&';
                                                                }
                                                                NoRecordFound(AdapterData.CustomObjectData[i], AdapterData.InteractionId, CustomNewData, AdapterData.CustomObjectData[i].CustomObjectURL, SearchData, '&whatId=');
                                                            }
                                                            return false;//exit loop
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                    }
                                }
                            });

                            if (norecordseachurl) {
                                popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&' + norecordseachurl + '&str=' + searchKey);
                            }
                        } else {
                            var multirecordIDs = [];
                            var pagename = '';
                            var searchurl = '';
                            var searchAll = false;
                            if (record) {



                                //Record found handling
                                $.each(record, function (id, item) {
                                    if (!isArrayEmpty(item)) {
                                        var recordlength = 0;
                                        if (item) {
                                            try {
                                                recordlength = Object.keys(item).length;
                                            } catch (e) {
                                                recordlength = Object.size(item);
                                            }
                                        }
                                        pagename = item[0].attributes.type;
                                        switch (pagename) {
                                            case "Contact":
                                                RecordMatch(item, AdapterData.ContactData, AdapterData.InteractionId, '&whoId=', searchKey);
                                                if (recordlength > 1) {
                                                    if (AdapterData.ContactData && AdapterData.ContactData.MultipleMatchRecord && AdapterData.ContactData.MultipleMatchRecord == "searchpage" && AdapterData.ContactData.SearchpageMode) {
                                                        if (AdapterData.ContactData.SearchpageMode == "all") {
                                                            searchAll = true;
                                                        } else {
                                                            searchurl += AdapterData.ContactData.SearchpageMode;
                                                        }
                                                    }
                                                }
                                                break;
                                            case "Case":
                                                RecordMatch(item, AdapterData.CaseData, AdapterData.InteractionId, '&whatId=', searchKey);
                                                if (recordlength > 1) {
                                                    if (AdapterData.CaseData && AdapterData.CaseData.MultipleMatchRecord && AdapterData.CaseData.MultipleMatchRecord == "searchpage" && AdapterData.CaseData.SearchpageMode) {

                                                        if (AdapterData.CaseData.SearchpageMode == "all") {
                                                            searchAll = true;
                                                        } else {
                                                            searchurl += AdapterData.CaseData.SearchpageMode;
                                                        }
                                                    }
                                                }
                                                break;
                                            case "Account":
                                                RecordMatch(item, AdapterData.AccountData, AdapterData.InteractionId, '&whatId=', searchKey);
                                                if (recordlength > 1) {
                                                    if (AdapterData.AccountData && AdapterData.AccountData.MultipleMatchRecord && AdapterData.AccountData.MultipleMatchRecord == "searchpage" && AdapterData.AccountData.SearchpageMode) {

                                                        if (AdapterData.AccountData.SearchpageMode == "all") {
                                                            searchAll = true;
                                                        } else {
                                                            searchurl += AdapterData.AccountData.SearchpageMode;
                                                        }

                                                    }
                                                }
                                                break;
                                            case "Lead":
                                                RecordMatch(item, AdapterData.LeadData, AdapterData.InteractionId, '&whoId=', searchKey);
                                                if (recordlength > 1) {
                                                    if (AdapterData.LeadData && AdapterData.LeadData.MultipleMatchRecord && AdapterData.LeadData.MultipleMatchRecord == "searchpage" && AdapterData.LeadData.SearchpageMode) {
                                                        if (AdapterData.LeadData.SearchpageMode == "all") {
                                                            searchAll = true;
                                                        } else {
                                                            searchurl += AdapterData.LeadData.SearchpageMode;
                                                        }
                                                    }
                                                }
                                                break;
                                            case "Opportunity":
                                                RecordMatch(item, AdapterData.OpportunityData, AdapterData.InteractionId, '&whatId=', searchKey);
                                                if (recordlength > 1) {
                                                    if (AdapterData.OpportunityData && AdapterData.OpportunityData.MultipleMatchRecord && AdapterData.OpportunityData.MultipleMatchRecord == "searchpage" && AdapterData.OpportunityData.SearchpageMode) {
                                                        if (AdapterData.OpportunityData.SearchpageMode == "all") {
                                                            searchAll = true;
                                                        } else {
                                                            searchurl += AdapterData.OpportunityData.SearchpageMode;
                                                        }
                                                    }
                                                }
                                                break;
                                            default:

                                                if (pagename.indexOf("__c") > -1 && AdapterData.CustomObjectData) {
                                                    $.each(AdapterData.CustomObjectData, function (i) {
                                                        if (AdapterData.CustomObjectData[i].ObjectName === pagename) {
                                                            RecordMatch(item, AdapterData.CustomObjectData[i], AdapterData.InteractionId, '&whatId=', searchKey);
                                                            if (recordlength > 1) {
                                                                if (AdapterData.CustomObjectData[i] && AdapterData.CustomObjectData[i].MultipleMatchRecord && AdapterData.CustomObjectData[i].MultipleMatchRecord == "searchpage" && AdapterData.CustomObjectData[i].SearchpageMode) {

                                                                    if (AdapterData.CustomObjectData[i].SearchpageMode == "all") {
                                                                        searchAll = true;
                                                                    } else {
                                                                        searchurl += AdapterData.CustomObjectData[i].SearchpageMode;
                                                                    }
                                                                }
                                                            }
                                                            return false;//exit loop
                                                        }
                                                    });
                                                }
                                                break;
                                        }
                                    }
                                });
                                if (searchAll) {   //all object search
                                    popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&str=' + searchKey);
                                } else if (searchurl) {
                                    //given object search
                                    popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&' + searchurl + '&str=' + searchKey);
                                }

                            }
                        }
                    }
                } catch (e) {
                    if (enableAlertError) {
                        alert("Common SFDCSearchObject error " + e.description);
                    }
                }
            });
        }

    } catch (e) {
        if (enableAlertError) {
            alert("Common process error " + e.description);
        }
    }
}
function isArrayEmpty(test) {

    for (var i in test) {
        if (!jQuery.isEmptyObject(test[i])) {
            return false;
        }
    }
    return true;

}

function RecordMatch(record, Info, InteractionId, typeid, searchKey) {
    var recordlength = 0;
    if (record) {
        try {
            recordlength = Object.keys(record).length;
        } catch (e) {
            recordlength = Object.size(record);
        }
    }
    try {
        //  $.each(record, function (id, item) {
        var i = 0;
        var multirecordIDs = [];
        for (i = 0; i <= recordlength; i++) {
            if (record[i]) {
                if (recordlength === 1) {

                    popupPage(record[i].Id);
                }
                if (recordlength === 1 && Info.ActivityLogData) {
                    sforce.interaction.saveLog('Task', Info.ActivityLogData + typeid + record[i].Id, function (response) {
                        if (response.result) {
                            $.ajax({
                                url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + Info.ObjectName + "&ActivityId=" + response.result,
                                dataType: "jsonp",
                                crossDomain: true,
                                contentType: "application/json; charset=utf-8",
                                async: true
                            });
                            if (enableAlertResponse) {
                                alert(" save log call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert(" save log call back error-" + response.error);
                            }
                        }
                    });
                } else if (recordlength > 1 && Info.MultipleMatchRecord) {
                    if (Info.MultipleMatchRecord == "openall") {
                        multirecordIDs.push(record[i].Id);
                    }
                }
            }
        }

        if (multirecordIDs.length > 0 && Info.MaxRecordOpenCount) {
            var count = Math.min(multirecordIDs.length, Info.MaxRecordOpenCount);
            for (var i = 0; i < count; i++) {
                popupPage(multirecordIDs[i]);
            }
        }
    } catch (e) {
        if (enableAlertError == "True") {
            alert("Error in Common RecordMatch:   " + e.message);
        }
    }

}

function NoRecordFound(Info, InteractionId, OpenNewData, SearchURL, searchKey, TypeId) {
    try {
        if (Info.NoRecordFound && Info.NoRecordFound != "none") {
            if (Info.NoRecordFound == "opennew" && OpenNewData) {
                popupPage(OpenNewData);
                return;
            }
            else if (Info.NoRecordFound == "createnew" && Info.CreateRecordFieldData) {
                sforce.interaction.saveLog(Info.ObjectName, Info.CreateRecordFieldData, function (response) {
                    if (response.result) {
                        var flag = true;
                        var recordid = response.result;
                        popupPage(response.result);
                        if (enableAlertResponse) {
                            alert(" Create response- " + response.result);;
                        }
                        if (recordid && Info.ActivityLogData && Info.CreateLogForNewRecord) {
                            sforce.interaction.saveLog("Task", Info.ActivityLogData + TypeId + recordid, function (response1) {
                                if (response1.result) {
                                    if (enableAlertResponse) {
                                        alert(" Create log response- " + response1.result);
                                    }
                                    var activityid = response1.result;
                                    flag = false;
                                    $.ajax({
                                        url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + Info.ObjectName + "&ActivityId=" + activityid + "&RecordId=" + recordid,
                                        dataType: "jsonp",
                                        crossDomain: true,
                                        contentType: "application/json; charset=utf-8",
                                        async: true
                                    });
                                } else {
                                    if (enableAlertError) {
                                        alert(" Create log Error- " + response1.error);
                                    }
                                }
                            });
                        }
                        if (flag) {
                            $.ajax({
                                url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + Info.ObjectName + "&RecordId=" + recordid,
                                dataType: "jsonp",
                                crossDomain: true,
                                contentType: "application/json; charset=utf-8",
                                async: true
                            });

                        }

                    } else {
                        if (enableAlertError) {
                            alert(" Create Error- " + response.error);
                        }
                    }
                });
            }
        }
    } catch (e) {
        if (enableAlertError == "True") {
            alert("Error in Common NoRecordFound:   " + e.message);
        }
    }
}
function NewFieldId(CommonSearchData, Info, PhoneField) {
    try {
        var NewFieldIds = "&";
        var KeyValue = "";
        if (Info.NewRecordFieldIds) {
            KeyValue = CommonSearchData.split(",");
            var NewRecordFieldIds = Info.NewRecordFieldIds.split(",");
            //Forming Lead new page prepopulation value
            $.each(KeyValue, function (i) {
                if (NewRecordFieldIds && KeyValue[i] != "^" && NewRecordFieldIds[i] != "n/a") {
                    if (PhoneField && NewRecordFieldIds[i] == PhoneField) {
                        if (KeyValue[i].length == 10) {
                            NewFieldIds += NewRecordFieldIds[i] + "=" + KeyValue[i] + "&";
                        }
                    } else {
                        NewFieldIds += NewRecordFieldIds[i] + "=" + KeyValue[i] + "&";
                    }
                }
            });
        }
        return NewFieldIds;
    } catch (e) {
        if (enableAlertError == "True") {
            alert("Error in Common NewFieldId:   " + e.message);
        }

    }

}
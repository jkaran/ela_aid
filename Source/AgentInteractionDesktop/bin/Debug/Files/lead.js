
/**************************************************************
 * Script name : SFDClead
 * Created date : SEP-04-2015
 * Created by : Pointel, Inc.
 * Purpose : JavaScript source code for salesforce lead Object
 *************************************************************/

function LeadProcess(LeadInfo, InteractionId) {
    try {
       if (LeadInfo && !$.isEmptyObject(LeadInfo) && InteractionId && LeadInfo.ObjectName) {

            var leadkeyvalue = "";
            var searchKey = "";
            var Leadnewfields = "&";
            if (LeadInfo.SearchData) {
                if (LeadInfo.SearchCondition) {
                    var removedcap = LeadInfo.SearchData.replace(/\^\,/g, "");
                    removedcap = removedcap.replace(/\^/g, "");
                    searchKey = removedcap.replace(/\,/g, " " + LeadInfo.SearchCondition + " ");
                }
                if (LeadInfo.NewRecordFieldIds) {
                    Leadkeyvalue = LeadInfo.SearchData.split(",");
                    var LeadNewRecordFieldIds = LeadInfo.NewRecordFieldIds.split(",");

                    //Forming Lead new page prepopulation value
                    $.each(Leadkeyvalue, function (i) {
                        if (Leadkeyvalue[i] != "^" && LeadNewRecordFieldIds[i] != "n/a") {
                            if (LeadNewRecordFieldIds[i] == "lea8") {
                                if (Leadkeyvalue[i].length == 10) {
                                    Leadnewfields += LeadNewRecordFieldIds[i] + "=" + Leadkeyvalue[i] + "&";
                                }
                            } else {
                                Leadnewfields += LeadNewRecordFieldIds[i] + "=" + Leadkeyvalue[i] + "&";
                            }
                        }
                    });
                }
                LeadInfo.SearchData = LeadInfo.SearchData.replace(/\^\,/g, "");
                LeadInfo.SearchData = LeadInfo.SearchData.replace(/\^/g, "");
            }

            if (LeadInfo.UpdateRecordFields && LeadInfo.UpdateRecordFieldsData && LeadInfo.RecordID) {
                sforce.interaction.saveLog(LeadInfo.ObjectName, LeadInfo.UpdateRecordFieldsData + "&Id=" + LeadInfo.RecordID, function (response) {
                    if (response.result) {
                        if (enableAlertResponse) {
                            alert("Lead Update response- "+response.result);
                        }

                    } else {
                        if (enableAlertError) {
                            alert("Lead Update Error- " + response.error);
                        }
                    }
                });
            }

            if (LeadInfo.UpdateActivityLog) {
                if (LeadInfo.ActivityRecordID && LeadInfo.UpdateActivityLogData) {
                    sforce.interaction.saveLog('Task', LeadInfo.UpdateActivityLogData + "&Id=" + LeadInfo.ActivityRecordID, function (response) {
                        if (response.result) {
                            if (enableAlertResponse) {
                                alert("Update Lead activity history call back response-"+response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("Update Lead activity history call back error-"+response.error);
                            }
                        }
                    });
                }
            } else {
                //Click to dail log handling
                if (LeadInfo.ClickToDialRecordId && LeadInfo.ActivityLogData) {
                    sforce.interaction.saveLog('Task', LeadInfo.ActivityLogData + "&" + LeadInfo.ClickToDialRecordId + "&CallType=Outbound", function (response) {
                        if (response.result) {
                            $.ajax({
                                url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + LeadInfo.ObjectName + "&ActivityId=" + response.result,
                                dataType: "jsonp",
                                crossDomain: true,
                                contentType: "application/json; charset=utf-8",
                                async: true
                            });
                            if (enableAlertResponse) {
                                alert("Lead CTD call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("conatct call back error-"+ response.error);
                            }
                        }
                    });
                } else if (LeadInfo.SearchData && LeadInfo.SearchData != "^") {
                    sforce.interaction.runApex('SFDCSearchObject', 'Search', 'SFsearchkey=' + LeadInfo.SearchData + '&SearchCondition=' + LeadInfo.SearchCondition + '&format=' + LeadInfo.PhoneNumberSearchFormat + '&Pagename=' + LeadInfo.ObjectName + '', function (response) {
                        try {
                            if (response.result) {
                                var record1 = $.parseJSON(response.result);
                                var record = 0;
                                record = record1[0];
                                // var firstRecord = record[0];
                                var tempsk = LeadInfo.SearchData.split(',');
                                var jsonCount = 0;
                                if (record1[0]) {
                                    try {
                                        jsonCount = Object.keys(record1[0]).length;
                                    } catch (e) {
                                        jsonCount = Object.size(record1[0]);
                                    }
                                }
                                if (response.result == "[[]]") {
                                             $.ajax({
                                                 url: Url + "/NoRecordFound?InteractionId=" + InteractionId + "&SearchData=" + LeadInfo.SearchData + '&SearchCondition=' + LeadInfo.SearchCondition + '&format=' + LeadInfo.PhoneNumberSearchFormat + '&Pagename=' + LeadInfo.ObjectName,
                                        dataType: "jsonp",
                                        crossDomain: true,
                                        contentType: "application/json; charset=utf-8",
                                        async: true
                                    });
                                    if (LeadInfo.NoRecordFound) {
                                        if (LeadInfo.NoRecordFound == "opennew") {
                                            if (LeadInfo.NewRecordFieldIds) {
                                                //Empty new lead page
                                                popupPage('/00Q/e?' + Leadnewfields);
                                                Leadnewfields = "";
                                            } else {
                                                //new lead page only with phone field
                                                if (tempsk[0].length == 10) {
                                                    popupPage('/00Q/e?lea8=' + tempsk[0]);
                                                } else {
                                                    popupPage('/00Q/e');
                                                }
                                            }
                                            return;
                                        } else if (LeadInfo.NoRecordFound == "searchpage") {
                                            //Empty lead search page
                                            popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&sen=00Q&str=' + searchKey);
                                            return;
                                        }
                                        else if (LeadInfo.NoRecordFound == "createnew" && LeadInfo.CreateRecordFieldData) {
                                            sforce.interaction.saveLog(LeadInfo.ObjectName, LeadInfo.CreateRecordFieldData, function (response) {
                                                if (response.result) {
                                                    var flag = true;
                                                    var recordid = response.result;
                                                    popupPage(response.result);
                                                    if (enableAlertResponse) {
                                                        alert("Lead Create response- " + response.result);;
                                                    }
                                                    if (recordid && LeadInfo.ActivityLogData && LeadInfo.CreateLogForNewRecord) {
                                                        sforce.interaction.saveLog("Task", LeadInfo.ActivityLogData + "&whoId=" + recordid, function (response1) {
                                                            if (response1.result) {
                                                                if (enableAlertResponse) {
                                                                    alert("Lead Create log response- " + response1.result);
                                                                }
                                                                var activityid = response1.result;
                                                                flag = false;
                                                                $.ajax({
                                                                    url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + LeadInfo.ObjectName + "&ActivityId=" + activityid + "&RecordId=" + recordid,
                                                                    dataType: "jsonp",
                                                                    crossDomain: true,
                                                                    contentType: "application/json; charset=utf-8",
                                                                    async: true
                                                                });
                                                            } else {
                                                                if (enableAlertError) {
                                                                    alert("Lead Create log Error- "+response1.error);
                                                                }
                                                            }
                                                        });
                                                    }
                                                    if (flag) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + LeadInfo.ObjectName + "&RecordId=" + recordid,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });

                                                    }

                                                } else {
                                                    if (enableAlertError) {
                                                        alert("Lead Create Error- " + response.error);
                                                    }
                                                }
                                            });
                                            return;
                                        } else {
                                            return;
                                        }
                                    } else {
                                        return;
                                    }
                                }
                                var multirecordIDs = [];
                                if (record) {
                                    $.each(record, function (id, item) {
                                        if (item.Id) {
                                            if (jsonCount == 1) {
                                                popupPage(item.Id);
                                            }
                                            if (jsonCount == 1 && LeadInfo.ActivityLogData) {
                                                sforce.interaction.saveLog('Task', LeadInfo.ActivityLogData + '&whoId=' + item.Id, function (response) {
                                                    if (response.result) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + LeadInfo.ObjectName + "&ActivityId=" + response.result,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });
                                                        if (enableAlertResponse) {
                                                            alert("Lead save log call back response-"+response.result);
                                                        }
                                                    } else {
                                                        if (enableAlertError) {
                                                            alert("Lead save log call back error-"+response.error);
                                                        }
                                                    }
                                                });
                                            } else if (jsonCount > 1 && LeadInfo.MultipleMatchRecord) {
                                                if (LeadInfo.MultipleMatchRecord == "searchpage" && LeadInfo.SearchpageMode) {
                                                    if (LeadInfo.SearchpageMode == 'all') {
                                                        // All page search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&str=' + searchKey);
                                                    } else {
                                                        //Lead search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&' + LeadInfo.SearchpageMode + '&str=' + searchKey);
                                                    }
                                                    return false; //exit loop  
                                                } else if (LeadInfo.MultipleMatchRecord == "openall") {
                                                    multirecordIDs.push(item.Id);
                                                }
                                            }
                                        }
                                    }
                                        );
                                    if (multirecordIDs.length > 0 && LeadInfo.MaxRecordOpenCount) {
                                        var count = Math.min(multirecordIDs.length, LeadInfo.MaxRecordOpenCount);
                                        for (var i = 0; i < count; i++) {
                                            popupPage(multirecordIDs[i]);
                                        }
                                    }
                                }
                            }
                        } catch (e) {
                            if (enableAlertError) {
                                alert("Lead SFDCSearchObject error "+e.description);
                            }
                        }
                    });
                }
            }
        }
    } catch (e) {
        if (enableAlertError) {
            alert("LeadProcess error " + e.description);
        }
    }
}
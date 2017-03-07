
/**************************************************************
 * Script name : Case
 * Created date : SEP-04-2015
 * Created by : Pointel, Inc.
 * Purpose : JavaScript source code for salesforce Case Object
 *************************************************************/

function CaseProcess(CaseInfo, InteractionId) {
    try {
       if (CaseInfo && !$.isEmptyObject(CaseInfo)&&InteractionId  &&CaseInfo.ObjectName) {
            var Casekeyvalue = "";
            var searchKey = "";
                    var Casenewfields = "&";
            if (CaseInfo.SearchData) {
                if (CaseInfo.SearchCondition) {
                    var removedcap = CaseInfo.SearchData.replace(/\^\,/g, "");
                    removedcap = removedcap.replace(/\^/g, "");
                    searchKey = removedcap.replace(/\,/g, " " + CaseInfo.SearchCondition + " ");
                }
                if (CaseInfo.NewRecordFieldIds) {
                    Casekeyvalue = CaseInfo.SearchData.split(",");
                    var CaseNewRecordFieldIds = CaseInfo.NewRecordFieldIds.split(",");
                    //Forming Case new page prepopulation value
                    $.each(Casekeyvalue, function (i) {
                        if (Casekeyvalue[i] != "^" && CaseNewRecordFieldIds[i] != "n/a") {
                            Casenewfields += CaseNewRecordFieldIds[i] + "=" + Casekeyvalue[i] + "&";
                        }
                    });
                }
                CaseInfo.SearchData = CaseInfo.SearchData.replace(/\^\,/g, "");
                CaseInfo.SearchData = CaseInfo.SearchData.replace(/\^/g, "");
            }

            if (CaseInfo.UpdateRecordFields && CaseInfo.UpdateRecordFieldsData && CaseInfo.RecordID) {
                sforce.interaction.saveLog(CaseInfo.ObjectName, CaseInfo.UpdateRecordFieldsData + "&Id=" + CaseInfo.RecordID, function (response) {
                    if (response.result) {
                        if (enableAlertResponse) {
                            alert("Case Update response- " + response.result);
                        }
                    } else {
                        if (enableAlertError) {
                            alert("Case Update Error- " + response.error);
                        }
                    }
                });
            }

            if (CaseInfo.UpdateActivityLog) {
                if (CaseInfo.ActivityRecordID && CaseInfo.UpdateActivityLogData) {
                    sforce.interaction.saveLog('Task', CaseInfo.UpdateActivityLogData + "&Id=" + CaseInfo.ActivityRecordID, function (response) {
                        if (response.result) {
                            if (enableAlertResponse) {
                                alert("Update Case activity history call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("Update Case activity history call back error-" + response.error);
                            }
                        }
                    });
                }
            } else {
                //Click to dail log handling
                if (CaseInfo.ClickToDialRecordId && CaseInfo.ActivityLogData) {
                    sforce.interaction.saveLog('Task', CaseInfo.ActivityLogData + "&" + CaseInfo.ClickToDialRecordId + "&CallType=Outbound", function (response) {
                        if (response.result) {
                            $.ajax({
                                url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CaseInfo.ObjectName + "&ActivityId=" + response.result,
                                dataType: "jsonp",
                                crossDomain: true,
                                contentType: "application/json; charset=utf-8",
                                async: true
                            });
                            if (enableAlertResponse) {
                                alert("Case CTD call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("conatct call back error-" + response.error);
                            }
                        }
                    });
                } else if (CaseInfo.SearchData && CaseInfo.SearchData != "^") {
                    sforce.interaction.runApex('SFDCSearchObject', 'Search', 'SFsearchkey=' + CaseInfo.SearchData + '&SearchCondition=' + CaseInfo.SearchCondition + '&format=' + CaseInfo.PhoneNumberSearchFormat + '&Pagename=' + CaseInfo.ObjectName + '', function (response) {
                        try {
                            if (response.result) {
                                var record1 = $.parseJSON(response.result);
                                var record = 0;
                                record = record1[0];
                                var firstRecord = record[0];
                                var tempsk = CaseInfo.SearchData.split(',');
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
                                        url: Url + "/NoRecordFound?InteractionId=" + InteractionId + "&SearchData=" + CaseInfo.SearchData + '&SearchCondition=' + CaseInfo.SearchCondition + '&format=' + CaseInfo.PhoneNumberSearchFormat + '&Pagename=' + CaseInfo.ObjectName,
                                        dataType: "jsonp",
                                        crossDomain: true,
                                        contentType: "application/json; charset=utf-8",
                                        async: true
                                    });
                                    if (CaseInfo.NoRecordFound) {
                                        if (CaseInfo.NoRecordFound == "opennew") {
                                            if (CaseInfo.NewRecordFieldIds) {
                                                //Empty new Case page
                                                popupPage('/500/e?' + Casenewfields);
                                                Casenewfields = "";
                                            } else {
                                                popupPage('/500/e');
                                            }
                                            return;
                                        } else if (CaseInfo.NoRecordFound == "searchpage") {
                                            //Empty Case search page
                                            popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&sen=500&str=' + searchKey);
                                            return;
                                        }
                                        else if (CaseInfo.NoRecordFound == "createnew" && CaseInfo.CreateRecordFieldData) {
                                            sforce.interaction.saveLog(CaseInfo.ObjectName, CaseInfo.CreateRecordFieldData, function (response) {
                                                if (response.result) {
                                                    var flag = true;
                                                    var recordid = response.result;
                                                    popupPage(response.result);
                                                    if (enableAlertResponse) {
                                                        alert("Case Create response- " + response.result);
                                                    }
                                                    if (recordid && CaseInfo.ActivityLogData && CaseInfo.CreateLogForNewRecord) {
                                                        sforce.interaction.saveLog("Task", CaseInfo.ActivityLogData + "&whatId=" + recordid, function (response1) {
                                                            if (response1.result) {
                                                                if (enableAlertResponse) {
                                                                    alert("Case Create log response- " + response1.result);
                                                                }
                                                                var activityid = response1.result;
                                                                flag = false;
                                                                $.ajax({
                                                                    url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CaseInfo.ObjectName + "&ActivityId=" + activityid + "&RecordId=" + recordid,
                                                                    dataType: "jsonp",
                                                                    crossDomain: true,
                                                                    contentType: "application/json; charset=utf-8",
                                                                    async: true
                                                                });
                                                            } else {
                                                                if (enableAlertError) {
                                                                    alert("Case Create log Error- " + response1.error);
                                                                }
                                                            }
                                                        });
                                                    }
                                                    if (flag) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CaseInfo.ObjectName + "&RecordId=" + recordid,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });

                                                    }

                                                } else {
                                                    if (enableAlertError) {
                                                        alert("Case Create Error- " + response.error);
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
                                            if (jsonCount == 1 && CaseInfo.ActivityLogData) {
                                                sforce.interaction.saveLog('Task', CaseInfo.ActivityLogData + '&whatId=' + item.Id, function (response) {
                                                    if (response.result) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CaseInfo.ObjectName + "&ActivityId=" + response.result,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });
                                                        if (enableAlertResponse) {
                                                            alert("Case save log call back response-" + response.result);
                                                        }
                                                    } else {
                                                        if (enableAlertError) {
                                                            alert("Case save log call back error-" + response.error);
                                                        }
                                                    }
                                                });
                                            } else if (jsonCount > 1 && CaseInfo.MultipleMatchRecord) {
                                                if (CaseInfo.MultipleMatchRecord == "searchpage" && CaseInfo.SearchpageMode) {
                                                    if (CaseInfo.SearchpageMode == 'all') {
                                                        // All page search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&str=' + searchKey);
                                                    } else {
                                                        //Case search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&' + CaseInfo.SearchpageMode + '&str=' + searchKey);
                                                    }
                                                    return false; //exit loop  
                                                } else if (CaseInfo.MultipleMatchRecord == "openall" && CaseInfo.MaxRecordOpenCount) {
                                                    multirecordIDs.push(item.Id);
                                                }
                                            }
                                        }
                                    }
                                    );
                                    if (multirecordIDs.length > 0 && CaseInfo.MaxRecordOpenCount) {
                                        var count = Math.min(multirecordIDs.length,CaseInfo.MaxRecordOpenCount);
                                        for (var i = 0; i < count; i++) {
                                            popupPage(multirecordIDs[i]);
                                        }
                                    }
                                }
                            }
                        } catch (e) {
                            if (enableAlertError) {
                                alert("SFDCSearchObject error " + e.description);
                            }
                        }
                    });
                }
            }
        }

    }
    catch (e) {
        if (enableAlertError) {
            alert("CaseProcess error " + e.description);
        }
    }
}
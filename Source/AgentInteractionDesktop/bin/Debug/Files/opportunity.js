
/**************************************************************
 * Script name : Opportunity
 * Created date : SEP-04-2015
 * Created by : Pointel, Inc.
 * Purpose : JavaScript source code for salesforce Opportunity Object
 *************************************************************/

function OpportunityProcess(OpportunityInfo, InteractionId) {
    try {
        if (OpportunityInfo && !$.isEmptyObject(OpportunityInfo) && InteractionId && OpportunityInfo.ObjectName) {
            var Opportunitykeyvalue = "";
            var Opportunitynewfields = "&";
            var searchKey = "";
            if (OpportunityInfo.SearchData) {
                if (OpportunityInfo.SearchCondition) {
                    var removedcap = OpportunityInfo.SearchData.replace(/\^\,/g, "");
                    removedcap = removedcap.replace(/\^/g, "");
                    searchKey = removedcap.replace(/\,/g, " " + OpportunityInfo.SearchCondition + " ");
                }
                if (OpportunityInfo.NewRecordFieldIds) {
                    Opportunitykeyvalue = OpportunityInfo.SearchData.split(",");
                    var OpportunityNewRecordFieldIds = OpportunityInfo.NewRecordFieldIds.split(",");
                    //Forming Opportunity new page prepopulation value
                    $.each(Opportunitykeyvalue, function (i) {
                        if (Opportunitykeyvalue[i] != "^" && OpportunityNewRecordFieldIds[i] != "n/a") {
                            Opportunitynewfields += OpportunityNewRecordFieldIds[i] + "=" + Opportunitykeyvalue[i] + "&";
                        }
                    });
                }
                OpportunityInfo.SearchData = OpportunityInfo.SearchData.replace(/\^\,/g, "");
                OpportunityInfo.SearchData = OpportunityInfo.SearchData.replace(/\^/g, "");
            }

            if (OpportunityInfo.UpdateRecordFields && OpportunityInfo.UpdateRecordFieldsData && OpportunityInfo.RecordID) {
                sforce.interaction.saveLog(OpportunityInfo.ObjectName, OpportunityInfo.UpdateRecordFieldsData + "&Id=" + OpportunityInfo.RecordID, function (response) {
                    if (response.result) {
                        if (enableAlertResponse) {
                            alert("Opportunity Update response- " + response.result);
                        }
                    } else {
                        if (enableAlertError) {
                            alert("Opportunity Update Error- " + response.error);
                        }
                    }
                });
            }
            if (OpportunityInfo.UpdateActivityLog) {
                if (OpportunityInfo.ActivityRecordID && OpportunityInfo.UpdateActivityLogData) {
                    sforce.interaction.saveLog('Task', OpportunityInfo.UpdateActivityLogData + "&Id=" + OpportunityInfo.ActivityRecordID, function (response) {
                        if (response.result) {
                            if (enableAlertResponse) {
                                alert("Update Opportunity activity history call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("Update Opportunity activity history call back error-" + response.error);
                            }
                        }
                    });
                }
            } else {
                //Click to dail log handling
                if (OpportunityInfo.ClickToDialRecordId && OpportunityInfo.ActivityLogData) {
                    sforce.interaction.saveLog('Task', OpportunityInfo.ActivityLogData + "&" + OpportunityInfo.ClickToDialRecordId + "&CallType=Outbound", function (response) {
                        if (response.result) {
                            $.ajax({
                                url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + OpportunityInfo.ObjectName + "&ActivityId=" + response.result,
                                dataType: "jsonp",
                                crossDomain: true,
                                contentType: "application/json; charset=utf-8",
                                async: true
                            });
                            if (enableAlertResponse) {
                                alert("Opportunity CTD call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("Opportunity call back error-" + response.error);
                            }
                        }
                    });
                } else if (OpportunityInfo.SearchData && OpportunityInfo.SearchData != "^") {
                    sforce.interaction.runApex('SFDCSearchObject', 'Search', 'SFsearchkey=' + OpportunityInfo.SearchData + '&SearchCondition=' + OpportunityInfo.SearchCondition + '&format=' + OpportunityInfo.PhoneNumberSearchFormat + '&Pagename=' + OpportunityInfo.ObjectName + '', function (response) {
                        try {
                            if (response.result) {
                                var record1 = $.parseJSON(response.result);
                                var record = 0;
                                record = record1[0];
                                var firstRecord = record[0];
                                var tempsk = OpportunityInfo.SearchData.split(',');
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
                                        url: Url + "/NoRecordFound?InteractionId=" + InteractionId + "&SearchData=" + OpportunityInfo.SearchData + '&SearchCondition=' + OpportunityInfo.SearchCondition + '&format=' + OpportunityInfo.PhoneNumberSearchFormat + '&Pagename=' + OpportunityInfo.ObjectName,
                                        dataType: "jsonp",
                                        crossDomain: true,
                                        contentType: "application/json; charset=utf-8",
                                        async: true
                                    });
                                    if (OpportunityInfo.NoRecordFound) {
                                        if (OpportunityInfo.NoRecordFound == "opennew") {
                                            if (OpportunityInfo.NewRecordFieldIds) {
                                                //Empty new Opportunity page
                                                popupPage('/006/e?' + Opportunitynewfields);
                                                Opportunitynewfields = "";
                                            } else {
                                                //new Opportunity page only with phone field
                                                popupPage('/006/e');
                                            }
                                            return;
                                        } else if (OpportunityInfo.NoRecordFound == "searchpage") {
                                            //Empty Opportunity search page
                                            popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&sen=006&str=' + searchKey);
                                            return;
                                        }
                                        else if (OpportunityInfo.NoRecordFound == "createnew" && OpportunityInfo.CreateRecordFieldData) {
                                            sforce.interaction.saveLog(OpportunityInfo.ObjectName, OpportunityInfo.CreateRecordFieldData, function (response) {
                                                if (response.result) {
                                                    var flag = true;
                                                    var recordid = response.result;
                                                    popupPage(response.result);
                                                    if (enableAlertResponse) {
                                                        alert("Opportunity Create response- " + response.result);
                                                    }
                                                    if (recordid && OpportunityInfo.ActivityLogData && OpportunityInfo.CreateLogForNewRecord) {
                                                        sforce.interaction.saveLog("Task", OpportunityInfo.ActivityLogData + "&whatId=" + recordid, function (response1) {
                                                            if (response1.result) {
                                                                if (enableAlertResponse) {
                                                                    alert("Opportunity Create log response- " + response1.result);
                                                                }
                                                                var activityid = response1.result;
                                                                flag = false;
                                                                $.ajax({
                                                                    url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + OpportunityInfo.ObjectName + "&ActivityId=" + activityid + "&RecordId=" + recordid,
                                                                    dataType: "jsonp",
                                                                    crossDomain: true,
                                                                    contentType: "application/json; charset=utf-8",
                                                                    async: true
                                                                });
                                                            } else {
                                                                if (enableAlertError) {
                                                                    alert("Opportunity Create log Error- " + response1.error);
                                                                }
                                                            }
                                                        });
                                                    }
                                                    if (flag) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + OpportunityInfo.ObjectName + "&RecordId=" + recordid,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });

                                                    }

                                                } else {
                                                    if (enableAlertError) {
                                                        alert("Opportunity Create Error- " + response.error);
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
                                            if (jsonCount == 1 && OpportunityInfo.ActivityLogData) {
                                                sforce.interaction.saveLog('Task', OpportunityInfo.ActivityLogData + '&whatId=' + item.Id, function (response) {
                                                    if (response.result) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + OpportunityInfo.ObjectName + "&ActivityId=" + response.result,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });
                                                        if (enableAlertResponse) {
                                                            alert("Opportunity save log call back response-" + response.result);
                                                        }
                                                    } else {
                                                        if (enableAlertError) {
                                                            alert("Opportunity save log call back error-" + response.error);
                                                        }
                                                    }
                                                });
                                            } else if (jsonCount > 1 && OpportunityInfo.MultipleMatchRecord) {
                                                if (OpportunityInfo.MultipleMatchRecord == "searchpage" && OpportunityInfo.SearchpageMode) {
                                                    if (OpportunityInfo.SearchpageMode == 'all') {
                                                        // All page search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&str=' + searchKey);
                                                    } else {
                                                        //Opportunity search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&' + OpportunityInfo.SearchpageMode + '&str=' + searchKey);
                                                    }
                                                    return false; //exit loop  
                                                } else if (OpportunityInfo.MultipleMatchRecord == "openall" && OpportunityInfo.MaxRecordOpenCount) {
                                                    multirecordIDs.push(item.Id);
                                                }
                                            }
                                        }
                                    }
                                    );
                                    if (multirecordIDs.length > 0 && OpportunityInfo.MaxRecordOpenCount) {
                                        var count = Math.min(multirecordIDs.length, OpportunityInfo.MaxRecordOpenCount);
                                        for (var i = 0; i < count; i++) {
                                            popupPage(multirecordIDs[i]);
                                        }
                                    }
                                }
                            }
                        } catch (e) {
                            if (enableAlertError) {
                                alert("Opportunity SFDCSearchObject error " + e.description);
                            }
                        }
                    });
                }
            }
        }
    }
    catch (e) {
        if (enableAlertError) {
            alert("OpportunityProcess error " + e.description);
        }
    }
}
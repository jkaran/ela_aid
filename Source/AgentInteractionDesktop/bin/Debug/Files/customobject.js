
/**************************************************************
 * Script name : CustomObject
 * Created date : SEP-04-2015
 * Created by : Pointel, Inc.
 * Purpose : JavaScript source code for salesforce Custom Object
 *************************************************************/

function CustomObjectProcess(CustomObjectInfo, InteractionId) {
    try {
        if (CustomObjectInfo && !$.isEmptyObject(CustomObjectInfo) && InteractionId && CustomObjectInfo.ObjectName) {
            var CustomObjectkeyvalue = "";
            var searchKey = "";
            var CustomObjectnewfields = "&";
            if (CustomObjectInfo.SearchData) {
                if (CustomObjectInfo.SearchCondition) {
                    var removedcap = CustomObjectInfo.SearchData.replace(/\^\,/g, "");
                    removedcap = removedcap.replace(/\^/g, "");
                    searchKey = removedcap.replace(/\,/g, " " + CustomObjectInfo.SearchCondition + " ");
                }
                if (CustomObjectInfo.NewRecordFieldIds) {
                    CustomObjectkeyvalue = CustomObjectInfo.SearchData.split(",");
                    var CustomObjectNewRecordFieldIds = CustomObjectInfo.NewRecordFieldIds.split(",");
                    //Forming CustomObject new page prepopulation value
                    $.each(CustomObjectkeyvalue, function (i) {
                        if (CustomObjectkeyvalue[i] != "^" && CustomObjectNewRecordFieldIds[i] != "n/a") {
                            CustomObjectnewfields += CustomObjectNewRecordFieldIds[i] + "=" + CustomObjectkeyvalue[i] + "&";
                        }
                    });
                }
                CustomObjectInfo.SearchData = CustomObjectInfo.SearchData.replace(/\^\,/g, "");
                CustomObjectInfo.SearchData = CustomObjectInfo.SearchData.replace(/\^/g, "");
            }

            if (CustomObjectInfo.UpdateRecordFields && CustomObjectInfo.UpdateRecordFieldsData && CustomObjectInfo.RecordID) {
                sforce.interaction.saveLog(CustomObjectInfo.ObjectName, CustomObjectInfo.UpdateRecordFieldsData + "&Id=" + CustomObjectInfo.RecordID, function (response) {
                    if (response.result) {
                        if (enableAlertResponse) {
                            alert("CustomObject Update response- " + response.result);
                        }
                    } else {
                        if (enableAlertError) {
                            alert("CustomObject Update Error- " + response.error);
                        }
                    }
                });
            }
            if (CustomObjectInfo.UpdateActivityLog) {
                if (CustomObjectInfo.ActivityRecordID && CustomObjectInfo.UpdateActivityLogData) {
                    sforce.interaction.saveLog('Task', CustomObjectInfo.UpdateActivityLogData + "&Id=" + CustomObjectInfo.ActivityRecordID, function (response) {
                        if (response.result) {
                            if (enableAlertResponse) {
                                alert("Update CustomObject activity history call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("Update CustomObject activity history call back error-" + response.error);
                            }
                        }
                    });
                }
            } else {
                //Click to dail log handling
                if (CustomObjectInfo.ClickToDialRecordId && CustomObjectInfo.ActivityLogData) {
                    sforce.interaction.saveLog('Task', CustomObjectInfo.ActivityLogData + "&" + CustomObjectInfo.ClickToDialRecordId + "&CallType=Outbound", function (response) {
                        if (response.result) {
                            $.ajax({
                                url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CustomObjectInfo.ObjectName + "&ActivityId=" + response.result,
                                dataType: "jsonp",
                                crossDomain: true,
                                contentType: "application/json; charset=utf-8",
                                async: true
                            });
                            if (enableAlertResponse) {
                                alert("CustomObject CTD call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("CustomObject call back error-" + response.error);
                            }
                        }
                    });
                } else if (CustomObjectInfo.SearchData && CustomObjectInfo.SearchData != "^") {
                    sforce.interaction.runApex('SFDCSearchObject', 'Search', 'SFsearchkey=' + CustomObjectInfo.SearchData + '&SearchCondition=' + CustomObjectInfo.SearchCondition + '&format=' + CustomObjectInfo.PhoneNumberSearchFormat + '&Pagename=' + CustomObjectInfo.ObjectName + '', function (response) {
                        try {
                            if (response.result) {
                                var record1 = $.parseJSON(response.result);
                                var record = 0;
                                record = record1[0];
                                var firstRecord = record[0];
                                var tempsk = CustomObjectInfo.SearchData.split(',');
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
                                        url: Url + "/NoRecordFound?InteractionId=" + InteractionId + "&SearchData=" + CustomObjectInfo.SearchData + '&SearchCondition=' + CustomObjectInfo.SearchCondition + '&format=' + CustomObjectInfo.PhoneNumberSearchFormat + '&Pagename=' + CustomObjectInfo.ObjectName,
                                        dataType: "jsonp",
                                        crossDomain: true,
                                        contentType: "application/json; charset=utf-8",
                                        async: true
                                    });
                                    if (CustomObjectInfo.NoRecordFound) {
                                        if (CustomObjectInfo.NoRecordFound == "opennew" && CustomObjectInfo.CustomObjectURL) {
                                            if (CustomObjectInfo.NewRecordFieldIds) {
                                                //Empty new CustomObject page
                                                popupPage('/' + CustomObjectInfo.CustomObjectURL + '/e?' + CustomObjectnewfields);
                                                CustomObjectnewfields = "";
                                            } else {
                                                //new CustomObject page only with phone field
                                                popupPage('/' + CustomObjectInfo.CustomObjectURL + '/e');
                                            }
                                            return;
                                        } else if (CustomObjectInfo.NoRecordFound == "searchpage" && CustomObjectInfo.CustomObjectURL) {
                                            //Empty CustomObject search page
                                            popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&sen=' + CustomObjectInfo.CustomObjectURL + '&str=' + searchKey);
                                            return;
                                        }
                                        else if (CustomObjectInfo.NoRecordFound == "createnew" && CustomObjectInfo.CreateRecordFieldData) {
                                            //create new CustomObject record
                                            sforce.interaction.saveLog(CustomObjectInfo.ObjectName, CustomObjectInfo.CreateRecordFieldData, function (response) {
                                                if (response.result) {
                                                    var flag = true;
                                                    var recordid = response.result;
                                                    popupPage(response.result);
                                                    if (enableAlertResponse) {
                                                        alert("CustomObject Create response- " + response.result);
                                                    }
                                                    if (recordid && CustomObjectInfo.ActivityLogData && CustomObjectInfo.CreateLogForNewRecord) {
                                                        sforce.interaction.saveLog("Task", CustomObjectInfo.ActivityLogData + "&whatId=" + recordid, function (response1) {
                                                            if (response1.result) {
                                                                if (enableAlertResponse) {
                                                                    alert("CustomObject Create log response- " + response1.result);
                                                                }
                                                                var activityid = response1.result;
                                                                flag = false;
                                                                $.ajax({
                                                                    url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CustomObjectInfo.ObjectName + "&ActivityId=" + activityid + "&RecordId=" + recordid,
                                                                    dataType: "jsonp",
                                                                    crossDomain: true,
                                                                    contentType: "application/json; charset=utf-8",
                                                                    async: true
                                                                });
                                                            } else {
                                                                if (enableAlertError) {
                                                                    alert("CustomObject Create log Error- " + response1.error);
                                                                }
                                                            }
                                                        });
                                                    }
                                                    if (flag) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CustomObjectInfo.ObjectName + "&RecordId=" + recordid,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });

                                                    }

                                                } else {
                                                    if (enableAlertError) {
                                                        alert("CustomObject Create Error- " + response.error);
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
                                            if (jsonCount == 1 && CustomObjectInfo.ActivityLogData) {
                                                sforce.interaction.saveLog('Task', CustomObjectInfo.ActivityLogData + '&whatId=' + item.Id, function (response) {
                                                    if (response.result) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CustomObjectInfo.ObjectName + "&ActivityId=" + response.result,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });
                                                        if (enableAlertResponse) {
                                                            alert("CustomObject save log call back response-" + response.result);
                                                        }
                                                    } else {
                                                        if (enableAlertError) {
                                                            alert("CustomObject save log call back error-" + response.error);
                                                        }
                                                    }
                                                });
                                            } else if (jsonCount > 1 && CustomObjectInfo.MultipleMatchRecord) {
                                                if (CustomObjectInfo.MultipleMatchRecord == "searchpage" && CustomObjectInfo.SearchpageMode) {
                                                    if (CustomObjectInfo.SearchpageMode == 'all') {
                                                        // All page search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&str=' + searchKey);
                                                    } else {
                                                        //CustomObject search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&' + CustomObjectInfo.SearchpageMode + '&str=' + searchKey);
                                                    }
                                                    return false; //exit loop   
                                                } else if (CustomObjectInfo.MultipleMatchRecord == "openall" && CustomObjectInfo.MaxRecordOpenCount) {
                                                    multirecordIDs.push(item.Id);
                                                }
                                            }
                                        }
                                    }
                                    );

                                    if (multirecordIDs.length > 0 && CustomObjectInfo.MaxRecordOpenCount) {
                                        var count = Math.min(multirecordIDs.length, CustomObjectInfo.MaxRecordOpenCount);
                                        for (var i = 0; i < count; i++) {
                                            popupPage(multirecordIDs[i]);
                                        }
                                    }
                                }
                            }
                        } catch (e) {
                            if (enableAlertError) {
                                alert("CustomObject SFDCSearchObject error " + e.description);
                            }
                        }
                    });
                } else {

                    try {
                        if (CustomObjectInfo.NoRecordFound == "createnew" && CustomObjectInfo.CreateRecordFieldData) {
                            //create new CustomObject record
                            sforce.interaction.saveLog(CustomObjectInfo.ObjectName, CustomObjectInfo.CreateRecordFieldData, function (response) {
                                if (response.result) {
                                    var flag = true;
                                    var recordid = response.result;
                                    popupPage(response.result);
                                    if (enableAlertResponse) {
                                        alert("CustomObject Create response- " + response.result);
                                    }
                                    if (recordid && CustomObjectInfo.ActivityLogData && CustomObjectInfo.CreateLogForNewRecord) {
                                        sforce.interaction.saveLog("Task", CustomObjectInfo.ActivityLogData + "&whatId=" + recordid, function (response1) {
                                            if (response1.result) {
                                                if (enableAlertResponse) {
                                                    alert("CustomObject Create log response- " + response1.result);
                                                }
                                                var activityid = response1.result;
                                                flag = false;
                                                $.ajax({
                                                    url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CustomObjectInfo.ObjectName + "&ActivityId=" + activityid + "&RecordId=" + recordid,
                                                    dataType: "jsonp",
                                                    crossDomain: true,
                                                    contentType: "application/json; charset=utf-8",
                                                    async: true
                                                });
                                            } else {
                                                if (enableAlertError) {
                                                    alert("CustomObject Create log Error- " + response1.error);
                                                }
                                            }
                                        });
                                    }
                                    if (flag) {
                                        $.ajax({
                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + CustomObjectInfo.ObjectName + "&RecordId=" + recordid,
                                            dataType: "jsonp",
                                            crossDomain: true,
                                            contentType: "application/json; charset=utf-8",
                                            async: true
                                        });

                                    }

                                } else {
                                    if (enableAlertError) {
                                        alert("CustomObject Create Error- " + response.error);
                                    }
                                }
                            });
                        }
                    } catch (e) {
                        if (enableAlertError) {
                            alert("CustomObject Empty Create new error " + e.description);
                        }

                    }
                    return;
                }
            }
        }
    }
    catch (e) {
        if (enableAlertError) {
            alert("CustomObjectProcess error " + e.description);
        }
    }

}
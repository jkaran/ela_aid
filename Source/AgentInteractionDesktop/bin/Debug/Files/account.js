
/**************************************************************
 * Script name : Account
 * Created date : SEP-04-2015
 * Created by : Pointel, Inc.
 * Purpose : JavaScript source code for salesforce Account Object
 *************************************************************/

function AccountProcess(AccountInfo, InteractionId) {
   try {
        if (AccountInfo && !$.isEmptyObject(AccountInfo) &&InteractionId&& AccountInfo.ObjectName) {
            var Accountkeyvalue = "";
            var searchKey = "";
                    var Accountnewfields = "&";
            if (AccountInfo.SearchData) {
                if (AccountInfo.SearchCondition) {
                    var removedcap = AccountInfo.SearchData.replace(/\^\,/g, "");
                    removedcap = removedcap.replace(/\^/g, "");
                    searchKey = removedcap.replace(/\,/g, " " + AccountInfo.SearchCondition + " ");
                }
                if (AccountInfo.NewRecordFieldIds) {
                    Accountkeyvalue = AccountInfo.SearchData.split(",");
                    var AccountNewRecordFieldIds = AccountInfo.NewRecordFieldIds.split(",");
                    //Forming Account new page prepopulation value
                    $.each(Accountkeyvalue, function (i) {
                        if (Accountkeyvalue[i] != "^" && AccountNewRecordFieldIds[i] != "n/a") {
                            if (AccountNewRecordFieldIds[i] == "acc10") {
                                if (Accountkeyvalue[i].length == 10) {
                                    Accountnewfields += AccountNewRecordFieldIds[i] + "=" + Accountkeyvalue[i] + "&";
                                }
                            } else {
                                Accountnewfields += AccountNewRecordFieldIds[i] + "=" + Accountkeyvalue[i] + "&";
                            }
                        }
                    });
                }
                AccountInfo.SearchData = AccountInfo.SearchData.replace(/\^\,/g, "");
                AccountInfo.SearchData = AccountInfo.SearchData.replace(/\^/g, "");
            }
            if (AccountInfo.UpdateRecordFields && AccountInfo.UpdateRecordFieldsData && AccountInfo.RecordID) {
                sforce.interaction.saveLog(AccountInfo.ObjectName, AccountInfo.UpdateRecordFieldsData + "&Id=" + AccountInfo.RecordID, function (response) {
                    if (response.result) {
                        if (enableAlertResponse) {
                            alert("Account Update response- " + response.result);
                        }
                            
                    } else {
                        if (enableAlertError) {
                            alert("Account Update Error- " + response.error);
                        }
                    }
                });
            }
            if (AccountInfo.UpdateActivityLog) {
                if (AccountInfo.ActivityRecordID && AccountInfo.UpdateActivityLogData) {
                    sforce.interaction.saveLog('Task', AccountInfo.UpdateActivityLogData + "&Id=" + AccountInfo.ActivityRecordID, function (response) {
                        if (response.result) {
                            if (enableAlertResponse) {
                                alert("Update Account activity history call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("Update Account activity history call back error-" + response.error);
                            }
                        }
                    });
                }
            } else {
                //Click to dail log handling
                    if (AccountInfo.ClickToDialRecordId && AccountInfo.ActivityLogData) {
                        sforce.interaction.saveLog('Task', AccountInfo.ActivityLogData + "&" + AccountInfo.ClickToDialRecordId + "&CallType=Outbound", function (response) {
                            if (response.result) {
                                $.ajax({
                                    url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + AccountInfo.ObjectName + "&ActivityId=" + response.result,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });
                                if (enableAlertResponse) {
                                    alert("Account CTD call back response-" + response.result);
                                }
                            } else {
                                if (enableAlertError) {
                                    alert("Account call back error-" + response.error);
                                }
                            }
                        });
                    } else if (AccountInfo.SearchData && AccountInfo.SearchData != "^") {
                        sforce.interaction.runApex('SFDCSearchObject', 'Search', 'SFsearchkey=' + AccountInfo.SearchData + '&SearchCondition=' + AccountInfo.SearchCondition + '&format=' + AccountInfo.PhoneNumberSearchFormat + '&Pagename=' + AccountInfo.ObjectName + '', function (response) {
                            try {
                                if (response.result) {
                                    var record1 = $.parseJSON(response.result);
                                    var record = 0;
                                    record = record1[0];
                                    var firstRecord = record[0];
                                    var tempsk = AccountInfo.SearchData.split(',');
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
                                            url: Url + "/NoRecordFound?InteractionId=" + InteractionId + "&SearchData=" + AccountInfo.SearchData + '&SearchCondition=' + AccountInfo.SearchCondition + '&format=' + AccountInfo.PhoneNumberSearchFormat + '&Pagename=' + AccountInfo.ObjectName,
                                            dataType: "jsonp",
                                            crossDomain: true,
                                            contentType: "application/json; charset=utf-8",
                                            async: true
                                        });
                                        if (AccountInfo.NoRecordFound) {
                                            if (AccountInfo.NoRecordFound == "opennew") {
                                                if (AccountInfo.NewRecordFieldIds) {
                                                    //Empty new Account page
                                                    popupPage('/001/e?' + Accountnewfields);
                                                    Accountnewfields = "";
                                                } else {
                                                    //new Account page only with phone field
                                                    if (tempsk[0].length == 10) {
                                                        popupPage('/001/e?acc10=' + tempsk[0]);
                                                    } else {
                                                        popupPage('/001/e');
                                                    }
                                                }
                                                return;
                                            } else if (AccountInfo.NoRecordFound == "searchpage") {
                                                //Empty Account search page
                                                popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&sen=001&str=' + searchKey);
                                                return;
                                            }
                                            else if (AccountInfo.NoRecordFound == "createnew" && AccountInfo.CreateRecordFieldData && AccountInfo.ObjectName) {
                                                //create new Account record
                                                sforce.interaction.saveLog(AccountInfo.ObjectName, AccountInfo.CreateRecordFieldData, function (response) {
                                                    if (response.result) {
                                                        var flag = true;
                                                        var recordid = response.result;
                                                        popupPage(response.result);
                                                        if (enableAlertResponse) {
                                                            alert("Account Create response- " + response.result);
                                                        }
                                                        if (recordid && AccountInfo.ActivityLogData && AccountInfo.CreateLogForNewRecord) {
                                                            sforce.interaction.saveLog("Task", AccountInfo.ActivityLogData + "&whatId=" + recordid, function (response1) {
                                                                if (response1.result) {
                                                                    if (enableAlertResponse) {
                                                                        alert("Account Create log response- " + response1.result);
                                                                    }
                                                                    var activityid = response1.result;
                                                                    flag = false;
                                                                    $.ajax({
                                                                        url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + AccountInfo.ObjectName + "&ActivityId=" + activityid + "&RecordId=" + recordid,
                                                                        dataType: "jsonp",
                                                                        crossDomain: true,
                                                                        contentType: "application/json; charset=utf-8",
                                                                        async: true
                                                                    });
                                                                } else {
                                                                    if (enableAlertError) {
                                                                        alert("Account Create log Error- " + response1.error);
                                                                    }
                                                                }
                                                            });
                                                        }
                                                        if (flag) {
                                                            $.ajax({
                                                                url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + AccountInfo.ObjectName + "&RecordId=" + recordid,
                                                                dataType: "jsonp",
                                                                crossDomain: true,
                                                                contentType: "application/json; charset=utf-8",
                                                                async: true
                                                            });
                                                        }

                                                    } else {
                                                        if (enableAlertError) {
                                                            alert("Account Create Error- " + response.error);
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
                                                if (jsonCount == 1 && AccountInfo.ActivityLogData) {
                                                    sforce.interaction.saveLog('Task', AccountInfo.ActivityLogData + '&whatId=' + item.Id, function (response) {
                                                        if (response.result) {
                                                            $.ajax({
                                                                url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + AccountInfo.ObjectName + "&ActivityId=" + response.result,
                                                                dataType: "jsonp",
                                                                crossDomain: true,
                                                                contentType: "application/json; charset=utf-8",
                                                                async: true
                                                            });
                                                            if (enableAlertResponse) {
                                                                alert("Account save log call back response-" + response.result);
                                                            }
                                                        } else {
                                                            if (enableAlertError) {
                                                                alert("Account save log call back error-" + response.error);
                                                            }
                                                        }
                                                    });
                                                } else if (jsonCount > 1 && AccountInfo.MultipleMatchRecord) {
                                                    if (AccountInfo.MultipleMatchRecord == "searchpage" && AccountInfo.SearchpageMode) {
                                                        if (AccountInfo.SearchpageMode == 'all') {
                                                            // All page search page
                                                            popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&str=' + searchKey);
                                                        } else {
                                                            //Account search page
                                                            popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&' + AccountInfo.SearchpageMode + '&str=' + searchKey);
                                                        }
                                                        return false; //exit loop  
                                                    } else if (AccountInfo.MultipleMatchRecord == "openall" && AccountInfo.MaxRecordOpenCount) {
                                                        multirecordIDs.push(item.Id);
                                                    }
                                                }
                                            }
                                        }
                                        );
                                        if (multirecordIDs.length > 0 && AccountInfo.MaxRecordOpenCount) {
                                            var count = Math.min(multirecordIDs.length, AccountInfo.MaxRecordOpenCount);
                                            for (var i = 0; i < count; i++) {
                                                popupPage(multirecordIDs[i]);
                                            }
                                        }
                                    }
                                }
                            } catch (e) {
                                if (enableAlertError) {
                                    alert("Account SFDCSearchObject error " + e.description);
                                }
                            }
                        });
                    } 
            }
        }
    } catch (e) {
        if (enableAlertError) {
            alert("AccountProcess error " + e.description);
        }
    }
}
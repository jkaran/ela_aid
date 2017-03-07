
/**************************************************************
 * Script name : Contact
 * Created date : SEP-04-2015
 * Created by : Pointel, Inc.
 * Purpose : JavaScript source code for salesforce Contact Object
 *************************************************************/

function ContactProcess(ContactInfo, InteractionId) {
   try {
       if (ContactInfo && !$.isEmptyObject(ContactInfo) && InteractionId && ContactInfo.ObjectName) {
            var contactkeyvalue = "";
            var searchKey = "";
            var Contactnewfields = "&";
            if (ContactInfo.SearchData) {
                if (ContactInfo.SearchCondition) {
                    var removedcap = ContactInfo.SearchData.replace(/\^\,/g, "");
                    removedcap = removedcap.replace(/\^/g, "");
                    searchKey = removedcap.replace(/\,/g, " " + ContactInfo.SearchCondition + " ");
                }
                if (ContactInfo.NewRecordFieldIds) {
                    Contactkeyvalue = ContactInfo.SearchData.split(",");
                    var ContactNewRecordFieldIds = ContactInfo.NewRecordFieldIds.split(",");
                    //Forming Contact new page prepopulation value
                    $.each(Contactkeyvalue, function (i) {
                        if (Contactkeyvalue[i] != "^" && ContactNewRecordFieldIds[i] != "n/a") {
                            if (ContactNewRecordFieldIds[i] == "con13") {
                                if (Contactkeyvalue[i].length == 10) {
                                    Contactnewfields += ContactNewRecordFieldIds[i] + "=" + Contactkeyvalue[i] + "&";
                                }
                            } else {
                                Contactnewfields += ContactNewRecordFieldIds[i] + "=" + Contactkeyvalue[i] + "&";
                            }
                        }
                    });
                }
                ContactInfo.SearchData = ContactInfo.SearchData.replace(/\^\,/g, "");
                ContactInfo.SearchData = ContactInfo.SearchData.replace(/\^/g, "");
            }

            if (ContactInfo.UpdateRecordFields && ContactInfo.UpdateRecordFieldsData && ContactInfo.RecordID) {
                sforce.interaction.saveLog(ContactInfo.ObjectName, ContactInfo.UpdateRecordFieldsData + "&Id=" + ContactInfo.RecordID, function (response) {
                    if (response.result) {
                        if (enableAlertResponse) {
                            alert("Contact Update response- " + response.result);
                        }
                    } else {
                        if (enableAlertError) {
                            alert("Contact Update Error- " + response.error);
                        }
                    }
                });
            }
            if (ContactInfo.UpdateActivityLog) {
                if (ContactInfo.ActivityRecordID && ContactInfo.UpdateActivityLogData) {
                    sforce.interaction.saveLog('Task', ContactInfo.UpdateActivityLogData + "&Id=" + ContactInfo.ActivityRecordID, function (response) {
                        if (response.result) {
                            if (enableAlertResponse) {
                                alert("Update Contact activity history call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("Update Contact activity history call back error-" + response.error);
                            }
                        }
                    });
                }
            } else {
                //Click to dail log handling
                if (ContactInfo.ClickToDialRecordId && ContactInfo.ActivityLogData) {
                    sforce.interaction.saveLog('Task', ContactInfo.ActivityLogData + "&" + ContactInfo.ClickToDialRecordId + "&CallType=Outbound", function (response) {
                        if (response.result) {
                            $.ajax({
                                url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + ContactInfo.ObjectName + "&ActivityId=" + response.result,
                                dataType: "jsonp",
                                crossDomain: true,
                                contentType: "application/json; charset=utf-8",
                                async: true
                            });
                            if (enableAlertResponse) {
                                alert("Contact CTD call back response-" + response.result);
                            }
                        } else {
                            if (enableAlertError) {
                                alert("conatct call back error-" + response.error);
                            }
                        }
                    });
                } else if (ContactInfo.SearchData && ContactInfo.SearchData != "^") {
                    sforce.interaction.runApex('SFDCSearchObject', 'Search', 'SFsearchkey=' + ContactInfo.SearchData + '&SearchCondition=' + ContactInfo.SearchCondition + '&format=' + ContactInfo.PhoneNumberSearchFormat + '&Pagename=' + ContactInfo.ObjectName + '', function (response) {
                        try {
                            if (response.result) {
                                var record1 = $.parseJSON(response.result);
                                var record = 0;
                                record = record1[0];
                                var firstRecord = record[0];
                                var tempsk = ContactInfo.SearchData.split(',');
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
                                        url: Url + "/NoRecordFound?InteractionId=" + InteractionId + "&SearchData=" + ContactInfo.SearchData+ '&SearchCondition=' + ContactInfo.SearchCondition + '&format=' + ContactInfo.PhoneNumberSearchFormat + '&Pagename=' + ContactInfo.ObjectName ,
                                        dataType: "jsonp",
                                        crossDomain: true,
                                        contentType: "application/json; charset=utf-8",
                                        async: true
                                    });    
                                    if (ContactInfo.NoRecordFound) {
                                        if (ContactInfo.NoRecordFound == "opennew") {
                                            if (ContactInfo.NewRecordFieldIds) {
                                                //Empty new Contact page
                                                popupPage('/003/e?' + Contactnewfields);
                                                Contactnewfields = "";
                                            } else {
                                                //new Contact page only with phone field
                                                if (tempsk[0].length == 10) {
                                                    popupPage('/003/e?con13=' + tempsk[0]);
                                                } else {
                                                    popupPage('/003/e');
                                                }
                                            }
                                            return;
                                        } else if (ContactInfo.NoRecordFound == "searchpage") {
                                            //Empty Contact search page
                                            popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&sen=003&str=' + searchKey);
                                            return;
                                        }
                                        else if (ContactInfo.NoRecordFound == "createnew" && ContactInfo.CreateRecordFieldData) {
                                            sforce.interaction.saveLog(ContactInfo.ObjectName, ContactInfo.CreateRecordFieldData, function (response) {
                                                if (response.result) {
                                                    var flag = true;
                                                    var recordid = response.result;
                                                    popupPage(response.result);
                                                    if (enableAlertResponse) {
                                                        alert("Contact Create response- " + response.result);
                                                    }
                                                    if (recordid && ContactInfo.ActivityLogData && ContactInfo.CreateLogForNewRecord) {
                                                        sforce.interaction.saveLog("Task", ContactInfo.ActivityLogData + "&whoId=" + recordid, function (response1) {
                                                            if (response1.result) {
                                                                if (enableAlertResponse) {
                                                                    alert("Contact Create log response- " + response1.result);
                                                                }
                                                                var activityid = response1.result;
                                                                flag = false;
                                                                $.ajax({
                                                                    url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + ContactInfo.ObjectName + "&ActivityId=" + activityid + "&RecordId=" + recordid,
                                                                    dataType: "jsonp",
                                                                    crossDomain: true,
                                                                    contentType: "application/json; charset=utf-8",
                                                                    async: true
                                                                });
                                                            } else {
                                                                if (enableAlertError) {
                                                                    alert("Contact Create log Error- " + response1.error);
                                                                }
                                                            }
                                                        });
                                                    }
                                                    if (flag) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + ContactInfo.ObjectName + "&RecordId=" + recordid,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });

                                                    }

                                                } else {
                                                    if (enableAlertError) {
                                                        alert("Contact Create Error- " + response.error);
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
                                            if (jsonCount == 1 && ContactInfo.ActivityLogData) {
                                                sforce.interaction.saveLog('Task', ContactInfo.ActivityLogData + '&whoId=' + item.Id, function (response) {
                                                    if (response.result) {
                                                        $.ajax({
                                                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + ContactInfo.ObjectName + "&ActivityId=" + response.result,
                                                            dataType: "jsonp",
                                                            crossDomain: true,
                                                            contentType: "application/json; charset=utf-8",
                                                            async: true
                                                        });
                                                        if (enableAlertResponse) {
                                                            alert("Contact save log call back response-" + response.result);
                                                        }
                                                    } else {
                                                        if (enableAlertError) {
                                                            alert("Contact save log call back error-" + response.error);
                                                        }
                                                    }
                                                });
                                            } else if (jsonCount > 1 && ContactInfo.MultipleMatchRecord) {
                                                if (ContactInfo.MultipleMatchRecord == "searchpage" && ContactInfo.SearchpageMode) {
                                                    if (ContactInfo.SearchpageMode == 'all') {
                                                        // All page search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&str=' + searchKey);
                                                    } else {
                                                        //Contact search page
                                                        popupPage('/_ui/search/ui/UnifiedSearchResults?searchType=2&' + ContactInfo.SearchpageMode + '&str=' + searchKey);
                                                    }
                                                    return false; //exit loop  
                                                } else if (ContactInfo.MultipleMatchRecord == "openall" && ContactInfo.MaxRecordOpenCount) {
                                                    multirecordIDs.push(item.Id);
                                                }
                                            }
                                        }
                                    }
                                    );
                                    if (multirecordIDs.length > 0 && ContactInfo.MaxRecordOpenCount) {
                                        var count = Math.min(multirecordIDs.length,ContactInfo.MaxRecordOpenCount);
                                        for (var i = 0; i < count; i++) {
                                            popupPage(multirecordIDs[i]);
                                        }
                                    }
                                }
                            }
                        } catch (e) {
                            if (enableAlertError) {
                                alert("Contact SFDCSearchObject error " + e.description);
                            }
                        }
                    });
                }
            }
        }
    } catch (e) {
        if (enableAlertError) {
            alert("ContactProcess error " + e.description);
        }
    }
}
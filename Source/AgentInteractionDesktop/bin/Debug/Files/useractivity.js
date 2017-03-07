
/**************************************************************
 * Script name : useractivity
 * Created date : SEP-04-2015
 * Created by : Pointel, Inc.
 * Purpose : JavaScript source code for salesforce useractivity.
 *************************************************************/

function UserActivityProcess(UserActivityInfo, InteractionId) {
    try {
        if (UserActivityInfo.UpdateActivityLog) {
            if (UserActivityInfo.RecordID && UserActivityInfo.UpdateActivityLogData) {
                sforce.interaction.saveLog('Task', UserActivityInfo.UpdateActivityLogData + "&Id=" + UserActivityInfo.RecordID, function (response) {
                    if (response.result) {
                        if (enableAlertResponse) {
                            alert("Update User activity history call back response-" + response.result);
                        }
                    } else {
                        if (enableAlertError) {
                            alert("Update User activity history call back error-" + response.error);
                        }
                    }
                });
            }
        }
        if (UserActivityInfo.CreateActvityLog) {
            if (UserActivityInfo.ActivityLogData && UserActivityInfo.ObjectName && InteractionId) {
                sforce.interaction.saveLog('Task', UserActivityInfo.ActivityLogData, function (response) {
                    if (response.result) {
                        $.ajax({
                            url: Url + "/LogData?InteractionId=" + InteractionId + "&ObjectName=" + UserActivityInfo.ObjectName + "&ActivityId=" + response.result,
                            dataType: "jsonp",
                            crossDomain: true,
                            contentType: "application/json; charset=utf-8",
                            async: true
                        });
                        popupPage(response.result);
                        if (enableAlertResponse) {
                            alert("Create User activity history call back response-" + response.result);
                        }
                    } else {
                        if (enableAlertError) {
                            alert("Create User activity history call back error-" + response.error);
                        }
                    }
                });
            }
        }
    } catch (e) {
        if (enableAlertError) {
            alert("User activity error-" + e.description);
        }
    }
}    
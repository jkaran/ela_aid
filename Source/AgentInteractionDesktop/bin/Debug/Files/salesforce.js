    var interaction = document.createElement("script");
        interaction.type = "text/javascript";
        var params = parseQueryString(window.location.search);
        interaction.src = params["sfdcIFrameOrigin"] + "/support/api/27.0/interaction.js";
        document.getElementsByTagName('head')[0].appendChild(interaction);
        var Url = '';// for local host url
        var objId = '';// for click to dial what/who id
        var objType = '';// for click to dial object type
        var lastlogpagename = "";// for saveLogCallback to store pages
        var retryAttempt = 0;// Varfor retry
        if (typeof enableAlertResponse == "undefined") { enableAlertResponse = false; }
        if (typeof enableAlertError == "undefined") { enableAlertError = false; }
        if (typeof enableDataDisplay == "undefined") { enableDataDisplay = false; }

     window.onbeforeunload = function () {
            $.ajax({
                url: Url + "/closed",
                dataType: "jsonp",
                crossDomain: true,
                contentType: "application/json; charset=utf-8",
                async: true
            });
        }

        // TO parse query string
        function parseQueryString(queryString) {
            var params = {};
            if (typeof queryString !== 'string') {
                return params;
            }
            if (queryString.charAt(0) === '?') {
                queryString = queryString.slice(1);
            }
            if (queryString.length === 0) {
                return params;
            }

            var pairs = queryString.split('&');
            for (var i = 0; i < pairs.length; i++) {
                var pair = pairs[i].split('=');
                params[pair[0]] = !!pair[1] ? decodeURIComponent(pair[1]) : null;
            }
            return params;
        }

    

        //Enable click to dial
        function clickToDial() {
            try {
                sforce.interaction.cti.enableClickToDial(function (response) {
                });
            } catch (e) {
               
            }
        }

        //Collect the Salesforce record id of phone numbers that are clicked from SFDC
        function clicktoDialListener() {
            try {
                sforce.interaction.cti.onClickToDial(function (response) {
                    if (response.result) {
                        var item = $.parseJSON(response.result);

                        if (item.object == 'Contact') {

                            objId = 'whoId=' + item.objectId;
                            objType = 'contact';

                        } else if (item.object == 'Case') {

                            objId = 'whatId=' + item.objectId;
                            objType = 'case';
                        } else if (item.object == 'Account') {

                            objId = 'whatId=' + item.objectId;
                            objType = 'account';
                        }
                        else if (item.object == 'Lead') {

                            objId = 'whoId=' + item.objectId;
                            objType = 'lead';
                        }
                        else if (item.object == 'Opportunity') {

                            objId = 'whatId=' + item.objectId;
                            objType = 'opportunity';
                        }
                        else {
                            objId = 'whatId=' + item.objectId;
                            objType = item.object;
                        }

                        var Number = removeSpecialChars(item.number);
                        if (objId != '' && objType != '' && Number != '') {
                            $.ajax({
                                url: Url + "/dial?phoneno=" + Number + "&Id=" + objId + "&Type=" + objType,
                                dataType: "jsonp",
                                crossDomain: true,
                                contentType: "application/json; charset=utf-8",
                                async: true
                            });
                            objId = '';
                            objType = '';
                            Number = '';
                        }
                    } else {
                    }
                });
                return true;
            } catch (e) {
                if (enableAlertError == "True") {
                    alert("error in clicktoDialListener  " + e.message);
                }
            }
        }

        //Popup particular page based on content
        function popupPage(content) {
            sforce.interaction.screenPop(content, true, function (response) {
                if (response.result) {
                    if (enableAlertResponse) {
                        alert("popup page response- " + response.result);
                    }
                } else {
                    if (enableAlertError) {
                        alert("popup page error- " + response.error);
                    }
                }
            });
        }

        //Function to get jsoncount for IE8
        Object.size = function (obj) {
            var size = 0, key;
            for (key in obj) {
                if (obj.hasOwnProperty(key))
                    size++;
            }
            return size;
        };

        //Method to remove special characters.
        function removeSpecialChars(str) {
            try {
                return str.replace(/[^0-9]+/g, '');
            } catch (e) {
                if (enableAlertError) {
                    alert("Remove special characters error- " + e.description);
                }
            }
        }

        //Recursive method that receive Ping and Data from SFDC Adapter
        function poll(url) {

            var xmlhttp;
            if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
                xmlhttp = new XMLHttpRequest();

            }
            else {// code for IE6, IE5
                xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");

            }
            xmlhttp.onreadystatechange = function () {
                ////alert(xmlhttp.status);
                if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {

                    if (enableDataDisplay) {
                        document.getElementById("ev").innerHTML += xmlhttp.responseText;
                    }
                    var AdapterData = $.parseJSON(xmlhttp.responseText);

                    if (AdapterData && AdapterData.Event != "Ping") {
                      
                        if (AdapterData.UserActivityData && AdapterData.UserActivityData != "") {
                            if (typeof UserActivityProcess != "undefined") {
                                UserActivityProcess(AdapterData.UserActivityData, AdapterData.InteractionId);
                            }
                        }
                        if (AdapterData.CommonSearchData && AdapterData.CommonPopupObjects) {
                            if (typeof CommonProcess != "undefined") {
                                CommonProcess(AdapterData);
                            } 
                        } else {
                        if (AdapterData.LeadData && AdapterData.LeadData != "") {
                            if (typeof LeadProcess != "undefined") {
                                LeadProcess(AdapterData.LeadData, AdapterData.InteractionId);
                            }
                        }
                        if (AdapterData.ContactData && AdapterData.ContactData != "") {
                            if (typeof ContactProcess != "undefined") {
                                ContactProcess(AdapterData.ContactData, AdapterData.InteractionId);
                            }
                        }
                        if (AdapterData.AccountData && AdapterData.AccountData != "") {
                            if (typeof AccountProcess != "undefined") {
                                AccountProcess(AdapterData.AccountData, AdapterData.InteractionId);
                            }
                        }
                        if (AdapterData.CaseData && AdapterData.CaseData != "") {
                            if (typeof CaseProcess != "undefined") {
                                CaseProcess(AdapterData.CaseData, AdapterData.InteractionId);
                            }
                        }
                        if (AdapterData.OpportunityData && AdapterData.OpportunityData != "") {
                            if (typeof OpportunityProcess != "undefined") {
                                OpportunityProcess(AdapterData.OpportunityData, AdapterData.InteractionId);
                            }
                        }
                       
                        if (AdapterData.CustomObjectData && AdapterData.CustomObjectData != "") {
                            if (typeof CustomObjectProcess != "undefined") {
                                $.each(AdapterData.CustomObjectData, function (i) {
                                    CustomObjectProcess(AdapterData.CustomObjectData[i], AdapterData.InteractionId);
                                });
                            }
                        }
                    }
                        setTimeout(function () { poll("push?sid=" + Math.random() + "&ack=true") }, 500); //500
                    } else {

                        setTimeout(function () { poll("push?sid=" + Math.random()) }, 500); //500
                    }

                }
                else if (xmlhttp.readyState == 4 && xmlhttp.status >= 500) {
                    //problem in IWS listener internal error
                    if (retryAttempt <= 5) {
                        retryAttempt++;
                        //call ajax function here
                        setTimeout(function () { poll(url) }, 1000);

                    }

                } else if (xmlhttp.readyState == 4 && xmlhttp.status == 0) {
                    //IWS not started
                    if (retryAttempt <= 3) {
                        retryAttempt++;
                        setTimeout(function () { poll(url) }, 1000);
                    } else {
                        //alert("Telephony application not connected");
                    }

                }
            }
            xmlhttp.open("GET", url, true);
            xmlhttp.send();
        }

        function sendTimezoneToAdapter() {
            /// <summary>
            /// Sends the Timezone to adapter.
            /// </summary>
            /// <returns></returns>
            try {
                sforce.interaction.runApex('SessionId', 'GetTimeZone', '', function (response) {
                    if (response.result) {
                        $.ajax({
                            url: Url + "/timezone?timezone=" + response.result,
                            dataType: "jsonp",
                            crossDomain: true,
                            contentType: "application/json; charset=utf-8",
                            async: true
                        });
                    } else {

                    }
                });
            } catch (e) {
                if (enableConsoleError) {
                    console.log("sendTimezoneToAdapter function error- " + e.message);
                    console.log(e);
                }
            }
        }

        //Initial method that starts every thing.
        function init() {
            try {
                var http = location.protocol;
                var slashes = http.concat("//");
                var host = slashes.concat(window.location.hostname);
                Url = host.concat(":" + window.location.port);

                sendTimezoneToAdapter();

                $.ajax({
                    url: Url + "/opened",
                    dataType: "jsonp",
                    crossDomain: true,
                    contentType: "application/json; charset=utf-8",
                    async: true
                });
                setTimeout(function () { poll("push?sid=" + Math.random()) }, 500);

            } catch (e) {
                if (enableAlertError) {
                    alert("init error- " + e.description);
                }
            }
        }
        setTimeout('clickToDial()', 500);//Call click to dial enable method
        setTimeout('clicktoDialListener()', 500);//Call Click to dial listener method
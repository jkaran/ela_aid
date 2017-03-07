/**************************************************************
 * Script name : salesforce
 * Created date : 09-JAN-2016
 * Modified date : 02-March-2016
 * Created by : Pointel, Inc.
 * Purpose : JavaScript source code for salesforce popup
 *************************************************************/
var interaction = document.createElement("script");
interaction.type = "text/javascript";
var params = parseQueryString(window.location.search);
interaction.src = params["sfdcIFrameOrigin"] + "/support/api/27.0/interaction.js";
document.getElementsByTagName('head')[0].appendChild(interaction);
var Url = '';// for local host url
var objId = '';// for click to dial what/who id
var objType = '';// for click to dial object type
var retryAttempt = 0;// Varfor retry
var interactionid = null;
var enablePrompt = 'false';
if (typeof enableconsoleResponse == "undefined") { enableconsoleResponse = false; }
if (typeof enableConsoleError == "undefined") { enableConsoleError = false; }
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
		setTimeout('clickToDial()', 500);
	}
}

//Collect the Salesforce record id of phone numbers that are clicked from SFDC
function clicktoDialListener() {
	try {
		sforce.interaction.cti.onClickToDial(function (response) {
			if (response.result) {
				var item = $.parseJSON(response.result);
				if (item.object == 'Contact') {
					objId = item.objectId;
					objType = 'contact';
				} else if (item.object == 'Case') {
					objId = item.objectId;
					objType = 'case';
				} else if (item.object == 'Account') {
					objId = item.objectId;
					objType = 'account';
				}
				else if (item.object == 'Lead') {
					objId = item.objectId;
					objType = 'lead';
				}
				else if (item.object == 'Opportunity') {
					objId = item.objectId;
					objType = 'opportunity';
				}
				else {
					objId = item.objectId;
					objType = item.object;
				}
				var Number = removeSpecialChars(item.number);
				if (enablePrompt == 'true') {
					Number = prompt("Edit phone number", Number);
				}
				if (Number != null)
				{
					if(Number && !isNaN(Number)) 
					{
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
					else 
					{
						alert("Phone number should be valid");
					}
				}
			}
		});

	} catch (e) {
		if (enableConsoleError) {
			console.log("clicktoDialListener function error-  " + e.message);
			console.log(e);
		}
		SendErrorLogToAdapter("clicktoDialListener function error-  " + e.message);
		setTimeout('clicktoDialListener()', 500);
	}
}

//Popup particular page based on content
function popupPage(content) {
	sforce.interaction.screenPop(content, true, function (response) {
		if (response.result) {
			if (enableConsoleResponse) {
				console.log("popup page response- " + response.result);
			}
		} else {
			if (enableConsoleError) {
				console.log("popup page error- " + response.error);
			}
			SendErrorLogToAdapter("popup page function error- " + response.error);
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
		if (enableConsoleError) {
			console.log("Remove special characters function error- " + e.message);
			console.log(e);
		}
		SendErrorLogToAdapter("removeSpecialChars function error- " + e.message);
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
		if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {

			var AdapterData = $.parseJSON(xmlhttp.responseText);
			if (enableDataDisplay) {
				if (AdapterData.Event != "Ping")
					document.getElementById("ev").innerHTML += xmlhttp.responseText;
				else
					document.getElementById("ev").innerHTML += "..."
			}
			if (AdapterData && AdapterData.Event == "sessionid") {
				sendSessionToAdapter();
			}
			if (AdapterData && AdapterData.Event == "init") {
				enablePrompt = AdapterData.EnablePrompt;
			}
			if (AdapterData && AdapterData.Event == "timezone") {
				sendTimezoneToAdapter();
			}
			if (AdapterData && AdapterData.Event != "Ping") {
				//Process
				ProcessAdapterData(AdapterData);
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
			}
		}
	}
	xmlhttp.open("GET", url, true);
	xmlhttp.send();
}

//Initial method that starts every thing.
function init() {
	/// <summary>
	/// Initializes this instance.
	/// </summary>
	/// <returns></returns>
	try {
		var http = location.protocol;
		var slashes = http.concat("//");
		var host = slashes.concat(window.location.hostname);
		Url = host.concat(":" + window.location.port);
		//If console.log() not found use alert()
		if (typeof console == "undefined") {
			this.console = { log: function (msg) { /*alert(msg);*/ } };
		}
		sendSessionToAdapter();
		sendTimezoneToAdapter();
		$.ajax({
			url: Url + "/opened",
			dataType: "jsonp",
			crossDomain: true,
			contentType: "application/json; charset=utf-8",
			async: true
		});
		setTimeout(function () { poll("push") }, 500);

	} catch (e) {
		if (enableConsoleError) {
			console.log("init function error- " + e.message);
			console.log(e);
		}
		SendErrorLogToAdapter("init function error- " + e.message);
	}
}

function SendErrorLogToAdapter(log) {
	/// <summary>
	/// Sends the error log to adapter.
	/// </summary>
	/// <param name="log">The log.</param>
	/// <returns></returns>
	try {
		$.ajax({
			url: Url + "/scripterror?log=" + log,
			dataType: "jsonp",
			crossDomain: true,
			contentType: "application/json; charset=utf-8",
			async: true
		});
	} catch (e) {
		if (enableConsoleError) {
			console.log("SendErrorLogToAdapter function error- " + e.message);
			console.log(e);
		}
	}
}

function sendSessionToAdapter() {
	/// <summary>
	/// Sends the session to adapter.
	/// </summary>
	/// <returns></returns>
	try {
		sforce.interaction.runApex('ponitel0603194.SessionId', 'GetSessionId', '', function (response) {		
			if (response.result) {
				$.ajax({
					url: Url + "/sessionid?sessionid=" + response.result,
					dataType: "jsonp",
					crossDomain: true,
					contentType: "application/json; charset=utf-8",
					async: true
				});
			} else {
				alert(response.error);

			}
			
		});
	} catch (e) {
			alert("sendSessionToAdapter function error- " + e.message);
		if (enableConsoleError) {
			console.log("sendSessionToAdapter function error- " + e.message);
			console.log(e);
		}
	}
}

function sendTimezoneToAdapter() {
	/// <summary>
	/// Sends the Timezone to adapter.
	/// </summary>
	/// <returns></returns>
	try {
		sforce.interaction.runApex('ponitel0603194.SessionId', 'GetTimeZone', '', function (response) {		
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

function ProcessAdapterData(AdapterData) {
	/// <summary>
	/// Processes the adapter data.
	/// </summary>
	/// <param name="AdapterData">The adapter data.</param>
	/// <returns></returns>
	try {

		if (AdapterData.PopupUrl) {

			if (AdapterData.PopupUrl.indexOf(",") > -1) {
				var popupids = AdapterData.PopupUrl.split(",");
				for (var i = 0; i < popupids.length; i++) {
					if (popupids[i]) {
						popupPage(popupids[i]);
					}
				}
			} else {
				popupPage(AdapterData.PopupUrl);
			}
		}
	} catch (e) {
		if (enableConsoleError) {
			console.log("ProcessAdapterData function error- " + e.message);
			console.log(e);
		}
		SendErrorLogToAdapter("ProcessAdapterData function error- " + e.message);
	}
}

	setTimeout('clickToDial()', 500);//Call click to dial enable method
	setTimeout('clicktoDialListener()', 500);//Call Click to dial listener method


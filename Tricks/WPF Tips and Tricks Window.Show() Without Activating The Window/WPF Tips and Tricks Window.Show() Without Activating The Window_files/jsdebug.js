Type.registerNamespace('JundaWeb.Services');
JundaWeb.Services.Comments=function() {
JundaWeb.Services.Comments.initializeBase(this);
this._timeout = 0;
this._userContext = null;
this._succeeded = null;
this._failed = null;
}
JundaWeb.Services.Comments.prototype={
AddComment:function(EntryID,CommenterName,CommenterEmail,CommenterUrl,CommentBody,saveProfile,succeededCallback, failedCallback, userContext) {
return this._invoke(JundaWeb.Services.Comments.get_path(), 'AddComment',false,{EntryID:EntryID,CommenterName:CommenterName,CommenterEmail:CommenterEmail,CommenterUrl:CommenterUrl,CommentBody:CommentBody,saveProfile:saveProfile},succeededCallback,failedCallback,userContext); },
GetCommentsHTML:function(EntryID,succeededCallback, failedCallback, userContext) {
return this._invoke(JundaWeb.Services.Comments.get_path(), 'GetCommentsHTML',false,{EntryID:EntryID},succeededCallback,failedCallback,userContext); },
DeleteComment:function(CommentID,succeededCallback, failedCallback, userContext) {
return this._invoke(JundaWeb.Services.Comments.get_path(), 'DeleteComment',false,{CommentID:CommentID},succeededCallback,failedCallback,userContext); }}
JundaWeb.Services.Comments.registerClass('JundaWeb.Services.Comments',Sys.Net.WebServiceProxy);
JundaWeb.Services.Comments._staticInstance = new JundaWeb.Services.Comments();
JundaWeb.Services.Comments.set_path = function(value) { 
var e = Function._validateParams(arguments, [{name: 'path', type: String}]); if (e) throw e; JundaWeb.Services.Comments._staticInstance._path = value; }
JundaWeb.Services.Comments.get_path = function() { return JundaWeb.Services.Comments._staticInstance._path; }
JundaWeb.Services.Comments.set_timeout = function(value) { var e = Function._validateParams(arguments, [{name: 'timeout', type: Number}]); if (e) throw e; if (value < 0) { throw Error.argumentOutOfRange('value', value, Sys.Res.invalidTimeout); }
JundaWeb.Services.Comments._staticInstance._timeout = value; }
JundaWeb.Services.Comments.get_timeout = function() { 
return JundaWeb.Services.Comments._staticInstance._timeout; }
JundaWeb.Services.Comments.set_defaultUserContext = function(value) { 
JundaWeb.Services.Comments._staticInstance._userContext = value; }
JundaWeb.Services.Comments.get_defaultUserContext = function() { 
return JundaWeb.Services.Comments._staticInstance._userContext; }
JundaWeb.Services.Comments.set_defaultSucceededCallback = function(value) { 
var e = Function._validateParams(arguments, [{name: 'defaultSucceededCallback', type: Function}]); if (e) throw e; JundaWeb.Services.Comments._staticInstance._succeeded = value; }
JundaWeb.Services.Comments.get_defaultSucceededCallback = function() { 
return JundaWeb.Services.Comments._staticInstance._succeeded; }
JundaWeb.Services.Comments.set_defaultFailedCallback = function(value) { 
var e = Function._validateParams(arguments, [{name: 'defaultFailedCallback', type: Function}]); if (e) throw e; JundaWeb.Services.Comments._staticInstance._failed = value; }
JundaWeb.Services.Comments.get_defaultFailedCallback = function() { 
return JundaWeb.Services.Comments._staticInstance._failed; }
JundaWeb.Services.Comments.set_path("/irhetoric/Services/Comments.asmx");
JundaWeb.Services.Comments.AddComment= function(EntryID,CommenterName,CommenterEmail,CommenterUrl,CommentBody,saveProfile,onSuccess,onFailed,userContext) {JundaWeb.Services.Comments._staticInstance.AddComment(EntryID,CommenterName,CommenterEmail,CommenterUrl,CommentBody,saveProfile,onSuccess,onFailed,userContext); }
JundaWeb.Services.Comments.GetCommentsHTML= function(EntryID,onSuccess,onFailed,userContext) {JundaWeb.Services.Comments._staticInstance.GetCommentsHTML(EntryID,onSuccess,onFailed,userContext); }
JundaWeb.Services.Comments.DeleteComment= function(CommentID,onSuccess,onFailed,userContext) {JundaWeb.Services.Comments._staticInstance.DeleteComment(CommentID,onSuccess,onFailed,userContext); }

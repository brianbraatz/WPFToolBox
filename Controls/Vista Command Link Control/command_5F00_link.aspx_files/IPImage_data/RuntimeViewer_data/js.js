Type.registerNamespace('Microsoft.Springfield.Services');
Microsoft.Springfield.Services.IDEServiceAnonymous=function() {
Microsoft.Springfield.Services.IDEServiceAnonymous.initializeBase(this);
this._timeout = 0;
this._userContext = null;
this._succeeded = null;
this._failed = null;
}
Microsoft.Springfield.Services.IDEServiceAnonymous.prototype={
LoadAnonymousData:function(projectName,owner,filename,succeededCallback, failedCallback, userContext) {
return this._invoke(Microsoft.Springfield.Services.IDEServiceAnonymous.get_path(), 'LoadAnonymousData',false,{projectName:projectName,owner:owner,filename:filename},succeededCallback,failedCallback,userContext); },
AddAnonymousData:function(projectName,owner,filename,data,succeededCallback, failedCallback, userContext) {
return this._invoke(Microsoft.Springfield.Services.IDEServiceAnonymous.get_path(), 'AddAnonymousData',false,{projectName:projectName,owner:owner,filename:filename,data:data},succeededCallback,failedCallback,userContext); },
ClearAnonymousData:function(projectName,owner,filename,succeededCallback, failedCallback, userContext) {
return this._invoke(Microsoft.Springfield.Services.IDEServiceAnonymous.get_path(), 'ClearAnonymousData',false,{projectName:projectName,owner:owner,filename:filename},succeededCallback,failedCallback,userContext); }}
Microsoft.Springfield.Services.IDEServiceAnonymous.registerClass('Microsoft.Springfield.Services.IDEServiceAnonymous',Sys.Net.WebServiceProxy);
Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance = new Microsoft.Springfield.Services.IDEServiceAnonymous();
Microsoft.Springfield.Services.IDEServiceAnonymous.set_path = function(value) { Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._path = value; }
Microsoft.Springfield.Services.IDEServiceAnonymous.get_path = function() { return Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._path; }
Microsoft.Springfield.Services.IDEServiceAnonymous.set_timeout = function(value) { Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._timeout = value; }
Microsoft.Springfield.Services.IDEServiceAnonymous.get_timeout = function() { return Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._timeout; }
Microsoft.Springfield.Services.IDEServiceAnonymous.set_defaultUserContext = function(value) { Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._userContext = value; }
Microsoft.Springfield.Services.IDEServiceAnonymous.get_defaultUserContext = function() { return Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._userContext; }
Microsoft.Springfield.Services.IDEServiceAnonymous.set_defaultSucceededCallback = function(value) { Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._succeeded = value; }
Microsoft.Springfield.Services.IDEServiceAnonymous.get_defaultSucceededCallback = function() { return Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._succeeded; }
Microsoft.Springfield.Services.IDEServiceAnonymous.set_defaultFailedCallback = function(value) { Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._failed = value; }
Microsoft.Springfield.Services.IDEServiceAnonymous.get_defaultFailedCallback = function() { return Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance._failed; }
Microsoft.Springfield.Services.IDEServiceAnonymous.set_path("/PublicServices/IDEServiceAnonymous.asmx");
Microsoft.Springfield.Services.IDEServiceAnonymous.LoadAnonymousData= function(projectName,owner,filename,onSuccess,onFailed,userContext) {Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance.LoadAnonymousData(projectName,owner,filename,onSuccess,onFailed,userContext); }
Microsoft.Springfield.Services.IDEServiceAnonymous.AddAnonymousData= function(projectName,owner,filename,data,onSuccess,onFailed,userContext) {Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance.AddAnonymousData(projectName,owner,filename,data,onSuccess,onFailed,userContext); }
Microsoft.Springfield.Services.IDEServiceAnonymous.ClearAnonymousData= function(projectName,owner,filename,onSuccess,onFailed,userContext) {Microsoft.Springfield.Services.IDEServiceAnonymous._staticInstance.ClearAnonymousData(projectName,owner,filename,onSuccess,onFailed,userContext); }

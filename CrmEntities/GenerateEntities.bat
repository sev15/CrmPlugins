CrmSvcUtil.exe ^
/codeCustomization:"Microsoft.Xrm.Client.CodeGeneration.CodeCustomization, Microsoft.Xrm.Client.CodeGeneration" ^
/out:Entities.cs  /url:'Server Url'/XRMServices/2011/Organization.svc ^
/domain:'Domain' /username:'Username' /password:'Password' ^
/namespace:Sample.Crm.Entities /serviceContextName:XrmServiceContext
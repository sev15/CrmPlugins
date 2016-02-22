The Sample project contains some plugins to ensure some business rules for a test company :

- when a Case record is created and it is associated to an Account then the value of the ResponsibleContact attribute
  of the new Case must be set to the value of the PrimaryContact attribute of the Account.

- when the value of the PrimaryContact attribute of an Account is modified then the value of the ResponsibleContact
  attribute of all Cases associated to this Account must be updated.


The Sample project uses the Early-bound approach - https://msdn.microsoft.com/en-us/library/gg327971(v=crm.7).aspx#Anchor_1.
The VS CrmEntities project contains generated (https://msdn.microsoft.com/en-us/library/gg327844.aspx) crm entities.
You can use its but it would be better to generate the entities adapted for the data model of your CRM. To do that
you can use the GenerateEntities.bat file. But you will need to modify it by specifying the credentials necessery
to conect to your CRM.

The code of this sample is covered by unit tests (see the Plugins.Tests project) and by system tests
(see the Plugins.SysTests project). To make funcion the system tests you have to modify the connectionString tag in
App.config by specifying the credentials necessery to conect to your CRM.

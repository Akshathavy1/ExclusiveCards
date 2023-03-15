MANAGERS LAYER in EXCLUSIVE SOLUTION

The Manager Layer in exclusive should contain the bulk of the business logic. 
Each manager should work on a specific area of code, such as Customers or Payments

They MUST use the repository pattern when dealing with data. 
A well designed manager will use a small group of related repositories (not lots of random ones).

UNDER NO CIRCUMSTANCES SHOULD ANY DATA CONTEXTS BE USED IN A MANAGER
Managers must not use the dbContext/ExclusiveContext directly
THIS IS NOT ADO.NET
If you don't understand repository pattern and how to use it to work with data GO AND LEARN IT! Ask for help. Look at existing manager code and copy the methodology.

Managers are called by the service layer
DO NOT CALL MANAGERS from Controllers or Views.  They should call a service, not managers or data layer.

Managers can have multiple repositories and entity types 

Do not create one manager per database table
Each manager works on a group of tables, 
eg. Customer
    Contactdetails
    CustomerBankDetails
    ASPNETUser









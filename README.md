# ContactApi
###
- Used Technologies
● .Net 7
● PostgreSQL
● EntityFrameworkCore
● Npgsql
● FluentValidation
● Swagger

### Description
- Develop an API Service in c# dotnet 6 which manages contacts.
- The Service should provide a Rest API.
- This API should have following Methods:
● Creating a Contact
● Updating an existing Contact
● Deleting an existing Contact
● Reading one Contact by a given id
● Reading all Contacts.

- To store the Contacts the Service should use a Postgresql Database.

### Requirements
- The Project should run.
- It should provide a Swagger endpoint
- It should use a postgresql database

### Entity/ Business Object
- A Contact has the following properties with given requirement:

| Name                  | Description                                               | Requirement                                                                             |
|-----------------------|-----------------------------------------------------------|-----------------------------------------------------------------------------------------|
| Salutation            | the Salutation of the Contact                             | must not be empty must be longer than 2 characters can be changed                       |
| Firstname             | the First name of the Contact                             | must not be empty must be longer than 2 characters can be changed                       |
| Lastname              | the Last name of the Contact                              | must not be empty must be longer than 2 characters can be changed                       |
| Displayname           | the displayname of the contact                            | if empty its automatically filled with Salutation + FirstName + LastName can be changed |
| Birthdate             | the birthdate of the contact                              | can be empty can be changed                                                             |
| CreationTimestamp     | the Time when the Contact was created                     | can not be changed                                                                      |
| LastChangeTimestamp   | the Time when the Contact was last changed                | can not be set                                                                          |
| NotifyHasBirthdaySoon | describes if the Contacts birthday is in the next 14 days | can not be set                                                                          |
| Email                 | the email address of the contact                          | must not be empty can be changed is unique                                              |
| Phonenumber           | the phonenumber of the contact                            | can be empty can be changed                                                             |

## How to run
1- git clone https://github.com/berkalgl/ContactApi.git

2- cd .\ContactApi\

3- docker-compose up

4- http://localhost:5001/swagger/index.html

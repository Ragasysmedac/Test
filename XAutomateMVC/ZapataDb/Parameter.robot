*** Settings ***
Documentation     test

Suite Setup   Connect To Database   pymysql   ${DBName}  ${DBUser}  ${DBPass}  ${DBHost}  ${DBPort}

Suite Teardown   Disconnect From Database
Resource          ZapataDBMain.robot

*** Variables ***

${QueryName}   SELECT * FROM ${0}.${1}
${Condition}    row count is 0
${0}            zapatademo
${1}            orders


*** Test Cases ***

Testing Count
     row count is 0    SELECT * FROM zapatademo.orders;


table Name Validating

       table must exist  orders

COLOUMN Validating

        row count is 0    SELECT Orders_ID FROM zapatademo.orders;

Primary Validating

        row count is 0    SELECT Orders_ID FROM zapatademo.orders GROUP BY Orders_ID HAVING COUNT(1) > 1;
Check database
        check if exists in database   SELECT Orders_ID FROM zapatademo.orders WHERE OrderId = '200458';


Parameter Check
       row count is 0     ${QueryName}

       ${QueryName} = CALL Method  ${Cal}     add    40  20
      Should Be Equal As Numbers  ${QueryName}  60



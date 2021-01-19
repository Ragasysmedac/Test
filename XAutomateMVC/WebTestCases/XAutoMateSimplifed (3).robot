*** Settings ***
Documentation   This is the basic suite automating UI of XAutoMate Product
Library  SeleniumLibrary
Resource    XAutoMateResource.robot
Force Tags  Product Regression
Default Tags    tag1    tag2
Test Setup    Begin Web Test
Test Teardown    End web test Common


*** Test Cases ***
Navigate to login page of XAutoMate and login
      [Documentation]    This test case will provide credential to Login page and login to Xautomate UI Pressing Login
      [Tags]    Regression

    Go to login page of XAutoMate
    Enter Credientials at Login Screen of XAutomate

*** Test Cases ***
Create new DB connection string in XAutoMate UI
      [Documentation]    This test case will click and provide credientails to new DB connection
      [Tags]    Regression
    Go to login page of XAutoMate
    Enter Credientials at Login Screen of XAutomate
    Add New Connection to the Database

*** Test Cases ***
Execute Test Suite in XAutomateUI
      [Documentation]    This test case will execute a test suite from XAutoMate UI
      [Tags]    Regression
    Go to login page of XAutoMate
    Enter Credientials at Login Screen of XAutomate
    Execute a run of test suite

*** Test Cases ***
Check that report contains Tag from test execution
      [Documentation]    This test will check if robot report will have specified tag from test execution
      [Tags]    Regression
    Go to login page of XAutoMate
    Enter Credientials at Login Screen of XAutomate
    Check Robot Reporting for Tags

*** Test Cases ***
Check that ReportPortal got the data correctly
      [Documentation]    This test will check if reportportal will have specified tag from test execution
      [Tags]    Regression
      Check ReportPortal for Tags

#robot -d results -N "Full Regression" XAutoMateSimplified.robot



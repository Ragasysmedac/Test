*** Settings ***
Resource  skandiatestcaseResource.robot
Suite setup  Create Test Cases From File

# robot -d results tests

*** Variables ***

*** Test Cases ***
Check loaded data from Excel File
    Log  Loading Excel File

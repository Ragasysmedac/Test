*** Settings ***
Library  ExcelLibrary
Library  DatabaseLibrary
Library  skandiacreate_query.py
Library  skandiagenerate_test_cases.py

*** Variables ***
# How
${excel_file} =  test_file.xlsx
${schema} =  STAGING
${table} =  STG_VERA_FORMANSBESTAMDA_FORSAKRINGAR

*** Keywords ***
Create Test Cases From File
    Open Excel Document  ${excel_file}  0
    @{COL} =  Read Excel Column  0
    ${excel_length} =  Get Length  ${COL}
    @{HEADER} =  Read Excel Row  1

    FOR  ${n}  IN RANGE  2  ${excel_length}
        @{ROW} =  Read Excel Row  ${n}
        ${sql} =  Create Query  ${schema}  ${table}  ${HEADER}  ${ROW}
        Add test case  Check that ${ROW}[0] has been loaded  Check If Loaded  ${sql}
    END

    Close Current Excel Document

Check If Loaded
    [Arguments]  ${sql}
    Connect To Database
    Check If Exists In Database  ${sql}
    Disconnect From Database

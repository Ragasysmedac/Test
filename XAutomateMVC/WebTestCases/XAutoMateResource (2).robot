*** Settings ***
Documentation   This is the basic info about the suite
Library  SeleniumLibrary

*** Variables ***
${BROWSER} =    chrome
${START_URL} =    http://52.169.179.74:5000/
${USER_LOGIN} =    Admin
${USER_PWD} =    XAutomate@123
${CONNECTIONS_BUTTON} =  xpath=//*[@id="sidebar-wrapper"]/div/a[1]
${EXECUTION_BUTTON} =    xpath=//*[@id="sidebar-wrapper"]/div/a[4]
${SLEEP} =    2s
${Version} =    TestVersion
${RP_URL} =    http://52.169.179.74:8080/
${RP_USER} =    default
${RP_PWD} =    Xautomate@123
${RP_LOGINBUTTON} =   css=.bigButton__big-button--ivY7j
${RP_TEXT} =    seconds ago

*** Keywords ***
Begin Web Test
    open browser    about:blank    ${BROWSER}
    Maximize Browser Window

Go to login page of XAutoMate
    go to    ${START_URL}
    sleep    ${SLEEP}

Enter Credientials at Login Screen of XAutomate
    input text    id=login    ${USER_LOGIN}
    input text    id=password    ${USER_PWD}
    click button    id=Loginsub
    sleep    ${SLEEP}

Add New Connection to the Database
    Click Element   ${CONNECTIONS_BUTTON}
    sleep    ${SLEEP}

Execute a run of test suite
    Click Element   ${EXECUTION_BUTTON}
    Click Element    //*[@id="Multisuite"]/option[2]
    sleep    ${SLEEP}
    Click Element    css=#selectrule_chosen .default
    Click Element    css=.chosen-results > li:nth-child(2)
    Click Element    css=.search-field:nth-child(1) > .default
    Click Element    css=#TestsuidId_chosen > .chosen-drop li:nth-child(2)
    Click Element    //*[@id="ReleaseId"]/option[2]
    input text       id=tagid    ${Version}
    click button     id=Trigger
    sleep    ${SLEEP}

Check Robot Reporting for Tags
    Click Element   css=.list-group-item:nth-child(6) > h6
    Click Element   css=.dropdown-btn:nth-child(1) > h6
    Wait Until Page Contains   ${Version}
    sleep    ${SLEEP}

Check ReportPortal for Tags
    go to   ${RP_URL}
    Click Element   css=.type-text > .inputOutside__input--1Sg9p
    input text  css=.type-text > .inputOutside__input--1Sg9p   ${RP_USER}
    Click Element    css=.pageBlockContainer__page-block-container--2K6rq
    input text   css=.inputOutside__type-password--2sIQG > .inputOutside__input--1Sg9p   ${RP_PWD}
    Click button   ${RP_LOGINBUTTON}
    sleep    ${SLEEP}
    Click Element    css=.sidebarButton__active--3dvg_ .sidebarButton__btn-title--1086J
    sleep    ${SLEEP}
    Click Element    css=.sidebar__top-block--6oCNs > .sidebar__sidebar-btn--1kGbJ:nth-child(2) svg
    Click Element    css=.sidebarButton__active--3dvg_ path
    Wait Until Page Contains   ${RP_TEXT}
    sleep    ${SLEEP}


End web test Common
    close browser

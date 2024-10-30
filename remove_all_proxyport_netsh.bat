@echo off
for /f "skip=3 tokens=1,2 delims= " %%a in ('netsh interface portproxy show all') do (
    netsh interface portproxy delete v4tov4 listenport=%%b listenaddress=%%a
)
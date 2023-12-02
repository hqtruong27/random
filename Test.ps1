# Get the process ID (PID) of the console app
$appName = "Spending.exe"  # Replace with the actual name of your console app
$serverIP = "192.168.0.112"

$process = Get-WmiObject -Class Win32_Process -Filter "Name='$appName'" -ComputerName $serverIP

if ($process -ne $null) {
    $process | ForEach-Object {
        $processId = $_.ProcessId
        # Stop the process by its ID
        Stop-Process -Id $processId -Force
        Write-Host "Console app with PID $processId has been stopped."
    }
}
else {
    Write-Host "The console app is not currently running on the server."
}
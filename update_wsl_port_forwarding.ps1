# Get the current port forwarding rules
$portForwardingRules = netsh interface portproxy show all

# Skip the first 3 lines and process each rule
$lines = $portForwardingRules -split "`n" | Select-Object -Skip 3

foreach ($line in $lines) {
    # Split the line into tokens using space as a delimiter
    $tokens = $line -split "\s+"

    # Check if the line has enough tokens to access IP address and port
    if ($tokens.Count -ge 2) {
        $listenAddress = $tokens[0]
        $listenPort = $tokens[1]

        # Delete the port forwarding rule
        netsh interface portproxy delete v4tov4 listenport=$listenPort listenaddress=$listenAddress
    }
}

# Get the current WSL IP address and remove any spaces
$wslIP = (wsl hostname -I).Trim().Split(" ")[0]
$windowsIP = "192.168.0.102"

# Define the ports in an array
# 81: nginx reverse proxy manager
# 5432: postgres
# 27017: mongodb
$ports = @(81, 5432, 27017)

# Loop through each port
foreach ($port in $ports) {
    # Remove existing port forwarding if it exists
    # netsh interface portproxy delete v4tov4 listenport=$port listenaddress=$windowsIP

    # Add new port forwarding rule
    netsh interface portproxy add v4tov4 listenport=$port listenaddress=$windowsIP connectport=$port connectaddress=$wslIP
}

# Show all port forwarding rules
netsh interface portproxy show all

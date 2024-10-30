# Statistics

>WSL
>1. update_wsl_port_forwarding.ps1
>2. .wslconfig
 
Map IP wsl if use only docker in wsl and use netsh proxy port forwarding

# Automatically change ip when wsl ip changes
	1. Save the file: Save the file update_wsl_port_forwarding.ps1 to a known location, such as C:\Scripts\update_wsl_port_forwarding.ps1.
	2. Run the Script Automatically: You can use a scheduled task to run this script every time you start WSL. Here's how:
	  - Open Task Scheduler from the Start menu.
	  - Click on Create Basic Task on the right side.
	  - Name it (e.g., "Update WSL Port Forwarding") and click Next.
	  - Choose When I log on as the trigger and click Next.
	  - Choose Start a program and click Next.
	  - In the Program/script box, enter: powershell.exe
	  - In the Add arguments (optional) box, enter: -ExecutionPolicy Bypass -File "C:\Scripts\update_wsl_port_forwarding.ps1"
	3. Complete the wizard.

Now, every time you log in, the script will update the port forwarding rule with the current WSL IP.
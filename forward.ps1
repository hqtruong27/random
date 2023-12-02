# Variables
$solutionPath = "Statistics.sln"    # Path to your .NET Core solution
$targetPathService = "\\pc-truonghq\wwwroot\Statistics\Services"    # Target network path
$targetPathWebApi = "\\pc-truonghq\wwwroot\Statistics\Api"    # Target network path

# Build the solution
#dotnet build $solutionPath -c Release

# Publish the [Spending] project
dotnet publish -c Release .\Spending\Spending.csproj --output "$targetPathService\\Spending"

# Publish the [Discord] project
dotnet publish -c Release .\Discord\Discord.csproj --output "$targetPathService\\Discord"

# Publish the [WebApi] project
dotnet publish -c Release .\WebApi\WebApi.csproj --output $targetPathWebApi

# Copy files to the target path
#Copy-Item -Path ".\Spending\bin\Release\net8.0\publish\*" -Destination $targetPath -Recurse -Force
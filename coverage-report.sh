#!/bin/bash

# Define common variables
TOOL_PATH="$HOME/.dotnet/tools"
SLN_FILE="Ambev.DeveloperEvaluation.sln"

echo "Install tools if not present"
dotnet tool install --global coverlet.console 2>/dev/null || echo "coverlet.console already installed."
dotnet tool install --global dotnet-reportgenerator-globaltool 2>/dev/null || echo "reportgenerator already installed."

# Add global tools to PATH just in case
export PATH="$PATH:$TOOL_PATH"

echo "Clean and build solution"
dotnet restore $SLN_FILE
dotnet build $SLN_FILE --configuration Release --no-restore

echo "Run tests with coverage"
# Using XPlat Code Coverage is often easier, but keeping original intent with coverlet args
dotnet test $SLN_FILE \
    --no-restore \
    --verbosity normal \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=cobertura \
    /p:CoverletOutput=./TestResults/coverage.cobertura.xml \
    /p:Exclude='[Ambev.DeveloperEvaluation.ORM]*%2c[Ambev.DeveloperEvaluation.WebApi]*%2c[Ambev.DeveloperEvaluation.Common]*'

echo "Generate coverage report"
# Try using reportgenerator from PATH or direct path
if command -v reportgenerator &> /dev/null; then
    REPORT_CMD="reportgenerator"
elif [ -f "$TOOL_PATH/reportgenerator" ]; then
    REPORT_CMD="$TOOL_PATH/reportgenerator"
else
    echo "Error: reportgenerator not found in PATH or $TOOL_PATH"
    exit 1
fi

$REPORT_CMD \
    "-reports:tests/**/TestResults/coverage.cobertura.xml" \
    "-targetdir:TestResults/CoverageReport" \
    "-reporttypes:Html"

echo "Removing temporary files"
# be careful with rm -rf, maybe just leave them or clean specific test artifacts
# rm -rf bin obj # This is dangerous in root, better skip or be specific

echo ""
echo "Coverage report generated at TestResults/CoverageReport/index.html"
echo "You can open it with: xdg-open TestResults/CoverageReport/index.html"

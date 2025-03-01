# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /auth_svc

# Copy the .csproj file and restore the dependencies
COPY *.csproj .
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory inside the container
WORKDIR /auth_svc

# Copy the built application from the build image
COPY --from=build /auth_svc/out .

# Expose the port your app will run on (for web apps, usually 80 or 5000)
EXPOSE 5146

# Define the entry point to run the application
ENTRYPOINT ["dotnet", "JwtAuthService.dll"]

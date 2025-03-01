# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /protected_svc

# Copy the ProtectedService .csproj file and restore the dependencies
COPY ProtectedService/*.csproj ./ProtectedService/

# Copy the RabbitMqConsumer .csproj file and restore the dependencies
COPY ../RabbitMqConsumer/*.csproj ./RabbitMqConsumer/

# Restore dependencies for both projects
RUN dotnet restore ./ProtectedService/ProtectedService.csproj
RUN dotnet restore ./RabbitMqConsumer/RabbitMqConsumer.csproj

# Copy the entire source code for ProtectedService
COPY . .

# Copy the entire source code for RabbitMqConsumer
COPY ../RabbitMqConsumer/ ./RabbitMqConsumer/

# Build and publish the ProtectedService project
RUN dotnet publish ./ProtectedService/ProtectedService.csproj -c Release -o /protected_svc/out/protectedservice

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory inside the container
WORKDIR /protected_svc

# Copy the built application for ProtectedService
COPY --from=build /protected_svc/out/protectedservice .

# Expose the port your app will run on (for web apps, usually 80 or 5000)
EXPOSE 5146

# Define the entry point to run the application
ENTRYPOINT ["dotnet", "ProtectedService.dll"]

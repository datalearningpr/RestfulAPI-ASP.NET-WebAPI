# Restful API .NET Web API

A simple Restful API created by .NET Web API.


### Prerequisites

N.A.

### Installing

Build the Visual Studio Solution

## Example Use

> Web API is using OWIN to host and runs as windows service.

### Debug mode

Directly run the solution, with **debug** mode, the service will run with TopShelf, which makes it easy to debug for building windows services.

### Release mode

1. Build the solution with **release** mode.

2. Open CMD in the release folder.

3. Install the windows service with the below command:
```
installutil BlogService.exe  
```
the **installutil** should be inside somewhere like:
> C:\Windows\Microsoft.NET\Framework\v4.0.30319

4. Since the service is set to manual mode, go to the services in control panel, manually start the service.

## License

GPL

## Acknowledgments

The JWT authentication process is inspired by internet open resources.

# Country Blocking API: Geolocation-Based Access Control

[![Project Status](https://img.shields.io/badge/status-Complete%20%2F%20Implemented-brightgreen?style=for-the-badge)](https://github.com/yourusername/mernStackMilestoneProject_ITI)

# Code Documentation (from a business logic Perspective)

**__idea behind__**
Finally implemented a light-weight yet powerful and a scalable also an effeiciant software solution for companies or organizations that need to restrict access to their services based on geographic location or whatever their needs may be.

---

# key features 
- in-memory data storage without database dependencies
- third-part geolocation integration
- backround processing of temporary expired countries without performance overhead
- thread-safe under concurrent operations. (refer to microsoft docs about concurrent dictionaries/hashtables)
- comprehensive validation and testing at every level


## How to Install and Run the Country Blocking API

Follow these steps to install and run the Country Blocking API on your local machine:

### Prerequisites
- **.NET 8.0 SDK** or later installed on your machine
- **Git** (to clone the repository)

### Installation Steps

1. **Clone the repository:**

    ```bash
    git clone https://github.com/ahmedabougabal/dotnet-ip-geolocation-api.git
    ```

2. **Navigate to the project directory:**

    ```bash
    cd dotnet-ip-geolocation-api
    ```

3. **Build the application:**

    ```bash
    dotnet build
    ```

4. **Run the application:**

    ```bash
    dotnet run
    ```


### this should be what you see
![update](https://github.com/user-attachments/assets/2f4050c9-4108-4524-9520-6de744910b34)


### Accessing Swagger UI


Once the application is running, you can access the Swagger UI documentation using the following [link](http://localhost:5059/swagger/index.html).


![image](https://github.com/user-attachments/assets/b5a31154-5eaa-4d16-a27b-57a466e757e3)



This will open the interactive API documentation where you can test all the endpoints directly from your browser.

**Note**: The port number (`5059`) might be different on your machine. Check the console output when you run the application to see the actual URL.


  <img src="https://github.com/Govindv7555/Govindv7555/blob/main/49e76e0596857673c5c80c85b84394c1.gif" width="1000px" height="100px">



# Testing my Implementation  
**this was before refactoring the code after recognizing that the input should be the country code only and an ip address is not required based on the assignment**

<br />


![image](https://github.com/user-attachments/assets/3b82a0bd-9fc9-4b4d-a038-381fd0d7777d)

---
**this is after, works perfectly without any issues**

![Desktop Screenshot 2025 03 01 - 17 47 52 69](https://github.com/user-attachments/assets/0fb60a0a-3977-4ca8-92fb-f200e79df65a)




  <img src="https://github.com/Govindv7555/Govindv7555/blob/main/49e76e0596857673c5c80c85b84394c1.gif" width="1000px" height="100px">


# testing against an invalid country code "XX" as per task that shall return a Bad Request as the country code should be an ISO 3166-1 alpha-2 code. 

![image](https://github.com/user-attachments/assets/165a0846-03e1-4059-a31b-b4dc680e973c)

  <img src="https://github.com/Govindv7555/Govindv7555/blob/main/49e76e0596857673c5c80c85b84394c1.gif" width="1000px" height="100px">

# checking for Temporarily expired countires and flush them out from the memory as required
![image](https://github.com/user-attachments/assets/b79db62e-9188-4ad8-b619-37ca5a430a3d)

---
### An Ip address lookup endpoint that returns all user's data including the timezone, country, location, ISP based on the input IP Address

![Desktop Screenshot 2025 03 01 - 17 40 24 29](https://github.com/user-attachments/assets/dfd9028e-bbe9-42ac-9d71-47afa0b5b878)



  <img src="https://github.com/Govindv7555/Govindv7555/blob/main/49e76e0596857673c5c80c85b84394c1.gif" width="1000px" height="100px">

## these are some concerns and key takeaways related to Networking Fundamentals encountered when developing this app and testing it locally  

### 1. Empty Response from `/api/logs/blocked-attempts`
The `/api/logs/blocked-attempts` endpoint returns an empty response because no blocked attempts are recorded when testing locally, (in short : no actual one from a blocked country tried to access this API/Service) 

Thus :
- No access attempts are made from blocked countries during testing.
- The logs repository remains empty without sample data.

**Solution**: 
I have added a sample data generation for blocked attempts has been added when none exist as a mock, ensuring the endpoint returns meaningful data for testing purposes.

---

### 2. Local Environment IP Detection Limitations
The `/api/ip/check-block` endpoint faces limitations when running locally:

- On `localhost`, `HttpContext.Connection.RemoteIpAddress` returns loopback addresses (`::1` or `127.0.0.1`), which cannot be geolocated.
- This prevents accurate external IP detection.

**Solution**: 
I added a localEnviroment logic that has a fallback IP (Google DNS: `8.8.8.8`) is used for testing, along with the local address, while providing a clear message in the response explaining the limitation.

These solutions I implemented ensure a smooth addressing testing challenges and offering transparency.




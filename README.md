# Country Blocking API: Geolocation-Based Access Control

[![Project Status](https://img.shields.io/badge/status-Complete%20%2F%20Implemented-brightgreen?style=for-the-badge)](https://github.com/yourusername/mernStackMilestoneProject_ITI)

# Code Documentation (the know how)

**__idea behind__**
Finally implemented a light-weight yet powerful and a scalable also an effeiciant software solution for companies or organizations that need to restrict access to their services based on geographic location or whatever their needs may be.

---

# key features 
- in-memory data storage without database dependencies
- third-part geolocation integration
- backround processing of temporary expired countries without performance overhead
- thread-safe under concurrent operations. (refer to microsoft docs about concurrent dictionaries/hashtables)
- comprehensive validation and testing at every level


# Testing my Implementation  
**this was before refactoring the code after recognizing that the input should be the country code only and an ip address is not required based on the assignment**

<br />


![image](https://github.com/user-attachments/assets/3b82a0bd-9fc9-4b4d-a038-381fd0d7777d)

---
**this is after, works perfectly without any issues**


  <img src="https://github.com/Govindv7555/Govindv7555/blob/main/49e76e0596857673c5c80c85b84394c1.gif" width="1000px" height="100px">


# testing against an invalid country code "XX" as per task that shall return a Bad Request as the country code should be an ISO 3166-1 alpha-2 code. 

![image](https://github.com/user-attachments/assets/165a0846-03e1-4059-a31b-b4dc680e973c)

  <img src="https://github.com/Govindv7555/Govindv7555/blob/main/49e76e0596857673c5c80c85b84394c1.gif" width="1000px" height="100px">

# checking for temporally expired countires and flush them out from the memory as per tasked in the assignment 
![image](https://github.com/user-attachments/assets/b79db62e-9188-4ad8-b619-37ca5a430a3d)

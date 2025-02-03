Notes:
	
- Upload endpoint: use below json example in a file to test it.

- Testing: Oonly basic unit tests covering handlers in Application Layer, and some basic unit tests for Domain Layer. 
Solutions would of course need more unit tests for each layer, as well as Integration tests, and Functional Tests whitch should cover bussines use cases etc...

- Implemented Swagger and containerize the application using Docker. There's also a Postgres DB in a Docker container. Both should be started if you run debug in Visual Studio
using Docker Compose

- Content of the json file I used for testing:
  
	{
	  "trialId": "12345",
	  "title": "Clinical Trial for Diabetes",
  	  "startDate": "2024-07-30T18:00:00.000Z",
          "endDate": "2025-07-30T18:00:00.000Z",
	  "participants": 100,
	  "status": "Completed"
	}

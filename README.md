Notes:
	
 - Upload endpoint: I realize that some parts of my implementation should be moved to a separate class, like Json deserialization and schema validation. 
I didn't do that because I run out of time for proper implementation and because it would also require to be covered by a unit test.

- Testing: I implemented only basic unit tests covering handlers in Application Layer, and some basic unittests for Domain Layer. 
Solutions would of course need more unit tests for each layer, as well as Integration tests, and Functional Tests whitc should cover bussines use cases etc...

- I implemented Swagger and containerize the application using Docker. There's also a Postgres DB in a Docker container. Both should be started if you run debug in Visual Studio
using Docker Compose

- Content of the json file I used for testing:
{
  "trialId": "12345",
  "title": "Clinical Trial for Diabetes",
  "startDate": "2024-01-10",
  "endDate": "2025-11-10",
  "participants": 100,
  "status": "Completed"
}

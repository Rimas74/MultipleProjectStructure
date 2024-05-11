# MultipleProjectStructure
This .NET Core solution offers a comprehensive image management system, implementing clean architecture principles across controllers, business logic, and database layers.
## Features
Upload Images: Upload JPEG images via RESTful API.
Generate Thumbnails: Automatically generate thumbnails for uploaded images.
Retrieve Images: Fetch both original and thumbnail images using their unique IDs.
## Unit Tests
Controllers Tests: Verify the correctness of the API endpoints. Tests are located in the ControllersUnitTest project.

Service Tests: Validate business logic operations, including image uploading and thumbnail generation. Tests are located in the BusinessLogicUnitTest project.

Repository Tests: Ensure data storage and retrieval operations are accurate. Tests are located in the DatabaseUnitTest project.

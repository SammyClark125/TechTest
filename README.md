## Project Evaluation
Over the course of this project, I have developed the User Management System to include the following functionality:
* User activity filters
* Added DateOfBirth to the User Model & developed logic/views to support
* Added CRUD actions for Users
* Added Data Logging through a new controller, immutably logging create, edit and delete events for all users, with advanced features including search, filters and pagination on the Index page
* Refactored the application for asynchronous data access (for both Users and Logs)
* Added CI pipelines which run tests and build the application within GitHub Actions
* Added CD pipelines which deploy the working build to Azure App Services dependant on which git branch is pushed to
* Added IaC to support parameterized deployment to new environments and automate provisioning of resources within Azure

* Extended test projects throughout development to validate logic and behaviour, ensuring consistency, protection against regression and facilitating easier development
* Added documentation to Interfaces and code comments in key areas to make code understandable and easy to work with for others

Throughout development, I aimed for long-term maintainability by creating extensible interfaces and services as well as writing clean, well-structured code to support collaboration with other developers in future. I believe the project is now in a strong position for both scalability and future enhancements - particularly through collaborative effort.



## Personal Progress & Evaluation
Tackling this project was a very valuable experience for me, challenging me in several ways and opening my eyes to new information and techniques. Here are some of my personal highlights of the experience:
* I challenged my existing understanding and knowledge as I ran into unexpected problems during core development (such as problems with EntityFramework and data relationships when trying to resolve issues with deletion behaviour)
* I learned new architectural patterns and project structures from the base project provided to me (such as the ServiceCollectionExtension which makes project setup easier and program.cs cleaner and easier to understand)
* I significantly developed my understanding of unit testing, an area which I have struggled in before
* I advanced my understanding of CI/CD, applying more advanced techniques such as using matrices for running test projects in parallel and creating and uploading artifacts as documentation of test results
* I gained hands-on experience with Infrastructure as Code through Bicep - this was new to me and helped me develop a new layer of understanding with cloud automation and real-world deployment strategies
* While I couldn't develop it in time, I researched and learned about background workers and message queues, techniques which were not taught to me at university

This project was a thoroughly enjoyable and rewarding experience, as both an opportunity to put my existing knowledge to the test and as a chance to explore new areas of software development. The challenges I faced during my time with this project have helped me grow in confidence and deepen my technical understanding. I particularly apprediciated the opportunity to get hands-on experience with new deployment techniques through Infrastructure as Code, an enlightening experience which will help me in my future work. Overall, I find myself feeling more capable and prepared for the type of work that I will face in the software development industry. Thank you for the opportunity.





# User Management Technical Exercise

The exercise is an ASP.NET Core web application backed by Entity Framework Core, which faciliates management of some fictional users.
We recommend that you use [Visual Studio (Community Edition)](https://visualstudio.microsoft.com/downloads) or [Visual Studio Code](https://code.visualstudio.com/Download) to run and modify the application.

**The application uses an in-memory database, so changes will not be persisted between executions.**

## The Exercise
Complete as many of the tasks below as you feel comfortable with. These are split into 4 levels of difficulty
* **Standard** - Functionality that is common when working as a web developer
* **Advanced** - Slightly more technical tasks and problem solving
* **Expert** - Tasks with a higher level of problem solving and architecture needed
* **Platform** - Tasks with a focus on infrastructure and scaleability, rather than application development.

### 1. Filters Section (Standard) ✅

The users page contains 3 buttons below the user listing - **Show All**, **Active Only** and **Non Active**. Show All has already been implemented. Implement the remaining buttons using the following logic:
* Active Only – This should show only users where their `IsActive` property is set to `true` ✅
* Non Active – This should show only users where their `IsActive` property is set to `false` ✅

### 2. User Model Properties (Standard) ✅

Add a new property to the `User` class in the system called `DateOfBirth` which is to be used and displayed in relevant sections of the app. ✅

### 3. Actions Section (Standard) ✅

Create the code and UI flows for the following actions
* **Add** – A screen that allows you to create a new user and return to the list ✅
* **View** - A screen that displays the information about a user ✅
* **Edit** – A screen that allows you to edit a selected user from the list ✅
* **Delete** – A screen that allows you to delete a selected user from the list ✅

Each of these screens should contain appropriate data validation, which is communicated to the end user. ✅

### 4. Data Logging (Advanced) ✅

Extend the system to capture log information regarding primary actions performed on each user in the app.
* In the **View** screen there should be a list of all actions that have been performed against that user. ✅
* There should be a new **Logs** page, containing a list of log entries across the application. ✅
* In the Logs page, the user should be able to click into each entry to see more detail about it. ✅
* In the Logs page, think about how you can provide a good user experience - even when there are many log entries. ✅

### 5. Extend the Application (Expert)

Make a significant architectural change that improves the application.
Structurally, the user management application is very simple, and there are many ways it can be made more maintainable, scalable or testable.
Some ideas are:
* Re-implement the UI using a client side framework connecting to an API. Use of Blazor is preferred, but if you are more familiar with other frameworks, feel free to use them.
* Update the data access layer to support asynchronous operations. ✅
* Implement authentication and login based on the users being stored.
* Implement bundling of static assets.
* Update the data access layer to use a real database, and implement database schema migrations.

### 6. Future-Proof the Application (Platform)

Add additional layers to the application that will ensure that it is scaleable with many users or developers. For example:
* Add CI pipelines to run tests and build the application. ✅
* Add CD pipelines to deploy the application to cloud infrastructure. ✅
* Add IaC to support easy deployment to new environments. ✅
* Introduce a message bus and/or worker to handle long-running operations.

## Additional Notes

* Please feel free to change or refactor any code that has been supplied within the solution and think about clean maintainable code and architecture when extending the project.
* If any additional packages, tools or setup are required to run your completed version, please document these thoroughly.

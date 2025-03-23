# Event_Driven_Loan_Evaluation
This is my attempt to learn about event-driven architecture in .NET by building a Loan Evaluation platform

## Getting Started
This is how you set up and run the project locally in your environment.
1. Download and set up Docker [here](https://docs.docker.com/get-started/get-docker/).
2. Clone the repository
```
git clone https://github.com/vinhngogia0906/Event_Driven_Loan_Evaluation.git
```
3. In the repository, run this command to build and spin up the platform's Docker containers:
```
docker-compose up -d
```
4. Wait for the RabbitMq service to finish spinning up (the disk usage will go down drastically), then start the two service applications and finally start the two portal applications. Make sure all the services are running before testing.
![Docker Desktop Screen](image-1.png)
5. Follow the instructions in the README files of the Customer Service Portal and Loan Application Portal applications to start using the Loan Evaluation Platform.
  - [Customer Service Portal](https://github.com/vinhngogia0906/Event_Driven_Loan_Evaluation/tree/main/customer-portal-app)
  - [Loan Application Service Portal](https://github.com/vinhngogia0906/Event_Driven_Loan_Evaluation/tree/main/loan-application-portal) 

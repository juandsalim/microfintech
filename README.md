# Micro-Fintech Core API

An enterprise-grade, event-driven microservices architecture simulating a high-performance financial ledger and real-time fraud detection system. 

Designed to demonstrate advanced backend engineering, strict adherence to **Clean Architecture**, and mathematical rigor in domain modeling.

## Architecture Overview

The system is fully containerized and divided into highly specialized, decoupled services:

1. **FinancialCore.API (RESTful):** The client-facing gateway. Handles incoming transactions, validates DTOs, and orchestrates the workflow.
2. **FraudDetection Engine (gRPC):** An internal, ultra-low-latency service. It applies analytical scoring and statistical deviation bounds to evaluate transaction risk in milliseconds.
3. **Transaction Worker (Background Service):** An asynchronous daemon that subscribes to an event bus to process approved transactions, ensuring the main thread is never blocked during heavy database I/O.



## Tech Stack & Patterns

* **Framework:** .NET 8 (C# 12)
* **Databases:** PostgreSQL (Relational Ledger) + Redis (In-Memory Distributed Cache & Message Broker)
* **Communication:** REST (External) + gRPC / Protobuf (Internal) + Redis Pub/Sub (Asynchronous Events)
* **Quality Assurance:** xUnit + FluentAssertions
* **Patterns Applied:** Clean Architecture, Domain-Driven Design (Rich Domain Model), Repository Pattern, Producer-Consumer (Event-Driven), Dependency Injection.

## Key Engineering Decisions

### 1. Rich Domain Model over Anemic Entities
In financial systems, state mutation must be strictly controlled. The `Account` entity encapsulates all business rules (invariants). You cannot externally modify the balance; you must invoke methods like `Withdraw()`, which logically guarantee that the balance never drops below zero, preventing double-spending anomalies.

### 2. High-Speed Fraud Scoring via gRPC
Traditional HTTP/1.1 REST is too slow for internal microservice communication in high-throughput environments. The Fraud Engine communicates with the Core API via **HTTP/2 and Protocol Buffers (gRPC)**. 
*Note: The scoring logic establishes the foundation for integrating complex statistical models and anomaly detection algorithms, bringing mathematical precision to risk management.*

### Machine Learning Integration (ONNX)
The Fraud Engine is not a simple rules-based system. It integrates a **Random Forest classification model** trained in Python using Scikit-Learn.
* **Mathematical Foundation:** The model is trained on synthetic financial datasets, identifying outliers based on standard deviation bounds and frequency distributions.
* **Real-time Inference:** Exported to the **ONNX** (Open Neural Network Exchange) format, the model is consumed natively within the .NET 8 gRPC service using `Microsoft.ML.OnnxRuntime`. This allows cross-language AI integration with near-zero latency, avoiding the overhead of external HTTP calls to Python APIs.

### 3. Event-Driven Settlement to prevent Bottlenecks
When a transaction is approved, the API does not force the user to wait for database locks. It returns a `202 Accepted` and publishes an event to a **Redis Pub/Sub channel**. A dedicated Worker Service consumes this queue and processes the settlement asynchronously, allowing the system to handle massive traffic spikes gracefully.

### 4. Distributed Caching for Exchange Rates
Querying relational databases for highly volatile data like exchange rates is inefficient. Rates are stored in **Redis** with an absolute expiration time (TTL), ensuring $O(1)$ lookup complexity while preventing stale financial data.

##  How to Run Locally

1. Clone the repository.
2. Spin up the infrastructure using Docker:
   ```bash
   docker-compose up -d

3. Apply Entity Framework Migrations:
    ```Bash
    dotnet ef database update --project src/Infrastructure --startup-project src/Services/FinancialCore.API

4. run the services (requires 3 terminals):
    ```Bash
    # Terminal 1: Fraud Engine
    dotnet run --project src/Services/FraudDetection.gRPC --launch-profile http

    # Terminal 2: Background Worker
    dotnet run --project src/Services/TransactionWorker

    # Terminal 3: Core API
    dotnet run --project src/Services/FinancialCore.API
    
5. Open Swagger at http://localhost:<PORT>/swagger to test the endpoints.


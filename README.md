# FashionShop E-commerce Platform

A modern e-commerce platform built with microservices architecture using .NET Core and Angular.

## Overview

FashionShop is a complete e-commerce system that allows users to shop for fashion clothing. The project is built with a microservices architecture to ensure scalability and maintainability.

### Technologies Used

- **Backend**: 
  - .NET 7.0
  - Entity Framework Core
  - Redis Cache
  - SQL Server
  - gRPC
  
- **Frontend**:
  - Angular 17
  - TailwindCSS
  - TypeScript

- **DevOps & Infrastructure**:
  - Docker
  - Kubernetes
  - Redis
  - Traefik (API Gateway)

## System Architecture

### Microservices

1. **User Service** - User management and authentication
   - Registration/Login
   - User profile management
   - JWT Authentication

2. **Product Service** - Product management
   - Product CRUD operations
   - Category management
   - Product search and filtering

3. **Order Service** - Order management
   - Order processing
   - Payment handling
   - Order history

4. **Inventory Service** - Inventory management
   - Stock control
   - Inventory updates
   - Out-of-stock notifications

5. **Cart Service** - Shopping cart management
   - Add/remove products
   - Price calculations
   - Cart persistence

## Installation & Deployment

### System Requirements

- Docker Desktop
- Kubernetes
- .NET 7.0 SDK
- Node.js 18+
- SQL Server

### Docker Deployment

```bash
# Build Docker images
docker build -t dgiap/fashion-user-service:latest -f FashionShop.UserService/Dockerfile .
docker build -t dgiap/fashion-product-service:latest -f FashionShop.ProductService/Dockerfile .
docker build -t dgiap/fashion-order-service:latest -f FashionShop.OrderService/Dockerfile .
docker build -t dgiap/fashion-inventory-service:latest -f FashionShop.InventoryService/Dockerfile .
docker build -t dgiap/fashion-cart-service:latest -f FashionShop.CartService/Dockerfile .

# Push to Docker Hub
docker push dgiap/fashion-user-service:latest
docker push dgiap/fashion-product-service:latest
docker push dgiap/fashion-order-service:latest
docker push dgiap/fashion-inventory-service:latest
docker push dgiap/fashion-cart-service:latest
```

### Kubernetes Deployment

```bash
# Deploy services
cd k8s
kubectl apply -f redis_product.yaml
kubectl apply -f user-depl.yaml
kubectl apply -f product-depl.yaml
kubectl apply -f order-depl.yaml
kubectl apply -f inventory-depl.yaml
kubectl apply -f cart-depl.yaml

# Verify deployments
kubectl get pods
kubectl get services
```

## Features

### Customer Features
- Account registration/login
- Browse and search products
- Shopping cart management
- Order placement and payment
- Order tracking
- Product reviews

### Admin Features
- User management
- Product and category management
- Order management
- Inventory management
- Reports and analytics

## Roadmap

Architecture and Design Patterns
1.	Implement CQRS Pattern - Separate read and write operations for better scalability and performance
2.	Apply Domain-Driven Design - Your services have a basic separation, but could benefit from stronger domain modeling
3.	Use Mediator Pattern - Consider MediatR to decouple request/response flows in your controllers
Resilience and Reliability
4.	Add Circuit Breaker Pattern - Your gRPC calls between services need protection from cascading failures
5.	Implement Retry Policies - For transient failures in service communication
6.	Add Timeout Handling - Set appropriate timeouts for all inter-service communication
Observability
7.	Replace Console Logging - Switch from Console.WriteLine to a structured logging framework (Serilog/NLog)
8.	Add Distributed Tracing - Implement OpenTelemetry to trace requests across services
9.	Implement Health Checks - Add health monitoring endpoints for services and dependencies
Testing
10.	Add Unit Tests - Cover critical service and business logic
11.	Implement Integration Tests - Test interactions between components
12.	Create API Tests - Test service endpoints independently
Security
13.	Add API Rate Limiting - Protect services from abuse
14.	Implement Security Headers - Add headers like CSP, X-XSS-Protection, etc.
15.	Add API Key Validation - For additional security layers beyond JWT
DevOps
16.	Complete Docker Implementation - Finalize containerization strategy
17.	Add Infrastructure as Code - Use Terraform or Pulumi for infrastructure
18.	Implement CI/CD Pipelines - Automate testing and deployment
API Management
19.	Add API Versioning - Prepare for future API changes
20.	Enhance API Documentation - Add examples and better descriptions to Swagger
Performance
21.	Optimize Database Queries - Add proper indexing and query optimization
22.	Refine Caching Strategy - More strategic use of Redis beyond basic implementation
23.	Add Performance Monitoring - Measure and log performance metrics

## Contributing

Contributions are welcome. Please feel free to submit issues and pull requests.

## License

MIT
# FashionShop E-commerce Platform

A modern e-commerce platform built with microservices architecture using .NET Core and Angular.

## Overview

FashionShop is a complete e-commerce system that allows users to shop for fashion clothing. The project is built with a microservices architecture to ensure scalability and maintainability.

### Technologies Used

- **Backend**: 
  - .NET 8.0
  - Entity Framework Core
  - Redis Cache
  - SQL Server
  - gRPC
  
- **Frontend**:
  - Angular 19
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

## Result UI
![image](https://github.com/user-attachments/assets/09184250-dd07-4599-9c18-4840b51305aa)
![image](https://github.com/user-attachments/assets/ed5410a8-7774-45b8-9f2a-7745c8c13bdb)
![image](https://github.com/user-attachments/assets/0711680b-f34b-4f1e-b01f-919cfb4d6f2d)
![image](https://github.com/user-attachments/assets/6d219bab-d46e-4cea-9548-76b7c2228e92)
![image](https://github.com/user-attachments/assets/581fd12e-b94b-4787-8581-04a3684d72df)

## Microservice Architecture
![image](https://github.com/user-attachments/assets/f3a933e1-ff8e-4322-b46e-6f13cc6e51bb)

## Installation & Deployment

### System Requirements

- Docker Desktop
- Kubernetes
- .NET 8.0 SDK
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

- [x] Build basic microservices architecture
- [x] Implement authentication/authorization
- [x] Integrate Redis cache
- [ ] Add payment gateway
- [ ] Performance optimization
- [ ] Implement CI/CD pipeline

## Contributing

Contributions are welcome. Please feel free to submit issues and pull requests.

## License

MIT

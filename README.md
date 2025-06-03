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
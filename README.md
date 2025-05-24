# FashionShop
# To do
- When User update profile , token will expire
- Role need to update (It don't have RoleService)
- Caching UserService
- Deploy on Kubernetes
- How to store image with product like clothers
- 

# Kubernetes
# Deploy redis product
cd k8s
kubectl apply -f redis_product.yaml
kubectl apply -f user-depl.yaml
kubectl apply -f product-depl.yaml
kubectl apply -f order-depl.yaml
kubectl apply -f inventory-depl.yaml
kubectl apply -f cart-depl.yaml

# kill service on kubernetes
kubectl delete deployment user-depl
kubectl delete deployment product-depl
kubectl delete deployment order-depl
kubectl delete deployment inventory-depl
kubectl delete deployment cart-depl

# Docker build images
 docker build -t dgiap/fashion-user-service:latest -f .\FashionShop.UserService\Dockerfile .
 docker build -t dgiap/fashion-product-service:latest -f .\FashionShop.ProductService\Dockerfile .
 docker build -t dgiap/fashion-order-service:latest -f .\FashionShop.OrderService\Dockerfile .
 docker build -t dgiap/fashion-inventory-service:latest -f .\FashionShop.InventoryService\Dockerfile .
 docker build -t dgiap/fashion-cart-service -f .\FashionShop.CartService\Dockerfile .
# Docker build push to docker hub
docker push dgiap/fashion-cart-service:latest
docker push dgiap/fashion-user-service:latest
docker push dgiap/fashion-order-service:latest
docker push dgiap/fashion-product-service:latest
docker push dgiap/fashion-inventory-service:latest
### 1. Install Ingress Controller
```bash
# Install NGINX Ingress Controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.1/deploy/static/provider/cloud/deploy.yaml

# Verify installation
kubectl get pods -n ingress-nginx
kubectl get services -n ingress-nginx
```

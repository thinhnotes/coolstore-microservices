#!/bin/bash
# Run on kubenetes with minikube
minikube start
minikube addons enable ingress
# install kubectl dashboard 

# link access dashboard
# http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/
# get token ---- kubectl -n kubernetes-dashboard create token admin-user
# tunnel ip to local to access services ---kube ssh -i ~/.minikube/machines/minikube/id_rsa -L 30939:127.0.0.1:30939 docker@192.168.49.2
echo "alias k='kubectl '" >> ~/.bashrc
echo "alias dk='docker '" >> ~/.bashrc
echo "alias dkc='docker-compose '" >> ~/.bashrc
bash
eval $(minikube docker-env)
docker-compose build

sudo echo "192.168.49.2 coolstore.local" | sudo tee -a /etc/hosts
sudo echo "192.168.49.2 id.coolstore.local" | sudo tee -a /etc/hosts

kubectl apply -f deploys/charts/catalog/
kubectl apply -f deploys/charts/mongodb/
kubectl apply -f deploys/charts/spa/
kubectl apply -f deploys/charts/idp/
kubectl apply -f deploys/charts/ingress/
kubectl apply -f deploys/charts/cartdb/
kubectl apply -f deploys/charts/cart/
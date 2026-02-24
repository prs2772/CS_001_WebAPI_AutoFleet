# 🐳 Docker

# 📌 1. Comandos Básicos

## Ver comandos disponibles
docker

## Ver ayuda de un comando
docker [comando] --help

---

# 🚀 2. Contenedores

## Ejecutar contenedores

### Ejecutar imagen (si no existe la descarga)
docker run hello-world

### Ejecutar en segundo plano (detached)
docker run -d --name mi-contenedor nginx

### Ejecutar y eliminar automáticamente al detenerse
docker run --rm nginx

### Exponer puertos
docker run -p 80:80 -d --name servidor nginx
# Formato: [PUERTO_PC]:[PUERTO_CONTENEDOR]

---

## Mantener contenedores vivos

docker run -d --name ubuntu-test ubuntu sleep 100000

docker run -d -it --name ubuntu-interactivo ubuntu

---

## Listar contenedores

### En ejecución
docker ps

### Todos (incluye detenidos)
docker ps -a

---

## Detener y eliminar

### Detener un contenedor
docker stop [id|nombre]

### Eliminar un contenedor detenido
docker rm [id|nombre]

### Detener todos
docker stop $(docker ps -aq)

### Eliminar todos
docker rm $(docker ps -aq)

---

## Ver logs
docker logs [id|nombre]

---

## Ejecutar comandos dentro de un contenedor

### Ejecutar comando simple
docker exec [id] ls

### Ejecutar de forma interactiva
docker exec -it [id] bash

---

# 🏗 3. Imágenes

## Descargar imagen
docker pull nginx:stable-alpine

(Si no se especifica repositorio usa Docker Hub)

---

## Listar imágenes
docker images
docker image ls

---

## Construir imagen

### Construir usando Dockerfile en la ruta actual
docker build .

### Construir con nombre y tag
docker build -t mi-imagen:v1 .

---

## Build con argumentos

docker build --build-arg NOMBRE_ARG=valor -t mi-imagen .

En Dockerfile:
ARG NOMBRE_ARG

---

## Etiquetar imagen
docker tag mi-imagen:v1 mi-imagen:latest

---

# 🧹 4. Limpieza

## Eliminar contenedores detenidos y recursos no usados
docker system prune

## También eliminar imágenes no utilizadas
docker system prune -a

---

# 📦 5. Volúmenes

## Crear volumen
docker volume create mi-volumen

## Listar volúmenes
docker volume ls

## Inspeccionar volumen
docker volume inspect mi-volumen

---

## Montar volumen por ruta local
docker run -v /ruta/pc:/ruta/contenedor -p 80:80 -d nginx

## Montar volumen nombrado
docker run -v mi-volumen:/ruta/contenedor -d nginx

---

# 🌐 6. Redes

## Listar redes
docker network ls

## Crear red
docker network create mi-red

Driver por defecto: bridge

---

# 🔐 7. Variables de entorno

docker run --name cliente -e MENSAJE="Hola amigo" ubuntu env

---

# 📊 8. Monitoreo

docker stats
(Muestra CPU, memoria y red en tiempo real)

---

# 🏭 9. Docker Compose

## Ejecutar servicios
docker compose up

## Ejecutar en segundo plano
docker compose up -d

## Ejecutar reconstruyendo imágenes
docker compose up -d --build

---

## Construir imágenes del compose
docker compose build

---

## Ejecutar archivo específico
docker compose -f docker-compose.dev.yml up -d

---

## Detener y eliminar servicios
docker compose down

---

## Ejecutar comando en servicio
docker compose run app ls

---

# 🚀 10. Docker Hub

## Login
docker login

## Subir imagen
docker push mi-imagen:v1

## Descargar imagen
docker pull mi-imagen:v1

# 🧠 Summary

- docker build → crea una imagen
- docker run → crea un contenedor desde la imagen
- Imagen = plantilla inmutable
- Contenedor = instancia ejecutable
- Volumen = persistencia de datos
- Red bridge = comunicación entre contenedores
- Compose = orquestación multi-servicio

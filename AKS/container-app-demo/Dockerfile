 # Imagen base
FROM node:18-alpine
 
# Directorio de trabajo
WORKDIR /app
 
# Copiar dependencias y proyecto
COPY package*.json ./
RUN npm install
COPY . .
 
# Puerto expuesto
EXPOSE 3000
 
# Comando de inicio
CMD ["node", "src/index.js"]

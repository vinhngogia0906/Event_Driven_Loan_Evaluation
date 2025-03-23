# Stage 1: Build the Angular app
FROM node:23 as build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build -- --configuration production
# Expose port 4200 (default for ng serve)
EXPOSE 4200

# Start the Angular development server
CMD ["npm", "start"]
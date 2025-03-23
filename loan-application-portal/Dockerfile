# Stage 1: Build the Angular app
FROM node:23 as build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build -- --configuration production
# Expose port 3000 (default for npm run start)
EXPOSE 3000

# Start the Angular development server
CMD ["npm", "start"]
# --- Étape de Build ---
FROM node:20-alpine

WORKDIR /app

# Dépendances (Cache-friendly)
COPY package.json package-lock.json ./
RUN npm ci

# Code Source
COPY . .

# Port exposé
EXPOSE 3000

# Lancer Next.js en mode développement par défaut
CMD ["npm", "run", "dev"]

# --- Étape de Build ---
FROM golang:1.22-alpine AS builder

WORKDIR /app

# Dépendances
COPY go.mod go.sum ./
RUN go mod download

# Code Source
COPY . .

# Compilation (Statique pour Alpine)
RUN CGO_ENABLED=0 GOOS=linux go build -o main ./cmd/main.go

# --- Étape Finale ---
FROM alpine:latest

WORKDIR /app
RUN apk --no-cache add ca-certificates tzdata

# Copie du binaire
COPY --from=builder /app/main .

# Port exposé
EXPOSE 4000

CMD ["./main"]

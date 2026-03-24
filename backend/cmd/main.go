package main

import (
	"fmt"
	"net/http"
)

func main() {
	port := ":8080"
	fmt.Printf("🚀 Server started on http://localhost%s\n", port)
	
	// Ici on appellera nos futurs controllers
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintf(w, "Welcome to kaGO Web API!")
	})

	http.ListenAndServe(port, nil)
}
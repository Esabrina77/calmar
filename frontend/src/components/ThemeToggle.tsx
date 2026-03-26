"use client";

import { useEffect, useState } from "react";

export default function ThemeToggle() {
  const [theme, setTheme] = useState("dark");
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    // 1. Lire au montage
    const savedTheme = localStorage.getItem("calmar-theme") || "dark";
    setTheme(savedTheme);
    setMounted(true);
  }, []);

  useEffect(() => {
    if (mounted) {
      // 2. Appliquer et Sauvegarder
      document.documentElement.setAttribute("data-theme", theme);
      localStorage.setItem("calmar-theme", theme);
    }
  }, [theme, mounted]);

  if (!mounted) return <div style={{ width: "80px" }}></div>; // Évite le flash au chargement

  const toggleTheme = () => {
    setTheme(theme === "dark" ? "light" : "dark");
  };

  return (
    <button
      onClick={toggleTheme}
      style={{
        background: "var(--surface-muted)",
        border: "1px solid var(--border)",
        cursor: "pointer",
        fontSize: "1rem",
        padding: "0.4rem 0.8rem",
        borderRadius: "var(--radius-md)",
        color: "var(--text-primary)",
        display: "flex",
        alignItems: "center",
        gap: "0.5rem",
        transition: "all 0.2s ease",
      }}
    >
      {theme === "dark" ? "☀️" : "🌙"}
      <span style={{ fontSize: "0.7rem", fontWeight: 600 }}>
        {theme === "dark" ? "CLAIR" : "SOMBRE"}
      </span>
    </button>
  );
}

"use client";

import Link from "next/link";
import ThemeToggle from "./ThemeToggle";

export default function Navbar() {
  return (
    <nav
      style={{
        borderBottom: "1px solid var(--border)",
        background: "var(--surface-glass)",
        backdropFilter: "blur(10px)",
        position: "sticky",
        top: 0,
        zIndex: 50,
        height: "70px",
        display: "flex",
        alignItems: "center",
      }}
    >
      <div
        className="container"
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          width: "100%",
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
          <div
            style={{
              width: "35px",
              height: "35px",
              borderRadius: "50%",
              background: "var(--primary)",
              display: "flex",
              alignItems: "center",
              justifyItems: "center",
              justifyContent: "center",
            }}
          >
            <span style={{ color: "white", fontWeight: "bold" }}>C</span>
          </div>
          <Link
            href="/"
            style={{
              fontWeight: "bold",
              fontSize: "1.2rem",
              letterSpacing: "-0.02em",
              color: "var(--text-primary)",
              textDecoration: "none",
            }}
          >
            CALMAR<span style={{ color: "var(--primary)" }}>WEB</span>
          </Link>
        </div>

        <div style={{ display: "flex", gap: "2.5rem" }}>
          <Link
            href="/simulator"
            style={{
              fontSize: "0.9rem",
              fontWeight: 500,
              color: "var(--text-primary)",
              textDecoration: "none",
            }}
          >
            Simulateur
          </Link>
          <Link
            href="/buoys"
            style={{
              fontSize: "0.9rem",
              color: "var(--text-muted)",
              textDecoration: "none",
            }}
          >
            Catalogue
          </Link>
          <Link
            href="/docs"
            style={{
              fontSize: "0.9rem",
              color: "var(--text-muted)",
              textDecoration: "none",
            }}
          >
            Aide
          </Link>
        </div>

        <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
          <ThemeToggle />
          <button
            className="btn btn-primary"
            style={{ padding: "0.5rem 1.5rem", fontSize: "0.75rem" }}
          >
            ADMIN
          </button>
        </div>
      </div>
    </nav>
  );
}

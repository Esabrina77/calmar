import type { Metadata } from "next";
import { Montserrat } from "next/font/google";
import "./globals.css";

const montserrat = Montserrat({
  variable: "--font-montserrat",
  subsets: ["latin"],
  weight: ["300", "400", "500", "600", "700"],
});

export const metadata: Metadata = {
  title: "Calmar Web | IALA Mooring Calculation",
  description: "Logiciel de calcul de lignes de mouillage caténaires - Mobilis SA",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="fr"
      className={`${montserrat.variable} h-full antialiased`}
      data-theme="dark"
    >
      <body className="min-h-full flex flex-col">{children}</body>
    </html>
  );
}

"use client";

import { MapContainer, TileLayer, Marker, useMapEvents } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import React from "react";

// Fix pour les icones Leaflet avec Next.js/Webpack
const icon = L.icon({
  iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
  iconSize: [25, 41],
  iconAnchor: [12, 41],
});

interface MapProps {
  latitude: number | "";
  longitude: number | "";
  onPositionChange: (lat: number, lng: number) => void;
}

function LocationMarker({ latitude, longitude, onPositionChange }: MapProps) {
  useMapEvents({
    click(e) {
      onPositionChange(e.latlng.lat, e.latlng.lng);
    },
  });

  if (latitude === "" || longitude === "") return null;

  return <Marker position={[latitude, longitude]} icon={icon} />;
}

export default function MapPicker({ latitude, longitude, onPositionChange }: MapProps) {
  const center: [number, number] = latitude && longitude ? [latitude, longitude] : [45, -1];

  return (
    <MapContainer
      center={center}
      zoom={8}
      style={{ height: "100%", width: "100%" }}
      scrollWheelZoom={true}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      <LocationMarker latitude={latitude} longitude={longitude} onPositionChange={onPositionChange} />
    </MapContainer>
  );
}

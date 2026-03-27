'use client';

import React from 'react';

import { Buoy, BuoyElement } from '../types/models';

interface BuoyRendererProps {
  buoy: Buoy | any;
  width?: number;
  height?: number;
  waterLine?: number;
}

export default function BuoyRenderer({ buoy, width = 300, height = 500, waterLine = 0 }: BuoyRendererProps) {
  if (!buoy) return <div style={{ color: 'var(--text-muted)', fontSize: '0.8rem' }}>Aucun modèle sélectionné</div>;

  const parts: { el: any; yBot: number; yTop: number; type: string }[] = [];
  let currentY = 0;
  let structureTop = 0;

  // 1. Structure
  const offsetFlotteur = (buoy.structure as any)?.OffsetFlotteur || 0;
  
  if (buoy.structure && buoy.structure.Elements) {
    currentY = 0;
    buoy.structure.Elements.forEach((el: BuoyElement) => {
      const yBot = currentY;
      const yTop = currentY + el.H;
      parts.push({ el, yBot, yTop, type: 'structure' });
      currentY = yTop;
    });
    structureTop = currentY; // Le sommet de la structure métallique
  }

  // 2. Flotteur (placé en fonction de l'Offset par rapport à la base de la structure)
  if (buoy.flotteur && buoy.flotteur.Elements) {
    currentY = offsetFlotteur;
    buoy.flotteur.Elements.forEach((el: BuoyElement) => {
      const yBot = currentY;
      const yTop = currentY + el.H;
      parts.push({ el, yBot, yTop, type: 'flotteur' });
      currentY = yTop;
    });
  }

  // 3. Pylônes (empilés au sommet de la structure)
  currentY = structureTop;
  if (buoy.pylone && Array.isArray(buoy.pylone)) {
    buoy.pylone.forEach((p: any) => {
      if (p.Height) {
        const yBot = currentY;
        const yTop = currentY + p.Height;
        parts.push({ 
          el: { H: p.Height, D0: p.WidthLow || 0.2, D1: p.WidthHigh || 0.2 }, 
          yBot, 
          yTop, 
          type: 'pylone' 
        });
        currentY = yTop;
      }
    });
  }

  // 4. Equipements (empilés au sommet du dernier pylône)
  if (buoy.equipement && Array.isArray(buoy.equipement)) {
    buoy.equipement.forEach((e: any) => {
      if (e.Height) {
        const yBot = currentY;
        const yTop = currentY + e.Height;
        parts.push({ 
          el: { H: e.Height, D0: e.WidthLow || 0.1, D1: e.WidthHigh || 0.1 }, 
          yBot, 
          yTop, 
          type: 'equipement' 
        });
        currentY = yTop;
      }
    });
  }

  // Déterminer la hauteur maximale et le diamètre maximal
  let maxY = 0;
  let maxDiameter = 1;
  parts.forEach((p) => {
    maxY = Math.max(maxY, p.yTop);
    maxDiameter = Math.max(maxDiameter, p.el.D0 || 0, p.el.D1 || 0);
  });

  const totalHeight = maxY;
  const padding = 20;
  const scaleY = (height - (padding * 2)) / (totalHeight || 1);
  const scaleX = (width - (padding * 2)) / (maxDiameter || 1);
  const scale = Math.min(scaleX, scaleY);
  const centerX = width / 2;

  // Obtenir la vraie couleur selon le type
  const getColor = (type: string) => {
    switch(type) {
      case 'flotteur': return '#FFEB00'; // Jaune Mobilis
      case 'structure': return '#555555'; // Gris métallique
      case 'pylone': return '#FFEB00';
      case 'equipement': return '#FFEB00';
      default: return '#cccccc';
    }
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '1rem' }}>
      <h4 style={{ margin: 0, fontSize: '1.2rem', fontWeight: 800, color: 'var(--text-primary)' }}>{buoy.name}</h4>
      <svg width={width} height={height} viewBox={`0 0 ${width} ${height}`}>
        <defs>
          <filter id="buoy-shadow" x="-20%" y="-20%" width="140%" height="140%">
            <feGaussianBlur in="SourceAlpha" stdDeviation="1.5" />
            <feOffset dx="1" dy="1" result="offsetblur" />
            <feComponentTransfer>
              <feFuncA type="linear" slope="0.4" />
            </feComponentTransfer>
            <feMerge>
              <feMergeNode />
              <feMergeNode in="SourceGraphic" />
            </feMerge>
          </filter>
        </defs>

        {parts.map((part, index) => {
          // Inversion de Y (Y=0 en bas du SVG, Y=Hauteur en haut)
          const svgY_bot = height - padding - (part.yBot * scale);
          const svgY_top = height - padding - (part.yTop * scale);

          const x0_top = centerX - ((part.el.D1 || 0) * scale / 2);
          const x1_top = centerX + ((part.el.D1 || 0) * scale / 2);
          const x0_bot = centerX - ((part.el.D0 || 0) * scale / 2);
          const x1_bot = centerX + ((part.el.D0 || 0) * scale / 2);

          // Ordre des points d'un trapèze SVG: Haut-Gauche, Haut-Droite, Bas-Droite, Bas-Gauche (par rapport à l'écran)
          // svgY_top est physiquement plus haut sur l'écran (donc un Y plus petit en SVG)
          const points = `${x0_top},${svgY_top} ${x1_top},${svgY_top} ${x1_bot},${svgY_bot} ${x0_bot},${svgY_bot}`;
          
          return (
            <polygon
              key={`${part.type}-${index}`}
              points={points}
              fill={getColor(part.type)}
              stroke="#000000"
              strokeWidth="1.2"
              filter="url(#buoy-shadow)"
            />
          );
        })}
        {/* Ligne pointillée de base (0) */}
        <line x1={padding} y1={height - padding} x2={width - padding} y2={height - padding} stroke="var(--border)" strokeDasharray="4 4" />
      </svg>
    </div>
  );
}

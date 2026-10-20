import { useState, useEffect } from 'react';
import { MapPin, Navigation, Clock, Star, Phone, X } from 'lucide-react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

// Fix default marker icons broken by bundlers
delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
});

const iconoSeleccionado = L.divIcon({
  className: '',
  html: `<div style="background:#16a34a;width:32px;height:32px;border-radius:50% 50% 50% 0;transform:rotate(-45deg);border:3px solid white;box-shadow:0 2px 8px rgba(0,0,0,0.35)"></div>`,
  iconSize: [32, 32],
  iconAnchor: [16, 32],
  popupAnchor: [0, -36],
});

const iconoNormal = L.divIcon({
  className: '',
  html: `<div style="background:#22c55e;width:24px;height:24px;border-radius:50% 50% 50% 0;transform:rotate(-45deg);border:2px solid white;box-shadow:0 1px 5px rgba(0,0,0,0.25)"></div>`,
  iconSize: [24, 24],
  iconAnchor: [12, 24],
  popupAnchor: [0, -28],
});

interface Store {
  id: string;
  nombre: string;
  tipo: string;
  distancia: number;
  direccion: string;
  horario: string;
  rating: number;
  telefono: string;
  coordenadas: { lat: number; lng: number };
  emoji: string;
}

const stores: Store[] = [
  {
    id: '1',
    nombre: 'Supermercado Fresh Market',
    tipo: 'Supermercado',
    distancia: 0.5,
    direccion: 'Av. Corrientes 1234, CABA',
    horario: '8:00 - 22:00',
    rating: 4.5,
    telefono: '+54 11 1234-5678',
    coordenadas: { lat: -34.6037, lng: -58.3816 },
    emoji: '🛒',
  },
  {
    id: '2',
    nombre: 'Verdulería Don José',
    tipo: 'Verdulería',
    distancia: 0.8,
    direccion: 'Av. Santa Fe 567, CABA',
    horario: '7:00 - 20:00',
    rating: 4.8,
    telefono: '+54 11 2345-6789',
    coordenadas: { lat: -34.5965, lng: -58.3940 },
    emoji: '🥦',
  },
  {
    id: '3',
    nombre: 'Carnicería La Preferida',
    tipo: 'Carnicería',
    distancia: 1.2,
    direccion: 'Av. Rivadavia 890, CABA',
    horario: '8:00 - 19:00',
    rating: 4.6,
    telefono: '+54 11 3456-7890',
    coordenadas: { lat: -34.6083, lng: -58.3709 },
    emoji: '🥩',
  },
  {
    id: '4',
    nombre: 'Dietética Natural Life',
    tipo: 'Dietética',
    distancia: 1.5,
    direccion: 'Defensa 321, San Telmo',
    horario: '9:00 - 21:00',
    rating: 4.7,
    telefono: '+54 11 4567-8901',
    coordenadas: { lat: -34.6178, lng: -58.3700 },
    emoji: '🌿',
  },
  {
    id: '5',
    nombre: 'Almacén Oriental',
    tipo: 'Especialidades',
    distancia: 2.0,
    direccion: 'Av. de Mayo 654, CABA',
    horario: '10:00 - 20:00',
    rating: 4.4,
    telefono: '+54 11 5678-9012',
    coordenadas: { lat: -34.6094, lng: -58.3850 },
    emoji: '🍜',
  },
];

// Centra el mapa en la tienda seleccionada
function MapController({ store }: { store: Store | null }) {
  const map = useMap();
  useEffect(() => {
    if (store) {
      map.flyTo([store.coordenadas.lat, store.coordenadas.lng], 16, { duration: 0.8 });
    }
  }, [store, map]);
  return null;
}

export function StoreLocator() {
  const [selectedStore, setSelectedStore] = useState<Store | null>(null);
  const [filter, setFilter] = useState<string | null>(null);

  const filteredStores = filter
    ? stores.filter(store => store.tipo === filter)
    : stores;

  const storeTypes = [...new Set(stores.map(store => store.tipo))];

  const centro = { lat: -34.6067, lng: -58.3800 };

  const seleccionarTienda = (store: Store) => {
    setSelectedStore(prev => prev?.id === store.id ? null : store);
  };

  return (
    <div className="min-h-screen pb-24">
      <div className="bg-gradient-to-r from-green-500 to-emerald-500 text-white p-6">
        <div className="max-w-lg mx-auto">
          <h1 className="text-2xl mb-2">Tiendas Cercanas</h1>
          <p className="text-green-100">Encontrá dónde comprar tus ingredientes</p>
        </div>
      </div>

      <div className="max-w-lg mx-auto px-4">
        {/* Filtros */}
        <div className="bg-white rounded-xl shadow-lg p-4 -mt-4 mb-4">
          <div className="flex items-center gap-2 mb-3">
            <Navigation size={18} className="text-gray-500" />
            <span className="text-sm text-gray-600">Mostrando tiendas cerca de vos</span>
          </div>
          <div className="flex gap-2 flex-wrap">
            <button
              onClick={() => setFilter(null)}
              className={`px-4 py-1.5 rounded-full text-sm transition-all ${
                filter === null ? 'bg-green-500 text-white' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              Todas
            </button>
            {storeTypes.map(type => (
              <button
                key={type}
                onClick={() => setFilter(filter === type ? null : type)}
                className={`px-4 py-1.5 rounded-full text-sm transition-all ${
                  filter === type ? 'bg-green-500 text-white' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                {type}
              </button>
            ))}
          </div>
        </div>

        {/* Mapa */}
        <div className="rounded-xl overflow-hidden shadow-lg mb-4" style={{ height: 300 }}>
          <MapContainer
            center={[centro.lat, centro.lng]}
            zoom={14}
            style={{ height: '100%', width: '100%' }}
            zoomControl={true}
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            <MapController store={selectedStore} />
            {filteredStores.map(store => (
              <Marker
                key={store.id}
                position={[store.coordenadas.lat, store.coordenadas.lng]}
                icon={selectedStore?.id === store.id ? iconoSeleccionado : iconoNormal}
                eventHandlers={{ click: () => seleccionarTienda(store) }}
              >
                <Popup>
                  <div className="text-sm min-w-[140px]">
                    <p className="font-semibold text-gray-800">{store.emoji} {store.nombre}</p>
                    <p className="text-gray-500">{store.direccion}</p>
                    <p className="text-gray-500">⏰ {store.horario}</p>
                    <p className="text-yellow-600">⭐ {store.rating}</p>
                  </div>
                </Popup>
              </Marker>
            ))}
          </MapContainer>
        </div>

        {/* Lista de tiendas */}
        <div className="space-y-3 mb-6">
          {filteredStores.map(store => (
            <div
              key={store.id}
              onClick={() => seleccionarTienda(store)}
              className={`bg-white rounded-xl shadow-md p-4 cursor-pointer transition-all ${
                selectedStore?.id === store.id
                  ? 'ring-2 ring-green-500 shadow-lg'
                  : 'hover:shadow-lg'
              }`}
            >
              <div className="flex items-start justify-between">
                <div className="flex items-start gap-3 flex-1 min-w-0">
                  <span className="text-2xl flex-shrink-0">{store.emoji}</span>
                  <div className="min-w-0">
                    <h3 className="font-medium text-gray-800 truncate">{store.nombre}</h3>
                    <p className="text-sm text-gray-500">{store.tipo}</p>
                  </div>
                </div>
                <div className="flex flex-col items-end gap-1 flex-shrink-0 ml-2">
                  <span className="text-sm font-medium text-green-700 bg-green-100 px-2 py-0.5 rounded-full">
                    {store.distancia} km
                  </span>
                  <div className="flex items-center gap-1">
                    <Star size={13} className="text-yellow-500 fill-yellow-500" />
                    <span className="text-sm text-gray-600">{store.rating}</span>
                  </div>
                </div>
              </div>

              {selectedStore?.id === store.id && (
                <div className="mt-3 pt-3 border-t border-gray-100 space-y-2">
                  <div className="flex items-start gap-2 text-sm text-gray-600">
                    <MapPin size={15} className="mt-0.5 flex-shrink-0 text-green-600" />
                    <span>{store.direccion}</span>
                  </div>
                  <div className="flex items-center gap-2 text-sm text-gray-600">
                    <Clock size={15} className="flex-shrink-0 text-green-600" />
                    <span>{store.horario}</span>
                  </div>
                  <div className="flex items-center gap-2 text-sm text-gray-600">
                    <Phone size={15} className="flex-shrink-0 text-green-600" />
                    <span>{store.telefono}</span>
                  </div>
                  <div className="flex gap-2 pt-2">
                    <a
                      href={`https://www.openstreetmap.org/directions?to=${store.coordenadas.lat},${store.coordenadas.lng}`}
                      target="_blank"
                      rel="noopener noreferrer"
                      onClick={e => e.stopPropagation()}
                      className="flex-1 px-4 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600 transition-colors text-sm text-center"
                    >
                      Cómo llegar
                    </a>
                    <a
                      href={`tel:${store.telefono}`}
                      onClick={e => e.stopPropagation()}
                      className="flex-1 px-4 py-2 border-2 border-green-500 text-green-600 rounded-lg hover:bg-green-50 transition-colors text-sm text-center"
                    >
                      Llamar
                    </a>
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>

        {filteredStores.length === 0 && (
          <div className="text-center py-12">
            <p className="text-gray-500">No se encontraron tiendas de este tipo</p>
            <button
              onClick={() => setFilter(null)}
              className="mt-4 px-6 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600 transition-colors"
            >
              Ver todas las tiendas
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

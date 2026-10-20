import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router';
import { Search, Clock, Users, TrendingUp, Filter, Heart } from 'lucide-react';
import { useUser } from '../context/UserContext';
import { recipes } from '../data/recipes';

export function Home() {
  const navigate = useNavigate();
  const { preferences } = useUser();
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedFilter, setSelectedFilter] = useState<string | null>(null);
  const [favorites, setFavorites] = useState<string[]>([]);

  const filteredRecipes = useMemo(() => {
    let filtered = recipes;

    if (preferences?.alergias && preferences.alergias.length > 0) {
      filtered = filtered.filter(recipe => {
        return !recipe.alergenos.some(alergeno =>
          preferences.alergias.some(alergia =>
            alergeno.toLowerCase().includes(alergia.toLowerCase())
          )
        );
      });
    }

    if (preferences?.restricciones && preferences.restricciones.length > 0) {
      filtered = filtered.filter(recipe => {
        return preferences.restricciones.every(restriccion =>
          recipe.restricciones.some(r =>
            r.toLowerCase().includes(restriccion.toLowerCase())
          )
        ) || recipe.restricciones.length === 0;
      });
    }

    if (searchTerm) {
      filtered = filtered.filter(recipe =>
        recipe.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
        recipe.descripcion.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (selectedFilter) {
      filtered = filtered.filter(recipe =>
        recipe.categorias.includes(selectedFilter.toLowerCase())
      );
    }

    return filtered;
  }, [preferences, searchTerm, selectedFilter]);

  const toggleFavorite = (id: string) => {
    setFavorites(prev =>
      prev.includes(id) ? prev.filter(fav => fav !== id) : [...prev, id]
    );
  };

  const filterOptions = ['Rápido', 'Saludable', 'Vegano', 'Asiática', 'Italiana', 'Mexicana'];

  return (
    <div className="min-h-screen pb-24">
      <div className="bg-gradient-to-r from-orange-500 to-amber-500 text-white p-6 pb-8">
        <div className="max-w-lg mx-auto">
          <h1 className="text-2xl mb-1">Hola, {preferences?.nombre || 'Chef'}! 👋</h1>
          <p className="text-orange-100 mb-6">¿Qué vamos a cocinar hoy?</p>

          <div className="relative">
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Buscar recetas..."
              className="w-full pl-12 pr-4 py-3 rounded-xl text-gray-800 focus:outline-none focus:ring-2 focus:ring-orange-300"
            />
          </div>
        </div>
      </div>

      <div className="max-w-lg mx-auto px-6 -mt-4">
        <div className="bg-white rounded-xl shadow-lg p-4 mb-6">
          <div className="flex items-center gap-2 mb-3">
            <Filter size={18} className="text-gray-600" />
            <span className="font-medium text-gray-700">Filtros</span>
          </div>
          <div className="flex gap-2 flex-wrap">
            {filterOptions.map(filter => (
              <button
                key={filter}
                onClick={() => setSelectedFilter(selectedFilter === filter ? null : filter)}
                className={`px-4 py-2 rounded-full text-sm transition-all ${
                  selectedFilter === filter
                    ? 'bg-orange-500 text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                {filter}
              </button>
            ))}
          </div>
        </div>

        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-xl text-gray-800">
            {selectedFilter ? `Recetas ${selectedFilter}` : 'Recetas para ti'}
          </h2>
          <span className="text-sm text-gray-500">{filteredRecipes.length} recetas</span>
        </div>

        <div className="space-y-4 mb-6">
          {filteredRecipes.map(recipe => (
            <div
              key={recipe.id}
              onClick={() => navigate(`/receta/${recipe.id}`)}
              className="bg-white rounded-xl shadow-md overflow-hidden cursor-pointer hover:shadow-xl transition-shadow"
            >
              <div className="flex">
                <div className="w-28 h-28 bg-gradient-to-br from-orange-200 to-amber-200 flex items-center justify-center text-5xl flex-shrink-0">
                  {recipe.imagen}
                </div>
                <div className="flex-1 p-4">
                  <div className="flex items-start justify-between mb-2">
                    <h3 className="font-medium text-gray-800">{recipe.nombre}</h3>
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        toggleFavorite(recipe.id);
                      }}
                      className="p-1"
                    >
                      <Heart
                        size={20}
                        className={favorites.includes(recipe.id) ? 'fill-red-500 text-red-500' : 'text-gray-400'}
                      />
                    </button>
                  </div>
                  <p className="text-sm text-gray-600 mb-3 line-clamp-2">{recipe.descripcion}</p>
                  <div className="flex items-center gap-4 text-xs text-gray-500">
                    <div className="flex items-center gap-1">
                      <Clock size={14} />
                      <span>{recipe.tiempoPreparacion} min</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <Users size={14} />
                      <span>{recipe.porciones} porciones</span>
                    </div>
                    <div className={`px-2 py-1 rounded-full ${
                      recipe.dificultad === 'fácil' ? 'bg-green-100 text-green-700' :
                      recipe.dificultad === 'media' ? 'bg-yellow-100 text-yellow-700' :
                      'bg-red-100 text-red-700'
                    }`}>
                      {recipe.dificultad}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        {filteredRecipes.length === 0 && (
          <div className="text-center py-12">
            <p className="text-gray-500">No se encontraron recetas con estos criterios</p>
            <button
              onClick={() => {
                setSearchTerm('');
                setSelectedFilter(null);
              }}
              className="mt-4 px-6 py-2 bg-orange-500 text-white rounded-lg hover:bg-orange-600 transition-colors"
            >
              Limpiar filtros
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
